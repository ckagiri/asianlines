Main.module('TeamsApp.List', function (List, App, Backbone, Marionette, $, _) {
    List.Layout = App.Views.Layout.extend({
        template: "#team-list-layout",
        regions: {
            panelRegion: "#panel-region",
            teamsRegion: "#teams-region",
            paginationRegion: "#pagination-region"
        }
    });

    List.Panel = App.Views.ItemView.extend({
        template: "#team-list-panel",
        triggers: {
            "click button.js-new-team": "new:team:button:clicked"
        },
    });

    List.Team = App.Views.ItemView.extend({
        tagName: "tr",
        template: "#team-list-item",

        triggers: {
            "click .js-delete": "team:delete:clicked",
            "click .js-edit": "team:edit:clicked",
            "click": "team:clicked"
        }
    });

    var NoTeamsView = App.Views.ItemView.extend({
        template: "#team-list-none",
        tagName: "tr",
        className: "alert"
    });

    //List.Pagination = App.Views.ItemView.extend({
    //    template: "#pagination-tmpl",
    //    initialize: function (options) {
    //        this.teams = options.collection;
    //    },
    //    onShow: function () {
    //        var view = new Utils.Views.Pager({
    //            el: $("#pager"),
    //            collection: this.teams
    //        });
    //        this.$el.append(view.render().$el);
    //    }
    //});

    List.Teams = App.Views.CompositeView.extend({
        tagName: "table",
        className: "table table-hover",
        template: "#team-list",
        emptyView: NoTeamsView,
        itemView: List.Team,
        itemViewContainer: "tbody"
    });
});