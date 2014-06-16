define(['plugins/router', 'durandal/app', 'knockout'], function (router, app, ko) {
    return {
        ShowNavigation: ko.observable(true),
        ShowPageTitle: ko.observable(true),
        router: router,
        PageTitle: function () {
            var activeInstruction = router.activeInstruction();
            if (activeInstruction) {
                return activeInstruction.config.title;
            }
            return null;
        },
        activate: function () {
            router.map([
                { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true, glyph: 'glyphicon glyphicon-home' },
                { route: 'signin', title: 'Sign In', moduleId: 'viewmodels/signin', nav: false },
                { route: 'install', title: 'Installation', moduleId: 'viewmodels/install', nav: false },
                { route: 'feeds', title: 'Feeds', moduleId: 'viewmodels/viewfeeds', nav: true, glyph: 'glyphicon glyphicon-list' },
                { route: 'feeds/view/:id', title: 'View Feed', moduleId: 'viewmodels/addeditfeed', nav: false },
                { route: 'feeds/create', title: 'Add Feed', moduleId: 'viewmodels/addeditfeed', nav: false },
                { route: 'retentionpolicies', title: 'Retention Policies', moduleId: 'viewmodels/viewretentionpolicies', nav: true, glyph: 'glyphicon glyphicon-calendar' }
            ]).buildNavigationModel();

            return router.activate('viewmodels/signin', 'entrance');
        }
    };
});