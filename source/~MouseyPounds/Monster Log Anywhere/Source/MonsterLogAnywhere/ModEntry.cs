using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;

namespace MonsterLogAnywhere
{
    public class ModEntry : Mod, IAssetEditor
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.Config.Monster_Log_Keybind)
            {
                this.Monitor.Log("Detected button press to open monster log", LogLevel.Trace);
                if (Game1.activeClickableMenu == null)
                {
					GameLocation adventureGuild = Game1.getLocationFromName("AdventureGuild");
					if (this.Helper.ModRegistry.IsLoaded("DefenTheNation.CustomGuildChallenges"))
					{
						IReflectedMethod method = this.Helper.Reflection.GetMethod(adventureGuild, "ShowNewMonsterKillList");
						if (method == null)
						{
                            this.Monitor.Log("Cannot access the Custom Guild Challenge log method", LogLevel.Error);
                        }
						else 
						{
                            this.Monitor.Log("Showing the Custom Guild Challenge monster log", LogLevel.Trace);
                            method.Invoke();
						}
					}
					else 
					{
                        this.Monitor.Log("Showing the vanilla monster log", LogLevel.Trace);
                        (adventureGuild as AdventureGuild).showMonsterKillList();
					}
				}
                else
                {
                    this.Monitor.Log("Can't show monster log because a menu is up.", LogLevel.Debug);
                }
                
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (this.Config.Change_Header_and_Footer && asset.AssetNameEquals("Strings/Locations"))
            {
                return true;
            }

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (this.Config.Change_Header_and_Footer && asset.AssetNameEquals("Strings/Locations"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                data["AdventureGuild_KillList_Header"] = this.Config.New_Header;
                data["AdventureGuild_KillList_Footer"] = this.Config.New_Footer;
            }
        }
    }
}