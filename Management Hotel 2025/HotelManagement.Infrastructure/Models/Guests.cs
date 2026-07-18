using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Mydata.Models
{
    [Table("Guest")]
    public class Guests
    {
        [Key]

        public int GuestId { get; set; }

    
        public string? CodePersonal { get; set; } = null!;

    
        public string? FullName { get; set; } = null!;


        public string? Gender { get; set; }


        public string? PhoneNumber { get; set; }

     
        public DateTime? BirthDay { get; set; }

        
        public string? Address { get; set; } = null!;

        
        public string? Nationality { get; set; } = null!;

        public string? Note { get; set; }

        public DateTime? TimeCheckIn { get; set; }

        public DateTime? TimeCheckOut { get; set; }

        public int BookingDetailId { get; set; }

        [ForeignKey("BookingDetailId")]
        public virtual BookingDetail BookingDetail { get; set; } = null!;


    }
}
