﻿// <copyright file="CheckboxEventHandlers.cs" company="Automate The Planet Ltd.">
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
using Bellatrix.Mobile.Events;
using Bellatrix.Mobile.IOS;
using OpenQA.Selenium.Appium.iOS;

namespace Bellatrix.Mobile.EventHandlers.IOS
{
    public class CheckboxEventHandlers : ComponentEventHandlers
    {
        public override void SubscribeToAll()
        {
            base.SubscribeToAll();
            CheckBox.Checking += CheckingEventHandler;
            CheckBox.Checked += CheckedEventHandler;
            CheckBox.Unchecking += UncheckingEventHandler;
            CheckBox.Unchecked += UncheckedEventHandler;
        }

        public override void UnsubscribeToAll()
        {
            base.UnsubscribeToAll();
            CheckBox.Checking -= CheckingEventHandler;
            CheckBox.Checked -= CheckedEventHandler;
            CheckBox.Unchecking -= UncheckingEventHandler;
            CheckBox.Unchecked -= UncheckedEventHandler;
        }

        protected virtual void UncheckingEventHandler(object sender, ComponentActionEventArgs<IOSElement> arg)
        {
        }

        protected virtual void UncheckedEventHandler(object sender, ComponentActionEventArgs<IOSElement> arg)
        {
        }

        protected virtual void CheckingEventHandler(object sender, ComponentActionEventArgs<IOSElement> arg)
        {
        }

        protected virtual void CheckedEventHandler(object sender, ComponentActionEventArgs<IOSElement> arg)
        {
        }
    }
}
