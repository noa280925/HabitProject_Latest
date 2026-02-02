using System;

namespace HabitConnectAPI.Entities
{
    public class Journal
    {
        public int Id { get; set; }
        public int HabitId { get; set; }
        public DateTime Date { get; set; }
        public bool Success { get; set; }
    }
}
