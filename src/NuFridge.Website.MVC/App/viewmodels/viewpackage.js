define(['plugins/router', 'durandal/app', 'knockout', 'viewmodels/shell', 'viewmodels/databinding/LuceneFeed', 'viewmodels/databinding/LucenePackage'], function (router, app, ko, shell, feed, feedPackage) {

    var ctor = function () {
        var self = this;
        this.Feed = ko.observable(new LuceneFeed());
        this.Package = ko.observable(new LucenePackage());
        this.PreviousVersions = ko.observableArray();
        this.FeedId = ko.observable();
        this.PackageId = ko.observable();
        this.CalculatedPackageName = ko.computed(function () {
            if (self.Package().title() != null) {
                return self.Package().title();
            }
            else {
                return self.Package().id();
            }
        });
    };

    ctor.prototype.getDownloadUrl = function(version) {
        var self = this;
        return "/api/feeds/DownloadPackage?feedId=" + self.FeedId() + "&packageId=" + self.PackageId() + "&version=" + version;
    };

    ctor.prototype.activate = function () {
        var self = this;

        self.FeedId(router.activeInstruction().params[0]);
        self.PackageId(router.activeInstruction().params[1]);

        shell.ShowNavigation(true);
        shell.ShowPageTitle(false);

        $.ajax({
            url: "/api/feeds/GetFeed/" + self.FeedId(),
            cache: false
        }).then(function (data) {
            ko.mapping.fromJS(data, LuceneFeed.mapping, self.Feed);


            $.ajax({
                url: self.Feed().FeedURL() + "/api/packages/" + self.PackageId(),
                cache: false,
                dataType: 'json'
            }).then(function (data) {
                ko.mapping.fromJS(data, LucenePackage.mapping, self.Package);
            }).fail(function (response) {
                alert("An error occurred loading the package.");
            });
            
            $.ajax({
                url: self.Feed().FeedURL() + "/api/v2/package-versions/" + self.PackageId(),
                cache: false,
                dataType: 'json'
            }).then(function (dataReturned) {
                self.PreviousVersions(dataReturned);
            }).fail(function (response) {
                alert("An error occurred loading the package history.");
            });


        }).fail(function () {
            alert("An error occurred loading the feed.");
        });
        

    };

    return ctor;
});