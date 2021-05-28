﻿// <copyright file="IOSApp.cs" company="Automate The Planet Ltd.">
// Copyright 2021 Automate The Planet Ltd.
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

using Bellatrix.Mobile.Android;
using Bellatrix.Mobile.EventHandlers.IOS;
using Bellatrix.Mobile.Services;
using Bellatrix.Mobile.Services.IOS;
using OpenQA.Selenium.Appium.iOS;

namespace Bellatrix.Mobile
{
    public class IOSApp : App<IOSDriver<IOSElement>, IOSElement>
    {
        public IOSAppService AppService => ServicesCollection.Current.Resolve<IOSAppService>();

        [Obsolete("DeviceService is deprecated use Device property instead.")]

        public IOSDeviceService DeviceService => ServicesCollection.Current.Resolve<IOSDeviceService>();
        public IOSDeviceService Device => ServicesCollection.Current.Resolve<IOSDeviceService>();

        [Obsolete("FileSystemService is deprecated use Files property instead.")]
        public FileSystemService<IOSDriver<IOSElement>, IOSElement> FileSystemService => ServicesCollection.Current.Resolve<FileSystemService<IOSDriver<IOSElement>, IOSElement>>();
        public FileSystemService<IOSDriver<IOSElement>, IOSElement> Files => ServicesCollection.Current.Resolve<FileSystemService<IOSDriver<IOSElement>, IOSElement>>();

        [Obsolete("KeyboardService is deprecated use Keyboard property instead.")]
        public KeyboardService<IOSDriver<IOSElement>, IOSElement> KeyboardService => ServicesCollection.Current.Resolve<KeyboardService<IOSDriver<IOSElement>, IOSElement>>();
        public KeyboardService<IOSDriver<IOSElement>, IOSElement> Keyboard => ServicesCollection.Current.Resolve<KeyboardService<IOSDriver<IOSElement>, IOSElement>>();

        [Obsolete("TouchActionsService is deprecated use TouchActions property instead.")]
        public TouchActionsService<IOSDriver<IOSElement>, IOSElement> TouchActionsService => ServicesCollection.Current.Resolve<TouchActionsService<IOSDriver<IOSElement>, IOSElement>>();
        public TouchActionsService<IOSDriver<IOSElement>, IOSElement> TouchActions => ServicesCollection.Current.Resolve<TouchActionsService<IOSDriver<IOSElement>, IOSElement>>();

        public override void Dispose()
        {
            DisposeDriverService.DisposeAllIOS();
            GC.SuppressFinalize(this);
        }

        public void AddComponentEventHandler<TComponentsEventHandler>()
          where TComponentsEventHandler : ComponentEventHandlers
        {
            var elementEventHandler = (TComponentsEventHandler)Activator.CreateInstance(typeof(TComponentsEventHandler));
            elementEventHandler.SubscribeToAll();
        }

        public void RemoveComponentEventHandler<TComponentsEventHandler>()
            where TComponentsEventHandler : ComponentEventHandlers
        {
            var elementEventHandler = (TComponentsEventHandler)Activator.CreateInstance(typeof(TComponentsEventHandler));
            elementEventHandler.UnsubscribeToAll();
        }
    }
}
