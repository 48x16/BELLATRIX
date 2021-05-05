﻿// <copyright file="AppRegistrationExtensions.cs" company="Automate The Planet Ltd.">
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
using System.Collections.Generic;
using System.Text;

using Bellatrix.DynamicTestCases;
using Bellatrix.DynamicTestCases.AzureDevOps;
using Bellatrix.DynamicTestCases.Contracts;
using Bellatrix.Plugins;

namespace Bellatrix
{
    public static class AzureDevOpsDynamicTestCasesPlugin
    {
        private static bool _isAdded = false;

        public static void Add()
        {
            if (!_isAdded)
            {
                ServicesCollection.Current.RegisterInstance(new DynamicTestCasesService());
                ServicesCollection.Current.RegisterType<ITestCaseManagementService, AzureDevOpsTestCaseManagementService>();
                ServicesCollection.Current.RegisterType<Plugin, Bellatrix.DynamicTestCases.Core.DynamicTestCasesPlugin>(Guid.NewGuid().ToString());
                _isAdded = true;
            }
        }
    }
}