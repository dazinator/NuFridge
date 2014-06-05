requirejs.config({
    paths: {
        'text': '../Scripts/text',
        'durandal': '../Scripts/durandal',
        'plugins': '../Scripts/durandal/plugins',
        'transitions': '../Scripts/durandal/transitions',
        'knockout': '../Scripts/knockout-2.3.0',
        'jquery': '../Scripts/jquery-1.9.1',
    }
});

define(['durandal/system', 'durandal/app', 'durandal/viewLocator', 'plugins/router'], function (system, app, viewLocator, router) {

    system.debug(true);
  
    app.title = 'NuFridge';

    app.configurePlugins({
        router: true
    });

    app.start().then(function () {
        viewLocator.useConvention();
        app.setRoot('viewmodels/shell', 'entrance');
    });
});