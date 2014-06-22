define(['plugins/router', 'durandal/app', 'knockout', 'viewmodels/shell', 'knockoutvalidation'], function (router, app, ko, shell, knockoutvalidation) {
    var ctor = function () {
        var self = this;
        this.ActiveStepIndex = ko.observable(1);
        this.MaxStepIndex = ko.observable(4);
        this.MinStepIndex = ko.observable(1);
        this.CanGoToPreviousStep = ko.computed(function () {
            return self.ActiveStepIndex() > self.MinStepIndex();
        });
        this.CanGoToNextStep = ko.computed(function () {
            var anotherStepAvailable = self.ActiveStepIndex() < self.MaxStepIndex();

            if (anotherStepAvailable) {
                if (self.ActiveStepIndex() == 2) {
                    return self.MongoDBConnectionSuccessful() != null && self.MongoDBConnectionSuccessful() == true;
                }
                else if (self.ActiveStepIndex() == 3) {
                    return self.IISStepIsValid() == true && ((self.IISWebsiteAlreadyExists() != null && self.IISWebsiteAlreadyExists() == true) || (self.PortNumber.isValid() && self.PhysicalDirectory.isValid()));
                }
                else {
                    return true;
                }
            }

            return false;
        });

        this.MongoDBServer = ko.observable("").extend({ required: true });
        this.MongoDBDatabase = ko.observable("").extend({ required: true });
        this.IISWebsiteName = ko.observable("").extend({required: true});
        this.PortNumber = ko.observable("80").extend({
            required: true,
            pattern: { message: 'Port number must be numeric', params: /^\d+$/ }
        });
        this.PhysicalDirectory = ko.observable("").extend({
            required: true,
            pattern: { message: 'Please provide a valid directory path', params: /^([A-Za-z]:)(\\[A-Za-z_\-\s0-9\.]+)+$/ }
        });

        this.TestingMongoDBConnection = ko.observable(false);
        this.TestingIISConfiguration = ko.observable(false);
        this.MongoDBConnectionSuccessful = ko.observable(null);
        this.IISWebsiteAlreadyExists = ko.observable(null);
        this.MongoDBDatabaseExists = ko.observable(null);

        this.MongoDBServer.subscribe(function () {
            self.MongoDBTextChanged();
        });

        this.MongoDBDatabase.subscribe(function () {
            self.MongoDBTextChanged();
        });

        this.IISWebsiteName.subscribe(function () {
            self.IISTextChanged();
        });

      

        //Shouldn't have to do this but I couldn't get the computed function to fire properly
        this.IISStepIsValid = ko.observable(false);
    };

    ctor.prototype.activate = function () {

        ko.validation.init({
            registerExtenders: true,
            insertMessages: false,
        });

        var self = this;
        shell.ShowNavigation(false);

        self.ConfigureWizard();
        self.GetData();
    };

    ctor.prototype.MongoDBTextChanged = function () {
        this.MongoDBDatabaseExists(null);
        this.MongoDBConnectionSuccessful(null);
    };

    ctor.prototype.IISTextChanged = function () {
        var self = this;
        this.IISWebsiteAlreadyExists(null);
        this.IISStepIsValid(self.IISWebsiteName.isValid() && self.PortNumber.isValid());
    };

    ctor.prototype.GetData = function () {
        var self = this;
        self.GetMongoDBServer();
        self.GetMongoDBDatabase();
    };

    ctor.prototype.GetMongoDBDatabase = function () {
        var self = this;
        $.ajax({
            url: "/api/MongoDB/GetMongoDBDatabase",
            dataType: 'json',
            cache: false
        }).then(function (data) {
            self.MongoDBDatabase(data);
        });
    };

    ctor.prototype.GetMongoDBServer = function () {
        var self = this;
        $.ajax({
            url: "/api/MongoDB/GetMongoDBServer",
            dataType: 'json',
            cache: false
        }).then(function (data) {
            self.MongoDBServer(data);
        });
    };

    ctor.prototype.ConfigureWizard = function () {
        var self = this;
        var navListItems = $('ul.setup-panel li a');
        var allWells = $('.setup-content');

        allWells.hide();

        navListItems.click(function (e) {
            e.preventDefault();
            var $target = $($(this).attr('href')),
                $item = $(this).closest('li');

            if (!$item.hasClass('disabled')) {
                navListItems.closest('li').removeClass('active');
                $item.addClass('active');
                allWells.hide();
                $target.show();
            }
        });

        $('ul.setup-panel li.active a').trigger('click');


        $('#activate-step-2').on('click', function (e) {
            $('ul.setup-panel li:eq(1)').removeClass('disabled');
            $('ul.setup-panel li a[href="#step-2"]').trigger('click');
            $(this).remove();
        })
    };

    ctor.prototype.TestIISConfiguration = function () {
        var self = this;
        self.IISWebsiteAlreadyExists(null);
        self.TestingIISConfiguration(true);

        $.ajax({
            url: '/api/website/TestWebsiteExists?name=' + self.IISWebsiteName(),
            type: 'POST',
            cache: false,
            dataType: 'json'
        }).then(function (result) {
            self.TestingIISConfiguration(false);
            self.IISWebsiteAlreadyExists(result);
        }).fail(function () {
            self.TestingIISConfiguration(false);
        });
    };

    ctor.prototype.TestMongoDBConnection = function () {
        var self = this;
        self.TestingMongoDBConnection(true);
        self.MongoDBConnectionSuccessful(null);
        self.MongoDBDatabaseExists(null);

        $.ajax({
            url: '/api/MongoDB/TestDatabaseConnection?server=' + self.MongoDBServer(),
            type: 'POST',
            cache: false,
            dataType: 'json'
        }).then(function (result) {
            self.MongoDBConnectionSuccessful(result);
            if (result == true) {
                $.ajax({
                    url: '/api/MongoDB/TestDatabaseExists?server=' + self.MongoDBServer() + '&database=' + self.MongoDBDatabase(),
                    type: 'POST',
                    cache: false,
                    dataType: 'json'
                }).then(function (result) {
                    self.TestingMongoDBConnection(false);
                    self.MongoDBDatabaseExists(result);
                }).fail(function () {
                    self.TestingMongoDBConnection(false);
                });
            }
            else {
                self.TestingMongoDBConnection(false);
            }
        }).fail(function () {
            self.TestingMongoDBConnection(false);
        });
    };

    ctor.prototype.Finish = function () {
        router.navigate("");
    };

    ctor.prototype.GoToNextStep = function () {
        var self = this;
        if (self.CanGoToNextStep()) {
            self.ActiveStepIndex(self.ActiveStepIndex() + 1);
        }
    };

    ctor.prototype.GoToPreviousStep = function () {
        var self = this;
        if (self.CanGoToPreviousStep()) {
            self.ActiveStepIndex(self.ActiveStepIndex() - 1);
        }
    };


    return ctor;
});