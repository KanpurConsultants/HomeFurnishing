using System.Collections.Generic;
using System.Web.Mvc;
using Service;

//using ProjLib.ViewModels;
using System.Data.SqlClient;
using System.Data;
using System;

namespace Module
{
    [Authorize]
    public class UpdateTableStructureController : Controller
    {
        string mQry = "";
        IModuleService _ModuleService;

        public UpdateTableStructureController(IModuleService mService)
        {
            _ModuleService = mService;
        }

        public ActionResult UpdateTables()
        {


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'BinLocations'") == 0)
                {
                    mQry = @"CREATE TABLE Web.BinLocations 
	                            (
	                            BinLocationId   INT IDENTITY NOT NULL,
	                            BinLocationCode NVARCHAR (10),
	                            BinLocationName NVARCHAR (50) NOT NULL,
	                            IsActive        BIT DEFAULT ((1)) NOT NULL,
	                            CreatedBy       NVARCHAR (max),
	                            ModifiedBy      NVARCHAR (max),
	                            CreatedDate     DATETIME NOT NULL,
	                            ModifiedDate    DATETIME NOT NULL,
	                            OMSId           NVARCHAR (50),
	                            GodownId        INT,
	                            CONSTRAINT [PK_Web.BinLocations] PRIMARY KEY (BinLocationId)
	                            WITH (FILLFACTOR = 90),
	                            CONSTRAINT [FK_Web.BinLocations_Web.Godowns_GodownId] FOREIGN KEY (GodownId) REFERENCES Web.Godowns (GodownId)
	                            )
                            

                            CREATE UNIQUE INDEX IX_BinLocation_BinLocationName
	                            ON Web.BinLocations (BinLocationName)
	                            WITH (FILLFACTOR = 90)
                            ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'PersonProductUids'") == 0)
                {
                    mQry = @"CREATE TABLE Web.PersonProductUids 
	                            (
	                            PersonProductUidId   INT IDENTITY NOT NULL,
	                            GenDocId                 INT,								
								GenDocNo                 NVARCHAR,
								GenDocTypeId             INT,
								GenLineId                INT,
								ProductUidName           NVARCHAR (50) NOT NULL,								
								ProductUidSpecification            NVARCHAR (Max),
								SaleOrderLineId          INT,
	                            CreatedBy       NVARCHAR (max),
	                            ModifiedBy      NVARCHAR (max),
	                            CreatedDate     DATETIME NOT NULL,
	                            ModifiedDate    DATETIME NOT NULL,
	                            OMSId           NVARCHAR (50),
	                            CONSTRAINT [PK_Web.PersonProductUid] PRIMARY KEY (PersonProductUidId)
	                            WITH (FILLFACTOR = 90),
	                            CONSTRAINT [FK_Web.PersonProductUid_Web.GenDocTypeId_DocumentTypeId] FOREIGN KEY (GenDocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                            CONSTRAINT [FK_Web.PersonProductUid_Web.SaleOrderLines_SaleOrderLineId] FOREIGN KEY (SaleOrderLineId) REFERENCES Web.SaleOrderLines (SaleOrderLineId),
	                            )
                            
                            ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }




            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Columns WHERE COLUMN_NAME =  'BinLocation' AND TABLE_NAME = 'ProductSiteDetails'") == 1)
                {
                    mQry = "ALTER TABLE Web.ProductSiteDetails DROP COLUMN BinLocation";
                    ExecuteQuery(mQry);
                    
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            AddFields("LedgerAccountGroups", "ParentLedgerAccountGroupId", "INT", "LedgerAccountGroups");
            AddFields("LedgerAccountGroups", "BSNature", "NVARCHAR (20)");
            AddFields("LedgerAccountGroups", "BSSr", "INT");
            AddFields("LedgerAccountGroups", "TradingNature", "NVARCHAR (20)");
            AddFields("LedgerAccountGroups", "TradingSr", "INT");
            AddFields("LedgerAccountGroups", "PLNature", "NVARCHAR (20)");
            AddFields("LedgerAccountGroups", "PLSr", "INT");


            AddFields("SaleInvoiceHeaderDetail", "FreightRemark", "NVARCHAR (50)");
            AddFields("SaleInvoiceHeaderDetail", "InsuranceRemark", "NVARCHAR (50)");
            AddFields("SaleInvoiceHeaderDetail", "Deduction", "Decimal(18,4)");

            AddFields("TdsRates", "LedgerAccountId", "INT", "LedgerAccounts");
            AddFields("ProductSiteDetails", "BinLocationId", "INT", "BinLocations");

            AddFields("Products", "ProductCategoryId", "INT", "ProductCategories");
            AddFields("Products", "SaleRate", " Decimal(18,4)");

            AddFields("Processes", "DepartmentId", "INT", "Departments");
            AddFields("SalaryLines", "BasicSalary", " Decimal(18,4)");
            AddFields("SalaryHeaders", "WagesPayType", "NVARCHAR (20)");

            AddFields("ProductTypeSettings", "isVisibleProductDescription", "BIT");
            AddFields("ProductTypeSettings", "isVisibleProductSpecification", "BIT");
            AddFields("ProductTypeSettings", "isVisibleProductCategory", "BIT");
            AddFields("ProductTypeSettings", "isVisibleSalesTaxGroup", "BIT");
            AddFields("ProductTypeSettings", "isVisibleSaleRate", "BIT");
            AddFields("ProductTypeSettings", "isVisibleStandardCost", "BIT");
            AddFields("ProductTypeSettings", "isVisibleTags", "BIT");
            AddFields("ProductTypeSettings", "isVisibleMinimumOrderQty", "BIT");
            AddFields("ProductTypeSettings", "isVisibleReOrderLevel", "BIT");
            AddFields("ProductTypeSettings", "isVisibleGodownId", "BIT");
            AddFields("ProductTypeSettings", "isVisibleBinLocationId", "BIT");
            AddFields("ProductTypeSettings", "isVisibleProfitMargin", "BIT");
            AddFields("ProductTypeSettings", "isVisibleCarryingCost", "BIT");
            AddFields("ProductTypeSettings", "isVisibleLotManagement", "BIT");
            AddFields("ProductTypeSettings", "isVisibleConsumptionDetail", "BIT");
            AddFields("ProductTypeSettings", "isVisibleProductProcessDetail", "BIT");

            AddFields("ProductTypeSettings", "ProductNameCaption", "NVARCHAR(Max)");
            AddFields("ProductTypeSettings", "ProductCodeCaption", "NVARCHAR(Max)");
            AddFields("ProductTypeSettings", "ProductDescriptionCaption", "NVARCHAR(Max)");

            AddFields("ProductTypeSettings", "ProductSpecificationCaption", "NVARCHAR(Max)");
            AddFields("ProductTypeSettings", "ProductGroupCaption", "NVARCHAR(Max)");
            AddFields("ProductTypeSettings", "ProductCategoryCaption", "NVARCHAR(Max)");


            AddFields("ProductUids", "ProductUidSpecification", "NVARCHAR(Max)");

            AddFields("SaleEnquiryLineExtendeds", "BuyerSku", "nvarchar(50)");
            AddFields("SaleEnquiryLineExtendeds", "BuyerUpcCode", "nvarchar(20)");
            AddFields("PackingLines", "PersonProductUidId", "Int", "PersonProductUids");
            AddFields("DocumentTypes", "DocumentNatureId", "Int", "DocumentNatures");


            AddFields("JobInvoiceHeaders", "FinancierId", "INT");
            AddFields("JobInvoiceHeaders", "IsDocumentPrinted", "Bit");

            AddFields("JobInvoiceLines", "RateDiscountPer", "Decimal(18,4)");
            AddFields("JobReceiveLines", "MfgDate", "DATETIME");

            AddFields("DocumentTypeSettings", "CostCenterCaption", "NVARCHAR (50)");
            AddFields("DocumentTypeSettings", "SpecificationCaption", "NVARCHAR (50)");

            AddFields("JobInvoiceSettings", "isGenerateProductUid", "BIT");
            AddFields("JobInvoiceSettings", "isVisiblePenalty", "BIT");
            AddFields("JobInvoiceSettings", "isVisibleIncentive", "BIT");
            AddFields("JobInvoiceSettings", "isVisibleMfgDate", "BIT");
            AddFields("JobInvoiceSettings", "isVisibleFinancier", "BIT");
            AddFields("JobInvoiceSettings", "isVisibleRateDiscountPer", "BIT");
            AddFields("JobInvoiceSettings", "isAllowedDuplicatePrint", "BIT NOT NULL DEFAULT(1)");

            AddFields("JobOrderSettings", "isVisibleFinancier", "BIT");
            AddFields("JobOrderSettings", "isVisibleSalesExecutive", "BIT");
            AddFields("JobOrderHeaders", "FinancierId", "Int", "People");
            AddFields("JobOrderHeaders", "SalesExecutiveId", "Int", "People");

            AddFields("Buyers", "ExtraSaleOrderHeaderId", "Int", "SaleOrderHeaders");

            AddFields("SaleOrderSettings", "isVisibleFinancier", "BIT");
            AddFields("SaleOrderSettings", "isVisibleSalesExecutive", "BIT");
            AddFields("SaleOrderHeaders", "FinancierId", "Int", "People");
            AddFields("SaleOrderHeaders", "SalesExecutiveId", "Int", "People");

            AddFields("SaleInvoiceSettings", "isVisibleFinancier", "BIT");
            AddFields("SaleInvoiceSettings", "isVisibleSalesExecutive", "BIT");
            AddFields("SaleInvoiceHeaders", "FinancierId", "Int", "People");
            AddFields("SaleInvoiceHeaders", "SalesExecutiveId", "Int", "People");

            AddFields("SaleInvoiceSettings", "isVisibleBLNo", "BIT");
            AddFields("SaleInvoiceSettings", "isVisibleCircularNo", "BIT");
            AddFields("SaleInvoiceSettings", "isVisibleOrderNo", "BIT");
            AddFields("SaleInvoiceSettings", "isVisibleKindsOfackages", "BIT");

            AddFields("JobOrderSettings", "isVisibleProcessHeader", "BIT");


            AddFields("SaleInvoiceLines", "RateRemark", "nvarchar(Max)");
            AddFields("SaleInvoiceHeaderDetail", "Insurance", "Decimal(18,4)");

            AddFields("PackingLines", "FreeQty", "Decimal(18,4)");
            AddFields("SaleOrderLines", "FreeQty", "Decimal(18,4)");

            AddFields("CarpetSkuSettings", "NameBaseOnSize", "nvarchar(50)");

            AddFields("WeavingRetensions", "ProductQualityId", "INT", "ProductQualities");

            AddFields("SalaryHeaders", "IsDocumentPrinted", "Bit");
            AddFields("SalarySettings", "isAllowedDuplicatePrint", "BIT NOT NULL DEFAULT(1)");
            AddFields("SalarySettings", "isPrintinLetterhead", "BIT");

            AddFields("LedgerHeaders", "IsDocumentPrinted", "Bit");
            AddFields("LedgerSettings", "isAllowedDuplicatePrint", "BIT NOT NULL DEFAULT(1)");
            AddFields("LedgerSettings", "isPrintinLetterhead", "BIT");
            AddFields("LedgerSettings", "isVisibleQty", "BIT NOT NULL DEFAULT(0)");
            AddFields("LedgerSettings", "isVisibleDealQty", "BIT NOT NULL DEFAULT(0)");
            AddFields("LedgerSettings", "isVisibleRate", "BIT NOT NULL DEFAULT(0)");
            AddFields("LedgerSettings", "isVisibleSpecification", "BIT NOT NULL DEFAULT(0)");
            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleQuotationHeaders'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleQuotationHeaders
	                        (
	                        SaleQuotationHeaderId       INT IDENTITY NOT NULL,
	                        DocTypeId                   INT NOT NULL,
	                        DocDate                     DATETIME NOT NULL,
	                        DocNo                       NVARCHAR (20),
	                        DivisionId                  INT NOT NULL,
	                        SiteId                      INT NOT NULL,
	                        ParentSaleQuotationHeaderId INT,
	                        DueDate                     DATETIME NOT NULL,
	                        ExpiryDate                  DATETIME NOT NULL,
	                        ProcessId                   INT NOT NULL,
	                        CostCenterId                INT,
	                        SaleToBuyerId               INT NOT NULL,
	                        CurrencyId                  INT NOT NULL,
	                        TermsAndConditions          NVARCHAR (max),
	                        Status                      INT NOT NULL,
	                        UnitConversionForId         TINYINT NOT NULL,
	                        SalesTaxGroupPersonId       INT,
	                        Remark                      NVARCHAR (max),
	                        LockReason                  NVARCHAR (max),
	                        ReviewCount                 INT,
	                        ReviewBy                    NVARCHAR (max),
	                        CreatedBy                   NVARCHAR (max),
	                        CreatedDate                 DATETIME NOT NULL,
	                        ModifiedBy                  NVARCHAR (max),
	                        ModifiedDate                DATETIME NOT NULL,
	                        ReferenceDocTypeId          INT,
	                        ReferenceDocId              INT,
	                        OMSId                       NVARCHAR (50),
	                        CONSTRAINT [PK_Web.SaleQuotationHeaders] PRIMARY KEY (SaleQuotationHeaderId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaders_Web.CostCenters_CostCenterId] FOREIGN KEY (CostCenterId) REFERENCES Web.CostCenters (CostCenterId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaders_Web.Currencies_CurrencyId] FOREIGN KEY (CurrencyId) REFERENCES Web.Currencies (ID),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaders_Web.Divisions_DivisionId] FOREIGN KEY (DivisionId) REFERENCES Web.Divisions (DivisionId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaders_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaders_Web.SaleQuotationHeaders_ParentSaleQuotationHeaderId] FOREIGN KEY (ParentSaleQuotationHeaderId) REFERENCES Web.SaleQuotationHeaders (SaleQuotationHeaderId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaders_Web.Processes_ProcessId] FOREIGN KEY (ProcessId) REFERENCES Web.Processes (ProcessId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaders_Web.DocumentTypes_ReferenceDocTypeId] FOREIGN KEY (ReferenceDocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaders_Web.ChargeGroupPersons_SalesTaxGroupPersonId] FOREIGN KEY (SalesTaxGroupPersonId) REFERENCES Web.ChargeGroupPersons (ChargeGroupPersonId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaders_Web.People_SaleToBuyerId] FOREIGN KEY (SaleToBuyerId) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaders_Web.Sites_SiteId] FOREIGN KEY (SiteId) REFERENCES Web.Sites (SiteId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaders_Web.UnitConversionFors_UnitConversionForId] FOREIGN KEY (UnitConversionForId) REFERENCES Web.UnitConversionFors (UnitconversionForId)
	                        )
                        

                        CREATE INDEX IX_DocTypeId
	                        ON Web.SaleQuotationHeaders (DocTypeId)
                        

                        CREATE INDEX IX_DivisionId
	                        ON Web.SaleQuotationHeaders (DivisionId)
                        

                        CREATE INDEX IX_SiteId
	                        ON Web.SaleQuotationHeaders (SiteId)
                        

                        CREATE INDEX IX_ParentSaleQuotationHeaderId
	                        ON Web.SaleQuotationHeaders (ParentSaleQuotationHeaderId)
                        

                        CREATE INDEX IX_ProcessId
	                        ON Web.SaleQuotationHeaders (ProcessId)
                        

                        CREATE INDEX IX_CostCenterId
	                        ON Web.SaleQuotationHeaders (CostCenterId)
                        

                        CREATE INDEX IX_SaleToBuyerId
	                        ON Web.SaleQuotationHeaders (SaleToBuyerId)
                        

                        CREATE INDEX IX_CurrencyId
	                        ON Web.SaleQuotationHeaders (CurrencyId)
                        

                        CREATE INDEX IX_UnitConversionForId
	                        ON Web.SaleQuotationHeaders (UnitConversionForId)
                        

                        CREATE INDEX IX_SalesTaxGroupPersonId
	                        ON Web.SaleQuotationHeaders (SalesTaxGroupPersonId)
                        

                        CREATE INDEX IX_ReferenceDocTypeId
	                        ON Web.SaleQuotationHeaders (ReferenceDocTypeId)
                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleQuotationHeaderDetails'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleQuotationHeaderDetails
	                        (
	                        SaleQuotationHeaderId INT NOT NULL,
	                        Priority              INT NOT NULL,
	                        ShipMethodId          INT NOT NULL,
	                        IsDoorDelivery        BIT,
	                        DeliveryTermsId       INT NOT NULL,
	                        CreditDays            INT NOT NULL,
	                        FinancierId           INT,
	                        SalesExecutiveId      INT,
	                        AgentId               INT,
	                        PayTermAdvancePer     DECIMAL (18, 4),
	                        PayTermOnDeliveryPer  DECIMAL (18, 4),
	                        PayTermOnDueDatePer   DECIMAL (18, 4),
	                        PayTermCashPer        DECIMAL (18, 4),
	                        PayTermBankPer        DECIMAL (18, 4),
	                        PayTermDescription    NVARCHAR (max),
	                        CONSTRAINT [PK_Web.SaleQuotationHeaderDetails] PRIMARY KEY (SaleQuotationHeaderId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderDetails_Web.SaleQuotationHeaders_SaleQuotationHeaderId] FOREIGN KEY (SaleQuotationHeaderId) REFERENCES Web.SaleQuotationHeaders (SaleQuotationHeaderId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderDetails_Web.People_AgentId] FOREIGN KEY (AgentId) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderDetails_Web.DeliveryTerms_DeliveryTermsId] FOREIGN KEY (DeliveryTermsId) REFERENCES Web.DeliveryTerms (DeliveryTermsId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderDetails_Web.People_FinancierId] FOREIGN KEY (FinancierId) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderDetails_Web.People_SalesExecutiveId] FOREIGN KEY (SalesExecutiveId) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderDetails_Web.ShipMethods_ShipMethodId] FOREIGN KEY (ShipMethodId) REFERENCES Web.ShipMethods (ShipMethodId)
	                        )
                        

                        CREATE INDEX IX_ShipMethodId
	                        ON Web.SaleQuotationHeaderDetails (ShipMethodId)
                        

                        CREATE INDEX IX_DeliveryTermsId
	                        ON Web.SaleQuotationHeaderDetails (DeliveryTermsId)
                        

                        CREATE INDEX IX_FinancierId
	                        ON Web.SaleQuotationHeaderDetails (FinancierId)
                        

                        CREATE INDEX IX_SalesExecutiveId
	                        ON Web.SaleQuotationHeaderDetails (SalesExecutiveId)
                        

                        CREATE INDEX IX_AgentId
	                        ON Web.SaleQuotationHeaderDetails (AgentId)
                         ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }




            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'LedgerHeaderCharges'") == 0)
                {
                    mQry = @"CREATE TABLE Web.LedgerHeaderCharges
	                        (
	                        Id                         INT IDENTITY NOT NULL,
	                        HeaderTableId              INT NOT NULL,
	                        Sr                         INT NOT NULL,
	                        ChargeId                   INT NOT NULL,
	                        AddDeduct                  TINYINT,
	                        AffectCost                 BIT NOT NULL,
	                        ChargeTypeId               INT,
	                        ProductChargeId            INT,
	                        CalculateOnId              INT,
	                        PersonID                   INT,
	                        LedgerAccountDrId          INT,
	                        LedgerAccountCrId          INT,
	                        ContraLedgerAccountId      INT,
	                        CostCenterId               INT,
	                        RateType                   TINYINT NOT NULL,
	                        IncludedInBase             BIT NOT NULL,
	                        ParentChargeId             INT,
	                        Rate                       DECIMAL (18, 4),
	                        Amount                     DECIMAL (18, 4),
	                        IsVisible                  BIT NOT NULL,
	                        OMSId                      NVARCHAR (50),
	                        IncludedCharges            NVARCHAR (250),
	                        IncludedChargesCalculation NVARCHAR (250),
	                        CONSTRAINT [PK_Web.LedgerHeaderCharges] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.LedgerHeaderCharges_Web.Charges_CalculateOnId] FOREIGN KEY (CalculateOnId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.LedgerHeaderCharges_Web.Charges_ChargeId] FOREIGN KEY (ChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.LedgerHeaderCharges_Web.ChargeTypes_ChargeTypeId] FOREIGN KEY (ChargeTypeId) REFERENCES Web.ChargeTypes (ChargeTypeId),
	                        CONSTRAINT [FK_Web.LedgerHeaderCharges_Web.LedgerAccounts_ContraLedgerAccountId] FOREIGN KEY (ContraLedgerAccountId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.LedgerHeaderCharges_Web.CostCenters_CostCenterId] FOREIGN KEY (CostCenterId) REFERENCES Web.CostCenters (CostCenterId),
	                        CONSTRAINT [FK_Web.LedgerHeaderCharges_Web.LedgerHeaders_HeaderTableId] FOREIGN KEY (HeaderTableId) REFERENCES Web.LedgerHeaders (LedgerHeaderId),
	                        CONSTRAINT [FK_Web.LedgerHeaderCharges_Web.LedgerAccounts_LedgerAccountCrId] FOREIGN KEY (LedgerAccountCrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.LedgerHeaderCharges_Web.LedgerAccounts_LedgerAccountDrId] FOREIGN KEY (LedgerAccountDrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.LedgerHeaderCharges_Web.Charges_ParentChargeId] FOREIGN KEY (ParentChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.LedgerHeaderCharges_Web.People_PersonID] FOREIGN KEY (PersonID) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.LedgerHeaderCharges_Web.Charges_ProductChargeId] FOREIGN KEY (ProductChargeId) REFERENCES Web.Charges (ChargeId)
	                        )


                        CREATE INDEX [IX_HeaderTableId]
	                        ON Web.LedgerHeaderCharges (HeaderTableId)


                        CREATE INDEX [IX_ChargeId]
	                        ON Web.LedgerHeaderCharges (ChargeId)


                        CREATE INDEX [IX_ChargeTypeId]
	                        ON Web.LedgerHeaderCharges (ChargeTypeId)


                        CREATE INDEX [IX_ProductChargeId]
	                        ON Web.LedgerHeaderCharges (ProductChargeId)


                        CREATE INDEX [IX_CalculateOnId]
	                        ON Web.LedgerHeaderCharges (CalculateOnId)


                        CREATE INDEX [IX_PersonID]
	                        ON Web.LedgerHeaderCharges (PersonID)


                        CREATE INDEX [IX_LedgerAccountDrId]
	                        ON Web.LedgerHeaderCharges (LedgerAccountDrId)


                        CREATE INDEX [IX_LedgerAccountCrId]
	                        ON Web.LedgerHeaderCharges (LedgerAccountCrId)


                        CREATE INDEX [IX_ContraLedgerAccountId]
	                        ON Web.LedgerHeaderCharges (ContraLedgerAccountId)


                        CREATE INDEX [IX_CostCenterId]
	                        ON Web.LedgerHeaderCharges (CostCenterId)


                        CREATE INDEX [IX_ParentChargeId]
	                        ON Web.LedgerHeaderCharges (ParentChargeId)



                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SalaryLineReferences'") == 0)
                {
                    mQry = @"
                        CREATE TABLE Web.SalaryLineReferences
	                    (
	                    SalaryLineId                   INT NOT NULL,
	                    ReferenceDocTypeId             INT,
                        ReferenceDocId                 INT NOT NULL,
	                    ReferenceDocLineId             INT NOT NULL,                  
                        CONSTRAINT [PK_Web.SalaryLineReferences] PRIMARY KEY (SalaryLineId,ReferenceDocTypeId,ReferenceDocId,ReferenceDocLineId),
	                    CONSTRAINT [FK_Web.SalaryLineReferences_Web.SalaryLines_SalaryLineIdId] FOREIGN KEY (SalaryLineId) REFERENCES Web.SalaryLines (SalaryLineId),
	                    CONSTRAINT [FK_Web.SalaryLineReferences_Web.DocumentType_ReferenceDocTypeId] FOREIGN KEY (ReferenceDocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId)
	                    )

                    CREATE INDEX [IX_SalaryLineId]
	                    ON Web.SalaryLineReferences (SalaryLineId)


                    CREATE INDEX [IX_ReferenceDocTypeId]
	                    ON Web.SalaryLineReferences (ReferenceDocTypeId)


                    CREATE INDEX [IX_ReferenceDocId]
	                    ON Web.SalaryLineReferences (ReferenceDocId)

                    CREATE INDEX [IX_ReferenceDocLineId]
	                    ON Web.SalaryLineReferences (ReferenceDocLineId)

                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'LedgerLineCharges'") == 0)
                {
                    mQry = @"
                        CREATE TABLE Web.LedgerLineCharges
	                    (
	                    Id                            INT IDENTITY NOT NULL,
	                    LineTableId                   INT NOT NULL,
	                    HeaderTableId                 INT NOT NULL,
	                    Sr                            INT NOT NULL,
	                    ChargeId                      INT NOT NULL,
	                    AddDeduct                     TINYINT,
	                    AffectCost                    BIT NOT NULL,
	                    ChargeTypeId                  INT,
	                    CalculateOnId                 INT,
	                    PersonID                      INT,
	                    LedgerAccountDrId             INT,
	                    LedgerAccountCrId             INT,
	                    ContraLedgerAccountId         INT,
	                    CostCenterId                  INT,
	                    RateType                      TINYINT NOT NULL,
	                    IncludedInBase                BIT NOT NULL,
	                    ParentChargeId                INT,
	                    Rate                          DECIMAL (18, 4),
	                    Amount                        DECIMAL (18, 4),
	                    DealQty                       DECIMAL (18, 4),
	                    IsVisible                     BIT NOT NULL,
	                    OMSId                         NVARCHAR (50),
	                    DeveloperNotes                NVARCHAR (250),
	                    IncludedCharges               NVARCHAR (250),
	                    IncludedChargesCalculation    NVARCHAR (250),
	                    IsVisibleLedgerAccountDr      BIT,
	                    IsVisibleLedgerAccountCr      BIT,
	                    filterLedgerAccountGroupsDrId INT,
	                    filterLedgerAccountGroupsCrId INT,
	                    CONSTRAINT [PK_Web.LedgerLineCharges] PRIMARY KEY (Id),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.LedgerAccountGroups_filterLedgerAccountGroupsDrId] FOREIGN KEY (filterLedgerAccountGroupsDrId) REFERENCES Web.LedgerAccountGroups (LedgerAccountGroupId),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.LedgerAccountGroups_filterLedgerAccountGroupsCrId] FOREIGN KEY (filterLedgerAccountGroupsCrId) REFERENCES Web.LedgerAccountGroups (LedgerAccountGroupId),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.Charges_CalculateOnId] FOREIGN KEY (CalculateOnId) REFERENCES Web.Charges (ChargeId),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.Charges_ChargeId] FOREIGN KEY (ChargeId) REFERENCES Web.Charges (ChargeId),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.ChargeTypes_ChargeTypeId] FOREIGN KEY (ChargeTypeId) REFERENCES Web.ChargeTypes (ChargeTypeId),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.LedgerAccounts_ContraLedgerAccountId] FOREIGN KEY (ContraLedgerAccountId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.CostCenters_CostCenterId] FOREIGN KEY (CostCenterId) REFERENCES Web.CostCenters (CostCenterId),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.LedgerLines_LineTableId] FOREIGN KEY (LineTableId) REFERENCES Web.LedgerLines (LedgerLineId),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.LedgerAccounts_LedgerAccountCrId] FOREIGN KEY (LedgerAccountCrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.LedgerAccounts_LedgerAccountDrId] FOREIGN KEY (LedgerAccountDrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.Charges_ParentChargeId] FOREIGN KEY (ParentChargeId) REFERENCES Web.Charges (ChargeId),
	                    CONSTRAINT [FK_Web.LedgerLineCharges_Web.People_PersonID] FOREIGN KEY (PersonID) REFERENCES Web.People (PersonID)
	                    )


                    CREATE INDEX [IX_LineTableId]
	                    ON Web.LedgerLineCharges (LineTableId)


                    CREATE INDEX [IX_ChargeId]
	                    ON Web.LedgerLineCharges (ChargeId)


                    CREATE INDEX [IX_ChargeTypeId]
	                    ON Web.LedgerLineCharges (ChargeTypeId)


                    CREATE INDEX [IX_CalculateOnId]
	                    ON Web.LedgerLineCharges (CalculateOnId)


                    CREATE INDEX [IX_PersonID]
	                    ON Web.LedgerLineCharges (PersonID)


                    CREATE INDEX [IX_LedgerAccountDrId]
	                    ON Web.LedgerLineCharges (LedgerAccountDrId)


                    CREATE INDEX [IX_LedgerAccountCrId]
	                    ON Web.LedgerLineCharges (LedgerAccountCrId)


                    CREATE INDEX [IX_ContraLedgerAccountId]
	                    ON Web.LedgerLineCharges (ContraLedgerAccountId)


                    CREATE INDEX [IX_CostCenterId]
	                    ON Web.LedgerLineCharges (CostCenterId)


                    CREATE INDEX [IX_ParentChargeId]
	                    ON Web.LedgerLineCharges (ParentChargeId)



                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleQuotationHeaderCharges'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleQuotationHeaderCharges
	                        (
	                        Id                         INT IDENTITY NOT NULL,
	                        HeaderTableId              INT NOT NULL,
	                        Sr                         INT NOT NULL,
	                        ChargeId                   INT NOT NULL,
	                        AddDeduct                  TINYINT,
	                        AffectCost                 BIT NOT NULL,
	                        ChargeTypeId               INT,
	                        ProductChargeId            INT,
	                        CalculateOnId              INT,
	                        PersonID                   INT,
	                        LedgerAccountDrId          INT,
	                        LedgerAccountCrId          INT,
	                        ContraLedgerAccountId      INT,
	                        CostCenterId               INT,
	                        RateType                   TINYINT NOT NULL,
	                        IncludedInBase             BIT NOT NULL,
	                        ParentChargeId             INT,
	                        Rate                       DECIMAL (18, 4),
	                        Amount                     DECIMAL (18, 4),
	                        IsVisible                  BIT NOT NULL,
	                        IncludedCharges            NVARCHAR (max),
	                        IncludedChargesCalculation NVARCHAR (max),
	                        OMSId                      NVARCHAR (50),
	                        CONSTRAINT [PK_Web.SaleQuotationHeaderCharges] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderCharges_Web.Charges_CalculateOnId] FOREIGN KEY (CalculateOnId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderCharges_Web.Charges_ChargeId] FOREIGN KEY (ChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderCharges_Web.ChargeTypes_ChargeTypeId] FOREIGN KEY (ChargeTypeId) REFERENCES Web.ChargeTypes (ChargeTypeId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderCharges_Web.LedgerAccounts_ContraLedgerAccountId] FOREIGN KEY (ContraLedgerAccountId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderCharges_Web.CostCenters_CostCenterId] FOREIGN KEY (CostCenterId) REFERENCES Web.CostCenters (CostCenterId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderCharges_Web.LedgerAccounts_LedgerAccountCrId] FOREIGN KEY (LedgerAccountCrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderCharges_Web.LedgerAccounts_LedgerAccountDrId] FOREIGN KEY (LedgerAccountDrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderCharges_Web.Charges_ParentChargeId] FOREIGN KEY (ParentChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderCharges_Web.People_PersonID] FOREIGN KEY (PersonID) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderCharges_Web.Charges_ProductChargeId] FOREIGN KEY (ProductChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderCharges_Web.SaleQuotationHeaders_HeaderTableId] FOREIGN KEY (HeaderTableId) REFERENCES Web.SaleQuotationHeaders (SaleQuotationHeaderId)
	                        )
                        

                        CREATE INDEX IX_HeaderTableId
	                        ON Web.SaleQuotationHeaderCharges (HeaderTableId)
                        

                        CREATE INDEX IX_ChargeId
	                        ON Web.SaleQuotationHeaderCharges (ChargeId)
                        

                        CREATE INDEX IX_ChargeTypeId
	                        ON Web.SaleQuotationHeaderCharges (ChargeTypeId)
                        

                        CREATE INDEX IX_ProductChargeId
	                        ON Web.SaleQuotationHeaderCharges (ProductChargeId)
                        

                        CREATE INDEX IX_CalculateOnId
	                        ON Web.SaleQuotationHeaderCharges (CalculateOnId)
                        

                        CREATE INDEX IX_PersonID
	                        ON Web.SaleQuotationHeaderCharges (PersonID)
                        

                        CREATE INDEX IX_LedgerAccountDrId
	                        ON Web.SaleQuotationHeaderCharges (LedgerAccountDrId)
                        

                        CREATE INDEX IX_LedgerAccountCrId
	                        ON Web.SaleQuotationHeaderCharges (LedgerAccountCrId)
                        

                        CREATE INDEX IX_ContraLedgerAccountId
	                        ON Web.SaleQuotationHeaderCharges (ContraLedgerAccountId)
                        

                        CREATE INDEX IX_CostCenterId
	                        ON Web.SaleQuotationHeaderCharges (CostCenterId)
                        

                        CREATE INDEX IX_ParentChargeId
	                        ON Web.SaleQuotationHeaderCharges (ParentChargeId)
                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }




            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleQuotationLines'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleQuotationLines
	                        (
	                        SaleQuotationLineId      INT IDENTITY NOT NULL,
	                        SaleQuotationHeaderId    INT NOT NULL,
	                        SaleEnquiryLineId        INT,
	                        ProductId                INT NOT NULL,
	                        Dimension1Id             INT,
	                        Dimension2Id             INT,
	                        Dimension3Id             INT,
	                        Dimension4Id             INT,
	                        Specification            NVARCHAR (50),
	                        Qty                      DECIMAL (18, 4) NOT NULL,
	                        DealUnitId               NVARCHAR (3),
	                        DealQty                  DECIMAL (18, 4) NOT NULL,
	                        UnitConversionMultiplier DECIMAL (18, 4),
	                        Rate                     DECIMAL (18, 4) NOT NULL,
	                        Amount                   DECIMAL (18, 4) NOT NULL,
	                        DiscountPer              DECIMAL (18, 4),
	                        ReferenceDocTypeId       INT,
	                        ReferenceDocLineId       INT,
	                        Sr                       INT NOT NULL,
	                        Remark                   NVARCHAR (max),
	                        LockReason               NVARCHAR (max),
	                        CreatedBy                NVARCHAR (max),
	                        ModifiedBy               NVARCHAR (max),
	                        CreatedDate              DATETIME NOT NULL,
	                        ModifiedDate             DATETIME NOT NULL,
	                        OMSId                    NVARCHAR (50),
	                        CONSTRAINT [PK_Web.SaleQuotationLines] PRIMARY KEY (SaleQuotationLineId),
	                        CONSTRAINT [FK_Web.SaleQuotationLines_Web.Units_DealUnitId] FOREIGN KEY (DealUnitId) REFERENCES Web.Units (UnitId),
	                        CONSTRAINT [FK_Web.SaleQuotationLines_Web.Dimension1_Dimension1Id] FOREIGN KEY (Dimension1Id) REFERENCES Web.Dimension1 (Dimension1Id),
	                        CONSTRAINT [FK_Web.SaleQuotationLines_Web.Dimension2_Dimension2Id] FOREIGN KEY (Dimension2Id) REFERENCES Web.Dimension2 (Dimension2Id),
	                        CONSTRAINT [FK_Web.SaleQuotationLines_Web.Dimension3_Dimension3Id] FOREIGN KEY (Dimension3Id) REFERENCES Web.Dimension3 (Dimension3Id),
	                        CONSTRAINT [FK_Web.SaleQuotationLines_Web.Dimension4_Dimension4Id] FOREIGN KEY (Dimension4Id) REFERENCES Web.Dimension4 (Dimension4Id),
	                        CONSTRAINT [FK_Web.SaleQuotationLines_Web.Products_ProductId] FOREIGN KEY (ProductId) REFERENCES Web.Products (ProductId),
	                        CONSTRAINT [FK_Web.SaleQuotationLines_Web.DocumentTypes_ReferenceDocTypeId] FOREIGN KEY (ReferenceDocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                        CONSTRAINT [FK_Web.SaleQuotationLines_Web.SaleEnquiryLines_SaleEnquiryLineId] FOREIGN KEY (SaleEnquiryLineId) REFERENCES Web.SaleEnquiryLines (SaleEnquiryLineId),
	                        CONSTRAINT [FK_Web.SaleQuotationLines_Web.SaleQuotationHeaders_SaleQuotationHeaderId] FOREIGN KEY (SaleQuotationHeaderId) REFERENCES Web.SaleQuotationHeaders (SaleQuotationHeaderId)
	                        )
                        

                        CREATE UNIQUE INDEX IX_SaleQuotationLine_SaleQuotationHeaderProductDueDate
	                        ON Web.SaleQuotationLines (SaleQuotationHeaderId, ProductId, Dimension1Id, Dimension2Id, Dimension3Id, Dimension4Id)
                        

                        CREATE INDEX IX_SaleEnquiryLineId
	                        ON Web.SaleQuotationLines (SaleEnquiryLineId)
                        

                        CREATE INDEX IX_Dimension1Id
	                        ON Web.SaleQuotationLines (Dimension1Id)
                        

                        CREATE INDEX IX_Dimension2Id
	                        ON Web.SaleQuotationLines (Dimension2Id)
                        

                        CREATE INDEX IX_Dimension3Id
	                        ON Web.SaleQuotationLines (Dimension3Id)
                        

                        CREATE INDEX IX_Dimension4Id
	                        ON Web.SaleQuotationLines (Dimension4Id)
                        

                        CREATE INDEX IX_DealUnitId
	                        ON Web.SaleQuotationLines (DealUnitId)
                        

                        CREATE INDEX IX_ReferenceDocTypeId
	                        ON Web.SaleQuotationLines (ReferenceDocTypeId)
                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }




            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleQuotationLineCharges'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleQuotationLineCharges
	                        (
	                        Id                         INT IDENTITY NOT NULL,
	                        LineTableId                INT NOT NULL,
	                        HeaderTableId              INT NOT NULL,
	                        Sr                         INT NOT NULL,
	                        ChargeId                   INT NOT NULL,
	                        AddDeduct                  TINYINT,
	                        AffectCost                 BIT NOT NULL,
	                        ChargeTypeId               INT,
	                        CalculateOnId              INT,
	                        PersonID                   INT,
	                        LedgerAccountDrId          INT,
	                        LedgerAccountCrId          INT,
	                        ContraLedgerAccountId      INT,
	                        CostCenterId               INT,
	                        RateType                   TINYINT NOT NULL,
	                        IncludedInBase             BIT NOT NULL,
	                        ParentChargeId             INT,
	                        Rate                       DECIMAL (18, 4),
	                        Amount                     DECIMAL (18, 4),
	                        DealQty                    DECIMAL (18, 4),
	                        IsVisible                  BIT NOT NULL,
	                        IncludedCharges            NVARCHAR (max),
	                        IncludedChargesCalculation NVARCHAR (max),
	                        OMSId                      NVARCHAR (50),
	                        CONSTRAINT [PK_Web.SaleQuotationLineCharges] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.SaleQuotationLineCharges_Web.Charges_CalculateOnId] FOREIGN KEY (CalculateOnId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SaleQuotationLineCharges_Web.Charges_ChargeId] FOREIGN KEY (ChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SaleQuotationLineCharges_Web.ChargeTypes_ChargeTypeId] FOREIGN KEY (ChargeTypeId) REFERENCES Web.ChargeTypes (ChargeTypeId),
	                        CONSTRAINT [FK_Web.SaleQuotationLineCharges_Web.LedgerAccounts_ContraLedgerAccountId] FOREIGN KEY (ContraLedgerAccountId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SaleQuotationLineCharges_Web.CostCenters_CostCenterId] FOREIGN KEY (CostCenterId) REFERENCES Web.CostCenters (CostCenterId),
	                        CONSTRAINT [FK_Web.SaleQuotationLineCharges_Web.LedgerAccounts_LedgerAccountCrId] FOREIGN KEY (LedgerAccountCrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SaleQuotationLineCharges_Web.LedgerAccounts_LedgerAccountDrId] FOREIGN KEY (LedgerAccountDrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SaleQuotationLineCharges_Web.Charges_ParentChargeId] FOREIGN KEY (ParentChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SaleQuotationLineCharges_Web.People_PersonID] FOREIGN KEY (PersonID) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.SaleQuotationLineCharges_Web.SaleQuotationLines_LineTableId] FOREIGN KEY (LineTableId) REFERENCES Web.SaleQuotationLines (SaleQuotationLineId)
	                        )
                        

                        CREATE INDEX IX_LineTableId
	                        ON Web.SaleQuotationLineCharges (LineTableId)
                        

                        CREATE INDEX IX_ChargeId
	                        ON Web.SaleQuotationLineCharges (ChargeId)
                        

                        CREATE INDEX IX_ChargeTypeId
	                        ON Web.SaleQuotationLineCharges (ChargeTypeId)
                        

                        CREATE INDEX IX_CalculateOnId
	                        ON Web.SaleQuotationLineCharges (CalculateOnId)
                        

                        CREATE INDEX IX_PersonID
	                        ON Web.SaleQuotationLineCharges (PersonID)
                        

                        CREATE INDEX IX_LedgerAccountDrId
	                        ON Web.SaleQuotationLineCharges (LedgerAccountDrId)
                        

                        CREATE INDEX IX_LedgerAccountCrId
	                        ON Web.SaleQuotationLineCharges (LedgerAccountCrId)
                        

                        CREATE INDEX IX_ContraLedgerAccountId
	                        ON Web.SaleQuotationLineCharges (ContraLedgerAccountId)
                        

                        CREATE INDEX IX_CostCenterId
	                        ON Web.SaleQuotationLineCharges (CostCenterId)
                        

                        CREATE INDEX IX_ParentChargeId
	                        ON Web.SaleQuotationLineCharges (ParentChargeId)
                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }




            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleQuotationSettings'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleQuotationSettings
	                        (
	                        SaleQuotationSettingsId           INT IDENTITY NOT NULL,
	                        DocTypeId                         INT NOT NULL,
	                        Priority                          INT NOT NULL,
	                        ShipMethodId                      INT NOT NULL,
	                        CurrencyId                        INT NOT NULL,
	                        DeliveryTermsId                   INT NOT NULL,
	                        SiteId                            INT NOT NULL,
	                        DivisionId                        INT NOT NULL,
	                        UnitConversionForId               TINYINT NOT NULL,
	                        ProcessId                         INT NOT NULL,
	                        TermsAndConditions                NVARCHAR (max),
	                        isVisibleCurrency                 BIT,
	                        isVisibleShipMethod               BIT,
	                        isVisibleSalesTaxGroupPerson      BIT,
	                        isVisibleDoorDelivery             BIT,
	                        isVisibleCreditDays               BIT,
	                        isVisibleCostCenter               BIT,
	                        isVisibleDeliveryTerms            BIT,
	                        isVisiblePriority                 BIT,
	                        isVisibleDimension1               BIT,
	                        isVisibleDimension2               BIT,
	                        isVisibleDimension3               BIT,
	                        isVisibleDimension4               BIT,
	                        isVisibleDealUnit                 BIT,
	                        isVisibleSpecification            BIT,
	                        isVisibleProductCode              BIT,
	                        isVisibleUnitConversionFor        BIT,
	                        isVisibleFinancier                BIT,
	                        isVisibleSalesExecutive           BIT,
	                        isVisiblePaymentTerms             BIT,
	                        isUniqueCostCenter                BIT,
	                        IsPersonWiseCostCenter            BIT,
	                        isVisibleFromSaleEnquiry          BIT NOT NULL,
	                        isVisibleAgent                    BIT NOT NULL,
	                        filterLedgerAccountGroups         NVARCHAR (max),
	                        filterLedgerAccounts              NVARCHAR (max),
	                        filterProductTypes                NVARCHAR (max),
	                        filterProductGroups               NVARCHAR (max),
	                        filterProducts                    NVARCHAR (max),
	                        filterPersonRoles                 NVARCHAR (max),
	                        filterContraDocTypes              NVARCHAR (max),
	                        filterContraSites                 NVARCHAR (max),
	                        filterContraDivisions             NVARCHAR (max),
	                        FilterProductDivision             NVARCHAR (max),
	                        filterProductCategories           NVARCHAR (max),
	                        DocumentPrint                     NVARCHAR (100),
	                        NoOfPrintCopies                   INT,
	                        DealUnitId                        NVARCHAR (3),
	                        SqlProcDocumentPrint              NVARCHAR (100),
	                        SqlProcDocumentPrint_AfterSubmit  NVARCHAR (100),
	                        SqlProcDocumentPrint_AfterApprove NVARCHAR (100),
	                        ImportMenuId                      INT,
	                        WizardMenuId                      INT,
	                        ExportMenuId                      INT,
	                        CalculationId                     INT,
	                        CreatedBy                         NVARCHAR (max),
	                        ModifiedBy                        NVARCHAR (max),
	                        CreatedDate                       DATETIME NOT NULL,
	                        ModifiedDate                      DATETIME NOT NULL,
	                        OMSId                             NVARCHAR (50),
	                        CONSTRAINT [PK_Web.SaleQuotationSettings] PRIMARY KEY (SaleQuotationSettingsId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.Calculations_CalculationId] FOREIGN KEY (CalculationId) REFERENCES Web.Calculations (CalculationId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.Currencies_CurrencyId] FOREIGN KEY (CurrencyId) REFERENCES Web.Currencies (ID),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.Units_DealUnitId] FOREIGN KEY (DealUnitId) REFERENCES Web.Units (UnitId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.DeliveryTerms_DeliveryTermsId] FOREIGN KEY (DeliveryTermsId) REFERENCES Web.DeliveryTerms (DeliveryTermsId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.Divisions_DivisionId] FOREIGN KEY (DivisionId) REFERENCES Web.Divisions (DivisionId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.Menus_ExportMenuId] FOREIGN KEY (ExportMenuId) REFERENCES Web.Menus (MenuId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.Menus_ImportMenuId] FOREIGN KEY (ImportMenuId) REFERENCES Web.Menus (MenuId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.Processes_ProcessId] FOREIGN KEY (ProcessId) REFERENCES Web.Processes (ProcessId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.ShipMethods_ShipMethodId] FOREIGN KEY (ShipMethodId) REFERENCES Web.ShipMethods (ShipMethodId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.Sites_SiteId] FOREIGN KEY (SiteId) REFERENCES Web.Sites (SiteId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.UnitConversionFors_UnitConversionForId] FOREIGN KEY (UnitConversionForId) REFERENCES Web.UnitConversionFors (UnitconversionForId),
	                        CONSTRAINT [FK_Web.SaleQuotationSettings_Web.Menus_WizardMenuId] FOREIGN KEY (WizardMenuId) REFERENCES Web.Menus (MenuId)
	                        )
                        

                        CREATE INDEX IX_DocTypeId
	                        ON Web.SaleQuotationSettings (DocTypeId)
                        

                        CREATE INDEX IX_ShipMethodId
	                        ON Web.SaleQuotationSettings (ShipMethodId)
                        

                        CREATE INDEX IX_CurrencyId
	                        ON Web.SaleQuotationSettings (CurrencyId)
                        

                        CREATE INDEX IX_DeliveryTermsId
	                        ON Web.SaleQuotationSettings (DeliveryTermsId)
                        

                        CREATE INDEX IX_SiteId
	                        ON Web.SaleQuotationSettings (SiteId)
                        

                        CREATE INDEX IX_DivisionId
	                        ON Web.SaleQuotationSettings (DivisionId)
                        

                        CREATE INDEX IX_UnitConversionForId
	                        ON Web.SaleQuotationSettings (UnitConversionForId)
                        

                        CREATE INDEX IX_ProcessId
	                        ON Web.SaleQuotationSettings (ProcessId)
                        

                        CREATE INDEX IX_DealUnitId
	                        ON Web.SaleQuotationSettings (DealUnitId)
                        

                        CREATE INDEX IX_ImportMenuId
	                        ON Web.SaleQuotationSettings (ImportMenuId)
                        

                        CREATE INDEX IX_WizardMenuId
	                        ON Web.SaleQuotationSettings (WizardMenuId)
                        

                        CREATE INDEX IX_ExportMenuId
	                        ON Web.SaleQuotationSettings (ExportMenuId)
                        

                        CREATE INDEX IX_CalculationId
	                        ON Web.SaleQuotationSettings (CalculationId)
                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleInvoiceLineDetails'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleInvoiceLineDetails
	                        (
	                        SaleInvoiceLineId INT NOT NULL,
	                        RewardPoints      DECIMAL (18, 4),
	                        CONSTRAINT [PK_Web.SaleInvoiceLineDetails] PRIMARY KEY (SaleInvoiceLineId),
	                        CONSTRAINT [FK_Web.SaleInvoiceLineDetails_Web.SaleInvoiceLines_SaleInvoiceLineId] FOREIGN KEY (SaleInvoiceLineId) REFERENCES Web.SaleInvoiceLines (SaleInvoiceLineId)
	                        )

                        CREATE INDEX IX_SaleInvoiceLineId
	                        ON Web.SaleInvoiceLineDetails (SaleInvoiceLineId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            AddFields("SaleInvoiceSettings", "isVisibleFreeQty", "BIT");
            AddFields("SaleInvoiceSettings", "isVisibleRewardPoints", "BIT");

            AddFields("SaleDispatchSettings", "isVisibleFreeQty", "BIT");

            AddFields("PackingLines", "SealNo", "nvarchar(Max)");
            AddFields("PackingLines", "RateRemark", "nvarchar(Max)");
            
            AddFields("PackingLines", "FreeQty", "Decimal(18,4)");

            AddFields("SaleOrderLines", "FreeQty", "Decimal(18,4)");

            AddFields("SaleDispatchSettings", "isVisibleFreeQty", "BIT");

            AddFields("StockLines", "StockInId", "Int","Stocks");


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'CompanySettings'") == 0)
                {
                    mQry = @"CREATE TABLE Web.CompanySettings
	                        (
	                        CompanySettingsId     INT IDENTITY NOT NULL,
	                        CompanyId             INT NOT NULL,
	                        IsVisibleMessage      BIT,
	                        IsVisibleTask         BIT,
	                        IsVisibleNotification BIT,
	                        SiteCaption           NVARCHAR (50),
	                        DivisionCaption       NVARCHAR (50),
	                        GodownCaption         NVARCHAR (50),
	                        CreatedBy             NVARCHAR (max),
	                        ModifiedBy            NVARCHAR (max),
	                        CreatedDate           DATETIME NOT NULL,
	                        ModifiedDate          DATETIME NOT NULL,
	                        OMSId                 NVARCHAR (50),
	                        CONSTRAINT [PK_Web.CompanySettings] PRIMARY KEY (CompanySettingsId),
	                        CONSTRAINT [FK_Web.CompanySettings_Web.Companies_CompanyId] FOREIGN KEY (CompanyId) REFERENCES Web.Companies (CompanyId)
	                        )


                        CREATE INDEX IX_CompanyId
	                        ON Web.CompanySettings (CompanyId)
                        ";
                    ExecuteQuery(mQry);


                    mQry = @" INSERT INTO Web.CompanySettings (CompanyId, CreatedBy, ModifiedBy, CreatedDate, ModifiedDate)
                                SELECT C.CompanyId, 'admin' AS CreatedBy, 'admin' AS ModifiedBy, getdate() AS CreatedDate, 
                                getdate() AS ModifiedDate
                                FROM Web.Companies C
                                LEFT JOIN Web.CompanySettings Cs ON C.CompanyId = Cs.CompanyId
                                WHERE Cs.CompanySettingsId IS NULL";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            AddFields("ProductTypeSettings", "ImportMenuId", "Int","Menus");
            AddFields("SaleInvoiceSettings", "IsVisibleCreditDays", "Bit");

            AddFields("JobReceiveLines", "ProductId", "Int","Products");
            AddFields("JobReceiveLines", "Dimension1Id", "Int", "Dimension1");
            AddFields("JobReceiveLines", "Dimension2Id", "Int", "Dimension2");
            AddFields("JobReceiveLines", "Dimension3Id", "Int", "Dimension3");
            AddFields("JobReceiveLines", "Dimension4Id", "Int", "Dimension4");
            AddFields("JobInvoiceLines", "RateDiscountAmt", "Decimal(18,4)");

            AddFields("LedgerHeaders", "DrCr", "nvarchar(2)");
            AddFields("LedgerSettings", "isVisibleDrCr", "BIT");

            AddFields("ProductionOrderSettings", "ImportMenuId", "Int", "Menus");
            AddFields("ProductionOrderSettings", "ExportMenuId", "Int", "Menus");
            AddFields("ProductionOrderSettings", "WizardMenuId", "Int", "Menus");

            AddFields("ProdOrderSettings", "WizardMenuId", "Int", "Menus");

            AddFields("JobInvoiceSettings", "IsVisibleCreditDays", "Bit");

            AddFields("ProductBuyers", "Dimension1Id", "Int", "Dimension1");
            AddFields("ProductBuyers", "Dimension2Id", "Int", "Dimension2");

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ChargeGroupSettings'") == 0)
                {
                    mQry = @"CREATE TABLE Web.ChargeGroupSettings
	                        (
	                        ChargeGroupSettingsId    INT IDENTITY NOT NULL,
	                        ChargeTypeId             INT NOT NULL,
	                        ChargeGroupPersonId      INT NOT NULL,
	                        ChargeGroupProductId     INT NOT NULL,
	                        ChargableLedgerAccountId INT NOT NULL,
	                        ChargePer                DECIMAL (18, 4) NOT NULL,
	                        ChargeLedgerAccountId    INT NOT NULL,
	                        CreatedBy                NVARCHAR (max),
	                        ModifiedBy               NVARCHAR (max),
	                        CreatedDate              DATETIME NOT NULL,
	                        ModifiedDate             DATETIME NOT NULL,
	                        OMSId                    NVARCHAR (50),
	                        CONSTRAINT [PK_Web.ChargeGroupSettings] PRIMARY KEY (ChargeGroupSettingsId),
	                        CONSTRAINT [FK_Web.ChargeGroupSettings_Web.LedgerAccounts_ChargableLedgerAccountId] FOREIGN KEY (ChargableLedgerAccountId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.ChargeGroupSettings_Web.ChargeGroupPersons_ChargeGroupPersonId] FOREIGN KEY (ChargeGroupPersonId) REFERENCES Web.ChargeGroupPersons (ChargeGroupPersonId),
	                        CONSTRAINT [FK_Web.ChargeGroupSettings_Web.ChargeGroupProducts_ChargeGroupProductId] FOREIGN KEY (ChargeGroupProductId) REFERENCES Web.ChargeGroupProducts (ChargeGroupProductId),
	                        CONSTRAINT [FK_Web.ChargeGroupSettings_Web.LedgerAccounts_ChargeLedgerAccountId] FOREIGN KEY (ChargeLedgerAccountId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.ChargeGroupSettings_Web.ChargeTypes_ChargeTypeId] FOREIGN KEY (ChargeTypeId) REFERENCES Web.ChargeTypes (ChargeTypeId)
	                        )


                        CREATE INDEX IX_ChargeTypeId
	                        ON Web.ChargeGroupSettings (ChargeTypeId)


                        CREATE INDEX IX_ChargeGroupPersonId
	                        ON Web.ChargeGroupSettings (ChargeGroupPersonId)


                        CREATE INDEX IX_ChargeGroupProductId
	                        ON Web.ChargeGroupSettings (ChargeGroupProductId)


                        CREATE INDEX IX_ChargableLedgerAccountId
	                        ON Web.ChargeGroupSettings (ChargableLedgerAccountId)


                        CREATE INDEX IX_ChargeLedgerAccountId
	                        ON Web.ChargeGroupSettings (ChargeLedgerAccountId)
                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            AddFields("StockHeaderSettings", "IsVisibleReferenceDocId", "BIT");
            AddFields("StockHeaderSettings", "SqlProcHelpListReferenceDocId", "nvarchar(100)");

            AddFields("StockLines", "ReferenceDocTypeId", "Int","DocumentTypes");
            AddFields("StockLines", "ReferenceDocId", "Int");
            AddFields("StockLines", "ReferenceDocLineId", "Int");


            AddFields("StockHeaderSettings", "isVisibleProcessHeader", "BIT");
            AddFields("StockHeaderSettings", "isMandatoryProductUID", "BIT");

            AddFields("DocumentTypeSettings", "ReferenceDocTypeCaption", "nvarchar(50)");
            AddFields("DocumentTypeSettings", "ReferenceDocIdCaption", "nvarchar(50)");

            AddFields("LedgerSettings", "isVisibleReferenceDocId", "BIT");
            AddFields("LedgerSettings", "isVisibleReferenceDocTypeId", "BIT");
            AddFields("LedgerSettings", "filterReferenceDocTypes", "nvarchar(Max)");


            AddFields("CompanySettings", "isVisibleCompanyName", "BIT");

            AddFields("JobOrderLines", "StockInId", "Int", "Stocks");
            AddFields("JobOrderSettings", "isVisibleStockIn", "Bit");



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DocumentTypeAttributes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.DocumentTypeAttributes
	                        (
	                        DocumentTypeAttributeId INT IDENTITY NOT NULL,
	                        Name                    NVARCHAR (max) NOT NULL,
	                        IsMandatory             BIT NOT NULL,
	                        DataType                NVARCHAR (max),
	                        ListItem                NVARCHAR (max),
	                        DefaultValue            NVARCHAR (max),
	                        IsActive                BIT NOT NULL,
	                        DocumentTypeId          INT NOT NULL,
	                        CreatedBy               NVARCHAR (max),
	                        ModifiedBy              NVARCHAR (max),
	                        CreatedDate             DATETIME NOT NULL,
	                        ModifiedDate            DATETIME NOT NULL,
	                        OMSId                   NVARCHAR (50),
	                        CONSTRAINT [PK_Web.DocumentTypeAttributes] PRIMARY KEY (DocumentTypeAttributeId),
	                        CONSTRAINT [FK_Web.DocumentTypeAttributes_Web.DocumentTypes_DocumentTypeId] FOREIGN KEY (DocumentTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId)
	                        )


                        CREATE INDEX IX_DocumentTypeId
	                        ON Web.DocumentTypeAttributes (DocumentTypeId)                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



 


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleDeliveryHeaders'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleDeliveryHeaders
	                        (
	                        SaleDeliveryHeaderId     INT IDENTITY NOT NULL,
	                        DocTypeId                INT NOT NULL,
	                        DocDate                  DATETIME NOT NULL,
	                        DocNo                    NVARCHAR (20) NOT NULL,
	                        DivisionId               INT NOT NULL,
	                        SiteId                   INT NOT NULL,
	                        SaleToBuyerId            INT NOT NULL,
	                        DeliverToPerson          NVARCHAR (100),
	                        DeliverToPersonReference NVARCHAR (20),
	                        ShipToPartyAddress       NVARCHAR (250),
	                        GatePassHeaderId         INT,
	                        Remark                   NVARCHAR (max),
	                        Status                   INT NOT NULL,
	                        ReviewCount              INT,
	                        ReviewBy                 NVARCHAR (max),
	                        CreatedBy                NVARCHAR (max),
	                        ModifiedBy               NVARCHAR (max),
	                        CreatedDate              DATETIME NOT NULL,
	                        ModifiedDate             DATETIME NOT NULL,
	                        OMSId                    NVARCHAR (50),
	                        CONSTRAINT [PK_Web..SaleDeliveryHeaders] PRIMARY KEY (SaleDeliveryHeaderId),
	                        CONSTRAINT [FK_Web.SaleDeliveryHeaders_Web.Divisions_DivisionId] FOREIGN KEY (DivisionId) REFERENCES Web.Divisions (DivisionId),
	                        CONSTRAINT [FK_Web.SaleDeliveryHeaders_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                        CONSTRAINT [FK_Web.SaleDeliveryHeaders_Web.GatePassHeaders_GatePassHeaderId] FOREIGN KEY (GatePassHeaderId) REFERENCES Web.GatePassHeaders (GatePassHeaderId),
	                        CONSTRAINT [FK_Web.SaleDeliveryHeaders_Web.People_SaleToBuyerId] FOREIGN KEY (SaleToBuyerId) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.SaleDeliveryHeaders_Web.Sites_SiteId] FOREIGN KEY (SiteId) REFERENCES Web.Sites (SiteId)
	                        )


                        CREATE UNIQUE INDEX IX_SaleDeliveryHeader_DocID
	                        ON Web.SaleDeliveryHeaders (DocTypeId, DocNo, DivisionId, SiteId)


                        CREATE INDEX IX_SaleToBuyerId
	                        ON Web.SaleDeliveryHeaders (SaleToBuyerId)


                        CREATE INDEX IX_GatePassHeaderId
	                        ON Web.SaleDeliveryHeaders (GatePassHeaderId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleDeliveryLines'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleDeliveryLines
	                        (
	                        SaleDeliveryLineId       INT IDENTITY NOT NULL,
	                        SaleDeliveryHeaderId     INT NOT NULL,
	                        SaleInvoiceLineId        INT NOT NULL,
	                        Qty                      DECIMAL (18, 4) NOT NULL,
	                        DealUnitId               NVARCHAR (3) NOT NULL,
	                        UnitConversionMultiplier DECIMAL (18, 4) NOT NULL,
	                        DealQty                  DECIMAL (18, 4) NOT NULL,
	                        Remark                   NVARCHAR (max),
	                        CreatedBy                NVARCHAR (max),
	                        ModifiedBy               NVARCHAR (max),
	                        CreatedDate              DATETIME NOT NULL,
	                        ModifiedDate             DATETIME NOT NULL,
	                        OMSId                    NVARCHAR (50),
	                        LockReason               NVARCHAR (max),
	                        CONSTRAINT [PK_Web..SaleDeliveryLines] PRIMARY KEY (SaleDeliveryLineId),
	                        CONSTRAINT [FK_Web.SaleDeliveryLines_Web.Units_DealUnitId] FOREIGN KEY (DealUnitId) REFERENCES Web.Units (UnitId),
	                        CONSTRAINT [FK_Web.SaleDeliveryLines_Web.SaleDeliveryHeaders_SaleDeliveryHeaderId] FOREIGN KEY (SaleDeliveryHeaderId) REFERENCES Web.SaleDeliveryHeaders (SaleDeliveryHeaderId),
	                        CONSTRAINT [FK_Web.SaleDeliveryLines_Web.SaleInvoiceLines_SaleInvoiceLineId] FOREIGN KEY (SaleInvoiceLineId) REFERENCES Web.SaleInvoiceLines (SaleInvoiceLineId)
	                        )


                        CREATE INDEX IX_SaleDeliveryHeaderId
	                        ON Web.SaleDeliveryLines (SaleDeliveryHeaderId)


                        CREATE INDEX IX_SaleInvoiceLineId
	                        ON Web.SaleDeliveryLines (SaleInvoiceLineId)


                        CREATE INDEX IX_DealUnitId
	                        ON Web.SaleDeliveryLines (DealUnitId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleDeliverySettings'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleDeliverySettings
	                        (
	                        SaleDeliverySettingId             INT IDENTITY NOT NULL,
	                        DocTypeId                         INT NOT NULL,
	                        SiteId                            INT NOT NULL,
	                        DivisionId                        INT NOT NULL,
	                        isVisibleDimension1               BIT,
	                        isVisibleDimension2               BIT,
	                        isVisibleDimension3               BIT,
	                        isVisibleDimension4               BIT,
	                        filterLedgerAccountGroups         NVARCHAR (max),
	                        filterLedgerAccounts              NVARCHAR (max),
	                        filterProductTypes                NVARCHAR (max),
	                        filterProductGroups               NVARCHAR (max),
	                        filterProducts                    NVARCHAR (max),
	                        filterContraDocTypes              NVARCHAR (max),
	                        filterContraSites                 NVARCHAR (max),
	                        filterContraDivisions             NVARCHAR (max),
	                        filterPersonRoles                 NVARCHAR (max),
	                        SqlProcDocumentPrint              NVARCHAR (100),
	                        SqlProcDocumentPrint_AfterSubmit  NVARCHAR (100),
	                        SqlProcDocumentPrint_AfterApprove NVARCHAR (100),
	                        SqlProcGatePass                   NVARCHAR (100),
	                        UnitConversionForId               TINYINT,
	                        ImportMenuId                      INT,
	                        ExportMenuId                      INT,
	                        isVisibleDealUnit                 BIT,
	                        isVisibleProductUid               BIT,
	                        ProcessId                         INT,
	                        CreatedBy                         NVARCHAR (max),
	                        ModifiedBy                        NVARCHAR (max),
	                        CreatedDate                       DATETIME NOT NULL,
	                        ModifiedDate                      DATETIME NOT NULL,
	                        CONSTRAINT [PK_Web..SaleDeliverySettings] PRIMARY KEY (SaleDeliverySettingId),
	                        CONSTRAINT [FK_Web.SaleDeliverySettings_Web.Divisions_DivisionId] FOREIGN KEY (DivisionId) REFERENCES Web.Divisions (DivisionId),
	                        CONSTRAINT [FK_Web.SaleDeliverySettings_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                        CONSTRAINT [FK_Web.SaleDeliverySettings_Web.Menus_ExportMenuId] FOREIGN KEY (ExportMenuId) REFERENCES Web.Menus (MenuId),
	                        CONSTRAINT [FK_Web.SaleDeliverySettings_Web.Menus_ImportMenuId] FOREIGN KEY (ImportMenuId) REFERENCES Web.Menus (MenuId),
	                        CONSTRAINT [FK_Web.SaleDeliverySettings_Web.Processes_ProcessId] FOREIGN KEY (ProcessId) REFERENCES Web.Processes (ProcessId),
	                        CONSTRAINT [FK_Web.SaleDeliverySettings_Web.Sites_SiteId] FOREIGN KEY (SiteId) REFERENCES Web.Sites (SiteId),
	                        CONSTRAINT [FK_Web.SaleDeliverySettings_Web.UnitConversionFors_UnitConversionForId] FOREIGN KEY (UnitConversionForId) REFERENCES Web.UnitConversionFors (UnitconversionForId)
	                        )


                        CREATE INDEX IX_DocTypeId
	                        ON Web.SaleDeliverySettings (DocTypeId)


                        CREATE INDEX IX_SiteId
	                        ON Web.SaleDeliverySettings (SiteId)


                        CREATE INDEX IX_DivisionId
	                        ON Web.SaleDeliverySettings (DivisionId)


                        CREATE INDEX IX_UnitConversionForId
	                        ON Web.SaleDeliverySettings (UnitConversionForId)


                        CREATE INDEX IX_ImportMenuId
	                        ON Web.SaleDeliverySettings (ImportMenuId)


                        CREATE INDEX IX_ExportMenuId
	                        ON Web.SaleDeliverySettings (ExportMenuId)


                        CREATE INDEX IX_ProcessId
	                        ON Web.SaleDeliverySettings (ProcessId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            AddFields("LedgerSettings", "PartyDocNoCaption", "nvarchar(50)");
            AddFields("LedgerSettings", "PartyDocDateCaption", "nvarchar(50)");

            AddFields("JobReceiveLines", "Specification", "nvarchar(50)");
            AddFields("LedgerLines", "Specification", "nvarchar(50)");
            AddFields("LedgerLines", "Qty", " Decimal(18,4)");
            AddFields("LedgerLines", "DealQty", " Decimal(18,4)");
            AddFields("LedgerLines", "Rate", " Decimal(18,4)");

            AddFields("LedgerSettings", "isVisibleAdjustmentType", "BIT");
            AddFields("LedgerSettings", "isVisiblePaymentFor", "BIT");
            AddFields("LedgerSettings", "isVisiblePartyDocNo", "BIT");
            AddFields("LedgerSettings", "isVisiblePartyDocDate", "BIT");

            AddFields("LedgerHeaders", "PartyDocNo", "nvarchar(50)");
            AddFields("LedgerHeaders", "PartyDocDate", "DateTime");
            AddFields("LedgerSettings", "isVisibleLedgerAdj", "BIT");

            AddFields("PersonSettings", "isVisibleAadharNo", "BIT");
            AddFields("PersonSettings", "isMandatoryAadharNo", "BIT");
            AddFields("PersonSettings", "isVisiblePersonAddressDetail", "BIT");
            AddFields("PersonSettings", "isVisiblePersonOpeningDetail", "BIT");


            AddFields("JobInvoiceSettings", "isVisibleSalesTaxGroupProduct", "BIT");
            AddFields("JobInvoiceSettings", "isVisibleSalesTaxGroupPerson", "BIT");
            AddFields("JobInvoiceLines", "SalesTaxGroupProductId", "Int", "ChargeGroupProducts");
            AddFields("JobInvoiceHeaders", "SalesTaxGroupPersonId", "Int", "ChargeGroupPerson");

            AddFields("JobOrderLines", "SalesTaxGroupProductId", "Int", "ChargeGroupProducts");

            AddFields("SaleInvoiceSettings", "SqlProcProductUidHelpList", "nvarchar(100)");

            AddFields("LedgerSettings", "IsAutoDocNo", "BIT NOT NULL DEFAULT(0)");

            AddFields("LedgerSettings", "filterExcludeLedgerAccountGroupHeaders", "nvarchar(Max)");
            AddFields("LedgerSettings", "filterExcludeLedgerAccountGroupLines", "nvarchar(Max)");
            AddFields("PersonSettings", "isVisibleLedgerAccountGroup", "BIT");



            AddFields("ProductTypeSettings", "isVisibleDefaultDimension1", "BIT");
            AddFields("ProductTypeSettings", "isVisibleDefaultDimension2", "BIT");
            AddFields("ProductTypeSettings", "isVisibleDefaultDimension3", "BIT");
            AddFields("ProductTypeSettings", "isVisibleDefaultDimension4", "BIT");


            AddFields("Products", "DefaultDimension1Id", "Int", "Dimension1");
            AddFields("Products", "DefaultDimension2Id", "Int", "Dimension2");
            AddFields("Products", "DefaultDimension3Id", "Int", "Dimension3");
            AddFields("Products", "DefaultDimension4Id", "Int", "Dimension4");


            AddFields("SaleInvoiceHeaders", "TermsAndConditions", "nvarchar(Max)");
            AddFields("SaleInvoiceSettings", "isVisibleTermsAndConditions", "BIT");

            AddFields("ProductTypeSettings", "isVisibleDiscontinueDate", "BIT");
            AddFields("Products", "DiscontinueDate", "DateTime");
            AddFields("Products", "DiscontinueReason", "nvarchar(Max)");


            AddFields("CostCenters", "ProductUidId", "Int","ProductUids");
            AddFields("SaleDispatchLines", "CostCenterId", "Int", "CostCenters");

            AddFields("ProductTypeSettings", "IndexFilterParameter", "nvarchar(20)");

            AddFields("LedgerAccountGroups", "Weightage", "TINYINT");
            AddFields("Ledgers", "Priority", "Int");


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DiscountTypes'") == 0)
                {
                    mQry = @" CREATE TABLE Web.DiscountTypes
	                        (
	                        DiscountTypeId   INT IDENTITY NOT NULL,
	                        DocTypeId        INT NOT NULL,
	                        DiscountTypeName NVARCHAR (50) NOT NULL,
	                        Rate             DECIMAL (18, 4) NOT NULL,
	                        IsActive         BIT DEFAULT ((1)) NOT NULL,
	                        CreatedBy        NVARCHAR (max),
	                        ModifiedBy       NVARCHAR (max),
	                        CreatedDate      DATETIME NOT NULL,
	                        ModifiedDate     DATETIME NOT NULL,
	                        OMSId            NVARCHAR (50),
	                        CONSTRAINT [PK_Web.DiscountTypes] PRIMARY KEY (DiscountTypeId)
	                        CONSTRAINT [FK_Web.DiscountTypes_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId)
	                        )

                        CREATE UNIQUE INDEX IX_DiscountType_DiscountTypeName
	                        ON Web.DiscountTypes (DiscountTypeName)
	                        WITH (FILLFACTOR = 90)
                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            AddFields("ProductTypeSettings", "isPostedInStock", "BIT Default(1)");
            AddFields("PackingLines", "Sr", "Int");
            AddFields("SaleDispatchLines", "Sr", "Int");
            AddFields("SaleDispatchLines", "DiscountTypeId", "Int", "DiscountTypes");


            AddFields("ChargeGroupSettings", "ProcessId", "Int", "Processes");

            AddFields("ProductTypeSettings", "SqlProcProductCode", "nvarchar(100)");
            AddFields("PersonSettings", "SqlProcPersonCode", "nvarchar(100)");



            AddFields("Ledgers", "ChqDate", "DATETIME");
            AddFields("Ledgers", "BankDate", "DATETIME");


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ImportHeaders'") == 0)
                {
                    mQry = @"CREATE TABLE Web.ImportHeaders
	                        (
	                        ImportHeaderId INT IDENTITY NOT NULL,
	                        ImportName     NVARCHAR (max),
	                        SqlProc        NVARCHAR (max),
	                        Notes          NVARCHAR (max),
	                        CreatedBy      NVARCHAR (max),
	                        ModifiedBy     NVARCHAR (max),
	                        CreatedDate    DATETIME NOT NULL,
	                        ModifiedDate   DATETIME NOT NULL,
	                        FileType       NVARCHAR (10),
	                        CONSTRAINT [PK_Web.ImportHeaders] PRIMARY KEY (ImportHeaderId)
	                        )";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ImportLines'") == 0)
                {
                    mQry = @"CREATE TABLE Web.ImportLines
	                        (
	                        ImportLineId    INT IDENTITY NOT NULL,
	                        ImportHeaderId  INT NOT NULL,
	                        DisplayName     NVARCHAR (max) NOT NULL,
	                        FieldName       NVARCHAR (max) NOT NULL,
	                        DataType        NVARCHAR (max) NOT NULL,
	                        Type            NVARCHAR (max) NOT NULL,
	                        ListItem        NVARCHAR (max),
	                        DefaultValue    NVARCHAR (max),
	                        IsVisible       BIT NOT NULL,
	                        SqlProcGetSet   NVARCHAR (100),
	                        Serial          INT NOT NULL,
	                        NoOfCharToEnter INT,
	                        SqlParameter    NVARCHAR (max),
	                        IsCollapse      BIT NOT NULL,
	                        IsMandatory     BIT NOT NULL,
	                        PlaceHolder     NVARCHAR (max),
	                        ToolTip         NVARCHAR (max),
	                        CreatedBy       NVARCHAR (max),
	                        ModifiedBy      NVARCHAR (max),
	                        CreatedDate     DATETIME NOT NULL,
	                        ModifiedDate    DATETIME NOT NULL,
	                        MaxLength       INT,
	                        FileNo          INT NOT NULL,
	                        CONSTRAINT [PK_Web.ImportLines] PRIMARY KEY (ImportLineId),
	                        CONSTRAINT [FK_Web.ImportLines_Web.ImportHeaders_ImportHeaderId] FOREIGN KEY (ImportHeaderId) REFERENCES Web.ImportHeaders (ImportHeaderId)
	                        )";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ImportMessages'") == 0)
                {
                    mQry = @"CREATE TABLE Web.ImportMessages
	                        (
	                            ImportMessageId INT IDENTITY NOT NULL,
	                            ImportKey       NVARCHAR (50),
	                            ImportHeaderId  INT NOT NULL,
                                SqlProcedure    NVARCHAR (100),
	                            RecordId        NVARCHAR (100),
	                            Head            NVARCHAR (max),
	                            Value           NVARCHAR (max),
	                            ValueType       NVARCHAR (max),
	                            Remark          NVARCHAR (max),
	                            IsActive        BIT NOT NULL,
	                            CreatedBy       NVARCHAR (max),
	                            CreatedDate     DATETIME NOT NULL,
	                            CONSTRAINT [PK_Web.ImportMessages] PRIMARY KEY (ImportMessageId)
	                        )";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            AddFields("JobOrderSettings", "SqlProcProductUidHelpList", "nvarchar(100)");
            AddFields("JobReceiveSettings", "SqlProcProductUidHelpList", "nvarchar(100)");
            AddFields("JobInvoiceSettings", "SqlProcProductUidHelpList", "nvarchar(100)");
            AddFields("StockHeaderSettings", "SqlProcProductUidHelpList", "nvarchar(100)");
            AddFields("SaleDispatchReturnLines", "GodownId", "Int","Godowns");
            AddFields("SaleInvoiceSettings", "SaleInvoiceReturnDocTypeId", "Int", "DocumentTypes");
            AddFields("JobOrderSettings", "filterUnitConversionFors", "nvarchar(Max)");

            AddFields("LedgerSettings", "isVisibleLineDrCr", "Bit");

            AddFields("LedgerLines", "DrCr", "nvarchar(2)");

            AddFields("SaleDeliveryLines", "Sr", "int");
            AddFields("SaleDeliverySettings", "WizardMenuId", "int","Menus");


            AddFields("TrialBalanceSettings", "ShowContraAccount", "BIT NOT NULL DEFAULT(1)");


            DropFields("JobInvoiceLines", "MfgDate");

            AddFields("Stocks", "MfgDate", "DateTime");
            AddFields("ProductUids", "MfgDate", "DateTime");



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SalesTaxProductCodes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SalesTaxProductCodes
	                            (
	                            SalesTaxProductCodeId INT IDENTITY NOT NULL,
	                            Code                  NVARCHAR (50) NOT NULL,
	                            Description           NVARCHAR (max),
	                            IsActive              BIT NOT NULL,
	                            CreatedBy             NVARCHAR (max),
	                            ModifiedBy            NVARCHAR (max),
	                            CreatedDate           DATETIME NOT NULL,
	                            ModifiedDate          DATETIME NOT NULL,
	                            OMSId                 NVARCHAR (50),
	                            CONSTRAINT [PK_Web.SalesTaxProductCodes] PRIMARY KEY (SalesTaxProductCodeId)
	                            )

                            CREATE UNIQUE INDEX IX_SalesTaxProduct_SalesTaxProductCode
	                            ON Web.SalesTaxProductCodes (Code)
                            ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }




            AddFields("Products", "SalesTaxProductCodeId", "Int", "SalesTaxProductCodes");
            AddFields("ProductGroups", "DefaultSalesTaxProductCodeId", "Int", "SalesTaxProductCodes");
            AddFields("ProductTypeSettings", "isVisibleSalesTaxProductCode", "Bit");
            AddFields("ProductTypeSettings", "SalesTaxProductCodeCaption", "nvarchar(50)");

            AddFields("SaleInvoiceSettings", "isVisibleShipToPartyAddress", "Bit");


            AddFields("PersonSettings", "isVisibleGstNo", "Bit");
            AddFields("PersonSettings", "isMandatoryGstNo", "Bit");

            AddFields("LedgerHeaders", "ForLedgerHeaderId", "Int", "LedgerHeaders");
            AddFields("LedgerSettings", "CancelDocTypeId", "Int","DocumentTypes");


            AddFields("CarpetSkuSettings", "isVisibleSalesTaxProductCode", "Bit");
            AddFields("CarpetSkuSettings", "SalesTaxProductCodeCaption", "nvarchar(50)");

            AddFields("ProductCategories", "DefaultSalesTaxProductCodeId", "Int", "SalesTaxProductCodes");

            AddFields("PackingSettings", "filterLedgerAccountGroups", "nvarchar(Max)");
            AddFields("PackingSettings", "filterLedgerAccounts", "nvarchar(Max)");

            AddFields("SaleInvoiceSettings", "DoNotUpdateProductUidStatus", "BIT");

            AddFields("StockHeaderSettings", "isPostedInStock", "BIT NOT NULL DEFAULT(1)");

            AddFields("PackingSettings", "isVisibleHeaderJobWorker", "BIT");
            AddFields("PackingSettings", "isVisibleBaleNoPattern", "BIT");
            AddFields("PackingSettings", "isVisibleGrossWeight", "BIT");
            AddFields("PackingSettings", "isVisibleNetWeight", "BIT");
            AddFields("PackingSettings", "isVisibleProductInvoiceGroup", "BIT");
            AddFields("PackingSettings", "isVisibleSaleDeliveryOrder", "BIT");

            AddFields("PackingSettings", "isVisibleProductUID", "BIT");
            AddFields("PackingSettings", "isVisibleShipMethod", "BIT");
            AddFields("PackingSettings", "filterProductDivision", "nvarchar(Max)");
			AddFields("PackingSettings", "isAllowtoUpdateBuyerSpecification", "BIT");

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*)  FROM Web.AspNetRoles WHERE Name = 'SysAdmin'") == 0)
                {
                    mQry = @"INSERT INTO Web.AspNetRoles (Id, Name)
                            VALUES ('3aa1e14b-aee8-4721-b7dc-180d67b09310', 'SysAdmin')";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            AddFields("ProductGroups", "DefaultSalesTaxGroupProductId", "Int", "ChargeGroupProducts");

            AddFields("PackingLines", "BaleCount", "Int");
            AddFields("PackingSettings", "isVisibleBaleCount", "Bit");

            AddFields("PackingSettings", "ProcessId", "Int","Processes");


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SiteDivisionSettings'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SiteDivisionSettings
	                        (
	                        SiteDivisionSettingsId INT IDENTITY NOT NULL,
	                        SiteId                 INT NOT NULL,
	                        DivisionId             INT NOT NULL,
	                        StartDate              DATETIME NOT NULL,
	                        EndDate                DATETIME,
	                        ReportHeaderTextLeft   NVARCHAR (max),
	                        ReportHeaderTextRight  NVARCHAR (max),
	                        IsApplicableVAT        BIT DEFAULT ((0)) NOT NULL,
	                        IsApplicableGST        BIT DEFAULT ((1)) NOT NULL,
	                        CreatedBy              NVARCHAR (max),
	                        ModifiedBy             NVARCHAR (max),
	                        CreatedDate            DATETIME NOT NULL,
	                        ModifiedDate           DATETIME NOT NULL,
	                        CONSTRAINT [PK_Web.SiteDivisionSettings] PRIMARY KEY (SiteDivisionSettingsId),
	                        CONSTRAINT [FK_Web.SiteDivisionSettings_Web.Divisions_DivisionId] FOREIGN KEY (DivisionId) REFERENCES Web.Divisions (DivisionId),
	                        CONSTRAINT [FK_Web.SiteDivisionSettings_Web.Sites_SiteId] FOREIGN KEY (SiteId) REFERENCES Web.Sites (SiteId)
	                        )
                        

                        CREATE INDEX IX_SiteId
	                        ON Web.SiteDivisionSettings (SiteId)
                        

                        CREATE INDEX IX_DivisionId
	                        ON Web.SiteDivisionSettings (DivisionId)
                        ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            DropFields("Sites", "ReportHeaderTextLeft");
            DropFields("Sites", "ReportHeaderTextRight");


            

            try
            {
                mQry = @"INSERT INTO Web.SiteDivisionSettings (SiteId, DivisionId, StartDate, EndDate, ReportHeaderTextLeft, ReportHeaderTextRight, IsApplicableVAT, IsApplicableGST, CreatedBy, ModifiedBy, CreatedDate, ModifiedDate)
                    SELECT S.SiteId, D.DivisionId, '01/Apr/1900' AS StartDate, '30/Jun/2017' AS EndDate, NULL AS ReportHeaderTextLeft, 
                    NULL AS ReportHeaderTextRight, 1 AS IsApplicableVAT, 0 AS IsApplicableGST, 'System' AS CreatedBy, 'System' AS ModifiedBy, 
                    getdate() AS CreatedDate, getdate() AS ModifiedDate
                    FROM Web.Sites S
                    LEFT JOIN Web.Divisions D ON 1 = 1
                    LEFT JOIN Web.SiteDivisionSettings Sds ON S.SiteId = Sds.SiteId
				                    AND D.DivisionId = Sds.DivisionId
				                    AND Sds.StartDate < '01/Jul/2017'
                    WHERE Sds.SiteDivisionSettingsId IS NULL

                    UNION ALL 

                    SELECT S.SiteId, D.DivisionId, '01/Jul/2017' AS StartDate, NULL AS EndDate, NULL AS ReportHeaderTextLeft, 
                    NULL AS ReportHeaderTextRight, 0 AS IsApplicableVAT, 1 AS IsApplicableGST, 'System' AS CreatedBy, 'System' AS ModifiedBy, 
                    getdate() AS CreatedDate, getdate() AS ModifiedDate
                    FROM Web.Sites S
                    LEFT JOIN Web.Divisions D ON 1 = 1
                    LEFT JOIN Web.SiteDivisionSettings Sds ON S.SiteId = Sds.SiteId
				                    AND D.DivisionId = Sds.DivisionId
				                    AND Sds.StartDate >= '01/Jul/2017'
                    WHERE Sds.SiteDivisionSettingsId IS NULL";
                ExecuteQuery(mQry);
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            //DropFields("SaleInvoiceLines", "SalesTaxGroupId");
            //DropFields("SaleInvoiceSettings", "SalesTaxGroupId");

            RenameFields("SaleInvoiceSettings", "isVisibleSalesTaxGroup", "isVisibleSalesTaxGroupPerson");

            AddFields("SaleInvoiceLines", "SalesTaxGroupProductId", "Int", "ChargeGroupProducts");
            AddFields("SaleInvoiceHeaders", "SalesTaxGroupPersonId", "Int", "ChargeGroupPersons");
            AddFields("SaleInvoiceSettings", "SalesTaxGroupPersonId", "Int", "ChargeGroupPersons");


            AddFields("SaleInvoiceSettings", "isVisibleSalesTaxGroupProduct", "Bit");

            AddFields("JobInvoiceSettings", "SalesTaxGroupPersonId", "Int", "ChargeGroupPersons");



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DocumentTypeHeaderAttributes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.DocumentTypeHeaderAttributes
	                    (
	                    DocumentTypeHeaderAttributeId INT IDENTITY NOT NULL,
	                    DocumentTypeId                INT NOT NULL,
	                    Sr                            INT NOT NULL,
	                    Name                          NVARCHAR (max) NOT NULL,
	                    IsMandatory                   BIT NOT NULL,
	                    DataType                      NVARCHAR (max),
	                    ListItem                      NVARCHAR (max),
	                    Value                         NVARCHAR (max),
	                    IsActive                      BIT NOT NULL,
	                    CreatedBy                     NVARCHAR (max),
	                    ModifiedBy                    NVARCHAR (max),
	                    CreatedDate                   DATETIME NOT NULL,
	                    ModifiedDate                  DATETIME NOT NULL,
	                    OMSId                         NVARCHAR (50),
	                    CONSTRAINT [PK_Web.DocumentTypeHeaderAttributes] PRIMARY KEY (DocumentTypeHeaderAttributeId),
	                    CONSTRAINT [FK_Web.DocumentTypeHeaderAttributes_Web.DocumentType_DocumentTypeId] FOREIGN KEY (DocumentTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId)
	                    )

                    CREATE INDEX IX_DocumentTypeId
	                    ON Web.DocumentTypeHeaderAttributes (DocumentTypeId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'JobOrderHeaderAttributes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.JobOrderHeaderAttributes
	                            (
	                            Id                            INT IDENTITY NOT NULL,
	                            HeaderTableId                 INT NOT NULL,
	                            DocumentTypeHeaderAttributeId INT NOT NULL,
	                            Value                         NVARCHAR (max),
	                            CONSTRAINT [PK_Web.JobOrderHeaderAttributes] PRIMARY KEY (Id),
	                            CONSTRAINT [FK_Web.JobOrderHeaderAttributes_Web.DocumentTypeHeaderAttributes_DocumentTypeHeaderAttributeId] FOREIGN KEY (DocumentTypeHeaderAttributeId) REFERENCES Web.DocumentTypeHeaderAttributes (DocumentTypeHeaderAttributeId),
	                            CONSTRAINT [FK_Web.JobOrderHeaderAttributes_Web.JobOrderHeaders_HeaderTableId] FOREIGN KEY (HeaderTableId) REFERENCES Web.JobOrderHeaders (JobOrderHeaderId)
	                            )

                            CREATE INDEX IX_HeaderTableId
	                            ON Web.JobOrderHeaderAttributes (HeaderTableId)

                            CREATE INDEX IX_DocumentTypeHeaderAttributeId
	                            ON Web.JobOrderHeaderAttributes (DocumentTypeHeaderAttributeId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'JobInvoiceHeaderAttributes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.JobInvoiceHeaderAttributes
	                        (
	                        Id                            INT IDENTITY NOT NULL,
	                        HeaderTableId                 INT NOT NULL,
	                        DocumentTypeHeaderAttributeId INT NOT NULL,
	                        Value                         NVARCHAR (max),
	                        CONSTRAINT [PK_Web.JobInvoiceHeaderAttributes] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.JobInvoiceHeaderAttributes_Web.DocumentTypeHeaderAttributes_DocumentTypeHeaderAttributeId] FOREIGN KEY (DocumentTypeHeaderAttributeId) REFERENCES Web.DocumentTypeHeaderAttributes (DocumentTypeHeaderAttributeId),
	                        CONSTRAINT [FK_Web.JobInvoiceHeaderAttributes_Web.JobInvoiceHeaders_HeaderTableId] FOREIGN KEY (HeaderTableId) REFERENCES Web.JobInvoiceHeaders (JobInvoiceHeaderId)
	                        )

                            CREATE INDEX IX_HeaderTableId
	                            ON Web.JobInvoiceHeaderAttributes (HeaderTableId)

                            CREATE INDEX IX_DocumentTypeHeaderAttributeId
	                            ON Web.JobInvoiceHeaderAttributes (DocumentTypeHeaderAttributeId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StockHeaderAttributes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.StockHeaderAttributes
	                        (
	                        Id                            INT IDENTITY NOT NULL,
	                        HeaderTableId                 INT NOT NULL,
	                        DocumentTypeHeaderAttributeId INT NOT NULL,
	                        Value                         NVARCHAR (max),
	                        CONSTRAINT [PK_Web.StockHeaderAttributes] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.StockHeaderAttributes_Web.DocumentTypeHeaderAttributes_DocumentTypeHeaderAttributeId] FOREIGN KEY (DocumentTypeHeaderAttributeId) REFERENCES Web.DocumentTypeHeaderAttributes (DocumentTypeHeaderAttributeId),
	                        CONSTRAINT [FK_Web.StockHeaderAttributes_Web.StockHeaders_HeaderTableId] FOREIGN KEY (HeaderTableId) REFERENCES Web.StockHeaders (StockHeaderId)
	                        )

                            CREATE INDEX IX_HeaderTableId
	                            ON Web.StockHeaderAttributes (HeaderTableId)

                            CREATE INDEX IX_DocumentTypeHeaderAttributeId
	                            ON Web.StockHeaderAttributes (DocumentTypeHeaderAttributeId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleInvoiceHeaderAttributes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleInvoiceHeaderAttributes
	                        (
	                        Id                            INT IDENTITY NOT NULL,
	                        HeaderTableId                 INT NOT NULL,
	                        DocumentTypeHeaderAttributeId INT NOT NULL,
	                        Value                         NVARCHAR (max),
	                        CONSTRAINT [PK_Web.SaleInvoiceHeaderAttributes] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.SaleInvoiceHeaderAttributes_Web.DocumentTypeHeaderAttributes_DocumentTypeHeaderAttributeId] FOREIGN KEY (DocumentTypeHeaderAttributeId) REFERENCES Web.DocumentTypeHeaderAttributes (DocumentTypeHeaderAttributeId),
	                        CONSTRAINT [FK_Web.SaleInvoiceHeaderAttributes_Web.SaleInvoiceHeaders_HeaderTableId] FOREIGN KEY (HeaderTableId) REFERENCES Web.SaleInvoiceHeaders (SaleInvoiceHeaderId)
	                        )

                            CREATE INDEX IX_HeaderTableId
	                            ON Web.SaleInvoiceHeaderAttributes (HeaderTableId)

                            CREATE INDEX IX_DocumentTypeHeaderAttributeId
	                            ON Web.SaleInvoiceHeaderAttributes (DocumentTypeHeaderAttributeId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ViewStockInBalance'") == 0)
                {
                    mQry = @"CREATE VIEW Web.ViewStockInBalance
                                AS 
                                SELECT L.StockId AS StockInId, H.DocNo AS StockInNo, H.DocDate AS StockInDate, 
                                H.PersonId, L.ProductId, L.Dimension1Id, L.Dimension2Id, L.Dimension3Id, L.Dimension4Id, L.LotNo, 
                                IsNull(L.Qty_Rec,0) - IsNull(VStockAdj.Qty,0) AS BalanceQty,
                                H.DivisionId, H.SiteId
                                FROM Web.Stocks L WITH (NoLock)
                                LEFT JOIN Web.StockHeaders H WITH (NoLock) ON L.StockHeaderId = H.StockHeaderId
                                LEFT JOIN (
	                                SELECT L.StockInId, Sum(L.AdjustedQty) AS Qty
	                                FROM Web.StockAdjs L WITH (NoLock)
	                                GROUP BY L.StockInId
                                ) AS VStockAdj ON L.StockId = VStockAdj.StockInId
                                WHERE 1=1
                                AND IsNull(L.Qty_Rec,0) > 0
                                AND IsNull(L.Qty_Rec,0) - IsNull(VStockAdj.Qty,0) > 0";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SaleQuotationHeaderAttributes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SaleQuotationHeaderAttributes
	                        (
	                        Id                            INT IDENTITY NOT NULL,
	                        HeaderTableId                 INT NOT NULL,
	                        DocumentTypeHeaderAttributeId INT NOT NULL,
	                        Value                         NVARCHAR (max),
	                        CONSTRAINT [PK_Web.SaleQuotationHeaderAttributes] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderAttributes_Web.DocumentTypeHeaderAttributes_DocumentTypeHeaderAttributeId] FOREIGN KEY (DocumentTypeHeaderAttributeId) REFERENCES Web.DocumentTypeHeaderAttributes (DocumentTypeHeaderAttributeId),
	                        CONSTRAINT [FK_Web.SaleQuotationHeaderAttributes_Web.SaleQuotationHeaders_HeaderTableId] FOREIGN KEY (HeaderTableId) REFERENCES Web.SaleQuotationHeaders (SaleQuotationHeaderId)
	                        )

                        CREATE INDEX IX_HeaderTableId
	                        ON Web.SaleQuotationHeaderAttributes (HeaderTableId)

                        CREATE INDEX IX_DocumentTypeHeaderAttributeId
	                        ON Web.SaleQuotationHeaderAttributes (DocumentTypeHeaderAttributeId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            AddFields("ProductUids", "ProductUidSpecification1", "NVARCHAR(Max)");

            AddFields("ProductSiteDetails", "ProcessId", "Int","Processes");
            AddFields("ProductSiteDetails", "ExcessReceiveAllowedAgainstOrderQty", "Decimal(18,4)");
            AddFields("ProductSiteDetails", "ExcessReceiveAllowedAgainstOrderPer", "Decimal(18,4)");

            
            AddFields("SaleQuotationSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("SaleOrderSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("SaleEnquirySettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("SaleDeliverySettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("SaleInvoiceSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("SaleDispatchSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("SaleDeliveryOrderSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");

            AddFields("JobReceiveSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("JobOrderInspectionSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("JobOrderSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("JobOrderInspectionRequestSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("JobConsumptionSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("JobInvoiceSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");
            AddFields("JobReceiveQASettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");

            AddFields("StockHeaderSettings", "DocumentPrintReportHeaderId", "Int", "ReportHeaders");


            AddFields("SaleQuotationSettings", "CalculateDiscountOnRate", "Bit");
            AddFields("SaleQuotationSettings", "isVisibleDiscountPer", "Bit");

            AddFields("SaleQuotationLines", "DiscountPer", "Decimal(18,4)");
            AddFields("SaleQuotationLines", "DiscountAmount", "Decimal(18,4)");


            AddFields("SaleQuotationLines", "SalesTaxGroupProductId", "Int", "ChargeGroupProducts");
            AddFields("SaleQuotationSettings", "isVisibleSalesTaxGroupProduct", "Bit");

            AddFields("ChargeTypes", "Category", "nvarchar(20)");

            AddFields("JobOrderSettings", "isVisibleSalesTaxGroupProduct", "BIT");


            AddFields("SiteDivisionSettings", "SalesTaxProductCodeCaption", "NVARCHAR(50)");
            AddFields("SiteDivisionSettings", "SalesTaxCaption", "NVARCHAR(50)");
            AddFields("SiteDivisionSettings", "SalesTaxGroupProductCaption", "NVARCHAR(50)");
            AddFields("SiteDivisionSettings", "SalesTaxGroupPersonCaption", "NVARCHAR(50)");
            AddFields("SiteDivisionSettings", "SalesTaxRegistrationCaption", "NVARCHAR(50)");

            AddFields("ChargeGroupProducts", "PrintingDescription", "NVARCHAR(50)");
            AddFields("ChargeGroupPersons", "PrintingDescription", "NVARCHAR(50)");


            AddFields("ProductGroups", "RateDecimalPlaces", "TINYINT Not Null DEFAULT(2)");

            AddFields("JobInvoiceReturnHeaders", "Nature", "nvarchar(20) Not Null DEFAULT('Return')");
            AddFields("SaleInvoiceReturnHeaders", "Nature", "nvarchar(20) Not Null DEFAULT('Return')");

            AddFields("States", "StateCode", "nvarchar(20)");
            AddFields("Charges", "PrintingDescription", "nvarchar(50)");


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM Web.DocumentCategories WHERE DocumentCategoryName = 'Sales Tax Product Code'") == 0)
                {
                    mQry = @"INSERT INTO Web.DocumentCategories (DocumentCategoryName, IsActive, IsSystemDefine, OMSId)
                            VALUES ('Sales Tax Product Code', 1, 0, NULL)";
                    ExecuteQuery(mQry);
                }

                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM Web.DocumentTypes WHERE DocumentTypeName = 'Sales Tax Product Code'") == 0)
                {
                    mQry = @"INSERT INTO Web.DocumentTypes (DocumentTypeShortName, DocumentTypeName, DocumentCategoryId, ControllerActionId, DomainName, VoucherType, IsSystemDefine, IsActive, ReportMenuId, Nature, IconDisplayName, ImageFileName, ImageFolderName, SupportGatePass, CreatedBy, ModifiedBy, CreatedDate, ModifiedDate, DatabaseTableName, ControllerName, ActionNamePendingToSubmit, OMSId, isDivisionBased, isSiteBased, ControllerNameDetail, ActionNameDetail, ActionName, PrintTitle)
                            VALUES ('STPC', 'Sales Tax Product Code', (SELECT DocumentCategoryId  FROM Web.DocumentCategories WHERE DocumentCategoryName = 'Sales Tax Product Code'), NULL, NULL, NULL, 1, 1, NULL, NULL, NULL, NULL, NULL, 0, 'Admin', 'Admin', '2015-06-18 18:56:12.907', '2015-06-18 18:56:12.907', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM Web.UnitConversionFors WHERE UnitconversionForName = 'LengthPerimeter'") == 0)
                {
                    mQry = @"INSERT INTO Web.UnitConversionFors (UnitconversionForId, UnitconversionForName)
                            VALUES (7, 'LengthPerimeter')";
                    ExecuteQuery(mQry);
                }

                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM Web.UnitConversionFors WHERE UnitconversionForName = 'WidthPerimeter'") == 0)
                {
                    mQry = @"INSERT INTO Web.UnitConversionFors (UnitconversionForId, UnitconversionForName)
                            VALUES (8, 'WidthPerimeter')";
                    ExecuteQuery(mQry);
                }

                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM Web.UnitConversionFors WHERE UnitconversionForName = 'TotalPerimeter'") == 0)
                {
                    mQry = @"INSERT INTO Web.UnitConversionFors (UnitconversionForId, UnitconversionForName)
                            VALUES (9, 'TotalPerimeter')";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            AddFields ("JobOrderSettings", "isVisibleLineUnitConversionFor", "BIT NOT NULL DEFAULT(0)");

            AddFields("CalculationLineLedgerAccounts", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("CalculationLineLedgerAccounts", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("CalculationLineLedgerAccounts", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("CalculationLineLedgerAccounts", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("JobInvoiceLineCharges", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("JobInvoiceLineCharges", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("JobInvoiceLineCharges", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("JobInvoiceLineCharges", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("JobInvoiceRateAmendmentLineCharges", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("JobInvoiceRateAmendmentLineCharges", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("JobInvoiceRateAmendmentLineCharges", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("JobInvoiceRateAmendmentLineCharges", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("JobInvoiceReturnLineCharges", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("JobInvoiceReturnLineCharges", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("JobInvoiceReturnLineCharges", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("JobInvoiceReturnLineCharges", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("JobOrderLineCharges", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("JobOrderLineCharges", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("JobOrderLineCharges", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("JobOrderLineCharges", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("PurchaseInvoiceLineCharges", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("PurchaseInvoiceLineCharges", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("PurchaseInvoiceLineCharges", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("PurchaseInvoiceLineCharges", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("PurchaseInvoiceReturnLineCharges", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("PurchaseInvoiceReturnLineCharges", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("PurchaseInvoiceReturnLineCharges", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("PurchaseInvoiceReturnLineCharges", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("PurchaseOrderLineCharges", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("PurchaseOrderLineCharges", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("PurchaseOrderLineCharges", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("PurchaseOrderLineCharges", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("PurchaseOrderRateAmendmentLineCharges", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("PurchaseOrderRateAmendmentLineCharges", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("PurchaseOrderRateAmendmentLineCharges", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("PurchaseOrderRateAmendmentLineCharges", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("SaleInvoiceLineCharges", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("SaleInvoiceLineCharges", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("SaleInvoiceLineCharges", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("SaleInvoiceLineCharges", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("SaleInvoiceReturnLineCharges", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("SaleInvoiceReturnLineCharges", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("SaleInvoiceReturnLineCharges", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("SaleInvoiceReturnLineCharges", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("SaleQuotationLineCharges", "IsVisibleLedgerAccountDr", "BIT");
            AddFields("SaleQuotationLineCharges", "IsVisibleLedgerAccountCr", "BIT");
            AddFields("SaleQuotationLineCharges", "filterLedgerAccountGroupsDrId", "Int", "LedgerAccountGroups");
            AddFields("SaleQuotationLineCharges", "filterLedgerAccountGroupsCrId", "Int", "LedgerAccountGroups");

            AddFields("JobInvoiceSettings", "isVisibleGodown", "BIT");
            AddFields("JobInvoiceSettings", "isVisibleJobReceiveBy", "BIT");

            AddFields("PackingSettings", "IsMandatoryStockIn", "BIT");
            AddFields("PackingSettings", "UnitConversionForId", "TINYINT", "UnitConversionFors");
            AddFields("PackingSettings", "ExportMenuId", "INT", "Menus");


            AddFields("PackingLines", "StockInId", "INT", "Stocks");


            AddFields("PackingSettings", "isVisibleStockIn", "BIT");
            AddFields("PackingSettings", "isVisibleSpecification", "BIT");
            AddFields("PackingSettings", "isVisibleLotNo", "BIT");
            AddFields("PackingSettings", "isVisibleBaleNo", "BIT");
            AddFields("PackingSettings", "isVisibleDealUnit", "BIT");


            AddFields("JobOrderSettings", "IsMandatoryStockIn", "BIT");
            AddFields("StockHeaderSettings", "IsMandatoryStockIn", "BIT");
            AddFields("StockHeaderSettings", "isMandatoryLotNo", "BIT");
            AddFields("StockHeaderSettings", "isMandatoryLotNoOrDimension1", "BIT");
            AddFields("JobReceiveSettings", "isMandatoryLotNo", "BIT");
            AddFields("JobReceiveSettings", "isMandatoryLotNoOrDimension1", "BIT");

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ChargeGroupPersonCalculations'") == 0)
                {
                    mQry = @"CREATE TABLE Web.ChargeGroupPersonCalculations
	                        (
	                        ChargeGroupPersonCalculationId INT IDENTITY NOT NULL,
	                        DocTypeId                      INT NOT NULL,
	                        DivisionId                     INT,
	                        SiteId                         INT,
	                        ChargeGroupPersonId            INT NOT NULL,
	                        CalculationId                  INT NOT NULL,
	                        CreatedBy                      NVARCHAR (max),
	                        ModifiedBy                     NVARCHAR (max),
	                        CreatedDate                    DATETIME NOT NULL,
	                        ModifiedDate                   DATETIME NOT NULL,
	                        OMSId                          NVARCHAR (50),
	                        CONSTRAINT [PK_Web.ChargeGroupPersonCalculations] PRIMARY KEY (ChargeGroupPersonCalculationId),
	                        CONSTRAINT [FK_Web.ChargeGroupPersonCalculations_Web.Calculations_CalculationId] FOREIGN KEY (CalculationId) REFERENCES Web.Calculations (CalculationId),
	                        CONSTRAINT [FK_Web.ChargeGroupPersonCalculations_Web.ChargeGroupPersons_ChargeGroupPersonId] FOREIGN KEY (ChargeGroupPersonId) REFERENCES Web.ChargeGroupPersons (ChargeGroupPersonId),
	                        CONSTRAINT [FK_Web.ChargeGroupPersonCalculations_Web.Divisions_DivisionId] FOREIGN KEY (DivisionId) REFERENCES Web.Divisions (DivisionId),
	                        CONSTRAINT [FK_Web.ChargeGroupPersonCalculations_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                        CONSTRAINT [FK_Web.ChargeGroupPersonCalculations_Web.Sites_SiteId] FOREIGN KEY (SiteId) REFERENCES Web.Sites (SiteId)
	                        )


                        CREATE UNIQUE INDEX ChargeGroupPersonCalculation_DocID
	                        ON Web.ChargeGroupPersonCalculations (DocTypeId, DivisionId, SiteId, ChargeGroupPersonId)


                        CREATE INDEX IX_CalculationId
	                        ON Web.ChargeGroupPersonCalculations (CalculationId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            AddFields("JobInvoiceSettings", "isVisibleJobOrder", "BIT");
            AddFields("JobInvoiceSettings", "isMandatoryJobOrder", "BIT");

            AddFields("JobInvoiceSettings", "isVisiblePassQty", "BIT");
            AddFields("JobInvoiceSettings", "isVisibleQty", "BIT DEFAULT ((1)) NOT NULL");
            AddFields("JobInvoiceSettings", "isVisibleRate", "BIT DEFAULT ((1)) NOT NULL");

            AddFields("JobOrderHeaders", "ReferenceDocTypeId", "Int","DocumentTypes");
            AddFields("JobOrderHeaders", "ReferenceDocId", "Int");

            AddFields("JobInvoiceSettings", "isVisibleJobReceive", "BIT");
            AddFields("JobInvoiceSettings", "isMandatoryJobReceive", "BIT");

            AddFields("JobReceiveSettings", "isVisibleProcessHeader", "BIT");
            AddFields("JobInvoiceSettings", "isVisibleProcessHeader", "BIT");

            AddFields("LedgerAccounts", "ProductId", "Int","Products");


            DropFields("JobInvoiceSettings", "IsVisibleQty");

            AddFields("JobInvoiceSettings", "IsVisibleDocQty", "Bit");
            AddFields("JobInvoiceSettings", "IsVisibleReceiveQty", "Bit");
            AddFields("JobInvoiceSettings", "IsVisibleAdditionalCharges", "Bit");

            AddFields("JobReceiveHeaders", "JobWorkerDocDate", "DATETIME");

            AddFields("ProductUids", "SaleOrderLineId", "Int","SaleOrderLines");
            AddFields("ProductUids", "ProdOrderLineId", "Int", "ProdOrderLines");
            AddFields("Menus", "AreaName", "nvarchar(50)");



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Projects'") == 0)
                {
                    mQry = @"CREATE TABLE Web.Projects
	                        (
	                        ProjectId    INT IDENTITY NOT NULL,
	                        ProjectName  NVARCHAR (50) NOT NULL,
	                        IsActive     BIT NOT NULL,
	                        CreatedBy    NVARCHAR (max),
	                        ModifiedBy   NVARCHAR (max),
	                        CreatedDate  DATETIME NOT NULL,
	                        ModifiedDate DATETIME NOT NULL,
	                        OMSId        NVARCHAR (50),
	                        CONSTRAINT [PK_Web.Projects] PRIMARY KEY (ProjectId)
	                        )


                        CREATE UNIQUE INDEX IX_Project_ProjectName
	                        ON Web.Projects (ProjectName)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Tasks'") == 0)
                {
                    mQry = @"CREATE TABLE Web.Tasks
	                            (
	                            TaskId          INT IDENTITY NOT NULL,
	                            TaskTitle       NVARCHAR (250) NOT NULL,
	                            TaskDescription NVARCHAR (max),
	                            ProjectId       INT,
	                            Priority        INT,
	                            Status          NVARCHAR (20),
	                            ForUser         NVARCHAR (max),
	                            DueDate         DATETIME,
	                            IsActive        BIT NOT NULL,
	                            CreatedBy       NVARCHAR (max),
	                            ModifiedBy      NVARCHAR (max),
	                            CreatedDate     DATETIME NOT NULL,
	                            ModifiedDate    DATETIME NOT NULL,
	                            OMSId           NVARCHAR (50),
	                            CONSTRAINT [PK_Web.Tasks] PRIMARY KEY (TaskId),
	                            CONSTRAINT [FK_Web.Tasks_Web.Projects_ProjectId] FOREIGN KEY (ProjectId) REFERENCES Web.Projects (ProjectId)
	                            )


                            CREATE INDEX IX_ProjectId
	                            ON Web.Tasks (ProjectId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DARs'") == 0)
                {
                    mQry = @"CREATE TABLE Web.DARs
	                        (
	                        DARId        INT IDENTITY NOT NULL,
	                        DARDate      DATETIME NOT NULL,
	                        FromTime     DATETIME NOT NULL,
	                        ToTime       DATETIME NOT NULL,
	                        WorkHours    DECIMAL (18, 4) NOT NULL,
	                        TaskId       INT NOT NULL,
	                        Description  NVARCHAR (max),
	                        Priority     INT,
	                        Status       NVARCHAR (20),
	                        ForUser      NVARCHAR (max),
	                        DueDate      DATETIME,
	                        IsActive     BIT NOT NULL,
	                        CreatedBy    NVARCHAR (max),
	                        ModifiedBy   NVARCHAR (max),
	                        CreatedDate  DATETIME NOT NULL,
	                        ModifiedDate DATETIME NOT NULL,
	                        OMSId        NVARCHAR (50),
	                        CONSTRAINT [PK_Web.DARs] PRIMARY KEY (DARId),
	                        CONSTRAINT [FK_Web.DARs_Web.Tasks_TaskId] FOREIGN KEY (TaskId) REFERENCES Web.Tasks (TaskId)
	                        )


                        CREATE INDEX IX_TaskId
	                        ON Web.DARs (TaskId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'UserTeams'") == 0)
                {
                    mQry = @"CREATE TABLE Web.UserTeams
	                        (
	                        UserTeamId   INT IDENTITY NOT NULL,
	                        Srl          NVARCHAR (5) NOT NULL,
	                        ProjectId    INT NOT NULL,
	                        [User]       NVARCHAR (255) NOT NULL,
	                        TeamUser     NVARCHAR (255) NOT NULL,
	                        CreatedBy    NVARCHAR (max),
	                        ModifiedBy   NVARCHAR (max),
	                        CreatedDate  DATETIME NOT NULL,
	                        ModifiedDate DATETIME NOT NULL,
	                        CONSTRAINT [PK_Web.UserTeams] PRIMARY KEY (UserTeamId),
	                        CONSTRAINT [FK_Web.UserTeams_Web.Projects_ProjectId] FOREIGN KEY (ProjectId) REFERENCES Web.Projects (ProjectId)
	                        )


                        CREATE UNIQUE INDEX IX_UserTeam_TeamName
	                        ON Web.UserTeams (ProjectId, [User], TeamUser)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'NotificationSubjects'") == 0)
                {
                    mQry = @"CREATE TABLE Web.NotificationSubjects
	                        (
	                        NotificationSubjectId   INT IDENTITY NOT NULL,
	                        NotificationSubjectName NVARCHAR (50) NOT NULL,
	                        IconName                NVARCHAR (max),
	                        IsActive                BIT NOT NULL,
	                        CreatedBy               NVARCHAR (max),
	                        ModifiedBy              NVARCHAR (max),
	                        CreatedDate             DATETIME NOT NULL,
	                        ModifiedDate            DATETIME NOT NULL,
	                        OMSId                   NVARCHAR (50),
	                        CONSTRAINT [PK_Web.NotificationSubjects] PRIMARY KEY (NotificationSubjectId)
	                        )


                        CREATE UNIQUE INDEX IX_NotificationSubject_NotificationSubjectName
	                        ON Web.NotificationSubjects (NotificationSubjectName)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Notifications'") == 0)
                {
                    mQry = @"CREATE TABLE Web.Notifications
	                        (
	                        NotificationId        INT IDENTITY NOT NULL,
	                        NotificationSubjectId INT NOT NULL,
	                        NotificationText      NVARCHAR (max) NOT NULL,
	                        NotificationUrl       NVARCHAR (max),
	                        UrlKey                NVARCHAR (max),
	                        ExpiryDate            DATETIME,
	                        ReadDate              DATETIME,
	                        SeenDate              DATETIME,
	                        IsActive              BIT NOT NULL,
	                        CreatedBy             NVARCHAR (max),
	                        ModifiedBy            NVARCHAR (max),
	                        CreatedDate           DATETIME NOT NULL,
	                        ModifiedDate          DATETIME NOT NULL,
	                        OMSId                 NVARCHAR (50),
	                        CONSTRAINT [PK_Web.Notifications] PRIMARY KEY (NotificationId),
	                        CONSTRAINT [FK_Web.Notifications_Web.NotificationSubjects_NotificationSubjectId] FOREIGN KEY (NotificationSubjectId) REFERENCES Web.NotificationSubjects (NotificationSubjectId)
	                        )


                        CREATE INDEX IX_NotificationSubjectId
	                        ON Web.Notifications (NotificationSubjectId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'NotificationUsers'") == 0)
                {
                    mQry = @"CREATE TABLE Web.NotificationUsers
	                            (
	                            NotificationUserId INT IDENTITY NOT NULL,
	                            NotificationId     INT NOT NULL,
	                            UserName           NVARCHAR (128) NOT NULL,
	                            OMSId              NVARCHAR (50),
	                            CONSTRAINT [PK_Web.NotificationUsers] PRIMARY KEY (NotificationUserId),
	                            CONSTRAINT [FK_Web.NotificationUsers_Web.Notifications_NotificationId] FOREIGN KEY (NotificationId) REFERENCES Web.Notifications (NotificationId)
	                            )


                            CREATE INDEX IX_NotificationId
	                            ON Web.NotificationUsers (NotificationId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DocumentNatures'") == 0)
                {
                    mQry = @"CREATE TABLE Web.DocumentNatures
	                        (
	                        DocumentNatureId   INT IDENTITY NOT NULL,
	                        DocumentNatureName NVARCHAR (50) NOT NULL,
	                        IsActive           BIT NOT NULL,
	                        IsSystemDefine     BIT NOT NULL,
	                        OMSId              NVARCHAR (50),
	                        CONSTRAINT PK_Web.DocumentNatures PRIMARY KEY (DocumentNatureId)
	                        )

                        CREATE UNIQUE INDEX IX_DocumentNature_DocumentNatureName
	                        ON Web.DocumentNatures (DocumentNatureName)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            AddFields("Sites", "SiteShortCode", "NVARCHAR (2)");
            AddFields("Divisions", "DivisionShortCode", "NVARCHAR (2)");
            AddFields("DocumentTypes", "DocumentNatureId", "Int");


            AddFields("JobInvoiceLines", "ReferenceDocTypeId", "Int", "DocumentTypes");
            AddFields("JobInvoiceLines", "ReferenceDocLineId", "Int");

            AddFields("JobInvoiceReturnHeaders", "ReferenceDocTypeId", "Int", "DocumentTypes");
            AddFields("JobInvoiceReturnHeaders", "ReferenceDocId", "Int");

            AddFields("JobInvoiceReturnLines", "ReferenceDocTypeId", "Int", "DocumentTypes");
            AddFields("JobInvoiceReturnLines", "ReferenceDocLineId", "Int");

            AddFields("JobReceiveLines", "PenaltyReason", "nvarchar(Max)");

            AddFields("JobOrderLines", "DiscountPer", "Decimal(18,4)");

            AddFields("JobReceiveLines", "BaleNo", "nvarchar(10)");

            AddFields("JobInvoiceReturnHeaderCharges", "IncludedCharges", "NVARCHAR (250)");
            AddFields("JobInvoiceReturnHeaderCharges", "IncludedChargesCalculation", "NVARCHAR (250)");

            AddFields("JobInvoiceReturnLineCharges", "IncludedCharges", "NVARCHAR (250)");
            AddFields("JobInvoiceReturnLineCharges", "IncludedChargesCalculation", "NVARCHAR (250)");


            AddFields("SaleInvoiceReturnLines", "DiscountAmount", "Decimal(18,4)");


            AddFields("JobOrderLines", "DiscountAmount", "Decimal(18,4)");
            AddFields("JobOrderSettings", "isVisibleDiscountPer", "BIT");
            AddFields("JobOrderSettings", "isVisibleProdOrder", "BIT");

            AddFields("JobOrderSettings", "CalculateDiscountOnRate", "Bit");

            AddFields("JobOrderLines", "UnitConversionForId", "TINYINT", "UnitConversionFors");

            AddFields("Menus", "ControllerName", "nvarchar(100)");
            AddFields("Menus", "ActionName", "nvarchar(100)");

            AddFields("JobInvoiceHeaders", "GovtInvoiceNo", "nvarchar(20)");

            AddFields("JobInvoiceSettings", "isVisibleGovtInvoiceNo", "BIT");

            AddFields("SaleInvoiceSettings", "isVisiblePacking", "BIT");


            AddFields("JobInvoiceReturnHeaders", "SalesTaxGroupPersonId", "Int", "ChargeGroupPersons");
            AddFields("JobInvoiceReturnLines", "SalesTaxGroupProductId", "Int", "ChargeGroupProducts");

            AddFields("DocumentTypes", "ActionName", "nvarchar(50)");


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'JobOrderBomMaterialIssues'") == 0)
                {
                    mQry = @"CREATE TABLE Web.JobOrderBomMaterialIssues
	                            (
	                            JobOrderBomMaterialIssueId INT IDENTITY NOT NULL,
	                            JobOrderBomId              INT,
	                            StockLineId                INT NOT NULL,
	                            IssueForQty                DECIMAL (18, 4) NOT NULL,
	                            Qty                        DECIMAL (18, 4) NOT NULL,
	                            CreatedBy                  NVARCHAR (max),
	                            ModifiedBy                 NVARCHAR (max),
	                            CreatedDate                DATETIME NOT NULL,
	                            ModifiedDate               DATETIME NOT NULL,
	                            OMSId                      NVARCHAR (50),
	                            CONSTRAINT [PK_Web.JobOrderBomMaterialIssues] PRIMARY KEY (JobOrderBomMaterialIssueId),
	                            CONSTRAINT [FK_Web.JobOrderBomMaterialIssues_Web.JobOrderBoms_JobOrderBomId] FOREIGN KEY (JobOrderBomId) REFERENCES Web.JobOrderBoms (JobOrderBomId),
	                            CONSTRAINT [FK_Web.JobOrderBomMaterialIssues_Web.StockLines_StockLineId] FOREIGN KEY (StockLineId) REFERENCES Web.StockLines (StockLineId)
	                            )

                            CREATE INDEX IX_JobOrderBomId
	                            ON Web.JobOrderBomMaterialIssues (JobOrderBomId)

                            CREATE INDEX IX_StockLineId
	                            ON Web.JobOrderBomMaterialIssues (StockLineId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            AddFields("Units", "GSTUnit", "nvarchar(50)");

            AddFields("PromoCodes", "ProcessId", "Int Not Null","Processes");
            AddFields("PromoCodes", "MinQty", "Decimal(18,4)");


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DivisionSettings'") == 0)
                {
                    mQry = @"CREATE TABLE Web.DivisionSettings
	                        (
	                            DivisionSettingId INT IDENTITY NOT NULL,
	                            DivisionId        INT NOT NULL,
	                            Dimension1Caption NVARCHAR (50),
	                            Dimension2Caption NVARCHAR (50),
	                            Dimension3Caption NVARCHAR (50),
	                            Dimension4Caption NVARCHAR (50),
	                            CreatedBy         NVARCHAR (max),
	                            ModifiedBy        NVARCHAR (max),
	                            CreatedDate       DATETIME NOT NULL,
	                            ModifiedDate      DATETIME NOT NULL,
	                            CONSTRAINT [PK_Web.DivisionSettings] PRIMARY KEY (DivisionSettingId),
	                            CONSTRAINT [FK_Web.DivisionSettings_Web.Divisions_DivisionId] FOREIGN KEY (DivisionId) REFERENCES Web.Divisions (DivisionId)
	                        )

                        CREATE INDEX IX_DivisionId
	                        ON Web.DivisionSettings (DivisionId)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            AddFields("Processes", "SalesTaxProductCodeId", "Int", "SalesTaxProductCodes");
            AddFields("JobInvoiceHeaders", "CurrencyId", "Int", "Currencies");
            AddFields("DocumentTypeSettings", "DocIdCaption", "NVARCHAR (100)");
            AddFields("DocumentTypeSettings", "PrintProductGroup", "Bit");
            AddFields("DocumentTypeSettings", "PrintProductDescription", "Bit");
            AddFields("DocumentTypeSettings", "PrintSpecification", "Bit");

            AddFields("DivisionSettings", "ProductUidCaption", "NVARCHAR (50)");

            AddFields("JobInvoiceReturnLines", "CostCenterId", "Int", "CostCenters");

            AddFields("UserRoles", "SiteId", "Int","Sites");
            AddFields("UserRoles", "DivisionId", "Int", "Divisions");

            AddFields("UserRoles", "CreatedBy", "nvarchar(Max)");
            AddFields("UserRoles", "CreatedDate", "DATETIME");
            AddFields("UserRoles", "ModifiedBy", "nvarchar(Max)");
            AddFields("UserRoles", "ModifiedDate", "DATETIME");


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'RolesDocTypes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.RolesDocTypes
	                        (
	                        RolesDocTypeId  INT IDENTITY NOT NULL,
	                        RoleId          NVARCHAR (128),
	                        DocTypeId       INT,
                            MenuId          INT,
	                        ControllerName  NVARCHAR (max) NOT NULL,
	                        ActionName      NVARCHAR (max) NOT NULL,
	                        CreatedBy       NVARCHAR (max),
	                        ModifiedBy      NVARCHAR (max),
	                        CreatedDate     DATETIME NOT NULL,
	                        ModifiedDate    DATETIME NOT NULL,
	                        CONSTRAINT [PK_Web.RolesDocTypes] PRIMARY KEY (RolesDocTypeId),
	                        CONSTRAINT [FK_Web.RolesDocTypes_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                        CONSTRAINT [FK_Web.RolesDocTypes_Web.AspNetRoles_RoleId] FOREIGN KEY (RoleId) REFERENCES Web.AspNetRoles (Id)
	                        )

                        CREATE INDEX IX_RoleId
	                        ON Web.RolesDocTypes (RoleId)

                        CREATE INDEX IX_DocTypeId
	                        ON Web.RolesDocTypes (DocTypeId)

                        CREATE INDEX IX_MenuId
	                        ON Web.RolesDocTypes (MenuId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'RolesDocTypeProcesses'") == 0)
                {
                    mQry = @"CREATE TABLE Web.RolesDocTypeProcesses
	                        (
	                            RolesDocTypeProcessId INT IDENTITY NOT NULL,
	                            RoleId                NVARCHAR (128) NOT NULL,
	                            DocTypeId             INT NOT NULL,
	                            ProcessId             INT NOT NULL,
	                            CreatedBy             NVARCHAR (max),
	                            ModifiedBy            NVARCHAR (max),
	                            CreatedDate           DATETIME NOT NULL,
	                            ModifiedDate          DATETIME NOT NULL,
	                            CONSTRAINT [PK_Web.RolesDocTypeProcesses] PRIMARY KEY (RolesDocTypeProcessId),
	                            CONSTRAINT [FK_Web.RolesDocTypeProcesses_Web.Processes_ProcessId] FOREIGN KEY (ProcessId) REFERENCES Web.Processes (ProcessId),
	                            CONSTRAINT [FK_Web.RolesDocTypeProcesses_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                            CONSTRAINT [FK_Web.RolesDocTypeProcesses_Web.AspNetRoles_RoleId] FOREIGN KEY (RoleId) REFERENCES Web.AspNetRoles (Id)
	                        )

                        CREATE INDEX IX_RoleId
	                        ON Web.RolesDocTypeProcesses (RoleId)

                        CREATE INDEX IX_ProcessId
	                        ON Web.RolesDocTypeProcesses (ProcessId)

                        CREATE INDEX IX_DocTypeId
	                        ON Web.RolesDocTypeProcesses (DocTypeId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DocumentTypeProcesses'") == 0)
                {
                    mQry = @"CREATE TABLE Web.DocumentTypeProcesses
	                            (
	                                DocumentTypeProcessId INT IDENTITY NOT NULL,
	                                DocumentTypeId        INT NOT NULL,
	                                ProcessId             INT NOT NULL,
	                                CreatedBy             NVARCHAR (max),
	                                ModifiedBy            NVARCHAR (max),
	                                CreatedDate           DATETIME NOT NULL,
	                                ModifiedDate          DATETIME NOT NULL,
	                                OMSId                 NVARCHAR (50),
	                                CONSTRAINT [PK_Web.DocumentTypeProcesses] PRIMARY KEY (DocumentTypeProcessId),
	                                CONSTRAINT [FK_Web.DocumentTypeProcesses_Web.DocumentTypes_DocumentTypeId] FOREIGN KEY (DocumentTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                                CONSTRAINT [FK_Web.DocumentTypeProcesses_Web.Processes_ProcessId] FOREIGN KEY (ProcessId) REFERENCES Web.Processes (ProcessId)
	                            )

                            CREATE INDEX IX_DocumentTypeId
	                            ON Web.DocumentTypeProcesses (DocumentTypeId)

                            CREATE INDEX IX_ProcessId
	                            ON Web.DocumentTypeProcesses (ProcessId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            AddFields("ControllerActions", "DisplayName", "nvarchar(50)");
            AddFields("Menus", "DocumentCategoryId", "Int", "DocumentCategories");

            AddFields("SaleInvoiceSettings", "isVisibleGodown", "Bit");

            AddFields("JobInvoiceSettings", "filterDocTypeCostCenter", "nvarchar(Max)");

            AddFields("RoleDocTypes", "ProductTypeId", "Int","ProductTypes");
            AddFields("Menus", "ProductNatureId", "Int", "ProductNatures");

            AddFields("SaleInvoiceSettings", "IsAutoDocNo", "BIT NOT NULL DEFAULT(0)");

            AddFields("SaleInvoiceReturnLines", "SalesTaxGroupProductId", "Int", "ChargeGroupProduct");
            AddFields("SaleInvoiceReturnHeaders", "SalesTaxGroupPersonId", "Int", "ChargeGroupPerson");


            AddFields("SaleInvoiceSettings", "isVisibleProductUid_Index", "Bit");
            AddFields("SaleInvoiceSettings", "isVisibleProduct_Index", "Bit");
            AddFields("SaleInvoiceSettings", "isVisibleProductGroup_Index", "Bit");

            AddFields("JobInvoiceSettings", "isVisibleProductUid_Index", "Bit");
            AddFields("JobInvoiceSettings", "isVisibleProduct_Index", "Bit");
            AddFields("JobInvoiceSettings", "isVisibleProductGroup_Index", "Bit");

            AddFields("SaleDeliverySettings", "isVisibleProductUid_Index", "Bit");
            AddFields("SaleDeliverySettings", "isVisibleProduct_Index", "Bit");
            AddFields("SaleDeliverySettings", "isVisibleProductGroup_Index", "Bit");
            AddFields("SaleDeliverySettings", "isVisibleSaleInvoice_Index", "Bit");


            //Start Property Tax

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Castes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.Castes
	                        (
	                        CasteId      INT IDENTITY NOT NULL,
	                        CasteName    NVARCHAR (50) NOT NULL,
	                        IsActive     BIT DEFAULT ((1)) NOT NULL,
	                        CreatedBy    NVARCHAR (max),
	                        ModifiedBy   NVARCHAR (max),
	                        CreatedDate  DATETIME NOT NULL,
	                        ModifiedDate DATETIME NOT NULL,
	                        OMSId        NVARCHAR (50),
	                        Site_SiteId  INT,
	                        GateId       INT,
	                        DocTypeId    INT,
	                        CONSTRAINT [PK_Web.Castes] PRIMARY KEY (CasteId)
	                        WITH (FILLFACTOR = 90),
	                        CONSTRAINT [FK_Web.Castes_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId)
	                        )

                        CREATE UNIQUE INDEX IX_Caste_CasteName
	                        ON Web.Castes (CasteName)
	                        WITH (FILLFACTOR = 90) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ProductBuyerLogs'") == 0)
                {
                    mQry = @"CREATE TABLE Web.ProductBuyerLogs
	                        (
	                        ProductBuyerLogId   INT IDENTITY NOT NULL,
	                        ProductBuyerId      INT NOT NULL,
	                        ProductId           INT NOT NULL,
	                        BuyerId             INT NOT NULL,
	                        BuyerSku            NVARCHAR (50) NULL,
	                        BuyerProductCode    NVARCHAR (50) NULL,
	                        BuyerUpcCode        NVARCHAR (20) NULL,
	                        BuyerSpecification  NVARCHAR (50) NULL,
	                        BuyerSpecification1 NVARCHAR (50) NULL,
	                        BuyerSpecification2 NVARCHAR (50) NULL,
	                        BuyerSpecification3 NVARCHAR (50) NULL,
	                        BuyerSpecification4 NVARCHAR (50) NULL,
	                        BuyerSpecification5 NVARCHAR (50) NULL,
	                        BuyerSpecification6 NVARCHAR (50) NULL,
	                        DateOfConsutruction DATETIME NOT NULL,
	                        DiscountTypeId      INT NULL,
	                        PropertyArea        DECIMAL (18, 4) NULL,
	                        TaxableArea         DECIMAL (18, 4) NULL,
	                        ARV                 DECIMAL (18, 4) NULL,
	                        TenantName          NVARCHAR (max) NULL,
	                        BillingType         NVARCHAR (max) NULL,
	                        Description         NVARCHAR (max) NULL,
	                        CoveredArea         DECIMAL (18, 4) NULL,
	                        GarageArea          DECIMAL (18, 4) NULL,
	                        BalconyArea         DECIMAL (18, 4) NULL,
	                        IsRented            BIT NULL,
	                        WEF                 DATETIME NOT NULL,
	                        TaxAmount           DECIMAL (18, 4) NULL,
	                        TaxPercentage       DECIMAL (18, 4) NULL,
	                        ModifyRemark        NVARCHAR (max) NULL,
	                        CreatedBy           NVARCHAR (max) NULL,
	                        ModifiedBy          NVARCHAR (max) NULL,
	                        CreatedDate         DATETIME NOT NULL,
	                        ModifiedDate        DATETIME NOT NULL,
	                        OMSId               NVARCHAR (50) NULL,
	                        CONSTRAINT [PK_Web.ProductBuyerLogs] PRIMARY KEY (ProductBuyerLogId),
	                        CONSTRAINT [FK_Web.ProductBuyerLogs_Web.People_BuyerId] FOREIGN KEY (BuyerId) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.ProductBuyerLogs_Web.DiscountTypes_DiscountTypeId] FOREIGN KEY (DiscountTypeId) REFERENCES Web.DiscountTypes (DiscountTypeId),
	                        CONSTRAINT [FK_Web.ProductBuyerLogs_Web.Products_ProductId] FOREIGN KEY (ProductId) REFERENCES Web.Products (ProductId),
	                        CONSTRAINT [FK_Web.ProductBuyerLogs_Web.ProductBuyers_ProductBuyerId] FOREIGN KEY (ProductBuyerId) REFERENCES Web.ProductBuyers (ProductBuyerId)
	                        )

                        CREATE INDEX IX_ProductBuyerId
	                        ON Web.ProductBuyerLogs (ProductBuyerId)

                        CREATE INDEX IX_ProductId
	                        ON Web.ProductBuyerLogs (ProductId)

                        CREATE INDEX IX_BuyerId
	                        ON Web.ProductBuyerLogs (BuyerId)

                        CREATE INDEX IX_DiscountTypeId
	                        ON Web.ProductBuyerLogs (DiscountTypeId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DocumentTypeAttributes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.DocumentTypeAttributes
	                        (
	                        DocumentTypeAttributeId INT IDENTITY NOT NULL,
	                        Name                    NVARCHAR (max) NOT NULL,
	                        IsMandatory             BIT NOT NULL,
	                        DataType                NVARCHAR (max),
	                        ListItem                NVARCHAR (max),
	                        DefaultValue            NVARCHAR (max),
	                        IsActive                BIT NOT NULL,
	                        DocumentTypeId          INT NOT NULL,
	                        CreatedBy               NVARCHAR (max),
	                        ModifiedBy              NVARCHAR (max),
	                        CreatedDate             DATETIME NOT NULL,
	                        ModifiedDate            DATETIME NOT NULL,
	                        OMSId                   NVARCHAR (50),
	                        CONSTRAINT [PK_Web.DocumentTypeAttributes] PRIMARY KEY (DocumentTypeAttributeId)
	                        WITH (FILLFACTOR = 90),
	                        CONSTRAINT [FK_Web.DocumentTypeAttributes_Web.DocumentTypes_DocumentTypeId] FOREIGN KEY (DocumentTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId)
	                        )

                        CREATE INDEX IX_DocumentType_DocumentTypeId
	                        ON Web.DocumentTypeAttributes (DocumentTypeId)
	                        WITH (FILLFACTOR = 90) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DiscountTypes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.DiscountTypes
	                        (
	                        DiscountTypeId   INT IDENTITY NOT NULL,
	                        DiscountTypeName NVARCHAR (50) NOT NULL,
	                        Rate             DECIMAL (18, 4) NOT NULL,
	                        IsActive         BIT DEFAULT ((1)) NOT NULL,
	                        CreatedBy        NVARCHAR (max),
	                        ModifiedBy       NVARCHAR (max),
	                        CreatedDate      DATETIME NOT NULL,
	                        ModifiedDate     DATETIME NOT NULL,
	                        OMSId            NVARCHAR (50),
	                        Site_SiteId      INT,
	                        GateId           INT,
	                        DocTypeId        INT,
	                        CONSTRAINT [PK_Web.DiscountTypes] PRIMARY KEY (DiscountTypeId)
	                        WITH (FILLFACTOR = 90),
	                        CONSTRAINT [FK_Web.DiscountTypes_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId)
	                        )

                        CREATE UNIQUE INDEX IX_DiscountType_DiscountTypeName
	                        ON Web.DiscountTypes (DiscountTypeName)
	                        WITH (FILLFACTOR = 90) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ProductBuyerExtendeds'") == 0)
                {
                    mQry = @"CREATE TABLE Web.ProductBuyerExtendeds
	                        (
	                        ProductBuyerId      INT NOT NULL,
	                        DateOfConsutruction DATETIME NOT NULL,
	                        DiscountTypeId      INT,
	                        PropertyArea        DECIMAL (18, 4),
	                        TaxableArea         DECIMAL (18, 4),
	                        ARV                 DECIMAL (18, 4),
	                        TenantName          NVARCHAR (100),
	                        BillingType         NVARCHAR (20),
	                        CoveredArea         DECIMAL (18, 4),
	                        GarageArea          DECIMAL (18, 4),
	                        BalconyArea         DECIMAL (18, 4),
	                        IsRented            BIT,
	                        TaxAmount           DECIMAL (18, 4),
	                        TaxPercentage       DECIMAL (18, 4),
	                        WEF                 DATETIME NOT NULL,
	                        Description         NVARCHAR (50),
                            LedgerHeaderId      Int,
	                        CONSTRAINT [PK_Web.ProductBuyerExtendeds] PRIMARY KEY (ProductBuyerId)
	                        WITH (FILLFACTOR = 90),
	                        CONSTRAINT [FK_Web.ProductBuyerExtendeds_Web.ProductBuyers_ProductBuyerId] FOREIGN KEY (ProductBuyerId) REFERENCES Web.ProductBuyers (ProductBuyerId),
	                        CONSTRAINT [FK_Web.ProductBuyerExtendeds_Web.DiscountType_DiscountTypeId] FOREIGN KEY (DiscountTypeId) REFERENCES Web.DiscountTypes (DiscountTypeId)
	                        )

                        CREATE INDEX IX_ProductBuyerId
	                        ON Web.ProductBuyerExtendeds (ProductBuyerId)
	                        WITH (FILLFACTOR = 90)

                        CREATE INDEX IX_DiscountTypeId
	                        ON Web.ProductBuyerExtendeds (DiscountTypeId)
	                        WITH (FILLFACTOR = 90) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Religions'") == 0)
                {
                    mQry = @"CREATE TABLE Web.Religions
	                        (
	                        ReligionId   INT IDENTITY NOT NULL,
	                        ReligionName NVARCHAR (50) NOT NULL,
	                        IsActive     BIT DEFAULT ((1)) NOT NULL,
	                        CreatedBy    NVARCHAR (max),
	                        ModifiedBy   NVARCHAR (max),
	                        CreatedDate  DATETIME NOT NULL,
	                        ModifiedDate DATETIME NOT NULL,
	                        OMSId        NVARCHAR (50),
	                        Site_SiteId  INT,
	                        GateId       INT,
	                        DocTypeId    INT,
	                        CONSTRAINT [PK_Web.Religions] PRIMARY KEY (ReligionId)
	                        WITH (FILLFACTOR = 90),
	                        CONSTRAINT [FK_Web.Religions_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId)
	                        )

                        CREATE UNIQUE INDEX IX_Religion_ReligionName
	                        ON Web.Religions (ReligionName)
	                        WITH (FILLFACTOR = 90) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'PersonAttributes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.PersonAttributes
	                        (
	                        PersonAttributeId       INT IDENTITY NOT NULL,
	                        PersonId                INT NOT NULL,
	                        DocumentTypeAttributeId INT NOT NULL,
	                        PersonAttributeValue    NVARCHAR (max),
	                        CreatedBy               NVARCHAR (max),
	                        ModifiedBy              NVARCHAR (max),
	                        CreatedDate             DATETIME NOT NULL,
	                        ModifiedDate            DATETIME NOT NULL,
	                        OMSId                   NVARCHAR (50),
	                        CONSTRAINT [PK_Web.PersonAttributes] PRIMARY KEY (PersonAttributeId)
	                        WITH (FILLFACTOR = 90),
	                        CONSTRAINT [FK_Web.PersonAttributes_Web.Persons_PersonId] FOREIGN KEY (PersonId) REFERENCES Web.People (PersonID)
	                        )

                        CREATE INDEX IX_PersonId
	                        ON Web.PersonAttributes (PersonId)
	                        WITH (FILLFACTOR = 90) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'PersonExtendeds'") == 0)
                {
                    mQry = @"CREATE TABLE Web.PersonExtendeds
	                        (
	                        PersonId          INT NOT NULL,
	                        GISId             NVARCHAR (20),
	                        GodownId          INT,
	                        BinLocationId     INT,
	                        HouseNo           NVARCHAR (50),
	                        AreaId            INT,
	                        FatherName        NVARCHAR (100),
	                        CasteId           INT,
	                        ReligionId        INT,
	                        PersonRateGroupId INT,
	                        TotalPropertyArea DECIMAL (18, 4),
	                        TotalTaxableArea  DECIMAL (18, 4),
	                        TotalARV          DECIMAL (18, 4),
	                        TotalTax          DECIMAL (18, 4),
	                        AadharNo          NVARCHAR (50),
	                        OldHouseNo        NVARCHAR (50),
	                        CONSTRAINT [PK_Web.PersonExtendeds] PRIMARY KEY (PersonId)
	                        WITH (FILLFACTOR = 90),
	                        CONSTRAINT [FK_Web.PersonExtendeds_Web.People_PersonId] FOREIGN KEY (PersonId) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.PersonExtendeds_Web.Godown_GodownId] FOREIGN KEY (GodownId) REFERENCES Web.Godowns (GodownId),
	                        CONSTRAINT [FK_Web.PersonExtendeds_Web.BinLocation_BinLocationId] FOREIGN KEY (BinLocationId) REFERENCES Web.BinLocations (BinLocationId),
	                        CONSTRAINT [FK_Web.PersonExtendeds_Web.Area_AreaId] FOREIGN KEY (AreaId) REFERENCES Web.Areas (AreaId),
	                        CONSTRAINT [FK_Web.PersonExtendeds_Web.Caste_CasteId] FOREIGN KEY (CasteId) REFERENCES Web.Castes (CasteId),
	                        CONSTRAINT [FK_Web.PersonExtendeds_Web.Religion_ReligionId] FOREIGN KEY (ReligionId) REFERENCES Web.Religions (ReligionId),
	                        CONSTRAINT [FK_Web.PersonExtendeds_Web.PersonRateGroup_PersonRateGroupId] FOREIGN KEY (PersonRateGroupId) REFERENCES Web.PersonRateGroups (PersonRateGroupId)
	                        )

                        CREATE INDEX IX_PersonId
	                        ON Web.PersonExtendeds (PersonId)
	                        WITH (FILLFACTOR = 90)

                        CREATE INDEX IX_GodownId
	                        ON Web.PersonExtendeds (GodownId)
	                        WITH (FILLFACTOR = 90)

                        CREATE INDEX IX_BinLocationId
	                        ON Web.PersonExtendeds (BinLocationId)
	                        WITH (FILLFACTOR = 90)

                        CREATE INDEX IX_CasteId
	                        ON Web.PersonExtendeds (CasteId)
	                        WITH (FILLFACTOR = 90)

                        CREATE INDEX IX_ReligionId
	                        ON Web.PersonExtendeds (ReligionId)
	                        WITH (FILLFACTOR = 90)

                        CREATE INDEX IX_PersonRateGroupId
	                        ON Web.PersonExtendeds (PersonRateGroupId)
	                        WITH (FILLFACTOR = 90) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'PaymentModes'") == 0)
                {
                    mQry = @"CREATE TABLE Web.PaymentModes
	                        (
	                        PaymentModeId   INT IDENTITY NOT NULL,
	                        PaymentModeName NVARCHAR (50) NOT NULL,
	                        IsActive        BIT DEFAULT ((1)) NOT NULL,
	                        CreatedBy       NVARCHAR (max),
	                        ModifiedBy      NVARCHAR (max),
	                        CreatedDate     DATETIME NOT NULL,
	                        ModifiedDate    DATETIME NOT NULL,
	                        OMSId           NVARCHAR (50),
	                        DocTypeId       INT,
	                        CONSTRAINT [PK_Web.PaymentModes] PRIMARY KEY (PaymentModeId)
	                        WITH (FILLFACTOR = 90),
	                        CONSTRAINT [FK_Web.PaymentModes_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId)
	                        )

                        CREATE UNIQUE INDEX IX_PaymentMode_PaymentModeName
	                        ON Web.PaymentModes (PaymentModeName)
	                        WITH (FILLFACTOR = 90) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'PaymentModeLedgerAccounts'") == 0)
                {
                    mQry = @"CREATE TABLE Web.PaymentModeLedgerAccounts
	                            (
	                            PaymentModeLedgerAccountId INT IDENTITY NOT NULL,
	                            PaymentModeId              INT NOT NULL,
	                            LedgerAccountId            INT NOT NULL,
	                            SiteId                     INT NOT NULL,
	                            DivisionId                 INT NOT NULL,
	                            CreatedBy                  NVARCHAR (max),
	                            ModifiedBy                 NVARCHAR (max),
	                            CreatedDate                DATETIME NOT NULL,
	                            ModifiedDate               DATETIME NOT NULL,
	                            OMSId                      NVARCHAR (50),
	                            CONSTRAINT [PK_Web.PaymentModeLedgerAccounts] PRIMARY KEY (PaymentModeLedgerAccountId)
	                            WITH (FILLFACTOR = 90)
	                            )

                            CREATE UNIQUE INDEX IX_PaymentModeLedgerAccounts
	                            ON Web.PaymentModeLedgerAccounts (PaymentModeId, SiteId, DivisionId)
	                            WITH (FILLFACTOR = 90) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'CollectionSettings'") == 0)
                {
                    mQry = @"CREATE TABLE Web.CollectionSettings
	                        (
	                        CollectionSettingsId              INT IDENTITY NOT NULL,
	                        DocTypeId                         INT NOT NULL,
	                        IsVisibleIntrestBalance           BIT,
	                        IsVisibleArearBalance             BIT,
	                        IsVisibleExcessBalance            BIT,
	                        IsVisibleCurrentYearBalance       BIT,
	                        IsVisibleNetOutstanding           BIT,
	                        IsVisibleReason                   BIT,
	                        CreatedBy                         NVARCHAR (max),
	                        ModifiedBy                        NVARCHAR (max),
	                        CreatedDate                       DATETIME NOT NULL,
	                        ModifiedDate                      DATETIME NOT NULL,
	                        DocumentPrint                     NVARCHAR (100),
	                        SqlProcDocumentPrint              NVARCHAR (100),
	                        SqlProcDocumentPrint_AfterSubmit  NVARCHAR (100),
	                        SqlProcDocumentPrint_AfterApprove NVARCHAR (100),
	                        CONSTRAINT [FK_Web.CollectionSettings_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId)
	                        ) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Dimension1Extended'") == 0)
                {
                    mQry = @"CREATE TABLE Web.Dimension1Extended
	                        (
	                        Dimension1Id INT NOT NULL,
	                        Multiplier   DECIMAL (18, 4),
	                        CostCenterId INT,
	                        PRIMARY KEY (Dimension1Id) WITH (FILLFACTOR = 90),
	                        CONSTRAINT [FK__Dimension__Dimen__7C3BFE99] FOREIGN KEY (Dimension1Id) REFERENCES Web.Dimension1 (Dimension1Id),
	                        CONSTRAINT [FK__Dimension__CostC__7D3022D2] FOREIGN KEY (CostCenterId) REFERENCES Web.CostCenters (CostCenterId)
	                        ) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            AddFields("Areas", "DocTypeId", "Int","DocumentTypes");

            AddFields("LedgerLines", "PaymentModeId", "Int", "PaymentModes");
            AddFields("LedgerLines", "AgentId", "Int", "People");
            AddFields("LedgerLines", "ReferenceLedgerAccountId", "Int", "LedgerAccounts");
            AddFields("LedgerLines", "DiscountAmount", "Decimal(18,4)");

            AddFields("People", "ReviewBy", "nvarchar(Max)");
            AddFields("People", "ReviewCount", "Int");
            AddFields("People", "Status", "Int");

            AddFields("ProductBuyerExtendeds", "WaterTaxAmount", "Decimal(18,4)");
            AddFields("ProductBuyerExtendeds", "WaterTaxPercentage", "Decimal(18,4)");


            AddFields("PersonExtendeds", "TotalWaterTax", "Decimal(18,4)");


            //End Start Property Tax


            AddFields("PersonSettings", "CalculationId", "Int", "Calculations");
            AddFields("LedgerSettings", "CalculationId", "Int", "Calculations");

            AddFields("Employee", "EmployeeId", "INT IDENTITY NOT NULL");
            AddFields("Employees", "BasicSalary", "Decimal(18,4)");


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EmployeeCharges'") == 0)
                {
                    mQry = @"CREATE TABLE Web.EmployeeCharges
	                        (
	                        Id                         INT IDENTITY NOT NULL,
	                        HeaderTableId              INT NOT NULL,
	                        Sr                         INT NOT NULL,
	                        ChargeId                   INT NOT NULL,
	                        AddDeduct                  TINYINT,
	                        AffectCost                 BIT NOT NULL,
	                        ChargeTypeId               INT,
	                        ProductChargeId            INT,
	                        CalculateOnId              INT,
	                        PersonID                   INT,
	                        LedgerAccountDrId          INT,
	                        LedgerAccountCrId          INT,
	                        ContraLedgerAccountId      INT,
	                        CostCenterId               INT,
	                        RateType                   TINYINT NOT NULL,
	                        IncludedInBase             BIT NOT NULL,
	                        ParentChargeId             INT,
	                        Rate                       DECIMAL (18, 4),
	                        Amount                     DECIMAL (18, 4),
	                        IsVisible                  BIT NOT NULL,
	                        IncludedCharges            NVARCHAR (max),
	                        IncludedChargesCalculation NVARCHAR (max),
	                        OMSId                      NVARCHAR (50),
	                        CONSTRAINT [PK_Web.EmployeeCharges] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.EmployeeCharges_Web.Charges_CalculateOnId] FOREIGN KEY (CalculateOnId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.EmployeeCharges_Web.Charges_ChargeId] FOREIGN KEY (ChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.EmployeeCharges_Web.ChargeTypes_ChargeTypeId] FOREIGN KEY (ChargeTypeId) REFERENCES Web.ChargeTypes (ChargeTypeId),
	                        CONSTRAINT [FK_Web.EmployeeCharges_Web.LedgerAccounts_ContraLedgerAccountId] FOREIGN KEY (ContraLedgerAccountId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.EmployeeCharges_Web.CostCenters_CostCenterId] FOREIGN KEY (CostCenterId) REFERENCES Web.CostCenters (CostCenterId),
	                        CONSTRAINT [FK_Web.EmployeeCharges_Web.Employees_EmployeeId] FOREIGN KEY (HeaderTableId) REFERENCES Web.Employees (EmployeeId),
	                        CONSTRAINT [FK_Web.EmployeeCharges_Web.LedgerAccounts_LedgerAccountCrId] FOREIGN KEY (LedgerAccountCrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.EmployeeCharges_Web.LedgerAccounts_LedgerAccountDrId] FOREIGN KEY (LedgerAccountDrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.EmployeeCharges_Web.Charges_ParentChargeId] FOREIGN KEY (ParentChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.EmployeeCharges_Web.People_PersonID] FOREIGN KEY (PersonID) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.EmployeeCharges_Web.Charges_ProductChargeId] FOREIGN KEY (ProductChargeId) REFERENCES Web.Charges (ChargeId)
	                        )

                        CREATE INDEX IX_HeaderTableId
	                        ON Web.EmployeeCharges (HeaderTableId)

                        CREATE INDEX IX_ChargeId
	                        ON Web.EmployeeCharges (ChargeId)

                        CREATE INDEX IX_ChargeTypeId
	                        ON Web.EmployeeCharges (ChargeTypeId)

                        CREATE INDEX IX_ProductChargeId
	                        ON Web.EmployeeCharges (ProductChargeId)

                        CREATE INDEX IX_CalculateOnId
	                        ON Web.EmployeeCharges (CalculateOnId)

                        CREATE INDEX IX_PersonID
	                        ON Web.EmployeeCharges (PersonID)

                        CREATE INDEX IX_LedgerAccountDrId
	                        ON Web.EmployeeCharges (LedgerAccountDrId)

                        CREATE INDEX IX_LedgerAccountCrId
	                        ON Web.EmployeeCharges (LedgerAccountCrId)

                        CREATE INDEX IX_ContraLedgerAccountId
	                        ON Web.EmployeeCharges (ContraLedgerAccountId)

                        CREATE INDEX IX_CostCenterId
	                        ON Web.EmployeeCharges (CostCenterId)

                        CREATE INDEX IX_ParentChargeId
	                        ON Web.EmployeeCharges (ParentChargeId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EmployeeLineCharges'") == 0)
                {
                    mQry = @"CREATE TABLE Web.EmployeeLineCharges
	                        (
	                        Id                            INT IDENTITY NOT NULL,
	                        LineTableId                   INT NOT NULL,
	                        HeaderTableId                 INT NOT NULL,
	                        Sr                            INT NOT NULL,
	                        ChargeId                      INT NOT NULL,
	                        AddDeduct                     TINYINT,
	                        AffectCost                    BIT NOT NULL,
	                        ChargeTypeId                  INT,
	                        CalculateOnId                 INT,
	                        PersonID                      INT,
	                        LedgerAccountDrId             INT,
	                        LedgerAccountCrId             INT,
	                        ContraLedgerAccountId         INT,
	                        CostCenterId                  INT,
	                        RateType                      TINYINT NOT NULL,
	                        IncludedInBase                BIT NOT NULL,
	                        ParentChargeId                INT,
	                        Rate                          DECIMAL (18, 4),
	                        Amount                        DECIMAL (18, 4),
	                        DealQty                       DECIMAL (18, 4),
	                        IsVisible                     BIT NOT NULL,
	                        IncludedCharges               NVARCHAR (max),
	                        IncludedChargesCalculation    NVARCHAR (max),
	                        IsVisibleLedgerAccountDr      BIT,
	                        filterLedgerAccountGroupsDrId INT,
	                        IsVisibleLedgerAccountCr      BIT,
	                        filterLedgerAccountGroupsCrId INT,
	                        OMSId                         NVARCHAR (50),
	                        CONSTRAINT [PK_Web.EmployeeLineCharges] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.Charges_CalculateOnId] FOREIGN KEY (CalculateOnId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.Charges_ChargeId] FOREIGN KEY (ChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.ChargeTypes_ChargeTypeId] FOREIGN KEY (ChargeTypeId) REFERENCES Web.ChargeTypes (ChargeTypeId),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.LedgerAccounts_ContraLedgerAccountId] FOREIGN KEY (ContraLedgerAccountId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.CostCenters_CostCenterId] FOREIGN KEY (CostCenterId) REFERENCES Web.CostCenters (CostCenterId),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.Employees_LineTableId] FOREIGN KEY (LineTableId) REFERENCES Web.Employees (EmployeeId),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.LedgerAccountGroups_filterLedgerAccountGroupsCrId] FOREIGN KEY (filterLedgerAccountGroupsCrId) REFERENCES Web.LedgerAccountGroups (LedgerAccountGroupId),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.LedgerAccountGroups_filterLedgerAccountGroupsDrId] FOREIGN KEY (filterLedgerAccountGroupsDrId) REFERENCES Web.LedgerAccountGroups (LedgerAccountGroupId),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.LedgerAccounts_LedgerAccountCrId] FOREIGN KEY (LedgerAccountCrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.LedgerAccounts_LedgerAccountDrId] FOREIGN KEY (LedgerAccountDrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.Charges_ParentChargeId] FOREIGN KEY (ParentChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.EmployeeLineCharges_Web.People_PersonID] FOREIGN KEY (PersonID) REFERENCES Web.People (PersonID)
	                        )

                        CREATE INDEX IX_LineTableId
	                        ON Web.EmployeeLineCharges (LineTableId)

                        CREATE INDEX IX_ChargeId
	                        ON Web.EmployeeLineCharges (ChargeId)

                        CREATE INDEX IX_ChargeTypeId
	                        ON Web.EmployeeLineCharges (ChargeTypeId)

                        CREATE INDEX IX_CalculateOnId
	                        ON Web.EmployeeLineCharges (CalculateOnId)

                        CREATE INDEX IX_PersonID
	                        ON Web.EmployeeLineCharges (PersonID)

                        CREATE INDEX IX_LedgerAccountDrId
	                        ON Web.EmployeeLineCharges (LedgerAccountDrId)

                        CREATE INDEX IX_LedgerAccountCrId
	                        ON Web.EmployeeLineCharges (LedgerAccountCrId)

                        CREATE INDEX IX_ContraLedgerAccountId
	                        ON Web.EmployeeLineCharges (ContraLedgerAccountId)

                        CREATE INDEX IX_CostCenterId
	                        ON Web.EmployeeLineCharges (CostCenterId)

                        CREATE INDEX IX_ParentChargeId
	                        ON Web.EmployeeLineCharges (ParentChargeId)

                        CREATE INDEX IX_filterLedgerAccountGroupsDrId
	                        ON Web.EmployeeLineCharges (filterLedgerAccountGroupsDrId)

                        CREATE INDEX IX_filterLedgerAccountGroupsCrId
	                        ON Web.EmployeeLineCharges (filterLedgerAccountGroupsCrId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }








            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SalaryHeaders'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SalaryHeaders
	                        (
	                        SalaryHeaderId INT IDENTITY NOT NULL,
	                        DocTypeId      INT NOT NULL,
	                        DocDate        DATETIME NOT NULL,
	                        DocNo          NVARCHAR (20),
	                        DivisionId     INT NOT NULL,
	                        SiteId         INT NOT NULL,
	                        Remark         NVARCHAR (max),
	                        Status         INT NOT NULL,
	                        ReviewCount    INT,
	                        ReviewBy       NVARCHAR (max),
	                        CreatedBy      NVARCHAR (max),
	                        CreatedDate    DATETIME NOT NULL,
	                        ModifiedBy     NVARCHAR (max),
	                        ModifiedDate   DATETIME NOT NULL,
	                        LockReason     NVARCHAR (max),
	                        OMSId          NVARCHAR (50),
	                        CONSTRAINT [PK_Web.SalaryHeaders] PRIMARY KEY (SalaryHeaderId),
	                        CONSTRAINT [FK_Web.SalaryHeaders_Web.Divisions_DivisionId] FOREIGN KEY (DivisionId) REFERENCES Web.Divisions (DivisionId),
	                        CONSTRAINT [FK_Web.SalaryHeaders_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                        CONSTRAINT [FK_Web.SalaryHeaders_Web.Sites_SiteId] FOREIGN KEY (SiteId) REFERENCES Web.Sites (SiteId)
	                        )

                        CREATE INDEX IX_DocTypeId
	                        ON Web.SalaryHeaders (DocTypeId)

                        CREATE INDEX IX_DivisionId
	                        ON Web.SalaryHeaders (DivisionId)

                        CREATE INDEX IX_SiteId
	                        ON Web.SalaryHeaders (SiteId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SalaryLines'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SalaryLines
	                        (
	                        SalaryLineId   INT IDENTITY NOT NULL,
	                        SalaryHeaderId INT NOT NULL,
	                        EmployeeId     INT NOT NULL,
	                        Days           DECIMAL (18, 4) NOT NULL,
	                        OtherAddition  DECIMAL (18, 4),
	                        OtherDeduction DECIMAL (18, 4),
	                        LoadEMI        DECIMAL (18, 4),
                            Advance        DECIMAL (18, 4),
	                        Remark         NVARCHAR (max),
	                        Sr             INT,
	                        CreatedBy      NVARCHAR (max),
	                        ModifiedBy     NVARCHAR (max),
	                        CreatedDate    DATETIME NOT NULL,
	                        ModifiedDate   DATETIME NOT NULL,
	                        LockReason     NVARCHAR (max),
	                        OMSId          NVARCHAR (50),
	                        CONSTRAINT [PK_Web.SalaryLines] PRIMARY KEY (SalaryLineId),
	                        CONSTRAINT [FK_Web.SalaryLines_Web.Employees_EmployeeId] FOREIGN KEY (EmployeeId) REFERENCES Web.Employees (EmployeeId),
	                        CONSTRAINT [FK_Web.SalaryLines_Web.SalaryHeaders_SalaryHeaderId] FOREIGN KEY (SalaryHeaderId) REFERENCES Web.SalaryHeaders (SalaryHeaderId)
	                        )

                        CREATE UNIQUE INDEX IX_SalaryLine_Unique
	                        ON Web.SalaryLines (SalaryHeaderId, EmployeeId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SalaryHeaderCharges'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SalaryHeaderCharges
	                        (
	                        Id                         INT IDENTITY NOT NULL,
	                        HeaderTableId              INT NOT NULL,
	                        Sr                         INT NOT NULL,
	                        ChargeId                   INT NOT NULL,
	                        AddDeduct                  TINYINT,
	                        AffectCost                 BIT NOT NULL,
	                        ChargeTypeId               INT,
	                        ProductChargeId            INT,
	                        CalculateOnId              INT,
	                        PersonID                   INT,
	                        LedgerAccountDrId          INT,
	                        LedgerAccountCrId          INT,
	                        ContraLedgerAccountId      INT,
	                        CostCenterId               INT,
	                        RateType                   TINYINT NOT NULL,
	                        IncludedInBase             BIT NOT NULL,
	                        ParentChargeId             INT,
	                        Rate                       DECIMAL (18, 4),
	                        Amount                     DECIMAL (18, 4),
	                        IsVisible                  BIT NOT NULL,
	                        IncludedCharges            NVARCHAR (max),
	                        IncludedChargesCalculation NVARCHAR (max),
	                        OMSId                      NVARCHAR (50),
	                        CONSTRAINT [PK_Web.SalaryHeaderCharges] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.SalaryHeaderCharges_Web.Charges_CalculateOnId] FOREIGN KEY (CalculateOnId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SalaryHeaderCharges_Web.Charges_ChargeId] FOREIGN KEY (ChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SalaryHeaderCharges_Web.ChargeTypes_ChargeTypeId] FOREIGN KEY (ChargeTypeId) REFERENCES Web.ChargeTypes (ChargeTypeId),
	                        CONSTRAINT [FK_Web.SalaryHeaderCharges_Web.LedgerAccounts_ContraLedgerAccountId] FOREIGN KEY (ContraLedgerAccountId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SalaryHeaderCharges_Web.CostCenters_CostCenterId] FOREIGN KEY (CostCenterId) REFERENCES Web.CostCenters (CostCenterId),
	                        CONSTRAINT [FK_Web.SalaryHeaderCharges_Web.LedgerAccounts_LedgerAccountCrId] FOREIGN KEY (LedgerAccountCrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SalaryHeaderCharges_Web.LedgerAccounts_LedgerAccountDrId] FOREIGN KEY (LedgerAccountDrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SalaryHeaderCharges_Web.Charges_ParentChargeId] FOREIGN KEY (ParentChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SalaryHeaderCharges_Web.People_PersonID] FOREIGN KEY (PersonID) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.SalaryHeaderCharges_Web.Charges_ProductChargeId] FOREIGN KEY (ProductChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SalaryHeaderCharges_Web.SalaryHeaders_HeaderTableId] FOREIGN KEY (HeaderTableId) REFERENCES Web.SalaryHeaders (SalaryHeaderId)
	                        )

                        CREATE INDEX IX_HeaderTableId
	                        ON Web.SalaryHeaderCharges (HeaderTableId)

                        CREATE INDEX IX_ChargeId
	                        ON Web.SalaryHeaderCharges (ChargeId)

                        CREATE INDEX IX_ChargeTypeId
	                        ON Web.SalaryHeaderCharges (ChargeTypeId)

                        CREATE INDEX IX_ProductChargeId
	                        ON Web.SalaryHeaderCharges (ProductChargeId)

                        CREATE INDEX IX_CalculateOnId
	                        ON Web.SalaryHeaderCharges (CalculateOnId)

                        CREATE INDEX IX_PersonID
	                        ON Web.SalaryHeaderCharges (PersonID)

                        CREATE INDEX IX_LedgerAccountDrId
	                        ON Web.SalaryHeaderCharges (LedgerAccountDrId)

                        CREATE INDEX IX_LedgerAccountCrId
	                        ON Web.SalaryHeaderCharges (LedgerAccountCrId)

                        CREATE INDEX IX_ContraLedgerAccountId
	                        ON Web.SalaryHeaderCharges (ContraLedgerAccountId)

                        CREATE INDEX IX_CostCenterId
	                        ON Web.SalaryHeaderCharges (CostCenterId)

                        CREATE INDEX IX_ParentChargeId
	                        ON Web.SalaryHeaderCharges (ParentChargeId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }




            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SalaryLineCharges'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SalaryLineCharges
	                        (
	                        Id                            INT IDENTITY NOT NULL,
	                        LineTableId                   INT NOT NULL,
	                        HeaderTableId                 INT NOT NULL,
	                        Sr                            INT NOT NULL,
	                        ChargeId                      INT NOT NULL,
	                        AddDeduct                     TINYINT,
	                        AffectCost                    BIT NOT NULL,
	                        ChargeTypeId                  INT,
	                        CalculateOnId                 INT,
	                        PersonID                      INT,
	                        LedgerAccountDrId             INT,
	                        LedgerAccountCrId             INT,
	                        ContraLedgerAccountId         INT,
	                        CostCenterId                  INT,
	                        RateType                      TINYINT NOT NULL,
	                        IncludedInBase                BIT NOT NULL,
	                        ParentChargeId                INT,
	                        Rate                          DECIMAL (18, 4),
	                        Amount                        DECIMAL (18, 4),
	                        DealQty                       DECIMAL (18, 4),
	                        IsVisible                     BIT NOT NULL,
	                        IncludedCharges               NVARCHAR (max),
	                        IncludedChargesCalculation    NVARCHAR (max),
	                        IsVisibleLedgerAccountDr      BIT,
	                        filterLedgerAccountGroupsDrId INT,
	                        IsVisibleLedgerAccountCr      BIT,
	                        filterLedgerAccountGroupsCrId INT,
	                        OMSId                         NVARCHAR (50),
	                        CONSTRAINT [PK_Web.SalaryLineCharges] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.Charges_CalculateOnId] FOREIGN KEY (CalculateOnId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.Charges_ChargeId] FOREIGN KEY (ChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.ChargeTypes_ChargeTypeId] FOREIGN KEY (ChargeTypeId) REFERENCES Web.ChargeTypes (ChargeTypeId),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.LedgerAccounts_ContraLedgerAccountId] FOREIGN KEY (ContraLedgerAccountId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.CostCenters_CostCenterId] FOREIGN KEY (CostCenterId) REFERENCES Web.CostCenters (CostCenterId),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.LedgerAccountGroups_filterLedgerAccountGroupsCrId] FOREIGN KEY (filterLedgerAccountGroupsCrId) REFERENCES Web.LedgerAccountGroups (LedgerAccountGroupId),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.LedgerAccountGroups_filterLedgerAccountGroupsDrId] FOREIGN KEY (filterLedgerAccountGroupsDrId) REFERENCES Web.LedgerAccountGroups (LedgerAccountGroupId),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.LedgerAccounts_LedgerAccountCrId] FOREIGN KEY (LedgerAccountCrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.LedgerAccounts_LedgerAccountDrId] FOREIGN KEY (LedgerAccountDrId) REFERENCES Web.LedgerAccounts (LedgerAccountId),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.Charges_ParentChargeId] FOREIGN KEY (ParentChargeId) REFERENCES Web.Charges (ChargeId),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.People_PersonID] FOREIGN KEY (PersonID) REFERENCES Web.People (PersonID),
	                        CONSTRAINT [FK_Web.SalaryLineCharges_Web.SalaryLines_LineTableId] FOREIGN KEY (LineTableId) REFERENCES Web.SalaryLines (SalaryLineId)
	                        )

                        CREATE INDEX IX_LineTableId
	                        ON Web.SalaryLineCharges (LineTableId)

                        CREATE INDEX IX_ChargeId
	                        ON Web.SalaryLineCharges (ChargeId)

                        CREATE INDEX IX_ChargeTypeId
	                        ON Web.SalaryLineCharges (ChargeTypeId)

                        CREATE INDEX IX_CalculateOnId
	                        ON Web.SalaryLineCharges (CalculateOnId)

                        CREATE INDEX IX_PersonID
	                        ON Web.SalaryLineCharges (PersonID)

                        CREATE INDEX IX_LedgerAccountDrId
	                        ON Web.SalaryLineCharges (LedgerAccountDrId)

                        CREATE INDEX IX_LedgerAccountCrId
	                        ON Web.SalaryLineCharges (LedgerAccountCrId)

                        CREATE INDEX IX_ContraLedgerAccountId
	                        ON Web.SalaryLineCharges (ContraLedgerAccountId)

                        CREATE INDEX IX_CostCenterId
	                        ON Web.SalaryLineCharges (CostCenterId)

                        CREATE INDEX IX_ParentChargeId
	                        ON Web.SalaryLineCharges (ParentChargeId)

                        CREATE INDEX IX_filterLedgerAccountGroupsDrId
	                        ON Web.SalaryLineCharges (filterLedgerAccountGroupsDrId)

                        CREATE INDEX IX_filterLedgerAccountGroupsCrId
	                        ON Web.SalaryLineCharges (filterLedgerAccountGroupsCrId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SalarySettings'") == 0)
                {
                    mQry = @"CREATE TABLE Web.SalarySettings
	                        (
	                        SalarySettingsId                  INT IDENTITY NOT NULL,
	                        DocTypeId                         INT NOT NULL,
	                        SiteId                            INT NOT NULL,
	                        DivisionId                        INT NOT NULL,
	                        filterContraSites                 NVARCHAR (max) NULL,
	                        filterContraDivisions             NVARCHAR (max) NULL,
	                        DocumentPrint                     NVARCHAR (100) NULL,
	                        NoOfPrintCopies                   INT NULL,
	                        SqlProcDocumentPrint              NVARCHAR (100) NULL,
	                        SqlProcDocumentPrint_AfterSubmit  NVARCHAR (100) NULL,
	                        SqlProcDocumentPrint_AfterApprove NVARCHAR (100) NULL,
	                        ImportMenuId                      INT NULL,
	                        WizardMenuId                      INT NULL,
	                        ExportMenuId                      INT NULL,
	                        CalculationId                     INT NULL,
	                        DocumentPrintReportHeaderId       INT NULL,
	                        CreatedBy                         NVARCHAR (max) NULL,
	                        ModifiedBy                        NVARCHAR (max) NULL,
	                        CreatedDate                       DATETIME NOT NULL,
	                        ModifiedDate                      DATETIME NOT NULL,
	                        OMSId                             NVARCHAR (50) NULL,
	                        CONSTRAINT [PK_Web.SalarySettings] PRIMARY KEY (SalarySettingsId),
	                        CONSTRAINT [FK_Web.SalarySettings_Web.Calculations_CalculationId] FOREIGN KEY (CalculationId) REFERENCES Web.Calculations (CalculationId),
	                        CONSTRAINT [FK_Web.SalarySettings_Web.Divisions_DivisionId] FOREIGN KEY (DivisionId) REFERENCES Web.Divisions (DivisionId),
	                        CONSTRAINT [FK_Web.SalarySettings_Web.DocumentTypes_DocTypeId] FOREIGN KEY (DocTypeId) REFERENCES Web.DocumentTypes (DocumentTypeId),
	                        CONSTRAINT [FK_Web.SalarySettings_Web.ReportHeaders_DocumentPrintReportHeaderId] FOREIGN KEY (DocumentPrintReportHeaderId) REFERENCES Web.ReportHeaders (ReportHeaderId),
	                        CONSTRAINT [FK_Web.SalarySettings_Web.Menus_ExportMenuId] FOREIGN KEY (ExportMenuId) REFERENCES Web.Menus (MenuId),
	                        CONSTRAINT [FK_Web.SalarySettings_Web.Menus_ImportMenuId] FOREIGN KEY (ImportMenuId) REFERENCES Web.Menus (MenuId),
	                        CONSTRAINT [FK_Web.SalarySettings_Web.Sites_SiteId] FOREIGN KEY (SiteId) REFERENCES Web.Sites (SiteId),
	                        CONSTRAINT [FK_Web.SalarySettings_Web.Menus_WizardMenuId] FOREIGN KEY (WizardMenuId) REFERENCES Web.Menus (MenuId)
	                        )

                        CREATE INDEX IX_DocTypeId
	                        ON Web.SalarySettings (DocTypeId)

                        CREATE INDEX IX_SiteId
	                        ON Web.SalarySettings (SiteId)

                        CREATE INDEX IX_DivisionId
	                        ON Web.SalarySettings (DivisionId)

                        CREATE INDEX IX_ImportMenuId
	                        ON Web.SalarySettings (ImportMenuId)

                        CREATE INDEX IX_WizardMenuId
	                        ON Web.SalarySettings (WizardMenuId)

                        CREATE INDEX IX_ExportMenuId
	                        ON Web.SalarySettings (ExportMenuId)

                        CREATE INDEX IX_CalculationId
	                        ON Web.SalarySettings (CalculationId)

                        CREATE INDEX IX_DocumentPrintReportHeaderId
	                        ON Web.SalarySettings (DocumentPrintReportHeaderId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            AddFields("Employees", "DateOfJoining", "DATETIME");
            AddFields("Employees", "DateOfRelieving", "DATETIME");

            AddFields("Employees", "WagesPayType", "nvarchar(10)");
            AddFields("Employees", "PaymentType", "nvarchar(10)");

            AddFields("SalaryLines", "Advance", "Decimal(18,4)");
            AddFields("SalaryLines", "NetPayable", "DECIMAL(18,4) NOT NULL DEFAULT((0))");

            AddFields("SalaryHeaders", "LedgerHeaderId", "Int","LedgerHeaders");


            AddFields("ReportHeaders", "IsHideHeaderDetail", "BIT");

            AddFields("JobInvoiceSettings", "JobInvoiceReturnDocTypeId", "Int","DocumentTypes");

            AddFields("Narrations", "DocTypeId", "Int","DocumentTypes");

            AddFields("LedgerLines", "SupplementaryForLedgerId", "Int", "Ledgers");


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'LedgerSupplementaries'") == 0)
                {
                    mQry = @"CREATE TABLE Web.LedgerSupplementaries
	                        (
	                        Id                    INT IDENTITY NOT NULL,
	                        LedgerId              INT NOT NULL,
	                        SupplementaryLedgerId INT NOT NULL,
	                        Amount                DECIMAL (18, 4) NOT NULL,
	                        CreatedBy             NVARCHAR (max) NULL,
	                        ModifiedBy            NVARCHAR (max) NULL,
	                        CreatedDate           DATETIME NOT NULL,
	                        ModifiedDate          DATETIME NOT NULL,
	                        OMSId                 NVARCHAR (50) NULL,
	                        CONSTRAINT [PK_Web.LedgerSupplementaries] PRIMARY KEY (Id),
	                        CONSTRAINT [FK_Web.LedgerSupplementaries_Web.Ledgers_LedgerId] FOREIGN KEY (LedgerId) REFERENCES Web.Ledgers (LedgerId),
	                        CONSTRAINT [FK_Web.LedgerSupplementaries_Web.Ledgers_SupplementaryLedgerId] FOREIGN KEY (SupplementaryLedgerId) REFERENCES Web.Ledgers (LedgerId)
	                        )

                        CREATE INDEX IX_LedgerId
	                        ON Web.LedgerSupplementaries (LedgerId)

                        CREATE INDEX IX_SupplementaryLedgerId
	                        ON Web.LedgerSupplementaries (SupplementaryLedgerId)
                         ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            ReCreateProcedures();
            DataCorrection();

            return RedirectToAction("Module", "Menu");
        }

        public void AddFields(string TableName, string FieldName, string DataType, string ForeignKeyTable = null)
        {
            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Columns WHERE COLUMN_NAME =  '" + FieldName + "' AND TABLE_NAME = '" + TableName + "'") == 0)
                {
                    mQry = "ALTER TABLE Web." + TableName + " ADD " + FieldName + " " + DataType;
                    ExecuteQuery(mQry);

                    if (ForeignKeyTable != "" && ForeignKeyTable != null)
                    {
                        string ForeignKeyTablePrimaryField = "";
                        mQry = " SELECT column_name " +
                                " FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC " +
                                " INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME " +
                                " and ku.table_name= '"+ ForeignKeyTable +"' " +
                                " ORDER BY KU.TABLE_NAME, KU.ORDINAL_POSITION ";
                        ForeignKeyTablePrimaryField = (string)ExecuteScaler(mQry);

                        mQry = " ALTER TABLE Web." + TableName + "  ADD CONSTRAINT [FK_Web." + TableName + "_Web." + ForeignKeyTable + "_" + FieldName + "] FOREIGN KEY (" + FieldName + ") REFERENCES Web." + ForeignKeyTable + "(" + ForeignKeyTablePrimaryField + ")";
                        ExecuteQuery(mQry);
                    }
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }
        }








        public void DropFields(string TableName, string FieldName)
        {
            string Foreign_Key_Name = "";
            string Index_Name = "";
            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Columns WHERE COLUMN_NAME =  '" + FieldName + "' AND TABLE_NAME = '" + TableName + "'") != 0)
                {
                    mQry = @"SELECT fk.name AS FK_name
                                FROM sys.objects o1
                                INNER JOIN sys.foreign_keys fk ON o1.object_id = fk.parent_object_id
                                INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                                INNER JOIN sys.columns c1 ON fkc.parent_object_id = c1.object_id
                                    AND fkc.parent_column_id = c1.column_id
                                WHERE o1.name = '" + TableName + "' AND c1.name = '" + FieldName + "'";
                    Foreign_Key_Name = (string)ExecuteScaler(mQry);
                    if (Foreign_Key_Name != null && Foreign_Key_Name != "")
                    {
                        mQry = "ALTER TABLE Web." + TableName + " DROP CONSTRAINT " + "[" + Foreign_Key_Name + "]";
                        ExecuteQuery(mQry);
                    }


                    mQry = @"SELECT ind.name
                            FROM sys.indexes ind 
                            INNER JOIN sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
                            INNER JOIN sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
                            INNER JOIN sys.tables t ON ind.object_id = t.object_id 
                            WHERE ind.is_primary_key = 0 
                            AND ind.is_unique = 0 
                            AND ind.is_unique_constraint = 0 
                            AND t.is_ms_shipped = 0 
                            AND t.name =  '" + TableName + "' AND col.name = '" + FieldName + "'";
                    Index_Name = (string)ExecuteScaler(mQry);
                    if (Index_Name != null && Index_Name != "")
                    {
                        mQry = "DROP Index Web." + TableName + "." + "[" + Index_Name + "]";
                        ExecuteQuery(mQry);
                    }


                    mQry = "ALTER TABLE Web." + TableName + " DROP COLUMN " + FieldName + " ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }
        }


        public void RenameFields(string TableName, string OldFieldName, string NewFieldName)
        {
            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Columns WHERE COLUMN_NAME =  '" + OldFieldName + "' AND TABLE_NAME = '" + TableName + "'") > 0)
                {
                    mQry = "EXEC sp_rename 'Web." + TableName + "." + OldFieldName + "', '" + NewFieldName + "', 'COLUMN'";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }
        }
       

        public void RecordError(Exception ex)
        {
            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


            mQry = @"INSERT INTO Web.ActivityLogs (DocId, ActivityType, Narration, UserRemark, CreatedBy, CreatedDate, DocStatus, SiteId, DivisionId)
                    SELECT 0 AS DocId, 1 AS ActivityType, 'Update Table Structure' AS Narration, '" + ex.Message.Replace("'", "") + "' AS UserRemark, 'System' AS CreatedBy, getdate() AS CreatedDate, 0 As DocStatus, " + CurrentSiteId + " As SiteId, " + CurrentDivisionId + " As DivisionId ";
            ExecuteQuery(mQry);
        }

        public void ExecuteQuery(string Qry)
        {
            string ConnectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(Qry);
                cmd.Connection = sqlConnection;
                cmd.ExecuteNonQuery();
            }
        }

        public object ExecuteScaler(string Qry)
        {
            object val = null;
            string ConnectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(Qry);
                cmd.Connection = sqlConnection;
                val = cmd.ExecuteScalar();
            }

            return val;
        }


        public void ReCreateProcedures()
        {
            try
            {
                mQry = @"IF OBJECT_ID ('Web.spGetHelpListLedgerAccountForGroup') IS NOT NULL
	                        DROP PROCEDURE Web.spGetHelpListLedgerAccountForGroup
                        CREATE PROCEDURE [Web].[spGetHelpListLedgerAccountForGroup]
                        (
	                        @LedgerAccountGroupId INT
                        )
                        AS

                        WITH CTE AS  
                        (    
	                        SELECT Ag.LedgerAccountGroupId
	                        FROM Web.LedgerAccountGroups Ag
	                        WHERE 1=1
	                        AND (@LedgerAccountGroupId IS NULL OR Ag.LedgerAccountGroupId = @LedgerAccountGroupId)   
	                        UNION ALL      
	                        SELECT Ag.LedgerAccountGroupId
	                        FROM Web.LedgerAccountGroups Ag 
	                        INNER JOIN CTE ON Ag.ParentLedgerAccountGroupId = CTE.LedgerAccountGroupId
                        )      
                        SELECT Convert(NVARCHAR,A.LedgerAccountId) AS [id], A.LedgerAccountName AS [text]
                        FROM CTE T
                        LEFT JOIN Web.LedgerAccounts A ON T.LedgerAccountGroupId = A.LedgerAccountGroupId    
                        WHERE A.LedgerAccountId IS NOT NULL
                        ORDER BY A.LedgerAccountName";
                ExecuteQuery(mQry);
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }
        }


        public void DataCorrection()
        {
            try
            {
                mQry = @"UPDATE Web.JobReceiveLines
                        SET Web.JobReceiveLines.ProductId = V1.ProductId
                        FROM (
	                        SELECT L.JobReceiveLineId, Jol.ProductId
	                        FROM Web.JobReceiveLines L 
	                        LEFT JOIN Web.JobOrderLines Jol ON L.JobOrderLineId = Jol.JobOrderLineId
	                        WHERE L.JobOrderLineId IS NOT NULL
	                        AND L.ProductId IS NULL
                        ) AS V1 WHERE Web.JobReceiveLines.JobReceiveLineId = V1.JobReceiveLineId";
                ExecuteQuery(mQry);
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                mQry = @"UPDATE Web.JobReceiveLines
                        SET Web.JobReceiveLines.Dimension1Id = V1.Dimension1Id
                        FROM (
	                        SELECT L.JobReceiveLineId, Jol.Dimension1Id
	                        FROM Web.JobReceiveLines L 
	                        LEFT JOIN Web.JobOrderLines Jol ON L.JobOrderLineId = Jol.JobOrderLineId
	                        WHERE L.JobOrderLineId IS NOT NULL
	                        AND L.Dimension1Id IS NULL AND Jol.Dimension1Id IS NOT NULL
                        ) AS V1 WHERE Web.JobReceiveLines.JobReceiveLineId = V1.JobReceiveLineId";
                ExecuteQuery(mQry);
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                mQry = @"UPDATE Web.JobReceiveLines
                        SET Web.JobReceiveLines.Dimension2Id = V1.Dimension2Id
                        FROM (
	                        SELECT L.JobReceiveLineId, Jol.Dimension2Id
	                        FROM Web.JobReceiveLines L 
	                        LEFT JOIN Web.JobOrderLines Jol ON L.JobOrderLineId = Jol.JobOrderLineId
	                        WHERE L.JobOrderLineId IS NOT NULL
	                        AND L.Dimension2Id IS NULL AND Jol.Dimension2Id IS NOT NULL
                        ) AS V1 WHERE Web.JobReceiveLines.JobReceiveLineId = V1.JobReceiveLineId";
                ExecuteQuery(mQry);
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                mQry = @"UPDATE Web.JobReceiveLines
                        SET Web.JobReceiveLines.Dimension3Id = V1.Dimension3Id
                        FROM (
	                        SELECT L.JobReceiveLineId, Jol.Dimension3Id
	                        FROM Web.JobReceiveLines L 
	                        LEFT JOIN Web.JobOrderLines Jol ON L.JobOrderLineId = Jol.JobOrderLineId
	                        WHERE L.JobOrderLineId IS NOT NULL
	                        AND L.Dimension3Id IS NULL AND Jol.Dimension3Id IS NOT NULL
                        ) AS V1 WHERE Web.JobReceiveLines.JobReceiveLineId = V1.JobReceiveLineId";
                ExecuteQuery(mQry);
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                mQry = @"UPDATE Web.JobReceiveLines
                        SET Web.JobReceiveLines.Dimension4Id = V1.Dimension4Id
                        FROM (
	                        SELECT L.JobReceiveLineId, Jol.Dimension4Id
	                        FROM Web.JobReceiveLines L 
	                        LEFT JOIN Web.JobOrderLines Jol ON L.JobOrderLineId = Jol.JobOrderLineId
	                        WHERE L.JobOrderLineId IS NOT NULL
	                        AND L.Dimension4Id IS NULL AND Jol.Dimension4Id IS NOT NULL
                        ) AS V1 WHERE Web.JobReceiveLines.JobReceiveLineId = V1.JobReceiveLineId";
                ExecuteQuery(mQry);
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*)  FROM Web.ProductNatures WHERE ProductNatureName = 'Ledger Account'") == 0)
                {
                    mQry = @"INSERT INTO Web.ProductNatures (ProductNatureName, IsActive, IsSystemDefine, CreatedBy, ModifiedBy, CreatedDate, ModifiedDate)
                            VALUES ('Ledger Account', 1, 0, 'System', 'System', getdate(), getdate())";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*)  FROM Web.ProductTypes WHERE ProductTypeName = 'Ledger Account'") == 0)
                {
                    mQry = @"INSERT INTO Web.ProductTypes (ProductTypeName, ProductNatureId, IsCustomUI, IsActive, IsPostedInStock, IsSystemDefine, CreatedBy, ModifiedBy, CreatedDate, ModifiedDate)
                            VALUES ('Ledger Account', (SELECT ProductNatureId  FROM Web.ProductNatures WHERE ProductNatureName = 'Ledger Account'), 0, 1, 1, 0, 'System', 'System', getdate(), getdate())";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*)  FROM Web.ProductGroups WHERE ProductGroupName = 'Ledger Account'") == 0)
                {
                    mQry = @"INSERT INTO Web.ProductGroups (ProductGroupName, ProductTypeId, IsSystemDefine, IsActive, CreatedBy, ModifiedBy, CreatedDate, ModifiedDate, RateDecimalPlaces)
                            VALUES ('Ledger Account', (SELECT ProductTypeId  FROM Web.ProductTypes WHERE ProductTypeName = 'Ledger Account'), 0, 1, 'System', 'System', getdate(), getdate(), 2)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT CHARACTER_MAXIMUM_LENGTH  FROM INFORMATION_SCHEMA.Columns WHERE TABLE_NAME = 'Products' AND COLUMN_NAME = 'ProductName'") == 50)
                {
                    mQry = @"ALTER TABLE Web.Products ALTER COLUMN ProductName NVARCHAR(100) NOT NULL";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) FROM INFORMATION_SCHEMA.Columns WHERE TABLE_NAME = 'JobReceiveHeaders' AND COLUMN_NAME = 'JobReceiveById' AND IS_NULLABLE = 'NO'") > 0)
                {
                    mQry = @"ALTER TABLE Web.JobReceiveHeaders ALTER COLUMN JobReceiveById INT NULL";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) FROM INFORMATION_SCHEMA.Columns WHERE TABLE_NAME = 'JobReceiveQAHeaders' AND COLUMN_NAME = 'QAById' AND IS_NULLABLE = 'NO'") > 0)
                {
                    mQry = @"ALTER TABLE Web.JobReceiveQAHeaders ALTER COLUMN QAById INT NULL";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }


            try
            {
                mQry = @"INSERT INTO Web.Products (ProductCode, ProductName, ProductDescription, ProductSpecification, StandardCost, ProductGroupId, SalesTaxGroupProductId, DrawBackTariffHeadId, UnitId, DivisionId, ImageFolderName, ImageFileName, StandardWeight, Tags, CBM, IsActive, IsSystemDefine, CreatedBy, ModifiedBy, CreatedDate, ModifiedDate, ReferenceDocTypeId, ReferenceDocId, OMSId, Discriminator, PurchaseProduction, CheckSum, ProductPhotoName, Photo, GrossWeight, ProfitMargin, CarryingCost, DocTypeId, ProductCategoryId, SaleRate, Dimension1, DefaultDimension1Id, DefaultDimension2Id, DefaultDimension3Id, DefaultDimension4Id, DiscontinueDate, DiscontinueReason, SalesTaxProductCodeId)
                        SELECT SubString(A.LedgerAccountName,1,50) AS ProductCode, A.LedgerAccountName, A.LedgerAccountName,
                        NULL AS ProductSpecification, 0 AS StandardCost, 
                        (SELECT ProductGroupId  FROM Web.ProductGroups WHERE ProductGroupName  = 'Ledger Account') AS ProductGroupId, 
                        (SELECT ChargeGroupProductId  FROM Web.ChargeGroupProducts WHERE ChargeGroupProductName = 'GST 5%') AS SalesTaxGroupProductId, 
                        NULL AS DrawBackTariffHeadId, 'Nos' AS UnitId, 1 AS DivisionId, 
                        NULL AS ImageFolderName, NULL AS ImageFileName, 0 AS StandardWeight, NULL AS Tags, NULL AS CBM, 
                        1 AS IsActive, 1 AS IsSystemDefine, A.CreatedBy, A.ModifiedBy, A.CreatedDate, A.ModifiedDate, 
                        NULL AS ReferenceDocTypeId, NULL AS ReferenceDocId, NULL AS OMSId, NULL AS Discriminator, 
                        NULL AS PurchaseProduction, NULL AS CheckSum, NULL AS ProductPhotoName, NULL AS Photo, 
                        0 AS GrossWeight, 0 AS ProfitMargin, 0 AS CarryingCost, 
                        NULL AS DocTypeId, NULL AS ProductCategoryId, 0 AS SaleRate, NULL AS Dimension1, 
                        NULL AS DefaultDimension1Id, NULL AS DefaultDimension2Id, NULL AS DefaultDimension3Id, NULL AS DefaultDimension4Id, 
                        NULL AS DiscontinueDate, NULL AS DiscontinueReason, NULL AS SalesTaxProductCodeId
                        FROM Web.LedgerAccounts A
                        LEFT JOIN Web.Products P ON A.LedgerAccountName = P.ProductName
                        WHERE A.PersonId IS NULL
                        AND P.ProductId IS NULL";
                ExecuteQuery(mQry);
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                mQry = @"UPDATE Web.LedgerAccounts
                        SET Web.LedgerAccounts.ProductId = V1.ProductId
                        FROM (
	                        SELECT A.LedgerAccountId, P.ProductId
	                        FROM Web.LedgerAccounts A
	                        LEFT JOIN Web.Products P ON A.LedgerAccountName = P.ProductName
	                        WHERE A.ProductId IS NULL
	                        AND P.ProductId IS NOT NULL
                        ) AS V1 WHERE Web.LedgerAccounts.LedgerAccountId = V1.LedgerAccountId";
                ExecuteQuery(mQry);
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) FROM Web.UserRoles WHERE SiteId IS NULL") > 0)
                {
                    mQry = @"UPDATE Web.UserRoles SET SiteId = (SELECT SiteId FROM Web.Sites WHERE SiteCode = 'Main') Where SiteId Is Null";
                    ExecuteQuery(mQry);

                    mQry = " ALTER TABLE Web.UserRoles ALTER COLUMN SiteId INT NOT NULL";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) FROM Web.UserRoles WHERE DivisionId IS NULL") > 0)
                {
                    mQry = @"UPDATE Web.UserRoles SET DivisionId = (SELECT DivisionId FROM Web.Divisions WHERE DivisionName = 'Main') Where DivisionId Is Null";
                    ExecuteQuery(mQry);

                    mQry = " ALTER TABLE Web.UserRoles ALTER COLUMN DivisionId INT NOT NULL";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) FROM Web.UserRoles WHERE CreatedDate IS NULL") > 0)
                {
                    mQry = @"UPDATE Web.UserRoles SET CreatedDate = getdate() Where CreatedDate Is Null";
                    ExecuteQuery(mQry);

                    mQry = " ALTER TABLE Web.UserRoles ALTER COLUMN CreatedDate DATETIME NOT NULL";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) FROM Web.UserRoles WHERE ModifiedDate IS NULL") > 0)
                {
                    mQry = @"UPDATE Web.UserRoles SET ModifiedDate = getdate() Where ModifiedDate Is Null";
                    ExecuteQuery(mQry);

                    mQry = " ALTER TABLE Web.UserRoles ALTER COLUMN ModifiedDate DATETIME NOT NULL";
                    ExecuteQuery(mQry);

                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) FROM Web.DocumentCategories WHERE DocumentCategoryName = 'User'") == 0)
                {
                    mQry = @"INSERT INTO Web.DocumentCategories (DocumentCategoryName, IsActive, IsSystemDefine, OMSId)
                                VALUES ('User', 1, 0, NULL)";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }

            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) FROM Web.DocumentTypes WHERE DocumentTypeName = 'User'") == 0)
                {
                    mQry = @"INSERT INTO Web.DocumentTypes (DocumentTypeShortName, DocumentTypeName, DocumentCategoryId, IsSystemDefine, IsActive, CreatedBy, ModifiedBy, CreatedDate, ModifiedDate)
                            VALUES ('User', 'User', (SELECT DocumentCategoryId  FROM Web.DocumentCategories WHERE DocumentCategoryName = 'User'), 
                            1, 1, 'Admin', 'Admin', getdate(), getdate()) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }



            try
            {
                if ((int)ExecuteScaler("SELECT Count(*) AS Cnt FROM INFORMATION_SCHEMA.Columns WHERE COLUMN_NAME =  'EmployeeId' AND TABLE_NAME = 'Employees'") == 0)
                {
                    mQry = @"ALTER TABLE Web.Employees ADD EmployeeId INT ";
                    ExecuteQuery(mQry);
                    mQry = @"UPDATE Web.Employees
                    SET Web.Employees.EmployeeId = V1.EmployeeId
                    FROM (
	                    SELECT Row_number() OVER (ORDER BY PersonID) AS EmployeeId, PersonID FROM Web.Employees
                    ) AS V1 WHERE Web.Employees.PersonId = V1.PersonId ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.Employees ALTER COLUMN EmployeeId INT  NOT NULL ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.JobReceiveHeaders DROP CONSTRAINT [FK_Web.JobReceiveHeaders_Web.Employees_JobReceiveById] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.JobOrderAmendmentHeaders DROP CONSTRAINT [FK_Web.JobOrderAmendmentHeaders_Web.Employees_OrderById] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.JobReceiveQAHeaders DROP CONSTRAINT [FK_Web.JobReceiveQAHeaders_Web.Employees_QAById] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.PurchaseOrderInspectionHeaders DROP CONSTRAINT [FK_Web.PurchaseOrderInspectionHeaders_Web.Employees_InspectionById] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.JobInvoiceAmendmentHeaders DROP CONSTRAINT [FK_Web.JobInvoiceAmendmentHeaders_Web.Employees_OrderById] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.JobOrderHeaders DROP CONSTRAINT [FK_Web.JobOrderHeaders_Web.Employees_OrderById] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.JobOrderCancelHeaders DROP CONSTRAINT [FK_Web.JobOrderCancelHeaders_Web.Employees_OrderById] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.JobOrderInspectionHeaders DROP CONSTRAINT [FK_Web.JobOrderInspectionHeaders_Web.Employees_InspectionById] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.JobReturnHeaders DROP CONSTRAINT [FK_Web.JobReturnHeaders_Web.Employees_OrderById] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.SaleDeliveryOrderCancelHeaders DROP CONSTRAINT [FK_Web.SaleDeliveryOrderCancelHeaders_Web.Employees_OrderById] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.GatePassHeaders DROP CONSTRAINT [FK__GatePassH__Order__42FBEF3C] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.AttendanceLines DROP CONSTRAINT [FK__Attendanc__Emplo__16935D19] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.Employees DROP CONSTRAINT [PK_Web.Employees] ";
                    ExecuteQuery(mQry);
                    mQry = @"ALTER TABLE Web.Employees ADD CONSTRAINT [PK_Web.Employees] PRIMARY KEY (EmployeeId) ";
                    ExecuteQuery(mQry);
                }
            }
            catch (Exception ex)
            {
                RecordError(ex);
            }
        }
    }
}



