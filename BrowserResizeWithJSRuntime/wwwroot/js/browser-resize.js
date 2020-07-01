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