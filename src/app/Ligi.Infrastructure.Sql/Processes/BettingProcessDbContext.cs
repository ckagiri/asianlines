using System.Data.Entity;
using Ligi.Core.Processes;

namespace Ligi.Infrastructure.Sql.Processes
{
    public class BettingProcessDbContext : DbContext
    {
        public BettingProcessDbContext(string nameOrConnectionString) 
            : base(nameOrConnectionString)
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BookieProcess>().ToTable("BookieProcess");
            modelBuilder.Entity<BetProcess>().ToTable("BetProcess");

        }

        // Define the available entity sets for the database.
        public DbSet<BookieProcess> BookieProcesses { get; set; }
        public DbSet<BetProcess> BetProcesses { get; set; }

        // Table for pending undispatched messages associated with a process manager.
        public DbSet<UndispatchedMessages> UndispatchedMessages { get; set; }


    }
}
