/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces.Spells;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.CraneGame;
using Microsoft.Xna.Framework.Content;
using StardewValley;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArsVenefici.Framework.GUI.DragNDrop
{
    public class SpellPartDraggable : Draggable<ISpellPart>
    {
        [JsonIgnore]
        public static int SIZE = 64;

        [JsonIgnore]
        private Texture2D sprite;

        [JsonIgnore]
        private string nametranslationKey;

        [JsonIgnore]
        private string descriptiontranslationKey;

        [JsonIgnore]
        private Dictionary<Key<Type>, object> data = new Dictionary<Key<Type>, object>();

        public SpellPartDraggable(ISpellPart content, ModEntry modEntry) : base(SIZE, SIZE, content)
        {
            string id = content.GetId();
            sprite = modEntry.spellPartIconManager.GetSprite(id);
            nametranslationKey = modEntry.Helper.Translation.Get($"spellpart.{id}.name");
            descriptiontranslationKey = modEntry.Helper.Translation.Get($"spellpart.{id}.description");
        }

        public ISpellPart GetPart()
        {
            return content;
        }

        public string GetNameTranslationKey()
        {
            return nametranslationKey;
        }

        public string GetDescriptionTranslationKey()
        {
            return descriptiontranslationKey;
        }

        public override void Draw(SpriteBatch spriteBatch, int positionX, int positionY, float pPartialTick)
        {
            spriteBatch.Draw(sprite, new Vector2(positionX, positionY), Color.White);
        }

        public void SetData<T>(Key<Type> key, T data)
        {
            this.data.Add(key, data);
        }

        public T GetData<T>(Key<Type> key, T defaultValue)
        {
            return (T)data.GetValueOrDefault(key, defaultValue);
        }

        public T GetData<T>(Key<Type> key)
        {
            return (T)data.GetValueSafe(key);
        }

        public class Key<T>
        {
            private static Dictionary<string, Key<T>> LOOKUP = new Dictionary<string, Key<T>>();
            string name;

            public Key(string name)
            {
                this.name = name;
            }


            public static Key<T> Get(string name)
            {
                return LOOKUP.GetValueOrDefault(name, new Key<T>(name));
            }
        }
    }
}
