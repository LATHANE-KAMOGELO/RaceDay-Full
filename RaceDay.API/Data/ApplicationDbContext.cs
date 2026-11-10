
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
using Route = RaceDay.API.Models.Route;

namespace RaceDay.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Route> Routes => Set<Route>();
        public DbSet<Enrolment> Enrolments => Set<Enrolment>();
        public DbSet<Result> Results => Set<Result>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prevent duplicate email addresses.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Event and organiser relationship.
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany()
                .HasForeignKey(e => e.OrganiserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .Property(e => e.Distance)
                .HasPrecision(8, 2);

            // Category relationship.
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .Property(c => c.Distance)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Category>()
                .Property(c => c.EntryFee)
                .HasPrecision(10, 2);

            // Route relationship.
            modelBuilder.Entity<Route>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Routes)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Route>()
                .Property(r => r.Distance)
                .HasPrecision(8, 2);

            // Enrolment and participant relationship.
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Participant)
                .WithMany()
                .HasForeignKey(e => e.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrolment and event relationship.
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Event)
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrolment and category relationship.
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // One participant can register only once per event.
            modelBuilder.Entity<Enrolment>()
                .HasIndex(e => new { e.ParticipantId, e.EventId })
                .IsUnique();

            // Result relationship.
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithMany()
                .HasForeignKey(r => r.EnrolmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // One result per enrolment.
            modelBuilder.Entity<Result>()
                .HasIndex(r => r.EnrolmentId)
                .IsUnique();
        }
    }
}
