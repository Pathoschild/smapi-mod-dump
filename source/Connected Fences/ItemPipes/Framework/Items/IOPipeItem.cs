/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Objects;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Factories;
using ItemPipes.Framework.Items.Objects;
using ItemPipes.Framework.Items.Tools;

using System.Xml.Serialization;

namespace ItemPipes.Framework.Items
{
    public abstract class IOPipeItem : PipeItem
    {
        [XmlIgnore]
        public Texture2D SignalTexture { get; set; }
        [XmlIgnore]
        public Texture2D OnSignal { get; set; }
        [XmlIgnore]
        public Texture2D OffSignal { get; set; }
        [XmlIgnore]
        public Texture2D UnconnectedSignal { get; set; }
        [XmlIgnore]
        public Texture2D NoChestSignal { get; set; }

        public IOPipeItem() : base()
        {
            LoadSignals();
            DrawGuide = new Dictionary<string, int>();
            PopulateDrawGuide();
        }
        public IOPipeItem(Vector2 position) : base(position)
        {
            LoadSignals();
            DrawGuide = new Dictionary<string, int>();
            PopulateDrawGuide();
        }

        public void LoadSignals()
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            OnSignal = DataAccess.Sprites["signal_on"];
            OffSignal = DataAccess.Sprites["signal_off"];
            UnconnectedSignal = DataAccess.Sprites["signal_unconnected"];
            NoChestSignal = DataAccess.Sprites["signal_nochest"];
            SignalTexture = UnconnectedSignal;
        }

        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t is Pickaxe)
            {
                var who = t.getLastFarmerToUse();
                this.performRemoveAction(this.TileLocation, location);
                Debris deb = new Debris(this.getOne(), who.GetToolLocation(), new Vector2(who.GetBoundingBox().Center.X, who.GetBoundingBox().Center.Y));
                Game1.currentLocation.debris.Add(deb);
                Game1.currentLocation.objects.Remove(this.TileLocation);
                return false;
            }
            if (t is WrenchItem)
            {
                Printer.Info("WRENCH");
                ChangeSignal();
                return false;
            }
            return false;
        }
        public override Item getOne()
        {
            return ItemFactory.CreateItem(IDName);
        }

        public override bool clicked(Farmer who)
        {
            if(who.CurrentTool == null)
            {
                DataAccess DataAccess = DataAccess.GetDataAccess();
                if (DataAccess.IOPipeNames.Contains(this.Name))
                {
                    List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
                    Node node = nodes.Find(n => n.Position.Equals(this.TileLocation));
                    if(node is IOPipeNode)
                    {
                        IOPipeNode pipe = (IOPipeNode)node;
                        if (pipe != null)
                        {
                            Printer.Info($"{Name} is {pipe.Signal}");
                        }
                    }
                }
            }
            return false;
        }

        public void ChangeSignal()
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
            IOPipeNode pipe = (IOPipeNode)nodes.Find(n => n.Position.Equals(this.TileLocation));
            pipe.ChangeSignal();
            UpdateSignal(pipe.Signal);
        }

        public void UpdateSignal(string signal)
        {
            switch (signal)
            {
                case "on":
                    SignalTexture = OnSignal;
                    break;
                case "off":
                    SignalTexture = OffSignal;
                    break;
                case "unconnected":
                    SignalTexture = UnconnectedSignal;
                    break;
                case "nochest":
                    SignalTexture = NoChestSignal;
                    break;
            }
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            base.draw(spriteBatch, x, y);
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if(DataAccess.LocationNodes.ContainsKey(Game1.currentLocation))
            {
                List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
                Node node = nodes.Find(n => n.Position.Equals(TileLocation));
                if (node != null && node is IOPipeNode)
                {
                    IOPipeNode IONode = (IOPipeNode)node;
                    if (IONode.Signal != null && !IONode.PassingItem)
                    {
                        UpdateSignal(IONode.Signal);
                        float transparency = 1f;
                        if (IONode.Passable)
                        {
                            Passable = true;
                            transparency = 0.5f;
                        }
                        else
                        {
                            Passable = false;
                            transparency = 1f;
                        }
                        Rectangle srcRect = new Rectangle(0, 0, 16, 16);
                        spriteBatch.Draw(SignalTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64)), srcRect, Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.002f);
                    }
                }
            }
        }

        public override string GetSpriteKey(GameLocation location)
        {
            bool CN = false;
            bool CS = false;
            bool CW = false;
            bool CE = false;
            string key = "";
            Vector2 position = this.TileLocation;
            position.Y -= 1f;
            if (location.objects.ContainsKey(position) && (location.objects[position] is PipeItem && ((PipeItem)location.objects[position]).countsForDrawing(this)
                || location.objects[position] is PPMItem))
            {
                key += "N";
            }
            else if (location.objects.ContainsKey(position) && location.objects[position] is Chest)
            {
                CN = true;
            }
            position = this.TileLocation;
            position.Y += 1f;
            if (location.objects.ContainsKey(position) && (location.objects[position] is PipeItem && ((PipeItem)location.objects[position]).countsForDrawing(this)
                || location.objects[position] is PPMItem))
            {
                key += "S";
            }
            else if (location.objects.ContainsKey(position) && location.objects[position] is Chest)
            {
                CS = true;
            }
            position = this.TileLocation;
            position.X += 1f;
            if (location.objects.ContainsKey(position) && (location.objects[position] is PipeItem && ((PipeItem)location.objects[position]).countsForDrawing(this)
                || location.objects[position] is PPMItem))
            {
                key += "W";
            }
            else if (location.objects.ContainsKey(position) && location.objects[position] is Chest)
            {
                CW = true;
            }
            position = this.TileLocation;
            position.X -= 1f;
            if (location.objects.ContainsKey(position) && (location.objects[position] is PipeItem && ((PipeItem)location.objects[position]).countsForDrawing(this)
                || location.objects[position] is PPMItem))
            {
                key += "E";
            }
            else if (location.objects.ContainsKey(position) && location.objects[position] is Chest)
            {
                CE = true;
            }
            if (CN || CS || CW || CE)
            {
                key = GetAdjChestsKey(key, CN, CS, CW, CE);
            }
            return key;
        }



        private static string GetAdjChestsKey(string drawSum, bool CN, bool CS, bool CW, bool CE)
        {
            switch (drawSum)
            {
                case "":
                    if (CN) { drawSum = "CN"; }
                    else if (CS) { drawSum = "CS"; }
                    else if (CW) { drawSum = "CW"; }
                    else if (CE) { drawSum = "CE"; }
                    break;
                case "N":
                    if (CS) { drawSum = "N_CS"; }
                    else if (CW) { drawSum = "N_CW"; }
                    else if (CE) { drawSum = "N_CE"; }
                    break;
                case "S":
                    if (CN) { drawSum = "S_CN"; }
                    else if (CW) { drawSum = "S_CW"; }
                    else if (CE) { drawSum = "S_CE"; }
                    break;
                case "W":
                    if (CN) { drawSum = "W_CN"; }
                    else if (CS) { drawSum = "W_CS"; }
                    else if (CE) { drawSum = "W_CE"; }
                    break;
                case "E":
                    if (CN) { drawSum = "E_CN"; }
                    else if (CS) { drawSum = "E_CS"; }
                    else if (CW) { drawSum = "E_CW"; }
                    break;
                case "NS":
                    if (CW) { drawSum = "NS_CW"; }
                    else if (CE) { drawSum = "NS_CE"; }
                    break;
                case "NW":
                    if (CS) { drawSum = "NW_CS"; }
                    else if (CE) { drawSum = "NW_CE"; }
                    break;
                case "NE":
                    if (CS) { drawSum = "NE_CS"; }
                    else if (CW) { drawSum = "NE_CW"; }
                    break;
                case "SW":
                    if (CN) { drawSum = "SW_CN"; }
                    else if (CE) { drawSum = "SW_CE"; }
                    break;
                case "SE":
                    if (CN) { drawSum = "SE_CN"; }
                    else if (CW) { drawSum = "SE_CW"; }
                    break;
                case "WE":
                    if (CN) { drawSum = "WE_CN"; }
                    else if (CS) { drawSum = "WE_CS"; }
                    break;
            }
            return drawSum;
        }

        public override void PopulateDrawGuide()
        {
            DrawGuide.Clear();
            DrawGuide.Add("", 0);
            DrawGuide.Add("N_CS", 1);
            DrawGuide.Add("CS", 1);
            DrawGuide.Add("N", 1);
            DrawGuide.Add("N_CW", 2);
            DrawGuide.Add("N_CE", 3);
            DrawGuide.Add("S_CN", 4);
            DrawGuide.Add("CN", 4);
            DrawGuide.Add("S", 4);
            DrawGuide.Add("S_CW", 5);
            DrawGuide.Add("S_CE", 6);
            DrawGuide.Add("W_CN", 7);
            DrawGuide.Add("W_CS", 8);
            DrawGuide.Add("W_CE", 9);
            DrawGuide.Add("CE", 9);
            DrawGuide.Add("W", 9);
            DrawGuide.Add("E_CN", 10);
            DrawGuide.Add("E_CS", 11);
            DrawGuide.Add("E_CW", 12);
            DrawGuide.Add("CW", 12);
            DrawGuide.Add("E", 12);
            DrawGuide.Add("NS_CW", 13);
            DrawGuide.Add("NS_CE", 14);
            DrawGuide.Add("NS", 14);
            DrawGuide.Add("NW_CS", 15);
            DrawGuide.Add("NW_CE", 16);
            DrawGuide.Add("NW", 16);
            DrawGuide.Add("NE_CS", 17);
            DrawGuide.Add("NE_CW", 18);
            DrawGuide.Add("NE", 18);
            DrawGuide.Add("SW_CN", 19);
            DrawGuide.Add("SW_CE", 20);
            DrawGuide.Add("SW", 20);
            DrawGuide.Add("SE_CN", 21);
            DrawGuide.Add("SE_CW", 22);
            DrawGuide.Add("SE", 22);
            DrawGuide.Add("WE_CN", 23);
            DrawGuide.Add("WE_CS", 24);
            DrawGuide.Add("WE", 24);
            DrawGuide.Add("NSW", 25);
            DrawGuide.Add("NSE", 26);
            DrawGuide.Add("NWE", 27);
            DrawGuide.Add("SWE", 28);
            DrawGuide.Add("NSWE", 29);
        }
    }
}
