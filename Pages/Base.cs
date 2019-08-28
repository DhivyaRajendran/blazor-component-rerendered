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

        protected bool isRendered {get;set;} = false;

        protected bool isClientChanges { get; set; } = false;

        protected static object CreateDotNetObjectRefSyncObj = new object();
        protected DotNetObjectRef<T> CreateDotNetObjectRef<T>(T value) where T : class
        {
            lock (CreateDotNetObjectRefSyncObj)
            {
                JSRuntime.SetCurrentJSRuntime(this.jsRuntime);
                return DotNetObjectRef.Create(value);
            }
        }

        protected void DisposeDotNetObjectRef<T>(DotNetObjectRef<T> value) where T : class
        {
            if (value != null)
            {
                lock (CreateDotNetObjectRefSyncObj)
                {
                    JSRuntime.SetCurrentJSRuntime(this.jsRuntime);
                    value.Dispose();
                }
            }
        }

        protected override void OnAfterRender()
        {
            if (!this.isRendered)
            {
                this.Initialize<string>(this, this.CreateDotNetObjectRef<Base>(this));
                this.isRendered = true;
            }
            else if (!this.isClientChanges)
            {
                this.SetProperties<string>(this);
            }
            this.isClientChanges = false;
        }

        // Initialize JS component from C# to JS using interop.js
        public Task<T> Initialize<T>(object compModel, DotNetObjectRef<Base> dotnetObj)
        {
            return this.jsRuntime.InvokeAsync<T>("interop.initComponent", this.compElement, compModel, dotnetObj);
        }

        // Update C# side property changes to JS through interop.js
        public Task<T> SetProperties<T>(object compModel)
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
