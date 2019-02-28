using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ChildBedConfig
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetLoader
    {

        /*****************************/
        /**      Properties         **/
        /*****************************/
        ///<summary>The config file from the player</summary>
        private ModConfig Config;
        private Farmer Farmer;

        /*****************************/
        /**      Public methods     **/
        /*****************************/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            Farmer = new Farmer();
        }

        public void GetFarmer()
        {
            for (int i = 0; i < Config.Farmers.Count; i++)
            {
                if (Config.Farmers[i].CharacterName == Game1.player.Name)
                {
                    Farmer = Config.Farmers[i];
                    break;
                }
            }

            if (Farmer.CharacterName.CompareTo("NoName") == 0)
            {
                Monitor.Log("No config information was found for this character.  Loading default map.", LogLevel.Info);
            }
            else
            {
                Monitor.Log("Config info found for " + Farmer.CharacterName + ".  Proceeding to load appropriate map.");
            }

        }

        /// <summary>
        /// Determines whether or not an asset can be loaded
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset">Basic metadata about the asset being loaded</param>
        /// <returns>Returns true if asset can be edited</returns>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Maps/FarmHouse2") || asset.AssetNameEquals("Maps/FarmHouse2_marriage"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Load a matched asset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            GetFarmer();
            //If we couldn't find any config info for the current player OR all values are set to true, we don't edit the map
            if (Farmer.CharacterName.CompareTo("NoName") != 0 || Farmer.ShowCrib && Farmer.ShowBed1 && Farmer.ShowBed2)
            {
                //If the player is married we replace FarmHouse2_marriage
                if (Game1.player.isMarried())
                {
                    //Show crib, no beds
                    if (Farmer.ShowCrib && !Farmer.ShowBed1 && !Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/married/Crib.tbin", ContentSource.ModFolder);
                    }
                    //Show crib, bed 1
                    else if (Farmer.ShowCrib && Farmer.ShowBed1 && !Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/married/Crib_Bed1.tbin", ContentSource.ModFolder);
                    }
                    //Show crib, bed 2
                    else if (Farmer.ShowCrib && !Farmer.ShowBed1 && Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/married/Crib_Bed2.tbin", ContentSource.ModFolder);
                    }
                    //Show no crib, both beds
                    else if (!Farmer.ShowCrib && Farmer.ShowBed1 && Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/married/Bed1_Bed2.tbin", ContentSource.ModFolder);
                    }
                    //Show only bed 1
                    else if (!Farmer.ShowCrib && Farmer.ShowBed1 && !Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/married/Bed1.tbin", ContentSource.ModFolder);
                    }
                    //Show only bed 2
                    else if (!Farmer.ShowCrib && !Farmer.ShowBed1 && Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/married/Bed2.tbin", ContentSource.ModFolder);
                    }
                    //Show none
                    else
                    {
                        return this.Helper.Content.Load<T>("assets/married/None.tbin", ContentSource.ModFolder);
                    }
                }

                //If the player is single we replace FarmHouse2
                else
                {
                    //Show crib, no beds
                    if (Farmer.ShowCrib && !Farmer.ShowBed1 && !Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/single/Crib.tbin", ContentSource.ModFolder);
                    }
                    //Show crib, bed 1
                    else if (Farmer.ShowCrib && Farmer.ShowBed1 && !Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/single/Crib_Bed1.tbin", ContentSource.ModFolder);
                    }
                    //Show crib, bed 2
                    else if (Farmer.ShowCrib && !Farmer.ShowBed1 && Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/single/Crib_Bed2.tbin", ContentSource.ModFolder);
                    }
                    //Show no crib, both beds
                    else if (!Farmer.ShowCrib && Farmer.ShowBed1 && Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/single/Bed1_Bed2.tbin", ContentSource.ModFolder);
                    }
                    //Show only bed 1
                    else if (!Farmer.ShowCrib && Farmer.ShowBed1 && !Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/single/Bed1.tbin", ContentSource.ModFolder);
                    }
                    //Show only bed 2
                    else if (!Farmer.ShowCrib && !Farmer.ShowBed1 && Farmer.ShowBed2)
                    {
                        return this.Helper.Content.Load<T>("assets/single/Bed2.tbin", ContentSource.ModFolder);
                    }
                    //Show none
                    else
                    {
                        return this.Helper.Content.Load<T>("assets/single/None.tbin", ContentSource.ModFolder);
                    }
                }
            }
            return this.Helper.Content.Load<T>("");
        }
    }
}