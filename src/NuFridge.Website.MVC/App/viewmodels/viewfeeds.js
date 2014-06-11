define(['plugins/router', 'durandal/app', 'viewmodels/shell'], function (router, app, shell) {
    
    var ctor = function () {
        var self = this;

        this.DisplayName = ko.observable('View Feeds');
        this.Feeds = ko.mapping.fromJS([]);
        
        this.IsLoadingFeeds = ko.observable(true);
        this.Error = ko.observable(false);

        this.ShowNoFeedsFound = ko.computed(function() {
            return this.Feeds != null && this.Feeds.length <= 0 && !this.IsLoadingFeed && !this.Error;
        });
        
        this.ShowFeedsLoading = ko.computed(function () {
            return this.IsLoadingFeeds && !this.Error();
        });

        this.ShowError = ko.computed (function() {
            return this.Error() && !this.IsLoadingFeeds;
        });
    };

    ctor.mapping = {
        create: function (options) {
            var vm = ko.mapping.fromJS(options.data);

            vm.EditUrl = ko.computed(function () {
                return '#feeds/edit/' + vm.Id();
            });

            return vm;
        }
    };

    ctor.prototype.AddFeed = function() {
        router.navigate('#feeds/create');
    };

    ctor.prototype.activate = function() {
        shell.ShowNavigation(true);
        var self = this;

        $.ajax({
            url: "/api/feeds/GetAllFeeds",
            cache: false,
            dataType: 'json'
        }).then(function (data) {
            self.IsLoadingFeeds(false);
            self.Error(false);
            ko.mapping.fromJS(data, ctor.mapping, self.Feeds);
        }).fail(function () {
            self.IsLoadingFeeds(false);
            self.Error(true);
        });
    };

    return ctor;
});