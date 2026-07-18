using Mydata.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyData.Models
{
    [Table("Images")]
    public class Images
    {

        [Key]
        public int IdImage { get; set; }

        [Required]
        [StringLength(300)]
        public string? LinkImage { get; set; }

        [Required]
        
        public int? IdRoom { get; set; }

        [ForeignKey("IdRoom")]
        public virtual Room? Room { get; set; }
    }
}
