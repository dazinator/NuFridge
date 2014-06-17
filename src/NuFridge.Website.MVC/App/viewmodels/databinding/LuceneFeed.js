
var LuceneFeed = function (config) {
    var self = this, data;

    // your default structure goes here
    data = $.extend({
        Name: ko.observable("").extend({ trackChange: true }),
        APIKey: ko.observable("").extend({ trackChange: true }),
        FeedURL: ko.observable(""),
        Id: ko.observable(),
        Packages: ko.observableArray()
    }, config);

    ko.mapping.fromJS(data, {}, self);


};

LuceneFeed.mapping = {
    create: function (options) {

        var fd = new LuceneFeed(options.data);

        fd.EditUrl = ko.computed(function () {
            return '#feeds/view/' + fd.Id();
        });

        return fd;
    }

};