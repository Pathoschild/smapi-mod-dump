using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using Custom_Shop_Mod_Redux;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace Custom_NPC_With_Custom_Shop_Redux
{

   
    public class Class1 : Mod
    {
        bool last_chat; //make sure that the dialogue will appear only on the last dialogue of the npc for that day.
        bool once; //make sure that my shop is only called once
        bool activate_shop; //speaks for itself.
        NPC myNPC; //my merchant npc
        string path; //path of .dll file of mod on disk
        string filename; //name of shop file
        bool game_loaded; //make sure player is loaded otherwise things blow up.
        bool first; ////an awfully named variable that prevents my npc from being updated a billion times.

        public override void Entry(params object[] objects)
        {
            StardewModdingAPI.Events.PlayerEvents.LoadedGame += PlayerEvents_LoadedGame;
            StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;
        }


        public void GameEvents_UpdateTick(object sender, EventArgs e) //run all the time for accuracy.
        {
            //the order of these if statments is critical.

            if (game_loaded == false) return;

            if (last_chat==true && Game1.currentSpeaker == null) //if i spoke to my npc, but we are through talking today.
            {
                Log.Info("HELLO? IS ANYONE HOME?");
                npc_shop();
                last_chat = false;
            }

            if (Game1.currentSpeaker == null) //keep the bools in check always if not speaking to an NPC.
            {
                once = false;
                first = true; 
                activate_shop = false;
                return;
            }

            if (Game1.currentSpeaker.name == myNPC.name) // my npc that I am looking for. Checked in The loading function.
            {
                
                //DO a bunch of checking. Seriously I recommend that this stays the same.
                if (first == false) return;
                Dialogue ehh = Game1.currentSpeaker.CurrentDialogue.Peek();
                if (Game1.currentSpeaker.CurrentDialogue.Peek() == null) //if I am on the last dialogue tell me so
                {
                    last_chat = true;
                }

                if (ehh.getCurrentDialogue() == "") last_chat = true; //if I am on the last dialogue tell me so
                last_chat = ehh.isOnFinalDialogue(); //if I am on the last dialogue tell me so
                first = false; //an awfully named variable that prevents my npc from being updated a billion times.
            }  
        }

        //actually call my shop.
       public void npc_shop()
        {
            if (activate_shop == false)
            {
                activate_shop = true; //if my shop isn't set propperly but you call this function, go ahead and set the shop anyways. You probably know what you are doing.
            }

            if (activate_shop == true && !StardewValley.Game1.onScreenMenus.Contains(Custom_Shop_Mod_Redux.Class1.mymenu)) //if my shop should be active and isn't on the screen, create it.
            {
                if (once == true) return;
                Custom_Shop_Mod_Redux.Class1.external_shop_file_call(path, filename,"Hey there aren't you going to buy something?", myNPC); //externally call my custom shop with added npc dialogue and npc portrait

               //  Custom_Shop_Mod_Redux.Class1.external_shop_file_call(path, filename); This is the older, much simpler default shop interface.
                once = true;
            }
}


public void PlayerEvents_LoadedGame(object sender, EventArgs e)
        {
            last_chat = false;
            first = true;
            game_loaded = true;
            activate_shop = false;
            once = false;
            path = PathOnDisk; //points to directory where .dll is stored
            filename = "My_Streaming_Shop.txt"; //name of shop file
            GameLocation NPClocation;
            foreach (StardewValley.GameLocation asdf in Game1.locations)
            {
                NPClocation = (GameLocation)asdf;


                foreach (StardewValley.NPC obj in NPClocation.characters)
                {
                    if (obj.name == "Abigail") //setup abigail to be my merchant npc. This can be any npc in the game.
                    {
                        Log.Info(obj.name);
                        myNPC = obj;
                    }
                }
            }
        }  //all these curleys in a row...
    }
}
