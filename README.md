# Hệ Thống Đặt Phòng & Quản Lý Khách Sạn 2025

Project is currently under development (10/08/2025)

Software Engineer : Phạm Trung Đức 

- Backend : Phạm Trung Đức

- Frontend : ChatGPT  , Copilot , Phạm Trung Đức

For contributions or feedback, please contact: ptrungduc1011@gmail.com

Images used in this project are sourced from the Internet.

Demo Link: 

 + P1 : Demo Main Flow  https://www.youtube.com/watch?v=tiAMaPyY0SQ
 + P2 : Demo Role Admin https://www.youtube.com/watch?v=FB3BIbSn_Ww
 + P3 : Demo Role Receptionist (Lễ Tân) https://www.youtube.com/watch?v=BZ0BFKbjFhg
 + P4 : Demo Test MiniGame https://www.youtube.com/watch?v=9bWgw6dF9vo

## Technologies Used

- Backend: C# , ASP.NET, Web API , JavaScript 

- Frontend: ASP.NET MVC / Razor Pages ,JavaScript , CSS ,Boostrapt , Html

- Database: SQL Server (Entity Framework Core)

- Authentication: JWT, OAuth2

- Payment Gateway: VNPAY, Quét QR (PayOS) 

- RealTime : SignalR(C#)

### Project Features
### 1. User (Customer)
  + Register / Login / Forgot Password

  + Browse room listings (by type, price, number of guests, amenities, availability)

  + Advanced search & filtering (by price, number of guests, floor, etc.)

  + View room details (photos, description, amenities, cancellation policy, previous customer reviews)

  + Online booking
   
    - Select check-in / check-out dates

    - Select number of rooms and guests

    - Add extra services (breakfast, spa, airport transfer, etc.)

  + Online payment (VNPAY, PayOS bank transfer, etc.)

  + View booking history

  + Receive email notifications about related services

### 2. Staff (Hotel Employee)

  + Staff login

  + View & manage booking list (by date, room, customer)

  + Check-in / Check-out guests at the counter (with QR Code check-in option)
   
  + Confirm payments via bank transfer (PayOS QR) or cash

  + Update room status (available, cleaning, booked, occupied, maintenance)

  + Manage customer information (ID card, passport, contact)

  + Add additional services to guest invoices (room service, car rental, laundry, minibar, etc.)

  + Print invoices for guests (VAT, services, total amount)


 ###  3. Admin (System Administrator)

  + Admin login

  + User management (User, Staff, Admin)

  + Role assignment (Staff, Receptionist, Manager, Senior Admin)

  + Room management
    
     + Add / Edit / Delete room types

     + Add / Delete rooms
  
     + Edit room information such as basic info, update photo gallery, add/remove amenities, change room avatar

     + Update room prices and quantities

     + Enable / Disable room display

  + Manage room amenities


  + Manage all bookings (filter by date, status, customer)

  + Manage payments (online/offline, revenue by day/month/year)


  + Reports & Analytics

      + Room occupancy rate

      + Revenue over time

### 4. System Hotel Core 
 + Automatically update booking code status daily after 10 PM and send auto-cancellation notifications for no-show guests
 + Automatically send check-in reminder notifications 1 day before check-in
 + Automatically send check-out reminder notifications
 + Real-time notifications for transactions or invoice payments
### 5. Provide APIs for partners

 + Features such as: view room list, view available rooms, view room details, advanced search, book room, cancel booking, make payments, etc.

###  6. Mini Gamne 
 + Slot machine game
 + Chat with other players
