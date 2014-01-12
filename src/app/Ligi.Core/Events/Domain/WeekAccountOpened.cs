using System;

namespace Ligi.Core.Events.Domain
{
    public class WeekAccountOpened : VersionedEvent
    {
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid SeasonId { get; set; }
    }
}
