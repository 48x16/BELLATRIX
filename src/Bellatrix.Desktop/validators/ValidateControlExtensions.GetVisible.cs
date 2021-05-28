﻿// <copyright file="ValidateControlExtensions.GetVisible.cs" company="Automate The Planet Ltd.">
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
using Bellatrix.Desktop.Contracts;
using Bellatrix.Desktop.Events;

namespace Bellatrix.Desktop
{
    public static partial class ValidateControlExtensions
    {
        public static void ValidateIsVisible<T>(this T control, int? timeout = null, int? sleepInterval = null)
            where T : IComponentVisible, IComponent
        {
            WaitUntil(() => control.IsVisible.Equals(true), "The control should be visible but was NOT.", timeout, sleepInterval);
            ValidatedIsVisibleEvent?.Invoke(control, new ComponentActionEventArgs(control));
        }

        public static void ValidateIsNotVisible<T>(this T control, int? timeout = null, int? sleepInterval = null)
            where T : IComponentVisible, IComponent
        {
            WaitUntil(() => !control.IsVisible.Equals(true), "The control should be NOT visible but was NOT.", timeout, sleepInterval);
            ValidatedIsNotVisibleEvent?.Invoke(control, new ComponentActionEventArgs(control));
        }

        public static event EventHandler<ComponentActionEventArgs> ValidatedIsVisibleEvent;
        public static event EventHandler<ComponentActionEventArgs> ValidatedIsNotVisibleEvent;
    }
}