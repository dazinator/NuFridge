define(['plugins/router', 'durandal/app', 'knockout', 'plugins/cssLoader'], function (router, app, ko, cssLoader) {
    return {
        ShowNavigation: ko.observable(true),
        ShowPageTitle: ko.observable(true),
        IsInstallationValid: ko.observable(),
        router: router,
        PageTitle: function () {
            var activeInstruction = router.activeInstruction();
            if (activeInstruction) {
                return activeInstruction.config.title;
            }
            return null;
        },
        bootstrapSkins: ko.observableArray(['Cerulean', 'Cosmo', 'Cyborg', 'Darkly', 'Flatly', 'Journal', 'Lumen', 'Readable', 'Slate', 'Superhero', 'United']),
        bootstrapSkinSelected: ko.observable(),
        allowSkinChange: ko.observable(true),
        activate: function () {
            var self = this;

            self.ChangeShellSkin('Darkly');

            router.map([
                { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true, glyph: 'glyphicon glyphicon-home' },
                { route: 'signin', title: 'Sign In', moduleId: 'viewmodels/signin', nav: false },
                { route: 'install', title: 'Installation', moduleId: 'viewmodels/install', nav: false },
                { route: 'feeds', title: 'Feeds', moduleId: 'viewmodels/viewfeeds', nav: true, glyph: 'glyphicon glyphicon-list' },
                { route: 'feeds/view/:id', title: 'View Feed', moduleId: 'viewmodels/addeditfeed', nav: false },
                { route: 'feeds/view/:id/package/:packageid', title: 'View Package', moduleId: 'viewmodels/viewpackage', nav: false },
                { route: 'feeds/create/:groupname', title: 'Add Feed', moduleId: 'viewmodels/addeditfeed', nav: false },
                { route: 'feeds/create', title: 'Add Feed', moduleId: 'viewmodels/addeditfeed', nav: false },
                { route: 'retentionpolicies', title: 'Retention Policies', moduleId: 'viewmodels/viewretentionpolicies', nav: false, glyph: 'glyphicon glyphicon-calendar' }
            ]).buildNavigationModel();

            router.activate();

            router.on('router:navigation:complete', function (instance, instruction, router) {
                var hasRunNavComplete = false;

                if (self.IsInstallationValid() == null) {
                    var hasRunNavComplete = true;

                    $.ajax({
                        url: '/api/installation/IsInstallationValid',
                        timeout: 5000,
                        dataType: 'json',
                        cache: false,
                        async: true
                    }).complete(function (response) {
                        if (response.status == 200 && response.responseText == 'true') {
                            self.IsInstallationValid(true);
                        } else {
                            self.IsInstallationValid(false);
                        }

                        self.NavCompleteCheckInstall(instruction);
                    });
                }
                if (!hasRunNavComplete) {
                    self.NavCompleteCheckInstall(instruction);
                }

            });
        },
        NavCompleteCheckInstall: function (instruction) {
            var self = this;
            if (self.IsInstallationValid() == false) {
                router.navigate("#install");
            }
            else if (instruction.config.hash == "#install") {
                router.navigate("");
            }
        },
        ChangeShellSkin: function (skinName) {
            if (this.bootstrapSkinSelected() != skinName) {
                this.allowSkinChange(false);
                this.bootstrapSkinSelected(skinName);
                cssLoader.removeModuleCss("shell");
                cssLoader.loadCss("shell", "../Content/themes/bootstrap/" + skinName + "/bootstrap.min.css");
                this.allowSkinChange(true);
            }
        }
    };
});