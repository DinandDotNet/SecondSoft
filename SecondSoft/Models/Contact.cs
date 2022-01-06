using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondSoft.Models
{
    public class Contact
    {
        [Required]
        [StringLength(35, ErrorMessage = "Name is too long.")]
        public string? Name { get; set; }
        [Required]
        [StringLength(35, ErrorMessage = "Name is too long.")]
        public string? CompanyName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [StringLength(25, ErrorMessage = "Name is too long.")]
        public string? Phone { get; set; }
        [StringLength(250, ErrorMessage = "Name is too long.")]
        public string? Message { get; set; }
    }
}
