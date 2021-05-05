﻿// <copyright file="AppRegistrationExtensions.cs" company="Automate The Planet Ltd.">
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
using System.IO;
using System.Reflection;
using Bellatrix.Plugins;
using Bellatrix.Plugins.Screenshots.Plugins;
using Bellatrix.Plugins.Video.Plugins;
using Bellatrix.Results.Allure;

namespace Bellatrix
{
    public static class AllurePlugin
    {
        public static void Add()
        {
            if (ConfigurationService.GetSection<AllureReportingSettings>().IsEnabled)
            {
                ServicesCollection.Current.RegisterType<Plugin, AllureWorkflowPlugin>(Guid.NewGuid().ToString());
                ServicesCollection.Current.RegisterType<IScreenshotPlugin, AllureWorkflowPlugin>(Guid.NewGuid().ToString());
                ServicesCollection.Current.RegisterType<IVideoPlugin, AllureWorkflowPlugin>(Guid.NewGuid().ToString());

                Environment.SetEnvironmentVariable("ALLURE_CONFIG", Path.Combine(GetAssemblyDirectory(), "allureConfig.json"));
            }
        }

        private static string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}