using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;


public class PolicyRecord
{
  public int year { get; set; }
  public int age { get; set; }
  public double contribution { get; set; }
  public double avPreFee { get; set; }
  public double fund1PreFee { get; set; }
  public double fund2PreFee { get; set; }
  public double fundFees { get; set; }
  public double onlyFundFees { get; set; }
  public double avPreWithdrawal { get; set; }
  public double fund1PreWithdrawal { get; set; }
  public double fund2PreWithdrawal { get; set; }
  public double withdrawalAmount { get; set; }
  public double avPostWithdrawal { get; set; }
  public double fund1PostWithdrawal { get; set; }
  public double fund2PostWithdrawal { get; set; }
  public double riderCharge { get; set; }
  public double avPostCharges { get; set; }
  public double fund1PostCharges { get; set; }
  public double fund2PostCharges { get; set; }
  public double deathPayments { get; set; }
  public double avPostDeathClaims { get; set; }
  public double fund1PostDeathClaims { get; set; }
  public double fund2PostDeathClaims { get; set; }
  public double fund1PostRebalance { get; set; }
  public double fund2PostRebalance { get; set; }
  public double ROPDeathBase { get; set; }
  public double NARDeathClaims { get; set; }
  public double riderDeathBenefitBase { get; set; }
  public double riderWithdrawalBase { get; set; }
  public double riderWithdrawalAmount { get; set; }
  public double riderCumulativeWithdrawal { get; set; }
  public double riderMaxAnnualWithdrawal { get; set; }
  public double riderMaxAnnualWithdrawalRate { get; set; }
  public double eligibleStepUp { get; set; }
  public double growthPhase { get; set; }
  public double withdrawalPhase { get; set; }
  public double automaticPeriodicBenefitStatus { get; set; }
  public double lastDeath { get; set; }
  public double fund1Return { get; set; }
  public double fund2Return { get; set; }
  public double rebalanceIndicator { get; set; }
  public double DF { get; set; }
  public double QX { get; set; }
  public double finalDeathClaims { get; set; }
  public double finalWithdrawalClaims { get; set; }
  public double finalRiderCharges { get; set; }
  public double finalFundFees { get; set; }
  public double fund1InterestCredited { get; set; }
  public double fund2InterestCredited { get; set; }
  public double totalInterestCredited { get; set; }
}

public class Policy
{
  public int age { get; set; }
  public int year;
  double fund1Size;
  const int INITIAL_PREMIUM = 100000;
  const double STEP_UP_RATE = 0.06;
  const int STEP_UP_PERIOD = 10;

  const int FIRST_WITHDRAWAL_AGE = 70;
  const int ANNUITY_COMMENCEMENT_AGE = 80;
  const int LAST_DEATH_AGE = 100;

  const double RIDER_CHARGE_RATE = 0.0085;
  const double WITHDRAWAL_RATE = 0.03;
  const double FUNDS_AUTOMATIC_REBALANCING_TARGET = 0.2;

  // const double RISK_FREE_RATE = 0.03; // input
  // const double FUND_FEE_RATE = 0.0015; // input
  double riskFreeRate, fundFeeRate, volatilityRate;
  const double M_AND_E_RATE = 0.014;

  const double AGE_1 = 59.5;
  const double AGE_2 = 65;
  const double AGE_3 = 76;
  const double AGE_4 = 80;

  const double MAX_ANNUAL_WITHDRAWAL_1 = 0.04;
  const double MAX_ANNUAL_WITHDRAWAL_2 = 0.05;
  const double MAX_ANNUAL_WITHDRAWAL_3 = 0.06;
  const double MAX_ANNUAL_WITHDRAWAL_4 = 0.07;


  double contribution, withdrawalAmount, riderCharge, deathPayments;
  double fundFees, onlyFundFees;

  double ROPDeathBase, NARDeathClaims;
  double riderDeathBenefitBase, riderWithdrawalBase, riderWithdrawalAmount, riderCumulativeWithdrawal, riderMaxAnnualWithdrawal, riderMaxAnnualWithdrawalRate;

  int eligibleStepUp, growthPhase, withdrawalPhase, automaticPeriodicBenefitStatus, lastDeath, rebalanceIndicator;
  double fund1Return, fund2Return, DF, QX;

  public double avPreFee, fund1PreFee, fund2PreFee;
  double avPreWithdrawal, fund1PreWithdrawal, fund2PreWithdrawal;
  double avPostWithdrawal, fund1PostWithdrawal, fund2PostWithdrawal;
  double avPostCharges, fund1PostCharges, fund2PostCharges;
  double avPostDeathClaims, fund1PostDeathClaims, fund2PostDeathClaims;
  public double fund1PostRebalance, fund2PostRebalance;

  double finalDeathClaims, finalWithdrawalClaims, finalRiderCharges, finalFundFees;
  double fund1InterestCredited, fund2InterestCredited, totalInterestCredited;

  public double cumulativeDeathClaims { get; set; }
  public double cumulativeWithdrawalClaims { get; set; }
  public double cumulativeRiderCharges { get; set; }
  public double cumulativeFundFees { get; set; }
  public List<PolicyRecord> _policyRecords = new List<PolicyRecord>();
  public IList<PolicyRecord> PolicyRecords { get { return _policyRecords; } }

  static double GenerateStandardNormal()
  {
    Random random = new Random();
    double u1 = random.NextDouble();
    double u2 = random.NextDouble();
    double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

    return randStdNormal;

  }


  public Policy(
    int initialAge,
    double qxMultiplier,
    double initialFund1Return,
    double volatilityRate,
    double riskFreeRate,
    double fundFeeRate,
    double fund1Size
  )
  {
    this.age = initialAge;
    this.year = 0;

    this.riskFreeRate = riskFreeRate;
    this.fundFeeRate = fundFeeRate;
    this.volatilityRate = volatilityRate;
    this.fund1Return = initialFund1Return;
    this.fund2Return = 0;
    // this.fund2Return = Math.Exp(Math.Log(1 + riskFreeRate) - 0.5 * Math.Pow(volatilityRate, 2) + volatilityRate * GenerateStandardNormal()) - 1;
    this.DF = Math.Pow(1 + riskFreeRate, year);
    this.QX = 0.005 * qxMultiplier; // PARAM??


    this.contribution = 0; // PARAM
    this.fundFees = 0;
    this.onlyFundFees = 0;
    this.withdrawalAmount = 0; // PARAM??
    this.riderCharge = 0;
    this.deathPayments = 0;

    this.eligibleStepUp = 0;
    this.growthPhase = 0;
    this.withdrawalPhase = 0;
    this.automaticPeriodicBenefitStatus = 0;
    this.lastDeath = 0;
    this.rebalanceIndicator = 0;

    this.fund1PreFee = INITIAL_PREMIUM * fund1Size * 0.8;
    this.fund2PreFee = INITIAL_PREMIUM * (1 - fund1Size) * 0.8;
    this.avPreFee = fund1PreFee + fund2PreFee;

    this.avPreWithdrawal = avPreFee + contribution - fundFees;
    this.fund1PreWithdrawal = avPreWithdrawal == 0 ? 0 : fund1PreFee * (avPreWithdrawal / avPreFee);
    this.fund2PreWithdrawal = avPreWithdrawal == 0 ? 0 : fund2PreFee * (avPreWithdrawal / avPreFee);

    this.avPostWithdrawal = avPreWithdrawal;
    this.fund1PostWithdrawal = avPostWithdrawal == 0 ? 0 : fund1PreWithdrawal * (avPostWithdrawal / avPreWithdrawal);
    this.fund2PostWithdrawal = avPostWithdrawal == 0 ? 0 : fund2PreWithdrawal * (avPostWithdrawal / avPreWithdrawal);

    this.avPostCharges = avPostWithdrawal - riderCharge;
    this.fund1PostCharges = avPostCharges == 0 ? 0 : fund1PostWithdrawal * (avPostCharges / avPostWithdrawal);
    this.fund2PostCharges = avPostCharges == 0 ? 0 : fund2PostWithdrawal * (avPostCharges / avPostWithdrawal);

    this.avPostDeathClaims = Math.Max(0, avPostCharges - deathPayments);
    this.fund1PostDeathClaims = avPostDeathClaims == 0 ? 0 : fund1PostCharges * (avPostDeathClaims / avPostCharges);
    this.fund2PostDeathClaims = avPostDeathClaims == 0 ? 0 : fund2PostCharges * (avPostDeathClaims / avPostCharges);

    this.fund1PostRebalance = rebalanceIndicator == 1 ? avPostDeathClaims * FUNDS_AUTOMATIC_REBALANCING_TARGET : fund1PostDeathClaims;
    this.fund2PostRebalance = avPostCharges - fund1PostRebalance;

    this.ROPDeathBase = INITIAL_PREMIUM;
    this.NARDeathClaims = Math.Max(0, deathPayments - avPostCharges);

    this.riderDeathBenefitBase = INITIAL_PREMIUM;
    this.riderWithdrawalBase = INITIAL_PREMIUM;
    this.riderWithdrawalAmount = 0;
    this.riderCumulativeWithdrawal = 0;
    this.riderMaxAnnualWithdrawal = 0;
    this.riderMaxAnnualWithdrawalRate = 0;

    this.finalDeathClaims = 0;
    this.finalWithdrawalClaims = 0;
    this.finalRiderCharges = 0;
    this.finalFundFees = 0;

    this.fund1InterestCredited = 0;
    this.fund2InterestCredited = 0;
    this.totalInterestCredited = 0;

    this.cumulativeDeathClaims = 0;
    this.cumulativeWithdrawalClaims = 0;
    this.cumulativeRiderCharges = 0;
    this.cumulativeFundFees = 0;

    _policyRecords.Add(new PolicyRecord
    {
      age = age,
      year = year,
      contribution = contribution,
      avPreFee = avPreFee,
      fund1PreFee = fund1PreFee,
      fund2PreFee = fund2PreFee,
      fundFees = fundFees,
      onlyFundFees = onlyFundFees,
      avPreWithdrawal = avPreWithdrawal,
      fund1PreWithdrawal = fund1PreWithdrawal,
      fund2PreWithdrawal = fund2PreWithdrawal,
      withdrawalAmount = withdrawalAmount,
      avPostWithdrawal = avPostWithdrawal,
      fund1PostWithdrawal = fund1PostWithdrawal,
      fund2PostWithdrawal = fund2PostWithdrawal,
      riderCharge = riderCharge,
      avPostCharges = avPostCharges,
      fund1PostCharges = fund1PostCharges,
      fund2PostCharges = fund2PostCharges,
      deathPayments = deathPayments,
      avPostDeathClaims = avPostDeathClaims,
      fund1PostDeathClaims = fund1PostDeathClaims,
      fund2PostDeathClaims = fund2PostDeathClaims,
      fund1PostRebalance = fund1PostRebalance,
      fund2PostRebalance = fund2PostRebalance,
      ROPDeathBase = ROPDeathBase,
      NARDeathClaims = NARDeathClaims,
      riderDeathBenefitBase = riderDeathBenefitBase,
      riderWithdrawalBase = riderWithdrawalBase,
      riderWithdrawalAmount = riderWithdrawalAmount,
      riderCumulativeWithdrawal = riderCumulativeWithdrawal,
      riderMaxAnnualWithdrawal = riderMaxAnnualWithdrawal,
      riderMaxAnnualWithdrawalRate = riderMaxAnnualWithdrawalRate,
      eligibleStepUp = eligibleStepUp,
      growthPhase = growthPhase,
      withdrawalPhase = withdrawalPhase,
      automaticPeriodicBenefitStatus = automaticPeriodicBenefitStatus,
      lastDeath = lastDeath,
      fund1Return = fund1Return,
      fund2Return = fund2Return,
      rebalanceIndicator = rebalanceIndicator,
      DF = DF,
      QX = QX,
      finalDeathClaims = finalDeathClaims,
      finalWithdrawalClaims = finalWithdrawalClaims,
      finalRiderCharges = finalRiderCharges,
      finalFundFees = finalFundFees,
      fund1InterestCredited = fund1InterestCredited,
      fund2InterestCredited = fund2InterestCredited,
      totalInterestCredited = totalInterestCredited
    });
  }

  public void IncrementYears(int years)
  {
    for (int i = 0; i < years; i++)
    {
      age++;
      year++;

      double oldAvPostDeathClaims = avPostDeathClaims;
      double oldWithdrawalAmount = withdrawalAmount;
      double oldROPDeathBase = ROPDeathBase;
      double oldRiderDeathBenefitBase = riderDeathBenefitBase;
      double oldWithdrawalPhase = withdrawalPhase;

      // PRIORITY UPDATES
      // #1
      DF = Math.Round(Math.Pow(1 + riskFreeRate, year * -1), 4);
      fund2Return = Math.Exp(Math.Log(1 + riskFreeRate) - 0.5 * Math.Pow(volatilityRate, 2) + volatilityRate * GenerateStandardNormal()) - 1;
      fundFees = oldAvPostDeathClaims * (M_AND_E_RATE + fundFeeRate);
      onlyFundFees = oldAvPostDeathClaims * fundFeeRate;

      fund1PreFee = fund1PostRebalance * (1 + fund1Return);
      fund2PreFee = fund2PostRebalance * (1 + fund2Return);
      avPreFee = fund1PreFee + fund2PreFee;

      fund1InterestCredited = fund1PreFee - fund1PostRebalance;
      fund2InterestCredited = fund2PreFee - fund2PostRebalance;
      totalInterestCredited = fund1InterestCredited + fund2InterestCredited;
      // Console.WriteLine($"{fund1PreFee} {fund2PreFee} {avPreFee}");

      // #2
      avPreWithdrawal = Math.Max(0, avPreFee + contribution - fundFees);
      fund1PreWithdrawal = avPreWithdrawal == 0 ? 0 : fund1PreFee * (avPreWithdrawal / avPreFee);
      fund2PreWithdrawal = avPreWithdrawal == 0 ? 0 : fund2PreFee * (avPreWithdrawal / avPreFee);
      // Console.WriteLine($"{fundFees} {avPreWithdrawal} {fund1PreWithdrawal} {fund2PreWithdrawal}");

      // #3
      growthPhase = age <= FIRST_WITHDRAWAL_AGE && age <= ANNUITY_COMMENCEMENT_AGE && age < LAST_DEATH_AGE ? 1 : 0;
      automaticPeriodicBenefitStatus = age >= LAST_DEATH_AGE ? 0 : oldWithdrawalPhase == 1 && oldAvPostDeathClaims == 0 ? 1 : automaticPeriodicBenefitStatus;
      withdrawalPhase = (age > FIRST_WITHDRAWAL_AGE || age > ANNUITY_COMMENCEMENT_AGE) && oldAvPostDeathClaims > 0 && age < LAST_DEATH_AGE ? 1 : 0;
      lastDeath = age == LAST_DEATH_AGE ? 1 : 0;
      eligibleStepUp = year <= STEP_UP_PERIOD && growthPhase == 1 ? 1 : 0;
      rebalanceIndicator = withdrawalPhase + automaticPeriodicBenefitStatus;
      // Console.WriteLine($"{eligibleStepUp} {growthPhase} {withdrawalPhase} {automaticPeriodicBenefitStatus} {lastDeath} {rebalanceIndicator}");

      // #4
      // we may have to diverge here
      if (withdrawalPhase == 0 && automaticPeriodicBenefitStatus == 0)
      {
        withdrawalAmount = 0;
        avPostWithdrawal = Math.Max(0, avPreWithdrawal - withdrawalAmount);
        fund1PostWithdrawal = avPostWithdrawal == 0 ? 0 : fund1PreWithdrawal * (avPostWithdrawal / avPreWithdrawal);
        fund2PostWithdrawal = avPostWithdrawal == 0 ? 0 : fund2PreWithdrawal * (avPostWithdrawal / avPreWithdrawal);
        // Console.WriteLine($"{withdrawalAmount} {avPostWithdrawal} {fund1PostWithdrawal} {fund2PostWithdrawal}");

        // #5
        riderCharge = RIDER_CHARGE_RATE * avPostWithdrawal;
        avPostCharges = avPostWithdrawal - riderCharge;
        fund1PostCharges = avPostCharges == 0 ? 0 : fund1PostWithdrawal * (avPostCharges / avPostWithdrawal);
        fund2PostCharges = avPostCharges == 0 ? 0 : fund2PostWithdrawal * (avPostCharges / avPostWithdrawal);
        // Console.WriteLine($"{riderCharge} {avPostCharges} {fund1PostCharges} {fund2PostCharges}");

        // #6
        deathPayments = growthPhase + withdrawalPhase + automaticPeriodicBenefitStatus + lastDeath == 0 ? 0 : Math.Max(oldRiderDeathBenefitBase, oldROPDeathBase) * QX;
        avPostDeathClaims = Math.Max(avPostCharges - deathPayments, 0);
        fund1PostDeathClaims = avPostDeathClaims == 0 ? 0 : fund1PostCharges * (avPostDeathClaims / avPostCharges);
        fund2PostDeathClaims = avPostDeathClaims == 0 ? 0 : fund2PostCharges * (avPostDeathClaims / avPostCharges);
        // Console.WriteLine($"{deathPayments} {avPostDeathClaims} {fund1PostDeathClaims} {fund2PostDeathClaims}");

        fund1PostRebalance = rebalanceIndicator == 1 ? avPostDeathClaims * FUNDS_AUTOMATIC_REBALANCING_TARGET : fund1PostDeathClaims;
        fund2PostRebalance = avPostCharges - fund1PostRebalance;
        // Console.WriteLine($"{fund1PostRebalance} {fund2PostRebalance}");

        // #7

        riderDeathBenefitBase = Math.Max(0, riderDeathBenefitBase * (1 - QX) + contribution - fundFees - oldWithdrawalAmount - riderCharge);
        riderWithdrawalBase = Math.Max(Math.Max(growthPhase == 1 ? avPostDeathClaims : 0, riderWithdrawalBase * (1 - QX) + contribution), eligibleStepUp == 1 ? riderWithdrawalBase * (1 - QX) * (1 + STEP_UP_RATE) + contribution - fundFees - riderCharge : 0);
        riderMaxAnnualWithdrawalRate = growthPhase == 1 ? 0 : age > AGE_4 ? MAX_ANNUAL_WITHDRAWAL_4 : age > AGE_3 ? MAX_ANNUAL_WITHDRAWAL_3 : age > AGE_2 ? MAX_ANNUAL_WITHDRAWAL_2 : age > AGE_1 ? MAX_ANNUAL_WITHDRAWAL_1 : 0;
        riderMaxAnnualWithdrawal = riderMaxAnnualWithdrawalRate * riderWithdrawalBase;
        riderWithdrawalAmount = withdrawalPhase == 1 ? WITHDRAWAL_RATE * riderWithdrawalBase : automaticPeriodicBenefitStatus == 1 ? riderMaxAnnualWithdrawal : 0;
        // Console.WriteLine($"{riderDeathBenefitBase} {riderWithdrawalBase} {riderWithdrawalAmount} {riderMaxAnnualWithdrawal} {riderMaxAnnualWithdrawalRate}");
      }
      else
      {
        // something is getting withdrawn --> GROWTH PHASE = 0 && ELIGIBLE UP PHASE = 0
        // #4
        riderMaxAnnualWithdrawalRate = growthPhase == 1 ? 0 : age > AGE_4 ? MAX_ANNUAL_WITHDRAWAL_4 : age > AGE_3 ? MAX_ANNUAL_WITHDRAWAL_3 : age > AGE_2 ? MAX_ANNUAL_WITHDRAWAL_2 : age > AGE_1 ? MAX_ANNUAL_WITHDRAWAL_1 : 0;
        riderWithdrawalBase = riderWithdrawalBase * (1 - QX) + contribution;
        riderMaxAnnualWithdrawal = riderMaxAnnualWithdrawalRate * riderWithdrawalBase;
        riderWithdrawalAmount = withdrawalPhase == 1 ? WITHDRAWAL_RATE * riderWithdrawalBase : automaticPeriodicBenefitStatus == 1 ? riderMaxAnnualWithdrawal : 0;

        withdrawalAmount = riderWithdrawalAmount;
        avPostWithdrawal = Math.Max(0, avPreWithdrawal - withdrawalAmount);
        fund1PostWithdrawal = avPostWithdrawal == 0 ? 0 : fund1PreWithdrawal * (avPostWithdrawal / avPreWithdrawal);
        fund2PostWithdrawal = avPostWithdrawal == 0 ? 0 : fund2PreWithdrawal * (avPostWithdrawal / avPreWithdrawal);
        // Console.WriteLine($"{withdrawalAmount} {avPostWithdrawal} {fund1PostWithdrawal} {fund2PostWithdrawal}");

        // #5
        riderCharge = RIDER_CHARGE_RATE * avPostWithdrawal;
        avPostCharges = avPostWithdrawal - riderCharge;
        fund1PostCharges = avPostCharges == 0 ? 0 : fund1PostWithdrawal * (avPostCharges / avPostWithdrawal);
        fund2PostCharges = avPostCharges == 0 ? 0 : fund2PostWithdrawal * (avPostCharges / avPostWithdrawal);
        // Console.WriteLine($"{riderCharge} {avPostCharges} {fund1PostCharges} {fund2PostCharges}");

        // #6
        deathPayments = growthPhase + withdrawalPhase + automaticPeriodicBenefitStatus + lastDeath == 0 ? 0 : Math.Max(oldRiderDeathBenefitBase, oldROPDeathBase) * QX;
        avPostDeathClaims = Math.Max(avPostCharges - deathPayments, 0);
        fund1PostDeathClaims = avPostDeathClaims == 0 ? 0 : fund1PostCharges * (avPostDeathClaims / avPostCharges);
        fund2PostDeathClaims = avPostDeathClaims == 0 ? 0 : fund2PostCharges * (avPostDeathClaims / avPostCharges);
        // Console.WriteLine($"{deathPayments} {avPostDeathClaims} {fund1PostDeathClaims} {fund2PostDeathClaims}");

        fund1PostRebalance = rebalanceIndicator == 1 ? avPostDeathClaims * FUNDS_AUTOMATIC_REBALANCING_TARGET : fund1PostDeathClaims;
        fund2PostRebalance = avPostCharges - fund1PostRebalance;
        // Console.WriteLine($"{fund1PostRebalance} {fund2PostRebalance}");

        riderDeathBenefitBase = Math.Max(0, riderDeathBenefitBase * (1 - QX) + contribution - fundFees - oldWithdrawalAmount - riderCharge);
      };

      // LAST
      ROPDeathBase = ROPDeathBase * (1 - QX);
      NARDeathClaims = Math.Max(0, deathPayments - avPostCharges);
      // Console.WriteLine($"{ROPDeathBase} {NARDeathClaims}");

      cumulativeDeathClaims += NARDeathClaims * DF;
      cumulativeWithdrawalClaims += Math.Max(0, withdrawalAmount - oldAvPostDeathClaims) * DF;
      cumulativeRiderCharges += riderCharge * DF;
      cumulativeFundFees += onlyFundFees * DF;


      double finalDeathClaims = NARDeathClaims;
      double finalWithdrawalClaims = Math.Max(riderWithdrawalAmount - oldAvPostDeathClaims, 0);
      double finalRiderCharges = riderCharge;
      double finalFundFees = onlyFundFees;


      // Console.WriteLine($"{riderCharge} {fundFees} {riderDeathBenefitBase} {riderWithdrawalBase} {withdrawalAmount} {riderMaxAnnualWithdrawal} {riderMaxAnnualWithdrawalRate}");
      // Console.WriteLine($"{finalDeathClaims} {finalWithdrawalClaims} {finalRiderCharges}");
      // Console.WriteLine($"{fund1InterestCredited} {fund2InterestCredited} {totalInterestCredited}");
      Console.WriteLine($"{fund2Return}");

      _policyRecords.Add(new PolicyRecord
      {
        age = age,
        year = year,
        contribution = contribution,
        avPreFee = avPreFee,
        fund1PreFee = fund1PreFee,
        fund2PreFee = fund2PreFee,
        fundFees = fundFees,
        onlyFundFees = onlyFundFees,
        avPreWithdrawal = avPreWithdrawal,
        fund1PreWithdrawal = fund1PreWithdrawal,
        fund2PreWithdrawal = fund2PreWithdrawal,
        withdrawalAmount = withdrawalAmount,
        avPostWithdrawal = avPostWithdrawal,
        fund1PostWithdrawal = fund1PostWithdrawal,
        fund2PostWithdrawal = fund2PostWithdrawal,
        riderCharge = riderCharge,
        avPostCharges = avPostCharges,
        fund1PostCharges = fund1PostCharges,
        fund2PostCharges = fund2PostCharges,
        deathPayments = deathPayments,
        avPostDeathClaims = avPostDeathClaims,
        fund1PostDeathClaims = fund1PostDeathClaims,
        fund2PostDeathClaims = fund2PostDeathClaims,
        fund1PostRebalance = fund1PostRebalance,
        fund2PostRebalance = fund2PostRebalance,
        ROPDeathBase = ROPDeathBase,
        NARDeathClaims = NARDeathClaims,
        riderDeathBenefitBase = riderDeathBenefitBase,
        riderWithdrawalBase = riderWithdrawalBase,
        riderWithdrawalAmount = riderWithdrawalAmount,
        riderCumulativeWithdrawal = riderCumulativeWithdrawal,
        riderMaxAnnualWithdrawal = riderMaxAnnualWithdrawal,
        riderMaxAnnualWithdrawalRate = riderMaxAnnualWithdrawalRate,
        eligibleStepUp = eligibleStepUp,
        growthPhase = growthPhase,
        withdrawalPhase = withdrawalPhase,
        automaticPeriodicBenefitStatus = automaticPeriodicBenefitStatus,
        lastDeath = lastDeath,
        fund1Return = fund1Return,
        fund2Return = fund2Return,
        rebalanceIndicator = rebalanceIndicator,
        DF = DF,
        QX = QX,
        finalDeathClaims = finalDeathClaims,
        finalWithdrawalClaims = finalWithdrawalClaims,
        finalRiderCharges = finalRiderCharges,
        finalFundFees = finalFundFees,
        fund1InterestCredited = fund1InterestCredited,
        fund2InterestCredited = fund2InterestCredited,
        totalInterestCredited = totalInterestCredited,
      });
    }
  }
}



