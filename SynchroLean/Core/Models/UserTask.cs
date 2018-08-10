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
        public bool IsRecurring { get; set; }
        public byte Weekdays { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsRemoved { get; set; }
        public int OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public virtual UserAccount Owner { get; set; }
        public Frequency Frequency { get; set; }
        public int? TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }
        public bool OccursOnDayOfWeek(DayOfWeek day)
        {
            return this.Frequency != Frequency.Daily || 0 < (Weekdays & (1 << (byte)day));
        }
        public virtual Todo Todo { get; set; }
        [NotMapped]
        public bool IsActive
        {
            get
            {
                return null == this.Todo;
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

        public void deleteTask()
        {
            IsRemoved = true;
        }
    }
}
