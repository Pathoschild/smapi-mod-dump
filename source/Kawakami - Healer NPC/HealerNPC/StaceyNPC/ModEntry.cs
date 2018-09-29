using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;


namespace HealerNPC
{
    public class ModEntry : Mod, IAssetEditor, IAssetLoader
    {

        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += AfterSaveLoaded;
            GameEvents.UpdateTick += Events_UpdateTick;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            // Prepering to edit files.
            if (asset.AssetNameEquals("Data/NPCDispositions"))
                return true;

            if (asset.AssetNameEquals("Data/NPCGiftTastes"))
                return true;

            if (asset.AssetNameEquals("Characters/Dialogue/rainy"))
                return true;

            if(asset.AssetNameEquals("Characters/Dialogue/rainy"))
                return true;

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            // Editing existing game files.
            if (asset.AssetNameEquals("Data/NPCDispositions"))
            {
                asset.AsDictionary<string, string>().Data["Kawakami"] = "adult/shy/outgoing/negative/female/non-datable/null/Town/fall 9/Gus 'dad'/Saloon 23 4/Kawakami";
            }

            if (asset.AssetNameEquals("Data/NPCGiftTastes"))
            {
                IDictionary<string, string> NPCGiftTastes = asset.AsDictionary<string, string>().Data;
                NPCGiftTastes["Kawakami"] = "Now that is a lovely gift, @. It will really help me!/227 228 220 428 440" +
                    " 787 247/That's so nice of you, @!/724 725" +
                    " 726 303 350/Uh... I'm not sure what should I do with this./2 24 90 174 190 336" +
                    " 194/Why would you even give this to me?/149 205 281 404 420 422/Thank you.//";
            }

            if (asset.AssetNameEquals("Characters/Dialogue/rainy"))
            {
                IDictionary<string, string> rainy = asset.AsDictionary<string, string>().Data;
                rainy["Kawakami"] = "Now I am all wet. Great.$s";
            }
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            //Preparing to load the assets.
            if (asset.AssetNameEquals("Characters/Dialogue/Kawakami"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Characters/Kawakami"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Portraits/Kawakami"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Characters/Schedules/Kawakami"))
            {
                return true;
            }

            return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            //Loading assets from Mod folder.
            if (asset.AssetNameEquals("Characters/Dialogue/Kawakami"))
            {
                return Helper.Content.Load<T>("assets/Kawakami_dialogue.xnb", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Characters/Schedules/Kawakami"))
            {
                return Helper.Content.Load<T>("assets/Kawakami_schedule.xnb", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Characters/Kawakami"))
            {
                return Helper.Content.Load<T>("assets/texture.png", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Portraits/Kawakami"))
            {
                return Helper.Content.Load<T>("assets/portrait.png", ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }


        private void AfterSaveLoaded(object sender, EventArgs args)
        {
            // Spwaning Kamakami in game after the game is loaded.
            Texture2D portrait = Helper.Content.Load<Texture2D>("assets/portrait.png", ContentSource.ModFolder);

            NPC KawakamiNPC = new NPC(null, new Vector2(23, 4), "Saloon", 3, "Kawakami", false, null, portrait);

            Monitor.Log($"Kawakami should have spawned at {KawakamiNPC.Position.X},{KawakamiNPC.Position.Y}");
        }


        private void Events_UpdateTick(object sender, EventArgs e)
        {
            int heal_id = 980;
            int noheal_id = 981;
            
            foreach (int dialogue_int in Game1.player.dialogueQuestionsAnswered)
            {
                if (dialogue_int == heal_id)
                {
                    //Calculating % of missing HP and Stamina to determine healing cost. Cost will vary in between 0-500 (while never reaching 500 because players health can never be 0)
                    float CurrentHealth = Game1.player.health;
                    float MaxHealth = Game1.player.maxHealth;
                    float CurrentStamina = Game1.player.Stamina;
                    float MaxStamina = Game1.player.MaxStamina;
                    int HealthPercentage = (int)Math.Ceiling((1 - (CurrentHealth / MaxHealth)) * 100);
                    int StaminaPercentage = (int)Math.Ceiling((1 - (CurrentStamina / MaxStamina)) * 100);
                    int healcost = (int)Math.Round(((float)(HealthPercentage + StaminaPercentage) / 200) * 500);

                    Monitor.Log($"You are missing {HealthPercentage} % health and {StaminaPercentage} % stamina.");

                    Monitor.Log($"You have been healed. You paid {healcost} gold");
                    Healme();
                    Game1.player.Money -= (healcost);
                    Game1.player.dialogueQuestionsAnswered.Remove(heal_id);
                    //Resetting dialogue choice so it can be used again next day.
                }
                if (dialogue_int == noheal_id)
                {
                    //Resetting dialogue if player did not choose option to get healed.
                    Monitor.Log($"You have chosen not to get healed.");
                    Game1.player.dialogueQuestionsAnswered.Remove(noheal_id);
                }
            }
        }

        private void Healme()
        {
            if (!Context.IsWorldReady || Game1.currentLocation == null)
                return;

            if ((Game1.player.health < Game1.player.maxHealth || Game1.player.stamina < Game1.player.MaxStamina))
            {
                Monitor.Log($"{Game1.player.Name} was healed and had energy restored.");
                //Setting players health and stamina to maximum amount.
                Game1.player.health = Game1.player.maxHealth;
                Game1.player.stamina = Game1.player.MaxStamina;
            }
        }
    }
}