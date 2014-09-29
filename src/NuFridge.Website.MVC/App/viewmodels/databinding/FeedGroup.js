define(['knockoutvalidation'], function (validation) {

    window.FeedGroup = function (config) {
        var self = this, data;

        // your default structure goes here
        data = $.extend({
            Name: ko.observable("").extend({
                required: true,
                minLength: 4,
                maxLength: 64,
                pattern: { message: 'Only alphanumeric characters are allowed in the feed group name', params: /^[A-Za-z\d\s]+$/ }
            }),
            Id: ko.observable()
        }, config);

        ko.mapping.fromJS(data, {}, self);
    };

    window.FeedGroup.mapping = {
        create: function (options) {
            var fd = new FeedGroup(options.data);
            return fd;
        }
    };
});