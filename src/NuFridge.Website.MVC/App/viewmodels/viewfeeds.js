define(['plugins/router', 'durandal/app', 'viewmodels/shell'], function (router, app, shell) {
    
    var ctor = function () {
        var self = this;

        this.DisplayName = ko.observable('View Feeds');
        this.Feeds = ko.observable();
        
        this.IsLoadingFeeds = ko.observable(false);
        this.LoadError = ko.observable(false);

        this.ShowNoFeedsFound = ko.computed(function() {
            return self.IsLoadingFeeds() == false && self.LoadError() == false && (self.Feeds() == null || self.Feeds().length <= 0);
        });
        
        this.ShowError = ko.computed(function () {
            return self.LoadError() == true && self.IsLoadingFeeds() == false;
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

        self.IsLoadingFeeds(true);

        $.ajax({
            url: "/api/feeds/GetAllFeeds",
            cache: false,
            dataType: 'json'
        }).then(function (data) {
            self.IsLoadingFeeds(false);
            self.LoadError(false);
            ko.mapping.fromJS(data, ctor.mapping, self.Feeds);
        }).fail(function (response) {
            self.IsLoadingFeeds(false);
            self.LoadError(true);
        });
    };

    return ctor;
});