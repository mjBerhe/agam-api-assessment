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
    Policy myPolicy = new Policy(years);
    myPolicy.IncrementYears(20);
    return myPolicy;
  }

  public static Policy GetStandardPolicy()
  {
    Policy myPolicy = new Policy(60);
    myPolicy.IncrementYears(40);
    Console.WriteLine(myPolicy._policyRecords);
    return myPolicy;
  }
}

