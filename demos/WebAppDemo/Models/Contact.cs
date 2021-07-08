using System.ComponentModel.DataAnnotations;

namespace WebAppDemo.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Notes { get; set; }
    }
}
