using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshleyMod
{
  public class AshleyEvents
  {
    public static int WizardHouse_6Hearts = 291842;
    public static int WizardHouse_6Hearts_DenyBrew = 291843;
    public static Dictionary<int, string> Events = new Dictionary<int, string>();

    public static void InitializeEvents()
    {
      AddEvent(WizardHouse_6Hearts, @"playful/-1000 -1000/farmer 8 24 0 Ashley 4 19 2/skippable/viewport 8 18 true
/pause 1000
/move farmer 0 -5 3
/pause 250
/emote Ashley 16
/faceDirection Ashley 1 true
/speak Ashley ""Hello, @!""
/move Ashley 2 0 1
/speak Ashley ""I finished up a new brew, just for you!$h#$b#So, I'd like you to try it out.$h#$b#Come over here.$h""
/move Ashley -3 0 1 false
/move farmer -4 0 2
/pause 500
/speak Ashley ""Go ahead, take a sip!$h#$b#I promise it's not dangerous.""
/pause 250
/question fork1 ""#Try the brew#Deny the brew""
/cFork " + WizardHouse_6Hearts_DenyBrew + @"
/friendship Ashley 50
/pause 500
/farmerEat 184
/pause 4000
/emote farmer 8
/pause 1000
/emote Ashley 28
/speak Ashley ""Of course. It doesn't work.$s""
/faceDirection farmer 0
/move Ashley 0 -1 0
/move Ashley 4 0 1
/faceDirection farmer 1
/pause 1000
/speak Ashley ""The wizard tells me I need to 'adapt', but...#$b#It's hard.$s""
/move Ashley 0 1 2
/pause 250
/speak Ashley ""Without Red, I'm practically an entirely different person now.$s""
/faceDirection Ashley 3
/pause 1000
/speak Ashley ""But, I can live without him if I have to.$s#$b#Maybe you could be my assistant one day.$h""
/pause 1000
/faceDirection Ashley 2
/pause 1000
/speak Ashley ""Well, thanks for coming by today.$h#$b#I should be getting back to my work now.""
/end");
      AddEvent(WizardHouse_6Hearts_DenyBrew, @"friendship Ashley -1000
/emote Ashley 12
/speak Ashley ""I make a brew just for you and you won't even try it?$a#$b#Whatever. I'm just going to go back to working on my own brews.$a""
/end");
    }

    public static bool CheckForEvent()
    {
      if (Game1.player == null || Game1.player.currentLocation == null || Game1.CurrentEvent != null)
        return false;
      bool startedEvent = false;
      if (!Game1.player.eventsSeen.Contains(WizardHouse_6Hearts) 
        && Game1.player.currentLocation.Name == "WizardHouse" 
        && Game1.player.getFriendshipLevelForNPC("Ashley") >= 1500)
      {
        Game1.currentLocation.currentEvent = new CustomEvent(Events[WizardHouse_6Hearts], WizardHouse_6Hearts);
        Game1.player.eventsSeen.Add(WizardHouse_6Hearts);
        startedEvent = true;
      }
      if (!startedEvent)
        return false;
      if (Game1.player.getMount() != null)
      {
        Game1.currentLocation.currentEvent.playerWasMounted = true;
        Game1.player.getMount().dismount();
      }
      foreach (NPC npc in Game1.currentLocation.characters)
        npc.clearTextAboveHead();
      Game1.eventUp = true;
      Game1.displayHUD = false;
      Game1.player.CanMove = false;
      Game1.player.showNotCarrying();
      return true;
    }

    public static bool IsCustomEvent()
    {
      if (Game1.CurrentEvent == null)
        return false;
      if (!(Game1.CurrentEvent is CustomEvent))
        return false;
      return (Events.ContainsKey((Game1.CurrentEvent as CustomEvent).EventID));
    }

    public static void CheckCustomCommands()
    {
      if (Game1.CurrentEvent == null)
        return;
      if (Game1.CurrentEvent.eventCommands == null || Game1.CurrentEvent.eventCommands.Length == 0)
        return;
      if (Game1.CurrentEvent.skipped || Game1.CurrentEvent.currentCommand >= Game1.CurrentEvent.eventCommands.Length)
        return;
      string[] split = Game1.CurrentEvent.eventCommands[Game1.CurrentEvent.currentCommand].Split(' ');
      if(split[0].Equals("cFork"))
      {
        if(split.Length > 2)
        {
          int result;
          if (Game1.player.mailReceived.Contains(split[1]) || int.TryParse(split[1], out result) && Game1.player.dialogueQuestionsAnswered.Contains(result))
          {
            Game1.CurrentEvent.eventCommands = Events[Convert.ToInt32(split[2])].Split('/');
            Game1.CurrentEvent.currentCommand = 0;
            Game1.CurrentEvent.forked = !Game1.CurrentEvent.forked;
          }
          else
            Game1.CurrentEvent.currentCommand++;
        }
        else if (Game1.CurrentEvent.specialEventVariable1)
        {
          Game1.CurrentEvent.eventCommands = Events[Convert.ToInt32(split[1])].Split('/');
          Game1.CurrentEvent.currentCommand = 0;
          Game1.CurrentEvent.forked = !Game1.CurrentEvent.forked;
        }
        else
        {
          Game1.CurrentEvent.currentCommand++;
        }
      }
    }

    static void AddEvent(int id, string eventString)
    {
      Events.Add(id, eventString.Replace(System.Environment.NewLine, ""));
    }
  }
}