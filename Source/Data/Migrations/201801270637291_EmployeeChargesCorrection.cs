namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeChargesCorrection : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Web.States", "Calculation_CalculationId", "Web.Calculations");
            DropForeignKey("Web.CalculationFooters", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.CalculationHeaderLedgerAccounts", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.CalculationLineLedgerAccounts", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.CalculationProducts", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.ChargeGroupPersonCalculations", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.JobInvoiceSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.JobOrderSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.JobReceiveSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.PersonSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.PurchaseInvoiceSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.PurchaseOrderSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.PurchaseQuotationSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.SaleEnquirySettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.SaleInvoiceSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.SaleOrderSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.SaleQuotationSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.CalculationHeaderLedgerAccounts", "CalculationFooterId", "Web.CalculationFooters");
            DropForeignKey("Web.CalculationLineLedgerAccounts", "CalculationProductId", "Web.CalculationProducts");
            DropColumn("Web.EmployeeCharges", "HeaderTableId");
            RenameColumn(table: "Web.EmployeeCharges", name: "EmployeeId", newName: "HeaderTableId");
            RenameIndex(table: "Web.EmployeeCharges", name: "IX_EmployeeId", newName: "IX_HeaderTableId");
            DropPrimaryKey("Web.Calculations");
            DropPrimaryKey("Web.CalculationFooters");
            DropPrimaryKey("Web.CalculationProducts");
            AlterColumn("Web.Calculations", "CalculationId", c => c.Int(nullable: false, identity: true));
            AlterColumn("Web.CalculationFooters", "CalculationFooterLineId", c => c.Int(nullable: false, identity: true));
            AlterColumn("Web.CalculationProducts", "CalculationProductId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("Web.Calculations", "CalculationId");
            AddPrimaryKey("Web.CalculationFooters", "CalculationFooterLineId");
            AddPrimaryKey("Web.CalculationProducts", "CalculationProductId");
            AddForeignKey("Web.States", "Calculation_CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.CalculationFooters", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.CalculationHeaderLedgerAccounts", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.CalculationLineLedgerAccounts", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.CalculationProducts", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.ChargeGroupPersonCalculations", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.JobInvoiceSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.JobOrderSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.JobReceiveSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.PersonSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.PurchaseInvoiceSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.PurchaseOrderSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.PurchaseQuotationSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.SaleEnquirySettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.SaleInvoiceSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.SaleOrderSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.SaleQuotationSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.CalculationHeaderLedgerAccounts", "CalculationFooterId", "Web.CalculationFooters", "CalculationFooterLineId");
            AddForeignKey("Web.CalculationLineLedgerAccounts", "CalculationProductId", "Web.CalculationProducts", "CalculationProductId");
        }
        
        public override void Down()
        {
            DropForeignKey("Web.CalculationLineLedgerAccounts", "CalculationProductId", "Web.CalculationProducts");
            DropForeignKey("Web.CalculationHeaderLedgerAccounts", "CalculationFooterId", "Web.CalculationFooters");
            DropForeignKey("Web.SaleQuotationSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.SaleOrderSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.SaleInvoiceSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.SaleEnquirySettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.PurchaseQuotationSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.PurchaseOrderSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.PurchaseInvoiceSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.PersonSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.JobReceiveSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.JobOrderSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.JobInvoiceSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.ChargeGroupPersonCalculations", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.CalculationProducts", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.CalculationLineLedgerAccounts", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.CalculationHeaderLedgerAccounts", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.CalculationFooters", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.States", "Calculation_CalculationId", "Web.Calculations");
            DropPrimaryKey("Web.CalculationProducts");
            DropPrimaryKey("Web.CalculationFooters");
            DropPrimaryKey("Web.Calculations");
            AlterColumn("Web.CalculationProducts", "CalculationProductId", c => c.Int(nullable: false));
            AlterColumn("Web.CalculationFooters", "CalculationFooterLineId", c => c.Int(nullable: false));
            AlterColumn("Web.Calculations", "CalculationId", c => c.Int(nullable: false));
            AddPrimaryKey("Web.CalculationProducts", "CalculationProductId");
            AddPrimaryKey("Web.CalculationFooters", "CalculationFooterLineId");
            AddPrimaryKey("Web.Calculations", "CalculationId");
            RenameIndex(table: "Web.EmployeeCharges", name: "IX_HeaderTableId", newName: "IX_EmployeeId");
            RenameColumn(table: "Web.EmployeeCharges", name: "HeaderTableId", newName: "EmployeeId");
            AddColumn("Web.EmployeeCharges", "HeaderTableId", c => c.Int(nullable: false));
            AddForeignKey("Web.CalculationLineLedgerAccounts", "CalculationProductId", "Web.CalculationProducts", "CalculationProductId");
            AddForeignKey("Web.CalculationHeaderLedgerAccounts", "CalculationFooterId", "Web.CalculationFooters", "CalculationFooterLineId");
            AddForeignKey("Web.SaleQuotationSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.SaleOrderSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.SaleInvoiceSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.SaleEnquirySettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.PurchaseQuotationSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.PurchaseOrderSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.PurchaseInvoiceSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.PersonSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.JobReceiveSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.JobOrderSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.JobInvoiceSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.ChargeGroupPersonCalculations", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.CalculationProducts", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.CalculationLineLedgerAccounts", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.CalculationHeaderLedgerAccounts", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.CalculationFooters", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.States", "Calculation_CalculationId", "Web.Calculations", "CalculationId");
        }
    }
}
