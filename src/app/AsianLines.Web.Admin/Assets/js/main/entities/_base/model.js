Main.module('Entities', function (Entities, App, Backbone, Marionette, $, _) {
    Entities.Model = Backbone.Model.extend({
        destroy: function(options) {
            if (options == null) {
                options = {};
            }
            _.defaults(options, {
                wait: true
            });
            this.set({
                _destroy: true
            });
            return Backbone.Model.prototype.destroy.call(this, options);
        },

        isDestroyed: function () {
            return this.get("_destroy");
        },
        
        save: function (data, options) {
            var isNew;
            if (options == null) {
                options = {};
            }
            isNew = this.isNew();
            _.defaults(options, {
                wait: true,
                success: _.bind(this.saveSuccess, this, isNew, options.collection),
                error: _.bind(this.saveError, this)
            });
            this.unset("_errors");
            Backbone.Model.prototype.save.call(this, data, options);
        },
        
        saveSuccess: function (isNew, collection) {
            console.info("success", this, isNew);
            if (isNew) { // model is being created
                if (collection) {
                    collection.add(this);
                }
                if (collection) {
                    collection.trigger("model:created", this);
                }
                this.trigger("created", this);
            } else { // model is being updated
                // if model has collection property defined, use that if no collection option exists
                if (collection == null) {
                    collection = this.collection;
                }
                if (collection) {
                    collection.trigger("model:updated", this);
                }
                this.trigger("updated", this);
            }
        },

        saveError: function (model, xhr, options) {
            var response;
            console.warn(xhr, model);
            if (!(xhr.status === 500 || xhr.status === 404)) {
                response = $.parseJSON(xhr.responseText);
                if (response) {
                    this.set({ _errors: response.errors });
                }
            }
        }
    });
});