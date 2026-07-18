using Mydata.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyData.Models
{
    [Table("RoomAmenities")]
    public class RoomAmenity
    {

        [Key]
        public int IDRoomAmenity { get; set; }

        public  int Quanlity { get; set; }



        public int RoomId { get; set; }

        public Room Room { get; set; }

        public int AmenityId { get; set; }

        public Amenity Amenity { get; set; }


    }
}
