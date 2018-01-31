namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Salary1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Web.SalaryHeaders",
                c => new
                    {
                        SalaryHeaderId = c.Int(nullable: false, identity: true),
                        DocTypeId = c.Int(nullable: false),
                        DocDate = c.DateTime(nullable: false),
                        DocNo = c.String(maxLength: 20),
                        DivisionId = c.Int(nullable: false),
                        SiteId = c.Int(nullable: false),
                        Remark = c.String(),
                        Status = c.Int(nullable: false),
                        ReviewCount = c.Int(),
                        ReviewBy = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                        LockReason = c.String(),
                        OMSId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.SalaryHeaderId)
                .ForeignKey("Web.Divisions", t => t.DivisionId)
                .ForeignKey("Web.DocumentTypes", t => t.DocTypeId)
                .ForeignKey("Web.Sites", t => t.SiteId)
                .Index(t => t.DocTypeId)
                .Index(t => t.DivisionId)
                .Index(t => t.SiteId);
            
            CreateTable(
                "Web.SalaryHeaderCharges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HeaderTableId = c.Int(nullable: false),
                        Sr = c.Int(nullable: false),
                        ChargeId = c.Int(nullable: false),
                        AddDeduct = c.Byte(),
                        AffectCost = c.Boolean(nullable: false),
                        ChargeTypeId = c.Int(),
                        ProductChargeId = c.Int(),
                        CalculateOnId = c.Int(),
                        PersonID = c.Int(),
                        LedgerAccountDrId = c.Int(),
                        LedgerAccountCrId = c.Int(),
                        ContraLedgerAccountId = c.Int(),
                        CostCenterId = c.Int(),
                        RateType = c.Byte(nullable: false),
                        IncludedInBase = c.Boolean(nullable: false),
                        ParentChargeId = c.Int(),
                        Rate = c.Decimal(precision: 18, scale: 4),
                        Amount = c.Decimal(precision: 18, scale: 4),
                        IsVisible = c.Boolean(nullable: false),
                        IncludedCharges = c.String(),
                        IncludedChargesCalculation = c.String(),
                        OMSId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Web.Charges", t => t.CalculateOnId)
                .ForeignKey("Web.Charges", t => t.ChargeId)
                .ForeignKey("Web.ChargeTypes", t => t.ChargeTypeId)
                .ForeignKey("Web.LedgerAccounts", t => t.ContraLedgerAccountId)
                .ForeignKey("Web.CostCenters", t => t.CostCenterId)
                .ForeignKey("Web.LedgerAccounts", t => t.LedgerAccountCrId)
                .ForeignKey("Web.LedgerAccounts", t => t.LedgerAccountDrId)
                .ForeignKey("Web.Charges", t => t.ParentChargeId)
                .ForeignKey("Web.People", t => t.PersonID)
                .ForeignKey("Web.Charges", t => t.ProductChargeId)
                .ForeignKey("Web.SalaryHeaders", t => t.HeaderTableId)
                .Index(t => t.HeaderTableId)
                .Index(t => t.ChargeId)
                .Index(t => t.ChargeTypeId)
                .Index(t => t.ProductChargeId)
                .Index(t => t.CalculateOnId)
                .Index(t => t.PersonID)
                .Index(t => t.LedgerAccountDrId)
                .Index(t => t.LedgerAccountCrId)
                .Index(t => t.ContraLedgerAccountId)
                .Index(t => t.CostCenterId)
                .Index(t => t.ParentChargeId);
            
            CreateTable(
                "Web.SalaryLines",
                c => new
                    {
                        SalaryLineId = c.Int(nullable: false, identity: true),
                        SalaryHeaderId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Days = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OtherAddition = c.Decimal(precision: 18, scale: 4),
                        OtherDeduction = c.Decimal(precision: 18, scale: 4),
                        LoadEMI = c.Decimal(precision: 18, scale: 4),
                        NetSalary = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Remark = c.String(),
                        Sr = c.Int(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        LockReason = c.String(),
                        OMSId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.SalaryLineId)
                .ForeignKey("Web.Employees", t => t.EmployeeId)
                .ForeignKey("Web.SalaryHeaders", t => t.SalaryHeaderId)
                .Index(t => new { t.SalaryHeaderId, t.EmployeeId }, unique: true, name: "IX_SalaryLine_Unique");
            
            CreateTable(
                "Web.SalaryLineCharges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LineTableId = c.Int(nullable: false),
                        HeaderTableId = c.Int(nullable: false),
                        Sr = c.Int(nullable: false),
                        ChargeId = c.Int(nullable: false),
                        AddDeduct = c.Byte(),
                        AffectCost = c.Boolean(nullable: false),
                        ChargeTypeId = c.Int(),
                        CalculateOnId = c.Int(),
                        PersonID = c.Int(),
                        LedgerAccountDrId = c.Int(),
                        LedgerAccountCrId = c.Int(),
                        ContraLedgerAccountId = c.Int(),
                        CostCenterId = c.Int(),
                        RateType = c.Byte(nullable: false),
                        IncludedInBase = c.Boolean(nullable: false),
                        ParentChargeId = c.Int(),
                        Rate = c.Decimal(precision: 18, scale: 4),
                        Amount = c.Decimal(precision: 18, scale: 4),
                        DealQty = c.Decimal(precision: 18, scale: 4),
                        IsVisible = c.Boolean(nullable: false),
                        IncludedCharges = c.String(),
                        IncludedChargesCalculation = c.String(),
                        IsVisibleLedgerAccountDr = c.Boolean(),
                        filterLedgerAccountGroupsDrId = c.Int(),
                        IsVisibleLedgerAccountCr = c.Boolean(),
                        filterLedgerAccountGroupsCrId = c.Int(),
                        OMSId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Web.Charges", t => t.CalculateOnId)
                .ForeignKey("Web.Charges", t => t.ChargeId)
                .ForeignKey("Web.ChargeTypes", t => t.ChargeTypeId)
                .ForeignKey("Web.LedgerAccounts", t => t.ContraLedgerAccountId)
                .ForeignKey("Web.CostCenters", t => t.CostCenterId)
                .ForeignKey("Web.LedgerAccountGroups", t => t.filterLedgerAccountGroupsCrId)
                .ForeignKey("Web.LedgerAccountGroups", t => t.filterLedgerAccountGroupsDrId)
                .ForeignKey("Web.LedgerAccounts", t => t.LedgerAccountCrId)
                .ForeignKey("Web.LedgerAccounts", t => t.LedgerAccountDrId)
                .ForeignKey("Web.Charges", t => t.ParentChargeId)
                .ForeignKey("Web.People", t => t.PersonID)
                .ForeignKey("Web.SalaryLines", t => t.LineTableId)
                .Index(t => t.LineTableId)
                .Index(t => t.ChargeId)
                .Index(t => t.ChargeTypeId)
                .Index(t => t.CalculateOnId)
                .Index(t => t.PersonID)
                .Index(t => t.LedgerAccountDrId)
                .Index(t => t.LedgerAccountCrId)
                .Index(t => t.ContraLedgerAccountId)
                .Index(t => t.CostCenterId)
                .Index(t => t.ParentChargeId)
                .Index(t => t.filterLedgerAccountGroupsDrId)
                .Index(t => t.filterLedgerAccountGroupsCrId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Web.SalaryLineCharges", "LineTableId", "Web.SalaryLines");
            DropForeignKey("Web.SalaryLineCharges", "PersonID", "Web.People");
            DropForeignKey("Web.SalaryLineCharges", "ParentChargeId", "Web.Charges");
            DropForeignKey("Web.SalaryLineCharges", "LedgerAccountDrId", "Web.LedgerAccounts");
            DropForeignKey("Web.SalaryLineCharges", "LedgerAccountCrId", "Web.LedgerAccounts");
            DropForeignKey("Web.SalaryLineCharges", "filterLedgerAccountGroupsDrId", "Web.LedgerAccountGroups");
            DropForeignKey("Web.SalaryLineCharges", "filterLedgerAccountGroupsCrId", "Web.LedgerAccountGroups");
            DropForeignKey("Web.SalaryLineCharges", "CostCenterId", "Web.CostCenters");
            DropForeignKey("Web.SalaryLineCharges", "ContraLedgerAccountId", "Web.LedgerAccounts");
            DropForeignKey("Web.SalaryLineCharges", "ChargeTypeId", "Web.ChargeTypes");
            DropForeignKey("Web.SalaryLineCharges", "ChargeId", "Web.Charges");
            DropForeignKey("Web.SalaryLineCharges", "CalculateOnId", "Web.Charges");
            DropForeignKey("Web.SalaryLines", "SalaryHeaderId", "Web.SalaryHeaders");
            DropForeignKey("Web.SalaryLines", "EmployeeId", "Web.Employees");
            DropForeignKey("Web.SalaryHeaderCharges", "HeaderTableId", "Web.SalaryHeaders");
            DropForeignKey("Web.SalaryHeaderCharges", "ProductChargeId", "Web.Charges");
            DropForeignKey("Web.SalaryHeaderCharges", "PersonID", "Web.People");
            DropForeignKey("Web.SalaryHeaderCharges", "ParentChargeId", "Web.Charges");
            DropForeignKey("Web.SalaryHeaderCharges", "LedgerAccountDrId", "Web.LedgerAccounts");
            DropForeignKey("Web.SalaryHeaderCharges", "LedgerAccountCrId", "Web.LedgerAccounts");
            DropForeignKey("Web.SalaryHeaderCharges", "CostCenterId", "Web.CostCenters");
            DropForeignKey("Web.SalaryHeaderCharges", "ContraLedgerAccountId", "Web.LedgerAccounts");
            DropForeignKey("Web.SalaryHeaderCharges", "ChargeTypeId", "Web.ChargeTypes");
            DropForeignKey("Web.SalaryHeaderCharges", "ChargeId", "Web.Charges");
            DropForeignKey("Web.SalaryHeaderCharges", "CalculateOnId", "Web.Charges");
            DropForeignKey("Web.SalaryHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SalaryHeaders", "DocTypeId", "Web.DocumentTypes");
            DropForeignKey("Web.SalaryHeaders", "DivisionId", "Web.Divisions");
            DropIndex("Web.SalaryLineCharges", new[] { "filterLedgerAccountGroupsCrId" });
            DropIndex("Web.SalaryLineCharges", new[] { "filterLedgerAccountGroupsDrId" });
            DropIndex("Web.SalaryLineCharges", new[] { "ParentChargeId" });
            DropIndex("Web.SalaryLineCharges", new[] { "CostCenterId" });
            DropIndex("Web.SalaryLineCharges", new[] { "ContraLedgerAccountId" });
            DropIndex("Web.SalaryLineCharges", new[] { "LedgerAccountCrId" });
            DropIndex("Web.SalaryLineCharges", new[] { "LedgerAccountDrId" });
            DropIndex("Web.SalaryLineCharges", new[] { "PersonID" });
            DropIndex("Web.SalaryLineCharges", new[] { "CalculateOnId" });
            DropIndex("Web.SalaryLineCharges", new[] { "ChargeTypeId" });
            DropIndex("Web.SalaryLineCharges", new[] { "ChargeId" });
            DropIndex("Web.SalaryLineCharges", new[] { "LineTableId" });
            DropIndex("Web.SalaryLines", "IX_SalaryLine_Unique");
            DropIndex("Web.SalaryHeaderCharges", new[] { "ParentChargeId" });
            DropIndex("Web.SalaryHeaderCharges", new[] { "CostCenterId" });
            DropIndex("Web.SalaryHeaderCharges", new[] { "ContraLedgerAccountId" });
            DropIndex("Web.SalaryHeaderCharges", new[] { "LedgerAccountCrId" });
            DropIndex("Web.SalaryHeaderCharges", new[] { "LedgerAccountDrId" });
            DropIndex("Web.SalaryHeaderCharges", new[] { "PersonID" });
            DropIndex("Web.SalaryHeaderCharges", new[] { "CalculateOnId" });
            DropIndex("Web.SalaryHeaderCharges", new[] { "ProductChargeId" });
            DropIndex("Web.SalaryHeaderCharges", new[] { "ChargeTypeId" });
            DropIndex("Web.SalaryHeaderCharges", new[] { "ChargeId" });
            DropIndex("Web.SalaryHeaderCharges", new[] { "HeaderTableId" });
            DropIndex("Web.SalaryHeaders", new[] { "SiteId" });
            DropIndex("Web.SalaryHeaders", new[] { "DivisionId" });
            DropIndex("Web.SalaryHeaders", new[] { "DocTypeId" });
            DropTable("Web.SalaryLineCharges");
            DropTable("Web.SalaryLines");
            DropTable("Web.SalaryHeaderCharges");
            DropTable("Web.SalaryHeaders");
        }
    }
}
