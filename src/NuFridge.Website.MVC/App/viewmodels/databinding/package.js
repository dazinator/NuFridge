    var PackageObject = function (config) {
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
            published: ko.observable()
        }, config);

        ko.mapping.fromJS(data, {}, self);
    };