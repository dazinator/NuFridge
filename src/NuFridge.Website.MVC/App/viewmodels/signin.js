define(['plugins/router', 'durandal/app', 'knockout', 'viewmodels/shell'], function (router, app, ko, shell) {
    var ctor = function () {
        var self = this;
        self.SignInVisible = ko.observable(true);

        self.SignIn = function () {
            router.navigate('home');
        }

        self.Register = function () {
            router.navigate('home');
        }

        self.GetHelp = function () {
            router.navigate('home');
        }

        shell.ShowNavigation(false);
    };

    //Note: This module exports a function. That means that you, the developer, can create multiple instances.
    //This pattern is also recognized by Durandal so that it can create instances on demand.
    //If you wish to create a singleton, you should export an object instead of a function.
    //See the "flickr" module for an example of object export.

    return ctor;
});