namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeLineCharge : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Web.EmployeeLineCharges",
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
                .ForeignKey("Web.Employees", t => t.LineTableId)
                .ForeignKey("Web.LedgerAccountGroups", t => t.filterLedgerAccountGroupsCrId)
                .ForeignKey("Web.LedgerAccountGroups", t => t.filterLedgerAccountGroupsDrId)
                .ForeignKey("Web.LedgerAccounts", t => t.LedgerAccountCrId)
                .ForeignKey("Web.LedgerAccounts", t => t.LedgerAccountDrId)
                .ForeignKey("Web.Charges", t => t.ParentChargeId)
                .ForeignKey("Web.People", t => t.PersonID)
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
            DropForeignKey("Web.EmployeeLineCharges", "PersonID", "Web.People");
            DropForeignKey("Web.EmployeeLineCharges", "ParentChargeId", "Web.Charges");
            DropForeignKey("Web.EmployeeLineCharges", "LedgerAccountDrId", "Web.LedgerAccounts");
            DropForeignKey("Web.EmployeeLineCharges", "LedgerAccountCrId", "Web.LedgerAccounts");
            DropForeignKey("Web.EmployeeLineCharges", "filterLedgerAccountGroupsDrId", "Web.LedgerAccountGroups");
            DropForeignKey("Web.EmployeeLineCharges", "filterLedgerAccountGroupsCrId", "Web.LedgerAccountGroups");
            DropForeignKey("Web.EmployeeLineCharges", "LineTableId", "Web.Employees");
            DropForeignKey("Web.EmployeeLineCharges", "CostCenterId", "Web.CostCenters");
            DropForeignKey("Web.EmployeeLineCharges", "ContraLedgerAccountId", "Web.LedgerAccounts");
            DropForeignKey("Web.EmployeeLineCharges", "ChargeTypeId", "Web.ChargeTypes");
            DropForeignKey("Web.EmployeeLineCharges", "ChargeId", "Web.Charges");
            DropForeignKey("Web.EmployeeLineCharges", "CalculateOnId", "Web.Charges");
            DropIndex("Web.EmployeeLineCharges", new[] { "filterLedgerAccountGroupsCrId" });
            DropIndex("Web.EmployeeLineCharges", new[] { "filterLedgerAccountGroupsDrId" });
            DropIndex("Web.EmployeeLineCharges", new[] { "ParentChargeId" });
            DropIndex("Web.EmployeeLineCharges", new[] { "CostCenterId" });
            DropIndex("Web.EmployeeLineCharges", new[] { "ContraLedgerAccountId" });
            DropIndex("Web.EmployeeLineCharges", new[] { "LedgerAccountCrId" });
            DropIndex("Web.EmployeeLineCharges", new[] { "LedgerAccountDrId" });
            DropIndex("Web.EmployeeLineCharges", new[] { "PersonID" });
            DropIndex("Web.EmployeeLineCharges", new[] { "CalculateOnId" });
            DropIndex("Web.EmployeeLineCharges", new[] { "ChargeTypeId" });
            DropIndex("Web.EmployeeLineCharges", new[] { "ChargeId" });
            DropIndex("Web.EmployeeLineCharges", new[] { "LineTableId" });
            DropTable("Web.EmployeeLineCharges");
        }
    }
}
