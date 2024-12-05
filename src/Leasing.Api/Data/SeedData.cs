using Leasing.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Api.Data;

public static class SeedData
{
    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<Equipment>().HasData(
            new { Code = 1, Name = "Equipment_1", Area = 1000f, },
            new { Code = 2, Name = "Equipment_2", Area = 150f, },
            new { Code = 3, Name = "Equipment_3", Area = 250f, });
        
        builder.Entity<ProductionFacility>().HasData(
            new { Code = 1, Name = "facility_1", Area = 1000f, },
            new { Code = 2, Name = "facility_2", Area = 1500f, },
            new { Code = 3, Name = "facility_3", Area = 2000f, });
    }
}