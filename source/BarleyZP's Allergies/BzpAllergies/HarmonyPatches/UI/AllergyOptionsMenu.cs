/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BZP_Allergies.HarmonyPatches.UI
{
    internal class AllergyOptionsMenu : OptionsPage
    {
        public Traverse HoverTextTraverse;
        private Traverse UpArrowTraverse;
        private Traverse DownArrowTraverse;
        private bool StandAlone;
        public AllergyOptionsMenu(int x, int y, int width, int height, bool standAlone = false)
            : base(x, y, width, height)
        {
            // do we start random or not?
            PopulateOptions(false);

            if (AllergenManager.ModDataGet(Game1.player, "BarleyZP.BzpAllergies_Random", out string val))
            {
                PopulateOptions(val == "true");
            }
            else
            {
                PopulateOptions(false);
                Game1.player.modData["BarleyZP.BzpAllergies_Random"] = "false";
            }

            HoverTextTraverse = Traverse.Create(this).Field("hoverText");
            UpArrowTraverse = Traverse.Create(this).Field("upArrow");
            DownArrowTraverse = Traverse.Create(this).Field("downArrow");
            StandAlone = standAlone;
        }

        public override void draw(SpriteBatch b)
        {
            if (StandAlone)
            {
                b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
                Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, speaker: false, drawOnlyBox: true);
            }
            base.draw(b);
            ClickableTextureComponent upArrowRef = UpArrowTraverse.GetValue<ClickableTextureComponent>();
            ClickableTextureComponent downArrowRef = DownArrowTraverse.GetValue<ClickableTextureComponent>();

            upArrowRef.visible = options.Count > 7;
            downArrowRef.visible = options.Count > 7;

            if (StandAlone) drawMouse(b, ignore_transparency: true);
        }

        public override void performHoverAction(int x, int y)
        {
            ITranslationHelper translation = ModEntry.Instance.Translation;
            base.performHoverAction(x, y);

            // do any of the options have tooltip?
            for (int i = 0; i < optionSlots.Count; i++)
            {
                if (currentItemIndex + i >= options.Count) continue;

                ClickableComponent slot = optionSlots[i];
                OptionsElement el = options[currentItemIndex + i];

                // are we in the left third of the slot (where the text probably is)?
                Rectangle shrunkBounds = new(slot.bounds.X, slot.bounds.Y, slot.bounds.Width / 3, slot.bounds.Height);
                bool inLeftThirdOfSlot = shrunkBounds.Contains(x, y);

                if (inLeftThirdOfSlot && el is CustomOptionsCheckbox customEl && customEl.HoverText.Length > 0)
                {
                    string fromMod = translation.Get("allergy-menu.from-hovertext", new { modName = customEl.HoverText });
                    HoverTextTraverse.SetValue(fromMod);
                    break;
                }
            }
        }

        public void PopulateOptions(bool random)
        {
            ITranslationHelper translation = ModEntry.Instance.Translation;
            options.Clear();
            options.Add(new OptionsElement(translation.Get("allergy-menu.title")));

            ISet<string> has = AllergenManager.ModDataSetGet(Game1.player, Constants.ModDataHas);

            if (random)
            {
                options.Add(new CustomOptionsSmallFontElement(translation.Get("allergy-menu.have-random")));
                options.Add(new CustomOptionsHorizontalLine());

                // get discovered allergies
                ISet<string> discovered = AllergenManager.ModDataSetGet(Game1.player, Constants.ModDataDiscovered);

                if (discovered.Count == 0)
                {
                    // render 0 count message
                    options.Add(new CustomOptionsSmallFontElement(translation.Get("allergy-menu.none-discovered")));
                }
                else
                {
                    // show all discovered messages
                    foreach (string allergy in discovered)
                    {
                        string displayName = AllergenManager.GetAllergenDisplayName(allergy);
                        options.Add(new CustomOptionsSmallFontElement("- " + displayName));
                    }
                }

                // if hint, show many discovered/total
                if (ModEntry.Instance.Config.AllergenCountHint)
                {
                    string hintCountText = translation.Get("allergy-menu.discovered-count",
                        new { discoverCount = discovered.Count, hasCount = has.Count });
                    options.Add(new CustomOptionsSmallFontElement(hintCountText));
                }

                options.Add(new CustomOptionsHorizontalLine());
                options.Add(new CustomOptionsButtonPair(translation.Get("allergy-menu.reroll"), translation.Get("allergy-menu.switch-sel"),
                    RerollAllergies, AllergySelectionToggle));
            }
            else
            {
                options.Add(new CustomOptionsSmallFontElement(translation.Get("allergy-menu.have-sel")));
                options.Add(new CustomOptionsHorizontalLine());

                // get all the possible allergies
                List<string> allergenIds = AllergenManager.ALLERGEN_DATA_ASSET.Keys.ToList();
                allergenIds.Sort(AllergySortKey);

                // render the checkboxes
                foreach (string id in allergenIds)
                {
                    AllergenModel data = AllergenManager.ALLERGEN_DATA_ASSET[id];
                    string addedBy = "";
                    if (data.AddedByContentPackId != null)
                    {
                        IModInfo modInfo = ModEntry.Instance.Helper.ModRegistry.Get(data.AddedByContentPackId);
                        addedBy = modInfo.Manifest.Name;
                    }


                    CustomOptionsCheckbox checkbox = new(data.DisplayName, has.Contains(id),
                        (val) => AllergenManager.TogglePlayerHasAllergy(id, val), addedBy);
                    options.Add(checkbox);
                }


                options.Add(new CustomOptionsHorizontalLine());
                options.Add(new OptionsButton(translation.Get("allergy-menu.switch-random"), AllergySelectionToggle));
            }

            // reset scroll
            currentItemIndex = 0;
            Traverse.Create(this).Method("setScrollBarToCurrentIndex").GetValue();
        }

        private void AllergySelectionToggle()
        {
            bool currentlyRandom = false;
            if (AllergenManager.ModDataGet(Game1.player, "BarleyZP.BzpAllergies_Random", out string val))
            {
                currentlyRandom = val == "true";
            }
            Game1.player.modData["BarleyZP.BzpAllergies_Random"] = currentlyRandom ? "false" : "true";
            Game1.player.modData[Constants.ModDataDiscovered] = "";
            Game1.player.modData[Constants.ModDataHas] = "";

            // if we switch from selection to random, roll allergies
            if (!currentlyRandom)
            {
                RerollAllergies();
            }

            PopulateOptions(!currentlyRandom);
        }

        // assumption: valid allergy Id
        private int AllergySortKey(string a1, string a2)
        {
            // first sort by whether they're checked
            ISet<string> HasAllergens = AllergenManager.ModDataSetGet(Game1.player, Constants.ModDataHas);
            if ((HasAllergens.Contains(a1) && HasAllergens.Contains(a2)) || (!HasAllergens.Contains(a1) && !HasAllergens.Contains(a2)))
            {
                // now sort by content pack
                string? a1Pack = AllergenManager.ALLERGEN_DATA_ASSET[a1].AddedByContentPackId;
                string? a2Pack = AllergenManager.ALLERGEN_DATA_ASSET[a2].AddedByContentPackId;
                if (a1Pack == a2Pack)
                {
                    // sort by name
                    return a1.CompareTo(a2);
                }
                else if (a1Pack == null)
                {
                    return -1;
                }
                else if (a2Pack == null)
                {
                    return 1;
                }
                return a1Pack.CompareTo(a2Pack);
            }
            else
            {
                // one is checked, one isn't
                return HasAllergens.Contains(a1) ? -1 : 1;
            }
        }

        private void RerollAllergies()
        {
            List<string> newAllergies = AllergenManager.RollRandomKAllergies(ModEntry.Instance.Config.NumberRandomAllergies);
            Game1.player.modData[Constants.ModDataDiscovered] = "";
            Game1.player.modData[Constants.ModDataHas] = string.Join(',', newAllergies);
            PopulateOptions(true);
        }
    }
}
