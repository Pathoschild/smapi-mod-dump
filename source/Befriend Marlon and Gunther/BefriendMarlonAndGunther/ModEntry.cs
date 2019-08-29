using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BefriendMarlonAndGunther
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod, IAssetLoader, IAssetEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary>The overridden Marlon NPC.</summary>
        private SocialNPC Marlon;

        /// <summary>The overridden Gunther NPC.</summary>
        private SocialNPC Gunther;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.Saving += this.OnSaving;
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return
                // sprites
                asset.AssetNameEquals("Characters/Gunther")
                || asset.AssetNameEquals("Characters/Marlon")

                // dialogue
                || asset.AssetNameEquals("Characters/Dialogue/Marlon")
                || asset.AssetNameEquals("Characters/Dialogue/Gunther")

                // schedules
                || asset.AssetNameEquals("Characters/Schedules/Marlon")
                || asset.AssetNameEquals("Characters/Schedules/Gunther")

                // maps
                || asset.AssetNameEquals("Maps/AdventureGuild")
                || asset.AssetNameEquals("Maps/ArchaeologyHouse");
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            // sprites
            if (asset.AssetNameEquals("Characters/Gunther"))
                return this.Helper.Content.Load<T>("assets/gunther/sprite.png");
            if (asset.AssetNameEquals("Characters/Marlon"))
                return this.Helper.Content.Load<T>("assets/marlon/sprite.png");

            // dialogue
            if (asset.AssetNameEquals("Characters/Dialogue/Gunther"))
                return this.Helper.Content.Load<T>("assets/gunther/dialogue.json");
            if (asset.AssetNameEquals("Characters/Dialogue/Marlon"))
                return this.Helper.Content.Load<T>("assets/marlon/dialogue.json");

            // schedules
            if (asset.AssetNameEquals("Characters/Schedules/Gunther"))
                return this.Helper.Content.Load<T>("assets/gunther/schedule.json");
            if (asset.AssetNameEquals("Characters/Schedules/Marlon"))
                return this.Helper.Content.Load<T>("assets/marlon/schedule.json");

            // maps
            if (asset.AssetNameEquals("Maps/AdventureGuild"))
                return this.Helper.Content.Load<T>("assets/AdventureGuild.tbin");
            if (asset.AssetNameEquals("Maps/ArchaeologyHouse"))
                return this.Helper.Content.Load<T>("assets/ArchaeologyHouse.tbin");

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return
                asset.AssetNameEquals("Data/NPCDispositions")
                || asset.AssetNameEquals("Data/Events/Mine")
                || asset.AssetNameEquals("Data/Events/ArchaeologyHouse")
                || asset.AssetNameEquals("Data/NPCGiftTastes");
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/NPCDispositions"))
            {
                var data = asset.AsDictionary<string, string>().Data;
                data["Marlon"] = "adult/neutral/outgoing/neutral/male/not-datable/Marnie/Town/fall 8/Wizard ''/AdventureGuild 8 4/Marlon";
                data["Gunther"] = "adult/polite/neutral/negative/male/not-datable/null/Town/spring 22/Marlon ''/ArchaeologyHouse 12 32/Gunther";
            }

            else if (asset.AssetNameEquals("Data/Events/Mine"))
            {
                var data = asset.AsDictionary<string, string>().Data;
                data["734039/f Marlon 1750/t 1000 1400"] = "MarlonsTheme/21 9/farmer 19 0 0 Marlon 23 6 2/skippable/pause 1000/warp farmer 18 13/move farmer 0 -5 0/faceDirection farmer 1/speak Marlon \"*sigh*... \"/pause 500/move farmer 0 -2 1/move farmer 4 0 1/pause 500/showFrame Marlon 0/pause 500/speak Marlon \"I was just thinking about something that happened a long time ago.#$b#Did you know that I used to have a family? A wife, a son, even a dog.\"/pause 500/showFrame Marlon 4/speak Marlon \"We were all adventurous, of course. My son and I would go down to the bottom of the mines once a week to explore.#$b#It was the best time of my life.\"/pause 500/showFrame Marlon 0/pause 500/speak Marlon \"But then he was swarmed by shadow beasts, one day. They ripped him apart right in front of me...\"/pause 500/speak Marlon \"I still can't bear to think about it. I lost my leg trying to save him, but it was too late.\"/pause 500/speak Marlon \"As if my life wasn't already destroyed, my wife blamed me for his death and left me.#$b#For a long time, I avoided the mines. No one else used them, so they fell into ruin and more and more monsters came.#$b#But then you came along, and you started exploring them.\"/pause 1000/speak Marlon \"I finally got over my fear of the mines, so I've been exploring them too.#$b#I can't thank you enough for finally showing me how to live again.\"/pause 300/speak Marlon \"Anyway, I'll let you go. That was just on my mind so I wanted to share it.\"/pause 500/end";
            }

            else if (asset.AssetNameEquals("Data/Events/ArchaeologyHouse"))
            {
                var data = asset.AsDictionary<string, string>().Data;
                data["5478328/f Gunther 2000/t 1600 1800/e 66"] = "libraryTheme/43 5/farmer 47 8 0 Gunther 47 4 2/skippable/pause 1000/move farmer 0 -2 0/speak Gunther \"Oh, greetings @. \"/pause 500/speak Gunther \"I'm just taking in the fact that I finally fulfilled my dream.#$b#I honestly didn't expect this museum would be successful once I learned that the other curator took all the artifacts.#$b#And it wouldn't have been, if it wasn't for your help.\"/pause 500/speak Gunther \"I hope the key proved useful. I really don't know how else I can thank you. This museum means the world to me.#$b#I used to live in the city, working at an insurance company. I hurt so many people.#$b#Apologies for my harsh language, but I absolutely despise big corporations like Joja. They ruin people's lives just to keep shareholders happy.#$b#I was working for a company like that. We didn't help people. I left because I wanted to do something good for the world.\"/pause 500/faceDirection Gunther 3/pause 1000/faceDirection Gunther 2/speak Gunther \"I've always loved history, and I strongly believe that we can improve our world today by learning about our past.#$b#I was also having a bit of a midlife crisis, if I'm being honest.#$b#When I heard that there was a museum for sale, I decided to spend all of the money I had made from my insurance job to purchase it.#$b#Of course, it had a full collection of artifacts and books at the time. I learned soon after I paid for it that they were gone.#$b#No one came here, so I was constantly in the red. That's why I haven't been able to make any improvements around here, and why I don't even have somewhere to sleep.#$b#But that's all changed, thanks to you. The museum is thriving now. So thank you.#$b#I'm glad I've made a friend here. Have a good evening, @.\"/pause 500/end";
            }

            else if (asset.AssetNameEquals("Data/NPCGiftTastes"))
            {
                var data = asset.AsDictionary<string, string>().Data;
                data["Marlon"] = "Wow, you're giving this to me? I can't believe it./559 560 561 562 563/Thanks for the great gift!/542 543 544 545 546/This isn't the best gift I've ever received.../450 436 420 384 382/I'd rather lose my other eye than look at this./226 203 196 80 24/Thanks!// ";
                data["Gunther"] = "I love this. I'll definitely add it to my personal collection./101 102 103 104 105 106 107 108 109 110/I appreciate that you got me this./80 82 84 86/I already have this in my collection, but thanks./60 62 64 68/Honestly, I really don't like this./2 4 75 76 77 78/Thank you./378 380 384 386/ ";
            }

            else
                throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // swap in social NPC
            this.Gunther = new SocialNPC(Game1.getCharacterFromName("Gunther", mustBeVillager: true), new Vector2(3, 8));
            this.Marlon = new SocialNPC(Game1.getCharacterFromName("Marlon", mustBeVillager: true), new Vector2(8, 5));
            foreach (SocialNPC npc in new[] { this.Gunther, this.Marlon })
            {
                npc.OriginalNpc.currentLocation.characters.Add(npc);
                npc.OriginalNpc.currentLocation.characters.Remove(npc.OriginalNpc);
                npc.ForceReload();
            }
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaving(object sender, SavingEventArgs args)
        {
            // swap in original NPC
            foreach (SocialNPC npc in new[] { this.Gunther, this.Marlon })
            {
                npc.currentLocation.characters.Add(npc.OriginalNpc);
                npc.currentLocation.characters.Remove(npc);
            }
        }
    }
}
