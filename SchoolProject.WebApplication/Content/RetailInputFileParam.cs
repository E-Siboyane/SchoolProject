using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public class RetailInputFileParam {
        public RetailPoolingFileType ProductName { get; set; }
        public string FilePath { get; set; }
        public string ContentDelimeter { get; set; }
        public DbContextType DbContextType  { get; set; }
        public string ContentLine { get; set; }
    }
}
