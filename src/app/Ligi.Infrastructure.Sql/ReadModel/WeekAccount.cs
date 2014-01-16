using System;

namespace Ligi.Infrastructure.Sql.ReadModel
{
    public class WeekAccount
    {
        public WeekAccount()
        {
            Credit = 1000;
            Available = 1000;
        }

        public WeekAccount(DateTime startDate, DateTime endDate) : this()
        {
            Id = Guid.NewGuid();
            StartDate = startDate;
            EndDate = endDate;
        }

        public WeekAccount(Guid id, Guid userId, DateTime startDate, DateTime endDate, Guid seasonId, int version = 0)
        {
            Id = id;
            UserId = userId;
            StartDate = startDate;
            EndDate = endDate;
            SeasonId = seasonId;
            Credit = 1000;
            Version = version;
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid SeasonId { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalPayout { get; set; }
        public decimal Credit { get; set; }
        public decimal Available { get; set; }
        public decimal TotalStake { get; set; }
        public decimal Profit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Version { get; internal set; }
    }
}
