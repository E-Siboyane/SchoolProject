using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    [Table("AnalysisJobManager")]
    public class AnalysisJobManager {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnalysisJobManagerId { get; set; }
        [Required]
        public int JobId { get; set; }
        public virtual Job Job { get; set; }
        [Required]
        public string PortfolioName { get; set; }
        [Required]
        public string HoldingDate { get; set; }
        [Required]
        public string AnalysisDate { get; set; }
        [Required]
        public string AnalysisSettings { get; set; }
        [Required]
        public string DataSettings { get; set; }
        [Required]
        public bool IsProcessed { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime? ProcessingDate { get; set; }
    }
}
