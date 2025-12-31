using GameDayParty.Models;
using Microsoft.EntityFrameworkCore; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GameDayParty.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Represents the database tables
        public DbSet<Event> Events { get; set; }
        public DbSet<FoodSuggestion> FoodSuggestions { get; set; }
        public DbSet<UserVote> UserVotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Event>()
                .HasKey(e => e.EventId); 
    
            modelBuilder.Entity<FoodSuggestion>()
                .HasKey(s => s.FoodSuggestionId);
            
            modelBuilder.Entity<Event>().HasData(
                new Event {
                    EventId = 1,
                    EventName = "Sunday Night Football Fiesta",
                    HostName = "Jared Slone",
                    EventDate = new DateTime(2025, 12, 23, 0, 0, 0, DateTimeKind.Utc),
                    GameDetails = "Packers vs. Vikings (NFC North)",
                    IsFinalized = false
                }
            );
            modelBuilder.Entity<FoodSuggestion>().HasData(
                new FoodSuggestion {
                    FoodSuggestionId = 1,
                    EventId = 1,
                    FoodName = "Buffalo Wings", 
                    SuggestedByName = "Jared"   
                }
            );
            // FIX 2: Global UTC Mapper (The "Safety Net")
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                            v => v
                        ));
                    }
                }
            }
            
        }
    }
}