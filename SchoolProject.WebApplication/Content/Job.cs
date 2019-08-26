using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class Job {
       public int JobId { get; set; }
       public string JobName { get; set; }
       public string Username { get; set; }
       public string Password { get; set; }
       public string Status { get; set; }
       public DateTime InsertTime { get; set; }
       public bool OverWrite { get; set; }
       public string ExecutionHost { get; set; }
    }
}
