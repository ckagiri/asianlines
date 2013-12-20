using System.Collections.Generic;
using AsianLines.Core.Model;

namespace AsianLines.Infrastructure.Sql.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<Sql.Database.AdminDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "AsianLines.Infrastructure.Sql.Database.AdminDbContext";
        }

        protected override void Seed(Sql.Database.AdminDbContext context)
        {
            # region Teams
            var kplTeams = new List<Team>
                            {
                                new Team
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Gor Mahia",
                                        Code = "GOR",
                                        HomeGround = "City Stadium",
                                        Tags = "TPL|Tusker Premier League|Kenya|East Africa"
                                    },
                                new Team
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "AFC Leopards",
                                        Code = "AFC",
                                        HomeGround = "Chui Stadium",
                                        Tags = "TPL|Tusker Premier League|Kenya|East Africa"
                                    },
                                new Team
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Tusker",
                                        Code = "TUSK",
                                        HomeGround = "The Breweries",
                                        Tags = "TPL|Tusker Premier League|Kenya|East Africa"
                                    },
                                new Team
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Sofapaka",
                                        Code = "SOFP",
                                        HomeGround = "Sofapaka Stadium",
                                        Tags = "TPL|Tusker Premier League|Kenya|East Africa"
                                    },
                                new Team
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Ulinzi Stars",
                                        Code = "ULNZ",
                                        HomeGround = "Ulinzi Grounds",
                                        Tags = "TPL|Tusker Premier League|Kenya|East Africa"
                                    }
                            };

            var eplTeams = new List<Team>
                               {
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Manchester United",
                                           Code = "MANU",
                                           HomeGround = "Old Trafford",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Manchester City",
                                           Code = "MANC",
                                           HomeGround = "Etihad",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Chelsea",
                                           Code = "CHE",
                                           HomeGround = "Stamford Bridge",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Arsenal",
                                           Code = "ASNL",
                                           HomeGround = "Emirates",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Tottenham Hotspur",
                                           Code = "TOTT",
                                           HomeGround = "White Hart Lane",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Everton",
                                           Code = "EVE",
                                           HomeGround = "Goodison Park",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Liverpool",
                                           Code = "LIV",
                                           HomeGround = "Anfield",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "West Bromwich Albion",
                                           Code = "WBA",
                                           HomeGround = "The Hawthorns",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Swansea",
                                           Code = "SWA",
                                           HomeGround = "Liberty Stadium",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "West Ham United",
                                           Code = "WHU",
                                           HomeGround = "Boleyn Ground",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Norwich City",
                                           Code = "NOR",
                                           HomeGround = "Carrow Road",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Fulham",
                                           Code = "FUL",
                                           HomeGround = "Craven Cottage",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Stoke City",
                                           Code = "STO",
                                           HomeGround = "Britannia Stadium",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Southampton",
                                           Code = "SOU",
                                           HomeGround = "St. Marys",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Aston Villa",
                                           Code = "AVIL",
                                           HomeGround = "Villa Park",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Newcastle United",
                                           Code = "NUTD",
                                           HomeGround = "St. James' park",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Sunderland",
                                           Code = "SUN",
                                           HomeGround = "Stadium Of Light",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Hull City",
                                           Code = "HUC",
                                           HomeGround = "KC Stadium",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Cardiff City",
                                           Code = "CAC",
                                           HomeGround = "Cardiff City Stadium",
                                           Tags = "EPL|England|UK"
                                       },
                                   new Team
                                       {
                                           Id = Guid.NewGuid(),
                                           Name = "Crystal Palace",
                                           Code = "CRP",
                                           HomeGround = "Selhurst Park",
                                           Tags = "EPL|England|UK"
                                       },
                               };

            var teams = kplTeams.Concat(eplTeams).ToList();
            teams.ForEach(t => context.Teams.Add(t));
            context.SaveChanges();
            #endregion

            #region Leagues & Seasons
            var leagues = new List<League>
                              {
                                  new League
                                      {
                                          Id = Guid.NewGuid(),
                                          Name = "English Premier League",
                                          Code = "EPL"
                                      },
                                  new League
                                      {
                                          Id = Guid.NewGuid(),
                                          Name = "Tusker Premier League",
                                          Code = "KPL"
                                      }
                              };

            leagues.ForEach(l => context.Leagues.Add(l));

            var now = DateTime.Now;
            var eplSeasons = new List<Season>
                                 {
                                     new Season
                                         {
                                             Id = Guid.NewGuid(),
                                             LeagueId = leagues.First(l => l.Code == "EPL").Id,
                                             StartDate = now.AddDays(-14),
                                             EndDate = now.AddDays(91),
                                             Name = "2013 - 2014",
                                         },
                                     new Season
                                         {
                                             Id = Guid.NewGuid(),
                                             LeagueId = leagues.First(l => l.Code == "EPL").Id,
                                             StartDate = now.AddMonths(-24),
                                             EndDate = now.AddDays(-12),
                                             Name = "2012 - 2013",
                                         }
                                 };
            var kplSeasons = new List<Season>
                                 {
                                     new Season
                                         {
                                             Id = Guid.NewGuid(),
                                             LeagueId = leagues.First(l => l.Code == "KPL").Id,
                                             StartDate = now.AddDays(-14),
                                             EndDate = now.AddDays(21),
                                             Name = "2013 - 2014",
                                         }
                                 };

            var seasons = eplSeasons.Concat(kplSeasons).ToList();
            seasons.ForEach(s => context.Seasons.Add(s));
            context.SaveChanges();
            #endregion

            #region Season-Teams
            var epl = leagues.First(l => l.Code == "EPL").Id;
            var kpl = leagues.First(l => l.Code == "KPL").Id;
            var eplSeason = context.Seasons.First(s => s.LeagueId == epl && s.Name == "2013 - 2014");
            var kplSeason = context.Seasons.First(s => s.LeagueId == kpl && s.Name == "2013 - 2014");

            var teamsKpl = teams.Take(5).ToList();
            var teamsEpl = teams.Skip(5).ToList();
            
            teamsEpl.ForEach(t => eplSeason.Teams.Add(t));
            teamsKpl.ForEach(t => kplSeason.Teams.Add(t));

            context.SaveChanges();
            #endregion
        }
    }
}
