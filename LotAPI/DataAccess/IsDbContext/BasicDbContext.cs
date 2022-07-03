using LotAPI.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace LotAPI.DataAccess.IsDbContext
{
    public class BasicDbContext : DbContext
    {
        public BasicDbContext(DbContextOptions<BasicDbContext> options) : base(options) { }

        #region Entity mapping db table
        public virtual DbSet<LotMaster> LotMaster { get; set; }
        public virtual DbSet<LotAwardList> LotAwardList { get; set; }
        public virtual DbSet<LotAwardMan> LotAwardMan { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            OracleModelSettings(modelBuilder);

            #region Configure the primary key of the table
            modelBuilder.Entity<LotMaster>().HasKey(x => new { x.LotMasterId });
            modelBuilder.Entity<LotAwardList>().HasKey(x => new { x.LotMasterId, x.Seq });
            modelBuilder.Entity<LotAwardMan>().HasKey(x => new { x.LotMasterId, x.AwardManNumber });
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        public void OracleModelSettings(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entity.Name, builder =>
                {
                    builder.ToTable(entity.ClrType.Name.ToUpper());
                    foreach (var property in entity.GetProperties())
                    {
                        builder.Property(property.Name).HasColumnName(property.Name.ToUpper());
                    }
                });
            }
        }
    }
}
