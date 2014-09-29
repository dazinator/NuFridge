requirejs.config({
    paths: {
        'text': '../Scripts/text',
        'durandal': '../Scripts/durandal',
        'plugins': '../Scripts/durandal/plugins',
        'transitions': '../Scripts/durandal/transitions',
        'knockout': '../Scripts/knockout-3.1.0',
        'knockoutmapping': '../Scripts/knockout.mapping',
        'jQuery': '../Scripts/jquery-1.9.1',
        'introjs': '../Scripts/intro',
        'masonry': '../Scripts/masonry',
        'knockoutvalidation': '../Scripts/knockout.validation'
    }
});

define('jquery', function() { return jQuery; });
define('knockout', ko);

define(['durandal/system', 'durandal/app', 'durandal/viewLocator', 'knockoutmapping'], function (system, app, viewLocator, komapping) {

    ko.mapping = komapping;

    ko.bindingHandlers.href = {
        update: function (element, valueAccessor) {
            ko.bindingHandlers.attr.update(element, function () {
                return { href: valueAccessor() };
            });
        }
    };

    ko.bindingHandlers.src = {
        update: function (element, valueAccessor) {
            ko.bindingHandlers.attr.update(element, function () {
                return { src: valueAccessor() };
            });
        }
    };

    ko.bindingHandlers.returnAction = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = ko.utils.unwrapObservable(valueAccessor());

            $(element).keydown(function (e) {
                if (e.which === 13) {
                    value(viewModel);
                }
            });
        }
    };

    //>>excludeStart("build", true);
    system.debug(true);
    //>>excludeEnd("build");

    app.title = 'NuFridge';

    app.configurePlugins({
        router: true,
        dialog: true,
        widget: true
    });

    app.start().then(function() {
        //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
        //Look for partial views in a 'views' folder in the root.
        viewLocator.useConvention();

        //Show the app by setting the root view model for our application with a transition.
        app.setRoot('viewmodels/shell', 'entrance');
    });
});