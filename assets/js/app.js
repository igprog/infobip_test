/*!
app.js
(c) 2020 Igor Gasparovic
*/
angular.module('app', ['ui.router'])
.config(['$stateProvider', '$urlRouterProvider', '$httpProvider', '$locationProvider', ($stateProvider, $urlRouterProvider, $httpProvider, $locationProvider) => {

    $stateProvider
        .state('ridesharing', {
            url: '/ridesharing', templateUrl: './assets/partials/ridesharing.html', controller: 'ridesharingCtrl'
        })
        .state('travelplan', {
            url: '/travelplan/:id', templateUrl: './assets/partials/travelplan.html', controller: 'travelplanCtrl'
        })
        .state('cars', {
            url: '/cars', templateUrl: './assets/partials/cars.html', controller: 'carsCtrl'
        })
        .state('employees', {
            url: '/employees', templateUrl: './assets/partials/employees.html', controller: 'employeesCtrl'
        })

    $urlRouterProvider.otherwise("/");
    $locationProvider.hashPrefix('');

    //*******************disable catche**********************
    if (!$httpProvider.defaults.headers.get) {
        $httpProvider.defaults.headers.get = {};
    }
    $httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
    $httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
    $httpProvider.defaults.headers.get['Pragma'] = 'no-cache';
    //*******************************************************
}])

.factory('f', ['$http', ($http) => {
    return {
        post: (service, method, data) => {
            return $http({
                url: '../' + service + '.asmx/' + method, method: 'POST', data: data
            }).then((response) => {
                return JSON.parse(response.data.d);
            },(response) => {
                return response.data.d;
            });
        }
    }
}])

.controller('appCtrl', ['$scope', '$http', 'f', '$state', function ($scope, $http, f, $state) {

    $scope.d = null;

    $scope.go = (x) => {
        $state.go(x);
    }
    $state.go('ridesharing');

    $scope.goPlan = (id) => {
        $state.go('travelplan', { id: id });
    }

}])

.controller('ridesharingCtrl', ['$scope', '$http', 'f', function ($scope, $http, f) {
    var service = 'TravelPlan';
    var load = () => {
        f.post(service, 'Load', {}).then((d) => {
            $scope.d = d;
        });
    }
    load();

    $scope.save = (x) => {
        f.post(service, 'Save', { x: x }).then((d) => {
            load();
        });
    }

    $scope.remove = (id) => {
        if (confirm('Delete travel plan?')) {
            f.post(service, 'Delete', { id: id }).then((d) => {
                load();
            });
        }
    }

}])

.controller('travelplanCtrl', ['$scope', '$http', 'f', '$stateParams', function ($scope, $http, f, $stateParams) {
    var service = 'TravelPlan';
    $scope.employees = [];
    $scope.availableCars = [];

    var getEmployees = () => {
        f.post('Employees', 'Load', {}).then((d) => {
            $scope.employees = d;
            angular.forEach($scope.employees, function (value, key) {
                value.isSelected = $scope.d.employees.find(a => a.id === value.id) !== undefined ? true : false;
            });
        });
    }

    var get = (id) => {
        f.post(service, 'Get', { id: id }).then((d) => {
            $scope.d = d;
            $scope.d.startDate = new Date(d.startDate);
            $scope.d.endDate = new Date(d.endDate);
            getEmployees();
        });
    }
    get($stateParams.id);

    $scope.getAvailableCars = (x) => {
        f.post('Cars', 'GetAvailableCars', { x: x }).then((d) => {
            $scope.availableCars = d;
        });
    }

    $scope.save = (x, employees) => {
        x.employees = [];
        angular.forEach(employees, function (value, key) {
            if (value.isSelected) {
                x.employees.push(value);
            }
        });

        /***** Check available seats *****/
        if (x.car.seats < x.employees.length) {
			alert("Number of employees in one car cannot be more than available seats.");
            return false;
        }

        /***** Check driver's license *****/
        if (x.employees.filter(a => a.isDriver).length === 0) {
            alert("At least one traveler must have a driver's license.");
            return false;
        }

        f.post(service, 'Save', { x: x }).then((d) => {
            load();
        });
    }

    $scope.remove = (id) => {
        if (confirm('Delete travel plan?')) {
            f.post(service, 'Delete', { id: id }).then((d) => {
                load();
            });
        }
    }

}])

.controller('carsCtrl', ['$scope', '$http', 'f', function ($scope, $http, f) {
    var service = 'Cars';
    var load = () => {
        f.post(service, 'Load', {}).then((d) => {
            $scope.d = d;
        });
    }
    load();

}])

.controller('employeesCtrl', ['$scope', '$http', 'f', function ($scope, $http, f) {
    var service = 'Employees';
    var load = () => {
        f.post(service, 'Load', {}).then((d) => {
            $scope.d = d;
        });
    }
    load();

}])


/********** Directives **********/
.directive('jsonDirective', function () {
    return {
        restrict: 'E',
        scope: { data: '=', desc: '=' },
        templateUrl: '../assets/partials/directive/djson.html',
        controller: 'jsonCtrl'
    };
})
.controller('jsonCtrl', ['$scope', function ($scope) {
    $scope.isShow = false;
    $scope.show = function () {
        $scope.isShow = !$scope.isShow;
    }
}])
/********** Directives **********/

;