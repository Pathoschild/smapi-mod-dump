/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using BattleRoyale.Extensions;
using BattleRoyale.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace BattleRoyale
{

    //Collapse this and never look inside again
    [Serializable]
    public class Phase
    {
        public static readonly int PixelFillRate = 340;

        [Serializable]
        public class Location
        {
            public string LocationName { get; set; }
            public Storm.Direction Direction { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public TwoPointRectangle CloseInRectangle { get; set; } = null;

            public Location() { }

            public Location(string locationName, Storm.Direction direction, TwoPointRectangle closeInRectangle = null)
            {
                LocationName = locationName;
                Direction = direction;
                CloseInRectangle = closeInRectangle;
            }
        }

        [Serializable]
        public class TwoPointRectangle
        {
            public int X1 { get; set; } = 0;
            public int Y1 { get; set; } = 0;
            public int X2 { get; set; } = 0;
            public int Y2 { get; set; } = 0;

            public TwoPointRectangle() { }

            public TwoPointRectangle(int x1, int y1, int x2, int y2)
            {
                X1 = x1;
                X2 = x2;
                Y1 = y1;
                Y2 = y2;
            }

            public override string ToString()
            {
                return $"[({X1}, {Y1}), ({X2}, {Y2})]";
            }
        }

        public TimeSpan GetDuration()
        {
            if (Duration != null)
                return (TimeSpan)Duration;

            int maxFill = 0;
            foreach (Location location in Locations)
            {
                GameLocation gameLocation = Game1.getLocationFromName(location.LocationName);
                xTile.Layers.Layer layer = gameLocation.Map.GetLayer("Back");
                int area = layer.LayerHeight * layer.LayerWidth;

                int fillTime = Math.Max(Math.Min(area / PixelFillRate, 59), 15);
                if (fillTime > maxFill)
                    maxFill = fillTime;
            }

            return new TimeSpan(0, 0, seconds: maxFill);
        }

        public List<Location> Locations { get; set; }

        public TimeSpan? Duration { get; set; } = null;

        public TimeSpan Delay { get; set; } = new TimeSpan(0, 0, seconds: 8);

        // JSON.Net needs a parameter-less constructor
        public Phase() { }


        public Phase(List<object> locations, TimeSpan delay, TimeSpan? duration = null)
        {
            Locations = locations.Select((x) => x is Tuple<string, Storm.Direction, TwoPointRectangle> y ? new Location(
                    y.Item1,
                    y.Item2,
                    y.Item3
                ) :
                x is Tuple<string, Storm.Direction> y2 ?
                new Location(
                y2.Item1,
                y2.Item2) : null).ToList();

            Duration = duration;
            Delay = delay;
        }

        public Phase(List<object> locations, TimeSpan? duration = null) : this(locations, new TimeSpan(0, 0, seconds: 8), duration) { }
    }

    public class Storm
    {
        private static readonly Color StormBaseColor = new(147, 112, 219);
        private static readonly float StormAlpha = 0.6f;

        private static Texture2D pixelTexture = null;

        public static List<Phase> Phases { get; set; }
        public static bool GingerIslandPhase = false;
        private static TimeSpan totalLengthOfPhases = new(0);

        public enum Direction
        {
            LeftToRight, RightToLeft, UpToDown, DownToUp, CloseIn
        }

        private static DateTime startTime;
        public static Dictionary<GameLocation, DateTime> TimeLocationWasReached = new();

        public static void StartStorm(int stormIndex)
        {
            InitializePhases(stormIndex);
            startTime = DateTime.Now;

            totalLengthOfPhases = new TimeSpan(0);
            foreach (var phase in Phases)
            {
                totalLengthOfPhases += phase.Delay;
                totalLengthOfPhases += phase.GetDuration();
            }

            TimeLocationWasReached.Clear();
        }

        public static void SendReachedLocationData()
        {
            var b = new BinaryFormatter();
            var stream = new MemoryStream();

            Dictionary<string, DateTime> serializedLocations = new();

            foreach (var kvp in TimeLocationWasReached)
                serializedLocations[kvp.Key.Name] = kvp.Value;

            b.Serialize(stream, serializedLocations);
            byte[] msg = stream.GetBuffer();

            var message = new StardewValley.Network.OutgoingMessage(NetworkUtils.uniqueMessageType, Game1.player,
                (int)NetworkUtils.MessageTypes.SEND_STORM_LOCATION_DATA,
                msg);

            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                if (Game1.player == farmer)
                    continue;

                Game1.server.sendMessage(farmer.UniqueMultiplayerID, message);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {

            if (!ModEntry.BRGame.InProgress)
                return;

            //var (currentPhase, currentPhaseAmount) = GetCurrentPhase();
            var tp = GetCurrentPhase();
            var currentPhase = tp.Item1;
            var currentPhaseAmount = tp.Item2;

            if (Game1.currentLocation != null)
            {
                //var (amount, direction, closeInRectangle) = GetStormAmountInLocation(currentPhase, currentPhaseAmount, Game1.currentLocation);
                var tp2 = GetStormAmountInLocation(currentPhase, currentPhaseAmount, Game1.currentLocation);
                var amount = tp2.Item1;
                var direction = tp2.Item2;
                var closeInRectangle = tp2.Item3;

                DrawSingleStorm(direction, amount, Game1.currentLocation, spriteBatch, closeInRectangle);
            }
        }

        //(double amount, Direction direction, Phase.TwoPointRectangle closeInRectangle)
        public static Tuple<double, Direction, Phase.TwoPointRectangle> GetStormAmountInLocation(int currentPhase, double currentPhaseAmount, GameLocation gameLocation)
        {
            if (!ModEntry.BRGame.InProgress || ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.SLUGFEST))
                return Tuple.Create<double, Direction, Phase.TwoPointRectangle>(0, Direction.DownToUp, null);

            if (TimeLocationWasReached.ContainsKey(gameLocation))
            {
                DateTime reachedAt = TimeLocationWasReached[gameLocation];
                double timeSince = (int)(DateTime.Now - reachedAt).TotalMilliseconds;
                double filledAmount = timeSince / 10000.0;  // 10,000ms = 10s

                return Tuple.Create<double, Direction, Phase.TwoPointRectangle>((filledAmount > 1) ? 1 : filledAmount, Direction.DownToUp, null);
            }

            for (int i = 0; i <= currentPhase; i++)
            {
                Phase phase = Phases[i];
                foreach (Phase.Location location in phase.Locations)
                {
                    if (gameLocation.Name == location.LocationName)
                    {
                        return Tuple.Create((i == currentPhase) ? currentPhaseAmount : 1, location.Direction, location.CloseInRectangle);
                    }
                }
            }

            foreach (Phase.Location location in Phases.SelectMany(x => x.Locations))
            {
                if (gameLocation.Name == location.LocationName)
                {
                    return Tuple.Create<double, Direction, Phase.TwoPointRectangle>(0, Direction.UpToDown, null);
                }
            }

            return Tuple.Create<double, Direction, Phase.TwoPointRectangle>(0, Direction.UpToDown, null); // For unregistered locations, e.g. sheds/barns
        }

        public static int GetRandomStormIndex()
        {
            int totalSize = StormDataModel.Phases.Length + StormDataModel.IslandPhases.Length;

            int attempts = 0;
            int randomIdx = Game1.random.Next(totalSize);

            while (ModEntry.BRGame.StormIndexHistory.Contains(randomIdx) && attempts < 10)
            {
                randomIdx = Game1.random.Next(totalSize);
                attempts++;
            }

            ModEntry.BRGame.StormIndexHistory.Add(randomIdx);
            if (ModEntry.BRGame.StormIndexHistory.Count > ModEntry.BRGame.StormIndexHistorySize)
                ModEntry.BRGame.StormIndexHistory.RemoveAt(0);

            if (randomIdx >= StormDataModel.Phases.Length)
                GingerIslandPhase = true;
            else
                GingerIslandPhase = false;

            return randomIdx;
        }

        public static bool IsGingerIslandPhase()
        {
            return GingerIslandPhase;
        }

        private static void InitializePhases(int stormIndex)
        {
            if (stormIndex >= StormDataModel.Phases.Length)
            {
                int adjustedIndex = stormIndex - StormDataModel.Phases.Length;
                Phases = StormDataModel.IslandPhases[adjustedIndex].ToList();
            }
            else if (stormIndex < StormDataModel.Phases.Length)
                Phases = StormDataModel.Phases[stormIndex].ToList();
        }


        //(int currentPhase, double currentPhaseAmount)
        public static Tuple<int, double> GetCurrentPhase()
        {
            int currentPhase = -1;
            double currentPhaseAmount = 1;

            //TODO: multiply speedMultiplier to speed it up when there are fewer players?
            double speedMultiplier = 1;
            TimeSpan gameDuration = DateTime.Now - startTime;
            gameDuration = new TimeSpan((long)(gameDuration.Ticks * speedMultiplier));

            TimeSpan n = TimeSpan.Zero;

            for (int i = 0; i < Phases.Count; i++)
            {
                Phase phase = Phases[i];

                n += phase.Delay;
                if (n >= gameDuration)
                    break;

                currentPhase = i;
                n += phase.GetDuration();
                if (n >= gameDuration)
                {
                    currentPhaseAmount = Math.Max(0, Math.Min(1, 1 - (n - gameDuration).TotalMilliseconds / phase.GetDuration().TotalMilliseconds));
                    break;
                }
            }

            return Tuple.Create(currentPhase, currentPhaseAmount);
        }

        /// <summary>
        /// amount: 0 to 1
        /// </summary>
        public static void DrawSingleStorm(Direction direction, double amount, GameLocation gameLocation, SpriteBatch spriteBatch, Phase.TwoPointRectangle closeInRectangle)
        {
            if (gameLocation == null)
                return;

            if (pixelTexture == null)
            {
                pixelTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
                pixelTexture.SetData(new Color[1] { StormBaseColor * StormAlpha });
            }

            if (SpectatorMode.InSpectatorMode && Game1.activeClickableMenu != null)
                return;

            Rectangle b = GetStormBounds(direction, amount, gameLocation, closeInRectangle);

            if (direction != Direction.CloseIn)
            {
                spriteBatch.Draw(pixelTexture, new Rectangle(b.X - Game1.viewport.X, b.Y - Game1.viewport.Y, b.Width, b.Height).ToUIScale(),
                    Color.White);
            }
            else
            {
                int locationWidth = gameLocation.Map.Layers[0].LayerWidth * Game1.tileSize;
                int locationHeight = gameLocation.Map.Layers[0].LayerHeight * Game1.tileSize;

                Rectangle[] rectangles = new Rectangle[4]
                {
                    new Rectangle(0 - Game1.viewport.X, 0 - Game1.viewport.Y, b.X, locationHeight).ToUIScale(),
                    new Rectangle(b.Right - Game1.viewport.X, 0 - Game1.viewport.Y, locationWidth - b.Right, locationHeight).ToUIScale(),
                    new Rectangle(b.X - Game1.viewport.X, 0 - Game1.viewport.Y, b.Width, b.Y).ToUIScale(),
                    new Rectangle(b.X - Game1.viewport.X, b.Y + b.Height - Game1.viewport.Y, b.Width, locationHeight - (b.Y + b.Height)).ToUIScale(),
                };

                foreach (Rectangle rectangle in rectangles)
                    spriteBatch.Draw(pixelTexture, rectangle, Color.White);
            }
        }

        private static Rectangle GetStormBounds(Direction direction, double amount, GameLocation gameLocation, Phase.TwoPointRectangle closeInRectangle)
        {
            if (amount < 0 || amount > 1)
                throw new ArgumentException("Must be between 0 and 1", nameof(amount));

            int locationWidth = gameLocation.Map.Layers[0].LayerWidth * Game1.tileSize;
            int locationHeight = gameLocation.Map.Layers[0].LayerHeight * Game1.tileSize;

            int x, y, w, h;
            x = y = w = h = 0;

            switch (direction)
            {
                case Direction.LeftToRight:
                    x = 0;
                    y = 0;
                    w = (int)(amount * locationWidth);
                    h = locationHeight;
                    break;
                case Direction.RightToLeft:
                    x = (int)((1 - amount) * locationWidth);
                    y = 0;
                    w = locationWidth; h = locationHeight;
                    break;
                case Direction.UpToDown:
                    x = 0;
                    y = 0;
                    w = locationWidth;
                    h = (int)(amount * locationHeight);
                    break;
                case Direction.DownToUp:
                    x = 0;
                    y = (int)((1 - amount) * locationHeight);
                    w = locationWidth;
                    h = locationHeight;
                    break;
                case Direction.CloseIn:
                    x = (int)(amount * closeInRectangle.X1);
                    y = (int)(amount * closeInRectangle.Y1);
                    w = (int)(locationWidth - amount * (locationWidth - closeInRectangle.X2)) - x;
                    h = (int)(locationHeight - amount * (locationHeight - closeInRectangle.Y2)) - y;
                    break;
                default:
                    break;
            }

            return new Rectangle(x, y, w, h);
        }

        public static bool LocationHasAPhase(GameLocation location)
        {
            foreach (Phase phase in Phases)
            {
                foreach (Phase.Location phaseLocation in phase.Locations)
                {
                    if (phaseLocation.LocationName == location.Name)
                        return true;
                }
            }

            return false;
        }

        public static void QuarterSecUpdate(List<Farmer> alivePlayers)
        {
            if (!Game1.IsServer)
                return;

            Round round = ModEntry.BRGame.GetActiveRound();

            //var (currentPhase, currentPhaseAmount) = GetCurrentPhase();
            var tp = GetCurrentPhase();
            var currentPhase = tp.Item1;
            var currentPhaseAmount = tp.Item2;

            //var amounts = new Dictionary<GameLocation, (double amount, Direction direction, Phase.TwoPointRectangle closeInRectangle)>();
            var amounts = new Dictionary<GameLocation, Tuple<double, Direction, Phase.TwoPointRectangle>>();
            foreach (GameLocation location in Game1.locations)
            {
                if (location != null && !amounts.ContainsKey(location))
                    amounts.Add(location, GetStormAmountInLocation(currentPhase, currentPhaseAmount, location));
            }

            foreach (var pair in amounts)
            {
                GameLocation gameLocation = pair.Key;
                //var (amount, direction, closeInRectangle) = pair.Value;
                var t = pair.Value;
                double amount = t.Item1;
                Direction direction = t.Item2;
                Phase.TwoPointRectangle closeInRectangle = t.Item3;

                Rectangle bounds = GetStormBounds(direction, amount, gameLocation, closeInRectangle);

                if (Game.AllWarps.ContainsKey(gameLocation))
                {
                    foreach (DoorOrWarp warp in Game.AllWarps[gameLocation])
                    {
                        bool contains = bounds.Contains(warp);
                        contains = contains && (closeInRectangle == null) || (!contains) && (closeInRectangle != null);
                        if (contains && !TimeLocationWasReached.ContainsKey(warp.TargetLocation) && !LocationHasAPhase(warp.TargetLocation))
                            TimeLocationWasReached[warp.TargetLocation] = DateTime.Now;
                    }
                }

                foreach (Farmer farmer in gameLocation.farmers.Where(x => alivePlayers.Contains(x)))
                {
                    bool contains = bounds.Contains(Utility.Vector2ToPoint(farmer.Position));
                    if (contains && (closeInRectangle == null) || (!contains) && (closeInRectangle != null))
                    {
                        int damage = ModEntry.Config.StormDamagePerSecond;
                        FarmerUtils.TakeDamage(farmer, DamageSource.STORM, damage);
                    }
                }

                foreach (Monster monster in gameLocation.characters.Where(c => c is Monster))
                {
                    bool contains = bounds.Contains(Utility.Vector2ToPoint(monster.Position));
                    if (contains && (closeInRectangle == null) || (!contains) && (closeInRectangle != null))
                    {
                        int damage = ModEntry.Config.StormDamagePerSecond * 100;//Effectively 20 damage per 1.2 sec because invincibility lasts 1.2 sec
                        monster.takeDamage(damage, 0, 0, false, 0.0, "hitEnemy");
                    }
                }
            }

            SendReachedLocationData();

            //When the phase has closed in, two players could not kill eachother and the game would last indefinitely.
            //When time > phasetime * 1.3, damage them anyway
            if (DateTime.Now - startTime >= TimeSpan.FromTicks(totalLengthOfPhases.Ticks * 2))
            {
                int damage = 10;

                foreach (Farmer farmer in round.AlivePlayers.ToList())
                    FarmerUtils.TakeDamage(farmer, DamageSource.STORM, damage);
            }
        }
    }

    public class StormDataModel
    {
        public static List<Phase>[] IslandPhases { get; set; } =
        {
            // Close on dig site
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("IslandSouthEastCave", Storm.Direction.RightToLeft),
                    Tuple.Create("IslandShrine", Storm.Direction.RightToLeft)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandSouthEast", Storm.Direction.RightToLeft),
                    Tuple.Create("IslandEast", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(0, 2960, 0, 2960)),
                    Tuple.Create("IslandWest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(6720, 2575, 6720, 2575)),
                    Tuple.Create("Caldera", Storm.Direction.UpToDown)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandSouth", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1095, 10, 1095, 10)),
                    Tuple.Create("VolcanoDungeon0", Storm.Direction.UpToDown)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandNorth", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(970, 2115, 1575, 2640))
                }, duration: new TimeSpan(0, 0, seconds: 60))
            },

            // Close on southeast
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("Caldera", Storm.Direction.UpToDown)
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandShrine", Storm.Direction.RightToLeft),
                    Tuple.Create("VolcanoDungeon0", Storm.Direction.UpToDown),
                }),

                new Phase(new List<object>()
                {

                    Tuple.Create("IslandWest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(6720, 2575, 6720, 2575)),
                    Tuple.Create("IslandEast", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(0, 2960, 0, 2960)),
                    Tuple.Create("IslandNorth", Storm.Direction.UpToDown),
                }, delay: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandSouthEastCave", Storm.Direction.RightToLeft),
                    Tuple.Create("IslandSouth", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2700, 1875, 2700, 1875))
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandSouthEast", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(815, 1065, 1600, 1675)),
                })
            },

            // Close on resort
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("Caldera", Storm.Direction.UpToDown)
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandShrine", Storm.Direction.RightToLeft),
                    Tuple.Create("IslandSouthEastCave", Storm.Direction.RightToLeft),
                    Tuple.Create("VolcanoDungeon0", Storm.Direction.UpToDown)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandEast", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(0, 2960, 0, 2960)),
                    Tuple.Create("IslandSouthEast", Storm.Direction.RightToLeft),
                    Tuple.Create("IslandNorth", Storm.Direction.UpToDown),
                    Tuple.Create("IslandWest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(6720, 2575, 6720, 2575))
                }, delay: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandSouth", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(775, 1540, 1640, 2030))
                })
            },

            // Close on IslandWest left
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("Caldera", Storm.Direction.UpToDown)
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandShrine", Storm.Direction.RightToLeft),
                    Tuple.Create("IslandSouthEastCave", Storm.Direction.RightToLeft),
                    Tuple.Create("VolcanoDungeon0", Storm.Direction.UpToDown)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandEast", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(0, 2960, 0, 2960)),
                    Tuple.Create("IslandSouthEast", Storm.Direction.RightToLeft),
                    Tuple.Create("IslandNorth", Storm.Direction.UpToDown)
                }, delay: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandSouth", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(0, 785, 0, 785))
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandWest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2150, 1550, 2730, 2060))
                })
            },

            // Close on IslandWest right
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("Caldera", Storm.Direction.UpToDown)
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandShrine", Storm.Direction.RightToLeft),
                    Tuple.Create("IslandSouthEastCave", Storm.Direction.RightToLeft),
                    Tuple.Create("VolcanoDungeon0", Storm.Direction.UpToDown)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandEast", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(0, 2960, 0, 2960)),
                    Tuple.Create("IslandSouthEast", Storm.Direction.RightToLeft),
                    Tuple.Create("IslandNorth", Storm.Direction.UpToDown)
                }, delay: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandSouth", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(0, 785, 0, 785))
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandWest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(5070, 3220, 5700, 3700))
                })
            },

            // Close on volcano floor 0
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("IslandSouthEastCave", Storm.Direction.RightToLeft),
                    Tuple.Create("IslandShrine", Storm.Direction.RightToLeft)
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandWest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(6720, 2575, 6720, 2575)),
                    Tuple.Create("IslandEast", Storm.Direction.UpToDown),
                    Tuple.Create("IslandSouthEast", Storm.Direction.RightToLeft)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandSouth", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1135, 0, 1135, 0))
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("IslandNorth", Storm.Direction.DownToUp)
                }, delay: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>()
                {
                    Tuple.Create("VolcanoDungeon0", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1600, 2420, 2330, 3220))
                })
            }
        };

        public static List<Phase>[] Phases { get; set; } =
        {
            // Close on south forest
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3050, 1750, 3050, 1800)),
                    Tuple.Create("Railroad", Storm.Direction.UpToDown)
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("BusStop", Storm.Direction.UpToDown),
                    Tuple.Create("Backwoods", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(900, 2350, 900, 2250)),
                    Tuple.Create("Mountain", Storm.Direction.UpToDown),
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Farm", Storm.Direction.UpToDown),
                    Tuple.Create("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(0, 5700, 0, 5800)),
                    Tuple.Create("Woods", Storm.Direction.LeftToRight)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(4720, 4650, 5350, 5090))
                })
            },

            // Close on Robin's
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("Woods", Storm.Direction.LeftToRight)
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(7650, 1615, 7650, 1615))
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3150, 1750, 3150, 1800)),
                    Tuple.Create("Farm", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2560, 0, 2560, 0)),
                    Tuple.Create("BusStop", Storm.Direction.LeftToRight),
                    Tuple.Create("Beach", Storm.Direction.DownToUp)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Railroad", Storm.Direction.UpToDown),
                    Tuple.Create("Backwoods", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3250, 910, 3250, 910)),
                    Tuple.Create("Town", Storm.Direction.DownToUp),
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Mountain", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(400, 1620, 1190, 2160))
                }, duration: new TimeSpan(0, 0, seconds: 60))
            },

            // Close on mine
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("Woods", Storm.Direction.LeftToRight)
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(7650, 1615, 7650, 1615))
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3150, 1750, 3150, 1800)),
                    Tuple.Create("Farm", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2560, 0, 2560, 0)),
                    Tuple.Create("BusStop", Storm.Direction.LeftToRight),
                    Tuple.Create("Beach", Storm.Direction.DownToUp)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Railroad", Storm.Direction.UpToDown),
                    Tuple.Create("Backwoods", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3250, 910, 3250, 910)),
                    Tuple.Create("Town", Storm.Direction.DownToUp),
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Mountain", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3460, 330, 3460, 330))
                }, duration: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>()
                {
                    Tuple.Create("Mine", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(740, 210, 1670, 820))
                })
            },

            // Close on beach
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3150, 1750, 3150, 1800))
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("Backwoods", Storm.Direction.LeftToRight),
                    Tuple.Create("Railroad", Storm.Direction.UpToDown)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Woods", Storm.Direction.LeftToRight),
                    Tuple.Create("BusStop", Storm.Direction.LeftToRight),
                    Tuple.Create("Farm", Storm.Direction.UpToDown),
                    Tuple.Create("Mountain", Storm.Direction.UpToDown)
                }, duration: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>()
                {
                    Tuple.Create("Town", Storm.Direction.UpToDown),
                    Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(7650, 2270, 7650, 2270))
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Beach", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2600, 710, 3500, 1230))
                })
            },

            // Close on railroad
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("Woods", Storm.Direction.LeftToRight),
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(7600, 1610, 7600, 1610))
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Beach", Storm.Direction.DownToUp),
                    Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3050, 1750, 3050, 1800)),
                    Tuple.Create("BusStop", Storm.Direction.LeftToRight),
                    Tuple.Create("Farm", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2560, 0, 2560, 0))
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Town", Storm.Direction.DownToUp),
                    Tuple.Create("Backwoods", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3100, 910, 3100, 910)),
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Mountain", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(620, 0, 620, 0))
                }, duration: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>()
                {
                    Tuple.Create("Railroad", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1920, 2480, 3400, 2920))
                })
            },

            // Close on Marnie's ranch
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3050, 1750, 3050, 1800)),
                    Tuple.Create("Railroad", Storm.Direction.UpToDown)
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("BusStop", Storm.Direction.UpToDown),
                    Tuple.Create("Backwoods", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(900, 2350, 900, 2250)),
                    Tuple.Create("Mountain", Storm.Direction.UpToDown),
                }, duration: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>()
                {
                    Tuple.Create("Farm", Storm.Direction.UpToDown),
                    Tuple.Create("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(0, 5700, 0, 5800)),
                    Tuple.Create("Woods", Storm.Direction.LeftToRight)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(4000, 760, 4980, 1430))
                })
            },

            // Close on CC
            new List<Phase>()
            {
                new Phase(new List<object>()
                {
                    Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3050, 1750, 3050, 1800)),
                    Tuple.Create("Woods", Storm.Direction.LeftToRight)
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>()
                {
                    Tuple.Create("Backwoods", Storm.Direction.LeftToRight),
                    Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(7650, 1610, 7650, 1650))
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Railroad", Storm.Direction.UpToDown),
                    Tuple.Create("Farm", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(5100, 1100, 5100, 1200))
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Beach", Storm.Direction.DownToUp),
                    Tuple.Create("BusStop", Storm.Direction.LeftToRight),
                    Tuple.Create("Mountain", Storm.Direction.UpToDown)
                }),

                new Phase(new List<object>()
                {
                    Tuple.Create("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2580, 1500, 3570, 2450))
                })
            },

            //Close on desert
            new List<Phase>()
            {
                new Phase(new List<object>(){
                    Tuple.Create("Railroad", Storm.Direction.UpToDown),
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>(){
                    Tuple.Create("Woods", Storm.Direction.LeftToRight),
                    Tuple.Create("Beach", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2368, -100, 2560, -100)),
                    Tuple.Create("Mountain", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(896, 2750, 1088, 2750)),
                }, duration: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>(){
                    Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(4288, -100, 4608, -100)),
                    Tuple.Create("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(-100, 3392, -100, 3520))
                }),

                new Phase(new List<object>(){
                    Tuple.Create("Farm", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(5150, -100, 5150, 1250))
                }),

                new Phase(new List<object>(){
                    Tuple.Create("BusStop", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(-100, 384, -100, 576))
                }),

                new Phase(new List<object>(){
                    Tuple.Create("Backwoods", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1472, 1792, 1472, 2048))
                }),

                new Phase(new List<object>(){
                    Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2176, 512, 2624, 896))
                }),
            },

            // Close on town (updated)
            new List<Phase>()
            {
                new Phase(new List<object>(){
                    Tuple.Create("Railroad", Storm.Direction.UpToDown),
                    Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3100, 1540, 3100, 1850)),
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>(){
                    Tuple.Create("Woods", Storm.Direction.LeftToRight),
                    Tuple.Create("Backwoods", Storm.Direction.LeftToRight),
                    Tuple.Create("Farm", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(5150, -100, 5150, 1250))
                }),

                new Phase(new List<object>(){
                    Tuple.Create("BusStop", Storm.Direction.LeftToRight),
                    Tuple.Create("Mountain", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(896, 2750, 1088, 2750)),
                    Tuple.Create("Beach", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2368, -100, 2560, -100)),
                    Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(7700, 1450, 7700, 1820)),
                }, duration: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>(){
                    Tuple.Create("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1400, 3990, 2380, 4754)),
                })
            },

            // Close on secret forest
            new List<Phase>()
            {
                new Phase(new List<object>(){
                    Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3100, 1540, 3100, 1850)),
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>(){
                    Tuple.Create("Railroad", Storm.Direction.UpToDown),
                    Tuple.Create("Backwoods", Storm.Direction.UpToDown),
                    Tuple.Create("Tunnel", Storm.Direction.LeftToRight),
                }),

                new Phase(new List<object>(){
                    Tuple.Create("BusStop", Storm.Direction.RightToLeft),
                    Tuple.Create("Beach", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2368, -100, 2560, -100)),
                    Tuple.Create("Mountain", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(896, 2750, 1088, 2750))
                }, duration: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>(){
                    Tuple.Create("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(-100, 5700, -100, 5950)),
                    Tuple.Create("Farm", Storm.Direction.UpToDown)
                }),

                new Phase(new List<object>(){
                    Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(-100, 320, -100, 520))
                }),

                new Phase(new List<object>(){
                    Tuple.Create("Woods", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(288, 384, 934, 864))
                })
            },

            // Close on bug lair
            new List<Phase>()
            {
                new Phase(new List<object>(){
                    Tuple.Create("Desert", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(3100, 1540, 3100, 1850)),
                    Tuple.Create("Tunnel", Storm.Direction.LeftToRight)
                }, delay: new TimeSpan(0, 0, seconds: 30)),

                new Phase(new List<object>(){
                    Tuple.Create("Railroad", Storm.Direction.UpToDown),
                    Tuple.Create("BusStop", Storm.Direction.UpToDown)
                }),

                new Phase(new List<object>(){
                    Tuple.Create("Farm", Storm.Direction.UpToDown),
                    Tuple.Create("Woods", Storm.Direction.LeftToRight),
                    Tuple.Create("Beach", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2368, -100, 2560, -100)),
                    Tuple.Create("Mountain", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(896, 2750, 1088, 2750)),
                    Tuple.Create("Backwoods", Storm.Direction.UpToDown)
                }, duration: new TimeSpan(0, 0, seconds: 60)),

                new Phase(new List<object>(){
                    Tuple.Create("Forest", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(6047, 6244, 6047, 6800)),
                    Tuple.Create("Town", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(2212, 6144, 2212, 6144)),
                }),

                new Phase(new List<object>(){
                    Tuple.Create("Sewer", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(225, 1044, 225, 1300))
                }),

                new Phase(new List<object>(){
                    Tuple.Create("BugLand", Storm.Direction.CloseIn, new Phase.TwoPointRectangle(1536, 200, 2464, 712))
                })
            }
        };
    }
}