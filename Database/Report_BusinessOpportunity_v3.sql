-- ============================================================
-- Bao cao tong hop co hoi / du an - phien ban 3
-- Bo sung so voi _v2:
--   1. Tra ve ObjectID + DataType de dieu huong sang trang chi tiet
--   2. Them 4 bo loc: LoaiKhachHang, SanPhamDichVu, LoaiDuAn, TrangThai
--   3. Sua loi ghep ten SPDV cua co hoi: ProductServiceIDs dung dau ';'
--      nhung _v2 tim theo dau ',' nen co hoi co nhieu dich vu bi trong
-- Script idempotent - chay lai nhieu lan khong loi
-- ============================================================
SET NOCOUNT ON;
GO

IF OBJECT_ID('dbo.RM_Report_BusinessOpportunity_v3', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Report_BusinessOpportunity_v3;
GO

CREATE PROCEDURE dbo.RM_Report_BusinessOpportunity_v3
    @TuKhoa           NVARCHAR(500) = NULL,
    @Nam              INT           = NULL,
    @Loai             VARCHAR(100)  = NULL,
    @CustomerTypeID   INT           = NULL,
    @ProductServiceID INT           = NULL,
    @ProjectTypeID    INT           = NULL,
    @StatusID         INT           = NULL,
    @Search           NVARCHAR(250) = NULL,
    @Order            VARCHAR(3)    = NULL,
    @OrderDir         VARCHAR(10)   = NULL,
    @PageIndex        INT           = NULL,
    @PageSize         INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @Order     = ISNULL(@Order, '0');
    SET @OrderDir  = ISNULL(@OrderDir, 'ASC');
    SET @PageIndex = ISNULL(@PageIndex, 0);
    SET @PageSize  = ISNULL(@PageSize, 10);

    -- Coi 0 nhu khong loc de tuong thich voi dropdown gui ve chuoi rong
    IF @CustomerTypeID   = 0 SET @CustomerTypeID   = NULL;
    IF @ProductServiceID = 0 SET @ProductServiceID = NULL;
    IF @ProjectTypeID    = 0 SET @ProjectTypeID    = NULL;
    IF @StatusID         = 0 SET @StatusID         = NULL;

    ;WITH CTE_RESULT AS (
        /* =========================================
           DU AN
        ========================================= */
        SELECT N'Project' AS DataType,
               rp.ProjectID AS ObjectID,
               rc.CustomerName,
               rct.CustomerTypeName,
               rcs.StatusName AS CustomerGroupName,
               STUFF((SELECT N', ' + rps2.NameProduct
                      FROM RM_ProductProject rpp2
                      LEFT JOIN RM_ProductService rps2 ON rpp2.ProductServiceID = rps2.ProductServiceID
                      WHERE rpp2.ProjectID = rp.ProjectID
                        AND rpp2.IsDeleted = 0
                      FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS ProductServiceNames,
               rp.ProjectName,
               rpsType.StatusName AS ProjectTypeName,
               rs.StatusName AS OpportunityStatusName,
               ISNULL(STUFF((SELECT DISTINCT N', ' + ISNULL(su.FullName, '')
                                    + CASE WHEN cv.TenChucVu  IS NOT NULL THEN N' - ' + cv.TenChucVu  ELSE N'' END
                                    + CASE WHEN bp.TenBoPhan  IS NOT NULL THEN N' - ' + bp.TenBoPhan  ELSE N'' END
                                    + CASE WHEN su.Phone      IS NOT NULL THEN N' - ' + su.Phone      ELSE N'' END
                             FROM RM_ProductProject rpp2
                             INNER JOIN RM_ProjectMember rpm ON rpp2.ProductProjectID = rpm.ProductProjectID AND rpm.IsDeleted = 0
                             INNER JOIN Sys_Users su ON su.UserId = rpm.Employee_ID
                             LEFT JOIN MN_ChucVu cv ON su.MaChucVu = cv.MaChucVu
                             LEFT JOIN MN_BoPhan bp ON su.MaBoPhan = bp.MaBoPhan
                             WHERE rpp2.ProjectID = rp.ProjectID
                               AND (';' + ISNULL(rpm.RoleID, '') + ';' LIKE '%;5;%')
                             FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),
                      ISNULL(u.FullName, '') + N' - ' + cv.TenChucVu + N' - ' + bp.TenBoPhan + N' - ' + u.Phone) AS ContactPersonInfo,
               SUM(ISNULL(rpp.ExpectedRevenue * 1000000, 0)) AS TotalExpectedValue,
               SUM(ISNULL(rrr.Amount, 0)) AS TotalVNPTValue,
               rp.ExecutionTime,
               dbo.fn_DecodeHtmlEntities(dbo.fn_StripHTML(ISNULL(rp.Note, ''))) AS Note
        FROM RM_Project rp
        LEFT JOIN RM_ProductProject rpp ON rp.ProjectID = rpp.ProjectID AND rpp.IsDeleted = 0
        LEFT JOIN RM_RevenueReceived rrr ON rpp.ProductProjectID = rrr.ProductProjectID AND rrr.IsDeleted = 0
        LEFT JOIN RM_Status rs ON rp.Status = rs.ID
        LEFT JOIN RM_Status rpsType ON rp.ProjectTypeID = rpsType.ID
        LEFT JOIN RM_Customer rc ON rp.CustomerID = rc.CustomerID
        LEFT JOIN RM_CustomerType rct ON rc.CustomerTypeID = rct.CustomerTypeID
        LEFT JOIN RM_Status rcs ON rc.CustomerStatusID = rcs.ID
        LEFT JOIN Sys_Users u ON u.UserName = rp.CreatedBy
        LEFT JOIN MN_ChucVu cv ON u.MaChucVu = cv.MaChucVu
        LEFT JOIN MN_BoPhan bp ON u.MaBoPhan = bp.MaBoPhan
        WHERE rp.IsDeleted = 0
          AND (@Nam = 0 OR @Nam IS NULL OR YEAR(rp.CreatedDate) = @Nam)
          -- Loai khach hang
          AND (@CustomerTypeID IS NULL OR rc.CustomerTypeID = @CustomerTypeID)
          -- Loai du an
          AND (@ProjectTypeID IS NULL OR rp.ProjectTypeID = @ProjectTypeID)
          -- Trang thai du an
          AND (@StatusID IS NULL OR rp.Status = @StatusID)
          -- San pham dich vu
          AND (@ProductServiceID IS NULL
               OR EXISTS (SELECT 1
                          FROM RM_ProductProject rpp3
                          WHERE rpp3.ProjectID = rp.ProjectID
                            AND rpp3.IsDeleted = 0
                            AND rpp3.ProductServiceID = @ProductServiceID))
        GROUP BY rp.ProjectID,
                 rp.ProjectName,
                 rs.StatusName,
                 rpsType.StatusName,
                 rc.CustomerName,
                 rct.CustomerTypeName,
                 rcs.StatusName,
                 rp.ExecutionTime,
                 rp.Note,
                 u.FullName,
                 u.Phone,
                 cv.TenChucVu,
                 bp.TenBoPhan

        UNION ALL

        /* =========================================
           CO HOI KINH DOANH
        ========================================= */
        SELECT N'BusinessOpportunity' AS DataType,
               rbo.BusinessOpportunityID AS ObjectID,
               rc.CustomerName,
               rct.CustomerTypeName,
               rcs.StatusName AS CustomerGroupName,
               -- Chuan hoa ca ';' va ',' ve cung mot dau phan cach truoc khi so khop
               STUFF((SELECT N', ' + rps.NameProduct
                      FROM RM_ProductService rps
                      WHERE CHARINDEX(',' + CAST(rps.ProductServiceID AS VARCHAR(20)) + ',',
                                      ',' + REPLACE(REPLACE(ISNULL(rbo.ProductServiceIDs, ''), ';', ','), ' ', '') + ',') > 0
                      FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS ProductServiceNames,
               rbo.OpportunityName AS ProjectName,
               NULL AS ProjectTypeName,
               rs.StatusName AS OpportunityStatusName,
               ISNULL(STUFF((SELECT DISTINCT N', ' + ISNULL(su.FullName, '')
                                    + CASE WHEN cv.TenChucVu IS NOT NULL THEN N' - ' + cv.TenChucVu ELSE N'' END
                                    + CASE WHEN bp.TenBoPhan IS NOT NULL THEN N' - ' + bp.TenBoPhan ELSE N'' END
                                    + CASE WHEN su.Phone     IS NOT NULL THEN N' - ' + su.Phone     ELSE N'' END
                             FROM RM_SalesTeamMembers rstm
                             INNER JOIN Sys_Users su ON su.UserId = rstm.EmployeeID
                             LEFT JOIN MN_ChucVu cv ON su.MaChucVu = cv.MaChucVu
                             LEFT JOIN MN_BoPhan bp ON su.MaBoPhan = bp.MaBoPhan
                             WHERE rstm.BusinessOpportunityID = rbo.BusinessOpportunityID
                               AND (';' + ISNULL(rstm.RoleID, '') + ';' LIKE '%;5;%')
                             FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),
                      ISNULL(u.FullName, '') + N' - ' + cv.TenChucVu + N' - ' + bp.TenBoPhan + N' - ' + u.Phone) AS ContactPersonInfo,
               ISNULL(rbo.ExpectedValue * 1000000, 0) AS TotalExpectedValue,
               NULL AS TotalVNPTValue,
               NULL AS ExecutionTime,
               (SELECT TOP 1 dbo.fn_DecodeHtmlEntities(dbo.fn_StripHTML(ISNULL(reh.ExchangeContent, '')))
                FROM RM_ExchangeHistory reh
                WHERE reh.BusinessOpportunityID = rbo.BusinessOpportunityID
                ORDER BY reh.ExchangeDate DESC) AS Note
        FROM RM_BusinessOpportunity rbo
        LEFT JOIN RM_Status rs ON rbo.StatusID = rs.ID
        LEFT JOIN RM_Customer rc ON rbo.CustomerID = rc.CustomerID
        LEFT JOIN RM_CustomerType rct ON rc.CustomerTypeID = rct.CustomerTypeID
        LEFT JOIN RM_Status rcs ON rc.CustomerStatusID = rcs.ID
        LEFT JOIN Sys_Users u ON u.UserName = rbo.CreatedBy
        LEFT JOIN MN_ChucVu cv ON u.MaChucVu = cv.MaChucVu
        LEFT JOIN MN_BoPhan bp ON u.MaBoPhan = bp.MaBoPhan
        WHERE rbo.IsDeleted = 0
          AND (@Nam = 0 OR @Nam IS NULL OR YEAR(rbo.CreatedDate) = @Nam)
          AND (rs.StatusCode NOT IN ('WON', 'CONVERTED') AND rs.StatusKey = 'Opportunity')
          -- Loai khach hang
          AND (@CustomerTypeID IS NULL OR rc.CustomerTypeID = @CustomerTypeID)
          -- Trang thai co hoi
          AND (@StatusID IS NULL OR rbo.StatusID = @StatusID)
          -- Co hoi khong co "loai du an" nen bi loai khi nguoi dung loc theo tieu chi nay
          AND (@ProjectTypeID IS NULL)
          -- San pham dich vu
          AND (@ProductServiceID IS NULL
               OR CHARINDEX(',' + CAST(@ProductServiceID AS VARCHAR(20)) + ',',
                            ',' + REPLACE(REPLACE(ISNULL(rbo.ProductServiceIDs, ''), ';', ','), ' ', '') + ',') > 0)
        GROUP BY rbo.BusinessOpportunityID,
                 rbo.OpportunityName,
                 rs.StatusName,
                 rc.CustomerName,
                 rct.CustomerTypeName,
                 rcs.StatusName,
                 rbo.ExpectedValue,
                 rbo.ProductServiceIDs,
                 u.FullName,
                 u.Phone,
                 cv.TenChucVu,
                 bp.TenBoPhan
    )
    , T AS (
        SELECT ROW_NUMBER() OVER (
                   ORDER BY CASE @Order WHEN N'0'  THEN d.DataType               END ASC
                           ,CASE @Order WHEN N'1'  THEN d.CustomerName           END ASC
                           ,CASE @Order WHEN N'2'  THEN d.CustomerTypeName       END ASC
                           ,CASE @Order WHEN N'3'  THEN d.CustomerGroupName      END ASC
                           ,CASE @Order WHEN N'4'  THEN d.ProductServiceNames    END ASC
                           ,CASE @Order WHEN N'5'  THEN d.ProjectName            END ASC
                           ,CASE @Order WHEN N'6'  THEN d.ProjectTypeName        END ASC
                           ,CASE @Order WHEN N'7'  THEN d.OpportunityStatusName  END ASC
                           ,CASE @Order WHEN N'8'  THEN d.ContactPersonInfo      END ASC
                           ,CASE @Order WHEN N'9'  THEN d.TotalExpectedValue     END ASC
                           ,CASE @Order WHEN N'10' THEN d.TotalVNPTValue         END ASC
                           ,CASE @Order WHEN N'11' THEN d.ExecutionTime          END ASC
               ) AS RowIndex, d.*
        FROM CTE_RESULT AS d
        WHERE (@Search IS NULL
               OR d.CustomerName        LIKE N'%' + @Search + '%'
               OR d.CustomerTypeName    LIKE N'%' + @Search + '%'
               OR d.CustomerGroupName   LIKE N'%' + @Search + '%'
               OR d.ProductServiceNames LIKE N'%' + @Search + '%'
               OR d.ProjectName         LIKE N'%' + @Search + '%'
               OR d.ProjectTypeName     LIKE N'%' + @Search + '%'
               OR d.ContactPersonInfo   LIKE N'%' + @Search + '%')
          AND (@TuKhoa IS NULL
               OR d.CustomerName        LIKE N'%' + @TuKhoa + '%'
               OR d.CustomerTypeName    LIKE N'%' + @TuKhoa + '%'
               OR d.CustomerGroupName   LIKE N'%' + @TuKhoa + '%'
               OR d.ProductServiceNames LIKE N'%' + @TuKhoa + '%'
               OR d.ProjectName         LIKE N'%' + @TuKhoa + '%'
               OR d.ProjectTypeName     LIKE N'%' + @TuKhoa + '%'
               OR d.ContactPersonInfo   LIKE N'%' + @TuKhoa + '%')
          AND UPPER(@OrderDir) = 'ASC'
          AND (@Loai = '' OR @Loai IS NULL OR @Loai = d.DataType)

        UNION ALL

        SELECT ROW_NUMBER() OVER (
                   ORDER BY CASE @Order WHEN N'0'  THEN d.DataType               END DESC
                           ,CASE @Order WHEN N'1'  THEN d.CustomerName           END DESC
                           ,CASE @Order WHEN N'2'  THEN d.CustomerTypeName       END DESC
                           ,CASE @Order WHEN N'3'  THEN d.CustomerGroupName      END DESC
                           ,CASE @Order WHEN N'4'  THEN d.ProductServiceNames    END DESC
                           ,CASE @Order WHEN N'5'  THEN d.ProjectName            END DESC
                           ,CASE @Order WHEN N'6'  THEN d.ProjectTypeName        END DESC
                           ,CASE @Order WHEN N'7'  THEN d.OpportunityStatusName  END DESC
                           ,CASE @Order WHEN N'8'  THEN d.ContactPersonInfo      END DESC
                           ,CASE @Order WHEN N'9'  THEN d.TotalExpectedValue     END DESC
                           ,CASE @Order WHEN N'10' THEN d.TotalVNPTValue         END DESC
                           ,CASE @Order WHEN N'11' THEN d.ExecutionTime          END DESC
               ) AS RowIndex, d.*
        FROM CTE_RESULT AS d
        WHERE (@Search IS NULL
               OR d.CustomerName        LIKE N'%' + @Search + '%'
               OR d.CustomerTypeName    LIKE N'%' + @Search + '%'
               OR d.CustomerGroupName   LIKE N'%' + @Search + '%'
               OR d.ProductServiceNames LIKE N'%' + @Search + '%'
               OR d.ProjectName         LIKE N'%' + @Search + '%'
               OR d.ProjectTypeName     LIKE N'%' + @Search + '%'
               OR d.ContactPersonInfo   LIKE N'%' + @Search + '%')
          AND (@TuKhoa IS NULL
               OR d.CustomerName        LIKE N'%' + @TuKhoa + '%'
               OR d.CustomerTypeName    LIKE N'%' + @TuKhoa + '%'
               OR d.CustomerGroupName   LIKE N'%' + @TuKhoa + '%'
               OR d.ProductServiceNames LIKE N'%' + @TuKhoa + '%'
               OR d.ProjectName         LIKE N'%' + @TuKhoa + '%'
               OR d.ProjectTypeName     LIKE N'%' + @TuKhoa + '%'
               OR d.ContactPersonInfo   LIKE N'%' + @TuKhoa + '%')
          AND UPPER(@OrderDir) = 'DESC'
          AND (@Loai = '' OR @Loai IS NULL OR @Loai = d.DataType)
    )
    SELECT T.*, (SELECT COUNT(RowIndex) FROM T) AS TotalRow
    FROM T
    WHERE (@PageSize > 0 AND T.RowIndex BETWEEN @PageIndex + 1 AND @PageIndex + @PageSize)
       OR @PageSize <= 0;
END
GO

PRINT 'RM_Report_BusinessOpportunity_v3 created';
