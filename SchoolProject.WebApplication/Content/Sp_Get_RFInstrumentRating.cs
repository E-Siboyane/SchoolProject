using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
    public class Sp_Get_RFInstrumentRating {
        public string InstrumentId { get; set; }
        public DateTime RatingDate { get; set; }
        public DateTime? RatingEndDate { get; set; }
        public string RatingSystemName { get; set; }
        public string Rating { get; set; }
    }
}
