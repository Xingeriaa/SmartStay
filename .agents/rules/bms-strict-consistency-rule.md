---
trigger: always_on
---

I. AGENT SKILL & TOOL PROTOCOL (BẮT BUỘC)
RULE-01: AI MUST ưu tiên sử dụng các kỹ năng đã được định nghĩa sẵn trong thư mục .agent/skills.

RULE-02: Trước khi thực hiện bất kỳ hành động nào (Query DB, Generate Code, Call API), AI SHALL phân tích danh sách skill hiện có để đảm bảo không "tái phát minh bánh xe".

RULE-03: Nếu một tác vụ có thể được giải quyết bằng Skill, AI MUST gọi Skill đó với đúng tham số kỹ thuật. Cấm tuyệt đối việc viết code thô (hard-coded) cho các tác vụ đã có Skill hỗ trợ.

RULE-04: AI MUST tuân thủ các ràng buộc đầu vào/đầu ra của Skill. Nếu dữ liệu người dùng cung cấp thiếu để chạy Skill, AI SHALL yêu cầu bổ sung thay vì tự suy diễn.

II. ARCHITECTURE & TECH STACK AWARENESS
AS-01: Hệ thống là ASP.NET Core MVC + REST API + SQL Server. Code sinh ra MUST tuân thủ Pattern: Controller -> Service -> Repository -> Entity.

AS-02: RBAC Enforcement: Mọi Action trong Controller MUST có Attribute [Authorize] và check Role/Permission tương ứng (Admin, Staff, Tenant).

AS-03: Financial Ledger Integrity: Mọi giao dịch tiền tệ MUST được ghi vào bảng TenantBalanceTransactions (Append-only). Cấm UPDATE trực tiếp cột Balance mà không có log.

III. DATA CONSISTENCY & INTEGRITY (CRITICAL)
DI-01: Snapshot Rule: Khi tạo hóa đơn (Invoice), UnitPrice MUST được copy (Snapshot) từ ContractServices vào InvoiceDetails. Tuyệt đối không được Reference ngược lại bảng Service để tránh sai lệch khi giá dịch vụ thay đổi trong tương lai.

DI-02: Meter Reading Safety: Chỉ số mới (NewIndex) MUST >= Chỉ số cũ (OldIndex). Ràng buộc UNIQUE(RoomId, MonthYear, Type) MUST được kiểm tra trước khi Insert.

DI-03: Contract Safety: Tại một thời điểm, một phòng (Unit) chỉ được phép có duy nhất một hợp đồng ở trạng thái Active. Việc gia hạn hợp đồng SHALL tạo bản ghi mới hoặc Update EndDate, không được phép làm sai lệch lịch sử giá cũ.

IV. FINANCIAL SAFETY & FORMULA
FS-01: Công thức tính tiền là bất biến (Immutable):
TotalAmount = Rent + Services + Electricity + Water + PreviousDebt - CreditBalance

FS-02: Deposit Policy: Tiền cọc (Deposit) là khoản nợ phải trả (Liability), không được tính vào doanh thu (Revenue) cho đến khi hợp đồng thanh lý.

FS-03: Idempotency: Các tác vụ thanh toán MUST sử dụng Idempotency Key để ngăn chặn tình trạng một hóa đơn bị trừ tiền hai lần do lỗi mạng hoặc Double Click.

V. SECURITY & SOFT DELETE POLICY
SS-01: Soft Delete: Các bảng nghiệp vụ (Buildings, Rooms, Contracts, Invoices) MUST dùng IsDeleted = 1. AI SHALL thêm filter Where(x => !x.IsDeleted) vào tất cả các câu truy vấn mặc định.

SS-02: Audit Logs: Mọi thay đổi trạng thái tài chính, thay đổi giá, và thanh lý hợp đồng MUST tạo bản ghi trong AuditLogs kèm theo UserId và Timestamp.

SS-03: Anti-Overposting: Luôn sử dụng DTO/ViewModel. MUST NOT binding trực tiếp Entity vào API Parameter để tránh lộ cấu trúc DB hoặc bị cập nhật các cột nhạy cảm (như Balance hoặc Role).

VI. ANALYZING & RESPONSE PROTOCOL
Khi tiếp nhận yêu cầu từ người dùng (Database schema, Use cases, Workflow), AI MUST thực hiện quy trình 3 bước:

Cross-check: Kiểm tra sự mâu thuẫn giữa yêu cầu mới và các Rule tài chính/RBAC đã thiết lập.

Conflict Detection: Cảnh báo ngay lập tức nếu yêu cầu vi phạm tính toàn vẹn dữ liệu (VD: Cho phép xóa lịch sử thanh toán).

Actionable Fix: Đề xuất giải pháp sửa lỗi cụ thể dựa trên kiến trúc hệ thống, không đưa ra lời khuyên chung chung.

Tone of Voice: Serious - Technical - Direct - Strict.

🔍 RISK SUMMARY (TỔNG KẾT RỦI RO)
CRITICAL: Vi phạm RULE-01 (Skill Protocol) dẫn đến việc AI hoạt động không đồng nhất với các công cụ tự động hóa đã tích hợp, gây rủi ro sai lệch logic khi triển khai Production.

CRITICAL: Vi phạm DI-01 (Snapshot Integrity) sẽ làm sai lệch toàn bộ báo cáo tài chính khi giá dịch vụ biến động.

HIGH: Vi phạm SS-01 (Soft Delete) có thể gây mất dữ liệu lịch sử phục vụ công tác thanh tra/audit sau này.

HIGH: Thiếu Audit Logs đồng nghĩa với việc không thể truy vết khi xảy ra tranh chấp tiền tệ với cư dân.
