using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Data
{
    public class EventEaseContext : DbContext
    {
        public EventEaseContext(DbContextOptions<EventEaseContext> options)
            : base(options)
        {
        }

        public DbSet<EventEase.Models.Venue> Venue { get; set; } = default!;
        public DbSet<EventEase.Models.Booking> Booking { get; set; } = default!;
        public DbSet<EventEase.Models.Event> Event { get; set; } = default!;
        public DbSet<EventEase.Models.EventType> EventType { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Booking -> Venue relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Venue)
                .WithMany() // A Venue can have many Bookings
                .HasForeignKey(b => b.VenueID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the Booking -> Event relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Bookings) // THE FIX: This correctly links the plural 'Bookings' list
                .HasForeignKey(b => b.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the Event -> EventType relationship (classification lookup)
            // Restrict on delete so a category in active use cannot be wiped out
            // underneath the events that depend on it.
            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany(t => t.Events)
                .HasForeignKey(e => e.EventTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // PREDEFINED TAXONOMY: Seed the lookup with the standard event categories so
            // the Advanced Filtering dropdown is populated the moment the database is created.
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeID = 1, EventTypeName = "Conference" },
                new EventType { EventTypeID = 2, EventTypeName = "Wedding" },
                new EventType { EventTypeID = 3, EventTypeName = "Concert" },
                new EventType { EventTypeID = 4, EventTypeName = "Corporate Function" },
                new EventType { EventTypeID = 5, EventTypeName = "Workshop" },
                new EventType { EventTypeID = 6, EventTypeName = "Exhibition" },
                new EventType { EventTypeID = 7, EventTypeName = "Birthday Party" },
                new EventType { EventTypeID = 8, EventTypeName = "Sports Event" }
            );

            // NOTE: The third block that was causing the crash has been completely removed!
        }
    }
}