///<reference path="../Scripts/jasmine.js"/>
///<reference path="../ViewModels/ExampleModel.js"/>
///<reference path="../../NuFridge.Website.MVC/Scripts/knockout-2.3.0.js"/>

describe("ExampleModel", function () {
    it("Can get fullname", function() {

        var model = new ExampleViewModel();
        model.firstName("Jack");
        model.lastName("Daniels");

        expect(model.fullName()).toBe("Jack Daniels");

    });
})