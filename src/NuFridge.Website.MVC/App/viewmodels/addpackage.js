define(['plugins/router', 'durandal/app', 'knockout', 'viewmodels/shell', 'viewmodels/databinding/LuceneFeed', 'plugins/dialog'], function (router, app, ko, shell, feed, dialog) {

    var ctor = function () {
        this.Feed = ko.observable(new LuceneFeed());
        this.FeedId = ko.observable();
        this.UploadMode = ko.observable(false);
        this.DownloadURLMode = ko.observable(false);
        this.UploadError = ko.observable(null);
        this.UploadingPackage = ko.observable(false);
    };

    ctor.prototype.CloseModal = function () {
        dialog.close(this);
    };

    ctor.prototype.compositionComplete = function () {
        $(document).on('change', '.btn-file :file', function () {
            var input = $(this),
                numFiles = input.get(0).files ? input.get(0).files.length : 1,
                label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
            input.trigger('fileselect', [numFiles, label]);
        });

        $('.btn-file :file').on('fileselect', function (event, numFiles, label) {

            var input = $(this).parents('.input-group').find(':text'),
                log = numFiles > 1 ? numFiles + ' files selected' : label;

            if (input.length) {
                input.val(log);
            } else {
                if (log) alert(log);
            }

        });
    };

    ctor.prototype.ActionUploadSelected = function () {
        var self = this;
        self.UploadMode(true);
        self.DownloadURLMode(false);
    };

    ctor.prototype.UploadPackage = function () {
        var self = this;

        var files = $("#packageFileUpload").get(0).files;
        var data = new FormData();
        // Add the uploaded image content to the form data collection
        if (files.length > 0) {
            data.append("package", files[0]);
        }
        else {
            return;
        }

        // Make Ajax request with the contentType = false, and procesDate = false
        var ajaxRequest = $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            url: "/api/feeds/UploadPackage?feedId=" + self.FeedId(),
            data: data
        });
        self.UploadingPackage(true);
        ajaxRequest.then(function (xhr, textStatus) {
            self.CloseModal();
            self.UploadingPackage(false);
        }).fail(function (response) {
            self.UploadingPackage(false);
            if (response.responseText != "") {
                var responseText = JSON.parse(response.responseText);
                if (responseText.ExceptionMessage) {
                    self.UploadError(responseText.ExceptionMessage);
                }
                else if (responseText.Message) {
                    self.UploadError(responseText.Message);
                }
                else {
                    self.UploadError(responseText);
                }
            }
            else {
                self.UploadError("An unknown error has occurred.");
            }

        });;
    };

    ctor.prototype.ActionDownloadSelected = function () {
        var self = this;
        self.UploadMode(false);
        self.DownloadURLMode(true);
    };

    ctor.prototype.activate = function () {
        var self = this;
        dialog.getContext().blockoutOpacity = 0.6;
        self.FeedId(router.activeInstruction().params[0]);

        $.ajax({
            url: "/api/feeds/GetFeed/" + self.FeedId(),
            cache: false
        }).then(function (data) {
            ko.mapping.fromJS(data, LuceneFeed.mapping, self.Feed);
        });
    };

    return ctor;
});