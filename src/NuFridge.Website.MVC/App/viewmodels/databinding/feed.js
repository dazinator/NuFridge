var FeedObject = function (config) {
    var self = this, data;

    // your default structure goes here
    data = $.extend({
        Name: ko.observable("").extend({ trackChange: true }),
        APIKey: ko.observable("").extend({ trackChange: true }),
        FeedURL: ko.observable(""),
        Id: ko.observable()
    }, config);

    ko.mapping.fromJS(data, {}, self);


};