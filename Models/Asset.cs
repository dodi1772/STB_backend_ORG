using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace STB_backend.Models
{
    [Table("assets")]
    public class Asset : BaseModel
    {
        /**
         *private int id;
        private string name;
        private string description;
        private int roomId;
        private int baseCreditPricePerHour;
        private int ownerId;
        private int monthlyDividendCredits;
        private bool isActive;
        private bool isDeleted;
        private DateTime createdAt; 
         */
        [PrimaryKey("id", false)]
        public string Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("roomId")]
        public string RoomId { get; set; }
        [Column("baseCreditPricePerHour")]
        public int BaseCreditPricePerHour { get; set; }
        [Column("ownerId")]
        public string OwnerId { get; set; }
        [Column("monthlyDividendCredits")]
        public int MonthlyDividendCredits { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("isDeleted")]
        public bool IsDeleted { get; set; }
        [Column("deletedAt")]
        public DateTime? DeletedAt { get; set; }
    }
}
