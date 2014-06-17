define(['plugins/router', 'durandal/app', 'viewmodels/shell', 'plugins/dialog', 'viewmodels/databinding/feed', 'viewmodels/databinding/package'], function (router, app, shell, dialog, feed, feedPackage) {

    var ctor = function () {
        var self = this;

        this.Feed = ko.observable(new FeedObject());

        this.ShowDeleteButton = ko.observable(true);
        this.EditFeedTitle = ko.observable();
        this.IsEditMode = ko.observable(false);
        this.PackagesLoading = ko.observable(true);
        this.ErrorLoadingPackages = ko.observable(false);
        this.NoPackagesFound = ko.computed(function () {
            return self.PackagesLoading() == false && self.ErrorLoadingPackages() == false && self.Feed().Packages().length <= 0;
        });

        this.IsFormDirty = ko.computed(function () {
            return self.Feed() != null && ((self.Feed().Name() != null && self.Feed().Name.isDirty() == true) || (self.Feed().APIKey() != null && self.Feed().APIKey.isDirty() == true));
        });

        this.activeTab = ko.observable();
    };

    var mapping = {
        create: function (options) {
            return new FeedObject(options.data);
        }
    };


    ctor.prototype.compositionComplete = function ()
    {
        var self = this;

        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            self.activeTab(e.target.hash);

            if (self.activeTab() == "#tabPackages") {
                self.LoadPackagesFromFeed();
            }
        });
    };


    ctor.packageMapping = {
        create: function (options) {
            var po = new PackageObject(options.data);
            po.ViewUrl = ko.computed(function () {
                   return '#feeds/view/' + router.activeInstruction().params[0] + "/package/" + po.id();
            });
            return po;
        }
    };


    ctor.prototype.LoadPackagesFromFeed = function () {
        var self = this;
        var feedURL = this.Feed().FeedURL();

        if (self.Feed().Packages().length <= 0) {
            $.ajax({
                url: feedURL + "/api/packages",
                data: { query: '', offset: 0, count: 5, originFilter: 'Any', sort: 'Score', order: 'Ascending', includePrerelease: true },
                cache: false,
                dataType: 'json'
            }).then(function (data) {
                ko.mapping.fromJS(data.hits, ctor.packageMapping, self.Feed().Packages);
                self.PackagesLoading(false);
                self.ErrorLoadingPackages(false);
            }).fail(function (response) {
                self.PackagesLoading(false);
                self.ErrorLoadingPackages(true);
            });
        }
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
                url: "/api/feeds/GetFeed/" + router.activeInstruction().params[0],
                cache: false
            }).then(function (data) {
                ko.mapping.fromJS(data, mapping, self.Feed);
                self.EditFeedTitle(self.Feed().Name());
                self.IsEditMode(true);
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