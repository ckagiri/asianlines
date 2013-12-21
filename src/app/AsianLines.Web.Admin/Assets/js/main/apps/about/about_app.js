Main.module('AboutApp', function (AboutApp, App, Backbone, Marionette, $, _) {
    AboutApp.Router = Marionette.AppRouter.extend({
        appRoutes: {
            "about" : "show"
        }
    });

    var API = {
        show: function () {
            return new AboutApp.Show.Controller();
        }
    };

    App.on("about:show", function() {
        App.navigate("about");
        API.show();
    });

    App.addInitializer(function() {
        return new AboutApp.Router({
            controller: API
        });
    });
});