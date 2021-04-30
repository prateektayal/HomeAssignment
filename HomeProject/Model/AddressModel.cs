using System.ComponentModel.DataAnnotations;

namespace HomeProject.Model
{
    public class AddressModel
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string ContactNumber { get; set; }
    }
}
