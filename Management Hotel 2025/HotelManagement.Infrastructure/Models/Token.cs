using System;
using System.Collections.Generic;
namespace Mydata.Models;
public partial class Token
{
    public int IdToken { get; set; }

    public string? ToketContent { get; set; }

    public DateTime Daycre { get; set; }

    public DateTime DayExpired { get; set; }

    public int IdUser { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
