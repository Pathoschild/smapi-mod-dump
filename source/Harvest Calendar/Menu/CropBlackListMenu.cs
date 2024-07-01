/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LonerAxl/Stardew_HarvestCalendar
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using HarvestCalendar.Framework;
using StardewValley.Menus;
using System.Runtime.CompilerServices;

namespace HarvestCalendar.Menu
{
    internal class CropBlackListMenu:ScrollableMenu
    {
        private Configuration _config;

        public CropBlackListMenu(Configuration config):base(I18n.UI_CropBlackList_Title()) 
        {
            _config = config;
            SetOptions();
        }

        protected override void SetOptions()
        {
            var slotWidth = _optionSlots[0].bounds.Width;
            _options.Clear();
            if (_config.CropBlackList.Count > 0)
            {
                foreach (var item in _config.CropBlackList)
                {
                    var produce = ItemRegistry.GetDataOrErrorItem(item);

                    _options.Add(new CropBlackListOptions(
                        label: produce.DisplayName,
                        slotWidth: slotWidth,
                        toggle: () =>
                        {
                            var confirmDialog = new ConfirmationDialog(
                                I18n.UI_CropBlackList_Confirm_Remove(produce.DisplayName),
                                (Farmer _) => RemoveFromBlackList(item),
                                (Farmer _) => CloseConfirmationDialog()
                                );
                            SetChildMenu(confirmDialog);
                        }
                        ));
                }
            }
            else 
            {
                _options.Add(new OptionsElement(I18n.UI_CropBlackList_NoItems()));
            }

        }

        private void RemoveFromBlackList(string item) 
        {
            CloseConfirmationDialog();
            _config.RemoveFromBlackList(item);
            ResetComponents();
            SetOptions();
        }

        private void CloseConfirmationDialog()
        {
            var dialog = GetChildMenu();

            if (dialog != null)
            {
                dialog.exitThisMenuNoSound();
                SetChildMenu(null);
            }
        }
    }
}
