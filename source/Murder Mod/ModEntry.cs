/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ArenKDesai/Murder-Mod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using xTile.Dimensions;

namespace MurderMod
{
    internal sealed class ModEntry : Mod
    {
        private GameTime killTime = new GameTime();
        private bool justKilled = false;
        private NPC justDied = null;
        private Microsoft.Xna.Framework.Rectangle playerRectangle =
            new Microsoft.Xna.Framework.Rectangle();

        /**
        * Entry method, allows for the mod to be loaded into the game.
        **/
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
        }

        /**
        * Method is called whenever an NPC needs to be killed. It changes their sprite to the deadnpc.xnb sprite and
        * fixes their schedule so they appear dead.
        **/

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            if (Game1.dayOfMonth == 28)
            {
                string deadStatusFilePath = Path.Combine(this.Helper.DirectoryPath, "dead.txt");
                File.WriteAllText(deadStatusFilePath, "");
            }
        }

        private void KillNPC(NPC npc)
        {
            // Could make it so dead NPCs don't show up on the map, but adding a corpse sprite is more fun
            //npc.HideShadow = true;
            //npc.IsInvisible = true;
            //npc.Position = new Microsoft.Xna.Framework.Vector2(-1000, -1000);
            try
            {
                npc.faceDirection(2);
                npc.Sprite.LoadTexture(Path.Combine(this.Helper.DirectoryPath, "dead_npc"));
                npc.Sprite.SourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 32);
                npc.Sprite.UpdateSourceRect();
                justDied = null;
                string deadStatusFilePath = Path.Combine(this.Helper.DirectoryPath, "dead.txt");
                string deadStatusTxt = File.ReadAllText(deadStatusFilePath);
                List<string> deadNpcNames = new List<string>(deadStatusTxt.Split(Environment.NewLine));
                if (!deadNpcNames.Contains(npc.name))
                    File.WriteAllText(
                        deadStatusFilePath,
                        deadStatusTxt + Environment.NewLine + npc.name
                    );
                npc.stopWithoutChangingFrame();
                justKilled = false;
            }
            catch {}
        }

        /**
        * Method is called on every day, checks for dead NPCs and kills them if they are considered dead.
        **/
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            string deadStatusFilePath = Path.Combine(this.Helper.DirectoryPath, "dead.txt");

            string deadStatusTxt = File.ReadAllText(deadStatusFilePath);
            List<string> deadNpcNames = new List<string>(deadStatusTxt.Split(Environment.NewLine));

            foreach (string npc in deadNpcNames)
            {


                var character = Game1.getCharacterFromName(npc);
                if (character == null)
                    continue;

                if (deadNpcNames.Contains(character.Name) && character is NPC)
                {
                    character.Sprite.LoadTexture(Path.Combine(this.Helper.DirectoryPath, "dead_npc"));
                    character.Sprite.SourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 32);
                    character.Sprite.UpdateSourceRect();
                    character.Halt();
                    character.farmerPassesThrough = true;
                    character.CurrentDialogue.Clear();
                    character.controller = null;
                    character.isEmoting = false;

                    Dictionary<int, SchedulePathDescription> schedule = new Dictionary<
                        int,
                        SchedulePathDescription
                    >
                {
                    {
                        0,
                        new SchedulePathDescription(
                            new Stack<Point>(),
                            character.DefaultFacingDirection,
                            "lin",
                            character.Name + " appears cold and unmoving..."
                        )
                    },
                    {
                        1,
                        new SchedulePathDescription(
                            new Stack<Point>(),
                            character.DefaultFacingDirection,
                            "lin",
                            character.Name + " appears cold and unmoving..."
                        )
                    },
                    {
                        2,
                        new SchedulePathDescription(
                            new Stack<Point>(),
                            character.DefaultFacingDirection,
                            "lin",
                            character.Name + " appears cold and unmoving..."
                        )
                    },
                    {
                        3,
                        new SchedulePathDescription(
                            new Stack<Point>(),
                            character.DefaultFacingDirection,
                            "lin",
                            character.Name + " appears cold and unmoving..."
                        )
                    }
                };
                    (character as NPC).Schedule = schedule;
                    character.faceDirection(2);


                }
            }
        }

        /**
        * Method is called every tick, checks for a condition to kill an NPC and kills them if the condition is met.
        **/
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.eventUp)
                return;

            // Check your arbitrary condition here
            bool conditionMet = CheckCondition();

            if (conditionMet)
            {
                // Select an NPC to die (replace "NPCName" with the actual name)
                NPC npc = null;
                playerRectangle = Game1.player.GetBoundingBox();
                foreach (NPC character in Game1.currentLocation.characters)
                {
                    Microsoft.Xna.Framework.Rectangle npcBoundingBox = character.GetBoundingBox();
                    npcBoundingBox.Inflate(32, 64);
                    if (playerRectangle.Intersects(npcBoundingBox))
                    {
                        npc = character;
                        break;
                    }
                    npcBoundingBox.Inflate(-32, -64);
                }
                if (npc == null || npc is Pet || npc is Horse)
                {
                }
                else
                {
                    KillingNPC(npc);
                }
            }

            if (justKilled)
            {
                if (
                    Game1.currentGameTime.TotalGameTime.TotalMilliseconds
                        - killTime.TotalGameTime.TotalMilliseconds
                    > 2000.0
                )
                {
                    KillNPC(justDied);
                }
            }
        }

        private bool CheckCondition()
        {
            try
            {
                if (Game1.player.UsingTool)
                {
                }
                if (
                    Game1.player.UsingTool
                    && (
                        Game1.player.CurrentTool
                            .getCategoryName()
                            .Substring(Game1.player.CurrentTool.getCategoryName().Length - 6)
                            == "Dagger"
                        || Game1.player.CurrentTool
                            .getCategoryName()
                            .Substring(Game1.player.CurrentTool.getCategoryName().Length - 4)
                            == "Club"
                        || Game1.player.CurrentTool
                            .getCategoryName()
                            .Substring(Game1.player.CurrentTool.getCategoryName().Length - 5)
                            == "Sword"
                    )
                )
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                this.Monitor.Log(e.ToString(), LogLevel.Debug);
                return false;
            }
            return false;
        }

        private void KillingNPC(NPC npc)
        {
            string deadStatusFilePath = Path.Combine(this.Helper.DirectoryPath, "dead.txt");
            string deadStatusTxt = File.ReadAllText(deadStatusFilePath);
            List<string> deadNpcNames = new List<string>(deadStatusTxt.Split(Environment.NewLine));
            if (!deadNpcNames.Contains(npc.getName()))
            {
                justKilled = true;
                killTime = Game1.currentGameTime.DeepClone();
                Game1.playSound("crit");
                npc.Halt();
                npc.farmerPassesThrough = true;
                npc.CurrentDialogue.Clear();
                npc.faceDirection(2);
                npc.controller = null;
                npc.isEmoting = false;

                Dictionary<int, SchedulePathDescription> schedule = new Dictionary<
                    int,
                    SchedulePathDescription
                >
                {
                    {
                        0,
                        new SchedulePathDescription(
                            new Stack<Point>(),
                            npc.DefaultFacingDirection,
                            "lin",
                            npc.Name + " appears cold and unmoving..."
                        )
                    },
                    {
                        1,
                        new SchedulePathDescription(
                            new Stack<Point>(),
                            npc.DefaultFacingDirection,
                            "lin",
                            npc.Name + " appears cold and unmoving..."
                        )
                    },
                    {
                        2,
                        new SchedulePathDescription(
                            new Stack<Point>(),
                            npc.DefaultFacingDirection,
                            "lin",
                            npc.Name + " appears cold and unmoving..."
                        )
                    },
                    {
                        3,
                        new SchedulePathDescription(
                            new Stack<Point>(),
                            npc.DefaultFacingDirection,
                            "lin",
                            npc.Name + " appears cold and unmoving..."
                        )
                    }
                };
                (npc as NPC).Schedule = schedule;

                justDied = npc;
                npc.faceDirection(2);
                npc.doEmote(16);
            }
        }
    }
}
