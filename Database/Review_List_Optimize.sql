-- ============================================================
-- Toi uu 2 SP danh sach man hinh Ra soat dinh ky (ReviewBatchItem)
--   RM_Review_GetProject / RM_Review_GetBusinessOpportunity
--
-- Toi uu:
--   1. TotalRow dung COUNT(1) OVER() ngay trong CTE -> CTE chi thuc thi 1 lan
--      (ban cu dat (SELECT COUNT(1) FROM T) ben ngoai lam CTE chay 2 lan)
--   2. Chuoi ten AM (FOR XML) chi build cho cac dong cua TRANG hien tai
--      sau khi phan trang, thay vi cho toan bo tap ket qua
--   3. Sort theo AM dung MIN(FullName) - chi tinh khi nguoi dung sort cot AM
--
-- Bo sung cot "Thong tin ra soat trong dot" (tu RM_ReviewHistory):
--   LastReviewDate    : thoi gian luot ra soat moi nhat trong dot
--   LastReviewerName  : nguoi thuc hien (ho ten, fallback username)
--   LastReviewComment : noi dung (da bo the HTML)
--   ReviewCount       : tong so luot ra soat trong dot
--
-- Dieu kien loc/quyen/phan cap GIU NGUYEN 100% so voi ban cu.
-- Script idempotent - chay lai nhieu lan khong loi
-- ============================================================
SET NOCOUNT ON;
GO

-- ---------- 1. RM_Review_GetProject ----------
IF OBJECT_ID('dbo.RM_Review_GetProject', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Review_GetProject;
GO
CREATE PROCEDURE [dbo].[RM_Review_GetProject]
(
    @ReviewBatchID  INT,
    @DepartmentID   INT,
    @EmployeeID     INT,
    @Status INT = NULL,
    @IsReviewed BIT = NULL,
    @Search NVARCHAR(250),
    @Order VARCHAR(3),
    @OrderDir VARCHAR(10),
    @PageIndex INT,
    @PageSize INT,
    @UserName VARCHAR(150)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ReviewLevel INT;
    SELECT @ReviewLevel = ReviewLevel FROM Sys_Users WHERE UserName = @UserName;

    DECLARE @UserTable TABLE (ID INT, UserName VARCHAR(150));
    INSERT INTO @UserTable (ID, UserName)
    EXEC Sys_User_GetByReviewDepartment @UserName;

    SET @Order = ISNULL(@Order, '0');
    SET @OrderDir = ISNULL(@OrderDir, 'DESC');
    SET @PageIndex = ISNULL(@PageIndex, 0);
    SET @PageSize = ISNULL(@PageSize, 10);
    SET @Search = ISNULL(RTRIM(LTRIM(@Search)), '');

    ;WITH DepartmentTree AS
    (
        SELECT bp.BoPhan_ID, bp.MaBoPhan
        FROM MN_BoPhan bp
        WHERE bp.BoPhan_ID = @DepartmentID

        UNION ALL

        SELECT c.BoPhan_ID, c.MaBoPhan
        FROM MN_BoPhan c
        JOIN DepartmentTree p ON c.BoPhanCha_ID = p.BoPhan_ID
    ),
    T AS
    (
        SELECT
            ROW_NUMBER() OVER
            (
                ORDER BY
                CASE WHEN @Order = '0' AND UPPER(@OrderDir) = 'ASC'  THEN p.ProjectName END ASC,
                CASE WHEN @Order = '0' AND UPPER(@OrderDir) = 'DESC' THEN p.ProjectName END DESC,
                CASE WHEN @Order = '1' AND UPPER(@OrderDir) = 'ASC'  THEN p.CreatedDate END ASC,
                CASE WHEN @Order = '1' AND UPPER(@OrderDir) = 'DESC' THEN p.CreatedDate END DESC,
                CASE WHEN @Order = '2' AND UPPER(@OrderDir) = 'ASC'  THEN c.CustomerName END ASC,
                CASE WHEN @Order = '2' AND UPPER(@OrderDir) = 'DESC' THEN c.CustomerName END DESC,
                CASE WHEN @Order = '3' AND UPPER(@OrderDir) = 'ASC'  THEN p.StartDate END ASC,
                CASE WHEN @Order = '3' AND UPPER(@OrderDir) = 'DESC' THEN p.StartDate END DESC,
                -- Sort theo AM: chi tinh ten AM dau tien khi thuc su sort cot nay
                CASE WHEN @Order = '4' AND UPPER(@OrderDir) = 'ASC' THEN
                    (SELECT MIN(su.FullName)
                     FROM RM_ProductProject pp2
                     JOIN RM_ProjectMember pm2 ON pm2.ProductProjectID = pp2.ProductProjectID
                     JOIN Sys_Users su ON su.UserId = pm2.Employee_ID
                     WHERE pp2.ProjectID = p.ProjectID AND pp2.IsDeleted = 0 AND pm2.IsDeleted = 0
                       AND (';' + ISNULL(pm2.RoleID, '') + ';' LIKE '%;5;%')) END ASC,
                CASE WHEN @Order = '4' AND UPPER(@OrderDir) = 'DESC' THEN
                    (SELECT MIN(su.FullName)
                     FROM RM_ProductProject pp2
                     JOIN RM_ProjectMember pm2 ON pm2.ProductProjectID = pp2.ProductProjectID
                     JOIN Sys_Users su ON su.UserId = pm2.Employee_ID
                     WHERE pp2.ProjectID = p.ProjectID AND pp2.IsDeleted = 0 AND pm2.IsDeleted = 0
                       AND (';' + ISNULL(pm2.RoleID, '') + ';' LIKE '%;5;%')) END DESC,
                p.ProjectID DESC
            ) AS RowIndex,

            COUNT(1) OVER () AS TotalRow,

            p.ProjectID,
            p.ProjectName,
            p.StartDate,
            c.CustomerName,
            s.StatusName,
            s.StatusClass,
            p.CreatedDate,
            i.ReviewBatchItemID,
            i.HighestReviewedLevel,
            i.LastReviewedDate,
            CAST(ISNULL(i.IsCompleted, 0) AS BIT) AS IsReviewed

        FROM RM_Project p

        LEFT JOIN RM_ReviewBatchItem i
            ON i.ObjectType = 2
            AND i.ObjectID = p.ProjectID
            AND i.ReviewBatchID = @ReviewBatchID
            AND ISNULL(i.IsDeleted, 0) = 0

        LEFT JOIN RM_Status s ON s.ID = p.Status
        LEFT JOIN RM_Customer c ON c.CustomerID = p.CustomerID

        WHERE
            ISNULL(p.IsDeleted, 0) = 0
            AND ISNULL(p.Status, 0) NOT IN (18, 80, 83)

            -- REVIEW LEVEL RULE
            AND
            (
                @ReviewLevel = 2
                OR (@ReviewLevel = 3 AND ISNULL(i.HighestReviewedLevel, 0) <> 2)
                OR (@ReviewLevel = 4 AND ISNULL(i.HighestReviewedLevel, 0) NOT IN (2,3))
            )

            -- PERMISSION
            AND
            (
                p.CreatedBy = @UserName
                OR EXISTS
                (
                    SELECT 1
                    FROM RM_ProductProject pp
                    JOIN RM_ProjectMember pm ON pm.ProductProjectID = pp.ProductProjectID
                    WHERE pp.ProjectID = p.ProjectID
                        AND pp.IsDeleted = 0
                        AND pm.IsDeleted = 0
                        AND pm.Employee_ID IN (SELECT ID FROM @UserTable)
                )
            )

            -- SEARCH
            AND (@Search = '' OR p.ProjectName LIKE '%' + @Search + '%')

            -- DEPARTMENT
            AND
            (
                @DepartmentID IS NULL
                OR @DepartmentID = 0
                OR EXISTS
                (
                    SELECT 1
                    FROM RM_ProductProject pp
                    JOIN RM_ProjectMember pm ON pm.ProductProjectID = pp.ProductProjectID
                    JOIN Sys_Users su ON su.UserId = pm.Employee_ID
                    WHERE pp.ProjectID = p.ProjectID
                        AND pm.IsDeleted = 0
                        AND su.MaBoPhan IN (SELECT MaBoPhan FROM DepartmentTree)
                )
            )

            -- EMPLOYEE
            AND
            (
                @EmployeeID IS NULL
                OR @EmployeeID = 0
                OR EXISTS
                (
                    SELECT 1
                    FROM RM_ProductProject pp
                    LEFT JOIN RM_ProjectMember pm ON pp.ProductProjectID = pm.ProductProjectID
                    WHERE pp.ProjectID = p.ProjectID
                        AND pp.IsDeleted = 0
                        AND pm.Employee_ID = @EmployeeID
                )
            )

            -- STATUS
            AND (@Status IS NULL OR p.Status = @Status)

            -- REVIEWED
            AND
            (
                @IsReviewed IS NULL
                OR
                (
                    @IsReviewed = 1
                    AND
                    (
                        (@ReviewLevel = 2 AND ISNULL(i.HighestReviewedLevel, 0) = 2)
                        OR (@ReviewLevel = 3 AND ISNULL(i.HighestReviewedLevel, 0) IN (2,3))
                        OR (@ReviewLevel = 4 AND ISNULL(i.HighestReviewedLevel, 0) IN (2,3,4))
                    )
                )
                OR
                (
                    @IsReviewed = 0
                    AND
                    (
                        (@ReviewLevel = 2 AND ISNULL(i.HighestReviewedLevel, 0) <> 2)
                        OR (@ReviewLevel = 3 AND ISNULL(i.HighestReviewedLevel, 0) NOT IN (2,3))
                        OR (@ReviewLevel = 4 AND ISNULL(i.HighestReviewedLevel, 0) NOT IN (2,3,4))
                    )
                )
            )
    )

    -- Chi build chuoi AM + thong tin ra soat cho cac dong cua TRANG hien tai
    SELECT
        T.*,
        AM.FullName AS AMName,
        rv.LastReviewDate,
        rv.LastReviewerName,
        rv.LastReviewComment,
        ISNULL(rv.ReviewCount, 0) AS ReviewCount
    FROM T

    OUTER APPLY
    (
        SELECT STUFF(
            (SELECT DISTINCT ', ' + su.FullName
             FROM RM_ProductProject pp2
             JOIN RM_ProjectMember pm2 ON pm2.ProductProjectID = pp2.ProductProjectID
             JOIN Sys_Users su ON su.UserId = pm2.Employee_ID
             WHERE pp2.ProjectID = T.ProjectID
                   AND pp2.IsDeleted = 0
                   AND pm2.IsDeleted = 0
                   AND (';' + ISNULL(pm2.RoleID, '') + ';' LIKE '%;5;%')
             FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS FullName
    ) AM

    -- Thong tin ra soat trong dot: luot moi nhat + tong so luot
    OUTER APPLY
    (
        SELECT TOP 1
               rh.CreatedDate AS LastReviewDate,
               ISNULL(NULLIF(su.FullName, ''), rh.CreatedBy) AS LastReviewerName,
               dbo.fn_DecodeHtmlEntities(dbo.fn_StripHtml(ISNULL(rh.ReviewComment, ''))) AS LastReviewComment,
               (SELECT COUNT(1)
                FROM RM_ReviewHistory rh2
                WHERE rh2.ReviewBatchItemID = T.ReviewBatchItemID
                  AND ISNULL(rh2.IsDeleted, 0) = 0) AS ReviewCount
        FROM RM_ReviewHistory rh
        LEFT JOIN Sys_Users su ON su.UserName = rh.CreatedBy
        WHERE rh.ReviewBatchItemID = T.ReviewBatchItemID
          AND ISNULL(rh.IsDeleted, 0) = 0
        ORDER BY rh.CreatedDate DESC, rh.ReviewHistoryID DESC
    ) rv

    WHERE
    (
        @PageSize > 0
        AND T.RowIndex BETWEEN @PageIndex + 1 AND @PageIndex + @PageSize
    )
    OR @PageSize <= 0

    ORDER BY T.RowIndex;
END
GO

-- ---------- 2. RM_Review_GetBusinessOpportunity ----------
IF OBJECT_ID('dbo.RM_Review_GetBusinessOpportunity', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Review_GetBusinessOpportunity;
GO
CREATE PROCEDURE [dbo].[RM_Review_GetBusinessOpportunity]
(
    @ReviewBatchID INT,
    @DepartmentID   INT,
    @EmployeeID     INT,
    @StatusID INT = NULL,
    @IsReviewed BIT = NULL,
    @Search NVARCHAR(250),
    @Order VARCHAR(3),
    @OrderDir VARCHAR(10),
    @PageIndex INT,
    @PageSize INT,
    @UserName VARCHAR(150)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ReviewLevel INT;
    SELECT @ReviewLevel = ReviewLevel FROM Sys_Users WHERE UserName = @UserName;

    DECLARE @UserTable TABLE (ID INT, UserName VARCHAR(150));
    INSERT INTO @UserTable (ID, UserName)
    EXEC Sys_User_GetByReviewDepartment @UserName;

    SET @Order = ISNULL(@Order, '0');
    SET @OrderDir = ISNULL(@OrderDir, 'DESC');
    SET @PageIndex = ISNULL(@PageIndex, 0);
    SET @PageSize = ISNULL(@PageSize, 10);
    SET @Search = ISNULL(RTRIM(LTRIM(@Search)), '');

    ;WITH DepartmentTree AS
    (
        SELECT bp.BoPhan_ID, bp.MaBoPhan
        FROM MN_BoPhan bp
        WHERE bp.BoPhan_ID = @DepartmentID

        UNION ALL

        SELECT c.BoPhan_ID, c.MaBoPhan
        FROM MN_BoPhan c
        JOIN DepartmentTree p ON c.BoPhanCha_ID = p.BoPhan_ID
    ),
    T AS
    (
        SELECT
            ROW_NUMBER() OVER
            (
                ORDER BY
                CASE WHEN @Order = '0' AND UPPER(@OrderDir) = 'ASC'  THEN bo.CodeOpportunity END ASC,
                CASE WHEN @Order = '0' AND UPPER(@OrderDir) = 'DESC' THEN bo.CodeOpportunity END DESC,
                CASE WHEN @Order = '1' AND UPPER(@OrderDir) = 'ASC'  THEN bo.OpportunityName END ASC,
                CASE WHEN @Order = '1' AND UPPER(@OrderDir) = 'DESC' THEN bo.OpportunityName END DESC,
                CASE WHEN @Order = '2' AND UPPER(@OrderDir) = 'ASC'  THEN bo.CreatedDate END ASC,
                CASE WHEN @Order = '2' AND UPPER(@OrderDir) = 'DESC' THEN bo.CreatedDate END DESC,
                CASE WHEN @Order = '4' AND UPPER(@OrderDir) = 'ASC'  THEN bo.ClosingProbability END ASC,
                CASE WHEN @Order = '4' AND UPPER(@OrderDir) = 'DESC' THEN bo.ClosingProbability END DESC,
                bo.BusinessOpportunityID DESC
            ) AS RowIndex,

            COUNT(1) OVER () AS TotalRow,

            bo.BusinessOpportunityID,
            bo.CodeOpportunity,
            bo.OpportunityName,
            c.CustomerName,
            s.StatusName,
            s.StatusClass,
            bo.ExpectedValue,
            bo.ClosingProbability,
            bo.CreatedDate,
            i.ReviewBatchItemID,
            i.HighestReviewedLevel,
            i.LastReviewedDate,
            CAST(ISNULL(i.IsCompleted, 0) AS BIT) AS IsReviewed

        FROM RM_BusinessOpportunity bo

        LEFT JOIN RM_ReviewBatchItem i
            ON i.ObjectType = 1
            AND i.ObjectID = bo.BusinessOpportunityID
            AND i.ReviewBatchID = @ReviewBatchID
            AND ISNULL(i.IsDeleted, 0) = 0

        LEFT JOIN RM_Status s ON s.ID = bo.StatusID
        LEFT JOIN RM_Customer c ON c.CustomerID = bo.CustomerID

        WHERE
            ISNULL(bo.IsDeleted, 0) = 0
            AND ISNULL(bo.StatusID, 0) NOT IN (15, 36)

            -- REVIEW LEVEL RULE
            AND
            (
                @ReviewLevel = 2
                OR (@ReviewLevel = 3 AND ISNULL(i.HighestReviewedLevel, 0) <> 2)
                OR (@ReviewLevel = 4 AND ISNULL(i.HighestReviewedLevel, 0) NOT IN (2,3))
            )

            -- PERMISSION
            AND
            (
                bo.CreatedBy = @UserName
                OR EXISTS
                (
                    SELECT 1
                    FROM RM_SalesTeamMembers stm
                    WHERE stm.BusinessOpportunityID = bo.BusinessOpportunityID
                        AND stm.IsDeleted = 0
                        AND stm.EmployeeID IN (SELECT ID FROM @UserTable)
                )
            )

            -- SEARCH
            AND
            (
                @Search = ''
                OR bo.CodeOpportunity LIKE '%' + @Search + '%'
                OR bo.OpportunityName LIKE '%' + @Search + '%'
            )

            -- DEPARTMENT
            AND
            (
                @DepartmentID IS NULL
                OR @DepartmentID = 0
                OR EXISTS
                (
                    SELECT 1
                    FROM RM_SalesTeamMembers stm
                    JOIN Sys_Users su ON stm.EmployeeID = su.UserId
                    WHERE stm.BusinessOpportunityID = bo.BusinessOpportunityID
                        AND stm.IsDeleted = 0
                        AND su.MaBoPhan IN (SELECT MaBoPhan FROM DepartmentTree)
                )
            )

            -- EMPLOYEE
            AND
            (
                @EmployeeID IS NULL
                OR @EmployeeID = 0
                OR EXISTS
                (
                    SELECT 1
                    FROM RM_SalesTeamMembers stm
                    WHERE stm.BusinessOpportunityID = bo.BusinessOpportunityID
                        AND stm.IsDeleted = 0
                        AND stm.EmployeeID = @EmployeeID
                )
            )

            -- STATUS
            AND (@StatusID IS NULL OR bo.StatusID = @StatusID)

            -- REVIEWED
            AND
            (
                @IsReviewed IS NULL
                OR
                (
                    @IsReviewed = 1
                    AND
                    (
                        (@ReviewLevel = 2 AND ISNULL(i.HighestReviewedLevel, 0) = 2)
                        OR (@ReviewLevel = 3 AND ISNULL(i.HighestReviewedLevel, 0) IN (2,3))
                        OR (@ReviewLevel = 4 AND ISNULL(i.HighestReviewedLevel, 0) IN (2,3,4))
                    )
                )
                OR
                (
                    @IsReviewed = 0
                    AND ISNULL(i.HighestReviewedLevel, 0) = 0
                )
            )
    )

    -- Chi build chuoi AM + thong tin ra soat cho cac dong cua TRANG hien tai
    SELECT
        T.*,
        AM.FullName AS AMName,
        rv.LastReviewDate,
        rv.LastReviewerName,
        rv.LastReviewComment,
        ISNULL(rv.ReviewCount, 0) AS ReviewCount
    FROM T

    OUTER APPLY
    (
        SELECT STUFF(
            (SELECT DISTINCT ', ' + su.FullName
             FROM RM_SalesTeamMembers st
             JOIN Sys_Users su ON su.UserId = st.EmployeeID
             WHERE st.BusinessOpportunityID = T.BusinessOpportunityID
                   AND st.IsDeleted = 0
                   AND (';' + ISNULL(st.RoleID, '') + ';' LIKE '%;5;%')
             FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS FullName
    ) AM

    -- Thong tin ra soat trong dot: luot moi nhat + tong so luot
    OUTER APPLY
    (
        SELECT TOP 1
               rh.CreatedDate AS LastReviewDate,
               ISNULL(NULLIF(su.FullName, ''), rh.CreatedBy) AS LastReviewerName,
               dbo.fn_DecodeHtmlEntities(dbo.fn_StripHtml(ISNULL(rh.ReviewComment, ''))) AS LastReviewComment,
               (SELECT COUNT(1)
                FROM RM_ReviewHistory rh2
                WHERE rh2.ReviewBatchItemID = T.ReviewBatchItemID
                  AND ISNULL(rh2.IsDeleted, 0) = 0) AS ReviewCount
        FROM RM_ReviewHistory rh
        LEFT JOIN Sys_Users su ON su.UserName = rh.CreatedBy
        WHERE rh.ReviewBatchItemID = T.ReviewBatchItemID
          AND ISNULL(rh.IsDeleted, 0) = 0
        ORDER BY rh.CreatedDate DESC, rh.ReviewHistoryID DESC
    ) rv

    WHERE
    (
        @PageSize > 0
        AND T.RowIndex BETWEEN @PageIndex + 1 AND @PageIndex + @PageSize
    )
    OR @PageSize <= 0

    ORDER BY T.RowIndex;
END
GO

-- ---------- 3. Index ho tro tra cuu lich su ra soat theo item ----------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RM_ReviewHistory_Item' AND object_id = OBJECT_ID('dbo.RM_ReviewHistory'))
    CREATE NONCLUSTERED INDEX IX_RM_ReviewHistory_Item
        ON dbo.RM_ReviewHistory (ReviewBatchItemID, IsDeleted) INCLUDE (CreatedBy, CreatedDate, ReviewComment);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RM_ReviewBatchItem_Object' AND object_id = OBJECT_ID('dbo.RM_ReviewBatchItem'))
    CREATE NONCLUSTERED INDEX IX_RM_ReviewBatchItem_Object
        ON dbo.RM_ReviewBatchItem (ReviewBatchID, ObjectType, ObjectID) INCLUDE (HighestReviewedLevel, LastReviewedDate, IsCompleted, IsDeleted);
GO

PRINT 'Review_List_Optimize.sql completed';
