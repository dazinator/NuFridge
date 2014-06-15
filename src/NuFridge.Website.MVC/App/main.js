requirejs.config({
    paths: {
        'text': '../Scripts/text',
        'durandal': '../Scripts/durandal',
        'plugins': '../Scripts/durandal/plugins',
        'transitions': '../Scripts/durandal/transitions',
        'knockout': '../Scripts/knockout-3.1.0',
        'knockoutmapping': '../Scripts/knockout.mapping',
        'jQuery': '../Scripts/jquery-1.9.1',
        'introjs': '../Scripts/intro'
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

    ko.extenders.trackChange = function (target, track) {
        if (track) {
            target.isDirty = ko.observable(false);
            target.originalValue = target();
            target.subscribe(function (newValue) {
                // use != not !== so numbers will equate naturally
                target.isDirty(newValue != target.originalValue);
            });
        }
        return target;
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