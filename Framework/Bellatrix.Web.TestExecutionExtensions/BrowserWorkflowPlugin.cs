﻿// <copyright file="BrowserWorkflowPlugin.cs" company="Automate The Planet Ltd.">
// Copyright 2020 Automate The Planet Ltd.
// Licensed under the Apache License, Version 2.0 (the "License");
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// <author>Anton Angelov</author>
// <site>https://bellatrix.solutions/</site>
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Bellatrix.TestWorkflowPlugins;
using Bellatrix.Web.Enums;
using Bellatrix.Web.Proxy;
using Bellatrix.Web.Services;
using OpenQA.Selenium;

namespace Bellatrix.Web.TestExecutionExtensions.Browser
{
    public class BrowserWorkflowPlugin : TestWorkflowPlugin
    {
        protected override void PreTestsArrange(object sender, TestWorkflowPluginEventArgs e)
        {
            if (e.TestClassType.GetCustomAttributes().Any(x => x.GetType().Equals(typeof(BrowserAttribute)) || x.GetType().IsSubclassOf(typeof(BrowserAttribute))))
            {
                // Resolve required data for decision making
                var currentBrowserConfiguration = GetCurrentBrowserConfiguration(e.TestMethodMemberInfo, e.TestClassType, e.Container);

                if (currentBrowserConfiguration != null)
                {
                    ResolvePreviousBrowserType(e.Container);

                    // Decide whether the browser needs to be restarted
                    bool shouldRestartBrowser = ShouldRestartBrowser(e.Container);

                    if (shouldRestartBrowser)
                    {
                        RestartBrowser(e.Container);
                    }
                }
                else
                {
                    e.Container.RegisterInstance(false, "_isBrowserStartedDuringPreTestsArrange");
                }
            }

            base.PreTestsArrange(sender, e);
        }

        protected override void PreTestInit(object sender, TestWorkflowPluginEventArgs e)
        {
            bool isBrowserStartedDuringPreTestsArrange = e.Container.Resolve<bool>("_isBrowserStartedDuringPreTestsArrange");
            if (!isBrowserStartedDuringPreTestsArrange)
            {
                // Resolve required data for decision making
                var currentBrowserConfiguration = GetCurrentBrowserConfiguration(e.TestMethodMemberInfo, e.TestClassType, e.Container);
                if (currentBrowserConfiguration != null)
                {
                    ResolvePreviousBrowserType(e.Container);

                    // Decide whether the browser needs to be restarted
                    bool shouldRestartBrowser = ShouldRestartBrowser(e.Container);

                    if (shouldRestartBrowser)
                    {
                        RestartBrowser(e.Container);
                    }
                }
            }

            e.Container.RegisterInstance(false, "_isBrowserStartedDuringPreTestsArrange");
            base.PreTestInit(sender, e);
        }

        protected override void PostTestCleanup(object sender, TestWorkflowPluginEventArgs e)
        {
            var currentBrowserConfiguration = GetCurrentBrowserConfiguration(e.TestMethodMemberInfo, e.TestClassType, e.Container);
            if (currentBrowserConfiguration != null)
            {
                if (currentBrowserConfiguration.ShouldCaptureHttpTraffic)
                {
                    var proxyService = e.Container.Resolve<ProxyService>();
                    if (proxyService != null)
                    {
                        proxyService.RequestsHistory.Clear();
                        proxyService.ResponsesHistory.Clear();
                    }
                }

                bool isParallelRun = ServicesCollection.Main.Resolve<bool>("isParallelRun");

                //// This if is important for Selenoid video recording per test
                if (isParallelRun)
                {
                    ShutdownBrowser(e.Container);
                }
                else if (currentBrowserConfiguration.BrowserBehavior == BrowserBehavior.RestartEveryTime || (currentBrowserConfiguration.BrowserBehavior == BrowserBehavior.RestartOnFail && !e.TestOutcome.Equals(TestOutcome.Passed)))
                {
                    ShutdownBrowser(e.Container);
                }
            }
        }

        private bool ShouldRestartBrowser(IServicesCollection container)
        {
            bool shouldRestartBrowser = false;
            var previousTestExecutionEngine = container.Resolve<TestExecutionEngine>();
            var previousBrowserConfiguration = container.Resolve<BrowserConfiguration>("_previousBrowserConfiguration");
            var currentBrowserConfiguration = container.Resolve<BrowserConfiguration>("_currentBrowserConfiguration");
            if (previousTestExecutionEngine == null ||
                currentBrowserConfiguration.BrowserBehavior == BrowserBehavior.RestartEveryTime ||
                previousBrowserConfiguration.BrowserType == BrowserType.NotSet ||
                !previousTestExecutionEngine.IsBrowserStartedCorrectly)
            {
                shouldRestartBrowser = true;
            }
            else if (!currentBrowserConfiguration.Equals(previousBrowserConfiguration))
            {
                shouldRestartBrowser = true;
            }

            return shouldRestartBrowser;
        }

        private void RestartBrowser(IServicesCollection container)
        {
            var currentBrowserConfiguration = container.Resolve<BrowserConfiguration>("_currentBrowserConfiguration");

            ShutdownBrowser(container);

            // Register the ExecutionEngine that should be used for the current run. Will be used in the next test as PreviousEngineType.
            var testExecutionEngine = new TestExecutionEngine();
            container.RegisterInstance(testExecutionEngine);

            // Register the Browser type that should be used for the current run. Will be used in the next test as PreviousBrowserType.
            container.RegisterInstance(currentBrowserConfiguration);

            // Start the current engine with current browser type.
            testExecutionEngine.StartBrowser(currentBrowserConfiguration, container);
        }

        private void ShutdownBrowser(IServicesCollection container)
        {
            // Disposing existing engine call only dispose if in parallel.
            var previousTestExecutionEngine = container.Resolve<TestExecutionEngine>();
            bool isParallelRun = ServicesCollection.Current.Resolve<bool>("isParallelRun");
            if (isParallelRun)
            {
                previousTestExecutionEngine?.Dispose(container);
            }
            else
            {
                previousTestExecutionEngine?.DisposeAll();
            }

            bool shouldScrollToVisible = ConfigurationService.Instance.GetWebSettings().ShouldScrollToVisibleOnElementFound;
            var browserConfiguration = new BrowserConfiguration(BrowserType.NotSet, false, shouldScrollToVisible);
            container.RegisterInstance(browserConfiguration, "_previousBrowserConfiguration");
            container.UnregisterSingleInstance<TestExecutionEngine>();
        }

        private void ResolvePreviousBrowserType(IServicesCollection container)
        {
            bool shouldScrollToVisible = ConfigurationService.Instance.GetWebSettings().ShouldScrollToVisibleOnElementFound;
            var browserConfiguration = new BrowserConfiguration(BrowserType.NotSet, false, shouldScrollToVisible);
            if (container.IsRegistered<BrowserConfiguration>())
            {
                browserConfiguration = container.Resolve<BrowserConfiguration>();
            }

            container.RegisterInstance(browserConfiguration, "_previousBrowserConfiguration");
        }

        private BrowserConfiguration GetCurrentBrowserConfiguration(MemberInfo memberInfo, Type testClassType, IServicesCollection container)
        {
            var browserAttribute = GetBrowserAttribute(memberInfo, testClassType);
            if (browserAttribute != null)
            {
                BrowserType currentBrowserType = browserAttribute.Browser;

                BrowserBehavior currentBrowserBehavior = browserAttribute.BrowserBehavior;
                bool shouldCaptureHttpTraffic = browserAttribute.ShouldCaptureHttpTraffic;
                Size currentBrowserSize = browserAttribute.Size;
                string classFullName = testClassType.FullName;
                ExecutionType executionType = browserAttribute.ExecutionType;
                bool shouldAutomaticallyScrollToVisible = browserAttribute.ShouldAutomaticallyScrollToVisible;
                var options = (browserAttribute as IDriverOptionsAttribute)?.CreateOptions(memberInfo, testClassType);
                var browserConfiguration = new BrowserConfiguration(executionType, currentBrowserBehavior, currentBrowserType, currentBrowserSize, classFullName, shouldCaptureHttpTraffic, shouldAutomaticallyScrollToVisible, options);
                container.RegisterInstance(browserConfiguration, "_currentBrowserConfiguration");

                return browserConfiguration;
            }
            else
            {
                container.RegisterInstance(default(BrowserConfiguration), "_currentBrowserConfiguration");

                return null;
            }
        }

        private BrowserAttribute GetBrowserAttribute(MemberInfo memberInfo, Type testClassType)
        {
            var currentPlatform = DetermineOS();

            var methodBrowserAttribute = memberInfo?.GetCustomAttributes<BrowserAttribute>(true).FirstOrDefault(x => x.OS.Equals(currentPlatform));
            var classBrowserAttribute = testClassType.GetCustomAttributes<BrowserAttribute>(true).FirstOrDefault(x => x.OS.Equals(currentPlatform));

            int? appMethodsAttributesCount = memberInfo?.GetCustomAttributes<BrowserAttribute>(true).Count();
            int appClassAttributesCount = testClassType.GetCustomAttributes<BrowserAttribute>(true).Count();

            if (methodBrowserAttribute != null && appMethodsAttributesCount == 1)
            {
                methodBrowserAttribute.OS = currentPlatform;
            }

            if (appClassAttributesCount == 1 && classBrowserAttribute != null)
            {
                classBrowserAttribute.OS = currentPlatform;
            }

            if (methodBrowserAttribute != null)
            {
                return methodBrowserAttribute;
            }
            else
            {
                return classBrowserAttribute;
            }
        }

        private OS DetermineOS()
        {
            var result = OS.Windows;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                result = OS.OSX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                result = OS.Linux;
            }

            return result;
        }
    }
}