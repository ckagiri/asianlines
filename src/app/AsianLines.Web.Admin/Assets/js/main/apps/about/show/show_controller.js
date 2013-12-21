Main.module('AboutApp.Show', function (Show, App, Backbone, Marionette, $, _) {
    Show.Controller = App.Controllers.Base.extend({
        initialize: function () {
            App.execute("set:header:active", "about");
            var view = this.getAboutView();
            this.show(view);
        },

        getAboutView: function() {
            return new Show.About();
        }
    });
});