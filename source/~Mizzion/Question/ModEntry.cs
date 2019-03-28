using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace Question
{
    public class ModEntry : Mod
    {
        string MapPath = "";

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += Button;
        }

        private void Button(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.IsDown(SButton.NumPad8))
            {
                

                //Lets clean this up some
                string question = Helper.Translation.Get("Wedding_Question");
                List<Response> options = new List<Response>()
                {
                    new Response("On the Beach.",this.Helper.Translation.Get("Wedding_Beach")),
                    new Response("In the Forest, near the river.",this.Helper.Translation.Get("Wedding_Forest")),
                    new Response("In the Deep Woods.",this.Helper.Translation.Get("Wedding_Wood")),
                    new Response("Where the Flower Dance takes place!",this.Helper.Translation.Get("Wedding_Flower"))
                };

                Game1.player.currentLocation.createQuestionDialogue(
                    question,
                    options.ToArray(),
                    answer
                );
            }

            if (e.IsDown(SButton.NumPad7))
            {
                Chest c = new Chest(true);
                if(Game1.currentLocation.isTileLocationTotallyClearAndPlaceable(Game1.player.getTileLocation()))
                    Game1.currentLocation.objects.Add(Game1.player.getTileLocation(), c);
                else
                    Game1.showGlobalMessage("Couldnt place chest");
            }

            if (e.IsDown(SButton.NumPad6))
            {
                var obj = Game1.currentLocation.objects.Pairs;
                foreach (var o in obj)
                {
                    if (o.Value.ParentSheetIndex == 372)
                    {
                        Game1.currentLocation.objects.Remove(o.Key);
;                    }
                }
            }
        }

        private void answer(Farmer who, string ans)
        {
                switch (ans)
                {
                    case "On the Beach.":
                        MapPath = "assets/WeddingBeach.tbin";
                        // Load Beach
                        break;
                    case "In the Forest, near the river.":
                        MapPath = "assets/WeddingForest.tbin";
                    // Load Forest
                    break;
                    case "In the Deep Woods.":
                        MapPath = "assets/WeddingWoods.tbin";
                    // Load Wood
                    break;
                    case "Where the Flower Dance takes place!":
                        MapPath = "assets/WeddingFlower.tbin";

                    // Load Flower Dance
                    break;
                }
            Game1.showGlobalMessage(MapPath);
        }
    }
}
