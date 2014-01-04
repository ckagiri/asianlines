Main.module('HeaderApp.List', function (List, App, Backbone, Marionette, $, _) {
    List.Controller = App.Controllers.Base.extend({
        initialize: function () {
            var headersView = this.getHeadersView();
            this.listenTo(headersView, "brand:clicked", function () {
                            App.trigger("teams:list");
                        });
            this.listenTo(headersView, "childview:navigate", function(childView, model) {
                var url = model.get('url');
                switch (url) {
                case "teams":
                    App.trigger("teams:list");
                    break;
                case "about":
                    App.trigger("about:show");
                    break;
                default:
                    throw "No such route: " + url;
                }
            });
            this.show(headersView);
        },
        getHeadersView: function () {
            var links = App.request("header:entities");
            return new List.Headers({collection: links});
        }
    });
});