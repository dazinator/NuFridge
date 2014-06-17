var RepositoryInfo = function (config) {
    var self = this, data;

    // your default structure goes here
    data = $.extend({
        indexingState: ko.observable(),
        synchronizationState: ko.observable(),
        packagesToIndex: ko.observable(),
        completedPackages: ko.observable(),
        totalPackages: ko.observable()
    }, config);

    ko.mapping.fromJS(data, {}, self);


};