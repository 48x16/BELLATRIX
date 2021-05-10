﻿// <copyright file="Element.DefaultActions.cs" company="Automate The Planet Ltd.">
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
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Bellatrix.Web.Events;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Bellatrix.Web
{
    public partial class Element
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Point Location => WrappedElement.Location;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Size Size => WrappedElement.Size;

        public string GetCssValue(string propertyName) => WrappedElement.GetCssValue(propertyName);

        protected virtual void DefaultSetAttribute(string attributeName, string attributeValue)
        {
            SettingAttribute?.Invoke(this, new ElementActionEventArgs(this));

            JavaScriptService.Execute(
                $"arguments[0].setAttribute('{attributeName}', '{attributeValue}');", this);

            AttributeSet?.Invoke(this, new ElementActionEventArgs(this));
        }

        protected virtual string GetClassAttribute()
        {
            return string.IsNullOrEmpty(WrappedElement.GetAttribute("class")) ? null : WrappedElement.GetAttribute("class");
        }

        internal void Click(EventHandler<ElementActionEventArgs> clicking, EventHandler<ElementActionEventArgs> clicked)
        {
            clicking?.Invoke(this, new ElementActionEventArgs(this));

            this.ToExists().ToBeClickable().WaitToBe();
            WrappedElement.Click();

            clicked?.Invoke(this, new ElementActionEventArgs(this));
        }

        internal void Hover(EventHandler<ElementActionEventArgs> hovering, EventHandler<ElementActionEventArgs> hovered)
        {
            hovering?.Invoke(this, new ElementActionEventArgs(this));

            JavaScriptService.Execute("arguments[0].onmouseover();", this);

            hovered?.Invoke(this, new ElementActionEventArgs(this));
        }

        internal string GetInnerText()
        {
            return WrappedElement.Text.Replace("\r\n", string.Empty);
        }

        internal void SetValue(EventHandler<ElementActionEventArgs> gettingValue, EventHandler<ElementActionEventArgs> gotValue, string value)
        {
            gettingValue?.Invoke(this, new ElementActionEventArgs(this, value));
            SetAttribute("value", value);
            gotValue?.Invoke(this, new ElementActionEventArgs(this, value));
        }

        internal string DefaultGetValue()
        {
            return WrappedElement.GetAttribute("value");
        }

        internal int? DefaultGetMaxLength()
        {
            int? result = string.IsNullOrEmpty(GetAttribute("maxlength")) ? null : (int?)int.Parse(GetAttribute("maxlength"));
            if (result != null && (result == 2147483647 || result == -1))
            {
                result = null;
            }

            return result;
        }

        internal int? DefaultGetMinLength()
        {
            int? result = string.IsNullOrEmpty(GetAttribute("minlength")) ? null : (int?)int.Parse(GetAttribute("minlength"));

            if (result != null && result == -1)
            {
                result = null;
            }

            return result;
        }

        internal int? GetSizeAttribute()
        {
            return string.IsNullOrEmpty(GetAttribute("size")) ? null : (int?)int.Parse(GetAttribute("size"));
        }

        internal int? GetHeightAttribute()
        {
            return string.IsNullOrEmpty(GetAttribute("height")) ? null : (int?)int.Parse(GetAttribute("height"));
        }

        internal int? GetWidthAttribute()
        {
            return string.IsNullOrEmpty(GetAttribute("width")) ? null : (int?)int.Parse(GetAttribute("width"));
        }

        internal string GetInnerHtmlAttribute()
        {
            return WrappedElement.GetAttribute("innerHTML");
        }

        internal string GetForAttribute()
        {
            return string.IsNullOrEmpty(GetAttribute("for")) ? null : GetAttribute("for");
        }

        protected bool GetDisabledAttribute()
        {
            string valueAttr = WrappedElement.GetAttribute("disabled");
            return valueAttr == "true";
        }

        internal string GetText()
        {
            return WrappedElement.Text;
        }

        internal int? GetMinAttribute()
        {
            return string.IsNullOrEmpty(GetAttribute("min")) ? null : (int?)int.Parse(GetAttribute("min"));
        }

        internal int? GetMaxAttribute()
        {
            return string.IsNullOrEmpty(GetAttribute("max")) ? null : (int?)int.Parse(GetAttribute("max"));
        }

        internal string GetMinAttributeAsString()
        {
            return string.IsNullOrEmpty(GetAttribute("min")) ? null : GetAttribute("min");
        }

        internal string GetMaxAttributeAsString()
        {
            return string.IsNullOrEmpty(GetAttribute("max")) ? null : GetAttribute("max");
        }

        internal int? GetStepAttribute()
        {
            return string.IsNullOrEmpty(GetAttribute("step")) ? null : (int?)int.Parse(GetAttribute("step"));
        }

        internal string GetPlaceholderAttribute()
        {
            return string.IsNullOrEmpty(GetAttribute("placeholder")) ? null : GetAttribute("placeholder");
        }

        internal bool GetAutoCompleteAttribute()
        {
            return GetAttribute("autocomplete") == "on";
        }

        internal bool GetReadonlyAttribute()
        {
            return !string.IsNullOrEmpty(GetAttribute("readonly"));
        }

        internal bool GetRequiredAttribute()
        {
            return !string.IsNullOrEmpty(GetAttribute("required"));
        }

        internal string GetList()
        {
            return string.IsNullOrEmpty(GetAttribute("list")) ? null : GetAttribute("list");
        }

        internal void DefaultSetText(EventHandler<ElementActionEventArgs> settingValue, EventHandler<ElementActionEventArgs> valueSet, string value)
        {
            settingValue?.Invoke(this, new ElementActionEventArgs(this, value));

            // HACK: (Anton: 06.09.2018): InternetExplorer SendKeys is typing everything twice.
            if (!WrappedWebDriverCreateService.BrowserConfiguration.BrowserType.Equals(BrowserType.InternetExplorer))
            {
                Thread.Sleep(50);

                // (Anton: 11.09.2019): We do this for optimization purposes so that we don't make two WebDriver calls.
                var wrappedElement = WrappedElement;
                try
                {
                    wrappedElement.Clear();
                }
                catch
                {
                    // ignore
                }

                Thread.Sleep(50);

                wrappedElement.SendKeys(value);
            }
            else
            {
                SetAttribute("value", value);
            }

            valueSet?.Invoke(this, new ElementActionEventArgs(this, value));
        }

        private void ScrollToVisible(bool shouldWait = true)
        {
            ScrollingToVisible?.Invoke(this, new ElementActionEventArgs(this));
            try
            {
                var wrappedElement = _wrappedElement ?? WrappedElement;
                JavaScriptService.Execute("arguments[0].scrollIntoView(true);", wrappedElement);
                if (shouldWait)
                {
                    Thread.Sleep(500);
                    this.ToExists().WaitToBe();
                }
            }
            catch (ElementNotInteractableException)
            {
                // ignore
            }

            ScrolledToVisible?.Invoke(this, new ElementActionEventArgs(this));
        }
    }
}