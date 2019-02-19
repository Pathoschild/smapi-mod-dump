using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;

namespace CreeperForage
{
    public class Assets : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\ObjectInformation") || asset.AssetNameEquals("Maps\\springobjects");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data\\ObjectInformation"))
                foreach (var item in Item.items)
                    try { asset.AsDictionary<int, string>().Data.Add(item.Value.internal_id, item.Value.GetData()); } catch { }
            else
            {
                var oldTex = asset.AsImage().Data;
                if (oldTex.Width != 4096)
                {
                    Texture2D newTex = new Texture2D(StardewValley.Game1.graphics.GraphicsDevice, oldTex.Width, System.Math.Max(oldTex.Height, 4096));
                    asset.ReplaceWith(newTex);
                    asset.AsImage().PatchImage(oldTex);
                }
                foreach (var obj in Item.items)
                    try { asset.AsImage().PatchImage(obj.Value.texture, null, new Rectangle(obj.Value.internal_id % 24 * 16, obj.Value.internal_id / 24 * 16, 16, 16)); } catch { }
            }

        }
    }

    public class Item
    {
        public static int base_id = 1920;
        public static Dictionary<string, Item> items;

        public string unique_id;
        public string displayname;
        public string description;
        public int internal_id;
        public int price;
        public int edibility;
        public Texture2D texture;

        public static void Setup()
        {
            Mod.instance.Helper.Content.AssetEditors.Add(new Assets());
            items = new Dictionary<string, Item>();
            CreateBasicPersonalItem("Haley", 1, "These look expensive.", 41);
            CreateBasicPersonalItem("Haley", 2, "These have clearly been worn.", 29);
            CreateBasicPersonalItem("Abigail", 1, "How exciting!", 29);
            CreateBasicPersonalItem("Abigail", 2, "They smell just like " + Config.GetNPC("Abigail").GetPronoun(1) + ".", 33);
            CreateBasicPersonalItem("Emily", 1, "I thought they'd be brighter.", 27);
            CreateBasicPersonalItem("Emily", 2, "This is a nice material.", 35);
            CreateBasicPersonalItem("Penny", 1, "How charming.", 29);
            CreateBasicPersonalItem("Penny", 2, "They smell kind of sweet.", 32);
            CreateBasicPersonalItem("Leah", 1, "I don't think " + Config.GetNPC("Leah").GetPronoun(0) + "'d mind.", 31);
            CreateBasicPersonalItem("Leah", 2, "What a lucky find!", 34); 
            CreateBasicPersonalItem("Jodi", 1, "Huh.", 22);
            CreateBasicPersonalItem("Jodi", 2, "A rare sight.", 22);
            CreateBasicPersonalItem("Caroline", 1, "About what I'd expect.", 24);
            CreateBasicPersonalItem("Caroline", 2, "Well how about that.", 32);
            CreateBasicPersonalItem("Maru", 1, "A little less shy on the inside.", 32);
            CreateBasicPersonalItem("Maru", 2, "Thoroughly lived in.", 33);
            CreateBasicPersonalItem("Robin", 1, "A lucky find?", 33);
            CreateBasicPersonalItem("Robin", 2, "They smell faintly of sawdust.", 36);
            CreateBasicPersonalItem("Alex", 1, "They're all sweaty.", 19);
            CreateBasicPersonalItem("Alex", 2, "Oh, these are nice.", 27);
            CreateBasicPersonalItem("Elliott", 1, "Nothing but class.", 29);
            CreateBasicPersonalItem("Elliott", 2, "Such graceful elegance.", 31);
            CreateBasicPersonalItem("Harvey", 1, "They seem reliable.", 24);
            CreateBasicPersonalItem("Harvey", 2, Config.GetNPC("Harvey").GetPronoun(0) + " kind of suprises me.", 28);
            CreateBasicPersonalItem("Sam", 1, "So adventurous!", 33);
            CreateBasicPersonalItem("Sam", 2, "They smell like " + Config.GetNPC("Sam").GetPronoun(1) + ".", 16);
            CreateBasicPersonalItem("Sebastian", 1, UppercaseFirst(Config.GetNPC("Sebastian").GetPronoun(0)) + " never disappoints.", 19);
            CreateBasicPersonalItem("Sebastian", 2, "They're kind of... damp inside.", 40);
            CreateBasicPersonalItem("Shane", 1, "Whoa, they have a strong smell.", 14);
            CreateBasicPersonalItem("Shane", 2, "They're nicer than you'd expect.", 21);
            CreateBasicPersonalItem("Clint", 1, "To think of the tools " + Config.GetNPC("Clint").GetPronoun(0) + " uses.", 26);
            CreateBasicPersonalItem("Clint", 2, "They have a strange, smoky quality.", 40);
            CreateBasicPersonalItem("Demetrius", 1, "I probably shouldn't have these.", 28);
            CreateBasicPersonalItem("Demetrius", 2, "They don't smell worn.", 24);
            CreateBasicPersonalItem("Gus", 1, "These definitely belong to " + Config.GetNPC("Gus").Name + ".", 33);
            CreateBasicPersonalItem("Gus", 2, "They're still kind of warm.", 36);
            CreateBasicPersonalItem("Kent", 1, "A bit less brave on the inside.", 17);
            CreateBasicPersonalItem("Kent", 2, "What parts of the world these have seen?", 22);
            CreateBasicPersonalItem("Lewis", 1, "Another fine display for the fair.", 33);
            CreateBasicPersonalItem("Lewis", 2, "Perfectly in character.", 19);
            CreateBasicPersonalItem("Marnie", 1, "Soooo comfortable!", 25);
            CreateBasicPersonalItem("Marnie", 2, "Smells like hay.", 16);
            CreateBasicPersonalItem("Pam", 1, "They're... broken in.", 14);
            CreateBasicPersonalItem("Pam", 2, "These smell like something, but what?", 19);
            CreateBasicPersonalItem("Pierre", 1, "Clearly doing alright for " + Config.GetNPC("Pierre").GetPronoun(1) + "self.", 33);
            CreateBasicPersonalItem("Pierre", 2, Config.GetNPC("Pierre").GetPronoun(0) + " should be more careful.", 36);
            CreateBasicPersonalItem("Sandy", 1, "Absolutely fabulous.", 40);
            CreateBasicPersonalItem("Sandy", 2, "Worth the trip.", 41);
            CreateBasicPersonalItem("Willy", 1, "Salty.", 17);
            CreateBasicPersonalItem("Willy", 2, "Smells vaguely like fish.", 21);
            CreateBasicPersonalItem("Wizard", 1, "Almost ethereal - a fabric like you've never seen.", 49);
            CreateBasicPersonalItem("Wizard", 2, "They are absolutely enchanting...", 41);
            CreateBasicPersonalItem("Vincent", 1, "At least they're clean.", 24);
            CreateBasicPersonalItem("Vincent", 2, "Ah, an air of adventure surrounds these.", 26);
            CreateBasicPersonalItem("Jas", 1, "They give you the chills.", 18);
            CreateBasicPersonalItem("Jas", 2, UppercaseFirst(Config.GetNPC("Jas").GetPronoun(0)) + " probably doesn't need these.", 16);
            CreateBasicPersonalItem("Linus", 1, "Oh my. Oh.", 14);
            CreateBasicPersonalItem("Linus", 2, "Hopefully " + Config.GetNPC("Linus").GetPronoun(0) +  " got new ones.", 15);
            CreateBasicPersonalItem("Evelyn", 1, "Aged and experienced.", 20);
            CreateBasicPersonalItem("Evelyn", 2, ".....", 17);
            CreateBasicPersonalItem("George", 1, "Very dependable.", 13);
            CreateBasicPersonalItem("George", 2, "It's dangerous to go alone! Take this.", 15);
        }

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static void CreateBasicPersonalItem(string npc, int variant, string desc, int price)
        {
            SetupItem("px." + npc.ToLower() + variant, "px" + Config.GetNPC(npc).Abbreviate(npc) + (Config.GetNPC(npc).HasMaleItems() ? "m" : "f") + variant, Config.GetNPC(npc).Name + "'s " + (Config.GetNPC(npc).HasMaleItems() ? "Underwear" : "Panties"), desc, price, 10);
        }

        public static void SetupItem(string id, string tx, string name, string desc, int price, int edibility)
        {
            items[id] = new Item(id, tx, name, desc, price, edibility);
        }

        public Item(string uid, string tx, string name, string desc, int price, int edibility)
        {
            unique_id = uid;
            displayname = name;
            description = desc;
            internal_id = base_id++;
            this.price = price;
            this.edibility = edibility;
            this.texture = Mod.instance.Helper.Content.Load<Texture2D>("./Assets/" + tx + ".png", ContentSource.ModFolder);
        }

        public string GetData()
        {
            return $"{displayname}/{price}/{edibility}/Basic {StardewValley.Object.artisanGoodsCategory}/{displayname}/{description}";
        }
    }
}
