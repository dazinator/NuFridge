define(['plugins/router', 'durandal/app', 'knockout', 'viewmodels/shell', 'viewmodels/databinding/feed', 'viewmodels/databinding/package'], function (router, app, ko, shell, feed, feedPackage) {

    var ctor = function () {
        this.Feed = ko.observable(new FeedObject());
        this.Package = ko.observable(new PackageObject());

        this.FeedId = ko.observable();
        this.PackageId = ko.observable();
    };

    ctor.prototype.activate = function () {
        var self = this;

        self.FeedId(router.activeInstruction().params[0])
        self.PackageId(router.activeInstruction().params[1])

        shell.ShowNavigation(true);
        shell.ShowPageTitle(false);

        $.ajax({
            url: "/api/feeds/GetFeed/" + self.FeedId(),
            cache: false
        }).then(function (data) {
            ko.mapping.fromJS(data, mapping, self.Feed);

            var feedURL = self.Feed().FeedURL();

            $.ajax({
                url: feedURL + "/api/packages/" + self.PackageId(),
                cache: false,
                dataType: 'json'
            }).then(function (data) {
                ko.mapping.fromJS(data, ctor.packageMapping, self.Package);
            }).fail(function (response) {
                alert("An error occurred loading the package.");
            });
        }).fail(function () {
            alert("An error occurred loading the feed.");
        });
    };

    var mapping = {
        create: function (options) {
            return new FeedObject(options.data);
        }
    };

    ctor.packageMapping = {
        create: function (options) {
            return new PackageObject(options.data);
        }
    };

    return ctor;
});