using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Buildings;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace PondPainter
{
    public class PondPainterEntry : Mod
    {
        internal static PondPainterEntry Instance;
        private bool HasLoggedMultiplayerMessage = false;
        private const string ContentPackFile = "pondcolors.json";
        private PondPainterConfig Config;
        private PondPainterData Data = new PondPainterData();
		private Dictionary<FishPond, PondPainterDataColorDef>ActiveAnimations = new Dictionary<FishPond, PondPainterDataColorDef>();
        private uint AnimationFrameCount = 0;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<PondPainterConfig>();

            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            if (Config.Enable_Animations)
            {
                helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            }

            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}", LogLevel.Trace);
                // We are assuming that we will receive the packs in proper dependency order.
                // However, dependencies are traditionally "whoever updates last wins" but tag matching is usually the opposite.
                // So we will try to do the following. If we get pack A, B, C we save the data as C entries, B entries, A entries.
                int entryIndex = 0;

                if (contentPack.HasFile(ContentPackFile))
                {
                    PondPainterPackData packData = contentPack.ReadJsonFile<PondPainterPackData>(ContentPackFile);

                    if (packData.EmptyPondColor != null)
                    {
                        Data.EmptyPondColor = ColorLookup.FromName(packData.EmptyPondColor);
                    }
                    int index = 0;
                    foreach (PondPainterPackEntry entry in packData.Entries)
                    {
                        index++;
						string LogName = String.Format("Entry {0}", index);
						if (entry.LogName != null && !entry.LogName.Equals("")) {
							LogName = entry.LogName;
						}
                        //this.Monitor.Log($"Found an entry called \"{LogName}\" and will now try to parse it.", LogLevel.Debug);
                        if (entry.Tags.Count == 0)
                        {
                            this.Monitor.Log($"Entry \"{LogName}\" has an empty Tags list and will be skipped.", LogLevel.Warn);
                            continue;
                        }
                        int cindex = 0;
						PondPainterDataEntry DataEntry = new PondPainterDataEntry(contentPack.Manifest.UniqueID, LogName, entry.Tags);
                        foreach (PondPainterPackColor c in entry.Colors)
                        {
							cindex++;
                            Color? theColor = null;
                            if (c.ColorName != null)
                            {
                                theColor = ColorLookup.FromName(c.ColorName);
                            }
                            if (theColor == null)
                            {
                                this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} does not have a ColorName defined and will be skipped.", LogLevel.Warn);
                                continue;
                            }
                            // We've passed the null check so it is time to get rid of the damned nullable type
                            Color theRealColor = (Color)theColor;
							bool HasAnimation = false;
							// Some defaults; note the range is a double even though the content pack only takes ints
							// This is because the internal ColorMine properties are all doubles
							int AnimationFrameDelay = 10;
							double AnimationRange = 20.0f;
                            // Of course I had to make this configurable too, which means I have to sanity-check it.
                            // For other options I could delay the sanity checks until I was certain we actually had a properly defined type, but this one
                            //  needs to be checked before I try to construct the animations. This means some meaningless error messages will be logged if
                            //  there is an animation type of "none" and something wrong with this value.
							// The total amount of steps is really twice this variable +1 since we go from base + steps to base - steps
							int AnimationSteps = 30;
                            if (c.AnimationTotalFrames != null)
                            {
                                AnimationSteps = Math.Abs((int)c.AnimationTotalFrames);
                                if (c.AnimationTotalFrames > -2 && c.AnimationTotalFrames < 2)
                                {
                                    this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} has animation total frames set to ({c.AnimationTotalFrames}). The minimum useful value is 2 and that will be used instead.", LogLevel.Warn);
                                    AnimationSteps = 2;
                                }
                                else if (c.AnimationTotalFrames < 0)
                                {
                                    this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} has animation total frames set to ({c.AnimationTotalFrames}). This value should be positive and will be changed to {AnimationSteps}.", LogLevel.Warn);
                                }
                            }
                            List<Color>AnimationColors = new List<Color>();
							if (c.AnimationType != null && !c.AnimationType.Equals("none"))
							{
								// An AnimationType was given, and each type is processed a bit differently.
								// The range will be interpreted differently for various types, but it should
								//   never be 0 since that represents an animation that does nothing.
								// Note that null right now is still ok; it is only an explicit 0 being excluded
								if (c.AnimationRange == null || c.AnimationRange != 0)
								{
									if (c.AnimationType.Equals("hue"))
									{
										if (c.AnimationRange != null)
										{
											// restrict range to (0, 180]
											if (c.AnimationRange < 0 || c.AnimationRange > 180 )
											{
												AnimationRange = Math.Min(180.0f, Math.Abs((int)c.AnimationRange));
												this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} has an animation range of ({c.AnimationRange}). Hue animations must have a positive range <= 180 so this will be interpreted as {AnimationRange}.", LogLevel.Warn);
											}
											else
											{
												AnimationRange = (double)c.AnimationRange;
											}
										}
										else
										{
											AnimationRange = 20.0f;
											this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} had no animation range listed; the default of {AnimationRange} will be used.", LogLevel.Debug);
										}
										HasAnimation = true;
                                        // Calculating the animation frames, in HSV via ColorMine
                                        // We use a sine model with the animation "range" as amplitude and a period of the number of steps.
                                        SimpleRGB BaseRGB = new SimpleRGB(theRealColor.R, theRealColor.G, theRealColor.B);
                                        SimpleHSV BaseHSV = BaseRGB.ToHSV();
                                        this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} Tracing HUE animation with base of {BaseHSV.H} and a range of {AnimationRange} in {AnimationSteps} steps.", LogLevel.Trace);
                                        for (int i = 0; i <= AnimationSteps; i++)
										{
                                            // We can't do the simpler NewHSV = BaseHSV because that is not a true copy
                                            SimpleHSV NewHSV = BaseRGB.ToHSV();
                                            double hue = (360 + BaseHSV.H + AnimationRange*Math.Sin(2*i*Math.PI/AnimationSteps)) % 360;
											NewHSV.H = hue;
                                            //this.Monitor.Log($"** Animation trace step {i}: hue {hue}. Base {BaseHSV.H}", LogLevel.Trace);
                                            SimpleRGB NewRGB = NewHSV.ToRGB();
											AnimationColors.Add(new Color((int)NewRGB.R, (int)NewRGB.G, (int)NewRGB.B));
										}
                                        
									}
									else if  (c.AnimationType.Equals("value"))
									{
										if (c.AnimationRange != null)
										{
											// restrict range to (0, 100] (will be converted to (0, 1] later)
											if (c.AnimationRange < 0 || c.AnimationRange > 100 )
											{
												AnimationRange = Math.Min(100, Math.Abs((int)c.AnimationRange));
												this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} has an animation range of {c.AnimationRange}. Value animations must have a positive range <= 100 so this will be interpreted as {AnimationRange}.", LogLevel.Warn);
											}
											else
											{
												AnimationRange = (double)c.AnimationRange;
											}
										}
										else
										{
											AnimationRange = 20.0f;
											this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} had no animation range listed; the default of {AnimationRange} will be used.", LogLevel.Info);
										}
										AnimationRange /= 100.0f;
										HasAnimation = true;
                                        // Calculating the animation frames, in HSV via ColorMine
                                        // We use a sine model with the animation "range" as amplitude and a period of the number of steps.
                                        
                                        SimpleRGB BaseRGB = new SimpleRGB(theRealColor.R, theRealColor.G, theRealColor.B);
										SimpleHSV BaseHSV = BaseRGB.ToHSV();
                                        //this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} Tracing VALUE animation with base of {BaseHSV.V} and a range of {AnimationRange} in {AnimationSteps} steps.", LogLevel.Trace);
                                        for (int i = 0; i <= AnimationSteps; i++)
										{
                                            // We can't do the simpler NewHSV = BaseHSV because that is not a true copy
                                            SimpleHSV NewHSV = BaseRGB.ToHSV();
                                            double val =  Math.Min(Math.Max(BaseHSV.V + AnimationRange*Math.Sin(2*i*Math.PI/AnimationSteps),0),1);
											NewHSV.V = val;
                                            this.Monitor.Log($"** Animation trace step {i}: value {val}. Base {BaseHSV.V}", LogLevel.Trace);
                                            SimpleRGB NewRGB = NewHSV.ToRGB();
											AnimationColors.Add(new Color((int)NewRGB.R, (int)NewRGB.G, (int)NewRGB.B));
										}
                                        
									}
									else
									{
										this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} has an unknown animation type ({c.AnimationType}). A static color will be used instead.", LogLevel.Warn);
									}
								}
								else // AnimationRange was 0
								{
									this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} has an animation range of zero. A static color will be used instead.", LogLevel.Warn);
								}
							}
							if (HasAnimation)
							{
								// To get here, we had a valid animation type with an appropriate range.
								// One final sanity check is on the timing.
								if (c.AnimationFrameDelay == null || c.AnimationFrameDelay == 0)
								{
									this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} had no animation frame delay listed; the default of {AnimationFrameDelay} frames will be used.", LogLevel.Info);
								}
								else
								{
									AnimationFrameDelay = Math.Abs((int)c.AnimationFrameDelay);
									if (c.AnimationFrameDelay < 0)
									{
										this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} had an animation frame delay of {c.AnimationFrameDelay}; timings must be positive so this will be interpreted as {AnimationFrameDelay}.", LogLevel.Warn);
									}
								}
								DataEntry.Colors.Add(c.MinPopulationForColor, new PondPainterDataColorDef(AnimationColors, AnimationFrameDelay));
                                this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} successfully added as a new animation.", LogLevel.Trace);
                            }
							else
							{
								DataEntry.Colors.Add(c.MinPopulationForColor, new PondPainterDataColorDef((Color)theColor));
                                this.Monitor.Log($"Entry \"{LogName}\" color definition {cindex} successfully added as a new static color.", LogLevel.Trace);
                            }				
                        }
                        Data.Entries.Insert(entryIndex++, DataEntry);
                        //this.Monitor.Log($"Entry \"{LogName}\" complete entry added to internal data.", LogLevel.Trace);
                    }
                }
                else
                {
                    this.Monitor.Log($"Unable to load content pack {contentPack.Manifest.Name} {contentPack.Manifest.Version} because no {ContentPackFile} file was found.", LogLevel.Warn);
                }
            }
            this.Monitor.Log($"Finished loading content packs. Data has {Data.Entries.Count} entries.", LogLevel.Trace);
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
			// Because pond colors are synched in multiplayer, animations are currently disabled in that context.
			// Future support might be added if the colors can be changed locally without triggering synch.
			// Tractor Mod might have some examples of using reflection in this way.
			// TODO: Make sure we have a way to remove ponds that are no longer being animated (or no longer exist)
			// and perhaps skip out of this function early if the ActiveAnimations count is 0.
			if (Config.Enable_Animations) {
				if (!Context.IsMultiplayer && Instance.ActiveAnimations.Count > 0) {
                    Instance.AnimationFrameCount++;
                    foreach (KeyValuePair<FishPond, PondPainterDataColorDef> kvp in Instance.ActiveAnimations)
                    { 
						if (kvp.Key != null && Instance.ActiveAnimations.ContainsKey(kvp.Key))
						{
							if (Instance.AnimationFrameCount % Instance.ActiveAnimations[kvp.Key].AnimationTiming == 0)
							{
								kvp.Value.AnimationCurrentFrame++;
								if (kvp.Value.AnimationCurrentFrame >= kvp.Value.AnimationColors.Count)
								{
                                    kvp.Value.AnimationCurrentFrame = 0;
								}
                                kvp.Key.overrideWaterColor.Value = kvp.Value.AnimationColors[kvp.Value.AnimationCurrentFrame];
                                //this.Monitor.Log($"Tried to change animation for pond at {kvp.Key.tileX}, {kvp.Key.tileY} to frame {kvp.Value.AnimationCurrentFrame}", LogLevel.Trace);
                            }
                        }
					}
				}
			}
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Game1.player.IsMainPlayer)
            {
                if (!this.HasLoggedMultiplayerMessage)
                {
                    this.Monitor.Log("Not main player, so no color changes will be attempted", LogLevel.Debug);
                    this.HasLoggedMultiplayerMessage = true;
                }
                return;
            }

            // Because we need to account for the possibility of previously animated ponds being no longer animated due to population changes
            //  (or no longer existing) and we are rescanning all ponds anyway, I am just gonna clear the Active Animations list every morning.
            Instance.ActiveAnimations.Clear();

            IEnumerable<BuildableGameLocation> BuildableLocations =
                from loc in Game1.locations where loc is BuildableGameLocation select loc as BuildableGameLocation;
            foreach (BuildableGameLocation bloc in BuildableLocations) {
                IEnumerable<FishPond> FishPonds =
                    from b in bloc.buildings where b is FishPond select b as FishPond;
                foreach (FishPond pond in FishPonds)
                {
                    bool pondHasCustomColor = false;
                    this.Monitor.Log($"Checking pond at {pond.tileX}, {pond.tileY}.", LogLevel.Trace);
                    if (Config.Enable_Custom_Pond_Coloring)
                    {
                        if (pond.fishType == -1 && Data.EmptyPondColor != null)
                        {
                            pond.overrideWaterColor.Value = Data.EmptyPondColor;
                            this.Monitor.Log($"Pond is empty and was recolored", LogLevel.Trace);
                            break;
                        }
                        foreach (PondPainterDataEntry entry in Data.Entries)
                        {
                            bool match = true;
                            //this.Monitor.Log($"Comparing with entry {entry.LogName}", LogLevel.Trace);
                            foreach (string t in entry.Tags)
                            {
								/* For now we are removing the capability to set a default color until I find a better
								way to do it.
                                if (t.Equals("pp_default"))
                                {
                                    // pp_default trumps all other tags and immediately sets the color
									// I need a better way to do this, possibly moving it to a config instead of per pack.
                                    ChangePondColor(pond, entry);
                                    match = false;
                                    break;
                                }
								//*/
								SObject pondFish = new SObject(pond.fishType.Value, 1, false, -1, 0);
								if (!pondFish.HasContextTag(t))
								{
									match = false;
								}
                            }
                            if (match) {
                                pondHasCustomColor = true;
                                this.Monitor.Log($"Found a match on entry {entry.LogName} with tags {String.Join(", ", entry.Tags)}.", LogLevel.Trace);
                                ChangePondColor(pond, entry);
                                break;
                            }
                        }
                    }
                    if (Config.Auto_Color_Other_Ponds_by_Dye_Color_of_Inhabitants && !pondHasCustomColor)
                    {
                        if (pond.FishCount > 0 && pond.FishCount >= Config.Minimum_Population_For_Auto_Coloring)
                        {
                            // Copying similar logic to how the roe is colored, but convert
                            // White to nearest color Snow since White means no override to the game
                            Color? c = TailoringMenu.GetDyeColor(pond.GetFishObject());
                            Color? white = new Color?(Color.White);
                            if (c.HasValue)
                            {
                                if (c.Equals(white))
                                {
                                    c = new Color?(Color.Snow);
                                }
                            }
                            else
                            {
                                c = white;
                            }
                            pond.overrideWaterColor.Value = c.Value;
                        }
                    }

                }
            }
            this.Monitor.Log($"Finished Day Update. There are {Instance.ActiveAnimations.Count} active animations.", LogLevel.Trace);
        }

        private void ChangePondColor(FishPond pond, PondPainterDataEntry entry)
        {
            // This should be iterating in descending order, so the first match we have takes priority
            foreach (KeyValuePair<int, PondPainterDataColorDef> kvp in entry.Colors)
            {
                if (pond.FishCount >= kvp.Key)
                {
					if (kvp.Value.HasAnimation) 
					{
						pond.overrideWaterColor.Value = kvp.Value.AnimationColors[0];
						kvp.Value.AnimationCurrentFrame = 0;
						Instance.ActiveAnimations[pond] = kvp.Value;
						this.Monitor.Log($"Set the color for the pond at {pond.tileX}, {pond.tileY} with population {pond.FishCount} to an animation starting with Color {kvp.Key}: {kvp.Value.AnimationColors[0]}.", LogLevel.Trace);
					}
					else
					{
						pond.overrideWaterColor.Value = kvp.Value.StaticColor;
						this.Monitor.Log($"Set the color for the pond at {pond.tileX}, {pond.tileY} with population {pond.FishCount} to static Color {kvp.Key}: {kvp.Value.StaticColor}", LogLevel.Trace);
					}
                    return;
                }
            }
            this.Monitor.Log($"Did not set color for pond at {pond.tileX}, {pond.tileY} with population {pond.FishCount} because no matching qualifying definition.", LogLevel.Trace);
        }

        public void Log(string message, LogLevel level = LogLevel.Debug)
        {
            // I don't know how to log directly from ColorLookup, so this will have to do.
            this.Monitor.Log(message, level);
        }
    }
}
