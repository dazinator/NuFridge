define(['plugins/router'], function (router) {
    return {
        router: router,
        activate: function () {
            router.map([
           { route: '', moduleId: 'viewmodels/signin/signin' },
                { route: 'home', moduleId: 'viewmodels/signin/signin', nav: true }
                //{ route: 'tickets', moduleId: 'tickets/index', nav: true },
                //{ route: 'tickets/:id', moduleId: 'tickets/thread' },
                //{ route: 'users(/:id)', moduleId: 'users/index', hash: '#users', nav: true },
                //{ route: 'settings*details', moduleId: 'settings/index', hash: '#settings', nav: true }
            ]).buildNavigationModel();
       
            return router.activate();
        }
    };
});