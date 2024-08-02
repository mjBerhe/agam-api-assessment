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
    Policy myPolicy = new Policy(years, 0.03, 0.16, 0.03, 0.0015, 0.2);
    myPolicy.IncrementYears(20);
    return myPolicy;
  }

  public static Policy GetStandardPolicy(
    int incYears,
    double fund1Return,
    double volatilityRate,
    double riskFreeRate,
    double fundFeeRate,
    double fund1Size
  )
  {
    Policy myPolicy = new Policy(60, fund1Return, volatilityRate, riskFreeRate, fundFeeRate, fund1Size);
    myPolicy.IncrementYears(incYears);
    // Console.WriteLine(myPolicy._policyRecords);
    return myPolicy;
  }
}

