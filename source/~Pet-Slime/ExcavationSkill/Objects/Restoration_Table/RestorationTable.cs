/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley.GameData.Machines;
using StardewValley.Objects;
using StardewValley;
using Object = StardewValley.Object;
using Netcode;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

namespace ArchaeologySkill.Objects.Restoration_Table
{

    [XmlType("Mods_moonslime.Archaeology.restoration_table")]
    public class RestorationTable : Object
    {
        public const int defaultDaysToMature = 7;

        [XmlElement("agingRate")]
        public readonly NetFloat agingRate = new NetFloat();

        [XmlElement("daysToMature")]
        public readonly NetFloat daysToMature = new NetFloat();

        public override string TypeDefinitionId => "(BC)";

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddField(agingRate, "agingRate").AddField(daysToMature, "daysToMature");
        }

        public RestorationTable()
        {
        }

        public RestorationTable(Vector2 v)
            : base(v, "moonslime.Archaeology.restoration_table")
        {
        }

        public override bool performToolAction(Tool t)
        {
            if (t != null && t.isHeavyHitter() && !(t is MeleeWeapon))
            {
                if (heldObject.Value != null)
                {
                    Game1.createItemDebris(heldObject.Value, tileLocation.Value * 64f, -1);
                }

                playNearbySoundAll("woodWhack");
                if (heldObject.Value == null)
                {
                    return true;
                }

                heldObject.Value = null;
                readyForHarvest.Value = false;
                minutesUntilReady.Value = -1;


                return false;
            }

            return base.performToolAction(t);
        }

        public static Item OutputCask(Object machine, Item inputItem, bool probe, MachineItemOutput outputData, out int? overrideMinutesUntilReady)
        {
            overrideMinutesUntilReady = null;
            if (!(machine is RestorationTable cask))
            {
                return null;
            }

            if (cask.Quality >= 4)
            {
                return null;
            }

            if (inputItem.Quality >= 4)
            {
                return null;
            }

            float result = 1f;
            if (outputData?.CustomData != null && outputData.CustomData.TryGetValue("AgingMultiplier", out string value) && (!float.TryParse(value, out result) || result <= 0f))
            {
                BirbCore.Attributes.Log.Error("Failed to parse cask aging multiplier '" + value + "' for trigger rule. This must be a positive float value.");
                return null;
            }

            if (result > 0f)
            {
                Object @object = (Object)inputItem.getOne();
                if (!probe)
                {
                    cask.agingRate.Value = result;
                    cask.daysToMature.Value = cask.GetDaysForQuality(@object.Quality);
                    overrideMinutesUntilReady = @object.Quality >= 4 ? 1 : 999999;
                    return @object;
                }

                return @object;
            }

            return null;
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            Vector2 vector = new Vector2(x / 64, y / 64);
            health = 10;
            Location = location;
            TileLocation = vector;
            if (who != null)
            {
                owner.Value = who.UniqueMultiplayerID;
            }
            location.objects.Add(vector, new RestorationTable(vector));
            location.playSound("hammer");
            return true;
        }

        public override bool TryApplyFairyDust(bool probe = false)
        {
            if (heldObject.Value == null)
            {
                return false;
            }

            if (heldObject.Value.Quality == 4)
            {
                return false;
            }

            if (!probe)
            {
                Utility.addSprinklesToLocation(Location, (int)tileLocation.X, (int)tileLocation.Y, 1, 2, 400, 40, Color.White);
                Game1.playSound("yoba");
                daysToMature.Value = GetDaysForQuality(GetNextQuality(heldObject.Value.Quality));
                CheckForMaturity();
            }

            return true;
        }

        public override void DayUpdate()
        {
            base.DayUpdate();
            if (heldObject.Value != null)
            {
                minutesUntilReady.Value = 999999;
                daysToMature.Value -= agingRate.Value;
                CheckForMaturity();
            }
        }

        public float GetDaysForQuality(int quality)
        {
            return quality switch
            {
                4 => 1f,
                2 => 3f,
                1 => 5f,
                _ => 7f,
            };
        }

        public int GetNextQuality(int quality)
        {
            switch (quality)
            {
                case 2:
                case 4:
                    return 4;
                case 1:
                    return 2;
                default:
                    return 1;
            }
        }

        public void CheckForMaturity()
        {
            if (daysToMature.Value <= GetDaysForQuality(GetNextQuality(heldObject.Value.Quality)))
            {
                heldObject.Value.Quality = GetNextQuality(heldObject.Value.Quality);
                if (heldObject.Value.Quality == 4)
                {
                    minutesUntilReady.Value = 1;
                }
            }
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (heldObject.Value == null)
            {
                showNextIndex.Value = false;
            } else
            {
                showNextIndex.Value = true;
            }
            base.draw(spriteBatch, x, y, alpha);
            if (heldObject.Value?.Quality > 0)
            {
                Vector2 vector = MinutesUntilReady > 0 ? Vector2.Zero : Vector2.Zero;
                vector *= 4f;
                Vector2 vector2 = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
                Rectangle destinationRectangle = new Rectangle((int)(vector2.X + 32f - 8f - vector.X / 2f) + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int)(vector2.Y + 64f + 8f - vector.Y / 2f) + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int)(16f + vector.X), (int)(16f + vector.Y / 2f));
                spriteBatch.Draw(Game1.mouseCursors, destinationRectangle, heldObject.Value.Quality < 4 ? new Rectangle(338 + (heldObject.Value.Quality - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8), Color.White * 0.95f, 0f, Vector2.Zero, SpriteEffects.None, (y + 1) * 64 / 10000f);
            }
        }
    }
}
