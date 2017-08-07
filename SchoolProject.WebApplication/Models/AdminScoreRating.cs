using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    public class AdminScoreRating : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScoreRatingId { get; set; }
        public string Rating { get; set; }
        public string RatingCode { get; set; }
        [Required]
        public decimal MinScore { get; set; }
        [Required]
        public decimal MaxScore { get; set; }
        [Required]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
