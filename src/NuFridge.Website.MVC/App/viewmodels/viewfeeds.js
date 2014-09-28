﻿define(['plugins/router', 'durandal/app', 'viewmodels/shell', 'introjs', 'plugins/cssLoader', 'viewmodels/databinding/LuceneFeed'], function (router, app, shell, introjs, cssLoader, feed) {
    
    var ctor = function () {
        var self = this;

        this.DisplayName = ko.observable('View Feeds');
        this.Feeds = ko.observableArray();
        
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
    };

    ctor.prototype.compositionComplete = function () {
        if (this.ShowNoFeedsFound() == true) {
            cssLoader.loadCss("viewfeeds", "../../Content/introjs.css");
            introjs().start();
        }
    };

    ctor.prototype.AddFeed = function() {
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

        self.IsLoadingFeeds(true);

        $.ajax({
            url: "/api/feeds/GetAllFeeds",
            cache: false,
            dataType: 'json'
        }).then(function (data) {
            self.IsLoadingFeeds(false);
            self.LoadError(false);
            ko.mapping.fromJS(data, LuceneFeed.mapping, self.Feeds);
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