using System.ComponentModel.DataAnnotations;

namespace SalesBoardSite.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Identifier { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
