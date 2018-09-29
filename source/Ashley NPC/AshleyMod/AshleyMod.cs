using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Inheritance;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Cat = StardewValley.Characters.Cat;

namespace AshleyMod
{
  public class AshleyMod : Mod
  {
    //public static Dictionary<string, Texture2D> itemTextures = new Dictionary<string, Texture2D>();

    public int modifiedWizardDialogue = 0;
    public bool megaModifiedWizardDialogue = false;

    public override void Entry(params object[] objects)
    {
      AshleyEvents.InitializeEvents();
      GameEvents.LoadContent += GameEvents_LoadContent;
      GameEvents.UpdateTick += GameEvents_UpdateTick;
      LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
      SaveGame.serializer = new XmlSerializer(typeof(SaveGame), new Type[28]
      {
        typeof (Tool),
        typeof (GameLocation),
        typeof (Crow),
        typeof (Duggy),
        typeof (Bug),
        typeof (BigSlime),
        typeof (Fireball),
        typeof (Ghost),
        typeof (Child),
        typeof (Pet),
        typeof (Dog),
        typeof (Cat),
        typeof (Horse),
        typeof (GreenSlime),
        typeof (LavaCrab),
        typeof (RockCrab),
        typeof (ShadowGuy),
        typeof (SkeletonMage),
        typeof (SquidKid),
        typeof (Grub),
        typeof (Fly),
        typeof (DustSpirit),
        typeof (Quest),
        typeof (MetalHead),
        typeof (ShadowGirl),
        typeof (Monster),
        typeof (TerrainFeature),
        typeof (AshleyNPC)
      });
      SaveGame.locationSerializer = new XmlSerializer(typeof(GameLocation), new Type[27]
      {
        typeof (Tool),
        typeof (Crow),
        typeof (Duggy),
        typeof (Fireball),
        typeof (Ghost),
        typeof (GreenSlime),
        typeof (LavaCrab),
        typeof (RockCrab),
        typeof (ShadowGuy),
        typeof (SkeletonWarrior),
        typeof (Child),
        typeof (Pet),
        typeof (Dog),
        typeof (Cat),
        typeof (Horse),
        typeof (SquidKid),
        typeof (Grub),
        typeof (Fly),
        typeof (DustSpirit),
        typeof (Bug),
        typeof (BigSlime),
        typeof (BreakableContainer),
        typeof (MetalHead),
        typeof (ShadowGirl),
        typeof (Monster),
        typeof (TerrainFeature),
        typeof (AshleyNPC)
      });
    }

    private void GameEvents_LoadContent(object sender, EventArgs e)
    {
      //itemTextures.Add("wario", Game1.content.Load<Texture2D>("AshleyModItems\\wario"));
    }

    private void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
    {
      AshleyEvents.CheckForEvent();
    }

    private void GameEvents_UpdateTick(object sender, EventArgs e)
    {
      if(Game1.currentLocation != null)
      {
        if (Game1.getCharacterFromName("Ashley") == null)
          foreach (var i in Game1.locations)
            if (i.name == "WizardHouse")
            {
              var ashley = new AshleyNPC(new AnimatedSprite(Game1.content.Load<Texture2D>("Characters\\Ashley"), 0, 24, Game1.tileSize * 2 / 4), new Vector2((float)(2 * Game1.tileSize), (float)(6 * Game1.tileSize)), "WizardHouse", 3, "Ashley", false, (Dictionary<int, int[]>)null, Game1.content.Load<Texture2D>("Portraits\\Ashley"));
              i.addCharacter(ashley);
              ashley.reloadSprite();
            }
        if (!Game1.NPCGiftTastes.ContainsKey("Ashley"))
          Game1.NPCGiftTastes.Add("Ashley", "Thanks! I needed this!$h/64 336/Thanks. I could use this.$h/-28 420 257 281 107 305 247/What purpose do you expect this to serve for me?$s/80 348 346 303 -74/This is a waste of my time. I'm in the middle of something./390 388 330 571 568 569/Um... thanks?$u// ");
      }
      if (Game1.CurrentEvent != null)
      {
        if (Game1.CurrentEvent.isFestival)
        {
          bool ashleyInFestival = false;
          foreach (var i in Game1.CurrentEvent.actors)
            if (i.name == "Ashley")
              ashleyInFestival = true;
          if (!ashleyInFestival)
            AddAshleyToFestival(Game1.CurrentEvent.FestivalName);
          if(Game1.CurrentEvent.FestivalName == "Stardew Valley Fair" && Game1.player.getFriendshipHeartLevelForNPC("Ashley") >= 4 && !megaModifiedWizardDialogue)
            foreach (var i in Game1.CurrentEvent.actors)
              if(i.name == "Wizard")
              {
                var oldDialogue = i.CurrentDialogue.Peek();
                i.CurrentDialogue.Clear();
                i.CurrentDialogue.Push(new Dialogue("Oh, did you want to speak to Ashley? She didn't want to come.#$b#She's busy working on her brews in the tower.", i));
                i.CurrentDialogue.Push(oldDialogue);
                megaModifiedWizardDialogue = true;
              }
        }
        else
        {
          NPC ashleyNPC = null;
          foreach (var i in Game1.CurrentEvent.actors)
            if (i.name == "Ashley" && !(i is AshleyNPC))
              ashleyNPC = i;
          if (ashleyNPC != null)
          {
            Game1.CurrentEvent.actors.Remove(ashleyNPC);
            Game1.CurrentEvent.actors.Add(new AshleyNPC(new AnimatedSprite(Game1.content.Load<Texture2D>("Characters\\Ashley"), 0, 24, Game1.tileSize * 2 / 4), ashleyNPC.position, "WizardHouse", ashleyNPC.facingDirection, ashleyNPC.name, false, null, Game1.content.Load<Texture2D>("Portraits\\Ashley")));
          }
        }
        if (AshleyEvents.IsCustomEvent())
          AshleyEvents.CheckCustomCommands();
      }
      else
      {
        megaModifiedWizardDialogue = false;
      }
      this.ModifyWizardDialogue();
    }

    public void ModifyWizardDialogue()
    {
      if (Game1.player == null || modifiedWizardDialogue == Game1.dayOfMonth)
        return;
      NPC Wizard = Game1.getCharacterFromName("Wizard");
      if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) == "Mon" && Game1.player.getFriendshipHeartLevelForNPC("Wizard") >= 2)
      {
        Wizard.CurrentDialogue.Clear();
        Wizard.CurrentDialogue.Push(new Dialogue("Young Ashley was... not the friendliest person when she arrived. I'm proud of how much she's grown.#$e#However, she comes from a distant land. She still has much to learn about this world and the arcane.", Wizard));
      }
      if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) == "Sun" && Game1.player.getFriendshipHeartLevelForNPC("Wizard") >= 2)
      {
        Wizard.CurrentDialogue.Clear();
        Wizard.CurrentDialogue.Push(new Dialogue("Ashley is a promising young apprentice, but her future and past are mysteries even to me.#$e#I cannot be sure that she will remain my apprentice when the time comes.", Wizard));
      }
      modifiedWizardDialogue = Game1.dayOfMonth;
    }

    public void AddAshleyToFestival(string festival)
    {
      if(festival == "Luau")
      {
        Vector2 ashleyPos = new Vector2((float)(14 * Game1.tileSize), (float)(39 * Game1.tileSize));
        int ashleyFacing = 1;
        AshleyNPC ashley = new AshleyNPC(new AnimatedSprite(Game1.content.Load<Texture2D>("Characters\\Ashley"), 0, 24, Game1.tileSize * 2 / 4), ashleyPos, "WizardHouse", ashleyFacing, "Ashley", false, null, Game1.content.Load<Texture2D>("Portraits\\Ashley"));
        ashley.CurrentDialogue.Push(new Dialogue("The wizard made me come here... why should I care?$a#$e#Merpeople are weird.$a", ashley));
        Game1.CurrentEvent.actors.Add(ashley);
      }
      else if(festival == "Dance Of The Moonlight Jellies")
      {
        Vector2 ashleyPos = new Vector2((float)(90 * Game1.tileSize), (float)(4 * Game1.tileSize));
        int ashleyFacing = 2;
        AshleyNPC ashley = new AshleyNPC(new AnimatedSprite(Game1.content.Load<Texture2D>("Characters\\Ashley"), 0, 24, Game1.tileSize * 2 / 4), ashleyPos, "WizardHouse", ashleyFacing, "Ashley", false, null, Game1.content.Load<Texture2D>("Portraits\\Ashley"));
        ashley.CurrentDialogue.Push(new Dialogue("Lunaloos, huh?#$b#They'd probably make a good ingredient.$h#$e#The wizard would kill me.$u", ashley));
        Game1.CurrentEvent.actors.Add(ashley);
      }
      else if(festival == "Spirit's Eve")
      {
        Vector2 ashleyPos = new Vector2((float)(39 * Game1.tileSize), (float)(13 * Game1.tileSize));
        int ashleyFacing = 2;
        AshleyNPC ashley = new AshleyNPC(new AnimatedSprite(Game1.content.Load<Texture2D>("Characters\\Ashley"), 0, 24, Game1.tileSize * 2 / 4), ashleyPos, "WizardHouse", ashleyFacing, "Ashley", false, null, Game1.content.Load<Texture2D>("Portraits\\Ashley"));
        ashley.CurrentDialogue.Push(new Dialogue("This is my element.$h", ashley));
        Game1.CurrentEvent.actors.Add(ashley);
      }
    }
  }
}
