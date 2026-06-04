/* EventEase Elite - Events Infrastructure
   Author: Joshua Marc Lourens
   Description: Manages administrative event definitions. 
   Implements referential integrity gates to prevent orphaned booking data and 
   includes pre-flight infrastructure checks for stable record creation.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventEaseContext _context;

        public EventsController(EventEaseContext context)
        {
            _context = context;
        }

        // GET: Events
        // Logic: ADVANCED FILTERING ENGINE. Layers four independent, fully-combinable
        // filters over the directory - free-text search, EventType classification,
        // a scheduling date range, and live venue availability - so specialists can
        // pinpoint records even during high-volume periods. Each filter is an optional
        // gate; supply any combination and the query composes them into one SQL pass.
        public async Task<IActionResult> Index(string searchString, int? eventTypeId, DateOnly? startDate, DateOnly? endDate, bool availableOnly)
        {
            // STATE PERSISTENCE: Echo every filter value back to the view so the UI
            // controls retain their selection after a query is submitted.
            ViewData["CurrentFilter"] = searchString;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["AvailableOnly"] = availableOnly;
            ViewBag.EventTypeId = new SelectList(_context.EventType, "EventTypeID", "EventTypeName", eventTypeId);

            // Base query: eager-load the classification and venue for grid display.
            var events = _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            // FILTER 1 - Free-text: partial match on event name or description.
            if (!String.IsNullOrEmpty(searchString))
            {
                events = events.Where(e => e.EventName!.Contains(searchString)
                                        || e.EventDescription!.Contains(searchString));
            }

            // FILTER 2 - EventType: restrict to a single predefined classification.
            if (eventTypeId.HasValue)
            {
                events = events.Where(e => e.EventTypeID == eventTypeId.Value);
            }

            // FILTER 3 - Date Range: return events whose schedule overlaps the window.
            // Each bound is applied independently, so the filter still works when only
            // a 'from' or only a 'to' date is supplied.
            if (startDate.HasValue)
            {
                events = events.Where(e => e.EventEndDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                events = events.Where(e => e.EventStartDate <= endDate.Value);
            }

            // FILTER 4 - Venue Availability: drop events whose venue already holds a
            // booking that clashes with the selected window. With no window supplied the
            // bounds widen to all-time, so the toggle then surfaces only venues that are
            // entirely free. This is why availability composes naturally with Filter 3.
            if (availableOnly)
            {
                var windowStart = startDate ?? DateOnly.MinValue;
                var windowEnd = endDate ?? DateOnly.MaxValue;

                var busyVenueIds = _context.Booking
                    .Where(b => b.BookingStartDate <= windowEnd && b.BookingEndDate >= windowStart)
                    .Select(b => b.VenueID);

                events = events.Where(e => e.VenueID != null && !busyVenueIds.Contains(e.VenueID.Value));
            }

            return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        // Logic: Eager loads the associated Venue and Bookings to provide a comprehensive event profile.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .Include(e => e.Bookings)
                    .ThenInclude(b => b.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            // SPECIALIST CHECK: Every event requires an existing Venue for allocation.
            // This prevents system crashes during the registration process.
            if (!_context.Venue.Any())
            {
                // UI Feedback: Forcing the specialist to register infrastructure (Venues) first.
                TempData["ErrorMessage"] = "SYSTEM ALERT: No Venues detected. You must register a Venue before creating an Event schedule.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName");
            // Populate the EventType lookup so the specialist can classify the new event.
            ViewData["EventTypeID"] = new SelectList(_context.EventType, "EventTypeID", "EventTypeName");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,EventName,EventDescription,EventStartDate,EventEndDate,VenueID,EventTypeID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event record successfully registered.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", @event.VenueID);
            ViewData["EventTypeID"] = new SelectList(_context.EventType, "EventTypeID", "EventTypeName", @event.EventTypeID);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return NotFound();

            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", @event.VenueID);
            ViewData["EventTypeID"] = new SelectList(_context.EventType, "EventTypeID", "EventTypeName", @event.EventTypeID);
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,EventName,EventDescription,EventStartDate,EventEndDate,VenueID,EventTypeID")] Event @event)
        {
            if (id != @event.EventID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event record updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", @event.VenueID);
            ViewData["EventTypeID"] = new SelectList(_context.EventType, "EventTypeID", "EventTypeName", @event.EventTypeID);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // REFERENTIAL INTEGRITY CHECK: Block deletion if records are linked to prevent data orphaning.
            // This maintains the stability of the administrative ledger.
            bool hasLinkedBookings = await _context.Booking.AnyAsync(b => b.EventID == id);

            if (hasLinkedBookings)
            {
                // BLOCK THE DELETE and provide tactical feedback to the interface.
                TempData["ErrorMessage"] = "CANNOT DELETE: This event is currently linked to active bookings. Remove associated bookings first.";
                return RedirectToAction(nameof(Index));
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event != null)
            {
                _context.Event.Remove(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event record successfully decommissioned.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id) => _context.Event.Any(e => e.EventID == id);
    }
}

/* TECHNICAL REFERENCES
   ---------------------------------------------------
   1. Referential Integrity Logic: Validating database constraints at the application level to ensure data stability.
   2. Eager Loading (.Include): Aggregating related Venue and Booking datasets in a single efficient query.
   3. UI Error Handling: Implementation of TempData-based global alert systems for real-time specialist feedback.
   4. Async Programming: Implementation of Task-based asynchronous controller actions for cloud-ready infrastructure.
   5. Advanced Filtering: Composable IQueryable predicates (Search + EventType + Date Range + Availability) deferred into a single SQL execution.
   6. Subquery Availability Gate: Booking-overlap subquery (.Contains on a projected key set) to exclude venues with clashing reservations.

   Microsoft Corporation (2024). Sort, filter, page, and group - EF Core. [Online] Available at: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page [Accessed 04 June 2026].
   Microsoft Corporation (2024). Querying Data - EF Core. [Online] Available at: https://learn.microsoft.com/en-us/ef/core/querying/ [Accessed 04 June 2026].
*/