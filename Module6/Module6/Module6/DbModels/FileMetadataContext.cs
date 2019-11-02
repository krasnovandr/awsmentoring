using Microsoft.EntityFrameworkCore;

namespace Module6.DbModels
{
    public class FileMetadataContext : DbContext
    {
        public FileMetadataContext()
        {
        }

        public FileMetadataContext(DbContextOptions<FileMetadataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Metadata> Metadata { get; set; }
    }
}