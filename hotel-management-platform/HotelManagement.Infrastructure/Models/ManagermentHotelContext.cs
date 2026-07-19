using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MyData.Models;

namespace Mydata.Models;

public partial class ManagermentHotelContext : DbContext
{
    public ManagermentHotelContext()
    {
    }

    public ManagermentHotelContext(DbContextOptions<ManagermentHotelContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<StaffAction> StaffActions { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<User> Users { get; set; }


    // new 
    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<Services> HotelServices { get; set; }

    // ngày thêm 2024-09-27
    public virtual DbSet<Images> Images { get; set; }

    // ngày thêm 2024-09-28

    public virtual DbSet<RoomAmenity> RoomAmenities { get; set; }

    // ngày thêm 24/10/2025

    public virtual DbSet<Guests> Guests { get; set; }

    // 10.11.2025

    public virtual DbSet<Amenity> Amenities { get; set; }

    // 13.11.2025
    public virtual DbSet<Order> Orders  { get; set; }

    //27/11/2025
    public virtual DbSet<BookingTemporaryPayOS> BookingTemporaryPayOs { get; set; }
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
      
    //}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // These keys are SQL Server IDENTITY columns. Keep the mapping explicit because
        // the entities are partly scaffolded and partly hand-written. Seed/application
        // inserts must omit these values and let SQL Server generate them.
        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId);
            entity.Property(e => e.AmenityId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Services>(entity =>
        {
            entity.HasKey(e => e.ServiceId);
            entity.Property(e => e.ServiceId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.Property(e => e.RoomTypeId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.Property(e => e.RoomId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<RoomAmenity>(entity =>
        {
            entity.HasKey(e => e.IDRoomAmenity);
            entity.Property(e => e.IDRoomAmenity).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<BookingDetail>();

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__bookings__5DE3A5B1846038DC");

            entity.ToTable("bookings");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookings__user_i__5AEE82B9");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842F10D16AE6");

            entity.ToTable("notifications");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__notificat__user___693CA210");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.BookingId, "IX_Payments_BookingId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TransactionId).HasMaxLength(100);

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments).HasForeignKey(d => d.BookingId);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__reviews__60883D904168ABEB");

            entity.ToTable("reviews");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__reviews__user_id__6477ECF3");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__rooms__19675A8A1E843B59");

            entity.ToTable("rooms");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Description)
                .HasMaxLength(400)
                .HasDefaultValue("");
            entity.Property(e => e.Floor).HasColumnName("floor");
            entity.Property(e => e.PathImage)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(20)
                .HasColumnName("room_number");
            entity.Property(e => e.RoomTypeId).HasColumnName("room_type_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("available")
                .HasColumnName("status");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__rooms__room_type__5441852A");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PK__room_typ__42395E8438F59C5E");

            entity.ToTable("room_types");

            entity.Property(e => e.RoomTypeId).HasColumnName("room_type_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MaxGuests).HasColumnName("max_guests");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<StaffAction>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("PK__staff_ac__74EFC21721148A1D");

            entity.ToTable("staff_actions");

            entity.Property(e => e.ActionId).HasColumnName("action_id");
            entity.Property(e => e.Action)
                .HasMaxLength(255)
                .HasColumnName("action");
            entity.Property(e => e.ActionTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("action_time");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");

            entity.HasOne(d => d.Staff).WithMany(p => p.StaffActions)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__staff_act__staff__5FB337D6");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.IdToken);

            entity.ToTable("Token");

            entity.HasIndex(e => e.IdUser, "IX_Token_IdUser");

            entity.Property(e => e.ToketContent).HasMaxLength(500);

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Tokens).HasForeignKey(d => d.IdUser);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F41C6AE08");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164AD37B433").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.Role)
                .HasMaxLength(10)
                .HasColumnName("role");
            entity.Property(e => e.Salt).HasDefaultValue("");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
