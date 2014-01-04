Main.module('Components.Loading', function (Loading, App, Backbone, Marionette, $, _) {
    Loading.LoadingController = App.Controllers.Base.extend({
        initialize: function(options) {
            var config, loadingView = {}, view;
            view = options.view, config = options.config;
            config = _.isBoolean(config) ? {} : config;
            _.defaults(config, {
                loadingType: "spinner",
                entities: this.getEntities(view),
                debug: false
            });
            
            switch (config.loadingType) {
                case "opacity":
                    this.region.currentView.$el.css("opacity", 0.5);
                    break;
                case "spinner":
                    loadingView = this.getLoadingView();
                    this.show(loadingView);
                    break;
                default:
                    throw new Error("Invalid loadingType");
            }

            this.showRealView(view, loadingView, config);
        },

        showRealView: function(realView, loadingView, config) {
            var _this = this;
            App.execute("when:fetched", config.entities, function() {
                console.log("fetched");
                //config.entities, function () {
                switch (config.loadingType) {
                    case "opacity":
                        _this.region.currentView.$el.removeAttr("style");
                        break;
                    case "spinner":
                        if (_this.region.currentView !== loadingView) {
                            return realView.close();
                        }
                }
                if (!config.debug) {
                    return _this.show(realView);
                }
                return void 0;
            });
        },

        getEntities: function(view) {
            return _.chain(view).pick("model", "collection").toArray().compact().value();
        },

        getLoadingView: function() {
            return new Loading.LoadingView;
        }
    });
    
    App.commands.setHandler("show:loading", function (view, options) {
        return new Loading.LoadingController({
            view: view,
            region: options.region,
            config: options.loading
        });
    });
});