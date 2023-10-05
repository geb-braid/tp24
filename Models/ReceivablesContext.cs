using Microsoft.EntityFrameworkCore;

namespace tp24_api.Models;

public class ReceivablesContext : DbContext
{
    public ReceivablesContext(DbContextOptions<ReceivablesContext> options)
        : base(options)
    {
    }

    public DbSet<Receivable> Receivables { get; set; } = null!;
}