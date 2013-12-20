Main.module('TeamsApp', function (TeamsApp, App, Backbone, Marionette, $, _) {
    TeamsApp.Router = Marionette.AppRouter.extend({
        appRoutes: {
            "teams": "list"
        }
    });

    var API = {
        list: function () {
            return new TeamsApp.List.Controller();
        }
    };

    App.on("teams:list", function () {
        App.navigate("teams");
        API.list();
    });

    App.addInitializer(function () {
        return new TeamsApp.Router({
            controller: API
        });
    });
});