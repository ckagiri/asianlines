Main.module('Entities', function (Entities, App, Backbone, Marionette, $, _) {
    Entities.Team = Entities.Model.extend({
        urlRoot: "/api/teams",

        defaults: {
            name: '',
            code: '',
            homeGround: ''
        }
    });

    //Entities.configureStorage(Entities.Team);

    Entities.TeamCollection = Backbone.Collection.extend({
        url: "/api/teams",
        model: Entities.Team,
        comparator: "name"
    });
    
    //Entities.configureStorage(Entities.TeamCollection);

    var initializeTeams = function () {
        var teams = new Entities.TeamCollection([
            { id: 1, name: 'Chelsea', code: 'CHE', homeGround: 'Stamford Bridge' },
            { id: 2, name: 'Manchester United', code: 'MANU', homeGround: 'Old Trafford' },
            { id: 3, name: 'Manchester City', code: 'MANC', homeGround: 'Etihad Stadium' }
        ]);
        teams.forEach(function (team) {
            team.save();
        });
        return teams.models;
    };
    var teams;
    var API = {
        getTeams: function () {
            var defer = $.Deferred();
            if (!teams) {
                teams = new Entities.TeamCollection();
                teams.fetch({
                    success: function (data) {
                        defer.resolve(data);
                    },
                    error: function (response) {
                        defer.resolve(undefined);
                    }
                });
            }
            var promise = defer.promise();
            $.when(promise).done(function () {
                if (teams.length === 0) {
                    var models = initializeTeams();
                    teams.reset(models);
                }
            });
            return teams;
        },
        getTeam: function (id) {
            var team = new Entities.Team({ id: id });
            team.fetch();
            return team;
        },
        newTeam: function () {
            var team = new Entities.Team();
            return team;
        }
    };

    App.reqres.setHandler("team:entities", function () {
        return API.getTeams();
    });

    App.reqres.setHandler("team:entity", function (id) {
        return API.getTeam(id);
    });

    App.reqres.setHandler("new:team:entity", function () {
        return API.newTeam();
    });
});