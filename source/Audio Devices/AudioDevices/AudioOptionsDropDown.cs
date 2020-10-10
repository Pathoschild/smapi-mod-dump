/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/AudioDevices
**
*************************************************/

using Microsoft.Xna.Framework;

namespace AudioDevices
{
    public class AudioOptionsDropDown : StardewValley.Menus.OptionsDropDown
    {
        public AudioOptionsDropDown() : base("Active Device", int.MaxValue, -1, -1)
        {
            UpdateDeviceList();
        }

        public override void leftClickReleased(int x, int y)
        {
            if (this.greyedOut || this.dropDownOptions.Count <= 0)
                return;
            ModEntry.Reflection.GetField<bool>(this, "clicked").SetValue(false);
            if (ModEntry.Reflection.GetField<Rectangle>(this, "dropDownBounds").GetValue().Contains(x, y))
            {
                ModEntry.UpdateAudioDevice(this.dropDownOptions[this.selectedOption]);
            }
            else
            {
                this.selectedOption = this.startingSelected;
            }
            AudioOptionsDropDown.selected = null;
        }

        public void UpdateDeviceList()
        {
            this.dropDownOptions.Clear();
            this.dropDownDisplayOptions.Clear();
            this.dropDownOptions.AddRange(ModEntry.GetAudioDeviceNames());
            this.dropDownDisplayOptions.AddRange(this.dropDownOptions.ConvertAll(n =>
                (n.Length > 24) ? (n.Substring(0, 22) + "...") : n
            ));
            this.selectedOption = this.dropDownOptions.FindIndex(o => o == ModEntry.mod.Settings.SelectedAudioDevice);
            ModEntry.Reflection.GetField<Rectangle>(this, "dropDownBounds").SetValue(new Rectangle(this.bounds.X, this.bounds.Y, this.bounds.Width - 48, this.bounds.Height * this.dropDownOptions.Count));
        }
    }
}
