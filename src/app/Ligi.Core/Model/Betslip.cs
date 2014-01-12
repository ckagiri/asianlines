using System;
using System.Collections.Generic;

namespace Ligi.Core.Model
{
    public class Betslip
    {
        public Betslip()
        {
            BetItems = new List<BetItem>();
        }
        public Guid SeasonId { get; set; }
        public List<BetItem> BetItems { get; set; }
    }
}
