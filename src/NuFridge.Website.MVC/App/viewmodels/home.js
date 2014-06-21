define(['plugins/router', 'durandal/app', 'knockout', 'viewmodels/shell'], function (router, app, ko, shell) {
    var ctor = function () {
    };

    ctor.prototype.activate = function () {
        shell.ShowNavigation(true);
    };

    return ctor;
});