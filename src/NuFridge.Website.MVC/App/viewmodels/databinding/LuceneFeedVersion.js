
var LuceneFeedVersion = function (config) {
    var self = this, data;

    // your default structure goes here
    data = $.extend({
        'klondike': ko.observable(),
        'nuGetLucene': ko.observable(),
        'nuGetCore': ko.observable(),
        'luceneNetLinq': ko.observable(),
        'luceneNet': ko.observable()
    }, config);

    ko.mapping.fromJS(data, {}, self);
};

LuceneFeedVersion.mapping = {
    create: function (options) {

        var fdv = new LuceneFeedVersion(options.data);
        return fdv;
    },
    update: function (options) {
        options.target.klondike(options.data['klondike']);
        options.target.nuGetLucene(options.data['nuGet.Lucene']);
        options.target.nuGetCore(options.data['nuGet.Core']);
        options.target.luceneNetLinq(options.data['lucene.Net.Linq']);
        options.target.luceneNet(options.data['lucene.Net']);
    }

};