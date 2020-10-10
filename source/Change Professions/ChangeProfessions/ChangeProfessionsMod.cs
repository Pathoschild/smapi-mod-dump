/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aleksanderwaagr/Stardew-ChangeProfessions
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ChangeProfessions
{
    public class ChangeProfessionsMod : Mod
    {
        private IModHelper _modHelper;
        private ProfessionManager _professionManager;

        public override void Entry(IModHelper modHelper)
        {
            _modHelper = modHelper;
            _professionManager = new ProfessionManager(modHelper);
            modHelper.Events.Input.ButtonReleased += OnButtonReleased;
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (!IsAcceptButton(e.Button))
                return;

            var clickedProfessionBar = GetClickedProfessionBar(e);
            if (clickedProfessionBar == null)
                return;

            if (!int.TryParse(clickedProfessionBar.name, out var clickedProfessionId))
                return;

            ShowProfessionChooserMenu(clickedProfessionId);
        }

        private bool IsAcceptButton(SButton button)
        {
            return button == SButton.MouseLeft || button == SButton.ControllerA;
        }

        private ClickableTextureComponent GetClickedProfessionBar(ButtonReleasedEventArgs e)
        {
            var skillsPage = GetSkillsPage();
            if (skillsPage == null)
                return null;
            return GetActiveBarAtPosition(skillsPage, e.Cursor.ScreenPixels);
        }

        private void ShowProfessionChooserMenu(int professionId)
        {
            var professionIdsToChoose = _professionManager.GetProfessionSet(professionId).Ids;
            var professionChooserMenu = new ProfessionChooserMenu(_modHelper, professionId, professionIdsToChoose);

            professionChooserMenu.OnChangedProfession += newProfessionId =>
            {
                _professionManager.ChangeProfession(professionId, newProfessionId);
            };

            Game1.activeClickableMenu = professionChooserMenu;
        }

        private IClickableMenu GetSkillsPage()
        {
            if (!(Game1.activeClickableMenu is GameMenu menu))
                return null;
            var pages = Helper.Reflection.GetField<List<IClickableMenu>>(menu, "pages").GetValue();
            var page = pages[menu.currentTab];
            var pageName = page.GetType().Name;
            return pageName == nameof(SkillsPage) || pageName == "NewSkillsPage" ? page : null;
        }

        private ClickableTextureComponent GetActiveBarAtPosition(IClickableMenu skillsPage, Vector2 position)
        {
            var skillBars = Helper.Reflection.GetField<List<ClickableTextureComponent>>(skillsPage, "skillBars").GetValue();
            return skillBars.FirstOrDefault(skillBar => skillBar.containsPoint((int)position.X, (int)position.Y) &&
                                                                  skillBar.hoverText.Length > 0 &&
                                                                  skillBar.name != "-1");
        }

    }
}