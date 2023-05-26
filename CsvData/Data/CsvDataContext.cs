using CsvData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsvData.Data
{
    public class CsvDataContext: DbContext
    {
        public CsvDataContext(DbContextOptions<CsvDataContext> options)
            : base(options)
        {
        }

        public DbSet<CsvDataModel> CsvData { get; set; }
    }
}
