

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Characters;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Threading;
using StardewValley.Quests;
using StardewValley.Locations;
using System.Diagnostics;


namespace SMAPIMod3
{

    public class ModEntry : Mod, IAssetLoader
    {
        NPC BackupMarlon = new NPC(new AnimatedSprite("Characters\\Marlon", 0, 16, 32), new Vector2((float)(8 * StardewValley.Game1.tileSize), (float)(5 * StardewValley.Game1.tileSize)), "AdventureGuild", 2, "Marlon", false, (Dictionary<int, int[]>)null, Game1.content.Load<Texture2D>("Portraits\\Marlon"));
        NPC BackupGunther = new NPC(new AnimatedSprite("Characters\\Gunther", 0, 16, 32), new Vector2((float)(3 * StardewValley.Game1.tileSize), (float)(8 * StardewValley.Game1.tileSize)), "ArchaeologyHouse", 2, "Gunther", false, (Dictionary<int, int[]>)null, Game1.content.Load<Texture2D>("Portraits\\Gunther"));
        // Quest SocialQuest;
        SocialNPC MarlonSocial = new SocialNPC(new AnimatedSprite("Characters\\Marlon", 0, 16, 32), new Vector2((float)(8 * StardewValley.Game1.tileSize), (float)(5 * StardewValley.Game1.tileSize)), "AdventureGuild", 2, "Marlon", false, (Dictionary<int, int[]>)null, Game1.content.Load<Texture2D>("Portraits\\Marlon"));
        SocialNPC GuntherSocial = new SocialNPC(new AnimatedSprite("Characters\\Gunther", 0, 16, 32), new Vector2((float)(3 * StardewValley.Game1.tileSize), (float)(8 * StardewValley.Game1.tileSize)), "ArchaeologyHouse", 2, "Gunther", false, (Dictionary<int, int[]>)null, Game1.content.Load<Texture2D>("Portraits\\Gunther"));

        public bool CanLoad<T>(IAssetInfo asset)
        {

            if (asset.AssetNameEquals("Characters/Dialogue/Marlon"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Characters/Dialogue/Gunther"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Characters/Schedules/Marlon"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Characters/Schedules/Gunther"))
            {
                return true;
            }

            return false;
        }

        public T Load<T>(IAssetInfo asset)
        {


            if (asset.AssetNameEquals("Characters/Dialogue/Marlon"))
            {
                return this.Helper.Content.Load<T>("MarlonDialogue.xnb", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Characters/Dialogue/Gunther"))
            {
                return this.Helper.Content.Load<T>("GuntherDialogue.xnb", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Characters/Schedules/Marlon"))
            {
                return this.Helper.Content.Load<T>("MarlonSchedule.xnb", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Characters/Schedules/Gunther"))
            {
                return this.Helper.Content.Load<T>("GuntherSchedule.xnb", ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }
        public void AfterSaveCreated(object sender, EventArgs args) {
            
            BackupMarlon.followSchedule = true;
            BackupGunther.followSchedule = true;
            MarlonSocial.followSchedule = true;
            GuntherSocial.followSchedule = true;
            GuntherSocial.Name = "Gunther";
            MarlonSocial.Name = "Marlon";

            //   SocialQuest = new SocializeQuest();
            //   Game1.player.removeQuest(9);

            Game1.getLocationFromName("AdventureGuild").characters[0] = MarlonSocial;
            Game1.getLocationFromName("ArchaeologyHouse").characters[0] = GuntherSocial;
            //   Game1.player.questLog.Add(SocialQuest);

        }
       
        public void SaveEvents_AfterSave(object sender, EventArgs args)
        {
            Game1.getLocationFromName("AdventureGuild").characters[0] = MarlonSocial;
            Game1.getLocationFromName("ArchaeologyHouse").characters[0] = GuntherSocial;

            /*    if (!SocialQuest.completed)
                {
                    Game1.player.questLog.Add(SocialQuest);
                } */
        }

        private void AfterLoad(object sender, EventArgs args)
        {      
            Game1.getLocationFromName("AdventureGuild").characters[0] = MarlonSocial;           
            Game1.getLocationFromName("ArchaeologyHouse").characters[0] = GuntherSocial;
            GuntherSocial.Name = "Gunther";
            BackupMarlon.followSchedule = true;
            BackupGunther.followSchedule = true;
            MarlonSocial.followSchedule = true;
            GuntherSocial.followSchedule = true;
            MarlonSocial.checkSchedule(1000);
            MarlonSocial.Birthday_Day = 8;
            MarlonSocial.Birthday_Season = "fall";
            GuntherSocial.Birthday_Day = 22;
            GuntherSocial.Birthday_Season = "spring";

            /*   if (!(bool)SocialQuest.completed)
               {
                   Game1.player.questLog.Add(SocialQuest);
                   Monitor.Log("Added quest!");
                   Debug.Write("Added quest!"); 

               } */
        }

        private void AfterTitleReturn(object sender, EventArgs args) {          
            //    Game1.player.questLog.Remove(SocialQuest);
        }

        private void BeforeSave(object sender, EventArgs args)
        {
            
            Game1.getLocationFromName("AdventureGuild").characters[0] = BackupMarlon;          
            Game1.getLocationFromName("ArchaeologyHouse").characters[0] = BackupGunther;
           
        }

        public override void Entry(IModHelper helper)
        {
            Dictionary<string, string> source = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");

            if (!source.ContainsKey("Marlon"))
            {
                source.Add("Marlon", "adult/neutral/outgoing/neutral/male/not-datable/Marnie/Town/fall 2/Wizard ''/AdventureGuild 8 4/Marlon");
            }

            if (!source.ContainsKey("Gunther"))
            {
              source.Add("Gunther", "adult / polite / neutral / negative / male / not - datable / null / Town / summer 23/ Marlon ''/ArchaeologyHouse 12 32/Gunther");
            }

            Dictionary<string, string> source2 = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\Mine.xnb");

            if (!source2.ContainsKey("734039/f Marlon 1750/t 1000 1400"))
            {
                source2.Add("734039/f Marlon 1750/t 1000 1400", "MarlonsTheme/21 9/farmer 19 0 0 Marlon 23 6 2/skippable/pause 1000/warp farmer 18 13/move farmer 0 -5 0/faceDirection farmer 1/speak Marlon \"*sigh*... \"/pause 500/move farmer 0 -2 1/move farmer 4 0 1/pause 500/showFrame Marlon 0/pause 500/speak Marlon \"I was just thinking about something that happened a long time ago.#$b#Did you know that I used to have a family? A wife, a son, even a dog.\"/pause 500/showFrame Marlon 4/speak Marlon \"We were all adventurous, of course. My son and I would go down to the bottom of the mines once a week to explore.#$b#It was the best time of my life.\"/pause 500/showFrame Marlon 0/pause 500/speak Marlon \"But then he was swarmed by shadow beasts, one day. They ripped him apart right in front of me...\"/pause 500/speak Marlon \"I still can't bear to think about it. I lost my leg trying to save him, but it was too late.\"/pause 500/speak Marlon \"As if my life wasn't already destroyed, my wife blamed me for his death and left me.#$b#For a long time, I avoided the mines. No one else used them, so they fell into ruin and more and more monsters came.#$b#But then you came along, and you started exploring them.\"/pause 1000/speak Marlon \"I finally got over my fear of the mines, so I've been exploring them too.#$b#I can't thank you enough for finally showing me how to live again.\"/pause 300/speak Marlon \"Anyway, I'll let you go. That was just on my mind so I wanted to share it.\"/pause 500/end");
            }
            Dictionary<string, string> source5 = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\ArchaeologyHouse.xnb");

            if (!source5.ContainsKey("5478328/f Gunther 2000/t 1600 1800/e 66"))
            {
                source5.Add("5478328/f Gunther 2000/t 1600 1800/e 66", "libraryTheme/43 5/farmer 47 8 0 Gunther 47 4 2/skippable/pause 1000/move farmer 0 -2 0/speak Gunther \"Oh, greetings @. \"/pause 500/speak Gunther \"I'm just taking in the fact that I finally fulfilled my dream.#$b#I honestly didn't expect this museum would be successful once I learned that the other curator took all the artifacts.#$b#And it wouldn't have been, if it wasn't for your help.\"/pause 500/speak Gunther \"I hope the key proved useful. I really don't know how else I can thank you. This museum means the world to me.#$b#I used to live in the city, working at an insurance company. I hurt so many people.#$b#Apologies for my harsh language, but I absolutely despise big corporations like Joja. They ruin people's lives just to keep shareholders happy.#$b#I was working for a company like that. We didn't help people. I left because I wanted to do something good for the world.\"/pause 500/faceDirection Gunther 3/pause 1000/faceDirection Gunther 2/speak Gunther \"I've always loved history, and I strongly believe that we can improve our world today by learning about our past.#$b#I was also having a bit of a midlife crisis, if I'm being honest.#$b#When I heard that there was a museum for sale, I decided to spend all of the money I had made from my insurance job to purchase it.#$b#Of course, it had a full collection of artifacts and books at the time. I learned soon after I paid for it that they were gone.#$b#No one came here, so I was constantly in the red. That's why I haven't been able to make any improvements around here, and why I don't even have somewhere to sleep.#$b#But that's all changed, thanks to you. The museum is thriving now. So thank you.#$b#I'm glad I've made a friend here. Have a good evening, @.\"/pause 500/end");
            }
            Dictionary<string, string> source3 = Game1.content.Load<Dictionary<string, string>>("Data\\NPCGiftTastes.xnb");

            if (!source3.ContainsKey("Marlon"))
            {
                source3.Add("Marlon", "Wow, you're giving this to me? I can't believe it./559 560 561 562 563/Thanks for the great gift!/542 543 544 545 546/This isn't the best gift I've ever received.../450 436 420 384 382/I'd rather lose my other eye than look at this./226 203 196 80 24/Thanks!// ");
            }

            if (!source3.ContainsKey("Gunther"))
            {
                source3.Add("Gunther", "I love this. I'll definitely add it to my personal collection./101 102 103 104 105 106 107 108 109 110/I appreciate that you got me this./80 82 84 86/I already have this in my collection, but thanks./60 62 64 68/Honestly, I really don't like this./2 4 75 76 77 78/Thank you./378 380 384 386/ ");
            }
            SaveEvents.AfterCreate += AfterSaveCreated;
            SaveEvents.AfterLoad += AfterLoad;
            SaveEvents.AfterSave += SaveEvents_AfterSave;          
            SaveEvents.BeforeSave += BeforeSave;         
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override object GetApi()
        {
            return base.GetApi();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
     
    }
        }

    


