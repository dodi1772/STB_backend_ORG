using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace STB_backend.Models

{
    public enum Role { USER, ADMIN }
    public enum PartnerTier { NONE, BRONZE, SILVER, GOLD, PLATINUM }
    [Table("users")]
    public class AppUser: BaseModel
    {
        // a tábla oszlopainak megfelelően definiáljuk a mezőket
        [PrimaryKey("id", false)]
        public string Id { get; set; }
        [Column("firstName")]
        public string FirstName { get; set; }
        [Column("lastName")]
        public string LastName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("phoneNumber")]
        public string PhoneNumber { get; set; }
        [Column("creditBalance")]
        public int CreditBalance { get; set; }
        [Column("role")]
        public Role UserType { get; set; }

        [Column("partnerTier")]
        public PartnerTier PartnerTier { get; set; }

        [Column("discountRate")]
        public decimal DiscountRate { get; set; }
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }
        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        [Column("isDeleted")]
        public bool IsDeleted { get; set; }
        [Column("deletedAt")]
        public DateTime? DeletedAt { get; set; }
    }
}
