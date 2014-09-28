define(['plugins/router'], function (router) {
    window.LucenePackage = function (config) {
        var self = this, data;

        // your default structure goes here
        data = $.extend({
            title: ko.observable(),
            created: ko.observable(),
            downloadCount: ko.observable(),
            id: ko.observable(),
            copyright: ko.observable(),
            authors: ko.observableArray(),
            licenseUrl: ko.observable(),
            lastUpdated: ko.observable(),
            projectUrl: ko.observable(),
            path: ko.observable(),
            requireLicenseAcceptance: ko.observable(),
            summary: ko.observable(),
            tags: ko.observable(),
            version: ko.observable(),
            packageHash: ko.observable(),
            packageSize: ko.observable(),
            searchTitle: ko.observable(),
            symbolsAvailable: ko.observable(),
            iconUrl: ko.observable(""),
            isLatestVersion: ko.observable(),
            description: ko.observable(),
            published: ko.observable(),
        }, config);

        ko.mapping.fromJS(data, {}, self);


    };

    window.LucenePackage.mapping = {
        create: function (options) {
            var po = new LucenePackage(options.data);
            po.ViewUrl = ko.computed(function () {
                return '#feeds/view/' + router.activeInstruction().params[0] + "/package/" + po.id();
            });
            po.DownloadUrl = ko.computed(function () {
                return "/api/feeds/DownloadPackage?feedId=" + router.activeInstruction().params[0] + "&packageId=" + po.id() + "&version=" + po.version();
            });
            return po;
        }
    };
});