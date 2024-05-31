using Microsoft.EntityFrameworkCore;
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
            // Configure relationships and constraints if needed
            modelBuilder.Entity<Sport>()
                .HasMany(s => s.Events)
                .WithOne(e => e.Sport)
                .HasForeignKey(e => e.SportId);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Matches)
                .WithOne(m => m.Event)
                .HasForeignKey(m => m.EventId);

            modelBuilder.Entity<Match>()
                .HasMany(m => m.Bets)
                .WithOne(b => b.Match)
                .HasForeignKey(b => b.MatchId);

            modelBuilder.Entity<Bet>()
                .HasMany(b => b.Odds)
                .WithOne(o => o.Bet)
                .HasForeignKey(o => o.BetId);
        }
    }

}

