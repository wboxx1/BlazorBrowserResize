First create BrowserResize service

**BrowserResize.cs**
```C#
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BrowserResizeWithJSRuntime.Services
{
    public class BrowserResize
    {
        public static event Func<Task> OnResize;

        [JSInvokable]
        public static async Task OnBrowserResize()
        {
            await OnResize?.Invoke();
        }

        public static async Task<int> GetInnerHeight(IJSRuntime jsRuntime)
        {
            return await jsRuntime.InvokeAsync<int>("browserResize.getInnerHeight");
        }

        public static async Task<int> GetInnerWidth(IJSRuntime jsRuntime)
        {
            return await jsRuntime.InvokeAsync<int>("browserResize.getInnerWidth");
        }

    }
}
```

Next add service for injection.

**Program.cs**
```C#
using BrowserResizeWithJSRuntime.Services;

namespace BrowserResizeWithJSRuntime
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<BrowserResize>();

            await builder.Build().RunAsync();
        }
    }
}
```

Next add inject service and code into razor file.

**Index.razor**
```HTML+Razor
@page "/"
@using BrowserResizeWithJSRuntime.Services
@inject BrowserResize ResizeService
@inject IJSRuntime JSRuntime
@implements IDisposable

<h1>Hello, world!</h1>

Welcome to your new app.

<SurveyPrompt Title="How is Blazor working for you?" />

<h1>Height: @Height</h1>
<h1>Width: @Width</h1>

@code{
    public int Height { get; set; }
    public int Width { get; set; }


    protected override async Task OnInitializedAsync()
        {
            BrowserResize.OnResize += BrowserHasResized;

            await JSRuntime.InvokeAsync<object>("browserResize.registerResizeCallback");

            Height = await BrowserResize.GetInnerHeight(JSRuntime);
            Width = await BrowserResize.GetInnerWidth(JSRuntime);
        }

    private async Task BrowserHasResized()
    {
        Height = await BrowserResize.GetInnerHeight(JSRuntime);
        Width = await BrowserResize.GetInnerWidth(JSRuntime);

        StateHasChanged();
    }

    public void Dispose()
    {
        BrowserResize.OnResize -= BrowserHasResized;
    }

}
```

Finally, create javascript file and import to index.html

**browser-resize.js**
```JavaScript
var globalResizeTimer = null;

window.browserResize = {
    getInnerHeight: function () {
        return window.innerHeight;
    },
    getInnerWidth: function () {
        return window.innerWidth;
    },
    registerResizeCallback: function () {
        window.addEventListener("resize", browserResize.resized);
    },
    resized: function () {
        //DotNet.invokeMethod("BrowserResize", 'OnBrowserResize');
        if (globalResizeTimer != null) window.clearTimeout(globalResizeTimer);
        globalResizeTimer = window.setTimeout(function () {
            DotNet.invokeMethodAsync("BrowserResizeWithJSRuntime", "OnBrowserResize").then(data => data);
        }, 200);
    }
}
```

**index.html**
```HTML
<!DOCTYPE html>
<html>

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>BrowserResizeWithJSRuntime</title>
    <base href="/" />
    <link href="css/bootstrap/bootstrap.min.css" rel="stylesheet" />
    <link href="css/app.css" rel="stylesheet" />
</head>

<body>
    <app>Loading...</app>

    <div id="blazor-error-ui">
        An unhandled error has occurred.
        <a href="" class="reload">Reload</a>
        <a class="dismiss">🗙</a>
    </div>
    <script src="_framework/blazor.webassembly.js"></script>
    <script src="js/browser-resize.js"></script>
</body>

</html>
```
