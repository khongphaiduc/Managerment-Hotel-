# Hệ Thống Đặt Phòng & Quản Lý Khách Sạn 2025

Dự Án Đang Trong Quá Trình Phát Triển  ( 10/08/2025 ) 

Software Engineer : Phạm Trung Đức 

- Backend : Phạm Trung Đức

- Frontend : ChatGPT  , Copilot , Phạm Trung Đức

Mọi đóng góp hay góp ý xin liên hệ gmail : ptrungduc1011@gmail.com

Các ảnh được sử dụng trong dự án được tổng hợp từ  internet 

Link Demo : chưa có hihi

## Công nghệ được sử dụng 

- Backend: C# , ASP.NET, Web API , JavaScript 

- Frontend: ASP.NET MVC / Razor Pages ,JavaScript , CSS ,Boostrapt , Html

- Database: SQL Server (Entity Framework Core)

- Authentication: JWT, OAuth2

- Payment Gateway: VNPAY, Quét QR (PayOS) // pham trung duc tesst 

### Các tính năng của dự án (theo dự kiến)
### 1. User (Khách hàng)
  + Đăng ký / Đăng nhập / Quên mật khẩu

  + Xem danh sách phòng (theo loại, giá, số người, tiện nghi, tình trạng trống)

  + Tìm kiếm & lọc phòng nâng cao (theo giá, số người, tầng,)

  + Xem chi tiết phòng (ảnh, mô tả, tiện ích, chính sách hủy, đánh giá khách hàng trước)

  + Đặt phòng trực tuyến
   
    - Chọn ngày check-in / check-out

    - Chọn số lượng phòng, người ở

    - Thêm dịch vụ đi kèm (ăn sáng, spa, đưa đón sân bay, …)

  + Thanh toán trực tuyến (Paypal, Stripe, VNPAY, Momo, chuyển khoản…)

  + Quản lý booking cá nhân (xem, hủy, đổi lịch, xem trạng thái thanh toán)

  + Xem lịch sử đặt phòng

  + Đánh giá & phản hồi dịch vụ

  + Nhận email thông báo các dụ vụ liên quan

### 2. Staff (Nhân viên khách sạn)

  + Đăng nhập nhân viên

  + Xem & quản lý danh sách booking (theo ngày, theo phòng, theo khách)

  + Check-in / Check-out khách tại quầy (kết hợp check-in by QR Code)

  + Cập nhật tình trạng phòng (trống, đang dọn, đã đặt, đang ở, bảo trì)

  + Quản lý thông tin khách (CMND/CCCD, hộ chiếu, liên hệ)

  + Thêm dịch vụ phát sinh (gọi món, thuê xe, giặt ủi, minibar…) vào hóa đơn khách

  + In hóa đơn cho khách (VAT, dịch vụ, tổng tiền)


 ###  3. Admin (Quản trị hệ thống)

  + Đăng nhập quản trị

  + Quản lý người dùng (User, Staff, Admin)

  + Phân quyền (Staff, Lễ tân, Quản lý, Admin cấp cao)

  + Khóa/mở tài khoản

  + Quản lý phòng
    
  + Thêm/sửa/xóa loại phòng

  + Thêm/xóa phòng 
  
  + Chỉnh sửa thông tin phòng  như các thông tin cơ bản , cập nhật danh sách ảnh , thêm xóa tiện ích , thay đổi avatar phòng

  + Cập nhật giá phòng, số lượng phòng

  + Bật / Tăt hiển thị phòng 

  + Quản lý tiện ích của phòng

  + Quản lý dịch vụ khách sạn (Spa, Gym, Nhà hàng, Tour du lịch…)

  + Quản lý booking toàn hệ thống (lọc theo ngày, trạng thái, khách hàng)

  + Quản lý thanh toán (online/offline, doanh thu theo ngày/tháng/năm)

  + Quản lý khuyến mãi / voucher / mã giảm giá

  + Báo cáo – Thống kê

  + Công suất phòng (occupancy rate)

  + Doanh thu theo thời gian

  + Top khách hàng, dịch vụ được sử dụng nhiều nhất

  + Quản lý nhân viên (tài khoản, phân ca, lịch làm việc)
### 4. System Hotel Core 
 + Tự động cập nhật trạng thái booking code mỗi ngày sau 22h và gửi thông báo tự động hủy phòng(case khách không đến check-in)
 + Tự động gửi thông báo nhắc khách hàng thời gian check-in trước 1 ngày 
 + Tự động gửi thông báo nhẵn khách hàng thời gian trả phòng
### 5. Cung cấp API cho các đối tác như các tính năng Xem danh sách phòng,Xem danh sách phòng còn chống , Xem thông tin chi tiết của từng phòng , Search Advance , Đặt Phòng ,  Hủy Phòng , Thanh Toán ...
