Main.module('AboutApp.Show', function (Show, App, Backbone, Marionette, $, _) {
    Show.Message = Marionette.ItemView.extend({
        template: "#about-message"
    });
});
