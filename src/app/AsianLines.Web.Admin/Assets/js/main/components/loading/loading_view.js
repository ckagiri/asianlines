Main.module('Components.Loading', function (Loading, App, Backbone, Marionette, $, _) {
    Loading.LoadingView = App.Views.ItemView.extend({
        template: _.template(""),
        className: "loading-container",

        onShow: function () {
            var opts;
            opts = this._getOptions();
            this.$el.spin(opts);
        },

        onClose: function () {
            this.$el.spin(false);
        },

        _getOptions: function () {
            return {
                lines: 10,
                length: 6,
                width: 2.5,
                radius: 7,
                corners: 1,
                rotate: 9,
                direction: 1,
                color: '#000',
                speed: 1,
                trail: 60,
                shadow: false,
                hwaccel: true,
                className: 'spinner',
                zIndex: 2e9,
                top: 'auto',
                left: 'auto'
            };
        }
    });
});
