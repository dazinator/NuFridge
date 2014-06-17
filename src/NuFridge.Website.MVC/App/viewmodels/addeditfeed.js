define(['plugins/router', 'durandal/app', 'viewmodels/shell', 'plugins/dialog', 'viewmodels/databinding/LuceneFeed', 'viewmodels/databinding/LucenePackage'], function (router, app, shell, dialog, luceneFeed, lucenePackage) {

    var ctor = function () {
        var self = this;

        this.Feed = ko.observable(new LuceneFeed());
        this.PackageCount = ko.observable(0);
        this.ShowDeleteButton = ko.observable(true);
        this.EditFeedTitle = ko.observable();
        this.IsEditMode = ko.observable(false);

        this.PackagesLoading = ko.observable(true);
        this.StatusLoading = ko.observable(true);
        this.ErrorLoadingPackages = ko.observable(false);

        this.NoPackagesFound = ko.computed(function () {
            return self.PackagesLoading() == false && self.ErrorLoadingPackages() == false && self.Feed().Packages().length <= 0;
        });

        this.LoadingData = ko.computed(function () {
            return self.IsEditMode() && (self.PackagesLoading() || self.StatusLoading());
        });

        //TODO replace with knockout validation library
        this.IsFormDirty = ko.computed(function () {
            return self.Feed() != null && ((self.Feed().Name() != null && self.Feed().Name.isDirty() == true) || (self.Feed().APIKey() != null && self.Feed().APIKey.isDirty() == true));
        });
    };




    ctor.prototype.LoadPackagesFromFeed = function () {
        var self = this;

            $.ajax({
                url: this.Feed().FeedURL() + "/api/packages",
                data: { query: '', offset: 0, count: 5, originFilter: 'Any', sort: 'Score', order: 'Ascending', includePrerelease: true },
                dataType: 'json'
            }).then(function (data) {
                ko.mapping.fromJS(data.hits, LucenePackage.mapping, self.Feed().Packages);
                self.PackagesLoading(false);
                self.ErrorLoadingPackages(false);
            }).fail(function (response) {
                self.PackagesLoading(false);
                self.ErrorLoadingPackages(true);
            });
    };

    ctor.prototype.LoadFeedStatus = function () {
        var self = this;

        $.ajax({
            url: this.Feed().FeedURL() + "/api/indexing/status",
            dataType: 'json'
        }).then(function (data) {
            self.PackageCount(data.totalPackages);
            self.StatusLoading(false);
        }).fail(function() {
            self.StatusLoading(false);
        });
    };

    ctor.prototype.activate = function() {

        shell.ShowNavigation(true);
        shell.ShowPageTitle(false);

        $('#viewFeedTabs').tab();


        var self = this;

        var re = new RegExp("([a-z0-9]{8}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{12})");

        var createNew = false;

        if (router.activeInstruction().params.length == 1)
            createNew = true;

        if (createNew) {
            $.ajax({
                url: "/api/feeds/GetFeed/" + router.activeInstruction().params[0]
            }).then(function (data) {
                ko.mapping.fromJS(data, LuceneFeed.mapping, self.Feed);
                self.EditFeedTitle(self.Feed().Name());
                self.IsEditMode(true);
                self.LoadFeedStatus();
                self.LoadPackagesFromFeed();
            }).fail(function () {
                self.ShowDeleteButton(false);
                alert("An error occurred loading the feed.");
            });
        } else {

            self.ShowDeleteButton(false);
            self.EditFeedTitle = "Create Feed";
            self.IsEditMode(false);
        }
    };

    ctor.prototype.Delete = function() {
        var self = this;

        var result = self.ConfirmDeleteMessage().then(function (data) {
            if (data == "Yes") {
                $.ajax({
                    type: 'DELETE',
                    url: "/api/feeds/DeleteFeed/" + self.Feed().Id(),
                    cache: false
                }).then(function (item) {
                    router.navigate('#feeds');
                }).fail(function (data) {
                    self.ShowDeleteButton(false);
                    alert("An error occurred deleting the feed.");
                    router.navigate('#feeds');
                });
            }
        });
    };

    ctor.prototype.ConfirmDeleteMessage = function () {
        return app.showMessage('Are you sure you want to delete this feed? You will lose all packages stored in this feed.', 'Delete Feed', ['No', 'Yes']);
    };

    ctor.prototype.SaveChanges = function () {
        var self = this;

        if (this.Feed().Id() != null) {
            $.ajax({
                url: "/api/feeds/PutFeed/" + self.Feed().Id(),
                type: 'PUT',
                data: self.Feed(),
                dataType: 'json',
                cache: false,
                success: function (result) {
                    router.navigate('#feeds');
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                }
            });
        } else {
            $.ajax({
                url: "/api/feeds/PostFeed",
                type: 'POST',
                cache: false,
                dataType: 'json',
                data: self.Feed(),
                success: function (result) {
                    router.navigate('#feeds');
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                }
            });
        }
    };

    ctor.prototype.Cancel = function() {
        router.navigate('#feeds');
    };

    return ctor;
});