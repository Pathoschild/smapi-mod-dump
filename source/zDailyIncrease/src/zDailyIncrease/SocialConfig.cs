namespace zDailyIncrease
{
  public class SocialConfig
  {
    public bool enabled
    {
      get; set;
    }

    public bool noDecrease
    {
      get; set;
    }

    public bool noIncrease
    {
      get; set;
    }

    public bool randomIncrease
    {
      get; set;
    }

    public bool disableAllOutput
    {
      get; set;
    }

    public IndividualNpcConfig[] individualConfigs
    {
      get; set;
    } = new IndividualNpcConfig[1];

    public SocialConfig()
    {
      enabled = true;
      noDecrease = true;
      noIncrease = false;
      randomIncrease = false;
      disableAllOutput = false;

      individualConfigs[0] = new IndividualNpcConfig("Default", 2, 5, 2500);
    }
  }

  public class IndividualNpcConfig
  {
    public string name
    {
      get; set;
    }
    public int baseIncrease
    {
      get; set;
    }
    public int talkIncrease
    {
      get; set;
    }
    public int max
    {
      get; set;
    }

    public IndividualNpcConfig(string name, int baseIncrease, int talkIncrease, int max)
    {
      this.name = name;
      this.baseIncrease = baseIncrease;
      this.talkIncrease = talkIncrease;
      this.max = max;
    }
  }
}
