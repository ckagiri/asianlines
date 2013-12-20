Main.module('AboutApp', function (AboutApp, App, Backbone, Marionette, $, _) {
    AboutApp.Router = Marionette.AppRouter.extend({
        appRoutes: {
            "about" : "showAbout"
        }
    });

    var API = {
        showAbout: function() {
            AboutApp.Show.Controller.showAbout();
            App.HeaderApp.List.Controller.setActiveHeader("about");
        }
    };

    App.on("about:show", function() {
        App.navigate("about");
        API.showAbout();
    });

    App.addInitializer(function() {
        return new AboutApp.Router({
            controller: API
        });
    });
});