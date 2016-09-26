var myApp = angular.module("CarFinder", ["ui.bootstrap", "trNgGrid"]);

myApp.controller("CarController", ["$http", "$uibModal", function ($http, $uibModal) {

    var scope = this;
    scope.rowLimit = 10;
    scope.years = [];
    scope.makes = [];
    scope.models = [];
    scope.trims = [];
    scope.cars = [];
    scope.info = [];
    scope.id = {
        id: ''
    }
    scope.options =
        {
            year: '',
            make: '',
            model: '',
            trim: ''
        }
    scope.getYears = function () {
        $http.get("api/CarFind/years").then(function (response) {
            scope.years = response.data;
        });
    }
    scope.getYears();

    scope.getMakes = function () {
        scope.options.make = "";
        scope.options.model = "";
        scope.options.trim = "";
        scope.cars = [];

        $http.get("api/CarFind/makes", { params: { year: scope.options.year } }).then(function (response) {
            scope.makes = response.data;
            scope.getCars();
        });
    }
    scope.getModels = function () {
        scope.options.model = "";
        scope.options.trim = "";
        scope.cars = [];

        $http.get("api/CarFind/models", { params: { year: scope.options.year, make: scope.options.make } }).then(function (response) {
            scope.models = response.data;
            scope.getCars();
        });
    }
    scope.getTrims = function () {
        scope.options.trim = "";
        scope.cars = [];

        $http.get("api/CarFind/trims", { params: { year: scope.options.year, make: scope.options.make, model: scope.options.model } }).then(function (response) {
            scope.trims = response.data;
            scope.getCars();
        });
    }
    scope.getCars = function () {
        $http.get("api/CarFind/getCars", { params: { year: scope.options.year, make: scope.options.make, model: scope.options.model, trim: scope.options.trim } }).then(function (response) {
            scope.cars = response.data;
        });
    }

   // //scope.getInfo = function (id) {
   // //    scope.id.id = id;
   // //    $http.get("http://RiaCar.azurewebsites.net/api/cars/GetInfo", { params: { id: scope.id.id } }).then(function (response) {           
   // //      scope.info = response.data;
   // //    });
   //}
    scope.open = function (id) {
        scope.id.id = id;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: "carInfo.html",
            controller: "infoController as cm",
            windowClass: 'center-modal',
            size: "lg",
            resolve: {
                car: function () {
                    return $http.get("api/CarFind/getData", { params: { id: scope.id.id } });
                }
            }
        });
    }



}]);

myApp.controller("infoController", function ($uibModalInstance, car) {
    var self = this;
    self.car = car.data;

    self.ok = function () {
        $uibModalInstance.close();
    };
    self.cancel = function () {
        $uibModalInstance.dismiss();
    }
});
