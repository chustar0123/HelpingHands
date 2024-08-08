using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Helping_Hands.Models;

public partial class Grp0410HelpingHandsContext : DbContext
{
    public Grp0410HelpingHandsContext()
    {
    }

    public Grp0410HelpingHandsContext(DbContextOptions<Grp0410HelpingHandsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Business> Businesses { get; set; }

    public virtual DbSet<CareContract> CareContracts { get; set; }

    public virtual DbSet<CareVisit> CareVisits { get; set; }

    public virtual DbSet<ChronicCondition> ChronicConditions { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Nurse> Nurses { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PatientCondition> PatientConditions { get; set; }

    public virtual DbSet<PreferredSuburb> PreferredSuburbs { get; set; }

    public virtual DbSet<Suburb> Suburbs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=SICT-SQL.MANDELA.AC.ZA;Initial Catalog=GRP-04-10-HelpingHands;User ID=GRP-04-10;Password=grp-04-10-2023#;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.BusinessId).HasName("PK__Business__F1EAA34E1D398C07");

            entity.ToTable("Business");

            entity.Property(e => e.BusinessId).HasColumnName("BusinessID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nponumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NPONumber");
            entity.Property(e => e.OperatingHours)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OrganizationName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CareContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__CareCont__C90D3409113E9BF4");

            entity.ToTable("CareContract");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.AddressLine1)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContractDate).HasColumnType("date");
            entity.Property(e => e.ContractStatus)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.EndCareDate).HasColumnType("date");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.StartCareDate).HasColumnType("date");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.SuburbId).HasColumnName("SuburbID");
            entity.Property(e => e.WoundDescription)
                .IsRequired()
                .IsUnicode(false);

            entity.HasOne(d => d.AssignedNurseNavigation).WithMany(p => p.CareContracts)
                .HasForeignKey(d => d.AssignedNurse)
                .HasConstraintName("FK_CareContract_Nurses");

            entity.HasOne(d => d.Patient).WithMany(p => p.CareContracts)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CareContr__Patie__51300E55");

            entity.HasOne(d => d.Suburb).WithMany(p => p.CareContracts)
                .HasForeignKey(d => d.SuburbId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CareContr__Subur__5224328E");
        });

        modelBuilder.Entity<CareVisit>(entity =>
        {
            entity.HasKey(e => e.VisitId).HasName("PK__CareVisi__4D3AA1BE9B7D8EAD");

            entity.ToTable("CareVisit");

            entity.Property(e => e.VisitId)
            .ValueGeneratedOnAdd()
            .HasColumnName("VisitID");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.Notes)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.NurseId).HasColumnName("NurseID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.VisitDate).HasColumnType("date");
            entity.Property(e => e.WoundCondition)
                .IsRequired()
                .HasMaxLength(1000)
                .IsUnicode(false);

            entity.HasOne(d => d.Contract).WithMany(p => p.CareVisits)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__CareVisit__Contr__5629CD9C");

            entity.HasOne(d => d.Nurse).WithMany(p => p.CareVisits)
                .HasForeignKey(d => d.NurseId)
                .HasConstraintName("FK__CareVisit__Nurse__5535A963");
        });

        modelBuilder.Entity<ChronicCondition>(entity =>
        {
            entity.HasKey(e => e.ConditionId).HasName("PK__ChronicC__37F5C0EF48CA2D52");

            entity.Property(e => e.ConditionId).HasColumnName("ConditionID");
            entity.Property(e => e.ConditionName)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(300);
            entity.Property(e => e.Status)
                .HasMaxLength(7)
                .IsFixedLength();
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__City__F2D21A96183800E8");

            entity.ToTable("City");

            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.CityAbbreviation)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CityName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Nurse>(entity =>
        {
            entity.HasKey(e => e.NurseId).HasName("PK__Nurses__4384786943BFBCCB");

            entity.Property(e => e.NurseId)
                .ValueGeneratedNever()
                .HasColumnName("NurseID");
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(15);
            entity.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.Idnumber)
                .IsRequired()
                .HasMaxLength(13)
                .HasColumnName("IDNumber");
            entity.Property(e => e.Surname)
                .IsRequired()
                .HasMaxLength(15);

            entity.HasOne(d => d.NurseNavigation).WithOne(p => p.Nurse)
                .HasForeignKey<Nurse>(d => d.NurseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Nurses_Users");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC34664BEFD68");

            entity.Property(e => e.PatientId)
                .ValueGeneratedNever()
                .HasColumnName("PatientID");
            entity.Property(e => e.AdditionalInformation).IsUnicode(false);
            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.EmergencyContactNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.EmergencyContactPerson)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Idnumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDNumber");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.PatientNavigation).WithOne(p => p.Patient)
                .HasForeignKey<Patient>(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patients_Users");
        });

        modelBuilder.Entity<PatientCondition>(entity =>
        {
            entity.HasKey(e => new { e.PatientId, e.ConditionId }).HasName("PK__Patient___74719F4857EA5765");

            entity.ToTable("Patient_Conditions");

            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.ConditionId).HasColumnName("ConditionID");
            entity.Property(e => e.PatientConditionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("PatientConditionID");

            entity.HasOne(d => d.Condition).WithMany(p => p.PatientConditions)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Patient_C__Condi__4BAC3F29");

            entity.HasOne(d => d.Patient).WithMany(p => p.PatientConditions)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Patient_C__Patie__4AB81AF0");
        });

        modelBuilder.Entity<PreferredSuburb>(entity =>
        {
            entity.HasKey(e => new { e.NurseId, e.SuburbId }).HasName("PK__Preferre__483691C7DE9B844D");

            entity.ToTable("PreferredSuburb");

            entity.Property(e => e.NurseId).HasColumnName("NurseID");
            entity.Property(e => e.SuburbId).HasColumnName("SuburbID");
            entity.Property(e => e.PreferredSuburbId)
                .ValueGeneratedOnAdd()
                .HasColumnName("PreferredSuburbID");

            entity.HasOne(d => d.Nurse).WithMany(p => p.PreferredSuburbs)
                .HasForeignKey(d => d.NurseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Preferred__Nurse__59063A47");

            entity.HasOne(d => d.Suburb).WithMany(p => p.PreferredSuburbs)
                .HasForeignKey(d => d.SuburbId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Preferred__Subur__59FA5E80");
        });

        modelBuilder.Entity<Suburb>(entity =>
        {
            entity.HasKey(e => e.SuburbId).HasName("PK__Suburb__BB2E9AE14CFB46E0");

            entity.ToTable("Suburb");

            entity.Property(e => e.SuburbId).HasColumnName("SuburbID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SuburbName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.City).WithMany(p => p.Suburbs)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK__Suburb__CityID__4222D4EF");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC154E0280");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.DateAssigned).HasColumnType("date");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.UserType)
                .IsRequired()
                .HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
