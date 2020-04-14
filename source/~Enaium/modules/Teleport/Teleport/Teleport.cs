using System.Collections.Generic;
using EiTK.Gui;
using EiTK.Update;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace Teleport
{
    public class Teleport : Mod
    {
        
        private ModConfig Config;
        private List<TPData> _tpDatas;
        public static UpdateData updateData;
        public override void Entry(IModHelper helper)
        {
            updateData = helper.Data.ReadJsonFile<UpdateData>("manifest.json");
            if(UpdateUtils.isNewVersion(updateData)) Monitor.Log("You can update mod:" + updateData.contactDatas[0].websiteLink,LogLevel.Warn);

            Config = helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += onButtonPressed;
            init();
        }
        
        private void init()
        {
            _tpDatas = new List<TPData>();
            
            _tpDatas.Add(new TPData("farm","Farm",64, 15));
            _tpDatas.Add(new TPData("pierre-shop","Town", 43, 57));
            _tpDatas.Add(new TPData("blacksmith","Town",94,82));
            _tpDatas.Add(new TPData("museum","Town",101,90));
            _tpDatas.Add(new TPData("saloon","Town",45,71));
            _tpDatas.Add(new TPData("community-center","Town", 53, 20));
            _tpDatas.Add(new TPData("carpenter","Mountain",12,26));
            _tpDatas.Add(new TPData("adventurers-guild","Mountain",76,9));
            _tpDatas.Add(new TPData("ranch","Forest",76,9));
            _tpDatas.Add(new TPData("mines","Mine",13, 10));
            _tpDatas.Add(new TPData("skull","UndergroundMine121",0, 0));
            _tpDatas.Add(new TPData("willy-shop","Beach", 30, 34));
            _tpDatas.Add(new TPData("wizard-tower","Forest", 5, 27));
            _tpDatas.Add(new TPData("hats","Forest",34, 96));
            _tpDatas.Add(new TPData("desert","Desert",18, 28));
            _tpDatas.Add(new TPData("sandy-shop","SandyHouse",4, 8));
            _tpDatas.Add(new TPData("casino","Club", 53, 20));
            _tpDatas.Add(new TPData("quarry","Mountain",127, 12));
            _tpDatas.Add(new TPData("new-beach","Beach",87, 26));
            _tpDatas.Add(new TPData("secret-woods","Woods", 58, 15));
            _tpDatas.Add(new TPData("sewer","Sewer", 3, 48));
            _tpDatas.Add(new TPData("bathhouse","Railroad", 10, 57));
            _tpDatas.Add(new TPData("carpenter","Mountain",12,26));
            _tpDatas.Add(new TPData("joja","Town",96,51));
            _tpDatas.Add(new TPData("penny","Town",72,69));
            _tpDatas.Add(new TPData("leah","Forest",104,33));
            _tpDatas.Add(new TPData("haley","Town",20,89));
            _tpDatas.Add(new TPData("jodi","Town",10,86));
            _tpDatas.Add(new TPData("linus","Mountain",29,7));
            _tpDatas.Add(new TPData("lewis","Town",59,86));
            _tpDatas.Add(new TPData("elliott","Beach",49,11));
            _tpDatas.Add(new TPData("alex","Town",57,64));

        }
        
        private void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if(!Context.IsPlayerFree)
                return;
            if(!Context.IsWorldReady)
                return;
            if(e.Button != Config.tp)
                return;
            GuiHelper.openGui(new TPMenu(Helper,_tpDatas));
        }
        
        private void TP(string locationName, int tileX, int tileY)
        {
            Game1.exitActiveMenu();
            Game1.player.swimming.Value = false;
            Game1.player.changeOutOfSwimSuit();

            // warp
            Game1.warpFarmer(locationName, tileX, tileY, false);
        }

    }
}