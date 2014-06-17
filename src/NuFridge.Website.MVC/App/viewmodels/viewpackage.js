define(['plugins/router', 'durandal/app', 'knockout', 'viewmodels/shell', 'viewmodels/databinding/LuceneFeed', 'viewmodels/databinding/LucenePackage'], function (router, app, ko, shell, feed, feedPackage) {

    var ctor = function () {
        this.Feed = ko.observable(new LuceneFeed());
        this.Package = ko.observable(new LucenePackage());

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
            ko.mapping.fromJS(data, LuceneFeed.mapping, self.Feed);

            var feedURL = self.Feed().FeedURL();

            $.ajax({
                url: feedURL + "/api/packages/" + self.PackageId(),
                cache: false,
                dataType: 'json'
            }).then(function (data) {
                ko.mapping.fromJS(data, LucenePackage.mapping, self.Package);
            }).fail(function (response) {
                alert("An error occurred loading the package.");
            });
        }).fail(function () {
            alert("An error occurred loading the feed.");
        });
    };

    return ctor;
});