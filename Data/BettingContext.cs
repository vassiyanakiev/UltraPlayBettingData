using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using UltraPlayBettingData.Models;

namespace UltraPlayBettingData.Data
{
    public class BettingContext : DbContext
    {
        public BettingContext(DbContextOptions<BettingContext> options) : base(options) { }

        public DbSet<Sport> Sports { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Odd> Odds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<XmlSports>()
           .HasKey(x => x.CreateDate);

            modelBuilder.Entity<Sport>()
                .HasMany(s => s.Events)
                .WithOne(e => e.Sport)
                .HasForeignKey(e => e.SportID);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Matches)
                .WithOne(m => m.Event)
                .HasForeignKey(m => m.EventID);

            modelBuilder.Entity<Match>()
                .HasMany(m => m.Bets)
                .WithOne(b => b.Match)
                .HasForeignKey(b => b.MatchID);

            modelBuilder.Entity<Bet>()
                .HasMany(b => b.Odds)
                .WithOne(o => o.Bet)
                .HasForeignKey(o => o.BetID);

            // Specify precision for decimal properties
            modelBuilder.Entity<Odd>()
                .Property(o => o.Value)
                .HasColumnType("decimal(18,2)"); 

            modelBuilder.Entity<Odd>()
                .Property(o => o.SpecialBetValue)
                .HasColumnType("decimal(18,2)"); 

        }
    }

    public class BettingContextFactory : IDesignTimeDbContextFactory<BettingContext>
    {
        public BettingContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
           .Build();

            var builder = new DbContextOptionsBuilder<BettingContext>();
            var connectionString = configuration.GetConnectionString("BettingDatabase");
            builder.UseSqlServer(connectionString);

            return new BettingContext(builder.Options);
        }
    }

}

