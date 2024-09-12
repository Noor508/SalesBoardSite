using System.ComponentModel.DataAnnotations;

namespace SalesBoardSite.Models
{
    public class UserDetailsViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Contact is required.")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

   
    }
}
