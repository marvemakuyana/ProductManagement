using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage ="Category name is required.")]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required(ErrorMessage ="Category code is required")]
        [StringLength(6)]
        [RegularExpression(
            @"^[A-Z]{3}[0-9]{3}$",
            ErrorMessage = "Category Code must be 3 uppercase letters followed by 3 numbers (Example: ABC123)."
        )]
        public required string CategoryCode { get; set; }

        public bool IsActive { get; set; }

        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }


    }
}
