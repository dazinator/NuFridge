﻿define(['plugins/router', 'durandal/app', 'viewmodels/shell', 'plugins/dialog', 'viewmodels/databinding/LuceneFeed', 'viewmodels/databinding/LucenePackage', 'viewmodels/databinding/LuceneFeedVersion', 'knockoutvalidation'], function (router, app, shell, dialog, luceneFeed, lucenePackage, luceneFeedVersion, validation) {

    var ctor = function () {
        var self = this;

        this.Feed = ko.observable(new LuceneFeed());
        this.FeedVersion = ko.observable(new LuceneFeedVersion());
        this.ShowDeleteButton = ko.observable(true);
        this.EditFeedTitle = ko.observable();
        this.IsEditMode = ko.observable(false);
        this.SaveError = ko.observable(null);

        this.PackagesLoading = ko.observable(true);
        this.SavingFeed = ko.observable(false);
        this.DeletingFeed = ko.observable(false);
        this.VersionLoading = ko.observable(true);
        this.ErrorLoadingPackages = ko.observable(false);

        this.PackagePageSize = ko.observable(5);
        this.PackageActivePageIndex = ko.observable(1);
        this.PackagePackageHits = ko.observable(0);
        this.PackagePageCount = ko.computed(function () {
            var count = Math.ceil((self.PackagePackageHits() / self.PackagePageSize()));
            return count > 0 ? count : 1;
        });

        this.SearchForPackagesValue = ko.observable("");

        this.PackageCurrentPage = ko.observable(1);

        this.NoPackagesFound = ko.computed(function () {
            return self.PackagesLoading() == false && self.ErrorLoadingPackages() == false && self.Feed().Packages().length <= 0;
        });

        this.IsBusy = ko.computed(function () {
            return (self.IsEditMode() && (self.PackagesLoading() || self.VersionLoading()  || self.DeletingFeed())) || self.SavingFeed();
        });

        this.PackageSearchSuggestions = ko.observableArray();
    };

    ctor.prototype.LoadFeedVersion = function () {
        var self = this;

        $.ajax({
            url: this.Feed().FeedURL() + "/api/version",
            dataType: 'json',
            cache: false
        }).then(function (data) {
            ko.mapping.fromJS(data, LuceneFeedVersion.mapping, self.FeedVersion);
            self.VersionLoading(false);
        }).fail(function () {
            self.VersionLoading(false);
        });
    };

    ctor.prototype.activate = function () {
        var self = this;
        shell.ShowNavigation(true);
        shell.ShowPageTitle(false);

        $('#viewFeedTabs').tab();

        var createNew = true;

        if (router.activeInstruction().fragment.indexOf("feeds/view") >= 0)
            createNew = false;

        if (!createNew) {
            $.ajax({
                url: "/api/feeds/GetFeed/" + router.activeInstruction().params[0]
            }).then(function (data) {
                ko.mapping.fromJS(data, LuceneFeed.mapping, self.Feed);
                self.EditFeedTitle(self.Feed().Name());
                self.IsEditMode(true);
                self.LoadFeedVersion();
                self.LoadPackagesFromFeed();
            }).fail(function () {
                self.ShowDeleteButton(false);
                alert("An error occurred loading the feed.");
            });
        } else {
            if (router.activeInstruction().params.length == 1) {
                self.Feed().GroupName(router.activeInstruction().params[0]);
            }
            self.ShowDeleteButton(false);
            self.EditFeedTitle = "Create Feed";
            self.IsEditMode(false);
        }
    };

    ctor.prototype.Delete = function () {
        var self = this;

        var result = self.ConfirmDeleteMessage().then(function (data) {
            if (data == "Yes") {
                self.DeletingFeed(true);
                $.ajax({
                    type: 'DELETE',
                    url: "/api/feeds/DeleteFeed/" + self.Feed().Id(),
                    cache: false
                }).then(function (item) {
                    self.DeletingFeed(false);
                    router.navigate('#feeds');
                }).fail(function (data) {
                    self.DeletingFeed(false);
                    self.ShowDeleteButton(false);
                    alert("An error occurred deleting the feed.");
                    router.navigate('#feeds');
                });
            }
        });
    };

    ctor.prototype.AddPackage = function () {
        var self = this;
        app.showDialog("viewmodels/addpackage").done(function() {
            self.LoadPackagesFromFeed();
        });

    };

    ctor.prototype.ConfirmDeleteMessage = function () {
        return app.showMessage('Are you sure you want to delete this feed? You will lose all packages stored in this feed.', 'Delete Feed', ['No', 'Yes']);
    };

    ctor.prototype.SaveChanges = function () {
        var self = this;
        self.SaveError(null);
        self.SavingFeed(true);
        if (this.Feed().Id() != null) {
            $.ajax({
                url: "/api/feeds/PutFeed/" + self.Feed().Id(),
                type: 'PUT',
                data: self.Feed(),
                dataType: 'json',
                cache: false,
                success: function (result) {
                    self.SavingFeed(false);
                    router.navigate('#feeds');
                },
                error: function (XMLHttpResponse, textStatus, errorThrown) {
                    self.reportSaveError(XMLHttpResponse);
                    self.SavingFeed(false);
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
                    self.SavingFeed(false);
                    router.navigate('#feeds');
                },
                error: function (XMLHttpResponse, textStatus, errorThrown) {
                    self.SavingFeed(false);
                    self.reportSaveError(XMLHttpResponse);
                }
            });
        }
    };

    ctor.prototype.reportSaveError = function (response) {
        var self = this;
        if (response.responseText != "") {
            var responseText = JSON.parse(response.responseText);
            if (responseText.ExceptionMessage) {
                self.SaveError(responseText.ExceptionMessage);
            }
            else if (responseText.Message) {
                self.SaveError(responseText.Message);
            }
            else {
                self.SaveError(responseText);
            }
        }
        else {
            self.SaveError("An unknown error has occurred.");
        }
    };

    ctor.prototype.Cancel = function () {
        router.navigate('#feeds');
    };

    ctor.prototype.GetMatchingPackages = function () {
        var self = this;
        $.ajax({
            url: this.Feed().FeedURL() + "/api/v2/package-ids",
            data: { partialId: self.SearchForPackagesValue(), maxResults: 10, includePrerelease: true },
            dataType: 'json',
            cache: false
        }).then(function (data) {
            self.PackageSearchSuggestions(data);
        });
    };


    ctor.prototype.PackagePageChanged = function (pageNumber) {
        var self = this;
        if (self.PackageActivePageIndex() == pageNumber || pageNumber < 1 || pageNumber > self.PackagePageCount())
            return;

        self.PackageActivePageIndex(pageNumber);

        self.LoadPackagesFromFeed(pageNumber);
    };

    ctor.prototype.LoadPackagesFromFeed = function (pageNumber, searchInput) {
        var self = this;

        if (pageNumber == null)
            pageNumber = 1;

        if (searchInput == null)
            searchInput = '';

        self.PackageSearchSuggestions.removeAll();

        var offSet = (pageNumber - 1) * self.PackagePageSize();
        $.ajax({
            url: this.Feed().FeedURL() + "/api/packages",
            data: { query: searchInput, offset: offSet, count: self.PackagePageSize(), originFilter: 'Any', sort: 'Score', order: 'Ascending', includePrerelease: true },
            dataType: 'json',
            cache: false
        }).then(function (data) {
            self.PackagePackageHits(data.totalHits);
            ko.mapping.fromJS(data.hits, LucenePackage.mapping, self.Feed().Packages);
            self.PackagesLoading(false);
            self.ErrorLoadingPackages(false);

            if (data.totalHits == 0 && self.SearchForPackagesValue().length > 0) {
                self.GetMatchingPackages();
            }
        }).fail(function (response) {
            self.PackagesLoading(false);
            self.ErrorLoadingPackages(true);
        });
    };

    ctor.prototype.SearchForPackages = function (viewModel) {
        var self = viewModel || this;
        self.LoadPackagesFromFeed(1, self.SearchForPackagesValue());
    };

    ctor.prototype.SearchForSuggestion = function (value) {
        var self = this;
        self.SearchForPackagesValue(value);
        self.SearchForPackages();
    }


    return ctor;
});