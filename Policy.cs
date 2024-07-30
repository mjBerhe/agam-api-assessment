public class Policy
{
  public int age { get; set; }
  public int year;
  const int INITIAL_PREMIUM = 100000;
  const double STEP_UP_RATE = 0.06;
  const int STEP_UP_PERIOD = 10;

  const int FIRST_WITHDRAWAL_AGE = 70;
  const int ANNUITY_COMMENCEMENT_AGE = 80;
  const int LAST_DEATH_AGE = 100;

  const double RIDER_CHARGE_RATE = 0.0085;
  const double WITHDRAWAL_RATE = 0.03;
  const double FUNDS_AUTOMATIC_REBALANCING_TARGET = 0.2;

  const double M_AND_E_RATE = 0.014;
  const double FUND_FEE_RATE = 0.0015;
  const double RISK_FREE_RATE = 0.03;
  const double VOLATILITY_RATE = 0.16;

  const double AGE_1 = 59.5;
  const double AGE_2 = 65;
  const double AGE_3 = 76;
  const double AGE_4 = 80;

  const double MAX_ANNUAL_WITHDRAWAL_1 = 0.04;
  const double MAX_ANNUAL_WITHDRAWAL_2 = 0.05;
  const double MAX_ANNUAL_WITHDRAWAL_3 = 0.06;
  const double MAX_ANNUAL_WITHDRAWAL_4 = 0.07;


  double contribution, withdrawalAmount, riderCharge, deathPayments;
  public double fundFees { get; set; }

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

  public Policy(int initialAge)
  {
    this.age = initialAge;
    this.year = 0;

    this.contribution = 0; // PARAM
    this.fundFees = 0;
    this.withdrawalAmount = 0; // PARAM??
    this.riderCharge = 0;
    this.deathPayments = 0;

    this.eligibleStepUp = 0;
    this.growthPhase = 0;
    this.withdrawalPhase = 0;
    this.automaticPeriodicBenefitStatus = 0;
    this.lastDeath = 0;
    this.rebalanceIndicator = 0;

    this.fund1PreFee = INITIAL_PREMIUM * 0.16;
    this.fund2PreFee = INITIAL_PREMIUM * 0.64;
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

    this.fund1Return = 0.03; // PARAM??
    this.fund2Return = 0.035; // PARAM??
    this.DF = Math.Pow(1 + RISK_FREE_RATE, year);
    this.QX = 0.005; // PARAM??

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
      DF = Math.Round(Math.Pow(1 + RISK_FREE_RATE, year * -1), 4);
      fundFees = oldAvPostDeathClaims * (M_AND_E_RATE + FUND_FEE_RATE);

      fund1PreFee = fund1PostRebalance * (1 + fund1Return);
      fund2PreFee = fund2PostRebalance * (1 + fund2Return);
      avPreFee = fund1PreFee + fund2PreFee;
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

        // this guy is our problem
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

      double finalDeathClaims = NARDeathClaims;
      double finalWithdrawalClaims = Math.Max(riderWithdrawalAmount - oldAvPostDeathClaims, 0);
      double finalRiderCharges = riderCharge;


      // Console.WriteLine($"{riderCharge} {fundFees} {riderDeathBenefitBase} {riderWithdrawalBase} {withdrawalAmount} {riderMaxAnnualWithdrawal} {riderMaxAnnualWithdrawalRate}");
      Console.WriteLine($"{finalDeathClaims} {finalWithdrawalClaims} {finalRiderCharges}");
    }
  }
}