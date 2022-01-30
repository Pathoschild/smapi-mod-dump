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
  public interface IFluentDialogueBuilderAdd
  {
    IFluentDialogueBuilderAdd Season();
    IFluentDialogueBuilderAdd Season(bool enabled);
    IFluentDialogueBuilderAdd DayOfMonth();
    IFluentDialogueBuilderAdd DayOfMonth(bool enabled);
    IFluentDialogueBuilderAdd DayOfWeek();
    IFluentDialogueBuilderAdd DayOfWeek(bool enabled);
    IFluentDialogueBuilderAdd Hearts();
    IFluentDialogueBuilderAdd Hearts(bool enabled);
    IFluentDialogueBuilderAdd Hearts(int overrideHearts);
    IFluentDialogueBuilderAdd Friendship();
    IFluentDialogueBuilderAdd Friendship(bool enabled);
    IFluentDialogueBuilderAdd Friendship(int overrideFriendship);
    IFluentDialogueBuilderAdd FirstOrSecondYear();
    IFluentDialogueBuilderAdd FirstOrSecondYear(bool enabled);
    IFluentDialogueBuilderAdd Year();
    IFluentDialogueBuilderAdd Year(bool enabled);
    IFluentDialogueBuilderAdd Married();
    IFluentDialogueBuilderAdd Married(bool enabled);
    IFluentDialogueBuilderRemove Disable();
    string Build(IMonitor logger);
  }

  public interface IFluentDialogueBuilderRemove
  {
    IFluentDialogueBuilderAdd Season();
    IFluentDialogueBuilderAdd DayOfMonth();
    IFluentDialogueBuilderAdd DayOfWeek();
    IFluentDialogueBuilderAdd Hearts();
    IFluentDialogueBuilderAdd Friendship();
    IFluentDialogueBuilderAdd FirstOrSecondYear();
    IFluentDialogueBuilderAdd Year();
    IFluentDialogueBuilderAdd Married();
    string Build(IMonitor logger);
  }
}