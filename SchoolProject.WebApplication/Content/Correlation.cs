using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public class Correlation {
        public List<CommonFactors> CommonFactors { get; set; }
        public List<CommonFactorStandardDeviation> CommonFactorStandardDeviations { get; set; }
        public List<CorrelationModels> CorrelationModels { get; set; }
        public List<CountryFactorBetas> CountryFactorBetas { get; set; }
        public List<CountryFactors> CountryFactors { get; set; }
        public List<CountryFactorRSquared> CountryFactorRSquareds { get; set; }
        public List<ImportAsOfDate> ImportAsOfDate { get; set; }
        public List<IndustryFactorBetas> IndustryFactorBetas { get; set; }
        public List<IndustryFactorRSquared> IndustryFactorRSquareds { get; set; }
        public List<IndustryFactors> IndustryFactors { get; set; }
    }
}
