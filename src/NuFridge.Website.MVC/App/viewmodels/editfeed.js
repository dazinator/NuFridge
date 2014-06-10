define(['plugins/router', 'durandal/app', 'viewmodels/shell'], function (router, app, shell) {

    var ctor = function () {
        this.Feed = ko.observable();
        this.FeedLoaded = ko.observable(false);
        this.FeedFailedToLoad = ko.observable(false);
    };

    ctor.prototype.activate = function() {

        shell.ShowNavigation(true);

        var self = this;

        var re = new RegExp("([a-z0-9]{8}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{12})");

        var match = re.exec(document.location.href);

        if (match) {

            $.ajax({
                url: "/api/feeds/GetFeed/" + match[1],
                cache: false
            }).then(function(item) {
                self.Feed(item);
                self.FeedLoaded(true);
            }).fail(function() {
                self.FeedFailedToLoad(true);
            });
        }
    };

    ctor.prototype.SaveChanges = function () {

        var self = this;

        var json = JSON.stringify(self.Feed());
        
        $.ajax({
            url: "/api/feeds/PutFeed/" + self.Feed().Id,
            type: 'PUT',
            data: json,
            dataType: 'jsonp',
            success: function (result) {
                router.navigate('#feeds');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
        

      
    };

    ctor.prototype.Cancel = function() {
        router.navigate('#feeds');
    };

    return ctor;
});