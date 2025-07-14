using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace SMS_TYNB.Models;

public partial class SmsTynContext : DbContext
{
    public SmsTynContext()
    {
    }

    public SmsTynContext(DbContextOptions<SmsTynContext> options)
        : base(options)
    {
    }

    public virtual DbSet<WpCanbo> WpCanbo { get; set; }

    public virtual DbSet<WpCommentmeta> WpCommentmeta { get; set; }

    public virtual DbSet<WpComments> WpComments { get; set; }

    public virtual DbSet<WpDanhmuc> WpDanhmuc { get; set; }

    public virtual DbSet<WpFile> WpFile { get; set; }

    public virtual DbSet<WpGglcptchWhitelist> WpGglcptchWhitelist { get; set; }

    public virtual DbSet<WpLimitLogin> WpLimitLogin { get; set; }

    public virtual DbSet<WpLinks> WpLinks { get; set; }

    public virtual DbSet<WpNhom> WpNhom { get; set; }

    public virtual DbSet<WpNhomCanbo> WpNhomCanbo { get; set; }

    public virtual DbSet<WpOptions> WpOptions { get; set; }

    public virtual DbSet<WpPostmeta> WpPostmeta { get; set; }

    public virtual DbSet<WpPosts> WpPosts { get; set; }

    public virtual DbSet<WpSms> WpSms { get; set; }

    public virtual DbSet<WpSmsCanbo> WpSmsCanbo { get; set; }

    public virtual DbSet<WpSmsLog> WpSmsLog { get; set; }

    public virtual DbSet<WpSmsLogUpload> WpSmsLogUpload { get; set; }

    public virtual DbSet<WpSmsUpload> WpSmsUpload { get; set; }

    public virtual DbSet<WpTermRelationships> WpTermRelationships { get; set; }

    public virtual DbSet<WpTermTaxonomy> WpTermTaxonomy { get; set; }

    public virtual DbSet<WpTermmeta> WpTermmeta { get; set; }

    public virtual DbSet<WpTerms> WpTerms { get; set; }

    public virtual DbSet<WpUsermeta> WpUsermeta { get; set; }

    public virtual DbSet<WpUsers> WpUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=sms_tyn;uid=root;pwd=280503;charset=utf8mb4", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.3.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<WpCanbo>(entity =>
        {
            entity.HasKey(e => e.IdCanbo).HasName("PRIMARY");

            entity
                .ToTable("wp_canbo")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdCanbo).HasColumnName("id_canbo");
            entity.Property(e => e.Gioitinh).HasColumnName("gioitinh");
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

        modelBuilder.Entity<WpCommentmeta>(entity =>
        {
            entity.HasKey(e => e.MetaId).HasName("PRIMARY");

            entity
                .ToTable("wp_commentmeta")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CommentId, "comment_id");

            entity.HasIndex(e => e.MetaKey, "meta_key").HasAnnotation("MySql:IndexPrefixLength", new[] { 191 });

            entity.Property(e => e.MetaId).HasColumnName("meta_id");
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.MetaKey).HasColumnName("meta_key");
            entity.Property(e => e.MetaValue).HasColumnName("meta_value");
        });

        modelBuilder.Entity<WpComments>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PRIMARY");

            entity
                .ToTable("wp_comments")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => new { e.CommentApproved, e.CommentDateGmt }, "comment_approved_date_gmt");

            entity.HasIndex(e => e.CommentAuthorEmail, "comment_author_email").HasAnnotation("MySql:IndexPrefixLength", new[] { 10 });

            entity.HasIndex(e => e.CommentDateGmt, "comment_date_gmt");

            entity.HasIndex(e => e.CommentParent, "comment_parent");

            entity.HasIndex(e => e.CommentPostId, "comment_post_ID");

            entity.Property(e => e.CommentId).HasColumnName("comment_ID");
            entity.Property(e => e.CommentAgent)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("comment_agent");
            entity.Property(e => e.CommentApproved)
                .HasMaxLength(20)
                .HasDefaultValueSql("'1'")
                .HasColumnName("comment_approved");
            entity.Property(e => e.CommentAuthor)
                .HasColumnType("tinytext")
                .HasColumnName("comment_author");
            entity.Property(e => e.CommentAuthorEmail)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("comment_author_email");
            entity.Property(e => e.CommentAuthorIp)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("comment_author_IP");
            entity.Property(e => e.CommentAuthorUrl)
                .HasMaxLength(200)
                .HasDefaultValueSql("''")
                .HasColumnName("comment_author_url");
            entity.Property(e => e.CommentContent)
                .HasColumnType("text")
                .HasColumnName("comment_content");
            entity.Property(e => e.CommentDate)
                .HasDefaultValueSql("'0000-00-00 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("comment_date");
            entity.Property(e => e.CommentDateGmt)
                .HasDefaultValueSql("'0000-00-00 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("comment_date_gmt");
            entity.Property(e => e.CommentKarma).HasColumnName("comment_karma");
            entity.Property(e => e.CommentParent).HasColumnName("comment_parent");
            entity.Property(e => e.CommentPostId).HasColumnName("comment_post_ID");
            entity.Property(e => e.CommentType)
                .HasMaxLength(20)
                .HasDefaultValueSql("'comment'")
                .HasColumnName("comment_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<WpDanhmuc>(entity =>
        {
            entity.HasKey(e => e.IdDanhmuc).HasName("PRIMARY");

            entity.ToTable("wp_danhmuc");

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
            entity.HasKey(e => e.IdFile).HasName("PRIMARY");

            entity.ToTable("wp_file");

            entity.Property(e => e.IdFile).HasColumnName("id_file");
            entity.Property(e => e.BangLuuFile)
                .HasMaxLength(255)
                .HasColumnName("bang_luu_file");
            entity.Property(e => e.BangLuuFileId).HasColumnName("bang_luu_file_id");
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

        modelBuilder.Entity<WpGglcptchWhitelist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("wp_gglcptch_whitelist")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.Ip, "ip").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddTime)
                .HasColumnType("datetime")
                .HasColumnName("add_time");
            entity.Property(e => e.Ip)
                .HasMaxLength(31)
                .IsFixedLength()
                .HasColumnName("ip");
            entity.Property(e => e.IpFromInt).HasColumnName("ip_from_int");
            entity.Property(e => e.IpToInt).HasColumnName("ip_to_int");
        });

        modelBuilder.Entity<WpLimitLogin>(entity =>
        {
            entity.HasKey(e => e.LoginId).HasName("PRIMARY");

            entity
                .ToTable("wp_limit_login")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_vietnamese_ci");

            entity.Property(e => e.LoginId).HasColumnName("login_id");
            entity.Property(e => e.AttemptTime)
                .HasColumnType("datetime")
                .HasColumnName("attempt_time");
            entity.Property(e => e.LockedTime)
                .HasMaxLength(100)
                .HasColumnName("locked_time");
            entity.Property(e => e.LoginAttempts).HasColumnName("login_attempts");
            entity.Property(e => e.LoginIp)
                .HasMaxLength(50)
                .HasColumnName("login_ip");
        });

        modelBuilder.Entity<WpLinks>(entity =>
        {
            entity.HasKey(e => e.LinkId).HasName("PRIMARY");

            entity
                .ToTable("wp_links")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.LinkVisible, "link_visible");

            entity.Property(e => e.LinkId).HasColumnName("link_id");
            entity.Property(e => e.LinkDescription)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("link_description");
            entity.Property(e => e.LinkImage)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("link_image");
            entity.Property(e => e.LinkName)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("link_name");
            entity.Property(e => e.LinkNotes)
                .HasColumnType("mediumtext")
                .HasColumnName("link_notes");
            entity.Property(e => e.LinkOwner)
                .HasDefaultValueSql("'1'")
                .HasColumnName("link_owner");
            entity.Property(e => e.LinkRating).HasColumnName("link_rating");
            entity.Property(e => e.LinkRel)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("link_rel");
            entity.Property(e => e.LinkRss)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("link_rss");
            entity.Property(e => e.LinkTarget)
                .HasMaxLength(25)
                .HasDefaultValueSql("''")
                .HasColumnName("link_target");
            entity.Property(e => e.LinkUpdated)
                .HasDefaultValueSql("'0000-00-00 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("link_updated");
            entity.Property(e => e.LinkUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("link_url");
            entity.Property(e => e.LinkVisible)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Y'")
                .HasColumnName("link_visible");
        });

        modelBuilder.Entity<WpNhom>(entity =>
        {
            entity.HasKey(e => e.IdNhom).HasName("PRIMARY");

            entity.ToTable("wp_nhom");

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
            entity.HasKey(e => e.IdNhomCanbo).HasName("PRIMARY");

            entity.ToTable("wp_nhom_canbo");

            entity.HasIndex(e => e.IdCanbo, "fk_wp_canbo");

            entity.HasIndex(e => e.IdNhom, "fk_wp_nhom");

            entity.Property(e => e.IdNhomCanbo).HasColumnName("id_nhom_canbo");
            entity.Property(e => e.IdCanbo).HasColumnName("id_canbo");
            entity.Property(e => e.IdNhom).HasColumnName("id_nhom");

            entity.HasOne(d => d.IdCanboNavigation).WithMany(p => p.WpNhomCanbo)
                .HasForeignKey(d => d.IdCanbo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_wp_canbo");

            entity.HasOne(d => d.IdNhomNavigation).WithMany(p => p.WpNhomCanbo)
                .HasForeignKey(d => d.IdNhom)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_wp_nhom");
        });

        modelBuilder.Entity<WpOptions>(entity =>
        {
            entity.HasKey(e => e.OptionId).HasName("PRIMARY");

            entity
                .ToTable("wp_options")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Autoload, "autoload");

            entity.HasIndex(e => e.OptionName, "option_name").IsUnique();

            entity.Property(e => e.OptionId).HasColumnName("option_id");
            entity.Property(e => e.Autoload)
                .HasMaxLength(20)
                .HasDefaultValueSql("'yes'")
                .HasColumnName("autoload");
            entity.Property(e => e.OptionName)
                .HasMaxLength(191)
                .HasDefaultValueSql("''")
                .HasColumnName("option_name");
            entity.Property(e => e.OptionValue).HasColumnName("option_value");
        });

        modelBuilder.Entity<WpPostmeta>(entity =>
        {
            entity.HasKey(e => e.MetaId).HasName("PRIMARY");

            entity
                .ToTable("wp_postmeta")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.MetaKey, "meta_key").HasAnnotation("MySql:IndexPrefixLength", new[] { 191 });

            entity.HasIndex(e => e.PostId, "post_id");

            entity.Property(e => e.MetaId).HasColumnName("meta_id");
            entity.Property(e => e.MetaKey).HasColumnName("meta_key");
            entity.Property(e => e.MetaValue).HasColumnName("meta_value");
            entity.Property(e => e.PostId).HasColumnName("post_id");
        });

        modelBuilder.Entity<WpPosts>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("wp_posts")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.PostAuthor, "post_author");

            entity.HasIndex(e => e.PostName, "post_name").HasAnnotation("MySql:IndexPrefixLength", new[] { 191 });

            entity.HasIndex(e => e.PostParent, "post_parent");

            entity.HasIndex(e => new { e.PostType, e.PostStatus, e.PostDate, e.Id }, "type_status_date");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CommentCount).HasColumnName("comment_count");
            entity.Property(e => e.CommentStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("'open'")
                .HasColumnName("comment_status");
            entity.Property(e => e.Guid)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("guid");
            entity.Property(e => e.MenuOrder).HasColumnName("menu_order");
            entity.Property(e => e.PingStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("'open'")
                .HasColumnName("ping_status");
            entity.Property(e => e.Pinged)
                .HasColumnType("text")
                .HasColumnName("pinged");
            entity.Property(e => e.PostAuthor).HasColumnName("post_author");
            entity.Property(e => e.PostContent).HasColumnName("post_content");
            entity.Property(e => e.PostContentFiltered).HasColumnName("post_content_filtered");
            entity.Property(e => e.PostDate)
                .HasDefaultValueSql("'0000-00-00 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("post_date");
            entity.Property(e => e.PostDateGmt)
                .HasDefaultValueSql("'0000-00-00 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("post_date_gmt");
            entity.Property(e => e.PostExcerpt)
                .HasColumnType("text")
                .HasColumnName("post_excerpt");
            entity.Property(e => e.PostMimeType)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("post_mime_type");
            entity.Property(e => e.PostModified)
                .HasDefaultValueSql("'0000-00-00 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("post_modified");
            entity.Property(e => e.PostModifiedGmt)
                .HasDefaultValueSql("'0000-00-00 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("post_modified_gmt");
            entity.Property(e => e.PostName)
                .HasMaxLength(200)
                .HasDefaultValueSql("''")
                .HasColumnName("post_name");
            entity.Property(e => e.PostParent).HasColumnName("post_parent");
            entity.Property(e => e.PostPassword)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("post_password");
            entity.Property(e => e.PostStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("'publish'")
                .HasColumnName("post_status");
            entity.Property(e => e.PostTitle)
                .HasColumnType("text")
                .HasColumnName("post_title");
            entity.Property(e => e.PostType)
                .HasMaxLength(20)
                .HasDefaultValueSql("'post'")
                .HasColumnName("post_type");
            entity.Property(e => e.ToPing)
                .HasColumnType("text")
                .HasColumnName("to_ping");
        });

        modelBuilder.Entity<WpSms>(entity =>
        {
            entity.HasKey(e => e.IdSms).HasName("PRIMARY");

            entity.ToTable("wp_sms");

            entity.Property(e => e.IdSms).HasColumnName("id_sms");
            entity.Property(e => e.FileDinhKem)
                .HasMaxLength(5000)
                .HasColumnName("file_dinh_kem");
            entity.Property(e => e.IdNguoigui).HasColumnName("id_nguoigui");
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
            entity.HasKey(e => e.IdSmsCanbo).HasName("PRIMARY");

            entity.ToTable("wp_sms_canbo");

            entity.HasIndex(e => e.IdSms, "fk_wp_sms");

            entity.HasIndex(e => e.IdCanbo, "fk_wp_sms_canbo");

            entity.HasIndex(e => e.IdNhom, "fk_wp_sms_nhom");

            entity.Property(e => e.IdSmsCanbo).HasColumnName("id_sms_canbo");
            entity.Property(e => e.IdCanbo).HasColumnName("id_canbo");
            entity.Property(e => e.IdNhom).HasColumnName("id_nhom");
            entity.Property(e => e.IdSms).HasColumnName("id_sms");

            entity.HasOne(d => d.IdCanboNavigation).WithMany(p => p.WpSmsCanbo)
                .HasForeignKey(d => d.IdCanbo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_wp_sms_canbo");

            entity.HasOne(d => d.IdNhomNavigation).WithMany(p => p.WpSmsCanbo)
                .HasForeignKey(d => d.IdNhom)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_wp_sms_nhom");

            entity.HasOne(d => d.IdSmsNavigation).WithMany(p => p.WpSmsCanbo)
                .HasForeignKey(d => d.IdSms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_wp_sms");
        });

        modelBuilder.Entity<WpSmsLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("wp_sms_log")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_vietnamese_ci");

            entity.Property(e => e.Chitiet)
                .HasMaxLength(500)
                .HasColumnName("chitiet")
                .UseCollation("utf8mb3_unicode_ci");
            entity.Property(e => e.IdCanbo).HasColumnName("id_canbo");
            entity.Property(e => e.IdSms).HasColumnName("id_sms");
            entity.Property(e => e.NgayGui)
                .HasMaxLength(20)
                .HasColumnName("ngay_gui")
                .UseCollation("utf8mb3_unicode_ci");
            entity.Property(e => e.Trangthai).HasColumnName("trangthai");
        });

        modelBuilder.Entity<WpSmsLogUpload>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("wp_sms_log_upload")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_unicode_ci");

            entity.Property(e => e.Chitiet)
                .HasMaxLength(500)
                .HasColumnName("chitiet");
            entity.Property(e => e.IdSms).HasColumnName("id_sms");
            entity.Property(e => e.NgayGui)
                .HasMaxLength(20)
                .HasColumnName("ngay_gui");
            entity.Property(e => e.SoDt).HasColumnName("so_dt");
            entity.Property(e => e.Trangthai).HasColumnName("trangthai");
        });

        modelBuilder.Entity<WpSmsUpload>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("wp_sms_upload")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_unicode_ci");

            entity.Property(e => e.IdSms).HasColumnName("id_sms");
            entity.Property(e => e.SoDt).HasColumnName("so_dt");
        });

        modelBuilder.Entity<WpTermRelationships>(entity =>
        {
            entity.HasKey(e => new { e.ObjectId, e.TermTaxonomyId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("wp_term_relationships")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TermTaxonomyId, "term_taxonomy_id");

            entity.Property(e => e.ObjectId).HasColumnName("object_id");
            entity.Property(e => e.TermTaxonomyId).HasColumnName("term_taxonomy_id");
            entity.Property(e => e.TermOrder).HasColumnName("term_order");
        });

        modelBuilder.Entity<WpTermTaxonomy>(entity =>
        {
            entity.HasKey(e => e.TermTaxonomyId).HasName("PRIMARY");

            entity
                .ToTable("wp_term_taxonomy")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Taxonomy, "taxonomy");

            entity.HasIndex(e => new { e.TermId, e.Taxonomy }, "term_id_taxonomy").IsUnique();

            entity.Property(e => e.TermTaxonomyId).HasColumnName("term_taxonomy_id");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Parent).HasColumnName("parent");
            entity.Property(e => e.Taxonomy)
                .HasMaxLength(32)
                .HasDefaultValueSql("''")
                .HasColumnName("taxonomy");
            entity.Property(e => e.TermId).HasColumnName("term_id");
        });

        modelBuilder.Entity<WpTermmeta>(entity =>
        {
            entity.HasKey(e => e.MetaId).HasName("PRIMARY");

            entity
                .ToTable("wp_termmeta")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.MetaKey, "meta_key").HasAnnotation("MySql:IndexPrefixLength", new[] { 191 });

            entity.HasIndex(e => e.TermId, "term_id");

            entity.Property(e => e.MetaId).HasColumnName("meta_id");
            entity.Property(e => e.MetaKey).HasColumnName("meta_key");
            entity.Property(e => e.MetaValue).HasColumnName("meta_value");
            entity.Property(e => e.TermId).HasColumnName("term_id");
        });

        modelBuilder.Entity<WpTerms>(entity =>
        {
            entity.HasKey(e => e.TermId).HasName("PRIMARY");

            entity
                .ToTable("wp_terms")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Name, "name").HasAnnotation("MySql:IndexPrefixLength", new[] { 191 });

            entity.HasIndex(e => e.Slug, "slug").HasAnnotation("MySql:IndexPrefixLength", new[] { 191 });

            entity.Property(e => e.TermId).HasColumnName("term_id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasDefaultValueSql("''")
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(200)
                .HasDefaultValueSql("''")
                .HasColumnName("slug");
            entity.Property(e => e.TermGroup).HasColumnName("term_group");
        });

        modelBuilder.Entity<WpUsermeta>(entity =>
        {
            entity.HasKey(e => e.UmetaId).HasName("PRIMARY");

            entity
                .ToTable("wp_usermeta")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.MetaKey, "meta_key").HasAnnotation("MySql:IndexPrefixLength", new[] { 191 });

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.UmetaId).HasColumnName("umeta_id");
            entity.Property(e => e.MetaKey).HasColumnName("meta_key");
            entity.Property(e => e.MetaValue).HasColumnName("meta_value");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<WpUsers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("wp_users")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.UserEmail, "user_email");

            entity.HasIndex(e => e.UserLogin, "user_login_key");

            entity.HasIndex(e => e.UserNicename, "user_nicename");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(250)
                .HasDefaultValueSql("''")
                .HasColumnName("display_name");
            entity.Property(e => e.UserActivationKey)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("user_activation_key");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("user_email");
            entity.Property(e => e.UserLogin)
                .HasMaxLength(60)
                .HasDefaultValueSql("''")
                .HasColumnName("user_login");
            entity.Property(e => e.UserNicename)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("user_nicename");
            entity.Property(e => e.UserPass)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("user_pass");
            entity.Property(e => e.UserRegistered)
                .HasDefaultValueSql("'0000-00-00 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("user_registered");
            entity.Property(e => e.UserStatus).HasColumnName("user_status");
            entity.Property(e => e.UserUrl)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("user_url");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
