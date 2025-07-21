using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SMS_TYNB.Models.Master
{
    public partial class SmsTynContext : DbContext
    {
        public SmsTynContext()
        {
        }

        public SmsTynContext(DbContextOptions<SmsTynContext> options)
            : base(options)
        {
        }

        public virtual DbSet<WpCanbo> WpCanbo { get; set; } = null!;
        public virtual DbSet<WpDanhmuc> WpDanhmuc { get; set; } = null!;
        public virtual DbSet<WpFile> WpFile { get; set; } = null!;
        public virtual DbSet<WpLichsu> WpLichsu { get; set; } = null!;
        public virtual DbSet<WpNhom> WpNhom { get; set; } = null!;
        public virtual DbSet<WpNhomCanbo> WpNhomCanbo { get; set; } = null!;
        public virtual DbSet<WpSms> WpSms { get; set; } = null!;
        public virtual DbSet<WpSmsCanbo> WpSmsCanbo { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;database=sms_tyn;uid=root;pwd=280503;charset=utf8mb4", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.3.0-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<WpCanbo>(entity =>
            {
                entity.HasKey(e => e.IdCanbo)
                    .HasName("PRIMARY");

                entity.ToTable("wp_canbo");

                entity.HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.IdCanbo).HasColumnName("id_canbo");

                entity.Property(e => e.Gioitinh).HasColumnName("gioitinh");

                entity.Property(e => e.IdUser)
                    .HasMaxLength(255)
                    .HasColumnName("id_user");

                entity.Property(e => e.MaCanbo)
                    .HasMaxLength(20)
                    .HasColumnName("ma_canbo");

                entity.Property(e => e.Mota)
                    .HasMaxLength(98)
                    .HasColumnName("mota");

                entity.Property(e => e.SoDt)
                    .HasMaxLength(50)
                    .HasColumnName("so_dt");

                entity.Property(e => e.TenCanbo)
                    .HasMaxLength(50)
                    .HasColumnName("ten_canbo");

                entity.Property(e => e.Trangthai).HasColumnName("trangthai");
            });

            modelBuilder.Entity<WpDanhmuc>(entity =>
            {
                entity.HasKey(e => e.IdDanhmuc)
                    .HasName("PRIMARY");

                entity.ToTable("wp_danhmuc");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.IdDanhmuc).HasColumnName("id_danhmuc");

                entity.Property(e => e.MaDanhmuc).HasColumnName("ma_danhmuc");

                entity.Property(e => e.TenDanhmuc)
                    .HasMaxLength(255)
                    .HasColumnName("ten_danhmuc")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.Type)
                    .HasMaxLength(100)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<WpFile>(entity =>
            {
                entity.HasKey(e => e.IdFile)
                    .HasName("PRIMARY");

                entity.ToTable("wp_file");

                entity.Property(e => e.IdFile).HasColumnName("id_file");

                entity.Property(e => e.BangLuuFile)
                    .HasMaxLength(255)
                    .HasColumnName("bang_luu_file");

                entity.Property(e => e.BangLuuFileId).HasColumnName("bang_luu_file_id");

                entity.Property(e => e.DuoiFile)
                    .HasMaxLength(255)
                    .HasColumnName("duoi_file");

                entity.Property(e => e.FileUrl)
                    .HasMaxLength(1000)
                    .HasColumnName("file_url");

                entity.Property(e => e.TenFile)
                    .HasMaxLength(500)
                    .HasColumnName("ten_file")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<WpLichsu>(entity =>
            {
                entity.HasKey(e => e.IdLichsu)
                    .HasName("PRIMARY");

                entity.ToTable("wp_lichsu");

                entity.Property(e => e.IdLichsu).HasColumnName("id_lichsu");

                entity.Property(e => e.BangLuuLichsu)
                    .HasMaxLength(255)
                    .HasColumnName("bang_luu_lichsu");

                entity.Property(e => e.BangLuuLichsuId).HasColumnName("bang_luu_lichsu_id");

                entity.Property(e => e.HanhDong)
                    .HasMaxLength(255)
                    .HasColumnName("hanh_dong");

                entity.Property(e => e.NgayTao)
                    .HasColumnType("datetime")
                    .HasColumnName("ngay_tao");

                entity.Property(e => e.NguoiTao)
                    .HasMaxLength(255)
                    .HasColumnName("nguoi_tao");
            });

            modelBuilder.Entity<WpNhom>(entity =>
            {
                entity.HasKey(e => e.IdNhom)
                    .HasName("PRIMARY");

                entity.ToTable("wp_nhom");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.IdNhom).HasColumnName("id_nhom");

                entity.Property(e => e.IdNhomCha).HasColumnName("id_nhom_cha");

                entity.Property(e => e.TenNhom)
                    .HasMaxLength(255)
                    .HasColumnName("ten_nhom")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.TrangThai).HasColumnName("trang_thai");
            });

            modelBuilder.Entity<WpNhomCanbo>(entity =>
            {
                entity.HasKey(e => e.IdNhomCanbo)
                    .HasName("PRIMARY");

                entity.ToTable("wp_nhom_canbo");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.HasIndex(e => e.IdCanbo, "fk_wp_canbo");

                entity.HasIndex(e => e.IdNhom, "fk_wp_nhom");

                entity.Property(e => e.IdNhomCanbo).HasColumnName("id_nhom_canbo");

                entity.Property(e => e.IdCanbo).HasColumnName("id_canbo");

                entity.Property(e => e.IdNhom).HasColumnName("id_nhom");

                entity.HasOne(d => d.IdCanboNavigation)
                    .WithMany(p => p.WpNhomCanbo)
                    .HasForeignKey(d => d.IdCanbo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wp_canbo");

                entity.HasOne(d => d.IdNhomNavigation)
                    .WithMany(p => p.WpNhomCanbo)
                    .HasForeignKey(d => d.IdNhom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wp_nhom");
            });

            modelBuilder.Entity<WpSms>(entity =>
            {
                entity.HasKey(e => e.IdSms)
                    .HasName("PRIMARY");

                entity.ToTable("wp_sms");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.IdSms).HasColumnName("id_sms");

                entity.Property(e => e.FileDinhKem)
                    .HasMaxLength(5000)
                    .HasColumnName("file_dinh_kem");

                entity.Property(e => e.IdNguoigui)
                    .HasMaxLength(255)
                    .HasColumnName("id_nguoigui");

                entity.Property(e => e.Ngaygui)
                    .HasColumnType("datetime")
                    .HasColumnName("ngaygui");

                entity.Property(e => e.Noidung)
                    .HasMaxLength(5000)
                    .HasColumnName("noidung")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.SoTn).HasColumnName("so_tn");
            });

            modelBuilder.Entity<WpSmsCanbo>(entity =>
            {
                entity.HasKey(e => e.IdSmsCanbo)
                    .HasName("PRIMARY");

                entity.ToTable("wp_sms_canbo");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.HasIndex(e => e.IdSms, "fk_wp_sms");

                entity.HasIndex(e => e.IdCanbo, "fk_wp_sms_canbo");

                entity.HasIndex(e => e.IdNhom, "fk_wp_sms_nhom");

                entity.Property(e => e.IdSmsCanbo).HasColumnName("id_sms_canbo");

                entity.Property(e => e.IdCanbo).HasColumnName("id_canbo");

                entity.Property(e => e.IdNhom).HasColumnName("id_nhom");

                entity.Property(e => e.IdSms).HasColumnName("id_sms");

                entity.HasOne(d => d.IdCanboNavigation)
                    .WithMany(p => p.WpSmsCanbo)
                    .HasForeignKey(d => d.IdCanbo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wp_sms_canbo");

                entity.HasOne(d => d.IdNhomNavigation)
                    .WithMany(p => p.WpSmsCanbo)
                    .HasForeignKey(d => d.IdNhom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wp_sms_nhom");

                entity.HasOne(d => d.IdSmsNavigation)
                    .WithMany(p => p.WpSmsCanbo)
                    .HasForeignKey(d => d.IdSms)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wp_sms");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
