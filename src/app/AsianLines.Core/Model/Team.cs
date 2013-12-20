using System;

namespace AsianLines.Core.Model
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string HomeGround { get; set; }
        public string Country { get; set; }
    }
}
