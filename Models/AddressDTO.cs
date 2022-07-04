using System.ComponentModel.DataAnnotations.Schema;

namespace PloomesTest.Models
{
    public class AddressDTO
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}