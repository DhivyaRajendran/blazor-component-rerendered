using Microsoft.JSInterop;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace ComponentRerendered.Pages
{
    public class Base : ComponentBase
    {
        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        protected ElementReference compElement;

        protected bool isClientChanges { get; set; } = false;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                this.Initialize<string>(this, DotNetObjectReference.Create<Base>(this));
            }
            else if (!this.isClientChanges)
            {
                this.SetProperties<string>(this);
            }
            this.isClientChanges = false;
        }

        // Initialize JS component from C# to JS using interop.js
        public ValueTask<T> Initialize<T>(object compModel, DotNetObjectReference<Base> dotnetObj)
        {
            return this.jsRuntime.InvokeAsync<T>("interop.initComponent", this.compElement, compModel, dotnetObj);
        }

        // Update C# side property changes to JS through interop.js
        public ValueTask<T> SetProperties<T>(object compModel)
        {
            return this.jsRuntime.InvokeAsync<T>("interop.setProperties", this.compElement, compModel);
        }

        // Update client side changes to C# using interop.js call
        [JSInvokable]
        public void UpdateProperties(Dictionary<string, object> compModel)
        {
            this.isClientChanges = true;
            foreach (string key in compModel.Keys)
            {
                PropertyInfo Property = this.GetType().GetProperty(this.ConvertToBascalCase(key));
                if (Property != null)
                {
                    Type PropertyType = Property.PropertyType;
                    Property.SetValue(this, this.ChangeType(compModel[key], PropertyType));
                }
            }
            this.StateHasChanged();
        }

        protected string ConvertToBascalCase(string key)
        {
            return char.ToUpper(key[0]) + key.Substring(1);
        }

        protected object ChangeType(object value, Type conversionType)
        {
            var t = conversionType;
            value = (value == null) ? null : value.ToString();
            return Convert.ChangeType(value, t);
        }

    }

    public class NumberEventArgs {
        public int OldValue {get;set;}
        public int NewValue {get;set;}
    }

    public class StringEventArgs {
        public string OldValue {get;set;}
        public string NewValue {get;set;}
    }
}
