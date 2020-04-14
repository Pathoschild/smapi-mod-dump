using EiTK.Gui;
using EiTK.Update;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace EiTKDemo
{
    public class EiTKDemo : Mod
    {
        public static UpdateData updateData;

        public override void Entry(IModHelper helper)
        {
            updateData = helper.Data.ReadJsonFile<UpdateData>("manifest.json");
            if(UpdateUtils.isNewVersion(updateData)) Monitor.Log("You can update mod:" + updateData.contactDatas[0].websiteLink,LogLevel.Warn);
            helper.Events.Input.ButtonPressed += onButton;
        }

        public void onButton(object sender,ButtonPressedEventArgs e)
        {
            if(!Context.IsWorldReady)
                return;
            if(!Context.IsPlayerFree)
                return;
            if(e.Button != SButton.Y)
                return;
            GuiHelper.openGui(new Menu.Menu());
        }
        
    }
}