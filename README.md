# blazor-component-rerendered
Component state doesn't retain when client side interaction in JavaScript wrapper based blazor components	Component state doesn't retain when client side interaction in JavaScript wrapper based blazor components

### Describe the bug
We have set of JavaScript wrapper based Blazor components. When we change the JavaScript property based on client interaction and modify the same property value in C#, it doesn't retain after `EventCallback` triggered.

The `EventCallback` invokes `StateHasChanged` life cycle method which re-render the component with initial values. So, whatever the values set through `interop.js` will be eliminated while component re-rendering.

I have tried to call the `StateHasChanged` after the `interop.js` method invoked, but it doesn't retain the property changes.

We need a way to retain the client-side changes in the component reference object after `EventCallback` triggered / component re-rendering.

### To Reproduce
Steps to reproduce the behavior:
1. Clone the below repository and run the sample.

2. Change the number input value and focus out.
3. The text input value will be changed as True.
4. Click the number input again and perform focus-out without changing the number value.
5. The text input value is changed as False after focus out event triggered.

### Sample description
In the sample we have the Blazor components `NumberComponent` and `StringComponent`. Both components are rendered using JavaScript in the `interop.js`.  Both components had `Value` and `IsChanged` properties. 

When the value property changed by user interaction in the input, JavaScript `change` event will be invoked and `isChanged` property will be set as true in `interop.js`.  This changes need to be updated in the C# side. so, we invoked a C# method `UpdateProperties` from `interop.js` to set corresponding `IsChanged` property as true. 

The C# property changes will be modified in the JavaScript side by using `setProperties` method through `interop.js`. The `onfocusout` event is applied to `NumberComponent` for reproducing the reported issue.

### Expected behavior
The component need to be re-rendered with client-side changed values and it should not affect the component UI while re-rendering. 

### Additional context
```
.NET Core SDK (reflecting any global.json):
 Version:   3.0.100-preview9-014004
 Commit:    8e7ef240a5

Runtime Environment:
 OS Name:     Windows
 OS Version:  10.0.17763
 OS Platform: Windows
 RID:         win10-x64
 Base Path:   C:\Program Files\dotnet\sdk\3.0.100-preview9-014004\

Host (useful for support):
  Version: 3.0.0-preview9-19423-09
  Commit:  2be172345a

.NET Core SDKs installed:
  1.1.13 [C:\Program Files\dotnet\sdk]
  1.1.14 [C:\Program Files\dotnet\sdk]
  2.1.202 [C:\Program Files\dotnet\sdk]
  2.1.505 [C:\Program Files\dotnet\sdk]
  2.1.508 [C:\Program Files\dotnet\sdk]
  2.1.602 [C:\Program Files\dotnet\sdk]
  2.1.700-preview-009618 [C:\Program Files\dotnet\sdk]
  2.1.800-preview-009696 [C:\Program Files\dotnet\sdk]
  3.0.100-preview7-012821 [C:\Program Files\dotnet\sdk]
  3.0.100-preview8-013656 [C:\Program Files\dotnet\sdk]
  3.0.100-preview9-014004 [C:\Program Files\dotnet\sdk]

.NET Core runtimes installed:
  Microsoft.AspNetCore.All 2.1.9 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.All]
  Microsoft.AspNetCore.All 2.1.11 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.All]
  Microsoft.AspNetCore.All 2.1.12 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.All]
  Microsoft.AspNetCore.App 2.1.9 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]
  Microsoft.AspNetCore.App 2.1.11 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]
  Microsoft.AspNetCore.App 2.1.12 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]
  Microsoft.AspNetCore.App 3.0.0-preview7.19365.7 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]
  Microsoft.AspNetCore.App 3.0.0-preview8.19405.7 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]
  Microsoft.AspNetCore.App 3.0.0-preview9.19424.4 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]
  Microsoft.NETCore.App 1.0.15 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
  Microsoft.NETCore.App 1.0.16 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
  Microsoft.NETCore.App 1.1.12 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
  Microsoft.NETCore.App 1.1.13 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
  Microsoft.NETCore.App 2.0.9 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
  Microsoft.NETCore.App 2.1.9 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
  Microsoft.NETCore.App 2.1.11 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
  Microsoft.NETCore.App 2.1.12 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
  Microsoft.NETCore.App 3.0.0-preview7-27912-14 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
  Microsoft.NETCore.App 3.0.0-preview8-28405-07 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
  Microsoft.NETCore.App 3.0.0-preview9-19423-09 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
  Microsoft.WindowsDesktop.App 3.0.0-preview7-27912-14 [C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App]
  Microsoft.WindowsDesktop.App 3.0.0-preview8-28405-07 [C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App]
  Microsoft.WindowsDesktop.App 3.0.0-preview9-19423-09 [C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App]

To install additional .NET Core runtimes or SDKs:
  https://aka.ms/dotnet-download
```
