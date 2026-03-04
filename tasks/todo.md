## Chart.js Implementation Plan

- [ ] 1. **Tạo Endpoint API Dữ Liệu Biểu Đồ**: Viết thêm action `[HttpGet("chart")]` trong `DashboardApiController` để phân tích Dòng tiền (Revenue/Debt) theo 6 tháng gần nhất.
- [ ] 2. **Cập nhật Giao Diện (UI)**: Xóa placeholder chart cũ trong `Views/Dashboard/Index.cshtml` và thêm canvas element `<canvas id="cashFlowChart"></canvas>`.
- [ ] 3. **Tích hợp Chart.js**: Viết đoạn mã JavaScript trực tiếp trên file View (hoặc file js riêng) sử dụng thư viện Chart.js để gọi AJAX lấy dữ liệu `api/dashboard/chart` và biểu diễn dưới dạng biểu đồ Area Chart / Bar Chart một cách trực quan, đẹp mắt (Wow effect).
- [ ] 4. **Kiểm tra và Xác nhận (Verify)**: Đảm bảo khi thay đổi bộ lọc tháng hoặc load trang biểu đồ hoạt động trơn tru.

## Review (Trống)

...
