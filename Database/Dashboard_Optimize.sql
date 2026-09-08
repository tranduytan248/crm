-- ============================================================
-- Toi uu hieu nang Dashboard CRM
-- Phan 1: Bo sung PK/index cho cac bang HEAP va cot loc nong
-- Phan 2: SP moi RM_Dashboard_Plan_Get_V2 (gop nguoi lien quan,
--          loc theo owner HOAC nguoi lien quan ngay trong SQL)
-- Script idempotent - chay lai nhieu lan khong loi
-- ============================================================
SET NOCOUNT ON;

-- ---------- 1. Clustered PK cho 4 bang HEAP ----------
IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('RM_SalesTeamMembers') AND type = 'PK')
    ALTER TABLE dbo.RM_SalesTeamMembers ADD CONSTRAINT PK_RM_SalesTeamMembers PRIMARY KEY CLUSTERED (MemberID);

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('RM_ProjectMember') AND type = 'PK')
    ALTER TABLE dbo.RM_ProjectMember ADD CONSTRAINT PK_RM_ProjectMember PRIMARY KEY CLUSTERED (ProjectMemberID);

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('RM_ProductProject') AND type = 'PK')
    ALTER TABLE dbo.RM_ProductProject ADD CONSTRAINT PK_RM_ProductProject PRIMARY KEY CLUSTERED (ProductProjectID);

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('RM_RevenueReceived') AND type = 'PK')
    ALTER TABLE dbo.RM_RevenueReceived ADD CONSTRAINT PK_RM_RevenueReceived PRIMARY KEY CLUSTERED (RevenueReceivedID);
PRINT 'Step 1 done: clustered PKs';

-- ---------- 2. Nonclustered index cho cac duong truy van nong ----------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_RM_SalesTeamMembers_BO' AND object_id=OBJECT_ID('RM_SalesTeamMembers'))
    CREATE NONCLUSTERED INDEX IX_RM_SalesTeamMembers_BO
        ON dbo.RM_SalesTeamMembers (BusinessOpportunityID, IsDeleted) INCLUDE (EmployeeID, RoleID);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_RM_SalesTeamMembers_Emp' AND object_id=OBJECT_ID('RM_SalesTeamMembers'))
    CREATE NONCLUSTERED INDEX IX_RM_SalesTeamMembers_Emp
        ON dbo.RM_SalesTeamMembers (EmployeeID, IsDeleted) INCLUDE (BusinessOpportunityID);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_RM_ProjectMember_PP' AND object_id=OBJECT_ID('RM_ProjectMember'))
    CREATE NONCLUSTERED INDEX IX_RM_ProjectMember_PP
        ON dbo.RM_ProjectMember (ProductProjectID, IsDeleted) INCLUDE (Employee_ID, RoleID);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_RM_ProductProject_Project' AND object_id=OBJECT_ID('RM_ProductProject'))
    CREATE NONCLUSTERED INDEX IX_RM_ProductProject_Project
        ON dbo.RM_ProductProject (ProjectID, IsDeleted) INCLUDE (ExpectedRevenue);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_RM_RevenueReceived_PP' AND object_id=OBJECT_ID('RM_RevenueReceived'))
    CREATE NONCLUSTERED INDEX IX_RM_RevenueReceived_PP
        ON dbo.RM_RevenueReceived (ProductProjectID, IsDeleted) INCLUDE (Amount);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_RM_OppPlanRelatedPerson_Plan' AND object_id=OBJECT_ID('RM_OpportunityPlanRelatedPerson'))
    CREATE NONCLUSTERED INDEX IX_RM_OppPlanRelatedPerson_Plan
        ON dbo.RM_OpportunityPlanRelatedPerson (OpportunityPlanID, IsDeleted) INCLUDE (Username);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_RM_OpportunityPlan_WorkingDate' AND object_id=OBJECT_ID('RM_OpportunityPlan'))
    CREATE NONCLUSTERED INDEX IX_RM_OpportunityPlan_WorkingDate
        ON dbo.RM_OpportunityPlan (IsDeleted, WorkingDate) INCLUDE (BusinessOpportunityID, Username);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_RM_BusinessOpportunity_CreatedDate' AND object_id=OBJECT_ID('RM_BusinessOpportunity'))
    CREATE NONCLUSTERED INDEX IX_RM_BusinessOpportunity_CreatedDate
        ON dbo.RM_BusinessOpportunity (IsDeleted, CreatedDate)
        INCLUDE (StatusID, ClosingProbability, ExpectedValue, CustomerID);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_RM_ExchangeHistory_BO' AND object_id=OBJECT_ID('RM_ExchangeHistory'))
    CREATE NONCLUSTERED INDEX IX_RM_ExchangeHistory_BO
        ON dbo.RM_ExchangeHistory (BusinessOpportunityID, ExchangeDate DESC) INCLUDE (MemberID, ContactPerson_ID);
PRINT 'Step 2 done: nonclustered indexes';
GO

-- ---------- 3. RM_Dashboard_Plan_Get_V2 ----------
-- Thay the mau goi: Plan_Get (2 lan) + N x RelatedPerson_GetByOpportunityPlanID
-- bang 1 lan goi duy nhat:
--   - Tra kem RelatedPersonNames / RelatedPersonUsernames (gop chuoi)
--   - @EmployeeIds: loc ke hoach cua cac nhan su duoc phep xem
--   - @Username:    lay THEM ke hoach ma user hien tai la nguoi lien quan
--                   (khop ca alias truoc dau @ nhu logic cu tren C#)
IF OBJECT_ID('dbo.RM_Dashboard_Plan_Get_V2', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Dashboard_Plan_Get_V2;
GO
CREATE PROCEDURE dbo.RM_Dashboard_Plan_Get_V2
    @FromDate    DATETIME,
    @ToDate      DATETIME,
    @EmployeeIds NVARCHAR(MAX) = NULL,
    @Username    NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @FromDate = ISNULL(@FromDate, DATEADD(DAY, -30, CAST(GETDATE() AS DATE)));
    SET @ToDate   = ISNULL(@ToDate,   CAST(GETDATE() AS DATE));
    IF @Username = N'' SET @Username = NULL;

    DECLARE @Filter TABLE (Id INT PRIMARY KEY);
    IF @EmployeeIds IS NOT NULL
        INSERT INTO @Filter
        SELECT DISTINCT Id FROM dbo.fn_SplitIntList(@EmployeeIds);

    -- alias = phan truoc dau @ cua username hien tai
    DECLARE @UserPrefix NVARCHAR(150) =
        CASE WHEN @Username IS NULL THEN NULL
             WHEN CHARINDEX('@', @Username) > 0 THEN LEFT(@Username, CHARINDEX('@', @Username) - 1)
             ELSE @Username END;

    SELECT
        p.Id,
        p.BusinessOpportunityID,
        p.Username,
        p.PlanName,
        p.Content,
        p.WorkingDate,
        p.AddressMeeting,
        p.CreatedBy,
        p.CreatedDate,
        p.UpdatedBy,
        p.UpdatedDate,
        b.OpportunityName,
        b.CodeOpportunity,
        c.CustomerName,
        u.FullName AS EmployeeFullName,
        u.UserName AS EmployeeUserName,
        ISNULL(rp.Names, '')     AS RelatedPersonNames,
        ISNULL(rp.Usernames, '') AS RelatedPersonUsernames
    FROM RM_OpportunityPlan p
    INNER JOIN RM_BusinessOpportunity b
            ON b.BusinessOpportunityID = p.BusinessOpportunityID AND b.IsDeleted = 0
    LEFT JOIN RM_Customer c
            ON c.CustomerID = b.CustomerID AND c.IsDeleted = 0
    INNER JOIN Sys_Users u
            ON u.UserName = p.Username
    OUTER APPLY (
        SELECT
            STUFF((SELECT DISTINCT ', ' + ISNULL(NULLIF(su.FullName, N''), d.Username)
                   FROM RM_OpportunityPlanRelatedPerson d
                   LEFT JOIN Sys_Users su ON su.UserName = d.Username
                   WHERE d.OpportunityPlanID = p.Id AND d.IsDeleted = 0
                   FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Names,
            STUFF((SELECT DISTINCT ',' + d.Username
                   FROM RM_OpportunityPlanRelatedPerson d
                   WHERE d.OpportunityPlanID = p.Id AND d.IsDeleted = 0
                   FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') AS Usernames
    ) rp
    WHERE p.IsDeleted = 0
      AND p.WorkingDate >= CAST(@FromDate AS DATE)
      AND p.WorkingDate <  DATEADD(DAY, 1, CAST(@ToDate AS DATE))
      AND (
            (@EmployeeIds IS NULL AND @Username IS NULL)
            OR (@EmployeeIds IS NOT NULL AND u.UserId IN (SELECT Id FROM @Filter))
            OR (@Username IS NOT NULL AND EXISTS (
                    SELECT 1
                    FROM RM_OpportunityPlanRelatedPerson d
                    WHERE d.OpportunityPlanID = p.Id
                      AND d.IsDeleted = 0
                      AND (   d.Username = @Username
                           OR d.Username = @UserPrefix
                           OR (CHARINDEX('@', d.Username) > 0
                               AND LEFT(d.Username, CHARINDEX('@', d.Username) - 1) = @UserPrefix))
               ))
          )
    ORDER BY p.WorkingDate ASC, p.CreatedDate DESC;
END
GO
PRINT 'Step 3 done: RM_Dashboard_Plan_Get_V2';
