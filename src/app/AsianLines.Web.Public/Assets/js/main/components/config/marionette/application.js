(function (Backbone) {
    _.extend(Backbone.Marionette.Application.prototype, {
        navigate: function (route, options) {
            if (options == null) {
                options = {};
            }
            return Backbone.history.navigate(route, options);
        },
        getCurrentRoute: function () {
            var frag;
            frag = Backbone.history.fragment;
            if (_.isEmpty(frag)) {
                return null;
            } else {
                return frag;
            }
        },
        startHistory: function () {
            if (Backbone.history) {
                Backbone.history.start();
            }
        },
        register: function (instance, id) {
            if (this._registry == null) {
                this._registry = {};
            }
            this._registry[id] = instance;
        },
        unregister: function (instance, id) {
            delete this._registry[id];
        },
        resetRegistry: function () {
            var controller, key, msg, oldCount, registry;
            oldCount = this.getRegistrySize();
            registry = this._registry;
            for (key in registry) {
                controller = registry[key];
                controller.region.close();
            }
            msg = "There were " + oldCount + " controllers in the registry, there are now " + (this.getRegistrySize());
            if (this.getRegistrySize() > 0) {
                return console.warn(msg, this._registry);
            } else {
                return console.log(msg);
            }
        },
        getRegistrySize: function () {
            return _.size(this._registry);
        }
    });
})(Backbone)