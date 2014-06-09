define(['plugins/router', 'durandal/app'], function (router, app) {
    
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

    ctor.prototype.activate = function () {
        var self = this;

        $.ajax({
            url: "http://localhost:34313/api/feeds/GetAllFeeds",
            cache: false,
            dataType: 'json'
        }).then(function (data) {
            ko.mapping.fromJS(data, ctor.mapping, self.Feeds);
        });

    }
    return ctor;
});