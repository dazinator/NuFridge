define(['plugins/router', 'durandal/app', 'viewmodels/shell', 'introjs', 'plugins/cssLoader', 'viewmodels/databinding/LuceneFeed'], function (router, app, shell, introjs, cssLoader, feed) {
    
    var ctor = function () {
        var self = this;

        this.DisplayName = ko.observable('View Feeds');
        this.Feeds = ko.observableArray();
        this.Groups = ko.observableArray();
        
        this.IsLoadingFeeds = ko.observable(false);
        this.LoadError = ko.observable(false);
        this.ErrorMessage = ko.observable();
        this.ExceptionMessage = ko.observable();

        this.ShowNoFeedsFound = ko.computed(function() {
            return self.IsLoadingFeeds() == false && self.LoadError() == false && (self.Feeds() == null || self.Feeds().length <= 0);
        });
        
        this.ShowError = ko.computed(function () {
            return self.LoadError() == true && self.IsLoadingFeeds() == false;
        });

        this.FeedsInGroup = function (groupName) {
            var groupedFeeds = [];
            ko.utils.arrayForEach(self.Feeds(), function (feed) {
                if (feed.GroupName() == groupName) {
                    groupedFeeds.push(feed);
                }
            });
            var addNewFeedButton = new window.LuceneFeed();
            addNewFeedButton.Name("Create Feed");
            addNewFeedButton.EditUrl = '#feeds/create/' + groupName;
            addNewFeedButton.IsAddNewButton(true);
            groupedFeeds.push(addNewFeedButton);
            return groupedFeeds;
        };
    };

    ctor.prototype.compositionComplete = function () {
        if (this.ShowNoFeedsFound() == true) {
            cssLoader.loadCss("viewfeeds", "../../Content/introjs.css");
            introjs().start();
        }
    };



    ctor.prototype.AddFeed = function () {
        shell.ShowPageTitle(false);
        router.navigate('#feeds/create');
    };

    ctor.prototype.deactivate = function () {
        if (this.ShowNoFeedsFound() == true) {
            introjs().exit();
        }

        cssLoader.removeModuleCss("viewfeeds");
    };

    ctor.prototype.activate = function() {
        shell.ShowNavigation(true);
        var self = this;
        shell.ShowNavigation(true);
        shell.ShowPageTitle(false);
        self.IsLoadingFeeds(true);

        $.ajax({
            url: "/api/feeds/GetAllFeeds",
            cache: false,
            dataType: 'json'
        }).then(function (data) {
            self.IsLoadingFeeds(false);
            self.LoadError(false);
            ko.mapping.fromJS(data, LuceneFeed.mapping, self.Feeds);

            ko.utils.arrayForEach(self.Feeds(), function (feed) {
                if (self.Groups.indexOf(feed.GroupName()) < 0) {
                    self.Groups.push(feed.GroupName());
                }
            });

            self.Groups.sort(function (l, r) {
                return l > r;
            });

        }).fail(function (response) {
            var responseText = JSON.parse(response.responseText);

            if (responseText.ExceptionMessage != null) {
                self.ErrorMessage(responseText.Message);
                self.ExceptionMessage(responseText.ExceptionMessage);
            } else {
                self.ErrorMessage("There was a problem loading the feeds.");
                self.ExceptionMessage(responseText.Message);
            }
            
            self.IsLoadingFeeds(false);
            self.LoadError(true);
        });
    };

    return ctor;
});