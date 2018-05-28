namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LedgerSupplmentry : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Web.PersonAddresses", "CityId", "Web.Cities");
            DropForeignKey("Web.Sites", "CityId", "Web.Cities");
            DropForeignKey("Web.PurchaseWaybills", "FromCityId", "Web.Cities");
            DropForeignKey("Web.PurchaseWaybills", "ToCityId", "Web.Cities");
            DropForeignKey("Web.Companies", "CityId", "Web.Cities");
            DropForeignKey("Web.DispatchWaybillHeaders", "FromCityId", "Web.Cities");
            DropForeignKey("Web.DispatchWaybillHeaders", "ToCityId", "Web.Cities");
            DropForeignKey("Web.DispatchWaybillLines", "CityId", "Web.Cities");
            DropForeignKey("Web.RouteLines", "CityId", "Web.Cities");
            DropPrimaryKey("Web.Cities");
            DropPrimaryKey("Web.ChargeGroupSettings");
            CreateTable(
                "Web.LedgerHeaderCharges",
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
                .ForeignKey("Web.LedgerHeaders", t => t.HeaderTableId)
                .ForeignKey("Web.Charges", t => t.ParentChargeId)
                .ForeignKey("Web.People", t => t.PersonID)
                .ForeignKey("Web.Charges", t => t.ProductChargeId)
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
                "Web.LedgerLineCharges",
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
                .ForeignKey("Web.LedgerLines", t => t.LineTableId)
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
            
            CreateTable(
                "Web.LedgerSupplementaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LedgerId = c.Int(nullable: false),
                        SupplementaryLedgerId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        OMSId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Web.Ledgers", t => t.LedgerId)
                .ForeignKey("Web.Ledgers", t => t.SupplementaryLedgerId)
                .Index(t => t.LedgerId)
                .Index(t => t.SupplementaryLedgerId);
            
            CreateTable(
                "Web.SalaryLineReferences",
                c => new
                    {
                        SalaryLineId = c.Int(nullable: false),
                        ReferenceDocTypeId = c.Int(nullable: false),
                        ReferenceDocId = c.Int(nullable: false),
                        ReferenceDocLineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SalaryLineId, t.ReferenceDocTypeId, t.ReferenceDocId, t.ReferenceDocLineId })
                .ForeignKey("Web.DocumentTypes", t => t.ReferenceDocTypeId)
                .ForeignKey("Web.SalaryLines", t => t.SalaryLineId)
                .Index(t => t.SalaryLineId)
                .Index(t => t.ReferenceDocTypeId);
            
            AddColumn("Web.Processes", "DepartmentId", c => c.Int());
            AddColumn("Web.Employees", "DateOfJoining", c => c.DateTime());
            AddColumn("Web.Employees", "DateOfRelieving", c => c.DateTime());
            AddColumn("Web.Employees", "WagesPayType", c => c.String(maxLength: 10));
            AddColumn("Web.Employees", "PaymentType", c => c.String(maxLength: 10));
            AddColumn("Web.LedgerHeaders", "IsDocumentPrinted", c => c.Boolean());
            AddColumn("Web.JobOrderLines", "UnitConversionForId", c => c.Byte());
            AddColumn("Web.ReportHeaders", "IsHideHeaderDetail", c => c.Boolean());
            AddColumn("Web.JobInvoiceHeaders", "IsDocumentPrinted", c => c.Boolean());
            AddColumn("Web.JobInvoiceSettings", "isAllowedDuplicatePrint", c => c.Boolean());
            AddColumn("Web.JobInvoiceSettings", "JobInvoiceReturnDocTypeId", c => c.Int());
            AddColumn("Web.JobOrderSettings", "isVisibleLineUnitConversionFor", c => c.Boolean(nullable: false));
            AddColumn("Web.JobOrderSettings", "filterUnitConversionFors", c => c.String());
            AddColumn("Web.JobReceiveSettings", "isMandatoryLotNo", c => c.Boolean());
            AddColumn("Web.JobReceiveSettings", "isMandatoryLotNoOrDimension1", c => c.Boolean());
            AddColumn("Web.LedgerLines", "Qty", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("Web.LedgerLines", "DealQty", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("Web.LedgerLines", "Rate", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("Web.LedgerLines", "Specification", c => c.String(maxLength: 50));
            AddColumn("Web.LedgerLines", "SupplementaryLedgerId", c => c.Int());
            AddColumn("Web.LedgerSettings", "CalculationId", c => c.Int());
            AddColumn("Web.LedgerSettings", "isVisibleQty", c => c.Boolean());
            AddColumn("Web.LedgerSettings", "isVisibleDealQty", c => c.Boolean());
            AddColumn("Web.LedgerSettings", "isVisibleRate", c => c.Boolean());
            AddColumn("Web.LedgerSettings", "isVisibleSpecification", c => c.Boolean());
            AddColumn("Web.LedgerSettings", "isAllowedDuplicatePrint", c => c.Boolean());
            AddColumn("Web.LedgerSettings", "isPrintinLetterhead", c => c.Boolean());
            AddColumn("Web.StockHeaderSettings", "isMandatoryLotNo", c => c.Boolean());
            AddColumn("Web.StockHeaderSettings", "isMandatoryLotNoOrDimension1", c => c.Boolean());
            AddColumn("Web.PackingSettings", "isAllowtoUpdateBuyerSpecification", c => c.Boolean());
            AddColumn("Web.PackingSettings", "isVisibleHeaderJobWorker", c => c.Boolean());
            AddColumn("Web.PackingSettings", "isVisibleBaleNoPattern", c => c.Boolean());
            AddColumn("Web.PackingSettings", "isVisibleGrossWeight", c => c.Boolean());
            AddColumn("Web.PackingSettings", "isVisibleNetWeight", c => c.Boolean());
            AddColumn("Web.PackingSettings", "isVisibleProductInvoiceGroup", c => c.Boolean());
            AddColumn("Web.PackingSettings", "isVisibleSaleDeliveryOrder", c => c.Boolean());
            AddColumn("Web.ProductBuyerLogs", "WaterTaxAmount", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("Web.ProductBuyerLogs", "WaterTaxPercentage", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("Web.SalaryHeaders", "WagesPayType", c => c.String(maxLength: 20));
            AddColumn("Web.SalaryHeaders", "LedgerHeaderId", c => c.Int());
            AddColumn("Web.SalaryHeaders", "IsDocumentPrinted", c => c.Boolean());
            AddColumn("Web.SalaryLines", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Web.SalaryLines", "LoanEMI", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("Web.SalaryLines", "Advance", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("Web.SalaryLines", "NetPayable", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Web.SalarySettings", "isAllowedDuplicatePrint", c => c.Boolean());
            AddColumn("Web.SalarySettings", "isPrintinLetterhead", c => c.Boolean());
            AlterColumn("Web.Cities", "CityId", c => c.Int(nullable: false, identity: true));
            AlterColumn("Web.ChargeGroupSettings", "ChargeGroupSettingsId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("Web.Cities", "CityId");
            AddPrimaryKey("Web.ChargeGroupSettings", "ChargeGroupSettingsId");
            CreateIndex("Web.Processes", "DepartmentId");
            CreateIndex("Web.JobOrderLines", "UnitConversionForId");
            CreateIndex("Web.JobInvoiceReturnLineCharges", "HeaderTableId");
            CreateIndex("Web.JobInvoiceSettings", "JobInvoiceReturnDocTypeId");
            CreateIndex("Web.LedgerSettings", "CalculationId");
            CreateIndex("Web.SalaryHeaders", "LedgerHeaderId");
            CreateIndex("Web.SalaryLineCharges", "HeaderTableId");
            AddForeignKey("Web.Processes", "DepartmentId", "Web.Departments", "DepartmentId");
            AddForeignKey("Web.JobOrderLines", "UnitConversionForId", "Web.UnitConversionFors", "UnitconversionForId");
            AddForeignKey("Web.JobInvoiceReturnLineCharges", "HeaderTableId", "Web.JobInvoiceReturnHeaders", "JobInvoiceReturnHeaderId");
            AddForeignKey("Web.JobInvoiceSettings", "JobInvoiceReturnDocTypeId", "Web.DocumentTypes", "DocumentTypeId");
            AddForeignKey("Web.LedgerSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.SalaryHeaders", "LedgerHeaderId", "Web.LedgerHeaders", "LedgerHeaderId");
            AddForeignKey("Web.SalaryLineCharges", "HeaderTableId", "Web.SalaryHeaders", "SalaryHeaderId");
            AddForeignKey("Web.PersonAddresses", "CityId", "Web.Cities", "CityId");
            AddForeignKey("Web.Sites", "CityId", "Web.Cities", "CityId");
            AddForeignKey("Web.PurchaseWaybills", "FromCityId", "Web.Cities", "CityId");
            AddForeignKey("Web.PurchaseWaybills", "ToCityId", "Web.Cities", "CityId");
            AddForeignKey("Web.Companies", "CityId", "Web.Cities", "CityId");
            AddForeignKey("Web.DispatchWaybillHeaders", "FromCityId", "Web.Cities", "CityId");
            AddForeignKey("Web.DispatchWaybillHeaders", "ToCityId", "Web.Cities", "CityId");
            AddForeignKey("Web.DispatchWaybillLines", "CityId", "Web.Cities", "CityId");
            AddForeignKey("Web.RouteLines", "CityId", "Web.Cities", "CityId");
            DropColumn("Web.SalaryLines", "LoadEMI");
            DropColumn("Web.SalaryLines", "NetSalary");
        }
        
        public override void Down()
        {
            AddColumn("Web.SalaryLines", "NetSalary", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Web.SalaryLines", "LoadEMI", c => c.Decimal(precision: 18, scale: 4));
            DropForeignKey("Web.RouteLines", "CityId", "Web.Cities");
            DropForeignKey("Web.DispatchWaybillLines", "CityId", "Web.Cities");
            DropForeignKey("Web.DispatchWaybillHeaders", "ToCityId", "Web.Cities");
            DropForeignKey("Web.DispatchWaybillHeaders", "FromCityId", "Web.Cities");
            DropForeignKey("Web.Companies", "CityId", "Web.Cities");
            DropForeignKey("Web.PurchaseWaybills", "ToCityId", "Web.Cities");
            DropForeignKey("Web.PurchaseWaybills", "FromCityId", "Web.Cities");
            DropForeignKey("Web.Sites", "CityId", "Web.Cities");
            DropForeignKey("Web.PersonAddresses", "CityId", "Web.Cities");
            DropForeignKey("Web.SalaryLineReferences", "SalaryLineId", "Web.SalaryLines");
            DropForeignKey("Web.SalaryLineReferences", "ReferenceDocTypeId", "Web.DocumentTypes");
            DropForeignKey("Web.SalaryLineCharges", "HeaderTableId", "Web.SalaryHeaders");
            DropForeignKey("Web.SalaryHeaders", "LedgerHeaderId", "Web.LedgerHeaders");
            DropForeignKey("Web.LedgerSupplementaries", "SupplementaryLedgerId", "Web.Ledgers");
            DropForeignKey("Web.LedgerSupplementaries", "LedgerId", "Web.Ledgers");
            DropForeignKey("Web.LedgerSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.LedgerLineCharges", "PersonID", "Web.People");
            DropForeignKey("Web.LedgerLineCharges", "ParentChargeId", "Web.Charges");
            DropForeignKey("Web.LedgerLineCharges", "LineTableId", "Web.LedgerLines");
            DropForeignKey("Web.LedgerLineCharges", "LedgerAccountDrId", "Web.LedgerAccounts");
            DropForeignKey("Web.LedgerLineCharges", "LedgerAccountCrId", "Web.LedgerAccounts");
            DropForeignKey("Web.LedgerLineCharges", "filterLedgerAccountGroupsDrId", "Web.LedgerAccountGroups");
            DropForeignKey("Web.LedgerLineCharges", "filterLedgerAccountGroupsCrId", "Web.LedgerAccountGroups");
            DropForeignKey("Web.LedgerLineCharges", "CostCenterId", "Web.CostCenters");
            DropForeignKey("Web.LedgerLineCharges", "ContraLedgerAccountId", "Web.LedgerAccounts");
            DropForeignKey("Web.LedgerLineCharges", "ChargeTypeId", "Web.ChargeTypes");
            DropForeignKey("Web.LedgerLineCharges", "ChargeId", "Web.Charges");
            DropForeignKey("Web.LedgerLineCharges", "CalculateOnId", "Web.Charges");
            DropForeignKey("Web.LedgerHeaderCharges", "ProductChargeId", "Web.Charges");
            DropForeignKey("Web.LedgerHeaderCharges", "PersonID", "Web.People");
            DropForeignKey("Web.LedgerHeaderCharges", "ParentChargeId", "Web.Charges");
            DropForeignKey("Web.LedgerHeaderCharges", "HeaderTableId", "Web.LedgerHeaders");
            DropForeignKey("Web.LedgerHeaderCharges", "LedgerAccountDrId", "Web.LedgerAccounts");
            DropForeignKey("Web.LedgerHeaderCharges", "LedgerAccountCrId", "Web.LedgerAccounts");
            DropForeignKey("Web.LedgerHeaderCharges", "CostCenterId", "Web.CostCenters");
            DropForeignKey("Web.LedgerHeaderCharges", "ContraLedgerAccountId", "Web.LedgerAccounts");
            DropForeignKey("Web.LedgerHeaderCharges", "ChargeTypeId", "Web.ChargeTypes");
            DropForeignKey("Web.LedgerHeaderCharges", "ChargeId", "Web.Charges");
            DropForeignKey("Web.LedgerHeaderCharges", "CalculateOnId", "Web.Charges");
            DropForeignKey("Web.JobInvoiceSettings", "JobInvoiceReturnDocTypeId", "Web.DocumentTypes");
            DropForeignKey("Web.JobInvoiceReturnLineCharges", "HeaderTableId", "Web.JobInvoiceReturnHeaders");
            DropForeignKey("Web.JobOrderLines", "UnitConversionForId", "Web.UnitConversionFors");
            DropForeignKey("Web.Processes", "DepartmentId", "Web.Departments");
            DropIndex("Web.SalaryLineReferences", new[] { "ReferenceDocTypeId" });
            DropIndex("Web.SalaryLineReferences", new[] { "SalaryLineId" });
            DropIndex("Web.SalaryLineCharges", new[] { "HeaderTableId" });
            DropIndex("Web.SalaryHeaders", new[] { "LedgerHeaderId" });
            DropIndex("Web.LedgerSupplementaries", new[] { "SupplementaryLedgerId" });
            DropIndex("Web.LedgerSupplementaries", new[] { "LedgerId" });
            DropIndex("Web.LedgerSettings", new[] { "CalculationId" });
            DropIndex("Web.LedgerLineCharges", new[] { "filterLedgerAccountGroupsCrId" });
            DropIndex("Web.LedgerLineCharges", new[] { "filterLedgerAccountGroupsDrId" });
            DropIndex("Web.LedgerLineCharges", new[] { "ParentChargeId" });
            DropIndex("Web.LedgerLineCharges", new[] { "CostCenterId" });
            DropIndex("Web.LedgerLineCharges", new[] { "ContraLedgerAccountId" });
            DropIndex("Web.LedgerLineCharges", new[] { "LedgerAccountCrId" });
            DropIndex("Web.LedgerLineCharges", new[] { "LedgerAccountDrId" });
            DropIndex("Web.LedgerLineCharges", new[] { "PersonID" });
            DropIndex("Web.LedgerLineCharges", new[] { "CalculateOnId" });
            DropIndex("Web.LedgerLineCharges", new[] { "ChargeTypeId" });
            DropIndex("Web.LedgerLineCharges", new[] { "ChargeId" });
            DropIndex("Web.LedgerLineCharges", new[] { "LineTableId" });
            DropIndex("Web.LedgerHeaderCharges", new[] { "ParentChargeId" });
            DropIndex("Web.LedgerHeaderCharges", new[] { "CostCenterId" });
            DropIndex("Web.LedgerHeaderCharges", new[] { "ContraLedgerAccountId" });
            DropIndex("Web.LedgerHeaderCharges", new[] { "LedgerAccountCrId" });
            DropIndex("Web.LedgerHeaderCharges", new[] { "LedgerAccountDrId" });
            DropIndex("Web.LedgerHeaderCharges", new[] { "PersonID" });
            DropIndex("Web.LedgerHeaderCharges", new[] { "CalculateOnId" });
            DropIndex("Web.LedgerHeaderCharges", new[] { "ProductChargeId" });
            DropIndex("Web.LedgerHeaderCharges", new[] { "ChargeTypeId" });
            DropIndex("Web.LedgerHeaderCharges", new[] { "ChargeId" });
            DropIndex("Web.LedgerHeaderCharges", new[] { "HeaderTableId" });
            DropIndex("Web.JobInvoiceSettings", new[] { "JobInvoiceReturnDocTypeId" });
            DropIndex("Web.JobInvoiceReturnLineCharges", new[] { "HeaderTableId" });
            DropIndex("Web.JobOrderLines", new[] { "UnitConversionForId" });
            DropIndex("Web.Processes", new[] { "DepartmentId" });
            DropPrimaryKey("Web.ChargeGroupSettings");
            DropPrimaryKey("Web.Cities");
            AlterColumn("Web.ChargeGroupSettings", "ChargeGroupSettingsId", c => c.Int(nullable: false));
            AlterColumn("Web.Cities", "CityId", c => c.Int(nullable: false));
            DropColumn("Web.SalarySettings", "isPrintinLetterhead");
            DropColumn("Web.SalarySettings", "isAllowedDuplicatePrint");
            DropColumn("Web.SalaryLines", "NetPayable");
            DropColumn("Web.SalaryLines", "Advance");
            DropColumn("Web.SalaryLines", "LoanEMI");
            DropColumn("Web.SalaryLines", "BasicSalary");
            DropColumn("Web.SalaryHeaders", "IsDocumentPrinted");
            DropColumn("Web.SalaryHeaders", "LedgerHeaderId");
            DropColumn("Web.SalaryHeaders", "WagesPayType");
            DropColumn("Web.ProductBuyerLogs", "WaterTaxPercentage");
            DropColumn("Web.ProductBuyerLogs", "WaterTaxAmount");
            DropColumn("Web.PackingSettings", "isVisibleSaleDeliveryOrder");
            DropColumn("Web.PackingSettings", "isVisibleProductInvoiceGroup");
            DropColumn("Web.PackingSettings", "isVisibleNetWeight");
            DropColumn("Web.PackingSettings", "isVisibleGrossWeight");
            DropColumn("Web.PackingSettings", "isVisibleBaleNoPattern");
            DropColumn("Web.PackingSettings", "isVisibleHeaderJobWorker");
            DropColumn("Web.PackingSettings", "isAllowtoUpdateBuyerSpecification");
            DropColumn("Web.StockHeaderSettings", "isMandatoryLotNoOrDimension1");
            DropColumn("Web.StockHeaderSettings", "isMandatoryLotNo");
            DropColumn("Web.LedgerSettings", "isPrintinLetterhead");
            DropColumn("Web.LedgerSettings", "isAllowedDuplicatePrint");
            DropColumn("Web.LedgerSettings", "isVisibleSpecification");
            DropColumn("Web.LedgerSettings", "isVisibleRate");
            DropColumn("Web.LedgerSettings", "isVisibleDealQty");
            DropColumn("Web.LedgerSettings", "isVisibleQty");
            DropColumn("Web.LedgerSettings", "CalculationId");
            DropColumn("Web.LedgerLines", "SupplementaryLedgerId");
            DropColumn("Web.LedgerLines", "Specification");
            DropColumn("Web.LedgerLines", "Rate");
            DropColumn("Web.LedgerLines", "DealQty");
            DropColumn("Web.LedgerLines", "Qty");
            DropColumn("Web.JobReceiveSettings", "isMandatoryLotNoOrDimension1");
            DropColumn("Web.JobReceiveSettings", "isMandatoryLotNo");
            DropColumn("Web.JobOrderSettings", "filterUnitConversionFors");
            DropColumn("Web.JobOrderSettings", "isVisibleLineUnitConversionFor");
            DropColumn("Web.JobInvoiceSettings", "JobInvoiceReturnDocTypeId");
            DropColumn("Web.JobInvoiceSettings", "isAllowedDuplicatePrint");
            DropColumn("Web.JobInvoiceHeaders", "IsDocumentPrinted");
            DropColumn("Web.ReportHeaders", "IsHideHeaderDetail");
            DropColumn("Web.JobOrderLines", "UnitConversionForId");
            DropColumn("Web.LedgerHeaders", "IsDocumentPrinted");
            DropColumn("Web.Employees", "PaymentType");
            DropColumn("Web.Employees", "WagesPayType");
            DropColumn("Web.Employees", "DateOfRelieving");
            DropColumn("Web.Employees", "DateOfJoining");
            DropColumn("Web.Processes", "DepartmentId");
            DropTable("Web.SalaryLineReferences");
            DropTable("Web.LedgerSupplementaries");
            DropTable("Web.LedgerLineCharges");
            DropTable("Web.LedgerHeaderCharges");
            AddPrimaryKey("Web.ChargeGroupSettings", "ChargeGroupSettingsId");
            AddPrimaryKey("Web.Cities", "CityId");
            AddForeignKey("Web.RouteLines", "CityId", "Web.Cities", "CityId");
            AddForeignKey("Web.DispatchWaybillLines", "CityId", "Web.Cities", "CityId");
            AddForeignKey("Web.DispatchWaybillHeaders", "ToCityId", "Web.Cities", "CityId");
            AddForeignKey("Web.DispatchWaybillHeaders", "FromCityId", "Web.Cities", "CityId");
            AddForeignKey("Web.Companies", "CityId", "Web.Cities", "CityId");
            AddForeignKey("Web.PurchaseWaybills", "ToCityId", "Web.Cities", "CityId");
            AddForeignKey("Web.PurchaseWaybills", "FromCityId", "Web.Cities", "CityId");
            AddForeignKey("Web.Sites", "CityId", "Web.Cities", "CityId");
            AddForeignKey("Web.PersonAddresses", "CityId", "Web.Cities", "CityId");
        }
    }
}
