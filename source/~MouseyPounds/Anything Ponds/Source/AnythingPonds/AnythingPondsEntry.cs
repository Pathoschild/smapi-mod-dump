using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using SObject = StardewValley.Object;

namespace AnythingPonds
{
    public class AnythingPondsEntry : Mod, IAssetEditor
    {
        private AnythingPondsConfig Config;
        private AnythingPondsPondData AlgaePondData;
        private AnythingPondsPondData GenericPondData;
        private Dictionary<FishPond, AnythingPondsTracker> EmptyPonds;
        private bool HasAddedAlgaePondData = false;
        private bool HasAddedGenericPondData = false;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<AnythingPondsConfig>();
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;

            if (Config.Enable_Algae_and_Seaweed_Pond_Definitions)
            {
                AlgaePondData = helper.Data.ReadJsonFile<AnythingPondsPondData>("pond_algae.json");
                if (AlgaePondData == null)
                {
                    Monitor.Log("Can't find algae json file; a fresh copy will be created.", LogLevel.Info);
                    AlgaePondData = new AnythingPondsPondData("Algae");
                    helper.Data.WriteJsonFile("pond_algae.json", AlgaePondData);
                }
                else
                {
                    Monitor.Log("Algae json file loaded.");
                }
            }
            GenericPondData = helper.Data.ReadJsonFile<AnythingPondsPondData>("pond_generic.json");
            if (GenericPondData == null)
            {
                Monitor.Log("Can't find generic pond json file; a fresh copy will be created.", LogLevel.Info);
                GenericPondData = new AnythingPondsPondData("Generic");
                helper.Data.WriteJsonFile("pond_generic.json", GenericPondData);
            }
            else
            {
                Monitor.Log("Generic pond json file loaded.");
            }

            // Originally I had these all wrapped in an if (Config.Allow_Empty_Ponds_to_Become_Algae_or_Seaweed),
            //  but now we are tracking the ponds regardless mainly so that if somebody decides they want to turn
            //  that feature on and already have empty ponds they won't have to wait additional time. This does
            //  mean a bit of extra info is stored in saves that the user doesn't need if the option is off.
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            Helper.Content.InvalidateCache("Data/FishPondData");
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady &&
                Game1.currentLocation != null &&
                Game1.activeClickableMenu == null &&
                Game1.player.CurrentItem != null &&
                Game1.player.CurrentItem.canBeGivenAsGift() &&
                e.Button.IsActionButton())
            {
                FishPond pond = GetPondAtTile(Game1.currentLocation, e.Cursor.GrabTile);
                if (pond != null && !pond.isUnderConstruction())
                {
                    Monitor.Log($"Trying to add an item ({Game1.player.CurrentItem.ParentSheetIndex}) to a pond of type {pond.fishType.Value}.", LogLevel.Trace);
                    if (pond.fishType.Value == -1 || pond.fishType.Value == Game1.player.CurrentItem.ParentSheetIndex)
                    {
                        if (pond.currentOccupants.Value >= pond.maxOccupants.Value)
                        {
                            Monitor.Log("Right item, but pond is full.", LogLevel.Trace);
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:PondFull"));
                            Helper.Input.Suppress(e.Button);
                        }
                        else
                        {
                            bool success = Helper.Reflection.GetMethod(pond, "addFishToPond").Invoke<bool>(Game1.player, Game1.player.ActiveObject);
                            Monitor.Log("Pond is empty or pond contains this item with room for more so we tried to add it. Success? {success}.", LogLevel.Trace);
                            if (success)
                            {
                                Helper.Input.Suppress(e.Button);
                            }
                        }
                    }
                    else
                    {
                        Monitor.Log("This is not the right item for this pond. Mod will ignore it and not suppress the click.", LogLevel.Trace);
                    }
                }
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // This function only reads the save data and uses it to create our internal pond list
            // Because of this, it is only relevant to the host
            if (Context.IsMainPlayer)
            {
                Monitor.Log("Checking for and restoring empty pond data from save", LogLevel.Trace);
                EmptyPonds = new Dictionary<FishPond, AnythingPondsTracker>();
                AnythingPondsSaveData saveData = Helper.Data.ReadSaveData<AnythingPondsSaveData>("EmptyPonds");
                if (saveData != null)
                {
                    foreach (string key in saveData.EmptyPonds.Keys)
                    {
                        string[] keySplit = key.Split(new char[] { '/' });
                        BuildableGameLocation loc = Game1.getLocationFromName(keySplit[0]) as BuildableGameLocation;
                        if (loc != null)
                        {
                            Vector2 tile = new Vector2(Convert.ToInt32(keySplit[1]), Convert.ToInt32(keySplit[2]));
                            // We want to verify the pond exists and is still empty
                            FishPond pond = GetPondAtTile(loc, tile);
                            if (IsEmpty(pond))
                            {
                                EmptyPonds.Add(pond, new AnythingPondsTracker(keySplit[0], saveData.EmptyPonds[key]));
                            }
                        }
                    }
                }
            }
            else
            {
                if (Config.Allow_Empty_Ponds_to_Become_Algae_or_Seaweed)
                {
                    Monitor.Log("Not main player, so empty pond conversion to algae/seaweed unavailable", LogLevel.Debug);
                }
            }
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            // Now we recreate the save data from our pond list. Again, host only.
            if (Context.IsMainPlayer)
            {
                // First we will go through every pond on the map and add any that are empty to our list. 
                IEnumerable<BuildableGameLocation> BuildableLocations =
                    from loc in Game1.locations where loc is BuildableGameLocation select loc as BuildableGameLocation;
                foreach (BuildableGameLocation bloc in BuildableLocations)
                {
                    IEnumerable<FishPond> FishPonds =
                        from b in bloc.buildings where b is FishPond select b as FishPond;
                    foreach (FishPond pond in FishPonds)
                    {
                        if (IsEmpty(pond))
                        {
                            if (!EmptyPonds.ContainsKey(pond))
                            {
                                EmptyPonds.Add(pond, new AnythingPondsTracker(bloc.Name, 0));
                            }
                        }
                    }
                }
                // Now we will convert our list back into saveable data.
                // We need to make sure each pond still exists and will also reverify it is empty.
                AnythingPondsSaveData saveData = new AnythingPondsSaveData();
                foreach (FishPond pond in EmptyPonds.Keys)
                {
                    if (IsEmpty(pond))
                    {
                        string key = $"{EmptyPonds[pond].locationName}/{pond.tileX}/{pond.tileY}";
                        saveData.EmptyPonds[key] = EmptyPonds[pond].days;

                    }
                }
                Monitor.Log("Writing out empty pond data to save", LogLevel.Trace);
                Helper.Data.WriteSaveData<AnythingPondsSaveData>("EmptyPonds", saveData);
            }
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            // The only thing we are doing here is checking our ponds to see if they are empty; if so,
            //  we next check if it is time to turn them into an algae or seaweed pond.
            // Only the host should be running this so we start by verifying that.
            if (Context.IsMainPlayer)
            {
                IEnumerable<BuildableGameLocation> BuildableLocations =
                    from loc in Game1.locations where loc is BuildableGameLocation select loc as BuildableGameLocation;
                foreach (BuildableGameLocation bloc in BuildableLocations)
                {
                    IEnumerable<FishPond> FishPonds =
                        from b in bloc.buildings where b is FishPond select b as FishPond;
                    foreach (FishPond pond in FishPonds)
                    {
                        if (IsEmpty(pond))
                        {
                            // Not sure this would actually happen but better safe than sorry
                            if (!EmptyPonds.ContainsKey(pond))
                            {
                                EmptyPonds.Add(pond, new AnythingPondsTracker(bloc.Name, 0));
                            }
                            EmptyPonds[pond].Increment();
                            if (Config.Allow_Empty_Ponds_to_Become_Algae_or_Seaweed &&
                               EmptyPonds[pond].days >= Config.Number_of_Days_for_Empty_Pond_to_become_Algae_or_Seaweed)
                            {
                                int type = AnythingPondsConstants.Seaweed;
                                switch (Game1.random.Next(3))
                                {
                                    case 0: type = AnythingPondsConstants.Seaweed; break;
                                    case 1: type = AnythingPondsConstants.GreenAlgae; break;
                                    case 2: type = AnythingPondsConstants.WhiteAlgae; break;
                                }
                                AddFishToPond(pond, new SObject(type, 1, false, 1, 0));
                                Monitor.Log($"Empty pond at {EmptyPonds[pond].locationName} ({pond.tileX}, {pond.tileY}) now contains type {type}.", LogLevel.Debug);
                                EmptyPonds.Remove(pond);
                            }
                        }
                    }
                }
            }
        }

        private bool IsEmpty(FishPond pond)
        {
            // We do this check a lot so here's a helper
            return (pond != null && pond.fishType.Value == -1 && pond.currentOccupants == 0 && !pond.isUnderConstruction());
        }

        private FishPond GetPondAtTile(GameLocation loc, Vector2 tile)
        {
            if (loc is BuildableGameLocation)
            {
                BuildableGameLocation bloc = (BuildableGameLocation)loc;
                foreach (Building building in bloc.buildings)
                {
                    if (building is FishPond && building.occupiesTile(tile))
                    {
                        return (FishPond)building;
                    }
                }
                return null;
            }
            return null;
        }

        private void AddFishToPond(FishPond pond, SObject o)
        {
            // We don't want to call the game's addFishToPond because it requires a farmer object and
            //   1) reduces the currently selected item of the farmer by 1
            //   2) tries to show the throwing animation into the pond
            // These are problems because we want to stealthily add items on day start.
            // To counter this, instead of calling that single function via reflection, we
            //   try to mimic most of what that function does, reflecting whenever necessary
            if (pond.currentOccupants == 0)
            {
                pond.fishType.Value = o.ParentSheetIndex;
                Helper.Reflection.GetField<FishPondData>(pond, "_fishPondData").SetValue(null);
                pond.UpdateMaximumOccupancy();
            }
            pond.currentOccupants.Value++;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/FishPondData") && !HasAddedGenericPondData)
            {
                return true;
            }
            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/FishPondData"))
            {
                IList<FishPondData> data = asset.GetData<List<FishPondData>>();
                // Generic Pond data should be absolutely last, so just Add it
                if (!HasAddedGenericPondData)
                {
                    foreach (FishPondData newData in GenericPondData.Entries)
                    {
                        Monitor.Log($"Adding pond definition for {newData.RequiredTags[0]} to end of pond data", LogLevel.Trace);
                        data.Add(newData);
                    }
                    HasAddedGenericPondData = true;
                }

                // Seaweed and Algae and such should come a little earlier. We'd like
                //  to put it before the game's normal "category_fish" catchall, but if we
                //  can't find that, we'll insert it just before the last entry.
                if (!HasAddedAlgaePondData && Config.Enable_Algae_and_Seaweed_Pond_Definitions)
                {
                    int insertPosition = data.Count - 1;
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (data[i].RequiredTags[0].Equals("category_fish"))
                        {
                            insertPosition = i;
                            break;
                        }
                    }
                    foreach (FishPondData newData in AlgaePondData.Entries)
                    {
                        Monitor.Log($"Inserting pond definition for {newData.RequiredTags[0]} at position {insertPosition} of {data.Count}", LogLevel.Trace);
                        data.Insert(insertPosition++, newData);
                    }
                    HasAddedAlgaePondData = true;
                }
            }
        }
    }
}