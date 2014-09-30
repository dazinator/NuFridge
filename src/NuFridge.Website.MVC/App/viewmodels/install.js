define(['plugins/router', 'durandal/app', 'knockout', 'viewmodels/shell', 'knockoutvalidation', 'viewmodels/databinding/NuFridgeInstall'], function (router, app, ko, shell, knockoutvalidation, nufridgeinstall) {
    var ctor = function () {
        var self = this;

        this.Install = ko.observable(new NuFridgeInstall());
        
        this.ActiveStepIndex = ko.observable(1);
        this.SaveError = ko.observable();
        this.IsInstalling = ko.observable(false);
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
                    return self.IISConfigurationError() == false && self.IISStepIsValid() == true && ((self.IISWebsiteAlreadyExists() != null && self.IISWebsiteAlreadyExists() == true) || (self.Install().PortNumber.isValid() && self.Install().PhysicalDirectory.isValid()));
                }
                else {
                    return true;
                }
            }

            return false;
        });

        this.TestingMongoDBConnection = ko.observable(false);
        this.TestingIISConfiguration = ko.observable(false);
        this.MongoDBConnectionSuccessful = ko.observable(null);
        this.IISWebsiteAlreadyExists = ko.observable(null);
        this.MongoDBDatabaseExists = ko.observable(null);
        this.IISConfigurationError = ko.observable(false);

        this.Install().MongoDBServer.subscribe(function () {
            self.MongoDBTextChanged();
        });

        this.Install().MongoDBDatabase.subscribe(function () {
            self.MongoDBTextChanged();
        });

        this.Install().IISWebsiteName.subscribe(function () {
            self.IISTextChanged();
        });

      

        //Shouldn't have to do this but I couldn't get the computed function to fire properly
        this.IISStepIsValid = ko.observable(false);
    };

    ctor.prototype.activate = function () {

        var self = this;
        shell.ShowNavigation(false);

        self.ConfigureWizard();
        self.GetData();
    };
    
    ctor.prototype.reportSaveError = function (response) {
        var self = this;
        if (response.responseText != "") {
            var responseText = JSON.parse(response.responseText);
            if (responseText.ExceptionMessage) {
                self.SaveError(responseText.ExceptionMessage);
            }
            else if (responseText.Message) {
                self.SaveError(responseText.Message);
            }
            else {
                self.SaveError(responseText);
            }
        }
        else {
            self.SaveError("An unknown error has occurred.");
        }
    };

    ctor.prototype.MongoDBTextChanged = function () {
        this.MongoDBDatabaseExists(null);
        this.MongoDBConnectionSuccessful(null);
    };

    ctor.prototype.IISTextChanged = function () {
        var self = this;
        this.IISWebsiteAlreadyExists(null);
        this.IISStepIsValid(self.Install().IISWebsiteName.isValid() && self.Install().PortNumber.isValid());
    };

    ctor.prototype.GetData = function () {
        var self = this;
        self.GetNuFridgeInstall();
    };

    ctor.prototype.GetNuFridgeInstall = function () {
        var self = this;
        $.ajax({
            url: "/api/installation/GetInstallation",
            dataType: 'json',
            cache: false
        }).then(function (data) {
            ko.mapping.fromJS(data, NuFridgeInstall.mapping, self.Install);
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
            url: '/api/website/TestWebsiteExists?name=' + self.Install().IISWebsiteName(),
            type: 'POST',
            cache: false,
            dataType: 'json'
        }).then(function (result) {
            self.TestingIISConfiguration(false);
            self.IISWebsiteAlreadyExists(result);
            self.IISConfigurationError(false);
        }).fail(function () {
            self.TestingIISConfiguration(false);
            self.IISConfigurationError(true);
        });
    };

    ctor.prototype.TestMongoDBConnection = function () {
        var self = this;
        self.TestingMongoDBConnection(true);
        self.MongoDBConnectionSuccessful(null);
        self.MongoDBDatabaseExists(null);

        $.ajax({
            url: '/api/MongoDB/TestDatabaseConnection?server=' + self.Install().MongoDBServer(),
            type: 'POST',
            cache: false,
            dataType: 'json'
        }).then(function (result) {
            self.MongoDBConnectionSuccessful(result);
            if (result == true) {
                $.ajax({
                    url: '/api/MongoDB/TestDatabaseExists?server=' + self.Install().MongoDBServer() + '&database=' + self.Install().MongoDBDatabase(),
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
        var self = this;
        self.IsInstalling(true);
        self.SaveError();
        $.ajax({
            url: "/api/installation/PostInstallation",
            type: 'POST',
            cache: false,
            dataType: 'json',
            data: self.Install(),
            success: function (result) {
                self.IsInstalling(false);
                shell.IsInstallationValid(null); //Force the shell to check the install status again
                router.navigate('');
            },
            error: function (XMLHttpResponse, textStatus, errorThrown) {
                self.IsInstalling(false);
                self.reportSaveError(XMLHttpResponse);
            }
        });
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