using System;
using System.Collections.Generic;

namespace Mydata.Models;

public partial class RoomType
{
    public int RoomTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int MaxGuests { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
