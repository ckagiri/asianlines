Main.module('TeamsApp.List', function (List, App, Backbone, Marionette, $, _) {
    List.Controller = App.Controllers.Base.extend({
        initialize: function () {
            var self = this,
                teams = App.request("team:entities");
            App.execute("set:header:active", "teams");
            App.execute("when:fetched", [teams], function () {
                var x = teams;
                teams.reset(teams.sortBy("name"));
            });
            this.layout = this.getLayoutView();
            this.listenTo(this.layout, "show", function () {
                self.panelRegion();
                self.teamsRegion(teams);
                self.paginationRegion(teams);
            });
            this.show(this.layout, {
                loading: {
                    entities: teams,
                    debug: false
                }
            });
        },
        onClose: function () {
            console.info("closing controller");
        },
        paginationRegion: function (teams) {
            //var paginationView;
            //paginationView = this.getPaginationView(teams);
            //this.layout.paginationRegion.show(paginationView);
        },
        panelRegion: function () {
            var self = this;
            var panelView = this.getPanelView();
            this.listenTo(panelView, "new:team:button:clicked", function () {
                self.newRegion();
            });
            this.layout.panelRegion.show(panelView);
        },
        
        teamsRegion: function (teams) {
            var teamsView = this.getTeamsView(teams);
            this.listenTo(teamsView, "childview:team:clicked", function (childview, args) {
                App.vent.trigger("team:clicked", args.model);
            });

            this.listenTo(teamsView, "childview:team:edit:clicked", function (childview, args) {
                App.vent.trigger("team:clicked", args.model);
            });

            this.listenTo(teamsView, "childview:team:delete:clicked", function (childview, args) {
                var model = args.model;
                if (confirm("Are you sure you want to delete " + model.get('name') + " ?")) {
                    return model.destroy();
                } else {
                    return false;
                }
            });
            this.layout.teamsRegion.show(teamsView);
        },
       
        getPaginationView: function (teams) {
            return new List.Pagination({
                collection: teams
            });
        },
        getTeamsView: function (teams) {
            return new List.Teams({ collection: teams });
        },
        getPanelView: function () {
            return new List.Panel();
        },
        getLayoutView: function () {
            return new List.Layout();
        }
    });
})