using System;
using System.Collections.Generic;

namespace Mydata.Models;
public partial class StaffAction
{
    public int ActionId { get; set; }

    public int StaffId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime? ActionTime { get; set; }

    public virtual User Staff { get; set; } = null!;
}
