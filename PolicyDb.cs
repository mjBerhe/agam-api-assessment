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
    Policy myPolicy = new Policy(years, 1, 0.03, 0.16, 0.03, 0.0015, 0.2);
    myPolicy.IncrementYears(20);
    return myPolicy;
  }

  public static Policy GetStandardPolicy(
    int initialAge,
    double qxMultiplier,
    double fund1Return,
    double volatilityRate,
    double riskFreeRate,
    double fundFeeRate,
    double fund1Size
  )
  {
    Policy myPolicy = new Policy(initialAge, qxMultiplier, fund1Return, volatilityRate, riskFreeRate, fundFeeRate, fund1Size);
    myPolicy.IncrementYears(100 - initialAge);
    // Console.WriteLine(myPolicy._policyRecords);
    return myPolicy;
  }
}

