///<reference path="../../NuFridge.Website.MVC/Scripts/knockout-3.1.0.js"/>

function ExampleViewModel() {
    var self = this;
    self.firstName = ko.observable("");
    self.lastName = ko.observable("");
    self.fullName = ko.computed(function () {
        return self.firstName() + " " + self.lastName();
    }, self);
};