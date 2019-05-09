using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SadisticBundles
{
    public class BundleInjector : IAssetEditor
    {
        private IModHelper helper;
        private IMonitor monitor;

        public BundleInjector(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper;
            this.monitor = monitor;
            var newData = new Dictionary<string, string>();
            foreach (var kvp in mydata)
            {
                var parts = kvp.Value.Split('/');
                var name = helper.Translation.Get(parts[0]);
                if (name.HasValue())
                {
                    parts[0] = name;
                }
                else
                {
                    monitor.Log($"No translation for {parts[0]}", LogLevel.Warn);
                }
                newData[kvp.Key] = string.Join("/", parts);
            }
            mydata = newData;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/Bundles") && GameState.Current != null && GameState.Current.Activated)
            {
                return true;
            }
            return false;
        }

        // generated from spreadsheet https://docs.google.com/spreadsheets/d/1dLXiNOHzCFJL-q9IbMIO2Fv2Hk-PC5zklqHFYqOiUDA/edit?usp=sharing
        private Dictionary<string, string> mydata = new Dictionary<string, string>
        {
{"Crafts Room/0", "bundleDesc0/O 72 1/16 50 2 18 50 2 20 50 2 22 50 2 396 50 2 398 50 2 402 50 2 406 50 2 408 50 2 410 50 2 412 50 2 414 50 2/0"},
{"Crafts Room/1", "bundleDesc1/O 72 1/399 50 2 296 50 2 259 50 2 416 50 2 418 50 2 283 50 2 88 50 2 90 50 2 392 50 2/1"},
{"Crafts Room/2", "bundleDesc2/O 72 1/388 999 0 388 999 0 388 999 0 709 500 0 390 999 0 390 999 0 771 999 0 92 999 0 330 300 0 334 50 0/2"},
{"Crafts Room/3", "bundleDesc3/O 72 1/257 50 2 404 50 2 420 50 2 281 50 2 422 50 2 78 150 0/3"},
{"Crafts Room/4", "bundleDesc4/O 72 1/789 1 4 458 1 4 204 1 4 113 1 4 709 1 4 580 1 4 93 1 4 167 1 4 802 1 4 243 1 4 125 1 4/4"},
{"Crafts Room/5", "bundleDesc5/O 72 1/212 10 0 637 10 4 342 10 0 90 10 2 575 10 0 215 10 0 66 10 0 428 10 0 221 10 0 348 10 4 787 10 0 244 10 0/5"},
{"Pantry/6", "bundleDesc6/O 72 1/190 150 2 433 150 2 248 150 2 188 150 2 250 150 2 24 200 2 192 150 2 252 150 2 400 150 2 454 150 2 240 15 0/6"},
{"Pantry/7", "bundleDesc7/O 72 1/258 150 2 270 150 2 304 200 2 260 150 2 254 150 2 264 150 2 266 150 2 268 150 2 256 150 2 262 150 2/0"},
{"Pantry/8", "bundleDesc8/O 72 1/300 150 2 274 150 2 284 150 2 278 150 2 282 200 2 272 200 2 398 150 2 276 150 2 280 150 2 417 100 2/1"},
{"Pantry/9", "bundleDesc9/O 72 1/634 60 4 613 60 4 635 60 4 636 60 4 637 60 4 638 60 4 726 50 0 725 50 0 724 50 0 309 50 0 310 50 0 311 50 0/2"},
{"Pantry/10", "bundleDesc10/O 72 1/174 50 4 182 50 4 186 50 4 438 50 4 430 50 2 440 50 4 442 50 4 305 50 4 444 30 2 446 15 2/3"},
{"Pantry/11", "bundleDesc11/O 72 1/424 50 4 426 50 4 306 50 2 307 50 2 308 50 2 428 50 0 340 100 0 342 100 0 344 100 0/4"},
{"Bulletin Board/12", "bundleDesc12/O 72 1/201 50 0 211 50 0 215 50 0 226 50 0 237 50 0 732 50 0 730 50 0 610 50 0 231 50 0 236 50 0 208 50 0 195 50 0/5"},
{"Bulletin Board/13", "bundleDesc13/O 72 1/234 50 0 220 50 0 221 50 0 608 50 0 222 50 0 203 50 0 216 50 0 223 50 0 611 50 0 651 50 0 731 50 0 206 50 0/6"},
{"Bulletin Board/14", "bundleDesc14/O 72 1/597 50 2 591 50 2 376 50 2 593 50 2 421 100 2 595 100 2 458 20 0 591 50 2 376 50 2 593 50 2 597 50 2 340 100 0/0"},
{"Bulletin Board/15", "bundleDesc15/O 72 1/769 50 0 305 100 0 308 100 0 795 100 0 103 1 0 109 1 0 123 1 0 126 1 0 107 1 0/1"},
{"Bulletin Board/16", "bundleDesc16/O 72 1/74 2 0 373 3 0 394 30 2 397 30 2 422 30 2 416 30 2 243 30 0 233 30 0 230 30 0 282 100 1 635 100 1/2"},
{"Bulletin Board/17", "bundleDesc17/O 72 1/348 100 4 303 100 4 346 100 4 459 100 4 350 100 0 395 100 0 432 50 0 247 50 0 772 50 0 773 15 0 349 40 0 351 40 0/3"},
{"Boiler Room/18", "bundleDesc18/O 72 1/334 200 0 335 200 0 336 200 0 337 100 0 338 200 0 535 50 0 536 50 0 537 50 0 749 50 0/4"},
{"Boiler Room/19", "bundleDesc19/O 72 1/684 200 0 766 200 0 767 200 0 768 100 0 769 100 0 437 1 0 413 1 0 680 1 0 439 1 0/5"},
{"Boiler Room/20", "bundleDesc20/O 72 1/72 30 0 60 30 0 66 30 0 68 30 0 64 30 0 62 30 0 70 30 0 562 30 0 82 30 0 84 30 0 797 3 0 86 30 0/6"},
{"Boiler Room/21", "bundleDesc21/O 72 1/599 20 0 621 20 0 645 20 0 286 100 0 287 100 0 288 100 0 527 2 0 787 20 0/0"},
{"Boiler Room/22", "bundleDesc22/O 72 1/168 50 0 169 50 0 172 50 0 171 50 0 170 50 0 167 50 0 748 50 0/1"},
{"Boiler Room/23", "bundleDesc23/O 72 1/101 1 0 104 1 0 106 1 0 108 1 0 112 1 0 113 1 0 114 1 0 115 1 0 122 1 0 124 1 0 586 1 0 125 1 0/2"},
{"Fish Tank/24", "bundleDesc24/O 72 1/142 15 2 147 15 2 137 15 2 129 15 2 131 15 2 145 15 2 702 15 2 141 15 2 132 15 2 150 15 2 154 15 2 138 15 2/3"},
{"Fish Tank/25", "bundleDesc25/O 72 1/140 15 2 706 15 2 700 15 2 136 15 2 139 15 2 156 15 2 701 15 2 734 15 1 708 15 2 796 15 2 146 15 2 144 15 2/4"},
{"Fish Tank/26", "bundleDesc26/O 72 1/699 15 2 705 15 2 164 15 1 158 15 2 130 15 2 148 15 2 143 15 2 151 15 2 698 15 2 704 15 2 128 15 2 795 15 2/5"},
{"Fish Tank/27", "bundleDesc27/O 72 1/715 15 0 372 15 2 716 15 0 717 15 0 718 15 2 719 15 2 720 15 0 721 15 0 722 15 0 723 15 2 152 25 0 153 25 0/6"},
{"Fish Tank/28", "bundleDesc28/O 72 1/155 10 2 161 10 2 707 10 2 165 10 2 162 10 2 149 10 2 227 100 0/0"},
{"Fish Tank/29", "bundleDesc29/O 72 1/159 1 0 160 1 0 163 1 0 775 1 0 682 1 0 798 10 1 799 10 1 800 10 1/1"},
{"Vault/30", "bundleDesc30/O 72 1/-1 1000000 1000000/2"},
{"Vault/31", "bundleDesc31/O 72 1/-1 2000000 2000000/3"},
{"Vault/32", "bundleDesc32/O 72 1/-1 5000000 5000000/4"},
{"Vault/33", "bundleDesc33/O 72 1/-1 10000000 10000000/5"},
{"Bulletin Board/34", "bundleDesc34/O 72 1/395 999 0 395 999 0 395 999 0 395 999 0/6"},
        };

        public IList<Item> GetItems(int id)
        {
            var l = new List<Item>();
            var row = mydata.Single(x => x.Key.EndsWith($"/{id}")).Value;
            var items = row.Split('/')[2].Split(' ');
            for(var i = 0; i<items.Length; i += 3)
            {
                var it = new StardewValley.Object(int.Parse(items[i]), int.Parse(items[i + 1]), quality: int.Parse(items[i + 2]));
                l.Add(it);
            }
            return l;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/Bundles"))
            {
                asset.AsDictionary<string, string>().ReplaceWith(mydata);
                FixBundles(false);
            }
        }

        public void FixBundles(bool hard)
        {
            // this makes the "shape" of the game's internal bundle tracking representation match the bundles we have.
            var bundles = Game1.netWorldState.Value.Bundles;
            var rewards = Game1.netWorldState.Value.BundleRewards;
            if (hard)
            {
                bundles.Clear();
                rewards.Clear();
            }
            // remove anything in those collections we don't have a bun
            var desiredIds = mydata.Select(x => int.Parse(x.Key.Split('/')[1])).ToList();
            foreach (var nuke in bundles.Keys.Where(x => !desiredIds.Contains(x)))
            {
                bundles.Remove(nuke);
            }
            foreach (var nuke in rewards.Keys.Where(x => !desiredIds.Contains(x)))
            {
                rewards.Remove(nuke);
            }
            // now add anything not in existing collections. 
            // making sure to adjust ingredient counts as needed (so we can ever update mod).
            foreach (var kvp in mydata)
            {
                var id = int.Parse(kvp.Key.Split('/')[1]);
                var ings = kvp.Value.Split('/')[2].Split(' ');
                var count = ings.Count() / 3;
                if (!bundles.ContainsKey(id))
                {
                    bundles[id] = new bool[count];
                }
                if (count != bundles[id].Count())
                {
                    var b = new bool[count];
                    var old = bundles[id];
                    for (var i = 0; i < old.Length && i < count; i++)
                    {
                        b[i] = old[i];
                    }
                    bundles.Remove(id);
                    bundles[id] = b;
                }
                if (!rewards.ContainsKey(id))
                {
                    rewards[id] = false;
                }
            }
        }
    }
}
