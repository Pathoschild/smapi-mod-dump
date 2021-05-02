/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Omegasis.HappyBirthday.Framework;
using Omegasis.HappyBirthday.Framework.Gifts;
using StardewValley;
using static System.String;
using SObject = StardewValley.Object;

namespace Omegasis.HappyBirthday
{
    public class GiftManager
    {
        public ModConfig Config => HappyBirthday.Config;

        private Dictionary<string, string> defaultBirthdayGifts = new Dictionary<string, string>()
        {
            ["Universal_Love_Gift"] = "74 1 446 1 204 1 446 5 773 1",
            ["Universal_Like_Gift"] = "-2 3 -7 1 -26 2 -75 5 -80 3 72 1 220 1 221 1 395 1 613 1 634 1 635 1 636 1 637 1 638 1 724 1 233 1 223 1 465 20 -79 5",
            ["Universal_Neutral_Gift"] = "194 1 262 5 -74 5 -75 3 334 5 335 1 390 20 388 20 -81 5 -79 3",
            ["Robin"] = " BestGifts/224 1 426 1 636 1/GoodGift/-6 5 -79 5 424 1 709 1/NeutralGift//",
            ["Demetrius"] = " Best Gifts/207 1 232 1 233 1 400 1/Good Gifts/-5 3 -79 5 422 1/NeutralGift/-4 3/",
            ["Maru"] = " BestGift/72 1 197 1 190 1 215 1 222 1 243 1 336 1 337 1 400 1 787 1/Good Gift/-260 1 62 1 64 1 66 1 68 1 70 1 334 1 335 1 725 1 726 1/NeutralGift/",
            ["Sebastian"] = " Best/84 1 227 1 236 1 575 1 305 1 /Good/267 1 276 1/Neutral/-4 3/",
            ["Linus"] = " Best/88 1 90 1 234 1 242 1 280 1/Good/-5 3 -6 5 -79 5 -81 10/Neutral/-4 3/",
            ["Pierre"] = " Best/202 1/Good/-5 3 -6 5 -7 1 18 1 22 1 402 1 418 1 259 1/Neutral//",
            ["Caroline"] = " Best/213 1 593 1/Good/-7 1 18 1 402 1 418 1/Neutral// ",
            ["Abigail"] = " Best/66 1 128 1 220 1 226 1 276 1 611 1/Good//Neutral// ",
            ["Alex"] = " Best/201 1 212 1 662 1 664 1/Good/-5 3/Neutral// ",
            ["George"] = " Best/20 1 205 1/Good/18 1 195 1 199 1 200 1 214 1 219 1 223 1 231 1 233 1/Neutral// ",
            ["Evelyn"] = " Best/72 1 220 1 239 1 284 1 591 1 595 1/Good/-6 5 18 1 402 1 418 1/Neutral// ",
            ["Lewis"] = " Best/200 1 208 1 235 1 260 1/Good/-80 5 24 1 88 1 90 1 192 1 258 1 264 1 272 1 274 1 278 1/Neutral// ",
            ["Clint"] = " Best/60 1 62 1 64 1 66 1 68 1 70 1 336 1 337 1 605 1 649 1 749 1 337 5/Good/334 20 335 10 336 5/Neutral// ",
            ["Penny"] = " Best/60 1 376 1 651 1 72 1 164 1 218 1 230 1 244 1 254 1/Good/-6 5 20 1 22 1/Neutral// ",
            ["Pam"] = " Best/24 1 90 1 199 1 208 1 303 1 346 1/Good/-6 5 -75 5 -79 5 18 1 227 1 228 1 231 1 232 1 233 1 234 1 235 1 236 1 238 1 402 1 418 1/Neutral/-4 3/ ",
            ["Emily"] = " Best/60 1 62 1 64 1 66 1 68 1 70 1 241 1 428 1 440 1 /Good/18 1 82 1 84 1 86 1 196 1 200 1 207 1 230 1 235 1 402 1 418 1/Neutral// ",
            ["Haley"] = " Best/221 1 421 1 610 1 88 1 /Good/18 1 60 1 62 1 64 1 70 1 88 1 222 1 223 1 232 1 233 1 234 1 402 1 418 1 /Neutral// ",
            ["Jas"] = " Best/221 1 595 1 604 1 /Good/18 1 60 1 64 1 70 1 88 1 232 1 233 1 234 1 222 1 223 1 340 1 344 1 402 1 418 1 /Neutral// ",
            ["Vincent"] = " Best/221 1 398 1 612 1 /Good/18 1 60 1 64 1 70 1 88 1 232 1 233 1 234 1 222 1 223 1 340 1 344 1 402 1 418 1 /Neutral// ",
            ["Jodi"] = " Best/72 1 200 1 211 1 214 1 220 1 222 1 225 1 231 1 /Good/-5 3 -6 5 -79 5 18 1 402 1 418 1 /Neutral// ",
            ["Kent"] = " Best/607 1 649 1 /Good/-5 3 -79 5 18 1 402 1 418 1 /Neutral// ",
            ["Sam"] = " Best/90 1 206 1 655 1 658 1 562 1 731 1/Good/167 1 210 1 213 1 220 1 223 1 224 1 228 1 232 1 233 1 239 1 -5 3/Neutral// ",
            ["Leah"] = " Best/196 1 200 1 348 1 606 1 651 1 650 1 426 1 430 1 /Good/-5 3 -6 5 -79 5 -81 10 18 1 402 1 406 1 408 1 418 1 86 1 /Neutral// ",
            ["Shane"] = " Best/206 1 215 1 260 1 346 1 /Good/-5 3 -79 5 303 1 /Neutral// ",
            ["Marnie"] = " Best/72 1 221 1 240 1 608 1 /Good/-5 3 -6 5 402 1 418 1 /Neutral// ",
            ["Elliott"] = " Best/715 1 732 1 218 1 444 1 /Good/727 1 728 1 -79 5 60 1 80 1 82 1 84 1 149 1 151 1 346 1 348 1 728 1 /Neutral/-4 3 / ",
            ["Gus"] = " Best/72 1 213 1 635 1 729 1 /Good/348 1 303 1 -7 1 18 1 /Neutral// ",
            ["Dwarf"] = " Best/60 1 62 1 64 1 66 1 68 1 70 1 749 1 /Good/82 1 84 1 86 1 96 1 97 1 98 1 99 1 121 1 122 1 /Neutral/-28 20 / ",
            ["Wizard"] = " Best/107 1 155 1 422 1 769 1 768 1 /Good/-12 3 72 1 82 1 84 1/Neutral// ",
            ["Harvey"] = " Best/348 1 237 1 432 1 395 1 342 1 /Good/-81 10 -79 5 -7 1 402 1 418 1 422 1 436 1 438 1 442 1 444 1 422 1 /Neutral// ",
            ["Sandy"] = " Best/18 1 402 1 418 1 /Good/-75 5 -79 5 88 1 428 1 436 1 438 1 440 1 /Neutral// ",
            ["Willy"] = " Best/72 1 143 1 149 1 154 1 276 1 337 1 698 1 /Good/66 1 336 1 340 1 699 1 707 1 /Neutral/-4 3 / ",
            ["Krobus"] = " Best/72 1 16 1 276 1 337 1 305 1 /Good/66 1 336 1 340 1 /Neutral// "
        };

        public Dictionary<string, string> defaultSpouseBirthdayGifts = new Dictionary<string, string>()
        {

            ["Universal_Gifts"] = "74 1 446 1 204 1 446 5 773 1",
            ["Alex"] = "",
            ["Elliott"] = "",
            ["Harvey"] = "",
            ["Sam"] = "",
            ["Sebastian"] = "",
            ["Shane"] = "",
            ["Abigail"] = "",
            ["Emily"] = "",
            ["Haley"] = "",
            ["Leah"] = "",
            ["Maru"] = "",
            ["Penny"] = "",
        };


        /// <summary>The next birthday gift the player will receive.</summary>
        public Item BirthdayGiftToReceive;


        public static Dictionary<string, List<GiftInformation>> NPCBirthdayGifts;
        public static Dictionary<string, List<GiftInformation>> SpouseBirthdayGifts;
        public static List<GiftInformation> DefaultBirthdayGifts;

        /// <summary>Construct an instance.</summary>
        public GiftManager()
        {
            //this.BirthdayGifts = new List<Item>();


            NPCBirthdayGifts = new Dictionary<string, List<GiftInformation>>();
            SpouseBirthdayGifts = new Dictionary<string, List<GiftInformation>>();
            DefaultBirthdayGifts = new List<GiftInformation>();


            this.registerGiftIDS();
            this.loadDefaultBirthdayGifts();
            this.loadVillagerBirthdayGifts();
            this.loadSpouseBirthdayGifts();

            this.createNPCBirthdayGifts();
            this.createSpouseBirthdayGifts();
        }

        public void registerGiftIDS()
        {
            foreach (var v in GiftIDS.GetSDVObjects())
            {
                Item i = new StardewValley.Object((int)v, 1);
                string uniqueID = "StardewValley.Object." + Enum.GetName(typeof(GiftIDS.SDVObject), (int)v);
                HappyBirthday.ModMonitor.Log("Added gift with id: " + uniqueID);
                GiftIDS.RegisteredGifts.Add(uniqueID, i);
            }
            HappyBirthday.ModHelper.Data.WriteJsonFile<List<string>>(Path.Combine("ModAssets", "Gifts", "RegisteredGifts" + ".json"),GiftIDS.RegisteredGifts.Keys.ToList());
        }

        public void loadDefaultBirthdayGifts()
        {

            if (File.Exists(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, "ModAssets", "Gifts", "DefaultGifts" + ".json")))
            {
                DefaultBirthdayGifts = HappyBirthday.ModHelper.Data.ReadJsonFile<List<GiftInformation>>(Path.Combine("ModAssets", "Gifts", "DefaultGifts" + ".json"));
            }
            else
            {
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    //Seeds, minerals, and cooked dishes are all acceptable gifts.
                    if (v.Value.Category == -2)
                    {
                        if (v.Value.salePrice() <= 400)
                        {
                            //Add in possible minerals and gems as a 4 heart gift
                            //Exclude prismatic shards and diamonds because that is a bit much.
                            DefaultBirthdayGifts.Add(new GiftInformation(v.Key, 4, 1, 1));
                        }
                    }
                    if (v.Value.Category == -7)
                    {
                        //Add in all possible food dishes as a 6 heart value
                        DefaultBirthdayGifts.Add(new GiftInformation(v.Key, 6, 1, 1));
                    }
                    //Add in seeds as a 2 heart gift
                    if (v.Value.Category == -74)
                    {
                        int seedPrice = v.Value.salePrice();
                        if (seedPrice < 500)
                        {
                            int stackAmount = 0;
                            if (seedPrice < 100)
                            {
                                //Get 5 generic seeds
                                stackAmount = 5;
                            }
                            else
                            {
                                //Can get 2 rare seeds such as starfruit and well rare seeds.
                                stackAmount = 2;
                            }
                            if (v.Value.ParentSheetIndex == 499)
                            {
                                stackAmount = 1; //Prevent ancient fruit from giving more than 1 seed.
                            }

                            DefaultBirthdayGifts.Add(new GiftInformation(v.Key, 0, stackAmount, stackAmount));
                        }
                        else
                        {
                            //Dont add sapplings in as a gift.
                        }
                    }
                }
                HappyBirthday.ModHelper.Data.WriteJsonFile<List<GiftInformation>>(Path.Combine("ModAssets", "Gifts", "DefaultGifts" + ".json"), DefaultBirthdayGifts);
            }
        }

        /// <summary>Load birthday gift information from disk. Preferably from BirthdayGift.json in the mod's directory.</summary>
        public void loadVillagerBirthdayGifts()
        {
            Directory.CreateDirectory(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, "ModAssets", "Gifts"));
            string[] files = Directory.GetFiles(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, "ModAssets", "Gifts"));
            foreach (string File in files)
            {
                try
                {
                    NPCBirthdayGifts.Add(Path.GetFileNameWithoutExtension(File), HappyBirthday.ModHelper.Data.ReadJsonFile<List<GiftInformation>>(Path.Combine("ModAssets", "Gifts", Path.GetFileNameWithoutExtension(File) + ".json")));
                    HappyBirthday.ModMonitor.Log("Loaded in gifts for npc: " + Path.GetFileNameWithoutExtension(File));
                }
                catch(Exception err)
                {
                    
                }
            }
        }

        /// <summary>Used to load spouse birthday gifts from disk.</summary>
        public void loadSpouseBirthdayGifts()
        {
            Directory.CreateDirectory(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, "ModAssets", "Gifts", "Spouses"));
            string[] files = Directory.GetFiles(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, "ModAssets", "Gifts", "Spouses"));
            foreach (string File in files)
            {
                SpouseBirthdayGifts.Add(Path.GetFileNameWithoutExtension(File), HappyBirthday.ModHelper.Data.ReadJsonFile<List<GiftInformation>>(Path.Combine("ModAssets", "Gifts", "Spouses", Path.GetFileNameWithoutExtension(File) + ".json")));
                HappyBirthday.ModMonitor.Log("Loaded in spouse gifts for npc: " + Path.GetFileNameWithoutExtension(File));
            }
        }


        public void createNPCBirthdayGifts()
        {
            if (NPCBirthdayGifts.ContainsKey("Alex") == false)
            {
                NPCBirthdayGifts.Add("Alex", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Egg,0,4,4),
                    new GiftInformation(GiftIDS.SDVObject.SalmonDinner,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.CompleteBreakfast,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.PurpleMushroom,0,1,1)

                });
            }

            if (NPCBirthdayGifts.ContainsKey("Elliott") == false)
            {
                NPCBirthdayGifts.Add("Elliott", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.CrabCakes,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.DuckFeather,0,1,1),
                });

                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    if (v.Value.Category == -79)
                    {
                        NPCBirthdayGifts["Elliott"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                }
            }


            if (NPCBirthdayGifts.ContainsKey("Harvey") == false)
            {
                NPCBirthdayGifts.Add("Harvey", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.MuscleRemedy,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Coffee,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.SuperMeal,0,1,1),
                });
            }

            if (NPCBirthdayGifts.ContainsKey("Sam") == false)
            {
                NPCBirthdayGifts.Add("Sam", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Pizza,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.JojaCola,0,6,6),
                    new GiftInformation(GiftIDS.SDVObject.CherryBomb,0,4,4),
                });
            }

            if (NPCBirthdayGifts.ContainsKey("Sebastian") == false)
            {
                NPCBirthdayGifts.Add("Sebastian", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Quartz,0,2,2),
                    new GiftInformation(GiftIDS.SDVObject.BlueberryTart,234,1,1),
                    new GiftInformation(GiftIDS.SDVObject.CherryBomb,0,4,4),
                    new GiftInformation(GiftIDS.SDVObject.GhostCrystal,0,1,1),
                });
            }

            if (NPCBirthdayGifts.ContainsKey("Shane") == false)
            {
                NPCBirthdayGifts.Add("Shane", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Pizza,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Egg,0,6,6),
                    new GiftInformation(GiftIDS.SDVObject.JojaCola,0,6,6),
                    new GiftInformation(GiftIDS.SDVObject.Beer,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Milk,0,2,2),
                });
            }


            if (NPCBirthdayGifts.ContainsKey("Abigail") == false)
            {
                NPCBirthdayGifts.Add("Abigail", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Quartz,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.FireOpal,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Malachite,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.IceCream,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.PurpleMushroom,0,1,1),
                });
            }

            if (NPCBirthdayGifts.ContainsKey("Emily") == false)
            {
                NPCBirthdayGifts.Add("Emily", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Amethyst,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Ruby,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Topaz,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Aquamarine,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Cloth,0,1,1),
                });
            }

            if (NPCBirthdayGifts.ContainsKey("Haley") == false)
            {
                NPCBirthdayGifts.Add("Haley", new List<GiftInformation>());
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    //Haley gives flowers
                    if (v.Value.Category == -80)
                    {
                        NPCBirthdayGifts["Haley"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                }
            }

            if (NPCBirthdayGifts.ContainsKey("Leah") == false)
            {
                NPCBirthdayGifts.Add("Leah", new List<GiftInformation>() {
                    new GiftInformation( GiftIDS.SDVObject.Salad,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Wood,0,30,30),
                    new GiftInformation(GiftIDS.SDVObject.BlackberryCobbler,0,1,1)

                });
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    //Leah gives forged goods
                    if (v.Value.Category == -81)
                    {
                        NPCBirthdayGifts["Leah"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                }
            }

            if (NPCBirthdayGifts.ContainsKey("Maru") == false)
            {
                NPCBirthdayGifts.Add("Maru", new List<GiftInformation>() {
                    new GiftInformation( GiftIDS.SDVObject.IronBar,0,3,3),
                    new GiftInformation(GiftIDS.SDVObject.GoldBar,0,3,3),
                    new GiftInformation(GiftIDS.SDVObject.CopperBar,0,5,5),
                    new GiftInformation(GiftIDS.SDVObject.BatteryPack,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.IridiumSprinkler,6,1,1)

                });
            }

            if (NPCBirthdayGifts.ContainsKey("Penny") == false)
            {
                NPCBirthdayGifts.Add("Penny", new List<GiftInformation>() {

                    new GiftInformation(GiftIDS.SDVObject.Hashbrowns,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.CrispyBass,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.VegetableMedley,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.MixedSeeds,0,5,5),
                    new GiftInformation(GiftIDS.SDVObject.Sunflower,0,1,1)
                });
            }


            if (NPCBirthdayGifts.ContainsKey("Caroline") == false)
            {
                NPCBirthdayGifts.Add("Caroline", new List<GiftInformation>() {

                    new GiftInformation(GiftIDS.SDVObject.GreenTea,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.TeaLeaves,0,3,3),
                    new GiftInformation(GiftIDS.SDVObject.SummerSpangle,0,1,1)
                });
            }

            if (NPCBirthdayGifts.ContainsKey("Clint") == false)
            {
                NPCBirthdayGifts.Add("Clint", new List<GiftInformation>() {

                    new GiftInformation(GiftIDS.SDVObject.CopperBar,0,5,5),
                    new GiftInformation(GiftIDS.SDVObject.IronBar,0,3,3),
                    new GiftInformation(GiftIDS.SDVObject.GoldBar,0,3,3),
                    new GiftInformation(GiftIDS.SDVObject.IridiumBar,4,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Geode,0,5,5),
                    new GiftInformation(GiftIDS.SDVObject.FrozenGeode,0,3,3),
                    new GiftInformation(GiftIDS.SDVObject.MagmaGeode,0,2,2),
                    new GiftInformation(GiftIDS.SDVObject.OmniGeode,2,1,1),
                });
            }


            if (NPCBirthdayGifts.ContainsKey("Demetrius") == false)
            {
                NPCBirthdayGifts.Add("Demetrius", new List<GiftInformation>() {
                    new GiftInformation( GiftIDS.SDVObject.PurpleMushroom,0,2,2),
                    new GiftInformation( GiftIDS.SDVObject.RedMushroom,0,2,2),

                });
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    if (v.Value.Category == -79)
                    {
                        NPCBirthdayGifts["Demetrius"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                }
            }

            if (NPCBirthdayGifts.ContainsKey("Dwarf") == false)
            {
                NPCBirthdayGifts.Add("Dwarf", new List<GiftInformation>() {
                    new GiftInformation( GiftIDS.SDVObject.CherryBomb,0,4,4),
                    new GiftInformation( GiftIDS.SDVObject.Bomb,0,2,2),
                    new GiftInformation( GiftIDS.SDVObject.MegaBomb,0,1,1)

                });
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    if (v.Value.Category == -2)
                    {
                        if (v.Value.salePrice() <= 400)
                        {
                            //Add in possible minerals and gems as a 4 heart gift
                            //Exclude prismatic shards and diamonds because that is a bit much.
                            NPCBirthdayGifts["Dwarf"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                        }
                    }
                }
            }


            if (NPCBirthdayGifts.ContainsKey("Evelyn") == false)
            {
                NPCBirthdayGifts.Add("Evelyn", new List<GiftInformation>() {
                    new GiftInformation( GiftIDS.SDVObject.Cookie,0,3,3),

                });
            }

            if (NPCBirthdayGifts.ContainsKey("George") == false)
            {
                NPCBirthdayGifts.Add("George", new List<GiftInformation>() {
                    new GiftInformation( GiftIDS.SDVObject.CommonMushroom,0,1,1),
                    new GiftInformation( GiftIDS.SDVObject.FriedMushroom,0,1,1),
                    new GiftInformation( GiftIDS.SDVObject.PurpleMushroom,0,1,1),
                    new GiftInformation( GiftIDS.SDVObject.Morel,0,1,1),
                    new GiftInformation( GiftIDS.SDVObject.Truffle,0,1,1),
                    new GiftInformation( GiftIDS.SDVObject.SnowYam,0,1,1),

                });
            }


            if (NPCBirthdayGifts.ContainsKey("Gus") == false)
            {
                NPCBirthdayGifts.Add("Gus", new List<GiftInformation>());
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    if (v.Value.Category == -7)
                    {
                        if (v.Value.salePrice() >= 100)
                        {
                            //Gus will give a random food dish for the player's birthday that has some decent value to it.
                            NPCBirthdayGifts["Gus"].Add(new GiftInformation(v.Key, 0, 1, 1));
                        }

                    }
                }
            }


            if (NPCBirthdayGifts.ContainsKey("Jas") == false)
            {
                NPCBirthdayGifts.Add("Jas", new List<GiftInformation>());
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    //Jas gives flowers
                    if (v.Value.Category == -80)
                    {
                        NPCBirthdayGifts["Jas"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                }
            }

            if (NPCBirthdayGifts.ContainsKey("Jodi") == false)
            {
                NPCBirthdayGifts.Add("Jodi", new List<GiftInformation>() {
                    new GiftInformation(GiftIDS.SDVObject.BlueberryTart,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.BlackberryCobbler,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.PumpkinPie,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.RhubarbPie,0,1,1)

                });
            }

            if (NPCBirthdayGifts.ContainsKey("Kent") == false)
            {
                NPCBirthdayGifts.Add("Kent", new List<GiftInformation>() {
                    new GiftInformation(GiftIDS.SDVObject.RoastedHazelnuts,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.FiddleheadRisotto,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.PineTar,0,3,3),
                    new GiftInformation(GiftIDS.SDVObject.BrownEgg,0,6,6)

                });
            }

            if (NPCBirthdayGifts.ContainsKey("Krobus") == false)
            {
                NPCBirthdayGifts.Add("Krobus", new List<GiftInformation>() {
                    new GiftInformation(GiftIDS.SDVObject.SolarEssence,0,5,5),
                    new GiftInformation(GiftIDS.SDVObject.VoidEssence,0,5,5),
                    new GiftInformation(GiftIDS.SDVObject.VoidEgg,4,1,1),

                });
            }


            if (NPCBirthdayGifts.ContainsKey("Lewis") == false)
            {
                NPCBirthdayGifts.Add("Lewis", new List<GiftInformation>() {
                    new GiftInformation(GiftIDS.SDVObject.GreenTea,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.PepperPoppers,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.VegetableMedley,0,1,1),

                });
            }

            if (NPCBirthdayGifts.ContainsKey("Linus") == false)
            {
                NPCBirthdayGifts.Add("Linus", new List<GiftInformation>());
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    //Linus gives forged goods
                    if (v.Value.Category == -81)
                    {
                        NPCBirthdayGifts["Linus"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                }
            }

            if (NPCBirthdayGifts.ContainsKey("Marnie") == false)
            {
                NPCBirthdayGifts.Add("Marnie", new List<GiftInformation>());
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    //Marnie gives milk or cheese
                    if (v.Value.Category == -6)
                    {
                        NPCBirthdayGifts["Marnie"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                    if (v.Value.Category == -26)
                    {
                        NPCBirthdayGifts["Marnie"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                }
            }

            if (NPCBirthdayGifts.ContainsKey("Pam") == false)
            {
                NPCBirthdayGifts.Add("Pam", new List<GiftInformation>() {
                    new GiftInformation(GiftIDS.SDVObject.Mead,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.PaleAle,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Beer,0,1,1),

                });
            }

            if (NPCBirthdayGifts.ContainsKey("Pierre") == false)
            {
                NPCBirthdayGifts.Add("Pierre", new List<GiftInformation>() {
                    new GiftInformation(GiftIDS.SDVObject.QualityFertilizer,0,5,5),
                    new GiftInformation(GiftIDS.SDVObject.BasicFertilizer,0,10,10),
                });

                foreach(var v in GiftIDS.RegisteredGifts)
                {
                    if (v.Value.Category == -74)
                    {
                        int seedPrice = v.Value.salePrice();
                        if (seedPrice < 500)
                        {
                            int stackAmount = 0;
                            if (seedPrice < 100)
                            {
                                //Get 5 generic seeds
                                stackAmount = 5;
                            }
                            else
                            {
                                //Can get 2 rare seeds such as starfruit and well rare seeds.
                                stackAmount = 2;
                            }
                            if (v.Value.ParentSheetIndex == 499)
                            {
                                stackAmount = 1; //Prevent ancient fruit from giving more than 1 seed.
                            }

                            NPCBirthdayGifts["Pierre"].Add(new GiftInformation(v.Key, 0, stackAmount, stackAmount));
                        }
                        else
                        {
                            //Dont add sapplings in as a gift.
                        }
                    }
                }
            }

            if (NPCBirthdayGifts.ContainsKey("Robin") == false)
            {
                NPCBirthdayGifts.Add("Robin", new List<GiftInformation>() {
                    new GiftInformation(GiftIDS.SDVObject.Wood,0,50,50),
                    new GiftInformation(GiftIDS.SDVObject.Stone,0,50,50),
                    new GiftInformation(GiftIDS.SDVObject.Hardwood,0,20,20),

                });
            }

            if (NPCBirthdayGifts.ContainsKey("Sandy") == false)
            {
                NPCBirthdayGifts.Add("Sandy", new List<GiftInformation>() {
                    new GiftInformation(GiftIDS.SDVObject.Starfruit,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Milk,0,3,3)

                });
            }

            if (NPCBirthdayGifts.ContainsKey("Vincent") == false)
            {
                NPCBirthdayGifts.Add("Vincent", new List<GiftInformation>() {
                    new GiftInformation(GiftIDS.SDVObject.Snail,0,1,1),

                });
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    //Linus gives forged goods
                    if (v.Value.Category == -81)
                    {
                        NPCBirthdayGifts["Vincent"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                }
            }

            if (NPCBirthdayGifts.ContainsKey("Willy") == false)
            {
                NPCBirthdayGifts.Add("Willy", new List<GiftInformation>() {
                    new GiftInformation(GiftIDS.SDVObject.Bait,0,50,50),
                });
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    //Linus gives forged goods
                    if (v.Value.Category == -4)
                    {
                        if (v.Value.salePrice() <= 500 && v.Value.salePrice()>=150)
                        {
                            NPCBirthdayGifts["Willy"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                        }
                    }
                }
            }

            if (NPCBirthdayGifts.ContainsKey("Wizard") == false)
            {
                NPCBirthdayGifts.Add("Wizard", new List<GiftInformation>() {
                    new GiftInformation(GiftIDS.SDVObject.SolarEssence,0,5,5),
                    new GiftInformation(GiftIDS.SDVObject.VoidEssence,0,5,5),
                    new GiftInformation(GiftIDS.SDVObject.FireQuartz,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Obsidian,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.LifeElixir,0,1,1),
                });
            }

            foreach(var v in NPCBirthdayGifts)
            {
                HappyBirthday.ModHelper.Data.WriteJsonFile(Path.Combine("ModAssets", "Gifts", Path.GetFileNameWithoutExtension(v.Key) + ".json"),v.Value);
            }
        }

        public void createSpouseBirthdayGifts()
        {
            if (SpouseBirthdayGifts.ContainsKey("Alex") == false)
            {
                SpouseBirthdayGifts.Add("Alex", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Egg,0,4,4),
                    new GiftInformation(GiftIDS.SDVObject.SalmonDinner,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.CompleteBreakfast,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.PurpleMushroom,0,1,1)

                });
            }

            if (SpouseBirthdayGifts.ContainsKey("Elliott") == false)
            {
                SpouseBirthdayGifts.Add("Elliott", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.CrabCakes,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.DuckFeather,0,1,1),
                });

                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    if (v.Value.Category == -79)
                    {
                        SpouseBirthdayGifts["Elliott"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                }
            }


            if (SpouseBirthdayGifts.ContainsKey("Harvey") == false)
            {
                SpouseBirthdayGifts.Add("Harvey", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.MuscleRemedy,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Coffee,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.SuperMeal,0,1,1),
                });
            }

            if (SpouseBirthdayGifts.ContainsKey("Sam") == false)
            {
                SpouseBirthdayGifts.Add("Sam", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Pizza,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.JojaCola,0,6,6),
                    new GiftInformation(GiftIDS.SDVObject.CherryBomb,0,4,4),
                });
            }

            if (SpouseBirthdayGifts.ContainsKey("Sebastian") == false)
            {
                SpouseBirthdayGifts.Add("Sebastian", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Quartz,0,2,2),
                    new GiftInformation(GiftIDS.SDVObject.BlueberryTart,234,1,1),
                    new GiftInformation(GiftIDS.SDVObject.CherryBomb,0,4,4),
                    new GiftInformation(GiftIDS.SDVObject.GhostCrystal,0,1,1),
                });
            }

            if (SpouseBirthdayGifts.ContainsKey("Shane") == false)
            {
                SpouseBirthdayGifts.Add("Shane", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Pizza,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Egg,0,6,6),
                    new GiftInformation(GiftIDS.SDVObject.JojaCola,0,6,6),
                    new GiftInformation(GiftIDS.SDVObject.Beer,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Milk,0,2,2),
                });
            }


            if (SpouseBirthdayGifts.ContainsKey("Abigail") == false)
            {
                SpouseBirthdayGifts.Add("Abigail", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Quartz,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.FireOpal,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Malachite,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.IceCream,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.PurpleMushroom,0,1,1),
                });
            }

            if (SpouseBirthdayGifts.ContainsKey("Emily") == false)
            {
                SpouseBirthdayGifts.Add("Emily", new List<GiftInformation>()
                {
                    new GiftInformation(GiftIDS.SDVObject.Amethyst,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Ruby,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Topaz,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Aquamarine,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Cloth,0,1,1),
                });
            }

            if (SpouseBirthdayGifts.ContainsKey("Haley") == false)
            {
                SpouseBirthdayGifts.Add("Haley", new List<GiftInformation>());
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    //Haley gives flowers
                    if (v.Value.Category == -80)
                    {
                        SpouseBirthdayGifts["Haley"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                }
            }

            if (SpouseBirthdayGifts.ContainsKey("Leah") == false)
            {
                SpouseBirthdayGifts.Add("Leah", new List<GiftInformation>() {
                    new GiftInformation( GiftIDS.SDVObject.Salad,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.Wood,0,30,30),
                    new GiftInformation(GiftIDS.SDVObject.BlackberryCobbler,0,1,1)

                });
                foreach (var v in GiftIDS.RegisteredGifts)
                {
                    //Leah gives forged goods
                    if (v.Value.Category == -81)
                    {
                        SpouseBirthdayGifts["Leah"].Add(new GiftInformation(v.Key, 0, 20, 1, 1));
                    }
                }
            }

            if (SpouseBirthdayGifts.ContainsKey("Maru") == false)
            {
                SpouseBirthdayGifts.Add("Maru", new List<GiftInformation>() {
                    new GiftInformation( GiftIDS.SDVObject.IronBar,0,3,3),
                    new GiftInformation(GiftIDS.SDVObject.GoldBar,0,3,3),
                    new GiftInformation(GiftIDS.SDVObject.CopperBar,0,5,5),
                    new GiftInformation(GiftIDS.SDVObject.BatteryPack,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.IridiumSprinkler,6,1,1)

                });
            }

            if (SpouseBirthdayGifts.ContainsKey("Penny") == false)
            {
                SpouseBirthdayGifts.Add("Penny", new List<GiftInformation>() {

                    new GiftInformation(GiftIDS.SDVObject.Hashbrowns,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.CrispyBass,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.VegetableMedley,0,1,1),
                    new GiftInformation(GiftIDS.SDVObject.MixedSeeds,0,5,5),
                    new GiftInformation(GiftIDS.SDVObject.Sunflower,0,1,1)
                });
            }


            foreach (var v in SpouseBirthdayGifts)
            {
                HappyBirthday.ModHelper.Data.WriteJsonFile(Path.Combine("ModAssets", "Gifts","Spouses" ,Path.GetFileNameWithoutExtension(v.Key) + ".json"), v.Value);
            }
        }

        /// <summary>Set the next birthday gift the player will receive.</summary>
        /// <param name="name">The villager's name who's giving the gift.</param>
        /// <remarks>This returns gifts based on the speaker's heart level towards the player: neutral for 3-4, good for 5-6, and best for 7-10.</remarks>
        public void setNextBirthdayGift(string name)
        {
            /*
            Item gift;
            if (this.BirthdayGifts.Count > 0)
            {
                Random random = new Random();
                int index = random.Next(this.BirthdayGifts.Count);
                gift = this.BirthdayGifts[index];
                if (Game1.player.isInventoryFull())
                    Game1.createItemDebris(gift, Game1.player.getStandingPosition(), Game1.player.getDirection());
                else
                    this.BirthdayGiftToReceive = gift;
                return;
            }

            this.BirthdayGifts.AddRange(this.GetDefaultBirthdayGifts(name));

            Random rnd2 = new Random();
            int r2 = rnd2.Next(this.BirthdayGifts.Count);
            gift = this.BirthdayGifts.ElementAt(r2);
            //Attempt to balance sapplings from being too OP as a birthday gift.
            if (gift.Name.Contains("Sapling"))
            {
                gift.Stack = 1; //A good investment?
            }
            if (gift.Name.Contains("Rare Seed"))
            {
                gift.Stack = 2; //Still a little op but less so than 5.
            }

            if (Game1.player.isInventoryFull())
                Game1.createItemDebris(gift, Game1.player.getStandingPosition(), Game1.player.getDirection());
            else
                this.BirthdayGiftToReceive = gift;

            this.BirthdayGifts.Clear();
            */

            if (Game1.player.friendshipData.ContainsKey(name))
            {
                if (Game1.player.getSpouse() != null)
                {
                    if (Game1.player.getSpouse().Name.Equals(name))
                    {
                        //Get spouse gift here
                        this.getSpouseBirthdayGift(name);
                    }
                    else
                    {
                        this.getNonSpouseBirthdayGift(name);
                    }
                }
                else
                {
                    if (NPCBirthdayGifts.ContainsKey(name))
                    {
                        this.getNonSpouseBirthdayGift(name);
                    }
                    else
                    {
                        this.getDefaultBirthdayGift(name);

                    }
                }

            }
            else
            {
                if (NPCBirthdayGifts.ContainsKey(name))
                {
                    this.getNonSpouseBirthdayGift(name);
                }
                else
                {
                    this.getDefaultBirthdayGift(name);
                }
            }


        }

        /// <summary>
        /// Tries to get a default spouse birthday gift.
        /// </summary>
        /// <param name="name"></param>
        public void getNonSpouseBirthdayGift(string name)
        {
            int heartLevel = Game1.player.getFriendshipHeartLevelForNPC(name);


            List<Item> possibleItems = new List<Item>();
            if (NPCBirthdayGifts.ContainsKey(name))
            {
                List<GiftInformation> npcPossibleGifts = NPCBirthdayGifts[name];
                foreach (GiftInformation info in npcPossibleGifts)
                {
                    if (info.minRequiredHearts <= heartLevel && heartLevel<=info.maxRequiredHearts)
                    {
                        possibleItems.Add(info.getOne());
                    }
                }

                Item gift;
                int index = StardewValley.Game1.random.Next(possibleItems.Count);
                gift = possibleItems[index];
                if (Game1.player.isInventoryFull())
                    Game1.createItemDebris(gift, Game1.player.getStandingPosition(), Game1.player.getDirection());
                else
                    this.BirthdayGiftToReceive = gift;
            }
            else
            {

                this.getDefaultBirthdayGift(name);

            }

        }


        /// <summary>
        /// Tries to get a spouse birthday gift.
        /// </summary>
        /// <param name="name"></param>
        public void getSpouseBirthdayGift(string name)
        {

            if (string.IsNullOrEmpty(HappyBirthday.Instance.PlayerData.favoriteBirthdayGift) == false)
            {
                Item I = GiftIDS.RegisteredGifts[HappyBirthday.Instance.PlayerData.favoriteBirthdayGift];
                if (Game1.player.isInventoryFull())
                    Game1.createItemDebris(I.getOne(), Game1.player.getStandingPosition(), Game1.player.getDirection());
                else
                    this.BirthdayGiftToReceive = I.getOne();
                return;
            }


            if (string.IsNullOrEmpty(HappyBirthday.PlayerBirthdayData.favoriteBirthdayGift) == false)
            {
                if (GiftIDS.RegisteredGifts.ContainsKey(HappyBirthday.PlayerBirthdayData.favoriteBirthdayGift))
                {
                    GiftInformation info=new GiftInformation(HappyBirthday.PlayerBirthdayData.favoriteBirthdayGift, 0, 1, 1);
                    if (Game1.player.isInventoryFull())
                        Game1.createItemDebris(info.getOne(), Game1.player.getStandingPosition(), Game1.player.getDirection());
                    else
                        this.BirthdayGiftToReceive = info.getOne();
                }

                return;
            }


            int heartLevel = Game1.player.getFriendshipHeartLevelForNPC(name);


            List<Item> possibleItems = new List<Item>();
            if (SpouseBirthdayGifts.ContainsKey(name))
            {
                List<GiftInformation> npcPossibleGifts = SpouseBirthdayGifts[name];
                foreach (GiftInformation info in npcPossibleGifts)
                {
                    if (info.minRequiredHearts <= heartLevel && heartLevel<=info.maxRequiredHearts)
                    {
                        possibleItems.Add(info.getOne());
                    }
                }

                Item gift;
                int index = StardewValley.Game1.random.Next(possibleItems.Count);
                gift = possibleItems[index];
                if (Game1.player.isInventoryFull())
                    Game1.createItemDebris(gift, Game1.player.getStandingPosition(), Game1.player.getDirection());
                else
                    this.BirthdayGiftToReceive = gift;
            }
            else
            {

                this.getNonSpouseBirthdayGift(name);


            }

        }

        /// <summary>
        /// Tries to get a default birthday gift.
        /// </summary>
        /// <param name="name"></param>
        public void getDefaultBirthdayGift(string name)
        {
            int heartLevel = Game1.player.getFriendshipHeartLevelForNPC(name);

            List<Item> possibleItems = new List<Item>();

            List<GiftInformation> npcPossibleGifts = DefaultBirthdayGifts;
            foreach (GiftInformation info in npcPossibleGifts)
            {
                if (info.minRequiredHearts <= heartLevel && heartLevel <= info.maxRequiredHearts)
                {
                    possibleItems.Add(info.getOne());
                }
            }

            //Should have atleast 1 default birthday gift!!!!

            Item gift;
            int index = StardewValley.Game1.random.Next(possibleItems.Count);
            gift = possibleItems[index];
            if (Game1.player.isInventoryFull())
                Game1.createItemDebris(gift, Game1.player.getStandingPosition(), Game1.player.getDirection());
            else
                this.BirthdayGiftToReceive = gift;

        }
    }

}
