Main.module('Controllers', function (Controllers, App, Backbone, Marionette, $, _) {
    Controllers.Base = Marionette.Controller.extend({
        constructor: function (options) {
            if (options == null) {
                options = {};
            }
            this.region = options.region || App.request("default:region");
            this._instance_id = _.uniqueId("controller");
            App.execute("register:instance", this, this._instance_id);
            Marionette.Controller.prototype.constructor.call(this, options);
        },
        
        close: function () {
            var args = Array.prototype.slice.apply(arguments);
            //delete this.region;
            //delete this.options;
            App.execute("unregister:instance", this, this._instance_id);
            Backbone.Marionette.Controller.prototype.close.apply(this, args);
        },
        
        onClose: function () {
            console.info("controller closing",this);
        },
        
        show: function (view, options) {
            if (options == null) {
                options = {};
            }
            _.defaults(options, {
                loading: false,
                region: this.region
            });
            
            this._setMainView(view);
            this._manageView(view, options);
        },
        
        _setMainView: function (view) {
            if (this._mainView) {
                return void 0;
            }
            this._mainView = view;
            return this.listenTo(view, "close", this.close);
        },
        
        _manageView: function (view, options) {
            if (options.loading) {
                App.execute("show:loading", view, options);
            } else {
                options.region.show(view);
            }
        }
    });
});
