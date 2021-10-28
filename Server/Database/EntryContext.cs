using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TestServer.Database
{
    public partial class EntryContext : DbContext
    {
        public EntryContext()
        {
        }

        public EntryContext(DbContextOptions<EntryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Showroom> Showrooms { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=postgres;Username=postgres;Password=root;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("adminpack")
                .HasAnnotation("Relational:Collation", "Russian_Russia.1251");

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasIndex(e => e.ProfileId, "profiles_profileid_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.ProfileName, "profiles_profilename_uindex")
                    .IsUnique();

                entity.Property(e => e.ProfileName)
                    .IsRequired()
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<Showroom>(entity =>
            {
                entity.HasIndex(e => e.Id, "showrooms_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "showrooms_name_uindex")
                    .IsUnique();

                entity.Property(e => e.CameraPosition).HasColumnType("json");

                entity.Property(e => e.FirstVehicleSpawnPoint).HasColumnType("json");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.Property(e => e.SecondVehicleSpawnPoint).HasColumnType("json");

                entity.Property(e => e.TestDriveVehicleSpawnPoint).HasColumnType("json");

                entity.Property(e => e.VehiclesList).HasColumnType("json");

                entity.Property(e => e.X).HasColumnName("x");

                entity.Property(e => e.Y).HasColumnName("y");

                entity.Property(e => e.Z).HasColumnName("z");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Showrooms)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("showrooms_profiles_profileid_fk");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasIndex(e => e.VehicleId, "vehicles_vehicleid_uindex")
                    .IsUnique();

                entity.Property(e => e.ModelName)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.Property(e => e.X).HasColumnName("x");

                entity.Property(e => e.Y).HasColumnName("y");

                entity.Property(e => e.Z).HasColumnName("z");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("vehicles_profiles_profileId_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
