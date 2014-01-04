using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ligi.Core.Model
{
    public class Season
    {
        public Season()
        {
            Teams = new Collection<Team>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid LeagueId { get; set; }
        public League League { get; set; }

        public ICollection<Team> Teams { get; set; }
    }
}

