using System;
using System.Collections.Generic;
using GalvaERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GalvaERP.Infrastructure.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<APMuka> APMukas { get; set; }

    public virtual DbSet<A_MASTER_BARANG> A_MASTER_BARANGs { get; set; }

    public virtual DbSet<AwalBank> AwalBanks { get; set; }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<Barang> Barangs { get; set; }

    public virtual DbSet<Bayar> Bayars { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Faktur> Fakturs { get; set; }

    public virtual DbSet<FakturPajak> FakturPajaks { get; set; }

    public virtual DbSet<Gudang> Gudangs { get; set; }

    public virtual DbSet<LPB> LPBs { get; set; }

    public virtual DbSet<Master_User> Master_Users { get; set; }

    public virtual DbSet<PO> POs { get; set; }

    public virtual DbSet<POConfirmation> POConfirmations { get; set; }

    public virtual DbSet<SPB> SPBs { get; set; }

    public virtual DbSet<SubPOConfirmation> SubPOConfirmations { get; set; }

    public virtual DbSet<SUBFAKTUR> SUBFAKTURs { get; set; }

    public virtual DbSet<SaldoAP> SaldoAPs { get; set; }

    public virtual DbSet<Satuan> Satuans { get; set; }

    public virtual DbSet<SubBayar> SubBayars { get; set; }

    public virtual DbSet<SubLPB> SubLPBs { get; set; }

    public virtual DbSet<SubPO> SubPOs { get; set; }

    public virtual DbSet<SubSPB> SubSPBs { get; set; }

    public virtual DbSet<SubTandaTerimaAr> SubTandaTerimaArs { get; set; }

    public virtual DbSet<SubVoucherAP> SubVoucherAPs { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierGroup> SupplierGroups { get; set; }

    public virtual DbSet<TTP> TTPs { get; set; }

    public virtual DbSet<TTPRetur> TTPReturs { get; set; }

    public virtual DbSet<TandaTerimaAr> TandaTerimaArs { get; set; }

    public virtual DbSet<Tx_IdempotencyRecord> Tx_IdempotencyRecords { get; set; }

    public virtual DbSet<Tx_PushSubscription> Tx_PushSubscriptions { get; set; }

    public virtual DbSet<VoucherAP> VoucherAPs { get; set; }

    public virtual DbSet<subTTP> subTTPs { get; set; }

    public virtual DbSet<subTTPRetur> subTTPReturs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<APMuka>(entity =>
        {
            entity.HasKey(e => e.PKindex);

            entity.ToTable("APMuka");

            entity.Property(e => e.Doku).HasMaxLength(20);
            entity.Property(e => e.Doku_Bayar).HasMaxLength(20);
            entity.Property(e => e.Doku_PO).HasMaxLength(50);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Giro).HasMaxLength(25);
            entity.Property(e => e.Jenis).HasMaxLength(5);
            entity.Property(e => e.Kirim).HasColumnType("smalldatetime");
            entity.Property(e => e.Kode_Bank).HasMaxLength(20);
            entity.Property(e => e.Kode_Supplier).HasMaxLength(20);
            entity.Property(e => e.Kode_Valas).HasMaxLength(12);
            entity.Property(e => e.Memo).HasMaxLength(255);
            entity.Property(e => e.NamaUser).HasMaxLength(20);
            entity.Property(e => e.NoSeri).HasMaxLength(20);
            entity.Property(e => e.Sts).HasMaxLength(5);
            entity.Property(e => e.TglCair).HasColumnType("smalldatetime");
            entity.Property(e => e.TglDoku).HasColumnType("smalldatetime");
            entity.Property(e => e.TglDokuBayar).HasColumnType("smalldatetime");
            entity.Property(e => e.TglGiro).HasColumnType("smalldatetime");
            entity.Property(e => e.Tipe).HasMaxLength(20);
            entity.Property(e => e.UserID).HasMaxLength(100);
        });

        modelBuilder.Entity<A_MASTER_BARANG>(entity =>
        {
            entity.HasKey(e => e.PKindex);

            entity.ToTable("A_MASTER_BARANG");

            entity.Property(e => e.Area).HasMaxLength(255);
            entity.Property(e => e.KETERANGAN).HasMaxLength(50);
            entity.Property(e => e.kodebaru).HasMaxLength(255);
            entity.Property(e => e.kodelama).HasMaxLength(255);
            entity.Property(e => e.nama).HasMaxLength(255);
        });

        modelBuilder.Entity<AwalBank>(entity =>
        {
            entity.HasKey(e => e.PKindex);

            entity.ToTable("AwalBank");

            entity.Property(e => e.AC).HasMaxLength(255);
            entity.Property(e => e.Kode_Valas).HasMaxLength(255);
            entity.Property(e => e.Nama).HasMaxLength(255);
        });

        modelBuilder.Entity<Bank>(entity =>
        {
            entity.HasKey(e => e.PKindex);

            entity.ToTable("Bank");

            entity.Property(e => e.AC).HasMaxLength(35);
            entity.Property(e => e.AN).HasMaxLength(50);
            entity.Property(e => e.Alamat1).HasMaxLength(50);
            entity.Property(e => e.Alamat2).HasMaxLength(50);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Fax).HasMaxLength(20);
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Kode).HasMaxLength(20);
            entity.Property(e => e.KodeLama).HasMaxLength(50);
            entity.Property(e => e.KodePos).HasMaxLength(20);
            entity.Property(e => e.Kode_Area).HasMaxLength(20);
            entity.Property(e => e.Kode_JenisBayar).HasMaxLength(20);
            entity.Property(e => e.Kode_Valas).HasMaxLength(12);
            entity.Property(e => e.Kota).HasMaxLength(35);
            entity.Property(e => e.Major).HasMaxLength(12);
            entity.Property(e => e.MajorPajak).HasMaxLength(50);
            entity.Property(e => e.Nama).HasMaxLength(100);
            entity.Property(e => e.PPh23List)
                .HasMaxLength(2)
                .IsUnicode(false);
            entity.Property(e => e.Reference).HasMaxLength(12);
            entity.Property(e => e.Telepon).HasMaxLength(20);
            entity.Property(e => e.TglDiskontinu).HasColumnType("smalldatetime");
            entity.Property(e => e.UserID).HasMaxLength(100);
        });

        modelBuilder.Entity<Barang>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Barang");

            entity.Property(e => e.Harga);
            entity.Property(e => e.Kode).HasMaxLength(50);
            entity.Property(e => e.Merk).HasMaxLength(100);
            entity.Property(e => e.Nama).HasMaxLength(255);
            entity.Property(e => e.Satuan).HasMaxLength(10);
        });

        modelBuilder.Entity<Bayar>(entity =>
        {
            entity.HasKey(e => e.PKindex);

            entity.ToTable("Bayar");

            entity.Property(e => e.Cara).HasMaxLength(20);
            entity.Property(e => e.Doku)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Jenis).HasMaxLength(10);
            entity.Property(e => e.Keterangan).HasMaxLength(255);
            entity.Property(e => e.Kode_BankSupplier).HasMaxLength(50);
            entity.Property(e => e.Kode_Supplier).HasMaxLength(50);
            entity.Property(e => e.Kode_Valas).HasMaxLength(12);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.STS).HasMaxLength(1);
            entity.Property(e => e.StatusGL).HasMaxLength(12);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.UserID).HasMaxLength(100);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.id_category);

            entity.ToTable("Category");

            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Kode).HasMaxLength(50);
            entity.Property(e => e.Nama).HasMaxLength(255);
            entity.Property(e => e.UserID).HasMaxLength(100);
        });

modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.id_dept).HasName("PK_Dept");

            entity.ToTable("Dept");

            entity.Property(e => e.Kode).HasMaxLength(20);
            entity.Property(e => e.Nama).HasMaxLength(50);
            entity.Property(e => e.KodeGTC).HasMaxLength(12);
            entity.Property(e => e.KodeEPK).HasMaxLength(12);
            entity.Property(e => e.NamaUser).HasMaxLength(50);
            entity.Property(e => e.TglUpDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Head).HasMaxLength(35);
            entity.Property(e => e.Chief).HasMaxLength(35);
            entity.Property(e => e.Staff).HasMaxLength(35);
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.NewEPK).HasMaxLength(50);
            entity.Property(e => e.dept_group).HasMaxLength(50);
            entity.Property(e => e.NonAktifTime).HasColumnType("smalldatetime");
            entity.Property(e => e.Kode_Master_Department).HasMaxLength(50);
        });

        modelBuilder.Entity<Faktur>(entity =>
        {
            entity.HasKey(e => e.PKBAS);

            entity.ToTable("Faktur");

            entity.Property(e => e.AlmKirim).HasMaxLength(255);
            entity.Property(e => e.CDOutExpired).HasColumnType("smalldatetime");
            entity.Property(e => e.Case1).HasMaxLength(255);
            entity.Property(e => e.Case2).HasMaxLength(255);
            entity.Property(e => e.Case3).HasMaxLength(255);
            entity.Property(e => e.Case4).HasMaxLength(255);
            entity.Property(e => e.Case5).HasMaxLength(255);
            entity.Property(e => e.DIVISION).HasMaxLength(10);
            entity.Property(e => e.DOKU_KONTRAK).HasMaxLength(100);
            entity.Property(e => e.DOKU_PD).HasMaxLength(100);
            entity.Property(e => e.DOKU_PROYEK).HasMaxLength(100);
            entity.Property(e => e.Destination).HasMaxLength(255);
            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.DokuBC40).HasMaxLength(50);
            entity.Property(e => e.Doku_FP).HasMaxLength(50);
            entity.Property(e => e.Doku_Gabungan).HasMaxLength(50);
            entity.Property(e => e.Doku_SJ).HasMaxLength(50);
            entity.Property(e => e.Doku_SPB).HasMaxLength(50);
            entity.Property(e => e.Doku_paket).HasMaxLength(100);
            entity.Property(e => e.ETA).HasMaxLength(20);
            entity.Property(e => e.ETD).HasMaxLength(20);
            entity.Property(e => e.EclipseID).HasMaxLength(20);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.HAPUS).HasMaxLength(100);
            entity.Property(e => e.HAWB).HasMaxLength(25);
            entity.Property(e => e.HUBUNGI).HasMaxLength(40);
            entity.Property(e => e.JABATANSIGN1).HasMaxLength(25);
            entity.Property(e => e.JABATANSIGN2).HasMaxLength(25);
            entity.Property(e => e.JABATANSIGN3).HasMaxLength(25);
            entity.Property(e => e.JenisPajak).HasMaxLength(20);
            entity.Property(e => e.KODE_VALAS).HasMaxLength(10);
            entity.Property(e => e.Keterangan).HasMaxLength(255);
            entity.Property(e => e.KodePajak).HasMaxLength(50);
            entity.Property(e => e.Kode_Area).HasMaxLength(50);
            entity.Property(e => e.Kode_Customer).HasMaxLength(20);
            entity.Property(e => e.Kode_CustomerGanti).HasMaxLength(50);
            entity.Property(e => e.Kode_Dept).HasMaxLength(20);
            entity.Property(e => e.Kode_Gudang).HasMaxLength(20);
            entity.Property(e => e.Kode_IDN).HasMaxLength(50);
            entity.Property(e => e.Kode_Meterai).HasMaxLength(50);
            entity.Property(e => e.Kode_PIC).HasMaxLength(50);
            entity.Property(e => e.Kode_Sales).HasMaxLength(10);
            entity.Property(e => e.Kode_SubCustomer).HasMaxLength(20);
            entity.Property(e => e.LIHAT).HasMaxLength(1);
            entity.Property(e => e.LOADING).HasMaxLength(30);
            entity.Property(e => e.MAWB).HasMaxLength(25);
            entity.Property(e => e.MOS).HasMaxLength(35);
            entity.Property(e => e.Memo).HasMaxLength(255);
            entity.Property(e => e.ModulSource).HasMaxLength(50);
            entity.Property(e => e.NAMASIGN1).HasMaxLength(25);
            entity.Property(e => e.NAMASIGN2).HasMaxLength(25);
            entity.Property(e => e.NAMASIGN3).HasMaxLength(25);
            entity.Property(e => e.NAMAUSER).HasMaxLength(50);
            entity.Property(e => e.NOSERI).HasMaxLength(100);
            entity.Property(e => e.NO_PEB).HasMaxLength(10);
            entity.Property(e => e.NPO).HasMaxLength(50);
            entity.Property(e => e.NPWPSub).HasMaxLength(50);
            entity.Property(e => e.NamaKirim).HasMaxLength(100);
            entity.Property(e => e.NamaTTD).HasMaxLength(50);
            entity.Property(e => e.NewEPK).HasMaxLength(50);
            entity.Property(e => e.NoMaterai)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Order_Class).HasMaxLength(50);
            entity.Property(e => e.Order_Type).HasMaxLength(50);
            entity.Property(e => e.PAYMENT).HasMaxLength(50);
            entity.Property(e => e.PHD).HasMaxLength(1);
            entity.Property(e => e.ProyekKe)
                .HasMaxLength(5)
                .HasDefaultValue("");
            entity.Property(e => e.REALISASI).HasMaxLength(5);
            entity.Property(e => e.REV).HasMaxLength(2);
            entity.Property(e => e.ReportBenq).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SHIP).HasMaxLength(50);
            entity.Property(e => e.STS).HasMaxLength(2);
            entity.Property(e => e.SalesLama).HasMaxLength(20);
            entity.Property(e => e.Shiping).HasMaxLength(6);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.StatusGL).HasMaxLength(10);
            entity.Property(e => e.TGL_ETA).HasColumnType("smalldatetime");
            entity.Property(e => e.TGL_PACKING).HasColumnType("smalldatetime");
            entity.Property(e => e.TGL_PEB).HasColumnType("smalldatetime");
            entity.Property(e => e.TGL_REALISASI).HasColumnType("smalldatetime");
            entity.Property(e => e.TGL_SPB).HasColumnType("smalldatetime");
            entity.Property(e => e.TYPE).HasMaxLength(50);
            entity.Property(e => e.Terbilang).HasMaxLength(300);
            entity.Property(e => e.TerbilangEnglish).HasMaxLength(300);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.TglPreview).HasColumnType("smalldatetime");
            entity.Property(e => e.TglSewa1).HasColumnType("smalldatetime");
            entity.Property(e => e.TglSewa2).HasColumnType("smalldatetime");
            entity.Property(e => e.Tgl_ETD).HasColumnType("smalldatetime");
            entity.Property(e => e.Tgl_Gabungan).HasColumnType("datetime");
            entity.Property(e => e.TipePRoject).HasMaxLength(50);
            entity.Property(e => e.UserEdit).HasMaxLength(50);
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.VESSEL1).HasMaxLength(35);
            entity.Property(e => e.VESSEL2).HasMaxLength(35);
            entity.Property(e => e.ValidasiTime).HasColumnType("smalldatetime");
            entity.Property(e => e.Vessel3).HasMaxLength(50);
            entity.Property(e => e.Vessel4).HasMaxLength(50);
            entity.Property(e => e.WAKTU).HasColumnType("smalldatetime");
            entity.Property(e => e.emaillastsent).HasColumnType("datetime");
            entity.Property(e => e.imgTTD).HasColumnType("image");
            entity.Property(e => e.nAMApROYEK).HasMaxLength(100);
            entity.Property(e => e.tGLKirim).HasColumnType("smalldatetime");
            entity.Property(e => e.tGLNPO).HasColumnType("smalldatetime");
            entity.Property(e => e.tGLVerify).HasColumnType("smalldatetime");
            entity.Property(e => e.tGL_Kontrak).HasColumnType("smalldatetime");
            entity.Property(e => e.tGL_PD).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<FakturPajak>(entity =>
        {
            entity.HasKey(e => e.PKBAS);

            entity.ToTable("FakturPajak");

            entity.Property(e => e.Alamat_Gabung).HasMaxLength(300);
            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.DokuBC40).HasMaxLength(50);
            entity.Property(e => e.Doku_faktur).HasMaxLength(50);
            entity.Property(e => e.EFaktur).HasMaxLength(255);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.KodeRnd).HasMaxLength(100);
            entity.Property(e => e.Kode_Customer).HasMaxLength(50);
            entity.Property(e => e.Kode_CustomerGabung).HasMaxLength(50);
            entity.Property(e => e.Kode_IDN).HasMaxLength(50);
            entity.Property(e => e.MARKING).HasMaxLength(1);
            entity.Property(e => e.Memo).HasMaxLength(255);
            entity.Property(e => e.Memo_Gabung).HasMaxLength(300);
            entity.Property(e => e.NPWP_Gabung).HasMaxLength(50);
            entity.Property(e => e.Nama_CustomerGabung).HasMaxLength(255);
            entity.Property(e => e.ODec).HasMaxLength(10);
            entity.Property(e => e.OGrp).HasMaxLength(10);
            entity.Property(e => e.OMrk).HasMaxLength(10);
            entity.Property(e => e.OPrc).HasMaxLength(10);
            entity.Property(e => e.PKP_Gabung).HasMaxLength(50);
            entity.Property(e => e.Proyekkd).HasMaxLength(20);
            entity.Property(e => e.TTD).HasMaxLength(50);
            entity.Property(e => e.Terbilang).HasMaxLength(300);
            entity.Property(e => e.TerbilangEnglish).HasMaxLength(300);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.Tgl_faktur).HasColumnType("smalldatetime");
            entity.Property(e => e.TipeFaktur).HasMaxLength(100);
            entity.Property(e => e.UserID).HasMaxLength(50);
            entity.Property(e => e.csvI).HasMaxLength(50);
            entity.Property(e => e.kode_CustomerGANTi).HasMaxLength(50);
            entity.Property(e => e.kode_valas).HasMaxLength(50);
        });

        modelBuilder.Entity<Gudang>(entity =>
        {
            entity.HasKey(e => e.id_gudang);

            entity.ToTable("Gudang");

            entity.Property(e => e.Alamat1).HasMaxLength(50);
            entity.Property(e => e.Alamat2).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.GudangSync).HasMaxLength(25);
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Kode).HasMaxLength(12);
            entity.Property(e => e.KodeLama).HasMaxLength(12);
            entity.Property(e => e.KodeLokasi).HasMaxLength(50);
            entity.Property(e => e.KodeOpname).HasMaxLength(50);
            entity.Property(e => e.Kode_Area).HasMaxLength(20);
            entity.Property(e => e.Kode_AreaOld).HasMaxLength(50);
            entity.Property(e => e.Kode_GudangOpname).HasMaxLength(50);
            entity.Property(e => e.Kota).HasMaxLength(30);
            entity.Property(e => e.Nama).HasMaxLength(50);
            entity.Property(e => e.NewEPK).HasMaxLength(50);
            entity.Property(e => e.NoCounter1).HasMaxLength(20);
            entity.Property(e => e.NoCounter2).HasMaxLength(20);
            entity.Property(e => e.NoCounter3).HasMaxLength(20);
            entity.Property(e => e.PIC).HasMaxLength(50);
            entity.Property(e => e.UserID).HasMaxLength(100);
        });

        modelBuilder.Entity<LPB>(entity =>
        {
            entity.HasKey(e => e.id_lpb);

            entity.ToTable("LPB");

            entity.Property(e => e.CreatedByInWMS).HasMaxLength(50);
            entity.Property(e => e.CreatedDateInWMS).HasColumnType("datetime");
            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.Doku_PO).HasMaxLength(50);
            entity.Property(e => e.Doku_PCF).HasMaxLength(50);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Ext_Doku_PO).HasMaxLength(50);
            entity.Property(e => e.ForwardAgent).HasMaxLength(100);
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Kode_Dept).HasMaxLength(20);
            entity.Property(e => e.Kode_IDN).HasMaxLength(50);
            entity.Property(e => e.Kode_Sup_Biaya_Angkut).HasMaxLength(50);
            entity.Property(e => e.Kode_Sup_Biaya_Asuransi).HasMaxLength(50);
            entity.Property(e => e.Kode_Sup_Biaya_Bea).HasMaxLength(50);
            entity.Property(e => e.Kode_Sup_Biaya_Exp1).HasMaxLength(50);
            entity.Property(e => e.Kode_Sup_Biaya_Exp2).HasMaxLength(50);
            entity.Property(e => e.Kode_Sup_Biaya_Interest).HasMaxLength(50);
            entity.Property(e => e.Kode_Sup_Biaya_LC).HasMaxLength(50);
            entity.Property(e => e.Kode_Sup_Biaya_Lain).HasMaxLength(50);
            entity.Property(e => e.Kode_Supplier).HasMaxLength(20);
            entity.Property(e => e.Kode_Valas).HasMaxLength(20);
            entity.Property(e => e.Kode_Valas_Angkut).HasMaxLength(10);
            entity.Property(e => e.Kode_Valas_Asuransi).HasMaxLength(10);
            entity.Property(e => e.Kode_Valas_Bea).HasMaxLength(10);
            entity.Property(e => e.Kode_Valas_Exp1).HasMaxLength(10);
            entity.Property(e => e.Kode_Valas_Exp2).HasMaxLength(10);
            entity.Property(e => e.Kode_Valas_Interest).HasMaxLength(10);
            entity.Property(e => e.Kode_Valas_LC).HasMaxLength(10);
            entity.Property(e => e.Kode_Valas_Lain).HasMaxLength(10);
            entity.Property(e => e.Kode_buyer).HasMaxLength(100);
            entity.Property(e => e.MOBIL).HasMaxLength(10);
            entity.Property(e => e.Memo).HasMaxLength(78);
            entity.Property(e => e.ModulSource).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.STP).HasMaxLength(1);
            entity.Property(e => e.STS).HasMaxLength(3);
            entity.Property(e => e.STS_Biaya).HasMaxLength(3);
            entity.Property(e => e.Status).HasMaxLength(5);
            entity.Property(e => e.StatusGL).HasMaxLength(12);
            entity.Property(e => e.SuratJalan).HasMaxLength(20);
            entity.Property(e => e.Tgl).HasColumnType("datetime");
            entity.Property(e => e.TglCreate).HasColumnType("smalldatetime");
            entity.Property(e => e.TglSuratJalan).HasColumnType("smalldatetime");
            entity.Property(e => e.Tgl_Ganti).HasColumnType("smalldatetime");
            entity.Property(e => e.Tgl_Pembayaran).HasColumnType("smalldatetime");
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.rptUserId).HasMaxLength(100);
        });

        modelBuilder.Entity<Master_User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Master_U__1788CC4C56720FBE");

            entity.HasIndex(e => e.Username, "UQ__Master_U__536C85E40632F4C3").IsUnique();

            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.RefreshTokenHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("User");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<PO>(entity =>
        {
            entity.HasKey(e => e.id_po);

            entity.ToTable("PO");

            entity.Property(e => e.ADDITIONAL).HasMaxLength(20);
            entity.Property(e => e.Arrival).HasMaxLength(100);
            entity.Property(e => e.BLAWB).HasMaxLength(50);
            entity.Property(e => e.Carrier).HasMaxLength(100);
            entity.Property(e => e.ContactPr).HasMaxLength(40);
            entity.Property(e => e.CountryOrigin).HasMaxLength(100);
            entity.Property(e => e.CreatedByInWMS).HasMaxLength(50);
            entity.Property(e => e.CreatedDateInWMS).HasColumnType("datetime");
            entity.Property(e => e.Discharge).HasMaxLength(100);
            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.DokuExt).HasMaxLength(20);
            entity.Property(e => e.DokuVendor).HasMaxLength(100);
            entity.Property(e => e.Doku_POSem).HasMaxLength(50);
            entity.Property(e => e.Doku_SPPB).HasMaxLength(20);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Kode_IDN).HasMaxLength(50);
            entity.Property(e => e.Kode_Supplier).HasMaxLength(12);
            entity.Property(e => e.Kode_Valas).HasMaxLength(12);
            entity.Property(e => e.Kode_buyer).HasMaxLength(100);
            entity.Property(e => e.Kode_dept).HasMaxLength(12);
            entity.Property(e => e.LC)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Loading).HasMaxLength(100);
            entity.Property(e => e.MOS).HasMaxLength(100);
            entity.Property(e => e.Memo).HasColumnType("text");
            entity.Property(e => e.ModulSource).HasMaxLength(50);
            entity.Property(e => e.PEMBUATAN).HasMaxLength(30);
            entity.Property(e => e.PIUD).HasMaxLength(100);
            entity.Property(e => e.Packing).HasMaxLength(100);
            entity.Property(e => e.Pembayaran).HasMaxLength(255);
            entity.Property(e => e.Penyelesaian).HasMaxLength(255);
            entity.Property(e => e.Revisi).HasMaxLength(10);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.STS).HasMaxLength(3);
            entity.Property(e => e.STSPrint).HasMaxLength(1);
            entity.Property(e => e.Ship).HasMaxLength(100);
            entity.Property(e => e.Sign).HasMaxLength(10);
            entity.Property(e => e.Terms).HasMaxLength(50);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.TglCountryOrigin).HasColumnType("smalldatetime");
            entity.Property(e => e.TglDeparture).HasColumnType("smalldatetime");
            entity.Property(e => e.TglDokuVendor).HasColumnType("smalldatetime");
            entity.Property(e => e.TglPIUD).HasColumnType("smalldatetime");
            entity.Property(e => e.TglShip).HasColumnType("smalldatetime");
            entity.Property(e => e.TglVerify).HasColumnType("smalldatetime");
            entity.Property(e => e.Tgl_Pembayaran).HasColumnType("smalldatetime");
            entity.Property(e => e.Tgl_Pengiriman).HasColumnType("smalldatetime");
            entity.Property(e => e.Tipe).HasMaxLength(10);
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.Vessel).HasMaxLength(100);
            entity.Property(e => e.Wkt).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<POConfirmation>(entity =>
        {
            entity.HasKey(e => e.id_po_confirmation);

            entity.ToTable("POConfirmation");

            entity.Property(e => e.ContactPr).HasMaxLength(40);
            entity.Property(e => e.Diskon);
            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.Doku_PO).HasMaxLength(50);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Etd).HasColumnType("smalldatetime");
            entity.Property(e => e.Kode_Supplier).HasMaxLength(12);
            entity.Property(e => e.Kode_Valas).HasMaxLength(12);
            entity.Property(e => e.Kode_dept).HasMaxLength(12);
            entity.Property(e => e.Memo).HasColumnType("text");
            entity.Property(e => e.Nilai);
            entity.Property(e => e.PPN);
            entity.Property(e => e.Psd).HasColumnType("smalldatetime");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.STS).HasMaxLength(3);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<SPB>(entity =>
        {
            entity.HasKey(e => e.id_spb);

            entity.ToTable("SPB");

            entity.Property(e => e.AlmKirim).HasMaxLength(255);
            entity.Property(e => e.BusinessModel).HasMaxLength(50);
            entity.Property(e => e.CDOutBasicCal).HasMaxLength(20);
            entity.Property(e => e.CDOutDayCal).HasMaxLength(20);
            entity.Property(e => e.CDOutTglAkhir).HasColumnType("smalldatetime");
            entity.Property(e => e.CDOutTglAwal).HasColumnType("smalldatetime");
            entity.Property(e => e.ClaimCode).HasMaxLength(50);
            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.DokuSFA).HasMaxLength(50);
            entity.Property(e => e.Doku_Kontrak).HasMaxLength(100);
            entity.Property(e => e.Doku_LPB).HasMaxLength(50);
            entity.Property(e => e.Doku_PD).HasMaxLength(50);
            entity.Property(e => e.Doku_Sewa).HasMaxLength(50);
            entity.Property(e => e.EclipseID).HasMaxLength(20);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Hubungi).HasMaxLength(75);
            entity.Property(e => e.Jenis)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.KirimKd)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.Kode_Area).HasMaxLength(50);
            entity.Property(e => e.Kode_Customer).HasMaxLength(20);
            entity.Property(e => e.Kode_CustomerGanti).HasMaxLength(50);
            entity.Property(e => e.Kode_Dept).HasMaxLength(12);
            entity.Property(e => e.Kode_IDN).HasMaxLength(50);
            entity.Property(e => e.Kode_MarketSegment).HasMaxLength(20);
            entity.Property(e => e.Kode_MarketSegmentGrup).HasMaxLength(20);
            entity.Property(e => e.Kode_MarketSegmentGrupOld)
                .HasMaxLength(10)
                .HasDefaultValue("");
            entity.Property(e => e.Kode_PIC).HasMaxLength(50);
            entity.Property(e => e.Kode_Sales).HasMaxLength(12);
            entity.Property(e => e.Kode_SubCustomer).HasMaxLength(20);
            entity.Property(e => e.Kode_Valas).HasMaxLength(12);
            entity.Property(e => e.Lihat).HasMaxLength(1);
            entity.Property(e => e.MCCode).HasMaxLength(20);
            entity.Property(e => e.MEMO).HasMaxLength(255);
            entity.Property(e => e.ModulSource).HasMaxLength(50);
            entity.Property(e => e.NAMA_PD).HasMaxLength(100);
            entity.Property(e => e.NIK).HasMaxLength(20);
            entity.Property(e => e.NPO).HasMaxLength(50);
            entity.Property(e => e.NPWPSub).HasMaxLength(50);
            entity.Property(e => e.NamaKirim).HasMaxLength(255);
            entity.Property(e => e.NamaPenerima).HasMaxLength(50);
            entity.Property(e => e.NamaProyek).HasMaxLength(255);
            entity.Property(e => e.NewEPK).HasMaxLength(50);
            entity.Property(e => e.NoteHangusSO).HasMaxLength(50);
            entity.Property(e => e.Order_Class).HasMaxLength(50);
            entity.Property(e => e.Order_Type).HasMaxLength(50);
            entity.Property(e => e.Pay).HasMaxLength(10);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SalesLama).HasMaxLength(20);
            entity.Property(e => e.Ship).HasMaxLength(10);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Sts).HasMaxLength(2);
            entity.Property(e => e.Terbilang).HasMaxLength(300);
            entity.Property(e => e.TerbilangEnglish).HasMaxLength(300);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.TglDokuSFA).HasColumnType("smalldatetime");
            entity.Property(e => e.TglKirim).HasColumnType("smalldatetime");
            entity.Property(e => e.TglNPO).HasColumnType("smalldatetime");
            entity.Property(e => e.TglSewa1).HasColumnType("smalldatetime");
            entity.Property(e => e.TglSewa2).HasColumnType("smalldatetime");
            entity.Property(e => e.TglVerify).HasColumnType("smalldatetime");
            entity.Property(e => e.TipePRoject).HasMaxLength(50);
            entity.Property(e => e.Titip).HasMaxLength(10);
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.Waktu).HasColumnType("smalldatetime");
            entity.Property(e => e.lokasi).HasMaxLength(5);
            entity.Property(e => e.tgl_DokuPD).HasColumnType("smalldatetime");
            entity.Property(e => e.tgl_Kontrak).HasColumnType("smalldatetime");
            entity.Property(e => e.tgl_PD).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<SUBFAKTUR>(entity =>
        {
            entity.HasKey(e => e.PKindex);

            entity.ToTable("SUBFAKTUR");

            entity.Property(e => e.Alias).HasMaxLength(255);
            entity.Property(e => e.AliasCode).HasMaxLength(50);
            entity.Property(e => e.AlmKirim).HasMaxLength(300);
            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.Doku_SJ).HasMaxLength(50);
            entity.Property(e => e.Doku_SPB).HasMaxLength(50);
            entity.Property(e => e.Doku_paket).HasMaxLength(100);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.InfoCM).HasMaxLength(50);
            entity.Property(e => e.KodeRnd).HasMaxLength(100);
            entity.Property(e => e.Kode_Brg).HasMaxLength(50);
            entity.Property(e => e.Kode_Customer).HasMaxLength(20);
            entity.Property(e => e.Kode_CustomerGanti).HasMaxLength(50);
            entity.Property(e => e.Kode_Dept).HasMaxLength(20);
            entity.Property(e => e.Kode_Gudang).HasMaxLength(20);
            entity.Property(e => e.Kode_Sales).HasMaxLength(100);
            entity.Property(e => e.Kode_Valas).HasMaxLength(10);
            entity.Property(e => e.Kode_paket).HasMaxLength(100);
            entity.Property(e => e.Kode_tujuan).HasMaxLength(100);
            entity.Property(e => e.MAJORPPHJASA).HasMaxLength(50);
            entity.Property(e => e.MAJORPPNBM).HasMaxLength(50);
            entity.Property(e => e.MAJORRETUR).HasMaxLength(50);
            entity.Property(e => e.MODEL).HasMaxLength(50);
            entity.Property(e => e.MajorAR).HasMaxLength(100);
            entity.Property(e => e.MajorCustomer).HasMaxLength(100);
            entity.Property(e => e.MajorHPP).HasMaxLength(100);
            entity.Property(e => e.MajorPPbBm).HasMaxLength(50);
            entity.Property(e => e.MajorPPbnBm).HasMaxLength(50);
            entity.Property(e => e.MajorPPn).HasMaxLength(100);
            entity.Property(e => e.MajorPSD).HasMaxLength(100);
            entity.Property(e => e.MajordISKON).HasMaxLength(100);
            entity.Property(e => e.Memo).HasMaxLength(200);
            entity.Property(e => e.NPO).HasMaxLength(50);
            entity.Property(e => e.Nama_paket).HasMaxLength(100);
            entity.Property(e => e.NewEPK).HasMaxLength(50);
            entity.Property(e => e.ProyekKe)
                .HasMaxLength(5)
                .HasDefaultValue("");
            entity.Property(e => e.Proyekkd).HasMaxLength(20);
            entity.Property(e => e.SalesLama).HasMaxLength(20);
            entity.Property(e => e.Spec).HasMaxLength(10);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.TglKirim).HasColumnType("smalldatetime");
            entity.Property(e => e.TipePRoject).HasMaxLength(50);
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.kode_BRGganti).HasMaxLength(50);
            entity.Property(e => e.referencecustomer).HasMaxLength(100);
            entity.Property(e => e.sts).HasMaxLength(100);
            entity.Property(e => e.tgl_kirim).HasColumnType("smalldatetime");
            entity.Property(e => e.tgl_paket).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<SaldoAP>(entity =>
        {
            entity.HasKey(e => e.PKbas);

            entity.ToTable("SaldoAP");

            entity.Property(e => e.Kode_Supplier).HasMaxLength(255);
        });

        modelBuilder.Entity<Satuan>(entity =>
        {
            entity.HasKey(e => e.id_satuan);

            entity.ToTable("Satuan");

            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Kode).HasMaxLength(2);
            entity.Property(e => e.Nama).HasMaxLength(10);
            entity.Property(e => e.UserID).HasMaxLength(100);
        });

modelBuilder.Entity<SubBayar>(entity =>
        {
            entity.HasKey(e => e.PKbas).HasName("PK_SubBayar");

            entity.ToTable("SubBayar");

            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.Kode_Supplier).HasMaxLength(20);
            entity.Property(e => e.Doku_Faktur).HasMaxLength(50);
            entity.Property(e => e.Doku_LPB).HasMaxLength(50);
            entity.Property(e => e.SuratJalan).HasMaxLength(50);
            entity.Property(e => e.Giro).HasMaxLength(25);
            entity.Property(e => e.TglGiro).HasColumnType("smalldatetime");
            entity.Property(e => e.Sts).HasMaxLength(1);
            entity.Property(e => e.Doku_Muka).HasMaxLength(50);
            entity.Property(e => e.Cara).HasMaxLength(100);
            entity.Property(e => e.Kode_Valas).HasMaxLength(10);
            entity.Property(e => e.Kode_ValasBayar).HasMaxLength(10);
            entity.Property(e => e.Kode_Bank).HasMaxLength(20);
            entity.Property(e => e.Keterangan).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(1);
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Kode_Dept).HasMaxLength(10);
            entity.Property(e => e.Reference).HasMaxLength(20);
            entity.Property(e => e.ReferenceKasBank).HasMaxLength(50);
            entity.Property(e => e.FakturPajak).HasMaxLength(30);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<SubLPB>(entity =>
        {
            entity.HasKey(e => e.id_sub_lpb);

            entity.ToTable("SubLPB");

            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.Doku_PO).HasMaxLength(50);
            entity.Property(e => e.Doku_PCF).HasMaxLength(50);
            entity.Property(e => e.id_sub_po_confirmation);
            entity.Property(e => e.Doku_SPPB).HasMaxLength(50);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Estimated).HasMaxLength(50);
            entity.Property(e => e.Ext_Doku_PO).HasMaxLength(50);
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Keterangan).HasMaxLength(50);
            entity.Property(e => e.KodeRnd).HasMaxLength(100);
            entity.Property(e => e.Kode_Brg).HasMaxLength(50);
            entity.Property(e => e.Kode_Dept_PO).HasMaxLength(12);
            entity.Property(e => e.Kode_Gudang).HasMaxLength(10);
            entity.Property(e => e.Kode_Valas).HasMaxLength(12);
            entity.Property(e => e.MEMO1).HasMaxLength(255);
            entity.Property(e => e.Model).HasMaxLength(255);
            entity.Property(e => e.NAMAUSER).HasMaxLength(20);
            entity.Property(e => e.STP).HasMaxLength(1);
            entity.Property(e => e.SuratJalan).HasMaxLength(50);
            entity.Property(e => e.TGL_BAYAR).HasColumnType("smalldatetime");
            entity.Property(e => e.TempNama).HasMaxLength(70);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.TglCreate).HasColumnType("smalldatetime");
            entity.Property(e => e.Tgl_PO).HasColumnType("smalldatetime");
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.kode_BRGganti).HasMaxLength(50);
        });

        modelBuilder.Entity<SubPO>(entity =>
        {
            entity.HasKey(e => e.id_sub_po);

            entity.ToTable("SubPO");

            entity.Property(e => e.Alias).HasMaxLength(255);
            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.Doku_LPB).HasMaxLength(12);
            entity.Property(e => e.Doku_POSem).HasMaxLength(50);
            entity.Property(e => e.Doku_SO).HasMaxLength(255);
            entity.Property(e => e.Doku_SPPB).HasMaxLength(20);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.ExtDokuPO).HasMaxLength(20);
            entity.Property(e => e.KETNPSD).HasMaxLength(50);
            entity.Property(e => e.Keterangan).HasMaxLength(255);
            entity.Property(e => e.KodeRnd).HasMaxLength(100);
            entity.Property(e => e.KodeRnd_SO).HasMaxLength(255);
            entity.Property(e => e.Kode_Brg).HasMaxLength(50);
            entity.Property(e => e.Kode_Dept).HasMaxLength(12);
            entity.Property(e => e.Kode_Gudang).HasMaxLength(10);
            entity.Property(e => e.Kode_Valas).HasMaxLength(10);
            entity.Property(e => e.Major).HasMaxLength(20);
            entity.Property(e => e.Model).HasMaxLength(255);
            entity.Property(e => e.Merk).HasMaxLength(100);
            entity.Property(e => e.Satuan).HasMaxLength(10);
            entity.Property(e => e.DiscPct);
            entity.Property(e => e.Ref).HasMaxLength(20);
            entity.Property(e => e.TGL_LPB).HasColumnType("smalldatetime");
            entity.Property(e => e.TempNama).HasMaxLength(70);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.TglKirim).HasColumnType("smalldatetime");
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.kode_BRGganti).HasMaxLength(50);
        });

        modelBuilder.Entity<SubPOConfirmation>(entity =>
        {
            entity.HasKey(e => e.id_sub_po_confirmation);

            entity.ToTable("SubPOConfirmation");

            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Harga);
            entity.Property(e => e.Jumlah);
            entity.Property(e => e.Kode_Brg).HasMaxLength(50);
            entity.Property(e => e.Kode_Gudang).HasMaxLength(10);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Total);
        });

        modelBuilder.Entity<SubSPB>(entity =>
        {
            entity.HasKey(e => e.id_sub_spb);

            entity.ToTable("SubSPB");

            entity.Property(e => e.Alias).HasMaxLength(255);
            entity.Property(e => e.AliasCode).HasMaxLength(50);
            entity.Property(e => e.AlmKirim).HasMaxLength(255);
            entity.Property(e => e.CustKd).HasMaxLength(30);
            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.DokuSFA).HasMaxLength(50);
            entity.Property(e => e.Doku_Paket).HasMaxLength(100);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.InfoCM).HasMaxLength(50);
            entity.Property(e => e.Jenis)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.Jumhar).HasComputedColumnSql("([jumlah]*[Harga])", false);
            entity.Property(e => e.KirimKd)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.KodeRnd).HasMaxLength(255);
            entity.Property(e => e.Kode_Brg).HasMaxLength(50);
            entity.Property(e => e.Kode_Dept).HasMaxLength(20);
            entity.Property(e => e.Kode_Gudang).HasMaxLength(20);
            entity.Property(e => e.Kode_Sales).HasMaxLength(12);
            entity.Property(e => e.Kode_Tujuan).HasMaxLength(50);
            entity.Property(e => e.Kode_Valas).HasMaxLength(10);
            entity.Property(e => e.MajorAR).HasMaxLength(50);
            entity.Property(e => e.MajorCustomer).HasMaxLength(50);
            entity.Property(e => e.MajorDiskon).HasMaxLength(50);
            entity.Property(e => e.MajorHPP).HasMaxLength(50);
            entity.Property(e => e.MajorPPhJasa).HasMaxLength(50);
            entity.Property(e => e.MajorPPn).HasMaxLength(50);
            entity.Property(e => e.MajorPPnBM).HasMaxLength(50);
            entity.Property(e => e.MajorPSD).HasMaxLength(50);
            entity.Property(e => e.Memo).HasMaxLength(255);
            entity.Property(e => e.Nama_Paket).HasMaxLength(100);
            entity.Property(e => e.NewEPK).HasMaxLength(50);
            entity.Property(e => e.Nm_Brg).HasMaxLength(200);
            entity.Property(e => e.PPnBm).HasMaxLength(50);
            entity.Property(e => e.Proyekkd).HasMaxLength(30);
            entity.Property(e => e.ReferenceCustomer).HasMaxLength(50);
            entity.Property(e => e.SalesLama).HasMaxLength(20);
            entity.Property(e => e.SerialNumber).HasMaxLength(150);
            entity.Property(e => e.Spec).HasMaxLength(10);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.Sts).HasMaxLength(1);
            entity.Property(e => e.TIPEPROJECT).HasMaxLength(50);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.TglKirim).HasColumnType("smalldatetime");
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.kode_Paket).HasMaxLength(100);
            entity.Property(e => e.kode_brgGanti).HasMaxLength(50);
            entity.Property(e => e.tgl_Paket).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<SubTandaTerimaAr>(entity =>
        {
            entity.HasKey(e => e.PKbas);

            entity.ToTable("SubTandaTerimaAr");

            entity.Property(e => e.Cara).HasMaxLength(100);
            entity.Property(e => e.Doku).HasMaxLength(75);
            entity.Property(e => e.DokuKwitansiAR).HasMaxLength(50);
            entity.Property(e => e.Doku_Faktur).HasMaxLength(50);
            entity.Property(e => e.Doku_LPB).HasMaxLength(50);
            entity.Property(e => e.Doku_Muka).HasMaxLength(20);
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.Giro).HasMaxLength(50);
            entity.Property(e => e.Hapus).HasMaxLength(25);
            entity.Property(e => e.Keterangan).HasMaxLength(255);
            entity.Property(e => e.Kode_Bank).HasMaxLength(100);
            entity.Property(e => e.Kode_Customer).HasMaxLength(50);
            entity.Property(e => e.Kode_Valas).HasMaxLength(10);
            entity.Property(e => e.Kode_ValasBayar).HasMaxLength(10);
            entity.Property(e => e.MajorRef)
                .HasMaxLength(20)
                .HasDefaultValue("");
            entity.Property(e => e.Reference).HasMaxLength(50);
            entity.Property(e => e.STS).HasMaxLength(5);
            entity.Property(e => e.Status).HasMaxLength(5);
            entity.Property(e => e.SuratJalan).HasMaxLength(50);
            entity.Property(e => e.Tgl).HasColumnType("datetime");
            entity.Property(e => e.TglGiro).HasColumnType("datetime");
            entity.Property(e => e.UserID).HasMaxLength(20);
        });

        modelBuilder.Entity<SubVoucherAP>(entity =>
        {
            entity.HasKey(e => e.PKbas);

            entity.ToTable("SubVoucherAP");

            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.Doku_LPB).HasMaxLength(50);
            entity.Property(e => e.Doku_PO).HasMaxLength(50);
            entity.Property(e => e.Doku_PCF).HasMaxLength(50);
            entity.Property(e => e.TipeBiaya).HasMaxLength(20);
            entity.Property(e => e.SourceType).HasMaxLength(20);
            entity.Property(e => e.APRef).HasMaxLength(50);
            entity.Property(e => e.InvoiceNo).HasMaxLength(50);
            entity.Property(e => e.TglInvoice).HasColumnType("smalldatetime");
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Keterangan).HasMaxLength(255);
            entity.Property(e => e.Kode_Supplier).HasMaxLength(50);
            entity.Property(e => e.Kode_Valas).HasMaxLength(12);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.TglDokuLPB).HasColumnType("smalldatetime");
            entity.Property(e => e.TglDokuPO).HasColumnType("smalldatetime");
            entity.Property(e => e.Doku_FP).HasMaxLength(50);
            entity.Property(e => e.Tgl_FP).HasColumnType("smalldatetime");
            entity.Property(e => e.UserID).HasMaxLength(100);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.PKbas);

            entity.ToTable("Supplier");

            entity.Property(e => e.Alamat1).HasMaxLength(255);
            entity.Property(e => e.Alamat1Pabrik).HasMaxLength(255);
            entity.Property(e => e.Alamat1Pajak).HasMaxLength(255);
            entity.Property(e => e.Alamat2).HasMaxLength(255);
            entity.Property(e => e.Alamat2Pabrik).HasMaxLength(255);
            entity.Property(e => e.Alamat2Pajak).HasMaxLength(255);
            entity.Property(e => e.Benua).HasMaxLength(50);
            entity.Property(e => e.BenuaPabrik).HasMaxLength(50);
            entity.Property(e => e.Contact1).HasMaxLength(255);
            entity.Property(e => e.Contact2).HasMaxLength(255);
            entity.Property(e => e.Contact3).HasMaxLength(255);
            entity.Property(e => e.Contact4).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Fax).HasMaxLength(50);
            entity.Property(e => e.FaxPabrik).HasMaxLength(50);
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Jenis).HasMaxLength(50);
            entity.Property(e => e.Keterangan).HasMaxLength(255);
            entity.Property(e => e.Kode).HasMaxLength(50);
            entity.Property(e => e.KodeEPK).HasMaxLength(20);
            entity.Property(e => e.KodeGTC).HasMaxLength(20);
            entity.Property(e => e.KodeLama).HasMaxLength(50);
            entity.Property(e => e.KodePos).HasMaxLength(50);
            entity.Property(e => e.KodePosPabrik).HasMaxLength(50);
            entity.Property(e => e.KodePosPajak).HasMaxLength(50);
            entity.Property(e => e.KodeTrim).HasMaxLength(50);
            entity.Property(e => e.Kode_Area).HasMaxLength(50);
            entity.Property(e => e.Kode_Customer).HasMaxLength(20);
            entity.Property(e => e.Kode_Dept).HasMaxLength(50);
            entity.Property(e => e.Kode_Sales).HasMaxLength(50);
            entity.Property(e => e.Kode_Usaha).HasMaxLength(50);
            entity.Property(e => e.Kode_buyer).HasMaxLength(100);
            entity.Property(e => e.Kota).HasMaxLength(255);
            entity.Property(e => e.KotaPabrik).HasMaxLength(255);
            entity.Property(e => e.KotaPajak).HasMaxLength(50);
            entity.Property(e => e.LOGID).HasMaxLength(100);
            entity.Property(e => e.MOS).HasMaxLength(50);
            entity.Property(e => e.MTU).HasMaxLength(12);
            entity.Property(e => e.Major).HasMaxLength(50);
            entity.Property(e => e.NPWP).HasMaxLength(50);
            entity.Property(e => e.Nama).HasMaxLength(255);
            entity.Property(e => e.NamaPajak).HasMaxLength(100);
            entity.Property(e => e.NamaTrim).HasMaxLength(100);
            entity.Property(e => e.Negara).HasMaxLength(50);
            entity.Property(e => e.NegaraPabrik).HasMaxLength(50);
            entity.Property(e => e.NegaraPajak).HasMaxLength(50);
            entity.Property(e => e.PHD).HasMaxLength(50);
            entity.Property(e => e.PKP).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Propinsi).HasMaxLength(50);
            entity.Property(e => e.Reference).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.ServerFrom).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(2);
            entity.Property(e => e.SupGroup).HasMaxLength(100);
            entity.Property(e => e.SupGroupName).HasMaxLength(100);
            entity.Property(e => e.Telepon).HasMaxLength(50);
            entity.Property(e => e.TeleponPabrik).HasMaxLength(50);
            entity.Property(e => e.TglMasuk).HasColumnType("smalldatetime");
            entity.Property(e => e.TipeDiskon).HasMaxLength(1);
            entity.Property(e => e.TipeHarga).HasMaxLength(1);
            entity.Property(e => e.TipeHutang).HasMaxLength(10);
            entity.Property(e => e.TransferTime).HasMaxLength(50);
            entity.Property(e => e.UserID).HasMaxLength(100);
        });

        modelBuilder.Entity<SupplierGroup>(entity =>
        {
            entity.HasKey(e => e.PKbas);

            entity.ToTable("SupplierGroup");

            entity.Property(e => e.Kode).HasMaxLength(100);
            entity.Property(e => e.Nama).HasMaxLength(100);
        });

        modelBuilder.Entity<TTP>(entity =>
        {
            entity.HasKey(e => e.PKbas);

            entity.ToTable("TTP");

            entity.Property(e => e.AlmKirim).HasMaxLength(200);
            entity.Property(e => e.DOKU_SJ).HasMaxLength(50);
            entity.Property(e => e.Destination).HasMaxLength(255);
            entity.Property(e => e.Doku).HasMaxLength(100);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.EntryUpdate).HasColumnType("smalldatetime");
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Kode_Customer).HasMaxLength(100);
            entity.Property(e => e.Kode_CustomerGanti).HasMaxLength(50);
            entity.Property(e => e.Kode_Dept).HasMaxLength(50);
            entity.Property(e => e.Kode_Gudang).HasMaxLength(50);
            entity.Property(e => e.Kode_Sales).HasMaxLength(50);
            entity.Property(e => e.Kode_SubCustomer).HasMaxLength(50);
            entity.Property(e => e.Kode_gudanglama).HasMaxLength(50);
            entity.Property(e => e.NamaKirim).HasMaxLength(100);
            entity.Property(e => e.Nama_customer).HasMaxLength(50);
            entity.Property(e => e.Nama_sales).HasMaxLength(50);
            entity.Property(e => e.Sts).HasMaxLength(2);
            entity.Property(e => e.Sts_Temp).HasMaxLength(1);
            entity.Property(e => e.TGL).HasColumnType("smalldatetime");
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.UserUpdate).HasMaxLength(100);
            entity.Property(e => e.kode_area).HasMaxLength(50);
            entity.Property(e => e.kode_customerLama).HasMaxLength(50);
            entity.Property(e => e.kode_deptlama).HasMaxLength(50);
        });

        modelBuilder.Entity<TTPRetur>(entity =>
        {
            entity.HasKey(e => e.PKbas);

            entity.ToTable("TTPRetur");

            entity.Property(e => e.AlmKirim).HasMaxLength(200);
            entity.Property(e => e.DOKU_SJ).HasMaxLength(50);
            entity.Property(e => e.Destination).HasMaxLength(255);
            entity.Property(e => e.Doku).HasMaxLength(20);
            entity.Property(e => e.Doku_TTP).HasMaxLength(20);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Hapus).HasMaxLength(100);
            entity.Property(e => e.Kode_Customer).HasMaxLength(12);
            entity.Property(e => e.Kode_CustomerGanti).HasMaxLength(50);
            entity.Property(e => e.Kode_Dept).HasMaxLength(12);
            entity.Property(e => e.Kode_Gudang).HasMaxLength(10);
            entity.Property(e => e.Kode_Sales).HasMaxLength(12);
            entity.Property(e => e.Kode_SubCustomer).HasMaxLength(20);
            entity.Property(e => e.NamaKirim).HasMaxLength(100);
            entity.Property(e => e.Sts).HasMaxLength(2);
            entity.Property(e => e.Sts_Temp).HasMaxLength(1);
            entity.Property(e => e.TGL).HasColumnType("smalldatetime");
            entity.Property(e => e.Tgl_Ganti).HasColumnType("smalldatetime");
            entity.Property(e => e.UserCreate).HasMaxLength(100);
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.usernd).HasMaxLength(100);
        });

        modelBuilder.Entity<TandaTerimaAr>(entity =>
        {
            entity.HasKey(e => e.PKbas);

            entity.ToTable("TandaTerimaAr");

            entity.Property(e => e.Cara).HasMaxLength(20);
            entity.Property(e => e.Doku).HasMaxLength(75);
            entity.Property(e => e.EntryDate).HasMaxLength(30);
            entity.Property(e => e.Hapus).HasMaxLength(50);
            entity.Property(e => e.InUse).HasMaxLength(50);
            entity.Property(e => e.Jenis).HasMaxLength(20);
            entity.Property(e => e.Keterangan).HasMaxLength(255);
            entity.Property(e => e.Kode_BankCustomer).HasMaxLength(20);
            entity.Property(e => e.Kode_Customer).HasMaxLength(20);
            entity.Property(e => e.Kode_Valas).HasMaxLength(20);
            entity.Property(e => e.STS).HasMaxLength(1);
            entity.Property(e => e.StatusGL).HasMaxLength(10);
            entity.Property(e => e.StsTipe).HasMaxLength(5);
            entity.Property(e => e.Tgl).HasColumnType("datetime");
            entity.Property(e => e.UserArea).HasMaxLength(50);
            entity.Property(e => e.UserID).HasMaxLength(30);
        });

        modelBuilder.Entity<Tx_IdempotencyRecord>(entity =>
        {
            entity.HasKey(e => e.IdempotencyKey).HasName("PK__Tx_Idemp__A6D161D96D9C64EA");

            entity.ToTable("Tx_IdempotencyRecord");

            entity.Property(e => e.IdempotencyKey).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RequestHash).HasMaxLength(64);
        });

        modelBuilder.Entity<Tx_PushSubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__Tx_PushS__9A2B249D9C1914AC");

            entity.ToTable("Tx_PushSubscription");

            entity.Property(e => e.AuthKey).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Endpoint).HasMaxLength(500);
            entity.Property(e => e.P256dh).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Tx_PushSubscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tx_PushSu__UserI__04E4BC85");
        });

modelBuilder.Entity<VoucherAP>(entity =>
        {
            entity.HasKey(e => e.PKbas).HasName("PK_VoucherAP");

            entity.ToTable("VoucherAP");

            entity.Property(e => e.Doku).HasMaxLength(50);
            entity.Property(e => e.TglDoku).HasColumnType("smalldatetime");
            entity.Property(e => e.Kode_Supplier).HasMaxLength(20);
            entity.Property(e => e.Kode_Dept).HasMaxLength(20);
            entity.Property(e => e.Doku_LPB).HasMaxLength(50);
            entity.Property(e => e.Doku_PO).HasMaxLength(50);
            entity.Property(e => e.Doku_PCF).HasMaxLength(50);
            entity.Property(e => e.NOPEN).HasMaxLength(50);
            entity.Property(e => e.TglNopen).HasColumnType("smalldatetime");
            entity.Property(e => e.AWB_BL).HasMaxLength(50);
            entity.Property(e => e.TipeBiaya).HasMaxLength(10);
            entity.Property(e => e.SourceType).HasMaxLength(20);
            entity.Property(e => e.TglDokuLPB).HasColumnType("smalldatetime");
            entity.Property(e => e.TglDokuPO).HasColumnType("smalldatetime");
            entity.Property(e => e.TglJatuhTempo).HasColumnType("smalldatetime");
            entity.Property(e => e.Kode_Valas).HasMaxLength(12);
            entity.Property(e => e.Keterangan).HasMaxLength(255);
            entity.Property(e => e.STS).HasMaxLength(2);
            entity.Property(e => e.Tipe).HasMaxLength(10);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.Doku_FP).HasMaxLength(50);
            entity.Property(e => e.Tgl_FP).HasColumnType("smalldatetime");
            entity.Property(e => e.EFaktur).HasMaxLength(255);
            entity.Property(e => e.Kode_IDN).HasMaxLength(50);
            entity.Property(e => e.ModulSource).HasMaxLength(50);
            entity.Property(e => e.MajorDiskon).HasMaxLength(20);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<subTTP>(entity =>
        {
            entity.HasKey(e => e.PKbas);

            entity.ToTable("subTTP");

            entity.Property(e => e.Alias).HasMaxLength(50);
            entity.Property(e => e.Doku).HasMaxLength(100);
            entity.Property(e => e.Doku_TTP).HasMaxLength(100);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.JumlahAwal).HasMaxLength(50);
            entity.Property(e => e.KODE_CUSTOMERGANTI).HasMaxLength(50);
            entity.Property(e => e.Ket).HasMaxLength(255);
            entity.Property(e => e.KodeRnd).HasMaxLength(100);
            entity.Property(e => e.Kode_Brg).HasMaxLength(50);
            entity.Property(e => e.Kode_Customer).HasMaxLength(50);
            entity.Property(e => e.Kode_Dept).HasMaxLength(50);
            entity.Property(e => e.Kode_Gudang).HasMaxLength(50);
            entity.Property(e => e.Kode_GudangPinjaman).HasMaxLength(50);
            entity.Property(e => e.Kode_brgLama).HasMaxLength(50);
            entity.Property(e => e.Kode_deptLama).HasMaxLength(50);
            entity.Property(e => e.Kode_gudangLama).HasMaxLength(50);
            entity.Property(e => e.NAma_brg).HasMaxLength(50);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UserID).HasMaxLength(100);
            entity.Property(e => e.kode_BRGganti).HasMaxLength(50);
        });

        modelBuilder.Entity<subTTPRetur>(entity =>
        {
            entity.HasKey(e => e.PKbas);

            entity.ToTable("subTTPRetur");

            entity.Property(e => e.Alias).HasMaxLength(25);
            entity.Property(e => e.Doku).HasMaxLength(25);
            entity.Property(e => e.Doku_TTP).HasMaxLength(25);
            entity.Property(e => e.EntryDate).HasColumnType("smalldatetime");
            entity.Property(e => e.HAPUS).HasMaxLength(100);
            entity.Property(e => e.KODE_CUSTOMERGANTI).HasMaxLength(50);
            entity.Property(e => e.Ket).HasMaxLength(255);
            entity.Property(e => e.KodeRnd).HasMaxLength(100);
            entity.Property(e => e.Kode_Brg).HasMaxLength(50);
            entity.Property(e => e.Kode_BrggANTI).HasMaxLength(50);
            entity.Property(e => e.Kode_Customer).HasMaxLength(25);
            entity.Property(e => e.Kode_Dept).HasMaxLength(25);
            entity.Property(e => e.Kode_Gudang).HasMaxLength(25);
            entity.Property(e => e.Kode_GudangPinjaman).HasMaxLength(50);
            entity.Property(e => e.Tgl).HasColumnType("smalldatetime");
            entity.Property(e => e.UserID).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
