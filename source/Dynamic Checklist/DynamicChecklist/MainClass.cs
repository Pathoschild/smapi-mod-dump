namespace DynamicChecklist
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Graph.Graphs;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using ObjectLists;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;
    using StardewValley.Menus;

    public class MainClass : Mod
    {
        private Keys openMenuKey = Keys.NumPad1;
        private ModConfig config;
        private List<ObjectList> objectLists = new List<ObjectList>();
        private CompleteGraph graph;
        private OpenChecklistButton checklistButton;
        private bool doneLoading;

        public override void Entry(IModHelper helper)
        {
            this.config = this.Helper.ReadConfig<ModConfig>();
            helper.WriteConfig(this.config);
            IModEvents events = helper.Events;
            events.Display.MenuChanged += this.Display_MenuChanged;
            events.Input.ButtonPressed += this.Input_ButtonPressed;
            events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
            events.GameLoop.Saved += this.GameLoop_Saved;
            events.GameLoop.DayStarted += this.GameLoop_DayStarted;
            events.Display.RenderingHud += this.Display_RenderingHud;
            events.GameLoop.OneSecondUpdateTicked += this.UpdatePaths;
            events.Player.Warped += this.UpdatePaths;

            OverlayTextures.LoadTextures(this.Helper.DirectoryPath);
            try
            {
                this.openMenuKey = (Keys)Enum.Parse(typeof(Keys), this.config.OpenMenuKey);
            }
            catch
            {
                // use default value
            }
        }

        private bool MenuAllowed()
        {
            if ((Game1.dayOfMonth <= 0 ? 0 : (Game1.player.CanMove ? 1 : 0)) != 0 && !Game1.dialogueUp && (!Game1.eventUp || (Game1.isFestival() && Game1.CurrentEvent.festivalTimer <= 0)) && Game1.currentMinigame == null && Game1.activeClickableMenu == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GameLoop_Saved(object sender, EventArgs e)
        {
            this.Helper.WriteConfig(this.config);
        }

        private void Display_RenderingHud(object sender, EventArgs e)
        {
            if (!this.doneLoading || Game1.currentLocation == null || Game1.gameMode == 11 || Game1.currentMinigame != null || Game1.showingEndOfNightStuff || Game1.gameMode == 6 || Game1.gameMode == 0 || Game1.menuUp || Game1.activeClickableMenu != null)
            {
                return;
            }

            foreach (ObjectList ol in this.objectLists)
            {
                ol.BeforeDraw();
                ol.Draw(Game1.spriteBatch);
            }

            this.checklistButton.draw(Game1.spriteBatch);
        }

        private void UpdatePaths(object sender, EventArgs e)
        {
            if (!this.doneLoading || Game1.currentLocation == null || Game1.gameMode == 11 || Game1.currentMinigame != null || Game1.showingEndOfNightStuff || Game1.gameMode == 6 || Game1.gameMode == 0 || Game1.menuUp || Game1.activeClickableMenu != null)
            {
                return;
            }

            if (this.graph.LocationInGraph(Game1.currentLocation))
            {
                this.graph.SetPlayerPosition(Game1.currentLocation, Game1.player.Position);
                this.graph.Calculate(Game1.currentLocation);
                foreach (ObjectList ol in this.objectLists)
                {
                    if (ol.OverlayActive)
                    {
                        ol.UpdatePath();
                    }
                }
            }
            else
            {
                foreach (ObjectList ol in this.objectLists)
                {
                    ol.ClearPath();
                }
            }
        }

        private void GameLoop_DayStarted(object sender, EventArgs e)
        {
            foreach (ObjectList ol in this.objectLists)
            {
                ol.OnNewDay();
            }
        }

        private void ShowTaskDoneMessage(object sender, EventArgs e)
        {
            var s = (ObjectList)sender;
            Game1.showGlobalMessage(s.TaskDoneMessage);
        }

        private void OnOverlayActivated(object sender, EventArgs e)
        {
            this.graph.Calculate(Game1.currentLocation);
            var activatedObjectList = (ObjectList)sender;
            activatedObjectList.UpdatePath();
            if (!this.config.AllowMultipleOverlays)
            {
                foreach (ObjectList ol in this.objectLists)
                {
                    if (ol != sender)
                    {
                        ol.OverlayActive = false;
                    }
                }
            }
        }

        private void InitializeObjectLists()
        {
            var listNames = (TaskName[])Enum.GetValues(typeof(TaskName));
            foreach (var listName in listNames)
            {
                switch (listName)
                {
                    case TaskName.Milk:
                        this.objectLists.Add(new AnimalList(this.config, AnimalList.Action.Milk));
                        break;
                    case TaskName.Pet:
                        this.objectLists.Add(new AnimalList(this.config, AnimalList.Action.Pet));
                        break;
                    case TaskName.Shear:
                        this.objectLists.Add(new AnimalList(this.config, AnimalList.Action.Shear));
                        break;
                    case TaskName.CrabPot:
                        this.objectLists.Add(new CrabPotList(this.config));
                        break;
                    case TaskName.Hay:
                        this.objectLists.Add(new HayList(this.config));
                        break;
                    case TaskName.Egg:
                        this.objectLists.Add(new EggList(this.config));
                        break;
                    case TaskName.Water:
                        this.objectLists.Add(new ObjectLists.CropList(this.config, ObjectLists.CropList.Action.Water));
                        break;
                    case TaskName.Harvest:
                        this.objectLists.Add(new ObjectLists.CropList(this.config, ObjectLists.CropList.Action.Harvest));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            foreach (ObjectList ol in this.objectLists)
            {
                ol.OnNewDay();
            }

            ObjectList.Graph = this.graph;

            foreach (ObjectList o in this.objectLists)
            {
                o.TaskFinished += new EventHandler(this.ShowTaskDoneMessage);
                o.OverlayActivated += new EventHandler(this.OnOverlayActivated);
            }
        }

        private Texture2D LoadTexture(string texName)
        {
            var textureStream = new FileStream(Path.Combine(this.Helper.DirectoryPath, "Resources", texName), FileMode.Open);
            var t = Texture2D.FromStream(Game1.graphics.GraphicsDevice, textureStream);
            return t;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.ToString() == this.config.OpenMenuKey)
            {
                if (Game1.activeClickableMenu is ChecklistMenu)
                {
                    Game1.activeClickableMenu = null;
                    Game1.playSound("bigDeSelect");
                }
                else
                {
                    if (this.MenuAllowed())
                    {
                        ChecklistMenu.Open(this.config);
                    }
                }
            }
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!(e.NewMenu is GameMenu))
            {
                return;
            }

            var gameMenu = e.NewMenu;
        }

        private void GameLoop_SaveLoaded(object sender, EventArgs e)
        {
            this.graph = new CompleteGraph(Game1.locations);
            this.graph.Populate();
            this.InitializeObjectLists();
            ChecklistMenu.ObjectLists = this.objectLists;
            Func<int> crt = this.CountRemainingTasks;
            this.checklistButton = new OpenChecklistButton(() => ChecklistMenu.Open(this.config), crt, this.config, this.Helper.Events);
            Game1.onScreenMenus.Insert(0, this.checklistButton); // So that click is registered with priority
            this.doneLoading = true;
        }

        private int CountRemainingTasks()
        {
            return this.objectLists.FindAll(x => x.TaskLeft).Count;
        }
    }
}
