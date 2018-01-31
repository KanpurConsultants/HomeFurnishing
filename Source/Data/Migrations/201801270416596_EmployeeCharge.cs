namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeCharge : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Web.GatePassHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.JobOrderHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.JobInvoiceAmendmentHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.JobReceiveHeaders", "JobReceiveById", "Web.Employees");
            DropForeignKey("Web.JobReturnHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.JobOrderAmendmentHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.JobOrderCancelHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.JobOrderInspectionHeaders", "InspectionById", "Web.Employees");
            DropForeignKey("Web.JobReceiveQAHeaders", "QAById", "Web.Employees");
            DropForeignKey("Web.PurchaseOrderInspectionHeaders", "InspectionById", "Web.Employees");
            DropForeignKey("Web.SaleDeliveryOrderCancelHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.Gates", "SiteId", "Web.Sites");
            DropForeignKey("Web.PackingHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.GatePassHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.LedgerHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.StockHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.RequisitionHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleOrderHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDeliveryOrderHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.CostCenters", "SiteId", "Web.Sites");
            DropForeignKey("Web.MaterialPlanHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProdOrderHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.GateIns", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseWaybills", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseGoodsReceiptHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.MaterialPlanCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseIndentCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseIndentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.Godowns", "SiteId", "Web.Sites");
            DropForeignKey("Web.AttendanceHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PersonRateGroups", "SiteId", "Web.Sites");
            DropForeignKey("Web.CalculationHeaderLedgerAccounts", "SiteId", "Web.Sites");
            DropForeignKey("Web.CalculationLineLedgerAccounts", "SiteId", "Web.Sites");
            DropForeignKey("Web.CarpetSkuSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ChargeGroupPersonCalculations", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDispatchHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleInvoiceHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.CustomDetails", "SiteId", "Web.Sites");
            DropForeignKey("Web.DispatchWaybillHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocEmailContents", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocNotificationContents", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocSmsContents", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocumentTypeSites", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocumentTypeTimeExtensions", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocumentTypeTimePlans", "SiteId", "Web.Sites");
            DropForeignKey("Web.ExcessMaterialHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.ExcessMaterialSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobConsumptionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobInvoiceAmendmentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobReceiveHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobInvoiceHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobInvoiceReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobInvoiceSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderAmendmentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderInspectionHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderInspectionRequestHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderInspectionRequestCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderInspectionRequestSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderInspectionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobReceiveQAHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobReceiveQASettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobReceiveSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.LeaveTypes", "SiteId", "Web.Sites");
            DropForeignKey("Web.LedgerAdjs", "SiteId", "Web.Sites");
            DropForeignKey("Web.LedgerSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.StockHeaderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.MaterialPlanSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.MaterialReceiveSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.MaterialRequestSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.OverTimeApplicationHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PackingSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PerkDocumentTypes", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProductRateGroups", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProcessSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProdOrderCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProdOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProductBuyerSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProductionOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProductSiteDetails", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseGoodsReceiptSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseGoodsReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseIndentSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseInvoiceAmendmentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseInvoiceHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseInvoiceReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseInvoiceSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderAmendmentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderInspectionHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderInspectionRequestHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderInspectionRequestCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderInspectionRequestSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderInspectionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseQuotationHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseQuotationSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.RateConversionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.RateListHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.RequisitionCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.RequisitionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.RolesMenus", "SiteId", "Web.Sites");
            DropForeignKey("Web.RolesSites", "SiteId", "Web.Sites");
            DropForeignKey("Web.Rug_RetentionPercentage", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDeliveryHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDeliveryOrderCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDeliveryOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDeliverySettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDispatchReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDispatchSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleEnquiryHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleEnquirySettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleInvoiceReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleInvoiceSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleOrderAmendmentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleOrderCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleQuotationHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleQuotationSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SiteDivisionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.StockAdjs", "SiteId", "Web.Sites");
            DropForeignKey("Web.StockUid", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewJobOrderBalance", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewJobOrderBalanceForInspection", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewJobOrderBalanceForInspectionRequest", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewJobOrderBalanceFromStatus", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewJobOrderInspectionRequestBalance", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewRequisitionBalance", "SiteId", "Web.Sites");
            DropForeignKey("Web.WeavingRetensions", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProductUids", "CurrenctGodownId", "Web.Godowns");
            DropForeignKey("Web.PackingHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.StockHeaders", "FromGodownId", "Web.Godowns");
            DropForeignKey("Web.GatePassHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.StockHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.LedgerHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.Stocks", "GodownId", "Web.Godowns");
            DropForeignKey("Web.StockProcesses", "GodownId", "Web.Godowns");
            DropForeignKey("Web.StockLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.SaleOrderHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.PackingLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.JobOrderHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.MaterialPlanHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.JobOrderLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.PurchaseGoodsReceiptLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.PurchaseGoodsReceiptHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.MaterialPlanCancelHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.Sites", "DefaultGodownId", "Web.Godowns");
            DropForeignKey("Web.BinLocations", "GodownId", "Web.Godowns");
            DropForeignKey("Web.SaleDispatchLines", "GodownId", "Web.Godowns");
            DropForeignKey("Web.SaleDispatchLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.ExcessMaterialLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.ExcessMaterialHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.JobReceiveHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.JobReceiveLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.JobReturnHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.JobReturnLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.JobOrderCancelHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.JobOrderCancelLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.MaterialPlanSettings", "GodownId", "Web.Godowns");
            DropForeignKey("Web.OverTimeApplicationHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.ProductSiteDetails", "GodownId", "Web.Godowns");
            DropForeignKey("Web.ProductUidSiteDetails", "CurrenctGodownId", "Web.Godowns");
            DropForeignKey("Web.PurchaseGoodsReturnHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.PurchaseGoodsReturnLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.SaleDispatchReturnHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.SaleDispatchReturnLines", "GodownId", "Web.Godowns");
            DropForeignKey("Web.SaleDispatchSettings", "GodownId", "Web.Godowns");
            DropForeignKey("Web.SaleInvoiceSettings", "GodownId", "Web.Godowns");
            DropForeignKey("Web.StockUid", "GodownId", "Web.Godowns");
            DropIndex("Web.Sites", new[] { "CityId" });
            DropPrimaryKey("Web.Employees");
            DropPrimaryKey("Web.Sites");
            DropPrimaryKey("Web.Godowns");
            CreateTable(
                "Web._Menus",
                c => new
                    {
                        MenuId = c.Int(nullable: false, identity: true),
                        MenuName = c.String(),
                        Srl = c.String(),
                        IconName = c.String(),
                        Description = c.String(),
                        URL = c.String(),
                        ModuleId = c.Int(nullable: false),
                        SubModuleId = c.Int(nullable: false),
                        ControllerActionId = c.Int(nullable: false),
                        ControllerName = c.String(),
                        ActionName = c.String(),
                        IsVisible = c.Boolean(),
                        isDivisionBased = c.Boolean(),
                        isSiteBased = c.Boolean(),
                        RouteId = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MenuId);
            
            CreateTable(
                "Web._ReportHeaders",
                c => new
                    {
                        ReportHeaderId = c.Int(nullable: false, identity: true),
                        ReportName = c.String(),
                        Controller = c.String(),
                        Action = c.String(),
                        SqlProc = c.String(),
                        Notes = c.String(),
                        ParentReportHeaderId = c.Int(),
                        ReportSQL = c.String(),
                    })
                .PrimaryKey(t => t.ReportHeaderId);
            
            CreateTable(
                "Web._ReportLines",
                c => new
                    {
                        ReportLineId = c.Int(nullable: false, identity: true),
                        ReportHeaderId = c.Int(nullable: false),
                        DisplayName = c.String(),
                        FieldName = c.String(),
                        DataType = c.String(),
                        Type = c.String(),
                        ListItem = c.String(),
                        DefaultValue = c.String(),
                        ServiceFuncGet = c.String(),
                        ServiceFuncSet = c.String(),
                        SqlProcGetSet = c.String(),
                        SqlProcGet = c.String(),
                        SqlProcSet = c.String(),
                        CacheKey = c.String(),
                        Serial = c.Int(nullable: false),
                        NoOfCharToEnter = c.Int(),
                        SqlParameter = c.String(),
                        IsCollapse = c.Boolean(nullable: false),
                        IsMandatory = c.Boolean(nullable: false),
                        PlaceHolder = c.String(),
                        ToolTip = c.String(),
                    })
                .PrimaryKey(t => t.ReportLineId);
            
            CreateTable(
                "Web.Castes",
                c => new
                    {
                        CasteId = c.Int(nullable: false, identity: true),
                        DocTypeId = c.Int(nullable: false),
                        CasteName = c.String(nullable: false, maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CasteId)
                .ForeignKey("Web.DocumentTypes", t => t.DocTypeId)
                .Index(t => t.DocTypeId)
                .Index(t => t.CasteName, unique: true, name: "IX_Caste_Caste");
            
            CreateTable(
                "Web.CollectionSettings",
                c => new
                    {
                        CollectionSettingsId = c.Int(nullable: false, identity: true),
                        DocTypeId = c.Int(nullable: false),
                        IsVisibleIntrestBalance = c.Boolean(),
                        IsVisibleArearBalance = c.Boolean(),
                        IsVisibleExcessBalance = c.Boolean(),
                        IsVisibleCurrentYearBalance = c.Boolean(),
                        IsVisibleNetOutstanding = c.Boolean(),
                        IsVisibleReason = c.Boolean(),
                        SqlProcDocumentPrint = c.String(maxLength: 100),
                        SqlProcDocumentPrint_AfterSubmit = c.String(maxLength: 100),
                        SqlProcDocumentPrint_AfterApprove = c.String(maxLength: 100),
                        DocumentPrint = c.String(maxLength: 100),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CollectionSettingsId)
                .ForeignKey("Web.DocumentTypes", t => t.DocTypeId)
                .Index(t => t.DocTypeId);
            
            CreateTable(
                "Web.Dimension1Extended",
                c => new
                    {
                        Dimension1Id = c.Int(nullable: false),
                        Multiplier = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CostCenterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Dimension1Id)
                .ForeignKey("Web.CostCenters", t => t.CostCenterId)
                .ForeignKey("Web.Dimension1", t => t.Dimension1Id)
                .Index(t => t.Dimension1Id)
                .Index(t => t.CostCenterId);
            
            CreateTable(
                "Web.DiscountTypes",
                c => new
                    {
                        DiscountTypeId = c.Int(nullable: false, identity: true),
                        DocTypeId = c.Int(nullable: false),
                        DiscountTypeName = c.String(nullable: false, maxLength: 50),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DiscountTypeId)
                .ForeignKey("Web.DocumentTypes", t => t.DocTypeId)
                .Index(t => t.DocTypeId)
                .Index(t => t.DiscountTypeName, unique: true, name: "IX_DiscountType_DiscountType");
            
            CreateTable(
                "Web.DocumentTypeAttributes",
                c => new
                    {
                        DocumentTypeAttributeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsMandatory = c.Boolean(nullable: false),
                        DataType = c.String(),
                        ListItem = c.String(),
                        DefaultValue = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        DocumentTypeId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        OMSId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.DocumentTypeAttributeId)
                .ForeignKey("Web.DocumentTypes", t => t.DocumentTypeId)
                .Index(t => t.DocumentTypeId);
            
            CreateTable(
                "Web.EmployeeCharges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
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
                .ForeignKey("Web.Employees", t => t.EmployeeId)
                .ForeignKey("Web.LedgerAccounts", t => t.LedgerAccountCrId)
                .ForeignKey("Web.LedgerAccounts", t => t.LedgerAccountDrId)
                .ForeignKey("Web.Charges", t => t.ParentChargeId)
                .ForeignKey("Web.People", t => t.PersonID)
                .ForeignKey("Web.Charges", t => t.ProductChargeId)
                .Index(t => t.EmployeeId)
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
                "Web.PaymentModes",
                c => new
                    {
                        PaymentModeId = c.Int(nullable: false, identity: true),
                        DocTypeId = c.Int(nullable: false),
                        PaymentModeName = c.String(nullable: false, maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        OMSId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.PaymentModeId)
                .ForeignKey("Web.DocumentTypes", t => t.DocTypeId)
                .Index(t => t.DocTypeId)
                .Index(t => t.PaymentModeName, unique: true, name: "IX_PaymentMode_PaymentModeName");
            
            CreateTable(
                "Web.PaymentModeLedgerAccounts",
                c => new
                    {
                        PaymentModeLedgerAccountId = c.Int(nullable: false, identity: true),
                        PaymentModeId = c.Int(nullable: false),
                        LedgerAccountId = c.Int(nullable: false),
                        SiteId = c.Int(nullable: false),
                        DivisionId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        OMSId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.PaymentModeLedgerAccountId)
                .ForeignKey("Web.Divisions", t => t.DivisionId)
                .ForeignKey("Web.LedgerAccounts", t => t.LedgerAccountId)
                .ForeignKey("Web.PaymentModes", t => t.PaymentModeId)
                .ForeignKey("Web.Sites", t => t.SiteId)
                .Index(t => t.PaymentModeId)
                .Index(t => t.LedgerAccountId)
                .Index(t => t.SiteId)
                .Index(t => t.DivisionId);
            
            CreateTable(
                "Web.PersonAttributes",
                c => new
                    {
                        PersonAttributeId = c.Int(nullable: false, identity: true),
                        PersonId = c.Int(nullable: false),
                        DocumentTypeAttributeId = c.Int(nullable: false),
                        PersonAttributeValue = c.String(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        OMSId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.PersonAttributeId)
                .ForeignKey("Web.DocumentTypeAttributes", t => t.DocumentTypeAttributeId)
                .ForeignKey("Web.People", t => t.PersonId)
                .Index(t => t.PersonId)
                .Index(t => t.DocumentTypeAttributeId);
            
            CreateTable(
                "Web.PersonExtendeds",
                c => new
                    {
                        PersonId = c.Int(nullable: false),
                        GISId = c.String(),
                        GodownId = c.Int(nullable: false),
                        BinLocationId = c.Int(),
                        HouseNo = c.String(),
                        OldHouseNo = c.String(),
                        AreaId = c.Int(),
                        FatherName = c.String(),
                        AadharNo = c.String(maxLength: 50),
                        CasteId = c.Int(),
                        ReligionId = c.Int(),
                        PersonRateGroupId = c.Int(),
                        TotalPropertyArea = c.Decimal(precision: 18, scale: 4),
                        TotalTaxableArea = c.Decimal(precision: 18, scale: 4),
                        TotalARV = c.Decimal(precision: 18, scale: 4),
                        TotalTax = c.Decimal(precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.PersonId)
                .ForeignKey("Web.Areas", t => t.AreaId)
                .ForeignKey("Web.BinLocations", t => t.BinLocationId)
                .ForeignKey("Web.Castes", t => t.CasteId)
                .ForeignKey("Web.Godowns", t => t.GodownId)
                .ForeignKey("Web.People", t => t.PersonId)
                .ForeignKey("Web.PersonRateGroups", t => t.PersonRateGroupId)
                .ForeignKey("Web.Religions", t => t.ReligionId)
                .Index(t => t.PersonId)
                .Index(t => t.GodownId)
                .Index(t => t.BinLocationId)
                .Index(t => t.AreaId)
                .Index(t => t.CasteId)
                .Index(t => t.ReligionId)
                .Index(t => t.PersonRateGroupId);
            
            CreateTable(
                "Web.Religions",
                c => new
                    {
                        ReligionId = c.Int(nullable: false, identity: true),
                        DocTypeId = c.Int(nullable: false),
                        ReligionName = c.String(nullable: false, maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReligionId)
                .ForeignKey("Web.DocumentTypes", t => t.DocTypeId)
                .Index(t => t.DocTypeId)
                .Index(t => t.ReligionName, unique: true, name: "IX_Religion_Religion");
            
            CreateTable(
                "Web.ProductBuyerExtendeds",
                c => new
                    {
                        ProductBuyerId = c.Int(nullable: false),
                        DateOfConsutruction = c.DateTime(nullable: false),
                        DiscountTypeId = c.Int(),
                        PropertyArea = c.Decimal(precision: 18, scale: 4),
                        TaxableArea = c.Decimal(precision: 18, scale: 4),
                        ARV = c.Decimal(precision: 18, scale: 4),
                        TenantName = c.String(),
                        BillingType = c.String(),
                        Description = c.String(),
                        CoveredArea = c.Decimal(precision: 18, scale: 4),
                        GarageArea = c.Decimal(precision: 18, scale: 4),
                        BalconyArea = c.Decimal(precision: 18, scale: 4),
                        IsRented = c.Boolean(),
                        WEF = c.DateTime(nullable: false),
                        TaxAmount = c.Decimal(precision: 18, scale: 4),
                        TaxPercentage = c.Decimal(precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.ProductBuyerId)
                .ForeignKey("Web.DiscountTypes", t => t.DiscountTypeId)
                .ForeignKey("Web.ProductBuyers", t => t.ProductBuyerId)
                .Index(t => t.ProductBuyerId)
                .Index(t => t.DiscountTypeId);
            
            CreateTable(
                "Web.ProductBuyerLogs",
                c => new
                    {
                        ProductBuyerLogId = c.Int(nullable: false, identity: true),
                        ProductBuyerId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        BuyerId = c.Int(nullable: false),
                        BuyerSku = c.String(maxLength: 50),
                        BuyerProductCode = c.String(maxLength: 50),
                        BuyerUpcCode = c.String(maxLength: 20),
                        BuyerSpecification = c.String(maxLength: 50),
                        BuyerSpecification1 = c.String(maxLength: 50),
                        BuyerSpecification2 = c.String(maxLength: 50),
                        BuyerSpecification3 = c.String(maxLength: 50),
                        BuyerSpecification4 = c.String(maxLength: 50),
                        BuyerSpecification5 = c.String(maxLength: 50),
                        BuyerSpecification6 = c.String(maxLength: 50),
                        DateOfConsutruction = c.DateTime(nullable: false),
                        DiscountTypeId = c.Int(),
                        PropertyArea = c.Decimal(precision: 18, scale: 4),
                        TaxableArea = c.Decimal(precision: 18, scale: 4),
                        ARV = c.Decimal(precision: 18, scale: 4),
                        TenantName = c.String(),
                        BillingType = c.String(),
                        Description = c.String(),
                        CoveredArea = c.Decimal(precision: 18, scale: 4),
                        GarageArea = c.Decimal(precision: 18, scale: 4),
                        BalconyArea = c.Decimal(precision: 18, scale: 4),
                        IsRented = c.Boolean(),
                        WEF = c.DateTime(nullable: false),
                        TaxAmount = c.Decimal(precision: 18, scale: 4),
                        TaxPercentage = c.Decimal(precision: 18, scale: 4),
                        ModifyRemark = c.String(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        OMSId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ProductBuyerLogId)
                .ForeignKey("Web.People", t => t.BuyerId)
                .ForeignKey("Web.DiscountTypes", t => t.DiscountTypeId)
                .ForeignKey("Web.Products", t => t.ProductId)
                .ForeignKey("Web.ProductBuyers", t => t.ProductBuyerId)
                .Index(t => t.ProductBuyerId)
                .Index(t => t.ProductId)
                .Index(t => t.BuyerId)
                .Index(t => t.DiscountTypeId);
            
            AddColumn("Web.Menus", "ProductNatureId", c => c.Int());
            AddColumn("Web.People", "ReviewBy", c => c.String());
            AddColumn("Web.People", "ReviewCount", c => c.Int());
            AddColumn("Web.People", "Status", c => c.Int());
            AddColumn("Web.Employees", "EmployeeId", c => c.Int(nullable: false, identity: true));
            AddColumn("Web.Employees", "BasicSalary", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("Web.Godowns", "GodownCode", c => c.String(nullable: false, maxLength: 50));
            AddColumn("Web.Godowns", "PersonId", c => c.Int());
            AddColumn("Web.ProductBuyers", "Dimension1Id", c => c.Int());
            AddColumn("Web.ProductBuyers", "Dimension2Id", c => c.Int());
            AddColumn("Web.JobInvoiceSettings", "isVisibleProductUid_Index", c => c.Boolean());
            AddColumn("Web.JobInvoiceSettings", "isVisibleProduct_Index", c => c.Boolean());
            AddColumn("Web.JobInvoiceSettings", "isVisibleProductGroup_Index", c => c.Boolean());
            AddColumn("Web.JobInvoiceSettings", "filterDocTypeCostCenter", c => c.String());
            AddColumn("Web.LedgerLines", "PaymentModeId", c => c.Int());
            AddColumn("Web.LedgerLines", "ReferenceLedgerAccountId", c => c.Int());
            AddColumn("Web.LedgerLines", "AgentId", c => c.Int());
            AddColumn("Web.LedgerLines", "DiscountAmount", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("Web.PersonSettings", "CalculationId", c => c.Int());
            AddColumn("Web.RolesDocTypes", "ProductTypeId", c => c.Int());
            AddColumn("Web.SaleDeliverySettings", "isVisibleProductUid_Index", c => c.Boolean());
            AddColumn("Web.SaleDeliverySettings", "isVisibleProduct_Index", c => c.Boolean());
            AddColumn("Web.SaleDeliverySettings", "isVisibleProductGroup_Index", c => c.Boolean());
            AddColumn("Web.SaleDeliverySettings", "isVisibleSaleInvoice_Index", c => c.Boolean());
            AddColumn("Web.SaleInvoiceReturnHeaders", "SalesTaxGroupPersonId", c => c.Int());
            AddColumn("Web.SaleInvoiceReturnLines", "SalesTaxGroupProductId", c => c.Int());
            AddColumn("Web.SaleInvoiceSettings", "isVisibleGodown", c => c.Boolean());
            AddColumn("Web.SaleInvoiceSettings", "isVisibleProductUid_Index", c => c.Boolean());
            AddColumn("Web.SaleInvoiceSettings", "isVisibleProduct_Index", c => c.Boolean());
            AddColumn("Web.SaleInvoiceSettings", "isVisibleProductGroup_Index", c => c.Boolean());
            AddColumn("Web.SaleInvoiceSettings", "IsAutoDocNo", c => c.Boolean(nullable: false));
            AlterColumn("Web.Sites", "SiteId", c => c.Int(nullable: false, identity: true));
            AlterColumn("Web.Sites", "CityId", c => c.Int());
            AlterColumn("Web.Godowns", "GodownId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("Web.Employees", "EmployeeId");
            AddPrimaryKey("Web.Sites", "SiteId");
            AddPrimaryKey("Web.Godowns", "GodownId");
            CreateIndex("Web.Menus", "ProductNatureId");
            CreateIndex("Web.Godowns", "GodownCode", unique: true, name: "IX_Godown_GodownCode");
            CreateIndex("Web.Godowns", "PersonId");
            CreateIndex("Web.Sites", "CityId");
            CreateIndex("Web.ProductBuyers", "Dimension1Id");
            CreateIndex("Web.ProductBuyers", "Dimension2Id");
            CreateIndex("Web.LedgerLines", "PaymentModeId");
            CreateIndex("Web.LedgerLines", "ReferenceLedgerAccountId");
            CreateIndex("Web.LedgerLines", "AgentId");
            CreateIndex("Web.PersonSettings", "CalculationId");
            CreateIndex("Web.RolesDocTypes", "ProductTypeId");
            CreateIndex("Web.SaleInvoiceReturnHeaders", "SalesTaxGroupPersonId");
            CreateIndex("Web.SaleInvoiceReturnLines", "SalesTaxGroupProductId");
            AddForeignKey("Web.Godowns", "PersonId", "Web.People", "PersonID");
            AddForeignKey("Web.ProductBuyers", "Dimension1Id", "Web.Dimension1", "Dimension1Id");
            AddForeignKey("Web.ProductBuyers", "Dimension2Id", "Web.Dimension2", "Dimension2Id");
            AddForeignKey("Web.Menus", "ProductNatureId", "Web.ProductNatures", "ProductNatureId");
            AddForeignKey("Web.LedgerLines", "AgentId", "Web.People", "PersonID");
            AddForeignKey("Web.LedgerLines", "PaymentModeId", "Web.PaymentModes", "PaymentModeId");
            AddForeignKey("Web.LedgerLines", "ReferenceLedgerAccountId", "Web.LedgerAccounts", "LedgerAccountId");
            AddForeignKey("Web.PersonSettings", "CalculationId", "Web.Calculations", "CalculationId");
            AddForeignKey("Web.RolesDocTypes", "ProductTypeId", "Web.ProductTypes", "ProductTypeId");
            AddForeignKey("Web.SaleInvoiceReturnHeaders", "SalesTaxGroupPersonId", "Web.ChargeGroupPersons", "ChargeGroupPersonId");
            AddForeignKey("Web.SaleInvoiceReturnLines", "SalesTaxGroupProductId", "Web.ChargeGroupProducts", "ChargeGroupProductId");
            AddForeignKey("Web.GatePassHeaders", "OrderById", "Web.Employees", "EmployeeId");
            AddForeignKey("Web.JobOrderHeaders", "OrderById", "Web.Employees", "EmployeeId");
            AddForeignKey("Web.JobInvoiceAmendmentHeaders", "OrderById", "Web.Employees", "EmployeeId");
            AddForeignKey("Web.JobReceiveHeaders", "JobReceiveById", "Web.Employees", "EmployeeId");
            AddForeignKey("Web.JobReturnHeaders", "OrderById", "Web.Employees", "EmployeeId");
            AddForeignKey("Web.JobOrderAmendmentHeaders", "OrderById", "Web.Employees", "EmployeeId");
            AddForeignKey("Web.JobOrderCancelHeaders", "OrderById", "Web.Employees", "EmployeeId");
            AddForeignKey("Web.JobOrderInspectionHeaders", "InspectionById", "Web.Employees", "EmployeeId");
            AddForeignKey("Web.JobReceiveQAHeaders", "QAById", "Web.Employees", "EmployeeId");
            AddForeignKey("Web.PurchaseOrderInspectionHeaders", "InspectionById", "Web.Employees", "EmployeeId");
            AddForeignKey("Web.SaleDeliveryOrderCancelHeaders", "OrderById", "Web.Employees", "EmployeeId");
            AddForeignKey("Web.Godowns", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.Gates", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PackingHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.GatePassHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.LedgerHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.StockHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RequisitionHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleOrderHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDeliveryOrderHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.CostCenters", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.MaterialPlanHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProdOrderHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.GateIns", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseWaybills", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseGoodsReceiptHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.MaterialPlanCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseIndentCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseIndentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.AttendanceHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PersonRateGroups", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.CalculationHeaderLedgerAccounts", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.CalculationLineLedgerAccounts", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.CarpetSkuSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ChargeGroupPersonCalculations", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDispatchHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleInvoiceHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.CustomDetails", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DispatchWaybillHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocEmailContents", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocNotificationContents", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocSmsContents", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocumentTypeSites", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocumentTypeTimeExtensions", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocumentTypeTimePlans", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ExcessMaterialHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ExcessMaterialSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobConsumptionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobInvoiceAmendmentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobReceiveHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobInvoiceHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobInvoiceReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobInvoiceSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderAmendmentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderInspectionHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderInspectionRequestHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderInspectionRequestCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderInspectionRequestSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderInspectionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobReceiveQAHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobReceiveQASettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobReceiveSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.LeaveTypes", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.LedgerAdjs", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.LedgerSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.StockHeaderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.MaterialPlanSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.MaterialReceiveSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.MaterialRequestSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.OverTimeApplicationHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PackingSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PerkDocumentTypes", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProductRateGroups", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProcessSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProdOrderCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProdOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProductBuyerSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProductionOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProductSiteDetails", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseGoodsReceiptSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseGoodsReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseIndentSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseInvoiceAmendmentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseInvoiceHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseInvoiceReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseInvoiceSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderAmendmentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderInspectionHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderInspectionRequestHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderInspectionRequestCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderInspectionRequestSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderInspectionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseQuotationHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseQuotationSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RateConversionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RateListHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RequisitionCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RequisitionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RolesMenus", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RolesSites", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.Rug_RetentionPercentage", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDeliveryHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDeliveryOrderCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDeliveryOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDeliverySettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDispatchReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDispatchSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleEnquiryHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleEnquirySettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleInvoiceReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleInvoiceSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleOrderAmendmentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleOrderCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleQuotationHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleQuotationSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SiteDivisionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.StockAdjs", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.StockUid", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewJobOrderBalance", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewJobOrderBalanceForInspection", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewJobOrderBalanceForInspectionRequest", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewJobOrderBalanceFromStatus", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewJobOrderInspectionRequestBalance", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewRequisitionBalance", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.WeavingRetensions", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.Sites", "DefaultGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.ProductUids", "CurrenctGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PackingHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.StockHeaders", "FromGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.GatePassHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.StockHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.LedgerHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.Stocks", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.StockProcesses", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.StockLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleOrderHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PackingLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobOrderHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.MaterialPlanHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobOrderLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PurchaseGoodsReceiptLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PurchaseGoodsReceiptHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.MaterialPlanCancelHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.BinLocations", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleDispatchLines", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleDispatchLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.ExcessMaterialLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.ExcessMaterialHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobReceiveHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobReceiveLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobReturnHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobReturnLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobOrderCancelHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobOrderCancelLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.MaterialPlanSettings", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.OverTimeApplicationHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.ProductSiteDetails", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.ProductUidSiteDetails", "CurrenctGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PurchaseGoodsReturnHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PurchaseGoodsReturnLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleDispatchReturnHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleDispatchReturnLines", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleDispatchSettings", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleInvoiceSettings", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.StockUid", "GodownId", "Web.Godowns", "GodownId");
            DropColumn("Web.Cities", "Test");
        }
        
        public override void Down()
        {
            AddColumn("Web.Cities", "Test", c => c.String(maxLength: 50));
            DropForeignKey("Web.StockUid", "GodownId", "Web.Godowns");
            DropForeignKey("Web.SaleInvoiceSettings", "GodownId", "Web.Godowns");
            DropForeignKey("Web.SaleDispatchSettings", "GodownId", "Web.Godowns");
            DropForeignKey("Web.SaleDispatchReturnLines", "GodownId", "Web.Godowns");
            DropForeignKey("Web.SaleDispatchReturnHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.PurchaseGoodsReturnLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.PurchaseGoodsReturnHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.ProductUidSiteDetails", "CurrenctGodownId", "Web.Godowns");
            DropForeignKey("Web.ProductSiteDetails", "GodownId", "Web.Godowns");
            DropForeignKey("Web.OverTimeApplicationHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.MaterialPlanSettings", "GodownId", "Web.Godowns");
            DropForeignKey("Web.JobOrderCancelLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.JobOrderCancelHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.JobReturnLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.JobReturnHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.JobReceiveLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.JobReceiveHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.ExcessMaterialHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.ExcessMaterialLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.SaleDispatchLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.SaleDispatchLines", "GodownId", "Web.Godowns");
            DropForeignKey("Web.BinLocations", "GodownId", "Web.Godowns");
            DropForeignKey("Web.MaterialPlanCancelHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.PurchaseGoodsReceiptHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.PurchaseGoodsReceiptLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.JobOrderLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.MaterialPlanHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.JobOrderHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.PackingLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.SaleOrderHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.StockLines", "ProductUidCurrentGodownId", "Web.Godowns");
            DropForeignKey("Web.StockProcesses", "GodownId", "Web.Godowns");
            DropForeignKey("Web.Stocks", "GodownId", "Web.Godowns");
            DropForeignKey("Web.LedgerHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.StockHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.GatePassHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.StockHeaders", "FromGodownId", "Web.Godowns");
            DropForeignKey("Web.PackingHeaders", "GodownId", "Web.Godowns");
            DropForeignKey("Web.ProductUids", "CurrenctGodownId", "Web.Godowns");
            DropForeignKey("Web.Sites", "DefaultGodownId", "Web.Godowns");
            DropForeignKey("Web.WeavingRetensions", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewRequisitionBalance", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewJobOrderInspectionRequestBalance", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewJobOrderBalanceFromStatus", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewJobOrderBalanceForInspectionRequest", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewJobOrderBalanceForInspection", "SiteId", "Web.Sites");
            DropForeignKey("Web.ViewJobOrderBalance", "SiteId", "Web.Sites");
            DropForeignKey("Web.StockUid", "SiteId", "Web.Sites");
            DropForeignKey("Web.StockAdjs", "SiteId", "Web.Sites");
            DropForeignKey("Web.SiteDivisionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleQuotationSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleQuotationHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleOrderCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleOrderAmendmentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleInvoiceSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleInvoiceReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleEnquirySettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleEnquiryHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDispatchSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDispatchReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDeliverySettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDeliveryOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDeliveryOrderCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDeliveryHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.Rug_RetentionPercentage", "SiteId", "Web.Sites");
            DropForeignKey("Web.RolesSites", "SiteId", "Web.Sites");
            DropForeignKey("Web.RolesMenus", "SiteId", "Web.Sites");
            DropForeignKey("Web.RequisitionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.RequisitionCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.RateListHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.RateConversionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseQuotationSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseQuotationHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderInspectionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderInspectionRequestSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderInspectionRequestCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderInspectionRequestHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderInspectionHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderAmendmentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseInvoiceSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseInvoiceReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseInvoiceHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseInvoiceAmendmentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseIndentSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseGoodsReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseGoodsReceiptSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProductSiteDetails", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProductionOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProductBuyerSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProdOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProdOrderCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProcessSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProductRateGroups", "SiteId", "Web.Sites");
            DropForeignKey("Web.PerkDocumentTypes", "SiteId", "Web.Sites");
            DropForeignKey("Web.PackingSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.OverTimeApplicationHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.MaterialRequestSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.MaterialReceiveSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.MaterialPlanSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.StockHeaderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.LedgerSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.LedgerAdjs", "SiteId", "Web.Sites");
            DropForeignKey("Web.LeaveTypes", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobReceiveSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobReceiveQASettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobReceiveQAHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderInspectionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderInspectionRequestSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderInspectionRequestCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderInspectionRequestHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderInspectionHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderAmendmentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobInvoiceSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobInvoiceReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobReturnHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobInvoiceHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobReceiveHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobInvoiceAmendmentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobConsumptionSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ExcessMaterialSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.ExcessMaterialHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocumentTypeTimePlans", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocumentTypeTimeExtensions", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocumentTypeSites", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocSmsContents", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocNotificationContents", "SiteId", "Web.Sites");
            DropForeignKey("Web.DocEmailContents", "SiteId", "Web.Sites");
            DropForeignKey("Web.DispatchWaybillHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.CustomDetails", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleInvoiceHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDispatchHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.ChargeGroupPersonCalculations", "SiteId", "Web.Sites");
            DropForeignKey("Web.CarpetSkuSettings", "SiteId", "Web.Sites");
            DropForeignKey("Web.CalculationLineLedgerAccounts", "SiteId", "Web.Sites");
            DropForeignKey("Web.CalculationHeaderLedgerAccounts", "SiteId", "Web.Sites");
            DropForeignKey("Web.PersonRateGroups", "SiteId", "Web.Sites");
            DropForeignKey("Web.AttendanceHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseOrderCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseIndentHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseIndentCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.MaterialPlanCancelHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseGoodsReceiptHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PurchaseWaybills", "SiteId", "Web.Sites");
            DropForeignKey("Web.GateIns", "SiteId", "Web.Sites");
            DropForeignKey("Web.JobOrderHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.ProdOrderHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.MaterialPlanHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.CostCenters", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDeliveryOrderHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleOrderHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.RequisitionHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.StockHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.LedgerHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.GatePassHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.PackingHeaders", "SiteId", "Web.Sites");
            DropForeignKey("Web.Gates", "SiteId", "Web.Sites");
            DropForeignKey("Web.Godowns", "SiteId", "Web.Sites");
            DropForeignKey("Web.SaleDeliveryOrderCancelHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.PurchaseOrderInspectionHeaders", "InspectionById", "Web.Employees");
            DropForeignKey("Web.JobReceiveQAHeaders", "QAById", "Web.Employees");
            DropForeignKey("Web.JobOrderInspectionHeaders", "InspectionById", "Web.Employees");
            DropForeignKey("Web.JobOrderCancelHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.JobOrderAmendmentHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.JobReturnHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.JobReceiveHeaders", "JobReceiveById", "Web.Employees");
            DropForeignKey("Web.JobInvoiceAmendmentHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.JobOrderHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.GatePassHeaders", "OrderById", "Web.Employees");
            DropForeignKey("Web.SaleInvoiceReturnLines", "SalesTaxGroupProductId", "Web.ChargeGroupProducts");
            DropForeignKey("Web.SaleInvoiceReturnHeaders", "SalesTaxGroupPersonId", "Web.ChargeGroupPersons");
            DropForeignKey("Web.RolesDocTypes", "ProductTypeId", "Web.ProductTypes");
            DropForeignKey("Web.ProductBuyerLogs", "ProductBuyerId", "Web.ProductBuyers");
            DropForeignKey("Web.ProductBuyerLogs", "ProductId", "Web.Products");
            DropForeignKey("Web.ProductBuyerLogs", "DiscountTypeId", "Web.DiscountTypes");
            DropForeignKey("Web.ProductBuyerLogs", "BuyerId", "Web.People");
            DropForeignKey("Web.ProductBuyerExtendeds", "ProductBuyerId", "Web.ProductBuyers");
            DropForeignKey("Web.ProductBuyerExtendeds", "DiscountTypeId", "Web.DiscountTypes");
            DropForeignKey("Web.PersonSettings", "CalculationId", "Web.Calculations");
            DropForeignKey("Web.PersonExtendeds", "ReligionId", "Web.Religions");
            DropForeignKey("Web.Religions", "DocTypeId", "Web.DocumentTypes");
            DropForeignKey("Web.PersonExtendeds", "PersonRateGroupId", "Web.PersonRateGroups");
            DropForeignKey("Web.PersonExtendeds", "PersonId", "Web.People");
            DropForeignKey("Web.PersonExtendeds", "GodownId", "Web.Godowns");
            DropForeignKey("Web.PersonExtendeds", "CasteId", "Web.Castes");
            DropForeignKey("Web.PersonExtendeds", "BinLocationId", "Web.BinLocations");
            DropForeignKey("Web.PersonExtendeds", "AreaId", "Web.Areas");
            DropForeignKey("Web.PersonAttributes", "PersonId", "Web.People");
            DropForeignKey("Web.PersonAttributes", "DocumentTypeAttributeId", "Web.DocumentTypeAttributes");
            DropForeignKey("Web.PaymentModeLedgerAccounts", "SiteId", "Web.Sites");
            DropForeignKey("Web.PaymentModeLedgerAccounts", "PaymentModeId", "Web.PaymentModes");
            DropForeignKey("Web.PaymentModeLedgerAccounts", "LedgerAccountId", "Web.LedgerAccounts");
            DropForeignKey("Web.PaymentModeLedgerAccounts", "DivisionId", "Web.Divisions");
            DropForeignKey("Web.LedgerLines", "ReferenceLedgerAccountId", "Web.LedgerAccounts");
            DropForeignKey("Web.LedgerLines", "PaymentModeId", "Web.PaymentModes");
            DropForeignKey("Web.PaymentModes", "DocTypeId", "Web.DocumentTypes");
            DropForeignKey("Web.LedgerLines", "AgentId", "Web.People");
            DropForeignKey("Web.EmployeeCharges", "ProductChargeId", "Web.Charges");
            DropForeignKey("Web.EmployeeCharges", "PersonID", "Web.People");
            DropForeignKey("Web.EmployeeCharges", "ParentChargeId", "Web.Charges");
            DropForeignKey("Web.EmployeeCharges", "LedgerAccountDrId", "Web.LedgerAccounts");
            DropForeignKey("Web.EmployeeCharges", "LedgerAccountCrId", "Web.LedgerAccounts");
            DropForeignKey("Web.EmployeeCharges", "EmployeeId", "Web.Employees");
            DropForeignKey("Web.EmployeeCharges", "CostCenterId", "Web.CostCenters");
            DropForeignKey("Web.EmployeeCharges", "ContraLedgerAccountId", "Web.LedgerAccounts");
            DropForeignKey("Web.EmployeeCharges", "ChargeTypeId", "Web.ChargeTypes");
            DropForeignKey("Web.EmployeeCharges", "ChargeId", "Web.Charges");
            DropForeignKey("Web.EmployeeCharges", "CalculateOnId", "Web.Charges");
            DropForeignKey("Web.DocumentTypeAttributes", "DocumentTypeId", "Web.DocumentTypes");
            DropForeignKey("Web.DiscountTypes", "DocTypeId", "Web.DocumentTypes");
            DropForeignKey("Web.Dimension1Extended", "Dimension1Id", "Web.Dimension1");
            DropForeignKey("Web.Dimension1Extended", "CostCenterId", "Web.CostCenters");
            DropForeignKey("Web.CollectionSettings", "DocTypeId", "Web.DocumentTypes");
            DropForeignKey("Web.Castes", "DocTypeId", "Web.DocumentTypes");
            DropForeignKey("Web.Menus", "ProductNatureId", "Web.ProductNatures");
            DropForeignKey("Web.ProductBuyers", "Dimension2Id", "Web.Dimension2");
            DropForeignKey("Web.ProductBuyers", "Dimension1Id", "Web.Dimension1");
            DropForeignKey("Web.Godowns", "PersonId", "Web.People");
            DropIndex("Web.SaleInvoiceReturnLines", new[] { "SalesTaxGroupProductId" });
            DropIndex("Web.SaleInvoiceReturnHeaders", new[] { "SalesTaxGroupPersonId" });
            DropIndex("Web.RolesDocTypes", new[] { "ProductTypeId" });
            DropIndex("Web.ProductBuyerLogs", new[] { "DiscountTypeId" });
            DropIndex("Web.ProductBuyerLogs", new[] { "BuyerId" });
            DropIndex("Web.ProductBuyerLogs", new[] { "ProductId" });
            DropIndex("Web.ProductBuyerLogs", new[] { "ProductBuyerId" });
            DropIndex("Web.ProductBuyerExtendeds", new[] { "DiscountTypeId" });
            DropIndex("Web.ProductBuyerExtendeds", new[] { "ProductBuyerId" });
            DropIndex("Web.PersonSettings", new[] { "CalculationId" });
            DropIndex("Web.Religions", "IX_Religion_Religion");
            DropIndex("Web.Religions", new[] { "DocTypeId" });
            DropIndex("Web.PersonExtendeds", new[] { "PersonRateGroupId" });
            DropIndex("Web.PersonExtendeds", new[] { "ReligionId" });
            DropIndex("Web.PersonExtendeds", new[] { "CasteId" });
            DropIndex("Web.PersonExtendeds", new[] { "AreaId" });
            DropIndex("Web.PersonExtendeds", new[] { "BinLocationId" });
            DropIndex("Web.PersonExtendeds", new[] { "GodownId" });
            DropIndex("Web.PersonExtendeds", new[] { "PersonId" });
            DropIndex("Web.PersonAttributes", new[] { "DocumentTypeAttributeId" });
            DropIndex("Web.PersonAttributes", new[] { "PersonId" });
            DropIndex("Web.PaymentModeLedgerAccounts", new[] { "DivisionId" });
            DropIndex("Web.PaymentModeLedgerAccounts", new[] { "SiteId" });
            DropIndex("Web.PaymentModeLedgerAccounts", new[] { "LedgerAccountId" });
            DropIndex("Web.PaymentModeLedgerAccounts", new[] { "PaymentModeId" });
            DropIndex("Web.PaymentModes", "IX_PaymentMode_PaymentModeName");
            DropIndex("Web.PaymentModes", new[] { "DocTypeId" });
            DropIndex("Web.LedgerLines", new[] { "AgentId" });
            DropIndex("Web.LedgerLines", new[] { "ReferenceLedgerAccountId" });
            DropIndex("Web.LedgerLines", new[] { "PaymentModeId" });
            DropIndex("Web.EmployeeCharges", new[] { "ParentChargeId" });
            DropIndex("Web.EmployeeCharges", new[] { "CostCenterId" });
            DropIndex("Web.EmployeeCharges", new[] { "ContraLedgerAccountId" });
            DropIndex("Web.EmployeeCharges", new[] { "LedgerAccountCrId" });
            DropIndex("Web.EmployeeCharges", new[] { "LedgerAccountDrId" });
            DropIndex("Web.EmployeeCharges", new[] { "PersonID" });
            DropIndex("Web.EmployeeCharges", new[] { "CalculateOnId" });
            DropIndex("Web.EmployeeCharges", new[] { "ProductChargeId" });
            DropIndex("Web.EmployeeCharges", new[] { "ChargeTypeId" });
            DropIndex("Web.EmployeeCharges", new[] { "ChargeId" });
            DropIndex("Web.EmployeeCharges", new[] { "EmployeeId" });
            DropIndex("Web.DocumentTypeAttributes", new[] { "DocumentTypeId" });
            DropIndex("Web.DiscountTypes", "IX_DiscountType_DiscountType");
            DropIndex("Web.DiscountTypes", new[] { "DocTypeId" });
            DropIndex("Web.Dimension1Extended", new[] { "CostCenterId" });
            DropIndex("Web.Dimension1Extended", new[] { "Dimension1Id" });
            DropIndex("Web.CollectionSettings", new[] { "DocTypeId" });
            DropIndex("Web.Castes", "IX_Caste_Caste");
            DropIndex("Web.Castes", new[] { "DocTypeId" });
            DropIndex("Web.ProductBuyers", new[] { "Dimension2Id" });
            DropIndex("Web.ProductBuyers", new[] { "Dimension1Id" });
            DropIndex("Web.Sites", new[] { "CityId" });
            DropIndex("Web.Godowns", new[] { "PersonId" });
            DropIndex("Web.Godowns", "IX_Godown_GodownCode");
            DropIndex("Web.Menus", new[] { "ProductNatureId" });
            DropPrimaryKey("Web.Godowns");
            DropPrimaryKey("Web.Sites");
            DropPrimaryKey("Web.Employees");
            AlterColumn("Web.Godowns", "GodownId", c => c.Int(nullable: false));
            AlterColumn("Web.Sites", "CityId", c => c.Int(nullable: false));
            AlterColumn("Web.Sites", "SiteId", c => c.Int(nullable: false));
            DropColumn("Web.SaleInvoiceSettings", "IsAutoDocNo");
            DropColumn("Web.SaleInvoiceSettings", "isVisibleProductGroup_Index");
            DropColumn("Web.SaleInvoiceSettings", "isVisibleProduct_Index");
            DropColumn("Web.SaleInvoiceSettings", "isVisibleProductUid_Index");
            DropColumn("Web.SaleInvoiceSettings", "isVisibleGodown");
            DropColumn("Web.SaleInvoiceReturnLines", "SalesTaxGroupProductId");
            DropColumn("Web.SaleInvoiceReturnHeaders", "SalesTaxGroupPersonId");
            DropColumn("Web.SaleDeliverySettings", "isVisibleSaleInvoice_Index");
            DropColumn("Web.SaleDeliverySettings", "isVisibleProductGroup_Index");
            DropColumn("Web.SaleDeliverySettings", "isVisibleProduct_Index");
            DropColumn("Web.SaleDeliverySettings", "isVisibleProductUid_Index");
            DropColumn("Web.RolesDocTypes", "ProductTypeId");
            DropColumn("Web.PersonSettings", "CalculationId");
            DropColumn("Web.LedgerLines", "DiscountAmount");
            DropColumn("Web.LedgerLines", "AgentId");
            DropColumn("Web.LedgerLines", "ReferenceLedgerAccountId");
            DropColumn("Web.LedgerLines", "PaymentModeId");
            DropColumn("Web.JobInvoiceSettings", "filterDocTypeCostCenter");
            DropColumn("Web.JobInvoiceSettings", "isVisibleProductGroup_Index");
            DropColumn("Web.JobInvoiceSettings", "isVisibleProduct_Index");
            DropColumn("Web.JobInvoiceSettings", "isVisibleProductUid_Index");
            DropColumn("Web.ProductBuyers", "Dimension2Id");
            DropColumn("Web.ProductBuyers", "Dimension1Id");
            DropColumn("Web.Godowns", "PersonId");
            DropColumn("Web.Godowns", "GodownCode");
            DropColumn("Web.Employees", "BasicSalary");
            DropColumn("Web.Employees", "EmployeeId");
            DropColumn("Web.People", "Status");
            DropColumn("Web.People", "ReviewCount");
            DropColumn("Web.People", "ReviewBy");
            DropColumn("Web.Menus", "ProductNatureId");
            DropTable("Web.ProductBuyerLogs");
            DropTable("Web.ProductBuyerExtendeds");
            DropTable("Web.Religions");
            DropTable("Web.PersonExtendeds");
            DropTable("Web.PersonAttributes");
            DropTable("Web.PaymentModeLedgerAccounts");
            DropTable("Web.PaymentModes");
            DropTable("Web.EmployeeCharges");
            DropTable("Web.DocumentTypeAttributes");
            DropTable("Web.DiscountTypes");
            DropTable("Web.Dimension1Extended");
            DropTable("Web.CollectionSettings");
            DropTable("Web.Castes");
            DropTable("Web._ReportLines");
            DropTable("Web._ReportHeaders");
            DropTable("Web._Menus");
            AddPrimaryKey("Web.Godowns", "GodownId");
            AddPrimaryKey("Web.Sites", "SiteId");
            AddPrimaryKey("Web.Employees", "PersonID");
            CreateIndex("Web.Sites", "CityId");
            AddForeignKey("Web.StockUid", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleInvoiceSettings", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleDispatchSettings", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleDispatchReturnLines", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleDispatchReturnHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PurchaseGoodsReturnLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PurchaseGoodsReturnHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.ProductUidSiteDetails", "CurrenctGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.ProductSiteDetails", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.OverTimeApplicationHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.MaterialPlanSettings", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobOrderCancelLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobOrderCancelHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobReturnLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobReturnHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobReceiveLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobReceiveHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.ExcessMaterialHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.ExcessMaterialLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleDispatchLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleDispatchLines", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.BinLocations", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.Sites", "DefaultGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.MaterialPlanCancelHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PurchaseGoodsReceiptHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PurchaseGoodsReceiptLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobOrderLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.MaterialPlanHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.JobOrderHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PackingLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.SaleOrderHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.StockLines", "ProductUidCurrentGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.StockProcesses", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.Stocks", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.LedgerHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.StockHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.GatePassHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.StockHeaders", "FromGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.PackingHeaders", "GodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.ProductUids", "CurrenctGodownId", "Web.Godowns", "GodownId");
            AddForeignKey("Web.WeavingRetensions", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewRequisitionBalance", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewJobOrderInspectionRequestBalance", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewJobOrderBalanceFromStatus", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewJobOrderBalanceForInspectionRequest", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewJobOrderBalanceForInspection", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ViewJobOrderBalance", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.StockUid", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.StockAdjs", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SiteDivisionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleQuotationSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleQuotationHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleOrderCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleOrderAmendmentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleInvoiceSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleInvoiceReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleEnquirySettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleEnquiryHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDispatchSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDispatchReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDeliverySettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDeliveryOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDeliveryOrderCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDeliveryHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.Rug_RetentionPercentage", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RolesSites", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RolesMenus", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RequisitionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RequisitionCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RateListHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RateConversionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseQuotationSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseQuotationHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderInspectionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderInspectionRequestSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderInspectionRequestCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderInspectionRequestHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderInspectionHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderAmendmentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseInvoiceSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseInvoiceReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseInvoiceHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseInvoiceAmendmentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseIndentSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseGoodsReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseGoodsReceiptSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProductSiteDetails", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProductionOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProductBuyerSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProdOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProdOrderCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProcessSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProductRateGroups", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PerkDocumentTypes", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PackingSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.OverTimeApplicationHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.MaterialRequestSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.MaterialReceiveSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.MaterialPlanSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.StockHeaderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.LedgerSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.LedgerAdjs", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.LeaveTypes", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobReceiveSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobReceiveQASettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobReceiveQAHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderInspectionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderInspectionRequestSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderInspectionRequestCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderInspectionRequestHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderInspectionHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderAmendmentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobInvoiceSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobInvoiceReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobReturnHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobInvoiceHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobReceiveHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobInvoiceAmendmentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobConsumptionSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ExcessMaterialSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ExcessMaterialHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocumentTypeTimePlans", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocumentTypeTimeExtensions", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocumentTypeSites", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocSmsContents", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocNotificationContents", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DocEmailContents", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.DispatchWaybillHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.CustomDetails", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleInvoiceHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDispatchHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ChargeGroupPersonCalculations", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.CarpetSkuSettings", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.CalculationLineLedgerAccounts", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.CalculationHeaderLedgerAccounts", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PersonRateGroups", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.AttendanceHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.Godowns", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseOrderCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseIndentHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseIndentCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.MaterialPlanCancelHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseGoodsReceiptHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PurchaseWaybills", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.GateIns", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.JobOrderHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.ProdOrderHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.MaterialPlanHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.CostCenters", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDeliveryOrderHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleOrderHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.RequisitionHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.StockHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.LedgerHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.GatePassHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.PackingHeaders", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.Gates", "SiteId", "Web.Sites", "SiteId");
            AddForeignKey("Web.SaleDeliveryOrderCancelHeaders", "OrderById", "Web.Employees", "PersonID");
            AddForeignKey("Web.PurchaseOrderInspectionHeaders", "InspectionById", "Web.Employees", "PersonID");
            AddForeignKey("Web.JobReceiveQAHeaders", "QAById", "Web.Employees", "PersonID");
            AddForeignKey("Web.JobOrderInspectionHeaders", "InspectionById", "Web.Employees", "PersonID");
            AddForeignKey("Web.JobOrderCancelHeaders", "OrderById", "Web.Employees", "PersonID");
            AddForeignKey("Web.JobOrderAmendmentHeaders", "OrderById", "Web.Employees", "PersonID");
            AddForeignKey("Web.JobReturnHeaders", "OrderById", "Web.Employees", "PersonID");
            AddForeignKey("Web.JobReceiveHeaders", "JobReceiveById", "Web.Employees", "PersonID");
            AddForeignKey("Web.JobInvoiceAmendmentHeaders", "OrderById", "Web.Employees", "PersonID");
            AddForeignKey("Web.JobOrderHeaders", "OrderById", "Web.Employees", "PersonID");
            AddForeignKey("Web.GatePassHeaders", "OrderById", "Web.Employees", "PersonID");
        }
    }
}
