using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class TermLoanAmortizing : IDtoCommonMethod {
      public int Id { get; set; }
      public int JobId { get; set; }
      public string InstrumentId { get; set; }
      public string InstrumentDescription { get; set; }
      public string InstrumentCurrency { get; set; }
      public DateTime? OriginationDate { get; set; }
      public DateTime MaturityDate { get; set; }
      public string CounterpartyId { get; set; }
      public string SupportingCounterpartyId { get; set; }
      public string SupportTypeName { get; set; }
      public bool PrepayableFlag { get; set; }
      public decimal? FixedRate { get; set; }
      public int? NumberEffective { get; set; }
      public int? NumberActual { get; set; }
      public string LgdScheduleName { get; set; }
      public decimal Lgd { get; set; }
      public decimal? LgdVarianceParam { get; set; }
      public string ReferenceYieldCurve { get; set; }
      public string InterestTypeName { get; set; }
      public string PrincipalTypeName { get; set; }
      public decimal? UpFrontFee { get; set; }
      public decimal? DrawnSpread { get; set; }
      public string DrawnSpreadFreq { get; set; }
      public string PrincipalAmortFreq { get; set; }
      public string FixedRateInterestFreq { get; set; }
      public DateTime AmortizationStartDate { get; set; }
      public int? UserVariableInt { get; set; }
      public string UserVariableString1 { get; set; }
      public string UserVariableString2 { get; set; }
      public string UserVariableString3 { get; set; }
      public bool DefaultedAssetFlag { get; set; }
      public decimal? StressedLgd { get; set; }
      public decimal? StressedLgdVarianceParam { get; set; }

      private string TERMLOANAMORTIZING_HEADER_ROW = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", "InstrumentId,InstrumentDescription,",
                                                                   "InstrumentCurrency,OriginationDate,MaturityDate,CounterpartyId,",
                                                                   "SupportingCounterpartyId,SupportTypeName,PrepayableFlag,FixedRate,",
                                                                   "NumberEffective,NumberActual,LgdScheduleName,Lgd,LgdVarianceParam,",
                                                                   "ReferenceYieldCurve,InterestTypeName,PrincipalTypeName,UpFrontFee,",
                                                                   "DrawnSpread,DrawnSpreadFreq,PrincipalAmortFreq,FixedRateInterestFreq,",
                                                                   "AmortizationStartDate,UserVariableInt,UserVariableString2,",
                                                                   "UserVariableString3,DefaultedAssetFlag,StressedLgd,",
                                                                   "StressedLgdVarianceParam");

      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
         var termLoanAmortizings = new List<TermLoanAmortizing>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
            termLoanAmortizings.Add(ConvertToTermLoanAmortizing(row, jobId));
         });
         return termLoanAmortizings;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {
         var contents = new StringBuilder();
         var headerRow = TERMLOANAMORTIZING_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var ternloanAmortizing in itemsToConvertToString) {
            var item = ternloanAmortizing as TermLoanAmortizing;
            var row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}," +
                                     "{21},{22},{23},{24},{25},{26},{27},{28},{29}", item.InstrumentId, item.InstrumentDescription,
                                  item.InstrumentCurrency, item.OriginationDate, item.MaturityDate, item.CounterpartyId,
                                  item.SupportingCounterpartyId, item.SupportTypeName, item.PrepayableFlag, item.FixedRate,
                                  item.NumberEffective, item.NumberActual, item.LgdScheduleName, item.Lgd, item.LgdVarianceParam,
                                  item.ReferenceYieldCurve, item.InterestTypeName, item.PrincipalTypeName, item.UpFrontFee, item.DrawnSpread,
                                  item.DrawnSpreadFreq, item.PrincipalAmortFreq, item.FixedRateInterestFreq, item.AmortizationStartDate,
                                  item.UserVariableInt, item.UserVariableString2, item.UserVariableString3, item.DefaultedAssetFlag,
                                  item.StressedLgd, item.StressedLgdVarianceParam);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      private TermLoanAmortizing ConvertToTermLoanAmortizing(string[] content, int jobId) {
         try {
            return new TermLoanAmortizing() {
               InstrumentId = (string.IsNullOrEmpty(content[0].Trim()) || string.IsNullOrWhiteSpace(content[0].Trim())) ? null : content[0],
               InstrumentDescription = (string.IsNullOrEmpty(content[1].Trim()) || string.IsNullOrWhiteSpace(content[1].Trim())) ? null : content[1],
               InstrumentCurrency = (string.IsNullOrEmpty(content[2].Trim()) || string.IsNullOrWhiteSpace(content[2].Trim())) ? null : content[2],
               OriginationDate = string.IsNullOrEmpty(content[3]) ? (DateTime?)null : DateTime.Parse(content[3]).Date,
               MaturityDate = DateTime.Parse(content[4]).Date,
               CounterpartyId = (string.IsNullOrEmpty(content[5].Trim()) || string.IsNullOrWhiteSpace(content[5].Trim())) ? null : content[5],
               SupportingCounterpartyId = (string.IsNullOrEmpty(content[6].Trim()) || string.IsNullOrWhiteSpace(content[6].Trim())) ? null : content[6],
               SupportTypeName = (string.IsNullOrEmpty(content[7].Trim()) || string.IsNullOrWhiteSpace(content[7].Trim())) ? null : content[7],
               PrepayableFlag = content[8].ToBoolean(),
               FixedRate = string.IsNullOrEmpty(content[9]) ? (decimal?)null : decimal.Parse(content[9]),
               NumberEffective = string.IsNullOrEmpty(content[10]) ? (int?)null : int.Parse(content[10]),
               NumberActual = string.IsNullOrEmpty(content[11]) ? (int?)null : int.Parse(content[11]),
               LgdScheduleName = (string.IsNullOrEmpty(content[12].Trim()) || string.IsNullOrWhiteSpace(content[12].Trim())) ? null : content[12],
               Lgd = decimal.Parse(content[13]),
               LgdVarianceParam = string.IsNullOrEmpty(content[14]) ? (decimal?)null : decimal.Parse(content[14]),
               ReferenceYieldCurve = (string.IsNullOrEmpty(content[15].Trim()) || string.IsNullOrWhiteSpace(content[15].Trim())) ? null : content[15],
               InterestTypeName = (string.IsNullOrEmpty(content[16].Trim()) || string.IsNullOrWhiteSpace(content[16].Trim())) ? null : content[16],
               PrincipalTypeName = (string.IsNullOrEmpty(content[17].Trim()) || string.IsNullOrWhiteSpace(content[17].Trim())) ? null : content[17],
               UpFrontFee = string.IsNullOrEmpty(content[18]) ? (decimal?)null : decimal.Parse(content[18]),
               DrawnSpread = string.IsNullOrEmpty(content[19]) ? (decimal?)null : decimal.Parse(content[19]),
               DrawnSpreadFreq = (string.IsNullOrEmpty(content[20].Trim()) || string.IsNullOrWhiteSpace(content[20].Trim())) ? null : content[20],
               PrincipalAmortFreq = (string.IsNullOrEmpty(content[21].Trim()) || string.IsNullOrWhiteSpace(content[21].Trim())) ? null : content[21],
               FixedRateInterestFreq = (string.IsNullOrEmpty(content[22].Trim()) || string.IsNullOrWhiteSpace(content[22].Trim())) ? null : content[22],
               AmortizationStartDate = DateTime.Parse(content[23]),
               UserVariableInt = string.IsNullOrEmpty(content[24]) ? (int?)null : int.Parse(content[24]),
               UserVariableString2 = (string.IsNullOrEmpty(content[25].Trim()) || string.IsNullOrWhiteSpace(content[25].Trim())) ? null : content[25],
               UserVariableString3 = (string.IsNullOrEmpty(content[26].Trim()) || string.IsNullOrWhiteSpace(content[26].Trim())) ? null : content[26],
               DefaultedAssetFlag = content[27].ToBoolean(),
               StressedLgd = string.IsNullOrEmpty(content[28]) ? (decimal?)null : decimal.Parse(content[28]),
               StressedLgdVarianceParam = string.IsNullOrEmpty(content[29]) ? (decimal?)null : decimal.Parse(content[29]),
               JobId = jobId
            };
         }
         catch(Exception ex) {
            Console.WriteLine($"{ex.Message} | Record With Error : {content[0]}");
            throw new NotImplementedException(ex.Message);
         }
      }
   }
}
