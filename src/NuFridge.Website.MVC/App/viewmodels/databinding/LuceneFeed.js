﻿define(['knockoutvalidation'], function (validation) {
    window.LuceneFeed = function (config) {
        var self = this, data;

        // your default structure goes here
        data = $.extend({
            Name: ko.observable("").extend({
                required: true,
                minLength: 4,
                maxLength: 64,
                pattern: { message: 'Only alphanumeric characters are allowed in the feed name', params: /^[A-Za-z\d\s]+$/ }
            }),
            APIKey: ko.observable(""),
            FeedURL: ko.observable(""),
            Id: ko.observable(),
            Packages: ko.observableArray()
        }, config);

        ko.mapping.fromJS(data, {}, self);


    };

    window.LuceneFeed.mapping = {
        create: function (options) {

            var fd = new LuceneFeed(options.data);

            fd.EditUrl = ko.computed(function () {
                return '#feeds/view/' + fd.Id();
            });

            return fd;
        }

    };
});