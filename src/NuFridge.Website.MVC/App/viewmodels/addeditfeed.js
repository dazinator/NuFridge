define(['plugins/router', 'durandal/app', 'viewmodels/shell'], function (router, app, shell) {

    var ctor = function () {
        this.Feed = {};
        this.Feed.Name = ko.observable();
        this.Feed.Id = ko.observable();
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
                self.Feed.Name(item.Name);
                self.Feed.Id(item.Id);
            }).fail(function() {
                alert("An error occurred loading the feed.");
            });
        }
    };

    ctor.prototype.SaveChanges = function () {
        var self = this;

        if (this.Feed.Id() != null) {
            $.ajax({
                url: "/api/feeds/PutFeed/" + self.Feed.Id(),
                type: 'PUT',
                data: self.Feed,
                dataType: 'json',
                cache: false,
                success: function (result) {
                    router.navigate('#feeds');
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                }
            });
        } else {
            var json = JSON.stringify(ko.mapping.toJS(self.Feed));

            $.ajax({
                url: "/api/feeds/PostFeed",
                type: 'POST',
                cache: false,
                dataType: 'json',
                data: self.Feed,
                success: function (result) {
                    router.navigate('#feeds');
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                }
            });
        }
    };

    ctor.prototype.Cancel = function() {
        router.navigate('#feeds');
    };

    return ctor;
});