# Enaium Tool Kit

## GuiMenu

```
using System.Collections.Generic;
using EiTK.Gui;
using EiTK.Gui.Option;
using EiTK.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace EiTKDemo.Menu
{
    public class Menu : GuiMenu
    {
        public Menu()
        {


            List<GuiOptionsElements> optionLabels;
            optionLabels = new List<GuiOptionsElements>();


            optionLabels.Add(new GuiOptionLabel("----------------1"));
            optionLabels.Add(new GuiOptionLabel("----------------2"));
            optionLabels.Add(new GuiOptionLabel("----------------3"));
            optionLabels.Add(new GuiOptionLabel("----------------4"));
            optionLabels.Add(new GuiOptionLabel("----------------5"));
            optionLabels.Add(new GuiOptionLabel("----------------6"));
            optionLabels.Add(new GuiOptionButton("----------------7","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------7"));
            }));
            optionLabels.Add(new GuiOptionButton("----------------8","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------8"));
            }));            
            optionLabels.Add(new GuiOptionButton("----------------9","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------9"));
            }));
            optionLabels.Add(new GuiOptionLabel("----------------10"));
            optionLabels.Add(new GuiOptionLabel("----------------11"));
            optionLabels.Add(new GuiOptionLabel("----------------12"));
            optionLabels.Add(new GuiOptionLabel("----------------13"));
            optionLabels.Add(new GuiOptionLabel("----------------14"));
            optionLabels.Add(new GuiOptionLabel("----------------15"));
            optionLabels.Add(new GuiOptionButton("----------------16","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------16"));
            }));
            optionLabels.Add(new GuiOptionLabel("----------------17"));
            optionLabels.Add(new GuiOptionButton("----------------18","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------18"));
            }));
            optionLabels.Add(new GuiOptionLabel("----------------19"));
            optionLabels.Add(new GuiOptionButton("----------------20","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------20"));
            }));
            optionLabels.Add(new GuiOptionLabel("----------------21"));
            optionLabels.Add(new GuiOptionButton("----------------22","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------22"));
            }));
            
            this.optionLists.Add(new GuiOptionList(50,50,10)
            {
                guiOptionsElementses = optionLabels
            });
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            drawMouse(b);
        }
    }
}
```

openGui `GuiHelper.openGui(new Menu());`

## Update

```
        private UpdateData updateData;
        public override void Entry(IModHelper helper)
        {
            updateData = helper.Data.ReadJsonFile<UpdateData>("manifest.json");
            if(UpdateUtils.isNewVersion(updateData)) Monitor.Log("You can update mod:" + updateData.contactDatas[0].websiteLink,LogLevel.Warn);
        }
```

### manifest.json

```
{
  "Name": "EiTK",
  "Author": "Enaium",
  "Version": "1.0.0",
  "Description": "Enaium Toolkit",
  "UniqueID": "Enaium.EiTK",
  "EntryDll": "EiTK.dll",
  "MinimumApiVersion": "3.2.0",
  "UpdateKeys": [ "Github:0" ],
  "newVersionLink": "http://svmod.enaium.cn/modules/EiTK/EiTK/manifest.json",
  "contactDatas": [
    {
      "websiteName": "Enaium",
      "websiteLink": "http://svmod.enaium.cn/"
    },
    {
      "websiteName": "github",
      "websiteLink": "https://github.com/Enaium/Stardew_Valley_Mods/tree/master/modules/EiTK"
    },
    {
      "websiteName": "3dm",
      "websiteLink": "https://mod.3dmgame.com/mod/152314"
    },
    {
      "websiteName": "爱发电",
      "websiteLink": "https://afdian.net/p/a8a36fbc4e5711eaad5252540025c377"
    }
  ]
}
```

### manifest.json Example

```
{
  "Name": "<your project name>",
  "Author": "<your name>",
  "Version": "1.0.0",
  "Description": "<One or two sentences about the mod>",
  "UniqueID": "<your name>.<your project name>",
  "EntryDll": "<your project name>.dll",
  "MinimumApiVersion": "3.2.0",
  "UpdateKeys": [ "Github:0" ],
  "newVersionLink": "<your project manifest.json link>",
  "contactDatas": [
    {
      "websiteName": "<your Update websiteName>",
      "websiteLink": "<your Update websiteLink>"
    },
	{
      "websiteName": "<your websiteName>",
      "websiteLink": "<your websiteLink>"
    },
	{
      "websiteName": "<your websiteName>",
      "websiteLink": "<your websiteLink>"
    }
  ]
}
```
