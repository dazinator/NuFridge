define(['knockoutvalidation'], function (validation) {

    ko.validation.init({
        registerExtenders: true,
        insertMessages: false,
    });

    window.NuFridgeInstall = function (config) {
        var self = this, data;

        data = $.extend({
            MongoDBServer: ko.observable("").extend({ required: true }),
            MongoDBDatabase: ko.observable("").extend({ required: true }),
            IISWebsiteName: ko.observable("").extend({ required: true }),
            PortNumber: ko.observable("80").extend({
                required: true,
                pattern: { message: 'Port number must be numeric', params: /^\d+$/ }
            }),
            PhysicalDirectory: ko.observable("").extend({
                required: true,
                pattern: { message: 'Please provide a valid directory path', params: /^([A-Za-z]:)(\\[A-Za-z_\-\s0-9\.]+)+$/ }
            })
        }, config);

        ko.mapping.fromJS(data, {}, self);
    };

    window.NuFridgeInstall.mapping = {
        create: function (options) {

            var fd = new NuFridgeInstall(options.data);

            return fd;
        }

    };
});