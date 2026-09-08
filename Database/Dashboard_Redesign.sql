-- ============================================================
-- Lam lai Dashboard:
--   1. RM_Dashboard_GetOpportunityByGroupService : chart co hoi theo NHOM dich vu
--   2. RM_Dashboard_GetProjectByGroupService     : chart du an theo NHOM dich vu
--   3. RM_Dashboard_Opportunity_GetByGroupService: danh sach co hoi cua 1 nhom (popup)
--   4. RM_Dashboard_Project_GetByGroupService    : danh sach du an cua 1 nhom (popup)
--   5. ALTER RM_Dashboard_GetSummary_v2          : "Doanh thu thuc hien" tinh theo
--      ngay THUC NHAN TIEN (ReceivedDate) trong ky, thay vi ngay bat dau du an
-- Script idempotent - chay lai nhieu lan khong loi
-- ============================================================
SET NOCOUNT ON;
GO

-- ---------- 1. Chart: Co hoi theo nhom dich vu ----------
-- Dieu kien loc PHAI GIONG HET SP danh sach chi tiet
-- (RM_Dashboard_Opportunity_GetByGroupService) de so tren chart khop voi popup:
--   - chi dem co hoi DANG MO (bo WON / CONVERTED / LOST)
--   - quyen xem: nguoi tao HOAC thanh vien thuoc pham vi quan ly (@Username)
IF OBJECT_ID('dbo.RM_Dashboard_GetOpportunityByGroupService', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Dashboard_GetOpportunityByGroupService;
GO
CREATE PROCEDURE dbo.RM_Dashboard_GetOpportunityByGroupService
    @FromDate    DATETIME,
    @ToDate      DATETIME,
    @EmployeeIds NVARCHAR(MAX) = NULL,
    @Username    VARCHAR(150)  = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Filter TABLE (Id INT);
    IF @EmployeeIds IS NOT NULL
        INSERT INTO @Filter
        SELECT Id FROM dbo.fn_SplitIntList(@EmployeeIds);

    DECLARE @LostID INT = (SELECT ID FROM RM_Status WHERE StatusCode = 'LOST' AND StatusKey = 'Opportunity' AND IsDeleted = 0);

    DECLARE @WonStatus TABLE (ID INT);
    INSERT INTO @WonStatus (ID)
    SELECT ID FROM RM_Status
    WHERE StatusCode IN ('WON', 'CONVERTED') AND StatusKey = 'Opportunity' AND IsDeleted = 0;

    SELECT gs.GroupServiceID,
           gs.NameGroup AS Name,
           COUNT(DISTINCT o.BusinessOpportunityID) AS Quantity
    FROM   RM_BusinessOpportunity o
           LEFT JOIN @WonStatus ws ON o.StatusID = ws.ID
           CROSS APPLY dbo.SplitString(o.ProductServiceIDs, ';') ps
           JOIN RM_ProductService s ON ps.Value = s.ProductServiceID AND s.IsDeleted = 0
           JOIN RM_GroupService gs  ON gs.GroupServiceID = s.GroupServiceID AND ISNULL(gs.IsDeleted, 0) = 0
    WHERE  o.IsDeleted = 0
           AND (@FromDate IS NULL OR o.CreatedDate >= CAST(@FromDate AS DATE))
           AND (@ToDate   IS NULL OR o.CreatedDate <  DATEADD(DAY, 1, CAST(@ToDate AS DATE)))
           -- Chi co hoi dang mo, giong danh sach chi tiet
           AND (ws.ID IS NULL AND o.StatusID <> @LostID)
           -- Quyen xem: nguoi tao hoac thanh vien trong pham vi quan ly
           AND (
                (@Username IS NULL AND @EmployeeIds IS NULL)
                OR o.CreatedBy = @Username
                OR EXISTS (SELECT 1
                           FROM RM_SalesTeamMembers stm
                           JOIN @Filter f ON f.Id = stm.EmployeeID
                           WHERE stm.BusinessOpportunityID = o.BusinessOpportunityID
                             AND stm.IsDeleted = 0)
           )
    GROUP BY gs.GroupServiceID, gs.NameGroup
    ORDER BY Quantity DESC;
END
GO

-- ---------- 2. Chart: Du an theo nhom dich vu ----------
-- Dieu kien loc PHAI GIONG HET SP danh sach chi tiet
-- (RM_Dashboard_Project_GetByGroupService):
--   - quyen xem: nguoi tao HOAC thanh vien du an (@Username)
--   - quyen thanh vien xet o cap DU AN (moi product cua du an), khong xet
--     tung dong product rieng le
IF OBJECT_ID('dbo.RM_Dashboard_GetProjectByGroupService', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Dashboard_GetProjectByGroupService;
GO
CREATE PROCEDURE dbo.RM_Dashboard_GetProjectByGroupService
    @FromDate    DATETIME,
    @ToDate      DATETIME,
    @EmployeeIds NVARCHAR(MAX) = NULL,
    @Username    VARCHAR(150)  = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Filter TABLE (Id INT);
    IF @EmployeeIds IS NOT NULL
        INSERT INTO @Filter
        SELECT Id FROM dbo.fn_SplitIntList(@EmployeeIds);

    SELECT gs.GroupServiceID,
           gs.NameGroup AS Name,
           COUNT(DISTINCT p.ProjectID) AS Quantity
    FROM   RM_Project p
           JOIN RM_ProductProject pp ON pp.ProjectID = p.ProjectID AND pp.IsDeleted = 0
           JOIN RM_ProductService s  ON s.ProductServiceID = pp.ProductServiceID AND s.IsDeleted = 0
           JOIN RM_GroupService gs   ON gs.GroupServiceID = s.GroupServiceID AND ISNULL(gs.IsDeleted, 0) = 0
    WHERE  p.IsDeleted = 0
           AND (@FromDate IS NULL OR p.StartDate >= CAST(@FromDate AS DATE))
           AND (@ToDate   IS NULL OR p.StartDate <  DATEADD(DAY, 1, CAST(@ToDate AS DATE)))
           -- Quyen xem: nguoi tao hoac thanh vien cua du an (xet moi product cua du an)
           AND (
                (@Username IS NULL AND @EmployeeIds IS NULL)
                OR p.CreatedBy = @Username
                OR EXISTS (SELECT 1
                           FROM RM_ProductProject pp2
                           JOIN RM_ProjectMember pm ON pm.ProductProjectID = pp2.ProductProjectID
                           WHERE pp2.ProjectID = p.ProjectID
                             AND pm.IsDeleted = 0
                             AND pm.Employee_ID IN (SELECT Id FROM @Filter)))
    GROUP BY gs.GroupServiceID, gs.NameGroup
    ORDER BY Quantity DESC;
END
GO

-- ---------- 3. Popup: danh sach co hoi cua 1 nhom dich vu ----------
-- Cot tra ve giong het RM_Dashboard_Opportunity_Get_V2 de tai dung model + popup
IF OBJECT_ID('dbo.RM_Dashboard_Opportunity_GetByGroupService', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Dashboard_Opportunity_GetByGroupService;
GO
CREATE PROCEDURE dbo.RM_Dashboard_Opportunity_GetByGroupService
    @FromDate       DATETIME,
    @ToDate         DATETIME,
    @GroupServiceID INT,
    @Username       VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SET @FromDate = ISNULL(@FromDate, DATEADD(DAY, -30, CAST(GETDATE() AS DATE)));
    SET @ToDate   = ISNULL(@ToDate,   CAST(GETDATE() AS DATE));

    DECLARE @UserTable TABLE (ID INT, UserName VARCHAR(150));
    INSERT INTO @UserTable (ID, UserName)
    EXEC Sys_User_ByManager @Username;

    DECLARE @LostID INT = (SELECT ID FROM RM_Status WHERE StatusCode = 'LOST' AND StatusKey = 'Opportunity' AND IsDeleted = 0);

    DECLARE @WonStatus TABLE (ID INT);
    INSERT INTO @WonStatus (ID)
    SELECT ID FROM RM_Status
    WHERE StatusCode IN ('WON', 'CONVERTED') AND StatusKey = 'Opportunity' AND IsDeleted = 0;

    SELECT b.BusinessOpportunityID,
           b.CodeOpportunity,
           b.OpportunityName,
           o.StatusName,
           o.StatusClass,
           c.CustomerName,
           svc.ProductServices,
           b.ClosingProbability,
           b.ExpectedValue,
           b.Description,
           eh.ExchangeDate,
           eh.FullName,
           eh.ExchangeContent,
           eh.ContactPersonName,
           eh.ContactPersonPosition,
           ISNULL(mem.Members, '') AS Members
    FROM RM_BusinessOpportunity b
    LEFT JOIN RM_Customer c ON c.CustomerID = b.CustomerID AND c.IsDeleted = 0
    LEFT JOIN RM_Status o   ON o.ID = b.StatusID AND o.IsDeleted = 0
    LEFT JOIN @WonStatus ws ON b.StatusID = ws.ID
    OUTER APPLY (
        SELECT TOP 1 eh.ExchangeDate,
               e.FullName,
               eh.ExchangeContent,
               c2.FullName  AS ContactPersonName,
               cc.Position  AS ContactPersonPosition
        FROM RM_ExchangeHistory eh
        JOIN RM_SalesTeamMembers m ON eh.MemberID = m.MemberID
        JOIN Sys_Users e ON m.EmployeeID = e.UserId
        LEFT JOIN RM_ContactPersons c2 ON c2.ContactPerson_ID = eh.ContactPerson_ID
        LEFT JOIN RM_CustomerContact cc ON cc.ContactPersonID = eh.ContactPerson_ID
        WHERE eh.BusinessOpportunityID = b.BusinessOpportunityID
        ORDER BY eh.ExchangeDate DESC
    ) eh
    LEFT JOIN (
        SELECT b2.BusinessOpportunityID,
               STUFF((SELECT '; ' + ps.ShortNameProduct
                      FROM RM_BusinessOpportunity b3
                      CROSS APPLY dbo.SplitString(b3.ProductServiceIDs, ';') s
                      JOIN RM_ProductService ps ON ps.ProductServiceID = CAST(s.Value AS INT)
                      WHERE b3.BusinessOpportunityID = b2.BusinessOpportunityID
                      FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS ProductServices
        FROM RM_BusinessOpportunity b2
    ) svc ON svc.BusinessOpportunityID = b.BusinessOpportunityID
    LEFT JOIN (
        SELECT stm.BusinessOpportunityID,
               STUFF((SELECT '; ' + e.FullName +
                             CASE WHEN Roles IS NOT NULL AND Roles <> '' THEN ' (' + Roles + ')' ELSE '' END
                      FROM (SELECT stm2.BusinessOpportunityID,
                                   stm2.EmployeeID,
                                   STUFF((SELECT DISTINCT ', ' +
                                                 CASE WHEN CHARINDEX('(', r.RoleName) > 0
                                                           AND CHARINDEX(')', r.RoleName) > CHARINDEX('(', r.RoleName)
                                                      THEN SUBSTRING(r.RoleName,
                                                                     CHARINDEX('(', r.RoleName) + 1,
                                                                     CHARINDEX(')', r.RoleName) - CHARINDEX('(', r.RoleName) - 1)
                                                      ELSE r.RoleName END
                                          FROM RM_SalesTeamMembers stm3
                                          CROSS APPLY dbo.SplitString(stm3.RoleID, ';') rs
                                          JOIN RM_Roles r ON r.RoleID = rs.Value
                                          WHERE stm3.BusinessOpportunityID = stm2.BusinessOpportunityID
                                            AND stm3.EmployeeID = stm2.EmployeeID
                                            AND stm3.IsDeleted = 0
                                          FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Roles
                            FROM RM_SalesTeamMembers stm2
                            WHERE stm2.IsDeleted = 0
                            GROUP BY stm2.BusinessOpportunityID, stm2.EmployeeID) mem2
                      JOIN Sys_Users e ON e.UserId = mem2.EmployeeID
                      WHERE mem2.BusinessOpportunityID = stm.BusinessOpportunityID
                      FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Members
        FROM RM_SalesTeamMembers stm
        WHERE stm.IsDeleted = 0
        GROUP BY stm.BusinessOpportunityID
    ) mem ON mem.BusinessOpportunityID = b.BusinessOpportunityID
    WHERE b.IsDeleted = 0
      AND (@FromDate IS NULL OR b.CreatedDate >= CAST(@FromDate AS DATE))
      AND (@ToDate   IS NULL OR b.CreatedDate <  DATEADD(DAY, 1, CAST(@ToDate AS DATE)))
      -- Chi lay co hoi co it nhat 1 dich vu thuoc nhom duoc chon
      AND (@GroupServiceID IS NULL OR @GroupServiceID = 0
           OR EXISTS (SELECT 1
                      FROM dbo.SplitString(b.ProductServiceIDs, ';') ps
                      JOIN RM_ProductService s ON ps.Value = s.ProductServiceID AND s.IsDeleted = 0
                      WHERE s.GroupServiceID = @GroupServiceID))
      AND (ws.ID IS NULL AND b.StatusID <> @LostID)
      AND (b.CreatedBy = @Username
           OR EXISTS (SELECT 1
                      FROM RM_SalesTeamMembers stm
                      WHERE stm.BusinessOpportunityID = b.BusinessOpportunityID
                        AND stm.IsDeleted = 0
                        AND stm.EmployeeID IN (SELECT ID FROM @UserTable)))
    ORDER BY CASE WHEN eh.ExchangeDate IS NULL THEN 1 ELSE 0 END,
             eh.ExchangeDate DESC;
END
GO

-- ---------- 4. Popup: danh sach du an cua 1 nhom dich vu ----------
-- Cot tra ve giong het RM_Dashboard_Project_Get_V2 de tai dung model + popup
IF OBJECT_ID('dbo.RM_Dashboard_Project_GetByGroupService', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Dashboard_Project_GetByGroupService;
GO
CREATE PROCEDURE dbo.RM_Dashboard_Project_GetByGroupService
    @FromDate       DATETIME,
    @ToDate         DATETIME,
    @GroupServiceID INT,
    @Username       VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SET @FromDate = ISNULL(@FromDate, DATEADD(DAY, -30, CAST(GETDATE() AS DATE)));
    SET @ToDate   = ISNULL(@ToDate,   CAST(GETDATE() AS DATE));

    DECLARE @UserTable TABLE (ID INT, UserName VARCHAR(150));
    INSERT INTO @UserTable (ID, UserName)
    EXEC Sys_User_ByManager @Username;

    SELECT p.ProjectID,
           p.ProjectName,
           c.CustomerName,
           p.StartDate,
           o.StatusName,
           o.StatusClass,
           p.SuccessRate,
           p.Note,
           ISNULL(cost.TotalCost, 0)       AS TotalCost,
           ISNULL(revenue.TotalRevenue, 0) AS TotalRevenue,
           ISNULL(mem.Members, '')         AS Members
    FROM RM_Project p
    LEFT JOIN RM_Customer c ON p.CustomerID = c.CustomerID
    LEFT JOIN RM_Status o ON o.ID = p.Status
    LEFT JOIN (
        SELECT pp.ProjectID, SUM(pc.Amount) AS TotalCost
        FROM RM_ProductProject pp
        LEFT JOIN RM_ProductCost pc ON pp.ProductProjectID = pc.ProductProjectID AND pc.IsDeleted = 0
        GROUP BY pp.ProjectID
    ) cost ON cost.ProjectID = p.ProjectID
    LEFT JOIN (
        SELECT pp.ProjectID, SUM(r.Amount) AS TotalRevenue
        FROM RM_ProductProject pp
        LEFT JOIN RM_RevenueReceived r ON pp.ProductProjectID = r.ProductProjectID AND r.IsDeleted = 0
        GROUP BY pp.ProjectID
    ) revenue ON revenue.ProjectID = p.ProjectID
    LEFT JOIN (
        SELECT pp.ProjectID,
               STUFF((SELECT '; ' + e.FullName +
                             CASE WHEN mem2.Roles IS NOT NULL AND mem2.Roles <> '' THEN ' (' + mem2.Roles + ')' ELSE '' END
                      FROM (SELECT pp2.ProjectID,
                                   pm.Employee_ID,
                                   STUFF((SELECT DISTINCT ', ' +
                                                 CASE WHEN CHARINDEX('(', r.RoleName) > 0
                                                           AND CHARINDEX(')', r.RoleName) > CHARINDEX('(', r.RoleName)
                                                      THEN SUBSTRING(r.RoleName,
                                                                     CHARINDEX('(', r.RoleName) + 1,
                                                                     CHARINDEX(')', r.RoleName) - CHARINDEX('(', r.RoleName) - 1)
                                                      ELSE r.RoleName END
                                          FROM RM_ProductProject pp3
                                          JOIN RM_ProjectMember pm2 ON pm2.ProductProjectID = pp3.ProductProjectID
                                          CROSS APPLY dbo.SplitString(pm2.RoleID, ';') rs
                                          JOIN RM_Roles r ON r.RoleID = rs.Value
                                          WHERE pp3.ProjectID = pp2.ProjectID
                                            AND pm2.Employee_ID = pm.Employee_ID
                                            AND pm2.IsDeleted = 0
                                          FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Roles
                            FROM RM_ProductProject pp2
                            JOIN RM_ProjectMember pm ON pm.ProductProjectID = pp2.ProductProjectID
                            WHERE pm.IsDeleted = 0
                            GROUP BY pp2.ProjectID, pm.Employee_ID) mem2
                      JOIN Sys_Users e ON e.UserId = mem2.Employee_ID
                      WHERE mem2.ProjectID = pp.ProjectID
                      FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Members
        FROM RM_ProductProject pp
        WHERE pp.IsDeleted = 0
        GROUP BY pp.ProjectID
    ) mem ON mem.ProjectID = p.ProjectID
    WHERE p.IsDeleted = 0
      AND (@FromDate IS NULL OR p.StartDate >= CAST(@FromDate AS DATE))
      AND (@ToDate   IS NULL OR p.StartDate <  DATEADD(DAY, 1, CAST(@ToDate AS DATE)))
      -- Chi lay du an co it nhat 1 dich vu thuoc nhom duoc chon
      AND (@GroupServiceID IS NULL OR @GroupServiceID = 0
           OR EXISTS (SELECT 1
                      FROM RM_ProductProject pp
                      JOIN RM_ProductService s ON s.ProductServiceID = pp.ProductServiceID AND s.IsDeleted = 0
                      WHERE pp.ProjectID = p.ProjectID
                        AND pp.IsDeleted = 0
                        AND s.GroupServiceID = @GroupServiceID))
      AND (p.CreatedBy = @Username
           OR EXISTS (SELECT 1
                      FROM RM_ProductProject pp
                      JOIN RM_ProjectMember pm ON pm.ProductProjectID = pp.ProductProjectID
                      WHERE pp.ProjectID = p.ProjectID
                        AND pm.IsDeleted = 0
                        AND pm.Employee_ID IN (SELECT ID FROM @UserTable)));
END
GO

-- ---------- 5. Sua "Doanh thu thuc hien" trong GetSummary_v2 ----------
-- Truoc: cong Amount cua cac du an co StartDate trong ky -> luon 0 vi tien ve
--        khong phu thuoc ngay bat dau du an.
-- Sau  : cong Amount theo ngay THUC NHAN (ReceivedDate) trong ky loc.
IF OBJECT_ID('dbo.RM_Dashboard_GetSummary_v2', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Dashboard_GetSummary_v2;
GO
CREATE PROCEDURE [dbo].[RM_Dashboard_GetSummary_v2]
    @FromDate DATETIME,
    @ToDate DATETIME,
    @SuccessRate int,
    @EmployeeIds NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Filter TABLE (Id INT);
    IF @EmployeeIds IS NOT NULL
        INSERT INTO @Filter
        SELECT Id
        FROM   dbo.fn_SplitIntList(@EmployeeIds);

    DECLARE @TotalProjects int
    DECLARE @TotalOpportunities int
    DECLARE @TotalprojRevenue decimal(18, 2)
    DECLARE @TotalRevenue      decimal(18, 2)
    DECLARE @TotalExpectedValue decimal(18, 2)

    -- Trang thai LOST de loai khoi so dem co hoi, giong SP danh sach chi tiet
    DECLARE @LostID INT = (SELECT ID FROM RM_Status WHERE StatusCode = 'LOST' AND StatusKey = 'Opportunity' AND IsDeleted = 0);

    -- Doanh thu thuc hien: TONG TIEN DA VE cua cac du an trong danh sach chi tiet
    -- (du an bat dau trong ky + quyen nguoi tao/thanh vien). Cung tap va cung phep
    -- cong voi cot "Doanh thu" tren popup, nen so ngoai = tong cot trong popup.
    SELECT @TotalRevenue = ISNULL(SUM(r.Amount), 0)
    FROM   RM_RevenueReceived r
           JOIN RM_ProductProject pp
                ON  pp.ProductProjectID = r.ProductProjectID
                AND pp.IsDeleted = 0
           JOIN RM_Project p
                ON  p.ProjectID = pp.ProjectID
                AND p.IsDeleted = 0
    WHERE  r.IsDeleted = 0
           AND (@SuccessRate = 0 OR @SuccessRate IS NULL OR isNull(p.SuccessRate, 0) >= @SuccessRate)
           AND (@FromDate IS NULL OR p.StartDate >= CAST(@FromDate AS DATE))
           AND (@ToDate IS NULL OR p.StartDate < DATEADD(DAY, 1, CAST(@ToDate AS DATE)))
           AND (
                   @EmployeeIds IS NULL
                   OR EXISTS (
                          SELECT 1
                          FROM   RM_ProductProject pp2
                                 JOIN RM_ProjectMember pm
                                      ON  pm.ProductProjectID = pp2.ProductProjectID
                          WHERE  pp2.ProjectID = p.ProjectID
                                 AND pp2.IsDeleted = 0
                                 AND pm.IsDeleted = 0
                                 AND pm.Employee_ID IN (SELECT ID FROM @Filter)
                      )
                   OR EXISTS (
                          SELECT 1
                          FROM   Sys_Users su2
                          WHERE  su2.UserName = p.CreatedBy
                                 AND su2.UserId IN (SELECT Id FROM @Filter)
                      )
               )

    -- DT du kien du an: cung tap du an voi danh sach chi tiet (quyen nguoi tao/thanh vien,
    -- xet thanh vien o cap DU AN chu khong theo tung dong dich vu)
    Select @TotalprojRevenue = ISNULL(SUM(pp.ExpectedRevenue), 0)
           FROM   RM_Project p
                  LEFT JOIN RM_ProductProject pp
                       ON  p.ProjectID = pp.ProjectID
                       AND pp.IsDeleted = 0
           WHERE  p.IsDeleted = 0
                    AND (@SuccessRate = 0 OR @SuccessRate IS NULL OR isNull(p.SuccessRate, 0) >= @SuccessRate )
                  AND (
                          @FromDate IS NULL
                          OR p.StartDate >= CAST(@FromDate AS DATE)
                      )
                  AND (
                          @ToDate IS NULL
                          OR p.StartDate < DATEADD(DAY, 1, CAST(@ToDate AS DATE))
                      )
                  AND (
                          @EmployeeIds IS NULL
                          OR EXISTS (
                                 SELECT 1
                                 FROM   RM_ProductProject pp2
                                        JOIN RM_ProjectMember pm
                                             ON  pm.ProductProjectID = pp2.ProductProjectID
                                 WHERE  pp2.ProjectID = p.ProjectID
                                        AND pp2.IsDeleted = 0
                                        AND pm.IsDeleted = 0
                                        AND pm.Employee_ID IN (SELECT ID
                                                               FROM   @Filter)
                             )
                          OR EXISTS (
                                 SELECT 1
                                 FROM   Sys_Users su2
                                 WHERE  su2.UserName = p.CreatedBy
                                        AND su2.UserId IN (SELECT Id FROM @Filter)
                             )
                      )
    SELECT @TotalProjects = COUNT(DISTINCT p.ProjectID)
           FROM   RM_Project p
                  INNER JOIN Sys_Users AS su
                       ON  su.UserName = p.CreatedBy
           WHERE  p.IsDeleted = 0
                    AND (@SuccessRate = 0 OR @SuccessRate IS NULL OR isNull(p.SuccessRate, 0) >= @SuccessRate )
                  AND (
                          @FromDate IS NULL
                          OR p.StartDate >= CAST(@FromDate AS DATE)
                      )
                  AND (
                          @ToDate IS NULL
                          OR p.StartDate < DATEADD(DAY, 1, CAST(@ToDate AS DATE))
                      )
                  AND (
                          @EmployeeIds IS NULL
                          OR EXISTS (
                                 SELECT 1
                                 FROM   RM_ProductProject pp
                                        JOIN RM_ProjectMember pm
                                             ON  pm.ProductProjectID = pp.ProductProjectID
                                 WHERE  pp.ProjectID = p.ProjectID
                                        AND pp.IsDeleted = 0
                                        AND pm.IsDeleted = 0
                                        AND pm.Employee_ID IN (SELECT ID
                                                               FROM   @Filter)
                             )
                          OR su.UserId IN (SELECT f.Id
                                           FROM   @Filter f)
                      )
    -- So co hoi: dieu kien PHAI GIONG SP danh sach chi tiet (Opportunity_Get_V2):
    --   loai ca LOST, quyen tinh nguoi tao lan thanh vien
    SELECT @TotalOpportunities  = COUNT(DISTINCT b.BusinessOpportunityID)
           FROM   RM_BusinessOpportunity b
                  LEFT JOIN RM_Status s
                       ON  s.ID = b.StatusID
           WHERE  b.IsDeleted = 0
                    AND (@SuccessRate = 0 OR @SuccessRate IS NULL OR isNull(b.ClosingProbability, 0) >= @SuccessRate )
                  AND (
                          @FromDate IS NULL
                          OR b.CreatedDate >= CAST(@FromDate AS DATE)
                      )
                  AND (
                          @ToDate IS NULL
                          OR b.CreatedDate < DATEADD(DAY, 1, CAST(@ToDate AS DATE))
                      )
                  AND (
                          @EmployeeIds IS NULL
                          OR EXISTS (
                                 SELECT 1
                                 FROM   RM_SalesTeamMembers stm
                                        JOIN @Filter f
                                             ON  f.Id = stm.EmployeeID
                                 WHERE  stm.BusinessOpportunityID = b.BusinessOpportunityID
                                        AND stm.IsDeleted = 0
                             )
                          OR EXISTS (
                                 SELECT 1
                                 FROM   Sys_Users su2
                                 WHERE  su2.UserName = b.CreatedBy
                                        AND su2.UserId IN (SELECT Id FROM @Filter)
                             )
                      )
                  AND (
                          s.StatusCode NOT IN ('WON', 'CONVERTED')
                          AND s.StatusKey = 'Opportunity'
                      )
                  AND b.StatusID <> ISNULL(@LostID, -1)
    -- DT du kien cua cac co hoi dang mo: cung dieu kien voi @TotalOpportunities
    SELECT @TotalExpectedValue = ISNULL(SUM(b.ExpectedValue), 0)
           FROM   RM_BusinessOpportunity b
                  LEFT JOIN RM_Status s
                       ON  s.ID = b.StatusID
           WHERE  b.IsDeleted = 0
                    AND (@SuccessRate = 0 OR @SuccessRate IS NULL OR isNull(b.ClosingProbability, 0) >= @SuccessRate )
                  AND (@FromDate IS NULL OR b.CreatedDate >= CAST(@FromDate AS DATE))
                  AND (@ToDate IS NULL OR b.CreatedDate < DATEADD(DAY, 1, CAST(@ToDate AS DATE)))
                  AND (
                          @EmployeeIds IS NULL
                          OR EXISTS (
                                 SELECT 1
                                 FROM   RM_SalesTeamMembers stm
                                        JOIN @Filter f
                                             ON  f.Id = stm.EmployeeID
                                 WHERE  stm.BusinessOpportunityID = b.BusinessOpportunityID
                                        AND stm.IsDeleted = 0
                             )
                          OR EXISTS (
                                 SELECT 1
                                 FROM   Sys_Users su2
                                 WHERE  su2.UserName = b.CreatedBy
                                        AND su2.UserId IN (SELECT Id FROM @Filter)
                             )
                      )
                  AND (
                          s.StatusCode NOT IN ('WON', 'CONVERTED')
                          AND s.StatusKey = 'Opportunity'
                      )
                  AND b.StatusID <> ISNULL(@LostID, -1)

    -- Truoc day khi @SuccessRate <> 0 hai cot doanh thu bi TRAO CHO NHAU khien o
    -- ">= 80%" hien DT thuc hien duoi nhan "DT du kien". Nay o nao cung hien du
    -- ca 2 chi so nen luon tra dung ten cot.
    Select @TotalRevenue 'TotalRevenue'
            ,@TotalprojRevenue 'TotalprojRevenue'
            ,@TotalProjects 'TotalProjects'
            ,@TotalOpportunities 'TotalOpportunities'
            ,@TotalExpectedValue 'TotalExpectedValue'
END
GO

-- ---------- 6. Dong bo ve quyen "nguoi tao" cua 2 SP danh sach chi tiet ----------
-- Summary dem ca ban ghi do nhan su trong pham vi quan ly TAO (khong chi thanh vien),
-- nen popup cung phai liet ke tuong ung: CreatedBy IN @UserTable thay vi = @Username.
IF OBJECT_ID('dbo.RM_Dashboard_Opportunity_Get_V2', 'P') IS NOT NULL
BEGIN
    DECLARE @sqlOpp NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.RM_Dashboard_Opportunity_Get_V2'));
    IF CHARINDEX('b.CreatedBy = @UserName', @sqlOpp) > 0
    BEGIN
        SET @sqlOpp = REPLACE(@sqlOpp, 'CREATE PROCEDURE', 'ALTER PROCEDURE');
        SET @sqlOpp = REPLACE(@sqlOpp, 'b.CreatedBy = @UserName', 'b.CreatedBy IN (SELECT UserName FROM @UserTable)');
        EXEC (@sqlOpp);
        PRINT 'Opportunity_Get_V2: creator scope widened';
    END
END
GO

IF OBJECT_ID('dbo.RM_Dashboard_Project_Get_V2', 'P') IS NOT NULL
BEGIN
    DECLARE @sqlPrj NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.RM_Dashboard_Project_Get_V2'));
    IF CHARINDEX('p.CreatedBy = @UserName', @sqlPrj) > 0
    BEGIN
        SET @sqlPrj = REPLACE(@sqlPrj, 'CREATE PROCEDURE', 'ALTER PROCEDURE');
        SET @sqlPrj = REPLACE(@sqlPrj, 'p.CreatedBy = @UserName', 'p.CreatedBy IN (SELECT UserName FROM @UserTable)');
        EXEC (@sqlPrj);
        PRINT 'Project_Get_V2: creator scope widened';
    END
END
GO

-- Cung mo rong cho 2 SP danh sach theo nhom dich vu (tao o section 3-4 voi @Username)
IF OBJECT_ID('dbo.RM_Dashboard_Opportunity_GetByGroupService', 'P') IS NOT NULL
BEGIN
    DECLARE @sqlOppGs NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.RM_Dashboard_Opportunity_GetByGroupService'));
    IF CHARINDEX('b.CreatedBy = @Username', @sqlOppGs) > 0
    BEGIN
        SET @sqlOppGs = REPLACE(@sqlOppGs, 'CREATE PROCEDURE', 'ALTER PROCEDURE');
        SET @sqlOppGs = REPLACE(@sqlOppGs, 'b.CreatedBy = @Username', 'b.CreatedBy IN (SELECT UserName FROM @UserTable)');
        EXEC (@sqlOppGs);
        PRINT 'Opportunity_GetByGroupService: creator scope widened';
    END
END
GO

IF OBJECT_ID('dbo.RM_Dashboard_Project_GetByGroupService', 'P') IS NOT NULL
BEGIN
    DECLARE @sqlPrjGs NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.RM_Dashboard_Project_GetByGroupService'));
    IF CHARINDEX('p.CreatedBy = @Username', @sqlPrjGs) > 0
    BEGIN
        SET @sqlPrjGs = REPLACE(@sqlPrjGs, 'CREATE PROCEDURE', 'ALTER PROCEDURE');
        SET @sqlPrjGs = REPLACE(@sqlPrjGs, 'p.CreatedBy = @Username', 'p.CreatedBy IN (SELECT UserName FROM @UserTable)');
        EXEC (@sqlPrjGs);
        PRINT 'Project_GetByGroupService: creator scope widened';
    END
END
GO

-- Chart co hoi theo nhom dich vu (section 1) dung "o.CreatedBy = @Username" voi @Filter:
-- mo rong tuong tu de khop popup
IF OBJECT_ID('dbo.RM_Dashboard_GetOpportunityByGroupService', 'P') IS NOT NULL
BEGIN
    DECLARE @sqlChartOpp NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.RM_Dashboard_GetOpportunityByGroupService'));
    IF CHARINDEX('o.CreatedBy = @Username', @sqlChartOpp) > 0
    BEGIN
        SET @sqlChartOpp = REPLACE(@sqlChartOpp, 'CREATE PROCEDURE', 'ALTER PROCEDURE');
        SET @sqlChartOpp = REPLACE(@sqlChartOpp, 'o.CreatedBy = @Username',
            'EXISTS (SELECT 1 FROM Sys_Users suc WHERE suc.UserName = o.CreatedBy AND suc.UserId IN (SELECT Id FROM @Filter))');
        EXEC (@sqlChartOpp);
        PRINT 'Chart GetOpportunityByGroupService: creator scope widened';
    END
END
GO

IF OBJECT_ID('dbo.RM_Dashboard_GetProjectByGroupService', 'P') IS NOT NULL
BEGIN
    DECLARE @sqlChartPrj NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('dbo.RM_Dashboard_GetProjectByGroupService'));
    IF CHARINDEX('p.CreatedBy = @Username', @sqlChartPrj) > 0
    BEGIN
        SET @sqlChartPrj = REPLACE(@sqlChartPrj, 'CREATE PROCEDURE', 'ALTER PROCEDURE');
        SET @sqlChartPrj = REPLACE(@sqlChartPrj, 'p.CreatedBy = @Username',
            'EXISTS (SELECT 1 FROM Sys_Users suc WHERE suc.UserName = p.CreatedBy AND suc.UserId IN (SELECT Id FROM @Filter))');
        EXEC (@sqlChartPrj);
        PRINT 'Chart GetProjectByGroupService: creator scope widened';
    END
END
GO

PRINT 'Dashboard_Redesign.sql completed';
