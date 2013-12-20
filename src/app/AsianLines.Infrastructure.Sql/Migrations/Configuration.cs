using System.Collections.Generic;
using System.Collections.ObjectModel;
using AsianLines.Core.Model;
using AsianLines.Core.Utils;
using AsianLines.Infrastructure.Sql.Database;

namespace AsianLines.Infrastructure.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<AdminDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "AsianLines.Infrastructure.Sql.Database.AdminDbContext";
        }

        protected override void Seed(AdminDbContext context)
        {
            var leagues = AddLeagues(context);
            var seasons = AddSeasons(context, leagues);
            var teams = AddTeams(context);

            AddTeamsToSeason(context, leagues, seasons, teams);
            AddFixturesToSeason(context, leagues, seasons, teams);
        }

        private List<League> AddLeagues(AdminDbContext context)
        {
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
            context.SaveChanges();

            return leagues;
        }

        private List<Season> AddSeasons(AdminDbContext context, List<League> leagues)
        {
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

            return seasons;
        }

        private List<Team> AddTeams(AdminDbContext context)
        {
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

            return teams;
        }

        private void AddTeamsToSeason(AdminDbContext context, List<League> leagues, List<Season> seasons, List<Team> teams)
        {
            var epl = leagues.First(l => l.Code == "EPL").Id;
            var kpl = leagues.First(l => l.Code == "KPL").Id;
            var eplSeason = context.Seasons.First(s => s.LeagueId == epl && s.Name == "2013 - 2014");
            var kplSeason = context.Seasons.First(s => s.LeagueId == kpl && s.Name == "2013 - 2014");

            var teamsKpl = teams.Take(5).ToList();
            var teamsEpl = teams.Skip(5).ToList();

            teamsEpl.ForEach(t => eplSeason.Teams.Add(t));
            teamsKpl.ForEach(t => kplSeason.Teams.Add(t));

            context.SaveChanges();
        }

        private void AddFixturesToSeason(AdminDbContext context, List<League> leagues, List<Season> seasons, List<Team> teams)
        {
            var now = DateTime.Now;
            var epl = leagues.First(l => l.Code == "EPL").Id;
            var eplSeason = context.Seasons.First(s => s.LeagueId == epl && s.Name == "2013 - 2014");
            var eplTeams = teams.Skip(5).ToList();
            var day1 = new MonToSunWeek().Start().AddDays(-4).AddHours(-3);
            var day2 = new MonToSunWeek().Start().AddDays(-4);
            var day3 = now.AddHours(10);
            var day4 = now.AddHours(15);
            var day5 = now.AddDays(7);
            var day6 = now.AddDays(7).AddHours(3);
            var eplFixtures = new List<Fixture>
                                  {
                                      new Fixture
                                          {
                                              Id = Guid.NewGuid(),
                                              SeasonId = eplSeason.Id,
                                              HomeTeamId = eplTeams[0].Id,
                                              AwayTeamId = eplTeams[11].Id,
                                              Venue = eplTeams[0].HomeGround,
                                              KickOff = day1,
                                              StartOfWeek = new MonToSunWeek(day1).Start(),
                                              EndOfWeek = new MonToSunWeek(day1).End(),
                                              HomeAsianHandicap = -1,
                                              AwayAsianHandicap = 1,
                                              AsianGoalsHandicap = 3,
                                              HomeScore = 2,
                                              AwayScore = 2,
                                              MatchStatus = MatchStatus.Played,
                                              Odds = new Collection<MatchOdds>
                                                         {
                                                             new MatchOdds
                                                                 {
                                                                     OddsType = OddsType.AsianHandicap,
                                                                     Handicap = -1
                                                                 },
                                                             new MatchOdds
                                                                 {
                                                                     OddsType = OddsType.AsianGoals,
                                                                     Handicap = 3
                                                                 }
                                                         }
                                          },
                                      new Fixture
                                          {
                                              Id = Guid.NewGuid(),
                                              SeasonId = eplSeason.Id,
                                              HomeTeamId = eplTeams[4].Id,
                                              AwayTeamId = eplTeams[16].Id,
                                              Venue = eplTeams[4].HomeGround,
                                              KickOff = day2,
                                              StartOfWeek = new MonToSunWeek(day2).Start(),
                                              EndOfWeek = new MonToSunWeek(day2).End(),
                                              HomeAsianHandicap = -1.25m,
                                              AwayAsianHandicap = 1.25m,
                                              AsianGoalsHandicap = 2.5m,
                                              HomeScore = 2,
                                              AwayScore = 3,
                                              MatchStatus = MatchStatus.Played,
                                              Odds = new Collection<MatchOdds>
                                                         {
                                                             new MatchOdds
                                                                 {
                                                                     OddsType = OddsType.AsianHandicap,
                                                                     Handicap = -1.25m
                                                                 },
                                                             new MatchOdds
                                                                 {
                                                                     OddsType = OddsType.AsianGoals,
                                                                     Handicap = 2.5m
                                                                 }
                                                         }
                                          },
                                      new Fixture
                                          {
                                              Id = Guid.NewGuid(),
                                              SeasonId = eplSeason.Id,
                                              HomeTeamId = eplTeams[3].Id,
                                              AwayTeamId = eplTeams[10].Id,
                                              Venue = eplTeams[3].HomeGround,
                                              KickOff = day3,
                                              StartOfWeek = new MonToSunWeek(day3).Start(),
                                              EndOfWeek = new MonToSunWeek(day3).End(),
                                              HomeAsianHandicap = -1.5m,
                                              AwayAsianHandicap = 1.5m,
                                              AsianGoalsHandicap = 2.75m,
                                              Odds = new Collection<MatchOdds>
                                                         {
                                                             new MatchOdds
                                                                 {
                                                                     OddsType = OddsType.AsianHandicap,
                                                                     Handicap = -1.5m
                                                                 },
                                                             new MatchOdds
                                                                 {
                                                                     OddsType = OddsType.AsianGoals,
                                                                     Handicap = 2.75m
                                                                 }
                                                         }
                                          },
                                      new Fixture
                                          {
                                              Id = Guid.NewGuid(),
                                              SeasonId = eplSeason.Id,
                                              HomeTeamId = eplTeams[2].Id,
                                              AwayTeamId = eplTeams[14].Id,
                                              Venue = eplTeams[2].HomeGround,
                                              KickOff = day4,
                                              StartOfWeek = new MonToSunWeek(day4).Start(),
                                              EndOfWeek = new MonToSunWeek(day4).End(),
                                              HomeAsianHandicap = -1.75m,
                                              AwayAsianHandicap = 1.75m,
                                              AsianGoalsHandicap = 3.25m,
                                              Odds = new Collection<MatchOdds>
                                                         {
                                                             new MatchOdds
                                                                 {
                                                                     OddsType = OddsType.AsianHandicap,
                                                                     Handicap = -1.75m
                                                                 },
                                                             new MatchOdds
                                                                 {
                                                                     OddsType = OddsType.AsianGoals,
                                                                     Handicap = 3.25m
                                                                 }
                                                         }
                                          },
                                      new Fixture
                                          {
                                              Id = Guid.NewGuid(),
                                              SeasonId = eplSeason.Id,
                                              HomeTeamId = eplTeams[4].Id,
                                              AwayTeamId = eplTeams[1].Id,
                                              Venue = eplTeams[4].HomeGround,
                                              KickOff = day5,
                                              StartOfWeek = new MonToSunWeek(day5).Start(),
                                              EndOfWeek = new MonToSunWeek(day5).End(),
                                              HomeAsianHandicap = 0,
                                              AwayAsianHandicap = 0,
                                              Odds = new Collection<MatchOdds>
                                                         {
                                                             new MatchOdds
                                                                 {
                                                                     OddsType = OddsType.AsianHandicap,
                                                                     Handicap = 0
                                                                 },
                                                         }
                                          },
                                      new Fixture
                                          {
                                              Id = Guid.NewGuid(),
                                              SeasonId = eplSeason.Id,
                                              HomeTeamId = eplTeams[2].Id,
                                              AwayTeamId = eplTeams[9].Id,
                                              Venue = eplTeams[2].HomeGround,
                                              KickOff = day6.AddHours(3),
                                              StartOfWeek = new MonToSunWeek(day6).Start(),
                                              EndOfWeek = new MonToSunWeek(day6).End(),
                                              HomeAsianHandicap = -1,
                                              AwayAsianHandicap = -1,
                                          },
                                      new Fixture
                                          {
                                              Id = Guid.NewGuid(),
                                              SeasonId = eplSeason.Id,
                                              HomeTeamId = eplTeams[1].Id,
                                              AwayTeamId = eplTeams[13].Id,
                                              Venue = eplTeams[1].HomeGround,
                                              KickOff = day6.AddHours(6),
                                              StartOfWeek = new MonToSunWeek(day6).Start(),
                                              EndOfWeek = new MonToSunWeek(day6).End(),
                                              HomeAsianHandicap = -1,
                                              AwayAsianHandicap = -1,
                                              AsianGoalsHandicap = 2.5m,
                                              Odds = new Collection<MatchOdds>
                                                         {
                                                             new MatchOdds
                                                                 {
                                                                     OddsType = OddsType.AsianGoals,
                                                                     Handicap = 2.5m
                                                                 }
                                                         }
                                          }
                                  };
            eplFixtures.ForEach(f => context.Fixtures.Add(f));
            context.SaveChanges();
        }
    }
}
