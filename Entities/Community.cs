using System;

namespace HabitConnectAPI.Entities
{
    public class Community
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Purpose { get; set; }
    }
}
