using System;
using System.Collections.Generic;
using System.Linq;
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
