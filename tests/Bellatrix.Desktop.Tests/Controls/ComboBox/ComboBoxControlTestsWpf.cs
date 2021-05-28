﻿// <copyright file="ComboBoxControlTestsWpf.cs" company="Automate The Planet Ltd.">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bellatrix.Desktop.Tests
{
    [TestClass]
    [App(Constants.WpfAppPath, Lifecycle.RestartEveryTime)]
    [AllureSuite("ComboBox Control")]
    [AllureTag("WPF")]
    public class ComboBoxControlTestsWpf : MSTest.DesktopTest
    {
        [TestMethod]
        [TestCategory(Categories.Desktop)]
        public void MessageChanged_When_ComboBoxHovered_Wpf()
        {
            var comboBox = App.Components.CreateByAutomationId<ComboBox>("select");

            comboBox.Hover();

            var label = App.Components.CreateByAutomationId<Label>("ResultLabelId");
            Assert.AreEqual("selectHovered", label.InnerText);
        }

        [TestMethod]
        [TestCategory(Categories.CI)]
        [TestCategory(Categories.Desktop)]
        public void Item2Selected_When_ComboBoxSecondOption_Wpf()
        {
            var comboBox = App.Components.CreateByAutomationId<ComboBox>("select");

            comboBox.SelectByText("Item2");

            Assert.AreEqual("Item2", comboBox.InnerText);
        }

        [TestMethod]
        [TestCategory(Categories.CI)]
        [TestCategory(Categories.Desktop)]
        public void IsDisabledReturnsFalse_When_ComboBoxIsNotDisabled_Wpf()
        {
            var comboBox = App.Components.CreateByAutomationId<ComboBox>("select");

            Assert.AreEqual(false, comboBox.IsDisabled);
        }

        [TestMethod]
        [TestCategory(Categories.CI)]
        [TestCategory(Categories.Desktop)]
        public void IsDisabledReturnsTrue_When_ComboBoxIsDisabled_Wpf()
        {
            var comboBox = App.Components.CreateByAutomationId<ComboBox>("disalbedSelect");

            Assert.AreEqual(true, comboBox.IsDisabled);
        }
    }
}
