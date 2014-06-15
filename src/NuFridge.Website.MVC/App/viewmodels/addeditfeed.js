define(['plugins/router', 'durandal/app', 'viewmodels/shell', 'plugins/dialog'], function (router, app, shell, dialog) {

    var ctor = function () {
        var self = this;

        this.Feed = {};
        this.Feed.Name = ko.observable().extend({ trackChange: true });
        this.Feed.Id = ko.observable();
        this.Feed.APIKey = ko.observable().extend({ trackChange: true });
        this.ShowDeleteButton = ko.observable(true);
        this.EditFeedTitle = ko.observable();
        this.IsEditMode = ko.observable(false);

        this.IsFormDirty = ko.computed(function () {
            return (self.Feed.Name() != null && self.Feed.Name.isDirty() == true) || (self.Feed.APIKey() != null && self.Feed.APIKey.isDirty() == true);
        });
    };

    ctor.prototype.activate = function() {

        shell.ShowNavigation(true);
        shell.ShowPageTitle(false);

        $('#viewFeedTabs').tab();

        var self = this;

        var re = new RegExp("([a-z0-9]{8}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{12})");

        var match = re.exec(document.location.href);

        if (match) {
            $.ajax({
                url: "/api/feeds/GetFeed/" + match[1],
                cache: false
            }).then(function (item) {
                //TODO remove with ko mapping
                self.Feed.Name(item.Name);
                self.EditFeedTitle(self.Feed.Name());
                self.Feed.Id(item.Id);
                self.Feed.APIKey(item.APIKey);

                self.Feed.Name.isDirty(false);
                self.Feed.APIKey.isDirty(false);
                self.IsEditMode(true);
            }).fail(function () {
                self.ShowDeleteButton(false);
                alert("An error occurred loading the feed.");
            });
        } else {
            self.ShowDeleteButton(false);
            self.EditFeedTitle = "Create Feed";
            self.IsEditMode(false);
        }
    };

    ctor.prototype.Delete = function() {
        var self = this;

        var result = self.ConfirmDeleteMessage().then(function (data) {
            if (data == "Yes") {
                $.ajax({
                    type: 'DELETE',
                    url: "/api/feeds/DeleteFeed/" + self.Feed.Id(),
                    cache: false
                }).then(function (item) {
                    router.navigate('#feeds');
                }).fail(function (data) {
                    self.ShowDeleteButton(false);
                    alert("An error occurred deleting the feed.");
                    router.navigate('#feeds');
                });
            }
        });
    };

    ctor.prototype.ConfirmDeleteMessage = function () {
        return app.showMessage('Are you sure you want to delete this feed? You will lose all packages stored in this feed.', 'Delete Feed', ['No', 'Yes']);
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