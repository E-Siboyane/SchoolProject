﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class TermLoanBullet : IDtoCommonMethod {
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

      private string TERMLOANBULLET_HEADER_ROW = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", "InstrumentId,InstrumentDescription,",
                                                               "InstrumentCurrency,OriginationDate,MaturityDate,CounterPartyId,",
                                                               "SupportingCounterPartyId,SupportTypeName,PrePayableFlag,FixedRate,",
                                                               "DrawnSpread,NumberEffective,NumberActual,LgdScheduleName,Lgd,",
                                                               "LgdVarianceParam,ReferenceYieldCurve,InterestTypeName,UpFrontFee,",
                                                               "DrawnSpreadFreq,FixedRateInterestFreq,UUserVariableInt,",
                                                               "UserVariableString1,userVariableString2,UserVariableString3,",
                                                               "DefaultedAssetFlag,StressedLgd,StressedLgdVarianceParam");

      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
         var termLoanBullets = new List<TermLoanBullet>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
            termLoanBullets.Add(ConvertToTermLoanBullet(row, jobId));
         });
         return termLoanBullets;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {
         var contents = new StringBuilder();
         var headerRow = TERMLOANBULLET_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var termBullet in itemsToConvertToString) {
            var item = termBullet as TermLoanBullet;
            var row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}," +
                                     "{21},{22},{23},{24},{25},{26},{27}", item.InstrumentId, item.InstrumentDescription,
                                     item.InstrumentCurrency, item.OriginationDate, item.MaturityDate.Date, item.CounterpartyId,
                                     item.SupportingCounterpartyId, item.SupportTypeName, item.PrePayableFlag, item.FixedRate,
                                     item.DrawnSpread, item.NumberEffective, item.NumberActual, item.LgdScheduleName, item.Lgd,
                                     item.LgdVarianceParam, item.ReferenceYieldCurve, item.InterestTypeName, item.UpFrontFee,
                                     item.DrawnSpreadFreq, item.FixedRateInterestFreq, item.UUserVariableInt, item.UserVariableString1,
                                     item.userVariableString2, item.UserVariableString3, item.DefaultedAssetFlag, item.StressedLgd,
                                     item.StressedLgdVarianceParam);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      private TermLoanBullet ConvertToTermLoanBullet(string[] content, int jobId) {
         try {
            var format = new TermLoanBullet() {
               InstrumentId = (string.IsNullOrEmpty(content[0].Trim()) || string.IsNullOrWhiteSpace(content[0].Trim())) ? null : content[0],
               InstrumentDescription = (string.IsNullOrEmpty(content[1].Trim()) || string.IsNullOrWhiteSpace(content[1].Trim())) ? null : content[1],
               InstrumentCurrency = (string.IsNullOrEmpty(content[2].Trim()) || string.IsNullOrWhiteSpace(content[2].Trim())) ? null : content[2],
               OriginationDate = string.IsNullOrEmpty(content[3]) ? (DateTime?)null : DateTime.Parse(content[3]).Date,
               MaturityDate = DateTime.Parse(content[4]).Date,
               CounterpartyId = (string.IsNullOrEmpty(content[5].Trim()) || string.IsNullOrWhiteSpace(content[5].Trim())) ? null : content[5],
               SupportingCounterpartyId = (string.IsNullOrEmpty(content[6].Trim()) || string.IsNullOrWhiteSpace(content[6].Trim())) ? null : content[6],
               SupportTypeName = (string.IsNullOrEmpty(content[7].Trim()) || string.IsNullOrWhiteSpace(content[7].Trim())) ? null : content[7],
               PrePayableFlag = content[8].ToBoolean(),
               FixedRate = string.IsNullOrEmpty(content[9]) ? (decimal?)null : decimal.Parse(content[9]),
               DrawnSpread = decimal.Parse(content[10]),
               NumberEffective = string.IsNullOrEmpty(content[11]) ? (int?)null : int.Parse(content[11]),
               NumberActual = string.IsNullOrEmpty(content[12]) ? (int?)null : int.Parse(content[12]),
               LgdScheduleName = (string.IsNullOrEmpty(content[13].Trim()) || string.IsNullOrWhiteSpace(content[13].Trim())) ? null : content[13],
               Lgd = decimal.Parse(content[14]),
               LgdVarianceParam = string.IsNullOrEmpty(content[15]) ? (decimal?)null : decimal.Parse(content[15]),
               ReferenceYieldCurve = (string.IsNullOrEmpty(content[16].Trim()) || string.IsNullOrWhiteSpace(content[16].Trim())) ? null : content[16],
               InterestTypeName = (string.IsNullOrEmpty(content[17].Trim()) || string.IsNullOrWhiteSpace(content[17].Trim())) ? null : content[17],
               UpFrontFee = string.IsNullOrEmpty(content[18]) ? (decimal?)null : decimal.Parse(content[18]),
               DrawnSpreadFreq = (string.IsNullOrEmpty(content[19].Trim()) || string.IsNullOrWhiteSpace(content[19].Trim())) ? null : content[19],
               FixedRateInterestFreq = (string.IsNullOrEmpty(content[20].Trim()) || string.IsNullOrWhiteSpace(content[20].Trim())) ? null : content[20],
               UUserVariableInt = string.IsNullOrEmpty(content[21]) ? (int?)null : int.Parse(content[21]),
               UserVariableString1 = (string.IsNullOrEmpty(content[22].Trim()) || string.IsNullOrWhiteSpace(content[22].Trim())) ? null : content[22],
               userVariableString2 = (string.IsNullOrEmpty(content[23].Trim()) || string.IsNullOrWhiteSpace(content[23].Trim())) ? null : content[23],
               UserVariableString3 = (string.IsNullOrEmpty(content[24].Trim()) || string.IsNullOrWhiteSpace(content[24].Trim())) ? null : content[24],
               DefaultedAssetFlag = content[25].ToBoolean(),
               StressedLgd = string.IsNullOrEmpty(content[26]) ? (decimal?)null : decimal.Parse(content[26]),
               StressedLgdVarianceParam = string.IsNullOrEmpty(content[27]) ? (decimal?)null : decimal.Parse(content[27]),
               JobId = jobId
            };
            return (format);
         }
         catch(Exception ex) {
            Console.WriteLine($"{ex.Message} | Record With Error : {content[0]}");
            throw new NotImplementedException(ex.Message);
         }
      }
   }
}
