using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public enum ReviewProcessStatus {
        Unknown,
        Content_Creation,
        Content_Creattion_Completed,
        Employee_Scoring,
        Line_Manager_Scoring,
        Scoring_Completed
    }
}