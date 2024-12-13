using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FileDowanloaderPP.Models
{
    public class Context : DbContext
    {
        public Context() : base() { }
        public Context(DbContextOptions options): base(options) 
        { 

        }
        public DbSet<FileDowalod> Files { get; set; }
        
    }
}
