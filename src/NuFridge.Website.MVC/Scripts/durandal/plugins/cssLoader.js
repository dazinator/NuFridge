define(['jquery'], function ($) {
    return {
        loadCss: function (viewName, fileName) {
            var cssTag = document.createElement("link")
            cssTag.setAttribute("rel", "stylesheet")
            cssTag.setAttribute("type", "text/css")
            cssTag.setAttribute("href", fileName)
            cssTag.setAttribute("class", "__dynamicCss " + viewName)

            document.getElementsByTagName("head")[0].appendChild(cssTag)
        },
        removeModuleCss: function (viewName) {
            $(".__dynamicCss." + viewName).remove();

        }
    };
});