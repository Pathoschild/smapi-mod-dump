/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using StardewModdingAPI;

namespace DialogueExtension.Patches.Utility
{
  public sealed class FluentDialogueBuilder : IFluentDialogueBuilderAdd, IFluentDialogueBuilderRemove
  {
    private DialogueConditions _conditions;
    private string _overrideFriendship;
    private string _overrideHearts;


    private bool _removeNext;

    private bool _setDayOfMonth;

    private bool _setDayOfWeek;

    private bool _setFirstOrSecondYear;

    private bool _setFriendship;

    private bool _setHearts;

    private bool _setInlaw;

    private bool _setSeason;

    private bool _setYear;

    private FluentDialogueBuilder(DialogueConditions conditions) => _conditions = conditions;

    private string Dialogue =>
      $"{GetSeason}" +
      $"{GetDayOfWeek}{GetDayOfMonth}" +
      $"{GetHearts}{GetFriendship}" +
      $"{GetFirstOrSecondYear}{GetYear}" +
      $"{GetInlaw}";

    private string GetSeason =>
      _setSeason ? _conditions.Season.ToString().ToLower() + "_" : string.Empty;

    private string GetDayOfWeek =>
      _setDayOfWeek ? _conditions.DayOfWeek.ToString() : string.Empty;

    private string GetDayOfMonth =>
      _setDayOfMonth ? _conditions.DayOfMonth.ToString() : string.Empty;

    private string GetHearts =>
      _setHearts ? _overrideHearts ?? _conditions.Hearts.ToString() : string.Empty;

    private string GetFriendship =>
      _setFriendship ? _overrideFriendship ?? _conditions.Friendship.ToString() : string.Empty;

    private string GetFirstOrSecondYear =>
      _setFirstOrSecondYear ? "_" + _conditions.FirstOrSecondYear : string.Empty;

    private string GetYear =>
      _setYear ? "_" + _conditions.Year : string.Empty;

    private string GetInlaw =>
      _setInlaw ? _conditions.Inlaw : string.Empty;

    private bool RemoveNext
    {
      get
      {
        if (_removeNext)
        {
          _removeNext = false;
          return true;
        }

        return false;
      }
      set => _removeNext = value;
    }

    public IFluentDialogueBuilderAdd Season() => Season(true);

    public IFluentDialogueBuilderAdd Season(bool enabled)
    {
      if (!enabled || _conditions.Season == Utility.Season.Unknown) return this;
      _setSeason = !RemoveNext;
      return this;
    }

    public IFluentDialogueBuilderAdd DayOfMonth() => DayOfMonth(true);

    public IFluentDialogueBuilderAdd DayOfMonth(bool enabled)
    {
      if (!enabled) return this;
      _setDayOfMonth = !RemoveNext;
      _setDayOfWeek = false;
      return this;
    }

    public IFluentDialogueBuilderAdd DayOfWeek() => DayOfWeek(true);

    public IFluentDialogueBuilderAdd DayOfWeek(bool enabled)
    {
      if (!enabled || _conditions.DayOfWeek == Utility.DayOfWeek.Unknown) return this;
      _setDayOfWeek = !RemoveNext;
      _setDayOfMonth = false;
      return this;
    }

    public IFluentDialogueBuilderAdd Hearts() => Hearts(true);

    public IFluentDialogueBuilderAdd Hearts(bool enabled)
    {
      if (!enabled) return this;
      if (RemoveNext)
      {
        _setHearts = false;
        _overrideHearts = null;
      }
      else
      {
        _setHearts = true;
        _setFriendship = false;
      }

      return this;
    }

    public IFluentDialogueBuilderAdd Hearts(int overrideHearts)
    {
      _overrideHearts = overrideHearts.ToString();
      return Hearts(true);
    }

    public IFluentDialogueBuilderAdd Friendship() => Friendship(true);

    public IFluentDialogueBuilderAdd Friendship(bool enabled)
    {
      if (!enabled) return this;
      _setFriendship = !RemoveNext;
      _setHearts = false;
      return this;
    }

    public IFluentDialogueBuilderAdd Friendship(int overrideFriendship)
    {
      _overrideFriendship = overrideFriendship.ToString();
      return Friendship(true);
    }

    public IFluentDialogueBuilderAdd FirstOrSecondYear() => FirstOrSecondYear(true);

    public IFluentDialogueBuilderAdd FirstOrSecondYear(bool enabled)
    {
      if (!enabled) return this;
      _setFirstOrSecondYear = !RemoveNext;
      _setYear = false;
      return this;
    }

    public IFluentDialogueBuilderAdd Year() => Year(true);

    public IFluentDialogueBuilderAdd Year(bool enabled)
    {
      if (!enabled) return this;
      _setYear = !RemoveNext;
      _setFirstOrSecondYear = false;
      return this;
    }

    public IFluentDialogueBuilderAdd Married() => Married(true);

    public IFluentDialogueBuilderAdd Married(bool enabled)
    {
      if (!enabled) return this;
      _setInlaw = !RemoveNext;
      return this;
    }

    public IFluentDialogueBuilderRemove Disable()
    {
      if (!RemoveNext)
        RemoveNext = true;
      return this;
    }

    public string Build(IMonitor logger = null)
    {
      //logger?.Log(Dialogue, LogLevel.Warn);
      return Dialogue;
    }

    public static FluentDialogueBuilder New(DialogueConditions conditions)
      => new FluentDialogueBuilder(conditions);
  }
}