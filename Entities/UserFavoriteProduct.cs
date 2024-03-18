using System.ComponentModel.DataAnnotations;

namespace BiyLineApi.Entities
{
    public sealed class UserFavoriteProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } 
        public UserEntity User { get; set; }

        [Required]
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; }
    }
}
