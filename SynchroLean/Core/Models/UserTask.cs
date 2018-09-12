using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SynchroLean;

namespace SynchroLean.Core.Models
{
    public class UserTask
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public bool IsRecurring
        {
            get
            {
                return this.Frequency == Frequency.NotAvailable;
            }
        }
        public byte Weekdays { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DueDate { get; set; }
        [Required]
        public DateTime LastModified { get; set; }
        [Required]
        public string OwnerEmail { get; set; }
        [ForeignKey("OwnerEmail")]
        public virtual UserAccount Owner { get; set; }
        public Frequency Frequency { get; set; }
        public int? TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }
        public bool OccursToday(DayOfWeek day)
        {
            if(this.Frequency == Frequency.NotAvailable)
            {
                return DateTime.Today.Date == Convert.ToDateTime(DueDate.ToString()).Date;;
            } else if(this.Frequency == Frequency.Daily || this.Frequency == Frequency.Weekly) 
            {
                return 0 < (Weekdays & (1 << (byte)day));
            } else
            {
                return DateTime.Today.Day == Convert.ToDateTime(DueDate.ToString()).Day;
            }
        }
        public virtual Todo Todo { get; set; }
        [NotMapped]
        public bool IsActive
        {
            get
            {
                return null != this.Todo;
            }
        }
        [NotMapped]
        public IEnumerable<DayOfWeek> DaysOfWeek
        {
            get
            {
                byte bits = this.Weekdays;
                for (int i = 0; i < 7; i++)
                {
                    if ((bits & 1) > 0) yield return (DayOfWeek)i;
                    bits >>= 1;
                }
            }

            set
            {
                byte result = 0;
                foreach (DayOfWeek weekday in value)
                {
                    result |= (byte)(1 << (byte)(weekday));
                }
                Weekdays = result;
            }
        }
        public DateTime? Deleted { get; set; }
        [NotMapped]
        public bool IsDeleted { get { return this.Deleted != null; } }
        public void Delete()
        {
            if(!this.IsDeleted) this.Deleted = DateTime.Now;
        }
    }
}
