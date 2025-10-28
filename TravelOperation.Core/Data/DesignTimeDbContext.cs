using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelOperation.Core.Data
{
    public class DesignTimeDBContextFactory : IDesignTimeDbContextFactory<TravelDbContext>
    {
        public TravelDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TravelDbContext>();
            optionsBuilder.UseSqlite("Data Source=TravelExpense.db");
            
            // Do NOT add AuditInterceptor here
            return new TravelDbContext(optionsBuilder.Options);
        }
    }
}
