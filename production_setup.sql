-- ==========================================================
-- SCRIPT: CẤU HÌNH PRODUCTION CHO SMARTSTAY BMS
-- ==========================================================
-- Chức năng:
-- 1. Thêm Trigger chống chồng chéo thời gian trên cơ sở dữ liệu cho ServicePriceHistory
-- 2. Cấu hình Partitioning theo năm cho bảng AuditLogs để chống nghẽn nghẽn
-- ==========================================================

USE [YourDatabaseName]; -- Nhớ đổi tên Database của bạn cho đúng
GO

-- -------------------------------------------------------------------------
-- 1. TRIGGER: BẢO VỆ CHỐNG CHỒNG CHÉO THỜI GIAN SERVICE PRICE HISTORY
-- -------------------------------------------------------------------------
-- Mục tiêu: Đảm bảo không có 2 khoảng thời gian của cùng 1 Dịch Vụ bị đè lên nhau.
-- Logic: EffectiveFrom đến EffectiveTo mới Không được overlap vào khoảng của các bản ghi cũ.

IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_ServicePriceHistory_NoOverlap')
    DROP TRIGGER [dbo].[TR_ServicePriceHistory_NoOverlap];
GO

CREATE TRIGGER [dbo].[TR_ServicePriceHistory_NoOverlap]
ON [dbo].[ServicePriceHistory]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM Inserted i
        INNER JOIN [dbo].[ServicePriceHistory] p ON i.ServiceId = p.ServiceId 
            AND i.PriceId <> p.PriceId
        WHERE 
            -- Công thức Check Overlap: StartA <= EndB AND EndA >= StartB
            (i.EffectiveFrom <= ISNULL(p.EffectiveTo, '9999-12-31')) 
            AND 
            (ISNULL(i.EffectiveTo, '9999-12-31') >= p.EffectiveFrom)
    )
    BEGIN
        RAISERROR('Lỗi: Cập nhật giá dịch vụ bị CHỒNG CHÉO thời gian với một mức giá khác!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- -------------------------------------------------------------------------
-- 2. PARTITIONING: CHIA NHỎ BẢNG AUDITLOGS THEO TỪNG NĂM
-- -------------------------------------------------------------------------
-- Lưu ý: Thực thi phần này CẦN PHẢI DROP/RECREATE Khóa chính. 
-- Do `AuditLogs` có PK là AuditId, để gộp CreatedAt vào Partition ta phải làm:

-- A. Tạo Partition Function Tách theo từng năm (ví dụ từ 2024 - 2030)
IF NOT EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'PF_AuditLogs_Year')
BEGIN
    CREATE PARTITION FUNCTION PF_AuditLogs_Year (DATETIME2)
    AS RANGE RIGHT FOR VALUES 
    ('2024-01-01', '2025-01-01', '2026-01-01', '2027-01-01', '2028-01-01', '2029-01-01', '2030-01-01');
END
GO

-- B. Tạo Partition Scheme
IF NOT EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'PS_AuditLogs_Year')
BEGIN
    CREATE PARTITION SCHEME PS_AuditLogs_Year
    AS PARTITION PF_AuditLogs_Year
    ALL TO ([PRIMARY]);
END
GO

-- C. Đổi Clustered Index (Primary Key) sang Partition Scheme
-- Mặc định EntityFramework tạo PK CLUSTERED trên AuditId 
-- Vậy chúng ta phải drop PK cũ, sau đó tạo lại PK trên (AuditId, CreatedAt) ở Partition Scheme

/* BỎ MỞ COMMENT ĐỂ CHẠY (NẾU MUỐN MIGRATION ÁP DỤNG TRỰC TIẾP)
ALTER TABLE [dbo].[AuditLogs] DROP CONSTRAINT [PK_AuditLogs];
GO

ALTER TABLE [dbo].[AuditLogs] ADD CONSTRAINT [PK_AuditLogs] 
PRIMARY KEY CLUSTERED ([AuditId], [CreatedAt])
ON PS_AuditLogs_Year([CreatedAt]);
GO
*/
