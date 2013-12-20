Main.module('Views', function (Views, App, Backbone, Marionette, $, _) {
    var _remove = Marionette.View.prototype.remove;
    
    _.extend(Marionette.View.prototype, {
        addOpacityWrapper: function (init) {
            if (init == null) {
                init = true;
            }
            return this.$el.toggleWrapper({
                className: "opacity",
                backgroundColor: "white"
            }, init);
        },
        
        setInstancePropertiesFor: function () {
            
        },
        
        remove: function () {
            var self = this;
            var model, wrapper, args = Array.prototype.slice.call(arguments);
            console.log("removing", this);
            if ((model = this.model) && typeof model.isDestroyed === "function" && model.isDestroyed()) {
                wrapper = this.$el.toggleWrapper({
                    className: "opacity",
                    backgroundColor: "red"
                });
                wrapper.fadeOut(400, function () {
                    $(this).remove();
                });
                this.$el.fadeOut(400, function () {
                    _remove.apply(self, args);
                });
            } else {
                _remove.apply(self, args);
            }
        },
        
        templateHelpers: function () {
            
        }
    });
});