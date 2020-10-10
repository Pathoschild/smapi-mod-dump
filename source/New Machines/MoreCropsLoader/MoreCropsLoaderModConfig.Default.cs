/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data;

namespace Igorious.StardewValley.MoreCropsLoader
{
    partial class MoreCropsLoaderModConfig
    {
        public override void CreateDefaultConfiguration()
        {
            Crops = new Dictionary<int, string>
            {
                {745, "2 1 3 3 3/spring/36/400/4/0/true 1 1 5 .02/false/false"},
                {792, "2 2 3 3 3/spring/41/378/-1/0/true 2 4 8 .2/false/false"},
                {793, "3 3 3 3 3/summer/42/380/-1/0/true 2 4 8 .2/false/false"},
                {794, "4 4 4 4 4/fall/43/380/-1/0/true 2 4 8 .2/false/false"},
                {795, "2 2 2 2 2/spring summer/63/382/-1/0/true 2 4 8 .2/false/false"},
                {796, "5 5 5 5 5/winter/44/386/-1/0/true 2 4 8 .2/false/false"},
                {811, "2 3 3 4/spring/45/809/-1/0/true 1 2 8 .2/false/false"},
                {814, "1 1 1 1/summer/47/813/-1/0/true 1 3 8 .2/false/false"},
                {816, "2 2 2 2 2/fall/48/815/10/0/true 1 3 8 .2/false/false"},
                {821, "3 3 3 3 2/fall/49/820/14/0/true 1 3 8 .2/false/false"},
                {823, "2 2 2 2 3/spring/50/822/11/0/true 1 3 8 .2/false/false"},
                {831, "2 2 3 3 3/fall/56/826/13/0/true 2 3 8 .2/false/false"},
                {833, "1 1 2 2 2/spring/52/832/8/0/true 2 3 8 .2/false/false"},
                {835, "2 2 2 2 3/spring/53/834/11/0/true 1 2 8 .2/false/false"},
                {837, "1 1 1 1 2/summer fall/54/836/6/0/true 1 3 8 .2/false/false"},
                {838, "1 1 1 1 1/spring summer/62/839/10/0/true 10 25 8 .2/false/false"},
                {845, "1 1 2 2 2/fall/59/844/-1/0/true 1 3 8 .5/false/false"},
                {846, "1 1 1 1 1/winter/64/245/-1/0/true 1 3 8 .2/false/false"},
                {847, "1 1 1 1 1/winter/65/246/-1/0/true 1 3 8 .2/false/false"},
                {848, "1 1 1 1 1/winter/57/247/-1/0/true 1 3 8 .2/false/false"},
                {849, "1 1 1 1 1/winter/58/419/-1/0/true 1 3 8 .2/false/false"},
                {858, "1 1 1 2 2/summer/55/830/-1/0/true 1 2 8 .6/false/false"},
                {859, "2 2 2 2/fall/51/824/-1/0/true 1 3 8 .2/false/false"},
                {860, "1 1 1 1 2/fall/40/811/-1/0/true 1 3 8 .2/false/false"},
                {863, "1 1 2 2/spring/60/861/-1/0/true 1 2 8 .2/false/false"},
                {866, "1 2 2 2/summer/61/864/-1/0/true 1 2 8 .2/false/false"},
                {867, "1 1 1 1 1/spring summer/46/802/5/0/true 2 3 8 .2/false/false"},
                {871, "2 2 2 2 2/summer/67/870/-1/0/true 2 3 8 .2/false/false"},
                {872, "1 1 2 2 2/spring/66/869/8/0/true 1 3 8 .2/false/false"},
                {874, "1 1 2 2/fall/68/873/6/0/true 1 3 8 .2/false/false"},
            }.Select(kv => CropInformation.Parse(kv.Value, kv.Key)).ToList();
            Crops.ForEach(c =>
            {
                if (c.TextureIndex >= 40) c.ResourceIndex = c.TextureIndex - 40;
            });

            Items = new Dictionary<int, string>
            {
                {792, "Copper Seeds/20/-300/Seeds -74/Plant these to grow copper ore.Grows only in spring. Ore in 13 days."},
                {793, "Iron Seeds/30/-300/Seeds -74/Plant these to grow iron ore. Grows only in summer. Ore in 15 days."},
                {794, "Gold Seeds/40/-300/Seeds -74/Plant these to grow gold ore. Grows only in fall. Ore in 20 days."},
                {795, "Coal Seeds/10/-300/Seeds -74/Plant these to grow coal ore. Grows in spring and summer. Ore in 10 days."},
                {796, "Iridium Seeds/100/-300/Seeds -74/Plant these to grow iridium ore. Grows only inside the greenhouse. Ore in 25 days."},
                {797, "Copper Sapling/500/-300/Basic -74/Takes 28 days to produce a copper tree. Bears ore in the spring. Only grows if the 8 surrounding \"tiles\" are empty."},
                {798, "Iron Sapling/600/-300/Basic -74/Takes 28 days to produce a iron tree. Bears ore in the summer. Only grows if the 8 surrounding \"tiles\" are empty."},
                {799, "Gold Sapling/800/-300/Basic -74/Takes 28 days to produce a gold tree. Bears ore in the fall. Only grows if the 8 surrounding \"tiles\" are empty."},
                {800, "Coal Sapling/500/-300/Basic -74/Takes 28 days to produce a coal tree. Bears ore in the spring. Only grows if the 8 surrounding \"tiles\" are empty."},
                {801, "Iridium Sapling/500/-300/Basic -74/Takes 28 days to produce a iridium tree. Only grows if the 8 surrounding \"tiles\" are empty."},
                {802, "Beef/80/-300/Seeds -74/Not a good idea to eat it raw. Plant it or cook it!(problably in hot weather)."},
                {803, "Bacon/70/10/Basic -5/BACON! Maybe it's magical, maybe you should cook it."},
                {804, "Cooked Beef/150/64/Cooking -7/No vegans allowed./food/0 0 0 0 0 0 0 40 0 1 0/300"},
                {805, "Cooked Bacon/200/64/Cooking -7/To hell with eating healthy./food/0 0 0 0 0 0 0 40 0 1 0/300"},
                {806, "Bacon Sapling/60/-300/Basic -74/Takes 28 days to grow into a bacon tree. Yeah a BACON TREE!! Bears BACON in the summer. Only grows if the 8 surrounding \"tiles\" are empty."},
                {807, "Cocoa Sapling/30/-300/Basic -74/Takes 28 days to produce a cocoa tree. Bears fruit in the spring. Only grows if the 8 surrounding \"tiles\" are empty."},
                {808, "Cocoa/145/-300/Basic -75/Beans used to make famous treads."},
                {809, "Violet /70/-300/Basic -16/Beautiful purple thing, better not eat it."},
                {811, "Violet Seeds/10/-300/Seeds -74/Seeds to grow vioet flower. Grows in spring. Takes 8 days to produce flowers."},
                {812, "Olive/250/-300/Basic -79/Find some good cheese and a nice beer to eat these."},
                {813, "Lettuce /65/-300/Basic -75/Its round, its green.It is lettuce!."},
                {814, "Lettuce Seed/5/-300/Seeds -74/Grows in summer, 4 days."},
                {815, "Bell Pepper/90/-300/Basic -79/Not as spicy, still really delicious!"},
                {816, "Red Pepper Seeds/10/-300/Seeds -74/Plant these to grow red pepper. Grows only in fall. Pepper in 10 days."},
                {817, "Lemon/200/-300/Basic -79/The most sour of the sour fruits."},
                {818, "Papaya/183/-300/Basic -79/Sweet fruit used most to make deserts. Very healthy too!."},
                {819, "Papaya Sapling/12/-300/Basic -74/Takes 28 days to produce a papaya tree. Bears fruit in the summer. Only grows if the 8 surrounding \"tiles\" are empty."},
                {820, "Watermelon/175/-300/Basic -79/Man, this is heavier than I tought!"},
                {821, "Watermelon Seed/20/-300/Seeds -74/Grows in fall.Takes 14 days to produce."},
                {822, "Cannary Melon/120/-300/Basic -79/It's not that heavy..."},
                {823, "Cannary Melon Seed/6/-300/Seeds -74/Grows in spring.Takes 11 days to produce."},
                {824, "Onion/117/-300/Basic -79/Here comes the bad breath! Grows in fall,8 days"},
                {825, "Banana/245/-300/Basic -79/If you happen to find a monkey you can feed him this!"},
                {826, "Money/100/-300/Basic/Money makes the world spin."},
                {827, "Olive Sapling/15/-300/Basic -74/Takes 28 days to produce a olive tree. Bears fruit in the spring. Only grows if the 8 surrounding \"tiles\" are empty."},
                {828, "Lemon Sapling/15/-300/Basic -74/Takes 28 days to produce a lemon tree. Bears fruit in the spring. Only grows if the 8 surrounding \"tiles\" are empty."},
                {829, "Banana Sapling/15/-300/Basic -74/Takes 28 days to produce a banana tree. Bears fruit in the summer. Only grows if the 8 surrounding \"tiles\" are empty."},
                {830, "Cassava/80/-300/Basic -79/Grows in summer, 7 days. Not that famous, but still delicious when fried!"},
                {831, "Money Seeds/10/-300/Seeds -74/Plant these in fall. Takes 13 days to mature. Have you ever dreamed about doing this??"},
                {832, "Pineaple/83/-300/Basic -79/The king of the fruits!."},
                {833, "Pineaple Seeds/20/-300/Seeds -74/Plant these in spring. Takes 8 days to mature."},
                {834, "Cucumber /98/-300/Basic -75/ Don't you even think about doing it!"},
                {835, "Cucumber Seeds/5/-300/Seeds -74/Plant these in spring. Takes 11 days to mature."},
                {836, "Cotton/64/-300/Basic/Plant these soldier!"},
                {837, "Cotton Seeds/30/-300/Seeds -74/Plant these in summer and fall. Takes 6 days to produce."},
                {838, "Rice Seeds/8/-300/Seeds -74/Plant these in spring and summer. Takes 5 days to produce."},
                {839, "Rice/7/-300/Basic -75/It is rainning rice!"},
                {840, "Dragonfruit/160/-300/Basic -79/A fruit that looks like a dragon egg"},
                {841, "Dragonfruit Sapling/12/-300/Basic -74/Takes 28 days to produce a dragonfruit tree. Bears fruit in the summer. Only grows if the 8 surrounding \"tiles\" are empty."},
                {842, "Avocado/170/-300/Basic -79/Bear Grills said this is the best fruit..."},
                {843, "Avocado Sapling/14/-300/Basic -74/Takes 28 days to produce an avocado tree. Bears fruit in the summer. Only grows if the 8 surrounding \"tiles\" are empty."},
                {844, "Purple Sweet Potato/86/-300/Basic -79/Eat this with chicken to become a bodybuilder!"},
                {845, "Purple Sweet Potato Seeds/16/-300/Seeds -74/Plant these in fall. Takes 8 days to mature."},
                {846, "Sugar Seeds/11/-300/Seeds -74/Plant these in greenhouse. Takes 5 days to mature. Confusing but sweet..."},
                {847, "Flour Seeds/11/-300/Seeds -74/Plant these in the greenhouse. Takes 5 days to mature."},
                {848, "Oil Seeds/11/-300/Seeds -74/Plant these in the greenhouse. Takes 5 days to mature."},
                {849, "Vinegar Seeds/11/-300/Seeds -74/Plant these in the greenhouse. Takes 5 days to mature."},
                {850, "Fig/154/-300/Basic -79/A really swwet fruit."},
                {851, "Fig Sapling/35/-300/Basic -74/Takes 28 days to produce a fig tree. Bears fruit in the winter. Only grows if the 8 surrounding \"tiles\" are empty."},
                {852, "Pear/178/-300/Basic -79/The sexy fruit."},
                {853, "Pear Sapling/35/-300/Basic -74/Takes 28 days to produce a pear tree. Bears fruit in the winter. Only grows if the 8 surrounding \"tiles\" are empty."},
                {854, "Persimmon/166/-300/Basic -79/Not the same as tomato."},
                {855, "Persimmon Sapling/35/-300/Basic -74/Takes 28 days to produce a persimmon tree. Bears fruit in the winter. Only grows if the 8 surrounding \"tiles\" are empty."},
                {856, "Lime Sapling/35/-300/Basic -74/Takes 28 days to produce a lime tree. Bears fruit in the spring. Only grows if the 8 surrounding \"tiles\" are empty."},
                {857, "Lime/160/-300/Basic -79/A different colored lemon."},
                {858, "Cassava Seed/20/-300/Seeds -74/Grows in summer.Takes 7 days to mature."},
                {859, "Onion Seed/6/-300/Seeds -74/Grows in fall.Takes 8 days to produce."},
                {860, "Carrot Seed/6/-300/Seeds -74/Grows in fall.Takes 6 days to produce."},
                {861, "Pink Cat /100/-300/Basic -16/A plant native to the far off Mineral Town. It is known for its minty scent."},
                {863, "Pink Cat Seed/6/-300/Seeds -74/Grows in spring.Takes 6 days to produce."},
                {864, "Blue Mist /160/-300/Basic -16/Often a popular present for young ladies, these star-shaped blossoms bloom on the hillsides."},
                {866, "Blue Mist Seed/6/-300/Seeds -74/Grows in ssummer.Takes 7 days to produce."},
                {867, "Beef Seeds/3/-300/Seeds -74/Plant these to grow beef crop. Grows in summer and spring. Meat in 5 days."},
                {868, "Carrot/60/-300/Basic -79/Grows in fall, 6 days. Maybe you could try to feed it to a rabbit..."},
                {869, "Coffee Grains /75/-300/Basic -16/Need. To. Stay. Awoken."},
                {870, "Sugar Cane /100/-300/Basic -75/ Eat all the sugar!!"},
                {871, "Sugar Cane Seeds/11/-300/Seeds -74/Plant these in the summer. Takes 10 days to mature."},
                {872, "Coffee Seeds/11/-300/Seeds -74/Plant these in the spring. Takes 8 days to mature."},
                {873, "Peanut /60/-300/Basic -75/ Hard shell, delicious inside!"},
                {874, "Peanut Seeds/11/-300/Seeds -74/Plant these in the fall. Takes 6 days to mature."},
            }.Select(kv => ItemInformation.Parse(kv.Value, kv.Key)).ToList();
            Items.ForEach(c =>
            {
                if (c.ID >= 792) c.ResourceIndex = c.ID - 792;
            });

            Trees = new Dictionary<int, string>
            {
                {797, "1/spring/378/3000"},
                {798, "2/summer/380/3000"},
                {799, "3/fall/384/3000"},
                {800, "4/spring/382/3000"},
                {801, "0/winter/386/3000"},
                {806, "1/summer/803/3000"},
                {807, "2/spring/808/3000"},
                {819, "3/summer/818/3000"},
                {827, "4/spring/812/3000"},
                {828, "5/spring/817/3000"},
                {829, "6/summer/825/3000"},
                {841, "0/summer/840/3000"},
                {843, "1/winter/842/3000"},
                {851, "2/summer/850/3000"},
                {853, "3/winter/852/3000"},
                {855, "4/winter/854/3000"},
                {856, "0/spring/857/3000"},
            }.Select(kv => TreeInformation.Parse(kv.Value, kv.Key)).ToList();
        }


    }
}
