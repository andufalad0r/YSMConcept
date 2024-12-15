using System.ComponentModel.DataAnnotations;

namespace YSMConcept.Domain.ValueObjects
{
    public class Address
    {
        [MinLength(1)]
        [MaxLength(18)]
        public string City { get; set; } = null!;
        [MinLength(1)]
        [MaxLength(40)]
        public string Street { get; set; } = null!;
        public Address() { }
        public Address(string city, string street)
        {
            City = city;
            Street = street;
        }
    }
}
