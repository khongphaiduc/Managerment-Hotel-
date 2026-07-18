using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mydata.Models
{
    [Table("BookingDetails")]
    public class BookingDetail
    {
        [Key]
        public int BookingDetailId { get; set; }

        public int BookingId { get; set; }

        public int RoomId { get; set; }

        public int NumberOfGuests { get; set; }


        [Column("ExpectedCheckInDate")]
        public DateTime? CheckInDate { get; set; }

        [Column("ExpectedCheckOutDate")]
        public DateTime? CheckOutDate { get; set; }

        public string StatusCheckRoom { get; set; } = "Booked"; // Booked, CheckedIn, CheckedOut, Cancelled

        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; } = null!;


        [ForeignKey("BookingId")]

        public virtual Booking Booking { get; set; } = null!;
        public virtual ICollection<Guests> Guests { get; set; } = new List<Guests>();

        public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }

}
