using System;
using AutoMapper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SynchroLean.Models
{
    public class UserTask: IUserTask
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRecurring { get; set; }
        public byte Weekdays { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletionDate { get; set; }
        public bool IsRemoved { get; set; }
        public int OwnerId { get; set; }

        bool IUserTask.occursOnDayOfWeek(DayOfWeek day)
        {
            return 0 < (Weekdays & (1 << (byte)day));
        }

        IEnumerable<DayOfWeek> IUserTask.Weekdays
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

        void IUserTask.completeTask()
        {
            IsCompleted = true;
        }

        void IUserTask.deleteTask()
        {
            IsRemoved = true;
        }

        bool IUserTask.IsCompleted
        {
            get { return this.IsCompleted; }
        }

        bool IUserTask.IsDeleted
        {
            get { return this.IsRemoved; }
        }
    }
}
