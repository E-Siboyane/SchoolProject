using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
    public class Sp_Get_RFTermLoanBullet {
        public string InstrumentId { get; set; }
        public string InstrumentDescription { get; set; }
        public string InstrumentCurrency { get; set; }
        public DateTime? OriginationDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public string CounterPartyId { get; set; }
        public string SupportingCounterPartyId { get; set; }
        public string SupportTypeName { get; set; }
        public bool PrePayableFlag { get; set; }
        public decimal? FixedRate { get; set; }
        public decimal DrawnSpread { get; set; }
        public int? NumberEffective { get; set; }
        public int? NumberActual { get; set; }
        public string LgdScheduleName { get; set; }
        public decimal Lgd { get; set; }
        public decimal? LgdVarianceParam { get; set; }
        public string ReferenceYieldCurve { get; set; }
        public string InterestTypeName { get; set; }
        public decimal? UpFrontFee { get; set; }
        public string DrawnSpreadFreq { get; set; }
        public string FixedRateInterestFreq { get; set; }
        public int? UUserVariableInt { get; set; }
        public string UserVariableString1 { get; set; }
        public string userVariableString2 { get; set; }
        public string UserVariableString3 { get; set; }
        public bool? DefaultedAssetFlag { get; set; }
        public decimal? StressedLgd { get; set; }
        public decimal? StressedLgdVarianceParam { get; set; }
    }
}
