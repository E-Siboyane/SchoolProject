using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ABSARetailPoolingLGDSegment {
      [Key]
      public int Id { get; set; }
      public int PDRowLabelId { get; set; }
      public int ClusterRowId { get; set; }
      public int SegmentRowId { get; set; }
      public string Cluster { get; set; }
      public string Segment { get; set; }
      public string UniqueSegment { get; set; }
      public double LGD { get; set; }
      public int LGDCount { get; set; }
      public int MinOfSASNewLGD { get; set; }
      public int CumulativeSum { get; set; }
      public double Ratio { get; set; }
      public int InitialRank { get; set; }
      public double OriginalAveragePerPool { get; set; }
      public double AveragePerPool { get; set; }
      public int Count { get; set; }
      public int FinalPool { get; set; }
      public string Product { get; set; }
      public Guid RowId { get; set; }
      public int UniqueLGDCountPerPDPool { get; set; }
   }
}
