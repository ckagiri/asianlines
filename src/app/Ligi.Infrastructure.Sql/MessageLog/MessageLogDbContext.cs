using System.Data.Entity;

namespace Ligi.Infrastructure.Sql.MessageLog
{
    public class MessageLogDbContext : DbContext
    {
        public const string SchemaName = "MessageLog";

        public MessageLogDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MessageLogEntity>().ToTable("Messages", SchemaName);
        }
    }
}
