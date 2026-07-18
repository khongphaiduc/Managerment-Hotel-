using Mydata.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyData.Models
{
    public class DepositHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string TransactionCode { get; set; } = null!;

        public decimal Amount { get; set; } // số tiền nạp

        public DateTime DepositDate { get; set; } // ngày nạp tiền

        public string Status { get; set; } = "Pending";

        [ForeignKey("UserId")]

        public virtual User User { get; set; } = null!;
    }
}
