namespace SchoolProject.WebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminJobGrade",
                c => new
                    {
                        JobGradeId = c.Int(nullable: false, identity: true),
                        JobGrade = c.String(nullable: false, maxLength: 100),
                        JobGradeCode = c.String(nullable: false, maxLength: 50),
                        JobGradeDescription = c.String(),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.JobGradeId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.AdminStatus",
                c => new
                    {
                        StatusId = c.Int(nullable: false, identity: true),
                        StatusName = c.String(nullable: false, maxLength: 50),
                        StatusCode = c.String(nullable: false, maxLength: 50),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.StatusId);
            
            CreateTable(
                "dbo.AdminMeasure",
                c => new
                    {
                        MeasureId = c.Long(nullable: false, identity: true),
                        MeasureName = c.String(nullable: false),
                        DefaultWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DefaultSourceOfInformation = c.String(nullable: false),
                        DefaultSubjectMatterExpert = c.String(nullable: false),
                        TermId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.MeasureId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .ForeignKey("dbo.AdminTerm", t => t.TermId)
                .Index(t => t.TermId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.AdminTerm",
                c => new
                    {
                        TermId = c.Int(nullable: false, identity: true),
                        TermName = c.String(nullable: false, maxLength: 150),
                        TermCode = c.String(nullable: false, maxLength: 50),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.TermId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.AdminObjective",
                c => new
                    {
                        ObjectivesId = c.Long(nullable: false, identity: true),
                        ObjectiveName = c.String(nullable: false),
                        ObjectiveOverallWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ObjectivesId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.PerformanceYear",
                c => new
                    {
                        PerformanceYearId = c.Int(nullable: false, identity: true),
                        PerformanceYearName = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.PerformanceYearId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.PMDocumentType",
                c => new
                    {
                        DocumentTypeId = c.Int(nullable: false, identity: true),
                        DocumentTypeCode = c.String(nullable: false, maxLength: 50),
                        DocumentTypeName = c.String(),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.DocumentTypeId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.PMeasure",
                c => new
                    {
                        PMMeasureId = c.Long(nullable: false, identity: true),
                        PMStrategicGoalId = c.Int(nullable: false),
                        PMObjective = c.String(),
                        MeasureName = c.String(nullable: false),
                        SourceOfInformation = c.String(maxLength: 150),
                        SubjectMatterExpert = c.String(nullable: false, maxLength: 150),
                        TermId = c.Int(nullable: false),
                        MeasureWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EmployeeScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineManagerScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AuditScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EmployeeComments = c.String(),
                        LineManagerComments = c.String(),
                        AuditComments = c.String(),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.PMMeasureId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .ForeignKey("dbo.PMStrategicGoal", t => t.PMStrategicGoalId)
                .ForeignKey("dbo.AdminTerm", t => t.TermId)
                .Index(t => t.PMStrategicGoalId)
                .Index(t => t.TermId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.PMStrategicGoal",
                c => new
                    {
                        PMStrategicGoalId = c.Int(nullable: false, identity: true),
                        PMReviewId = c.Int(nullable: false),
                        StrategicGoalId = c.Int(nullable: false),
                        StrategicGoalWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.PMStrategicGoalId)
                .ForeignKey("dbo.PMReview", t => t.PMReviewId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .ForeignKey("dbo.AdminStrategicGoal", t => t.StrategicGoalId)
                .Index(t => t.PMReviewId)
                .Index(t => t.StrategicGoalId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.PMReview",
                c => new
                    {
                        PMReviewId = c.Int(nullable: false, identity: true),
                        ReviewReportingStructureId = c.Int(nullable: false),
                        PMReviewPeriodId = c.Int(nullable: false),
                        OverallEmployeeComments = c.String(),
                        OverallLineManagerComments = c.String(),
                        OverallAuditComments = c.String(),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.PMReviewId)
                .ForeignKey("dbo.PMReviewPeriod", t => t.PMReviewPeriodId)
                .ForeignKey("dbo.ReportingStructure", t => t.ReviewReportingStructureId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.ReviewReportingStructureId)
                .Index(t => t.PMReviewPeriodId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.PMReviewPeriod",
                c => new
                    {
                        PMReviewPeriodId = c.Int(nullable: false, identity: true),
                        ReviewPeriodId = c.Int(nullable: false),
                        PerformanceYearId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.PMReviewPeriodId)
                .ForeignKey("dbo.PerformanceYear", t => t.PerformanceYearId)
                .ForeignKey("dbo.AdminReviewPeriod", t => t.ReviewPeriodId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.ReviewPeriodId)
                .Index(t => t.PerformanceYearId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.AdminReviewPeriod",
                c => new
                    {
                        ReviewPeriodId = c.Int(nullable: false, identity: true),
                        ReviewPeriodName = c.String(nullable: false, maxLength: 100),
                        ReviewPeriodCode = c.String(nullable: false, maxLength: 50),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ReviewPeriodId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.ReportingStructure",
                c => new
                    {
                        ReviewReportingStructureId = c.Int(nullable: false, identity: true),
                        MemberId = c.Int(nullable: false),
                        ManagerId = c.Int(nullable: false),
                        DocumentTypeId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ReviewReportingStructureId)
                .ForeignKey("dbo.PMDocumentType", t => t.DocumentTypeId)
                .ForeignKey("dbo.Employee", t => t.ManagerId)
                .ForeignKey("dbo.Employee", t => t.MemberId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.MemberId)
                .Index(t => t.ManagerId)
                .Index(t => t.DocumentTypeId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.Employee",
                c => new
                    {
                        EmployeeRecordId = c.Int(nullable: false, identity: true),
                        EmployeeCode = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Surname = c.String(nullable: false),
                        NetworkUsername = c.String(nullable: false),
                        TeamId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        JobGradeId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.EmployeeRecordId)
                .ForeignKey("dbo.AdminJobGrade", t => t.JobGradeId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .ForeignKey("dbo.Team", t => t.TeamId)
                .Index(t => t.TeamId)
                .Index(t => t.StatusId)
                .Index(t => t.JobGradeId);
            
            CreateTable(
                "dbo.Team",
                c => new
                    {
                        TeamId = c.Int(nullable: false, identity: true),
                        TeamCode = c.String(nullable: false, maxLength: 50),
                        TeamName = c.String(nullable: false, maxLength: 256),
                        DepartmentId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.TeamId)
                .ForeignKey("dbo.StructureDepartment", t => t.DepartmentId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.DepartmentId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.StructureDepartment",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        DepartmentCode = c.String(),
                        DepartmentName = c.String(),
                        PortfolioId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.DepartmentId)
                .ForeignKey("dbo.Portfolio", t => t.PortfolioId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.PortfolioId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.Portfolio",
                c => new
                    {
                        PortfolioId = c.Int(nullable: false, identity: true),
                        PortfolioCode = c.String(nullable: false, maxLength: 50),
                        PortfolioName = c.String(nullable: false, maxLength: 256),
                        StructureOrganisationId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.PortfolioId)
                .ForeignKey("dbo.StructureOrganisation", t => t.StructureOrganisationId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.StructureOrganisationId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.StructureOrganisation",
                c => new
                    {
                        StructureOrganisationId = c.Int(nullable: false, identity: true),
                        OrganisationName = c.String(nullable: false, maxLength: 150),
                        OrganisationCode = c.String(nullable: false, maxLength: 150),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.StructureOrganisationId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.AdminStrategicGoal",
                c => new
                    {
                        StrategicGoalId = c.Int(nullable: false, identity: true),
                        StrategicGoalName = c.String(nullable: false, maxLength: 500),
                        StrategicGoalCode = c.String(nullable: false, maxLength: 150),
                        DefaultOverallWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.StrategicGoalId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.PMObjective",
                c => new
                    {
                        PMObjectiveId = c.Long(nullable: false, identity: true),
                        PMStrategicGoalId = c.Int(nullable: false),
                        ObjectiveName = c.String(),
                        ObjectiveWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.PMObjectiveId)
                .ForeignKey("dbo.PMStrategicGoal", t => t.PMStrategicGoalId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.PMStrategicGoalId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.PMProcessStage",
                c => new
                    {
                        ProcessStageId = c.Int(nullable: false, identity: true),
                        ProcessStageName = c.String(nullable: false, maxLength: 100),
                        ProcessStageCode = c.String(nullable: false, maxLength: 50),
                        ProcessingOrderNumber = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ProcessStageId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.PMReviewProgressStatus",
                c => new
                    {
                        PMReviewProgressStatusId = c.Long(nullable: false, identity: true),
                        PMReviewId = c.Int(nullable: false),
                        ProcessStageId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.PMReviewProgressStatusId)
                .ForeignKey("dbo.PMReview", t => t.PMReviewId)
                .ForeignKey("dbo.PMProcessStage", t => t.ProcessStageId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.PMReviewId)
                .Index(t => t.ProcessStageId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AdminScoreRating",
                c => new
                    {
                        ScoreRatingId = c.Int(nullable: false, identity: true),
                        Rating = c.String(),
                        RatingCode = c.String(),
                        MinScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StatusId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 100),
                        DateDeleted = c.DateTime(),
                        DeletedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ScoreRatingId)
                .ForeignKey("dbo.AdminStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AdminScoreRating", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PMReviewProgressStatus", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.PMReviewProgressStatus", "ProcessStageId", "dbo.PMProcessStage");
            DropForeignKey("dbo.PMReviewProgressStatus", "PMReviewId", "dbo.PMReview");
            DropForeignKey("dbo.PMProcessStage", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.PMObjective", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.PMObjective", "PMStrategicGoalId", "dbo.PMStrategicGoal");
            DropForeignKey("dbo.PMeasure", "TermId", "dbo.AdminTerm");
            DropForeignKey("dbo.PMeasure", "PMStrategicGoalId", "dbo.PMStrategicGoal");
            DropForeignKey("dbo.PMStrategicGoal", "StrategicGoalId", "dbo.AdminStrategicGoal");
            DropForeignKey("dbo.AdminStrategicGoal", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.PMStrategicGoal", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.PMStrategicGoal", "PMReviewId", "dbo.PMReview");
            DropForeignKey("dbo.PMReview", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.PMReview", "ReviewReportingStructureId", "dbo.ReportingStructure");
            DropForeignKey("dbo.ReportingStructure", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.ReportingStructure", "MemberId", "dbo.Employee");
            DropForeignKey("dbo.ReportingStructure", "ManagerId", "dbo.Employee");
            DropForeignKey("dbo.Employee", "TeamId", "dbo.Team");
            DropForeignKey("dbo.Team", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.Team", "DepartmentId", "dbo.StructureDepartment");
            DropForeignKey("dbo.StructureDepartment", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.StructureDepartment", "PortfolioId", "dbo.Portfolio");
            DropForeignKey("dbo.Portfolio", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.Portfolio", "StructureOrganisationId", "dbo.StructureOrganisation");
            DropForeignKey("dbo.StructureOrganisation", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.Employee", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.Employee", "JobGradeId", "dbo.AdminJobGrade");
            DropForeignKey("dbo.ReportingStructure", "DocumentTypeId", "dbo.PMDocumentType");
            DropForeignKey("dbo.PMReview", "PMReviewPeriodId", "dbo.PMReviewPeriod");
            DropForeignKey("dbo.PMReviewPeriod", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.PMReviewPeriod", "ReviewPeriodId", "dbo.AdminReviewPeriod");
            DropForeignKey("dbo.AdminReviewPeriod", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.PMReviewPeriod", "PerformanceYearId", "dbo.PerformanceYear");
            DropForeignKey("dbo.PMeasure", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.PMDocumentType", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.PerformanceYear", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.AdminObjective", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.AdminMeasure", "TermId", "dbo.AdminTerm");
            DropForeignKey("dbo.AdminTerm", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.AdminMeasure", "StatusId", "dbo.AdminStatus");
            DropForeignKey("dbo.AdminJobGrade", "StatusId", "dbo.AdminStatus");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AdminScoreRating", new[] { "StatusId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.PMReviewProgressStatus", new[] { "StatusId" });
            DropIndex("dbo.PMReviewProgressStatus", new[] { "ProcessStageId" });
            DropIndex("dbo.PMReviewProgressStatus", new[] { "PMReviewId" });
            DropIndex("dbo.PMProcessStage", new[] { "StatusId" });
            DropIndex("dbo.PMObjective", new[] { "StatusId" });
            DropIndex("dbo.PMObjective", new[] { "PMStrategicGoalId" });
            DropIndex("dbo.AdminStrategicGoal", new[] { "StatusId" });
            DropIndex("dbo.StructureOrganisation", new[] { "StatusId" });
            DropIndex("dbo.Portfolio", new[] { "StatusId" });
            DropIndex("dbo.Portfolio", new[] { "StructureOrganisationId" });
            DropIndex("dbo.StructureDepartment", new[] { "StatusId" });
            DropIndex("dbo.StructureDepartment", new[] { "PortfolioId" });
            DropIndex("dbo.Team", new[] { "StatusId" });
            DropIndex("dbo.Team", new[] { "DepartmentId" });
            DropIndex("dbo.Employee", new[] { "JobGradeId" });
            DropIndex("dbo.Employee", new[] { "StatusId" });
            DropIndex("dbo.Employee", new[] { "TeamId" });
            DropIndex("dbo.ReportingStructure", new[] { "StatusId" });
            DropIndex("dbo.ReportingStructure", new[] { "DocumentTypeId" });
            DropIndex("dbo.ReportingStructure", new[] { "ManagerId" });
            DropIndex("dbo.ReportingStructure", new[] { "MemberId" });
            DropIndex("dbo.AdminReviewPeriod", new[] { "StatusId" });
            DropIndex("dbo.PMReviewPeriod", new[] { "StatusId" });
            DropIndex("dbo.PMReviewPeriod", new[] { "PerformanceYearId" });
            DropIndex("dbo.PMReviewPeriod", new[] { "ReviewPeriodId" });
            DropIndex("dbo.PMReview", new[] { "StatusId" });
            DropIndex("dbo.PMReview", new[] { "PMReviewPeriodId" });
            DropIndex("dbo.PMReview", new[] { "ReviewReportingStructureId" });
            DropIndex("dbo.PMStrategicGoal", new[] { "StatusId" });
            DropIndex("dbo.PMStrategicGoal", new[] { "StrategicGoalId" });
            DropIndex("dbo.PMStrategicGoal", new[] { "PMReviewId" });
            DropIndex("dbo.PMeasure", new[] { "StatusId" });
            DropIndex("dbo.PMeasure", new[] { "TermId" });
            DropIndex("dbo.PMeasure", new[] { "PMStrategicGoalId" });
            DropIndex("dbo.PMDocumentType", new[] { "StatusId" });
            DropIndex("dbo.PerformanceYear", new[] { "StatusId" });
            DropIndex("dbo.AdminObjective", new[] { "StatusId" });
            DropIndex("dbo.AdminTerm", new[] { "StatusId" });
            DropIndex("dbo.AdminMeasure", new[] { "StatusId" });
            DropIndex("dbo.AdminMeasure", new[] { "TermId" });
            DropIndex("dbo.AdminJobGrade", new[] { "StatusId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AdminScoreRating");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.PMReviewProgressStatus");
            DropTable("dbo.PMProcessStage");
            DropTable("dbo.PMObjective");
            DropTable("dbo.AdminStrategicGoal");
            DropTable("dbo.StructureOrganisation");
            DropTable("dbo.Portfolio");
            DropTable("dbo.StructureDepartment");
            DropTable("dbo.Team");
            DropTable("dbo.Employee");
            DropTable("dbo.ReportingStructure");
            DropTable("dbo.AdminReviewPeriod");
            DropTable("dbo.PMReviewPeriod");
            DropTable("dbo.PMReview");
            DropTable("dbo.PMStrategicGoal");
            DropTable("dbo.PMeasure");
            DropTable("dbo.PMDocumentType");
            DropTable("dbo.PerformanceYear");
            DropTable("dbo.AdminObjective");
            DropTable("dbo.AdminTerm");
            DropTable("dbo.AdminMeasure");
            DropTable("dbo.AdminStatus");
            DropTable("dbo.AdminJobGrade");
        }
    }
}
