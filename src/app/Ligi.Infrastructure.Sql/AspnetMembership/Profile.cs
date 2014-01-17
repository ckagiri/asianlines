using System;

namespace Ligi.Infrastructure.Sql.AspnetMembership
{
    public class Profile
    {
        public Guid UserId { get; set; }
        public string PropertyNames { get; set; }
        public string PropertyValueStrings { get; set; }
        public byte[] PropertyValueBinary { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public virtual User User { get; set; }
    }
}
