Main.module('AboutApp.Show', function (Show, App, Backbone, Marionette, $, _) {
    Show.Controller = {
        showAbout: function() {
            var view = new Show.Message();
            App.mainRegion.show(view);
        }
    };
});