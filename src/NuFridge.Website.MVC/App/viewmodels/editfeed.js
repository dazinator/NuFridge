define(['plugins/router', 'durandal/app', 'viewmodels/shell'], function (router, app, shell) {

    var ctor = function () {
        this.FeedId = ko.observable();
        this.FeedName = ko.observable();
    };

    ctor.prototype.activate = function () {

        shell.ShowNavigation(true);

        var self = this;

        var re = new RegExp("([a-z0-9]{8}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{12})");

        var match = re.exec(document.location.href);

        if (match) {

            $.ajax({
                url: "http://localhost:34313/api/feeds/GetFeed/" + match[1],
                cache: false
            }).then(function (item) {

                self.FeedId(item.Id);
                self.FeedName(item.Name);

            });
        };


    }

    ctor.prototype.SaveChanges = function () {
        router.navigate('#feeds');
    };

    ctor.prototype.Cancel = function () {
        router.navigate('#feeds');
    }



    return ctor;



});