Main.module('HeaderApp.List', function (List, App, Backbone, Marionette, $, _) {
    List.Controller = {
        listHeader: function () {
            var links = App.request("header:entities");
            var headers = new List.Headers({ collection: links });

            headers.on("brand:clicked", function () {
                App.trigger("teams:list");
            });
            headers.on("itemview:navigate", function (childView, model) {
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
            App.headerRegion.show(headers);
        },
        setActiveHeader: function (headerUrl) {
            var links = App.request("header:entities");
            var headerToSelect = links.find(function (header) {
                return header.get('url') === headerUrl;
            });
            headerToSelect.select();
            links.trigger("reset");
        }
    };
});