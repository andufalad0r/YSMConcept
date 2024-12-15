using System.ComponentModel.DataAnnotations;

namespace YSMConcept.Domain.ValueObjects
{
    public class Date
    {
        [Range(2000, 2100)]
        public int Year { get; set; }
        [Range(1, 12)]
        public int Month { get; set; }
        public Date() { }
        public Date(int year, int month)
        {
            Year = year;
            Month = month;
        }
    }
}
