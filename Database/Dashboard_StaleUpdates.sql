SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.RM_Dashboard_StaleUpdates_Get', N'P') IS NULL
    EXEC(N'CREATE PROCEDURE dbo.RM_Dashboard_StaleUpdates_Get AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.RM_Dashboard_StaleUpdates_Get
    @AsOfDate DATE = NULL,
    @EmployeeIds NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Giữ tham số ngày để tương thích với DLL đang chạy nhưng luôn dùng ngày DB;
    -- EmployeeIds được dùng để giới hạn dữ liệu theo quyền Dashboard.
    SET @AsOfDate = CAST(GETDATE() AS DATE);

    DECLARE @Filter TABLE (Id INT PRIMARY KEY);
    IF @EmployeeIds IS NOT NULL
        INSERT INTO @Filter (Id)
        SELECT DISTINCT Id FROM dbo.fn_SplitIntList(@EmployeeIds);

    ;WITH ProjectActivities AS
    (
        SELECT tm.ProjectID, c.CreatedDate AS ActivityDate
        FROM RM_Comment c
        INNER JOIN RM_TaskManagement tm ON tm.TaskManagementID = c.TaskManagementID
        WHERE c.IsDeleted = 0 AND tm.IsDeleted = 0

        UNION ALL

        SELECT tm.ProjectID, tm.CreatedDate
        FROM RM_TaskManagement tm
        WHERE tm.IsDeleted = 0 AND ISNULL(tm.Description, '') <> ''

    ),
    ProjectLatest AS
    (
        SELECT ProjectID, MAX(ActivityDate) AS LastUpdateDate
        FROM ProjectActivities
        GROUP BY ProjectID
    ),
    ProjectMembers AS
    (
        SELECT p.ProjectID,
               STUFF((
                   SELECT DISTINCT ', ' + u.FullName
                   FROM RM_ProductProject pp
                   INNER JOIN RM_ProjectMember pm ON pm.ProductProjectID = pp.ProductProjectID AND pm.IsDeleted = 0
                   INNER JOIN Sys_Users u ON u.UserId = pm.Employee_ID
                   WHERE pp.ProjectID = p.ProjectID AND pp.IsDeleted = 0
                   FOR XML PATH(''), TYPE
               ).value('.', 'nvarchar(max)'), 1, 2, '') AS AMNames
        FROM RM_Project p
        WHERE p.IsDeleted = 0
          AND (
              @EmployeeIds IS NULL
              OR EXISTS (
                  SELECT 1
                  FROM RM_ProductProject pp
                  INNER JOIN RM_ProjectMember member
                      ON member.ProductProjectID = pp.ProductProjectID
                      AND member.IsDeleted = 0
                  INNER JOIN @Filter f ON f.Id = member.Employee_ID
                  WHERE pp.ProjectID = p.ProjectID
                    AND pp.IsDeleted = 0
              )
          )
    ),
    OpportunityLatest AS
    (
        SELECT BusinessOpportunityID, MAX(ExchangeDate) AS LastUpdateDate
        FROM RM_ExchangeHistory
        WHERE IsDeleted = 0
        GROUP BY BusinessOpportunityID
    ),
    OpportunityMembers AS
    (
        SELECT bo.BusinessOpportunityID,
               STUFF((
                   SELECT DISTINCT ', ' + u.FullName
                   FROM RM_SalesTeamMembers sm
                   INNER JOIN Sys_Users u ON u.UserId = sm.EmployeeID
                   WHERE sm.BusinessOpportunityID = bo.BusinessOpportunityID AND sm.IsDeleted = 0
                   FOR XML PATH(''), TYPE
               ).value('.', 'nvarchar(max)'), 1, 2, '') AS AMNames
        FROM RM_BusinessOpportunity bo
        WHERE bo.IsDeleted = 0
          AND (
              @EmployeeIds IS NULL
              OR EXISTS (
                  SELECT 1
                  FROM RM_SalesTeamMembers sm
                  INNER JOIN @Filter f ON f.Id = sm.EmployeeID
                  WHERE sm.BusinessOpportunityID = bo.BusinessOpportunityID
                    AND sm.IsDeleted = 0
              )
          )
    ),
    StaleItems AS
    (
        SELECT
            1 AS Type,
            p.ProjectID AS ObjectID,
            CAST(NULL AS NVARCHAR(100)) AS ObjectCode,
            p.ProjectName AS ObjectName,
            c.CustomerName,
            ISNULL(pm.AMNames, '') AS AMNames,
            s.StatusName,
            s.StatusClass,
            CAST(COALESCE(pl.LastUpdateDate, p.CreatedDate) AS DATETIME) AS LastUpdateDate
        FROM RM_Project p
        LEFT JOIN RM_Customer c ON c.CustomerID = p.CustomerID AND c.IsDeleted = 0
        LEFT JOIN RM_Status s ON s.ID = p.Status AND s.IsDeleted = 0
        LEFT JOIN ProjectLatest pl ON pl.ProjectID = p.ProjectID
        LEFT JOIN ProjectMembers pm ON pm.ProjectID = p.ProjectID
        WHERE p.IsDeleted = 0
          AND (
              @EmployeeIds IS NULL
              OR EXISTS (
                  SELECT 1
                  FROM RM_ProductProject pp
                  INNER JOIN RM_ProjectMember member
                      ON member.ProductProjectID = pp.ProductProjectID
                      AND member.IsDeleted = 0
                  INNER JOIN @Filter f ON f.Id = member.Employee_ID
                  WHERE pp.ProjectID = p.ProjectID
                    AND pp.IsDeleted = 0
              )
          )

        UNION ALL

        SELECT
            2 AS Type,
            bo.BusinessOpportunityID AS ObjectID,
            CAST(bo.CodeOpportunity AS NVARCHAR(100)) AS ObjectCode,
            bo.OpportunityName AS ObjectName,
            c.CustomerName,
            ISNULL(om.AMNames, '') AS AMNames,
            s.StatusName,
            s.StatusClass,
            CAST(COALESCE(ol.LastUpdateDate, bo.CreatedDate) AS DATETIME) AS LastUpdateDate
        FROM RM_BusinessOpportunity bo
        LEFT JOIN RM_Customer c ON c.CustomerID = bo.CustomerID AND c.IsDeleted = 0
        LEFT JOIN RM_Status s ON s.ID = bo.StatusID AND s.IsDeleted = 0
        LEFT JOIN OpportunityLatest ol ON ol.BusinessOpportunityID = bo.BusinessOpportunityID
        LEFT JOIN OpportunityMembers om ON om.BusinessOpportunityID = bo.BusinessOpportunityID
        WHERE bo.IsDeleted = 0
          AND (
              @EmployeeIds IS NULL
              OR EXISTS (
                  SELECT 1
                  FROM RM_SalesTeamMembers sm
                  INNER JOIN @Filter f ON f.Id = sm.EmployeeID
                  WHERE sm.BusinessOpportunityID = bo.BusinessOpportunityID
                    AND sm.IsDeleted = 0
              )
          )
    )
    SELECT
        Type,
        ObjectID,
        ObjectCode,
        ObjectName,
        CustomerName,
        AMNames,
        StatusName,
        StatusClass,
        LastUpdateDate,
        DATEDIFF(DAY, CAST(LastUpdateDate AS DATE), @AsOfDate) AS DaysWithoutUpdate
    FROM StaleItems
    WHERE LastUpdateDate IS NOT NULL
      AND DATEDIFF(DAY, CAST(LastUpdateDate AS DATE), @AsOfDate) >= 3
    ORDER BY DaysWithoutUpdate DESC, LastUpdateDate, ObjectName;
END;
GO
