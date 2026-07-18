using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mydata.Models
{
    [Table("Services")]
    public class Services
    {
        [Key]
        public int ServiceId { get; set; }

        [StringLength(200)]
        public string ServiceName { get; set; } = null!;

        [StringLength(500)]
        public string Description { get; set; } = null!;


        public decimal Price { get; set; }

        public decimal Discount { get; set; } = 0;

        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

         
    }
}
