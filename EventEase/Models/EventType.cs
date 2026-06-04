/* EventEase Elite - Event Classification Lookup
   Author: Joshua Marc Lourens
   Description: Defines the predefined category taxonomy used to classify events.
   Implemented as a dedicated lookup table (not a loose string) so that every event
   is tagged against a controlled vocabulary, guaranteeing data integrity and powering
   the Advanced Filtering engine on the Events directory.
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeID { get; set; }

        [Required]
        [Display(Name = "Event Type")]
        public string? EventTypeName { get; set; }

        // Navigation: a single classification can be applied to many events.
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }

    // Microsoft Corporation (2024). Relationships - EF Core. [Online] Available at: https://learn.microsoft.com/en-us/ef/core/modeling/relationships [Accessed 04 June 2026].
    // Microsoft Corporation (2024). Data Seeding - EF Core. [Online] Available at: https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding [Accessed 04 June 2026].
}
