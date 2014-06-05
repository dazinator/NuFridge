requirejs.config({
    paths: {
        'text': '../lib/require/text',
        'durandal': '../lib/durandal/js',
        'plugins': '../lib/durandal/js/plugins',
        'transitions': '../lib/durandal/js/transitions',
        'knockout': '../lib/knockout/knockout-3.1.0',
        'knockout.validation': '../lib/knockout/knockout.validation',
        'bootstrap': '../lib/bootstrap/js/bootstrap',
        'jquery': '../lib/jquery/jquery-1.9.1'
    },
    shim: {
        'bootstrap': {
            deps: ['jquery'],
            exports: 'jQuery'
        },
        'knockout.validation': {
            deps: ['knockout']
        }
    }
});

define(['durandal/system', 'durandal/app', 'durandal/viewLocator', 'durandal/composition', 'knockout', 'knockout.validation'],
   function (system, app, viewLocator, composition, ko) {

    system.debug(true);


    app.title = 'NuFridge';

    app.configurePlugins({
        router: true,
        dialog: true,
        widget: true
    });

    composition.addBindingHandler('hasFocus');

    configureKnockout();

    app.start().then(function () {
        //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
        //Look for partial views in a 'views' folder in the root.
        viewLocator.useConvention();

        //Show the app by setting the root view model for our application with a transition.
        app.setRoot('viewmodels/shell', 'entrance');
    });

    function configureKnockout() {
        ko.validation.init({
            insertMessages: true,
            decorateElement: true,
            errorElementClass: 'has-error',
            errorMessageClass: 'help-block'
        });
    }
});