Main = (function (Backbone, Marionette) {

    var App = new Marionette.Application();
    
    App.on("initialize:before", function (options) {
        // App.environment = options.environment;
        App.environment = "development";
    });

    App.addRegions({
        headerRegion: "#header-region",
        mainRegion: "#main-region",
    });

    App.rootRoute = "teams";

    App.addInitializer(function () {
        App.module("HeaderApp").start();
        //App.module("FooterApp").start();
    });
    
    App.reqres.setHandler("default:region", function () {
        return App.mainRegion;
    });
    
    App.commands.setHandler("register:instance", function (instance, id) {
        if (App.environment === "development") {
            App.register(instance, id);
        }
    });

    App.commands.setHandler("unregister:instance", function (instance, id) {
        if (App.environment === "development") {
            App.unregister(instance, id);
        }
    });

    App.on("initialize:after", function () {
        this.startHistory();
        if (this.getCurrentRoute() == null || this.getCurrentRoute() === "") {
            this.navigate(this.rootRoute, {
                trigger: true
            });
        }
    });

    return App;
})(Backbone, Marionette);