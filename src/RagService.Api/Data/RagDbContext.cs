using Microsoft.EntityFrameworkCore;
using RagService.Api.Entities;

namespace RagService.Api.Data;

public class RagDbContext : DbContext
{
    public RagDbContext(DbContextOptions<RagDbContext> options)
        : base(options)
    {
    }

    public DbSet<DocumentChunk> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<DocumentChunk>()
            .Property(x => x.Metadata)
            .HasColumnType("jsonb");
            
        // We use vector(1024) for OpenRouter liquid/lfm2.5-embedding-350m:free
        modelBuilder.Entity<DocumentChunk>()
            .Property(x => x.Embedding)
            .HasColumnType("vector(1024)");
            
        // Add a vector index for faster cosine distance queries
        modelBuilder.Entity<DocumentChunk>()
            .HasIndex(x => x.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");
    }
}
