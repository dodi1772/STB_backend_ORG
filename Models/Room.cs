using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace STB_backend.Models
{
    public enum RoomType { STUDIO, MEETING, HYBRID }
    [Table("rooms")]
    public class Room : BaseModel
    {
        /**
         * private int id;
        private string name;
        private enum RoomType { Studio, MeetingRoom, Both }
        private string description;
        private int baseCreditPricePerHour;
        private int capacity;
        private bool isActive;
        private bool isDeleted;
        private DateTime createdAt;
         * */

        [PrimaryKey("id", false)]
        public string Id { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        [Column("type")]
        public RoomType Type { get; set; }
        [Column("description")]
        public string? Description { get; set; }
        [Column("baseCreditPricePerHour")]
        public int baseCreditPricePerHour { get; set; }
        [Column("capacity")]
        public int Capacity { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("isDeleted")]
        public bool IsDeleted { get; set; }
        [Column("deletedAt")]
        public DateTime? DeletedAt { get; set; }
    }
}
