using Microsoft.EntityFrameworkCore;
namespace PolicyStore.DB;


class PolicyDb : DbContext
{
  public PolicyDb(DbContextOptions<PolicyDb> options)
      : base(options) { }

  public DbSet<Policy> Policies => Set<Policy>();
}

public class PolicyDB
{
  public static Policy GetPolicy(int years)
  {
    Policy myPolicy = new Policy(years, 0.03, 0.035);
    myPolicy.IncrementYears(20);
    return myPolicy;
  }

  public static Policy GetStandardPolicy(int incYears, double fund1Return, double fund2Return)
  {
    Policy myPolicy = new Policy(60, fund1Return, fund2Return);
    myPolicy.IncrementYears(incYears);
    Console.WriteLine(myPolicy._policyRecords);
    return myPolicy;
  }
}

