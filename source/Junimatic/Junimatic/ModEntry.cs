/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.Objects;

namespace NermNermNerm.Junimatic
{
    public class ModEntry
        : Mod, ISimpleLog
    {
        public const string BigCraftablesSpritesPseudoPath = "Mods/NermNermNerm/Junimatic/Sprites";
        public const string OneTileSpritesPseudoPath = "Mods/NermNermNerm/Junimatic/1x1Sprites";

        public const string SetJunimoColorEventCommand = "junimatic.setJunimoColor";

        public UnlockPortal UnlockPortalQuest = new UnlockPortal();
        public UnlockCropMachines CropMachineHelperQuest = new UnlockCropMachines();
        public UnlockMiner UnlockMiner = new UnlockMiner();
        public UnlockAnimal UnlockAnimal = new UnlockAnimal();
        public UnlockForest UnlockForest = new UnlockForest();
        public UnlockFishing UnlockFishing = new UnlockFishing();

        private readonly WorkFinder workFinder = new WorkFinder();
        public PetFindsThings PetFindsThings = new PetFindsThings();

        public static ModEntry Instance = null!;

        public ModEntry() { }

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            this.CropMachineHelperQuest.Entry(this);
            this.UnlockPortalQuest.Entry(this);
            this.UnlockMiner.Entry(this);
            this.UnlockAnimal.Entry(this);
            this.UnlockForest.Entry(this);
            this.workFinder.Entry(this);
            this.UnlockFishing.Entry(this);
            this.PetFindsThings.Entry(this);

            this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;

            Event.RegisterCommand(SetJunimoColorEventCommand, this.SetJunimoColor);
        }

        private void SetJunimoColor(Event @event, string[] split, EventContext context)
        {
            try
            {
                Color color = Color.Goldenrod;
                if (split.Length > 2)
                {
                    this.LogWarning($"{SetJunimoColorEventCommand} usage: [ <color> ]    where <color> is one of the constants in Microsoft.Xna.Framework.Color - e.g. 'Goldenrod'");
                    return;
                }

                if (split.Length == 2)
                {
                    var prop = typeof(Color).GetProperty(split[1], BindingFlags.Public | BindingFlags.Static);
                    if (prop is null)
                    {
                        this.LogWarning($"{SetJunimoColorEventCommand} was given '{split[1]}' as an argument, but that's not a valid Color constant.");
                        return;
                    }

                    color = (Color)prop.GetValue(null)!;
                }

                var junimo = @event.actors.OfType<Junimo>().FirstOrDefault();
                if (junimo is null)
                {
                    this.LogWarning($"{SetJunimoColorEventCommand} invoked when there wasn't a Junimo actor");
                    return;
                }

                var property = typeof(Junimo).GetField("color", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (property is null)
                {
                    this.LogError($"{SetJunimoColorEventCommand} can't set color because the game code is changed and the 'color' field is not there anymore.");
                    return;
                }

                ((NetColor)property.GetValue(junimo)!).Value = color;
            }
            finally
            {
                @event.CurrentCommand++;
            }
        }



        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(BigCraftablesSpritesPseudoPath))
            {
                e.LoadFromModFile<Texture2D>("assets/Sprites.png", AssetLoadPriority.Exclusive);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(OneTileSpritesPseudoPath))
            {
                e.LoadFromModFile<Texture2D>("assets/1x1_Sprites.png", AssetLoadPriority.Exclusive);
            }
            else if (e.NameWithoutLocale.StartsWith("Characters/Dialogue/"))
            {
                e.Edit(editor =>
                {
                    ConversationKeys.EditAssets(e.NameWithoutLocale, editor.AsDictionary<string, string>().Data);
                });
            }
        }

        public void WriteToLog(string message, LogLevel level, bool isOnceOnly)
        {
            if (isOnceOnly)
            {
                this.Monitor.LogOnce(message, level);
            }
            else
            {
                this.Monitor.Log(message, level);
            }
        }

        private static readonly Regex unQualifier = new Regex(@"^\([a-z]\)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        internal static void AddQuestItem(IDictionary<string, ObjectData> objects, string qiid, string displayName, string description, int spriteIndex)
        {
            string itemId = unQualifier.Replace(qiid, "");  // I don't think there is a more stylish way to unqualify a name
            objects[itemId] = new()
            {
                Name = itemId,
                DisplayName = displayName,
                Description = description,
                Type = "Quest",
                Category = -999,
                Price = 0,
                Texture = ModEntry.OneTileSpritesPseudoPath,
                SpriteIndex = spriteIndex,
                ContextTags = new() { "not_giftable", "not_placeable", "prevent_loss_on_death" },
            };
        }
    }
}
