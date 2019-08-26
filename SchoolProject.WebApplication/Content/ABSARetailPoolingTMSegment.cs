using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ABSARetailPoolingTMSegment {
      public int Id { get; set; }
      public int PDRowLabelId { get; set; }
      public int LGDRowLabelId { get; set; }
      public int ClusterRowId { get; set; }
      public int SegmentRowId { get; set; }
      public string Cluster { get; set; }
      public string Segment { get; set; }
      public string UniqueSegment { get; set; }
      public string UniqueSegmentPDLGD { get; set; }
      public int TM { get; set; }
      public int TMCount { get; set; }
      public int CumulativeSum { get; set; }
      public int InitialRank { get; set; }
      public double Ratio { get; set; }
      public double OriginalAveragePerPool { get; set; }
      public double AveragePerPool { get; set; }
      public int FinalPool { get; set; }
      public string Product { get; set; }
      public DateTime CreatedDate { get; set; }
      public Guid RowId { get; set; }
   }
}
