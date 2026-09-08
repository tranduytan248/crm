-- ============================================================
-- Phan he THONG BAO cho CRM (chuong thong bao tren header)
--  - Bang RM_Notification        : noi dung thong bao
--  - Bang RM_NotificationUser    : nguoi nhan + trang thai da doc
--  - 4 stored procedure: Save / GetByUser / MarkAsRead / MarkAllAsRead
-- Script idempotent - chay lai nhieu lan khong loi
-- ============================================================
SET NOCOUNT ON;
GO

-- ---------- 1. Bang du lieu ----------
IF OBJECT_ID('dbo.RM_Notification', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RM_Notification
    (
        NotificationID      INT IDENTITY(1,1) NOT NULL,
        Title               NVARCHAR(500)  NOT NULL,
        Content             NVARCHAR(2000) NULL,
        NotificationType    VARCHAR(100)   NULL,   -- ma loai nghiep vu
        SourceType          VARCHAR(50)    NULL,   -- Opportunity | Project | Task
        SourceID            INT            NULL,   -- ID co hoi / du an / cong viec
        DetailUrl           NVARCHAR(500)  NULL,   -- duong dan chi tiet (tuong doi)
        IconClass           VARCHAR(100)   NULL,   -- class icon fontawesome
        IconColor           VARCHAR(50)    NULL,   -- class mau icon
        CreatedBy           VARCHAR(250)   NULL,
        CreatedByFullName   NVARCHAR(250)  NULL,
        CreatedDate         DATETIME       NOT NULL CONSTRAINT DF_RM_Notification_CreatedDate DEFAULT (GETDATE()),
        IsDeleted           BIT            NOT NULL CONSTRAINT DF_RM_Notification_IsDeleted DEFAULT (0),
        CONSTRAINT PK_RM_Notification PRIMARY KEY CLUSTERED (NotificationID)
    );
    PRINT 'Created table RM_Notification';
END
GO

IF OBJECT_ID('dbo.RM_NotificationUser', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RM_NotificationUser
    (
        NotificationUserID  INT IDENTITY(1,1) NOT NULL,
        NotificationID      INT           NOT NULL,
        Username            VARCHAR(250)  NOT NULL,
        IsRead              BIT           NOT NULL CONSTRAINT DF_RM_NotificationUser_IsRead DEFAULT (0),
        ReadDate            DATETIME      NULL,
        CONSTRAINT PK_RM_NotificationUser PRIMARY KEY CLUSTERED (NotificationUserID)
    );
    PRINT 'Created table RM_NotificationUser';
END
GO

-- ---------- 2. Index ----------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RM_NotificationUser_User' AND object_id = OBJECT_ID('dbo.RM_NotificationUser'))
    CREATE NONCLUSTERED INDEX IX_RM_NotificationUser_User
        ON dbo.RM_NotificationUser (Username, IsRead) INCLUDE (NotificationID, ReadDate);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RM_NotificationUser_Notification' AND object_id = OBJECT_ID('dbo.RM_NotificationUser'))
    CREATE NONCLUSTERED INDEX IX_RM_NotificationUser_Notification
        ON dbo.RM_NotificationUser (NotificationID) INCLUDE (Username, IsRead);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RM_Notification_Created' AND object_id = OBJECT_ID('dbo.RM_Notification'))
    CREATE NONCLUSTERED INDEX IX_RM_Notification_Created
        ON dbo.RM_Notification (IsDeleted, CreatedDate DESC);
GO

-- ---------- 3. RM_Notification_Save ----------
-- Tao 1 thong bao va gan cho nhieu nguoi nhan (@Usernames phan cach bang , hoac ;)
-- Tra ve NotificationID vua tao
IF OBJECT_ID('dbo.RM_Notification_Save', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Notification_Save;
GO
CREATE PROCEDURE dbo.RM_Notification_Save
    @Title              NVARCHAR(500),
    @Content            NVARCHAR(2000) = NULL,
    @NotificationType   VARCHAR(100)   = NULL,
    @SourceType         VARCHAR(50)    = NULL,
    @SourceID           INT            = NULL,
    @DetailUrl          NVARCHAR(500)  = NULL,
    @IconClass          VARCHAR(100)   = NULL,
    @IconColor          VARCHAR(50)    = NULL,
    @CreatedBy          VARCHAR(250)   = NULL,
    @CreatedByFullName  NVARCHAR(250)  = NULL,
    @Usernames          NVARCHAR(MAX)  = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Title IS NULL OR LTRIM(RTRIM(@Title)) = N''
    BEGIN
        SELECT CAST(0 AS INT) AS NotificationID;
        RETURN;
    END

    -- Tach danh sach nguoi nhan, bo trung va bo rong
    DECLARE @Receivers TABLE (Username VARCHAR(250) PRIMARY KEY);
    IF @Usernames IS NOT NULL AND LTRIM(RTRIM(@Usernames)) <> N''
    BEGIN
        INSERT INTO @Receivers (Username)
        SELECT DISTINCT LTRIM(RTRIM(Value))
        FROM dbo.SplitString(REPLACE(@Usernames, ';', ','), ',')
        WHERE LTRIM(RTRIM(Value)) <> N'';
    END

    -- Khong co nguoi nhan thi khong tao thong bao
    IF NOT EXISTS (SELECT 1 FROM @Receivers)
    BEGIN
        SELECT CAST(0 AS INT) AS NotificationID;
        RETURN;
    END

    DECLARE @NewID INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.RM_Notification
            (Title, Content, NotificationType, SourceType, SourceID, DetailUrl,
             IconClass, IconColor, CreatedBy, CreatedByFullName, CreatedDate, IsDeleted)
        VALUES
            (@Title, @Content, @NotificationType, @SourceType, @SourceID, @DetailUrl,
             @IconClass, @IconColor, @CreatedBy, @CreatedByFullName, GETDATE(), 0);

        SET @NewID = SCOPE_IDENTITY();

        INSERT INTO dbo.RM_NotificationUser (NotificationID, Username, IsRead)
        SELECT @NewID, r.Username, 0
        FROM @Receivers r;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @NewID = 0;
    END CATCH

    SELECT ISNULL(@NewID, 0) AS NotificationID;
END
GO

-- ---------- 4. RM_Notification_GetByUser ----------
-- Lay danh sach thong bao cua 1 nguoi dung; tra kem tong so chua doc
IF OBJECT_ID('dbo.RM_Notification_GetByUser', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Notification_GetByUser;
GO
CREATE PROCEDURE dbo.RM_Notification_GetByUser
    @Username   NVARCHAR(250),
    @Top        INT = 20,
    @OnlyUnread BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF @Top IS NULL OR @Top <= 0 SET @Top = 20;
    SET @OnlyUnread = ISNULL(@OnlyUnread, 0);

    -- Ho tro ca username day du (a@b) lan phan truoc dau @
    DECLARE @Prefix NVARCHAR(250) =
        CASE WHEN CHARINDEX('@', @Username) > 0
             THEN LEFT(@Username, CHARINDEX('@', @Username) - 1)
             ELSE @Username END;

    DECLARE @UnreadCount INT;

    SELECT @UnreadCount = COUNT(*)
    FROM dbo.RM_NotificationUser nu
    INNER JOIN dbo.RM_Notification n ON n.NotificationID = nu.NotificationID AND n.IsDeleted = 0
    WHERE nu.IsRead = 0
      AND (nu.Username = @Username OR nu.Username = @Prefix);

    SELECT TOP (@Top)
        n.NotificationID,
        n.Title,
        n.Content,
        n.NotificationType,
        n.SourceType,
        n.SourceID,
        n.DetailUrl,
        n.IconClass,
        n.IconColor,
        n.CreatedBy,
        n.CreatedByFullName,
        n.CreatedDate,
        nu.IsRead,
        nu.ReadDate,
        ISNULL(@UnreadCount, 0) AS UnreadCount
    FROM dbo.RM_NotificationUser nu
    INNER JOIN dbo.RM_Notification n ON n.NotificationID = nu.NotificationID AND n.IsDeleted = 0
    WHERE (nu.Username = @Username OR nu.Username = @Prefix)
      AND (@OnlyUnread = 0 OR nu.IsRead = 0)
    ORDER BY n.CreatedDate DESC, n.NotificationID DESC;
END
GO

-- ---------- 5. RM_Notification_MarkAsRead ----------
IF OBJECT_ID('dbo.RM_Notification_MarkAsRead', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Notification_MarkAsRead;
GO
CREATE PROCEDURE dbo.RM_Notification_MarkAsRead
    @NotificationID INT,
    @Username       NVARCHAR(250)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Prefix NVARCHAR(250) =
        CASE WHEN CHARINDEX('@', @Username) > 0
             THEN LEFT(@Username, CHARINDEX('@', @Username) - 1)
             ELSE @Username END;

    UPDATE dbo.RM_NotificationUser
    SET IsRead = 1,
        ReadDate = GETDATE()
    WHERE NotificationID = @NotificationID
      AND IsRead = 0
      AND (Username = @Username OR Username = @Prefix);

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

-- ---------- 6. RM_Notification_MarkAllAsRead ----------
IF OBJECT_ID('dbo.RM_Notification_MarkAllAsRead', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RM_Notification_MarkAllAsRead;
GO
CREATE PROCEDURE dbo.RM_Notification_MarkAllAsRead
    @Username NVARCHAR(250)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Prefix NVARCHAR(250) =
        CASE WHEN CHARINDEX('@', @Username) > 0
             THEN LEFT(@Username, CHARINDEX('@', @Username) - 1)
             ELSE @Username END;

    UPDATE dbo.RM_NotificationUser
    SET IsRead = 1,
        ReadDate = GETDATE()
    WHERE IsRead = 0
      AND (Username = @Username OR Username = @Prefix);

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO

PRINT 'Notification_Setup.sql completed';
