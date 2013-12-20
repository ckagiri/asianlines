(function (Backbone, Marionette) {
    Marionette.Region.Dialog = Marionette.Region.extend({
        constructor: function () {
            console.log("custom dialog region");
        },
        
        onShow: function (view) {
            var options, self = this;
            this.setupBindings(view);
            options = this.getDefaultOptions(_.result(view, "dialog"));
            
            this.$el.dialog(options, {
                close: function(e, ui) {
                    self.closeDialog();
                }
            });
        },
        
        getDefaultOptions: function (options) {
            var self = this;
            if (options == null) {
                options = { };
            }
            return _.defaults(options, {
                title: "default title",
                modal: true,
                width: "auto",
                dialogClass: options.className,
                position: { 
                    my: "left bottom",
                    at: "center",
                    of: window
                }
                //buttons: [
                //    {
                //        text: options.button || "Ok",
                //        click: function() {
                //            return self.currentView.triggerMethod("dialog:button:clicked");
                //        }
                //    }
                //]
            });
        },
        
        setupBindings: function (view) {
            this.listenTo(view, "dialog:close", this.closeDialog);
            this.listenTo(view, "dialog:resize", this.resizeDialog);
            this.listenTo(view, "dialog:title", this.titleizeDialog);
        },
        
        closeDialog: function () {
            this.stopListening();
            this.close();
            this.$el.dialog("destroy");
        },
        
        resizeDialog: function () {
            console.log("resizing dialog");
            this.$el.dialog("option", {
                position: "center"
            });
        },
        
        titleizeDialog: function (title) {
            this.$el.dialog("option", {
                title: title
            });
        }
    });
})(Backbone, Marionette)