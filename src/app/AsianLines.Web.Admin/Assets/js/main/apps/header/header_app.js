Main.module('HeaderApp', function(Header, App, Backbone, Marionette, $, _) {
    this.startWithParent = false;

    var API = {
        list: function() {
            return new Header.List.Controller({
                region: App.headerRegion
            });
        }
    };

    Header.on("start", function() {
        API.list();
    });

    App.commands.setHandler("set:header:active", function(headerUrl) {
        var links = App.request("header:entities");
        var headerToSelect = links.find(function(header) {
            return header.get('url') === headerUrl;
        });
        headerToSelect.select();
        links.trigger("reset");
    });
});