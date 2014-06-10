﻿define(['plugins/router', 'durandal/app', 'viewmodels/shell'], function (router, app, shell) {
    
    var ctor = function () {
        this.DisplayName = ko.observable('View Feeds');
        this.Feeds = ko.mapping.fromJS([]);
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
        }).then(function(data) {
            ko.mapping.fromJS(data, ctor.mapping, self.Feeds);
        });
    };

    return ctor;
});