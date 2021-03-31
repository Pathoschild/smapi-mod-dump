/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Let-Me-Rest
**
*************************************************/

using System.Collections.Generic;

namespace LetMeRest.Framework.Lists
{
    public class DataBase
    {
        public static Dictionary<string, float> ItemDataBase;

        public static void GetDataBase()
        {
            Dictionary<string, float> holder = new Dictionary<string, float>();

            //===== FURNITURE =====
            //BEDS
            holder.Add("Child Bed", 0.05f);
            holder.Add("Bed", 0.05f);
            holder.Add("Double Bed", 0.065f);
            holder.Add("Birch Double Bed", 0.08f);
            holder.Add("Deluxe Red Double Bed", 0.08f);
            holder.Add("Exotic Double Bed", 0.01f);
            holder.Add("Fisher Double Bed", 0.01f);
            holder.Add("Modern Double Bed", 0.075f);
            holder.Add("Pirate Double Bed", 0.07f);
            holder.Add("Starry Double Bed", 0.07f);
            holder.Add("Strawberry Double Bed", 0.07f);
            holder.Add("Tropical Bed", 0.055f);
            holder.Add("Tropical Double Bed", 0.07f);
            holder.Add("Wild Double Bed", 0.075f);

            //CHAIRS
            holder.Add("Oak Chair", 0.02f);
            holder.Add("Walnut Chair", 0.02f);
            holder.Add("Birch Chair", 0.02f);
            holder.Add("Mahogany Chair", 0.045f);
            holder.Add("Red Diner Chair", 0.03f);
            holder.Add("Blue Diner Chair", 0.03f);
            holder.Add("Country Chair", 0.03f);
            holder.Add("Breakfast Chair", 0.03f);
            holder.Add("Pink Office Chair", 0.025f);
            holder.Add("Purple Office Chair", 0.025f);
            holder.Add("Green Office Stool", 0.02f);
            holder.Add("Orange Office Stool", 0.02f);
            holder.Add("Dark Throne", 0.065f);
            holder.Add("Dining Chair (yellow)", 0.05f);
            holder.Add("Dining Chair (red)", 0.05f);
            holder.Add("Dining Chair", 0.05f);
            holder.Add("Green Plush Seat", 0.03f);
            holder.Add("Pink Plush Seat", 0.03f);
            holder.Add("Winter Chair", 0.03f);
            holder.Add("Groovy Chair", 0.03f);
            holder.Add("Cute Chair", 0.05f);
            holder.Add("Stump Seat", 0.065f);
            holder.Add("Metal Chair", 0.035f);
            holder.Add("Green Stool", 0.02f);
            holder.Add("Blue Stool", 0.02f);
            holder.Add("King Chair", 0.08f);
            holder.Add("Crystal Chair", 0.1f);
            holder.Add("Tropical Chair", 0.06f);

            //BENCHES
            holder.Add("Birch Bench", 0.035f);
            holder.Add("Oak Bench", 0.035f);
            holder.Add("Walnut Bench", 0.035f);
            holder.Add("Mahogany Bench", 0.07f);
            holder.Add("Modern Bench", 0.07f);

            //COUCHES
            holder.Add("Blue Couch", 0.06f);
            holder.Add("Brown Couch", 0.06f);
            holder.Add("Green Couch", 0.06f);
            holder.Add("Red Couch", 0.06f);
            holder.Add("Yellow Couch", 0.06f);
            holder.Add("Dark Couch", 0.075f);
            holder.Add("Woodsy Couch", 0.09f);
            holder.Add("Wizard Couch", 0.1f);
            holder.Add("Large Brown Couch", 0.09f);
            holder.Add("Blue Armchair", 0.045f);
            holder.Add("Brown Armchair", 0.045f);
            holder.Add("Green Armchair", 0.045f);
            holder.Add("Red Armchair", 0.045f);
            holder.Add("Yellow Armchair", 0.045f);

            //TABLES
            holder.Add("Oak Table", 0.03f);
            holder.Add("Oak Tea-Table", 0.03f);
            holder.Add("Oak End Table", 0.025f);
            holder.Add("Birch Table", 0.03f);
            holder.Add("Birch Tea-Table", 0.03f);
            holder.Add("Birch End Table", 0.025f);
            holder.Add("Mahogany Table", 0.07f);
            holder.Add("Mahogany Tea-Table", 0.07f);
            holder.Add("Mahogany End Table", 0.05f);
            holder.Add("Walnut Table", 0.03f);
            holder.Add("Walnut Tea-Table", 0.03f);
            holder.Add("Walnut End Table", 0.025f);
            holder.Add("Modern Table", 0.06f);
            holder.Add("Modern Tea-Table", 0.05f);
            holder.Add("Modern End Table", 0.045f);
            holder.Add("Puzzle Table", 0.07f);
            holder.Add("Moon Table", 0.1f);
            holder.Add("Luxury Table", 0.08f);
            holder.Add("Diviner Table", 0.09f);
            holder.Add("Grandmother End Table", 0.05f);
            holder.Add("Pub Table", 0.045f);
            holder.Add("Luau Table", 0.05f);
            holder.Add("Dark Table", 0.08f);
            holder.Add("Candy Table", 0.05f);
            holder.Add("Sun Table", 0.1f);
            holder.Add("Winter Table", 0.06f);
            holder.Add("Winter End Table", 0.05f);
            holder.Add("Neolithic Table", 0.075f);
            holder.Add("Coffee Table", 0.06f);
            holder.Add("Stone Slab", 0.05f);

            //LONG TABLES
            holder.Add("Modern Dining Table", 0.075f);
            holder.Add("Mahogany Dining Table", 0.085f);
            holder.Add("Festive Dining Table", 0.1f);
            holder.Add("Winter Dining Table", 0.1f);

            //BOOK AND DRESSERS
            holder.Add("Artist Bookcase", 0.04f);
            holder.Add("Modern Bookcase", 0.05f);
            holder.Add("Luxury Bookcase", 0.06f);
            holder.Add("Dark Bookcase", 0.06f);
            holder.Add("Birch Dresser", 0.085f);
            holder.Add("Oak Dresser", 0.085f);
            holder.Add("Walnut Dresser", 0.085f);
            holder.Add("Mahogany Dresser", 0.1f);

            //FIREPLACES
            holder.Add("Brick Fireplace", 0.035f);
            holder.Add("Elegant Fireplace", 0.085f);
            holder.Add("Iridium Fireplace", 0.085f);
            holder.Add("Monster Fireplace", 0.1f);
            holder.Add("Stone Fireplace", 0.04f);
            holder.Add("Stove Fireplace", 0.055f);

            //RUGS
            holder.Add("Bamboo Mat", 0.02f);
            holder.Add("Burlap Rug", 0.025f);
            holder.Add("Woodcut Rug", 0.045f);
            holder.Add("Nautical Rug", 0.06f);
            holder.Add("Dark Rug", 0.075f);
            holder.Add("Red Rug", 0.06f);
            holder.Add("Large Red Rug", 0.06f);
            holder.Add("Monster Rug", 0.025f);
            holder.Add("Light Green Rug", 0.06f);
            holder.Add("Blossom Rug", 0.025f);
            holder.Add("Large Green Rug", 0.075f);
            holder.Add("Old World Rug", 0.075f);
            holder.Add("Large Cottage Rug", 0.07f);
            holder.Add("Green Cottage Rug", 0.025f);
            holder.Add("Red Cottage Rug", 0.025f);
            holder.Add("Mystic Rug", 0.025f);
            holder.Add("Bonec Rug", 0.025f);
            holder.Add("Snowy Rug", 0.025f);
            holder.Add("Pirate Rug", 0.025f);
            holder.Add("Patchwork Rug", 0.045f);
            holder.Add("Fruit Salad Rug", 0.035f);
            holder.Add("Oceanic Rug", 0.035f);
            holder.Add("Icy Rug", 0.095f);
            holder.Add("Funky Rug", 0.095f);
            holder.Add("Modern Rug", 0.095f);
            holder.Add("Floor Dividers", 0.01f);

            //LAMPS AND WINDOWS
            holder.Add("Country Lamp", 0.05f);
            holder.Add("Modern Lamp", 0.07f);
            holder.Add("Classic Lamp", 0.09f);
            holder.Add("Box Lamp", 0.07f);
            holder.Add("Candle Lamp", 0.09f);
            holder.Add("Ornate Lamp", 0.095f);
            holder.Add("Basic Window", 0.04f);
            holder.Add("Small Window", 0.04f);
            holder.Add("Boarded Window", 0.045f);
            holder.Add("Carved Window", 0.08f);
            holder.Add("Metal Window", 0.075f);
            holder.Add("Ornate Window", 0.08f);
            holder.Add("Porthole", 0.065f);

            //TVS
            holder.Add("Floor TV", 0.065f);
            holder.Add("Budget TV", 0.07f);
            holder.Add("Plasma TV", 0.1f);
            holder.Add("Tropical TV", 0.1f);


            //===== DECORATION =====
            //HOUSE PLANTS
            holder.Add("House Plant 1", 0.015f);
            holder.Add("House Plant 2", 0.015f);
            holder.Add("House Plant 3", 0.015f);
            holder.Add("House Plant 4", 0.015f);
            holder.Add("House Plant 5", 0.015f);
            holder.Add("House Plant 6", 0.015f);
            holder.Add("House Plant 7", 0.015f);
            holder.Add("House Plant 8", 0.015f);
            holder.Add("House Plant 9", 0.015f);
            holder.Add("House Plant 10", 0.015f);
            holder.Add("House Plant 11", 0.015f);
            holder.Add("House Plant 12", 0.015f);
            holder.Add("House Plant 13", 0.015f);
            holder.Add("House Plant 14", 0.015f);
            holder.Add("House Plant 15", 0.015f);

            //FREESTANDING DECORATIVE PLANTS
            holder.Add("Dried Sunflowers", 0.015f);
            holder.Add("Bonsai Tree", 0.035f);
            holder.Add("S. Pine", 0.025f);
            holder.Add("Tree Column", 0.06f);
            holder.Add("Small Plant", 0.015f);
            holder.Add("Table Plant", 0.015f);
            holder.Add("Deluxe Tree", 0.08f);
            holder.Add("Exotic Tree", 0.08f);
            holder.Add("Indoor Palm", 0.05f);
            holder.Add("Topiary Tree", 0.045f);
            holder.Add("Manicured Pine", 0.045f);
            holder.Add("Tree of the Winter Star", 0.125f);
            holder.Add("Long Cactus", 0.045f);
            holder.Add("Long Palm", 0.045f);

            //DECORATIVE HANGING PLANTS
            holder.Add("Ceiling Leaves", 0.015f);
            holder.Add("Jungle Decals", 0.025f);
            holder.Add("Indoor Hanging Basket", 0.02f);
            holder.Add("L. Light String", 0.02f);
            holder.Add("Palm Wall Ornament", 0.02f);
            holder.Add("S. Wall Flower", 0.03f);
            holder.Add("Wall Basket", 0.045f);
            holder.Add("Wall Cactus", 0.045f);
            holder.Add("Wall Flower", 0.045f);
            holder.Add("Wall Palm", 0.045f);

            //SEASONAL PLANTS


            //PAINTINGS
            holder.Add("'A Night On Eco-Hill'", 0.1f);
            holder.Add("'Jade Hills'", 0.1f);
            holder.Add("'Jade Hills Extended'", 0.1f);
            holder.Add("'Burnt Offering'", 0.1f);
            holder.Add("'Highway 89'", 0.1f);
            holder.Add("'Primal Motion'", 0.15f);
            holder.Add("'Spires'", 0.1f);
            holder.Add("'Queen of the Gem Sea'", 0.1f);
            holder.Add("'Pathways'", 0.1f);
            holder.Add("'The Muzzamaroo'", 0.1f);
            holder.Add("'Vanilla Villa'", 0.1f);
            holder.Add("'VGA Paradise'", 0.1f);
            holder.Add("'Frozen Dreams'", 0.1f);
            holder.Add("'Boat'", 0.1f);
            holder.Add("Foliage Print", 0.1f);
            holder.Add("'Physics 101'", 0.1f);
            holder.Add("'Kitemaster'95'", 0.1f);
            holder.Add("'Sun #44'", 0.1f);
            holder.Add("Calico Falls", 0.075f);
            holder.Add("'Sun #45'", 0.075f);
            holder.Add("'Blue City'", 0.075f);
            holder.Add("'Blueberries'", 0.075f);
            holder.Add("'Little Tree'", 0.075f);
            holder.Add("Needlepoint Flower", 0.075f);
            holder.Add("'Dancing Grass'", 0.075f);
            holder.Add("My First Painting", 0.075f);
            holder.Add("Colorful Set", 0.075f);
            holder.Add("'Vista'", 0.1f);
            holder.Add("'Volcano' Photo", 0.075f);

            //NIGHT MARKET PAINTINGS
            holder.Add("'Red Eagle'", 0.115f);
            holder.Add("'Portrait Of A Mermaid'", 0.115f);
            holder.Add("'Solar Kingdom'", 0.115f);
            holder.Add("'Clouds'", 0.115f);
            holder.Add("'1000 Years From Now'", 0.115f);
            holder.Add("'Three Trees'", 0.115f);
            holder.Add("'The Serpent'", 0.115f);
            holder.Add("'Tropical Fish #173'", 0.115f);
            holder.Add("'Land Of Clay'", 0.115f);

            //MOVIE POSTERS
            holder.Add("'It Howls In The Rain'", 0.12f);
            holder.Add("'Journey Of The Prairie King: The Motion Picture'", 0.12f);
            holder.Add("'Mysterium'", 0.12f);
            holder.Add("'Natural Wonders: Exploring Our Vibrant World'", 0.12f);
            holder.Add("'The Brave Little Sapling'", 0.12f);
            holder.Add("'The Miracle At Coldstar Ranch'", 0.12f);
            holder.Add("'The Zuzu City Express'", 0.12f);
            holder.Add("'Wumbus'", 0.12f);

            //BANNERS
            holder.Add("Clouds Banner", 0.085f);
            holder.Add("Icy Banner", 0.085f);
            holder.Add("Moonlight Jellies Banner", 0.085f);
            holder.Add("Pastel Banner", 0.085f);
            holder.Add("Winter Banner", 0.085f);

            //WALL HANGINGS
            holder.Add("Anchor", 0.02f);
            holder.Add("Calendar", 0.02f);
            holder.Add("Ceiling Flags", 0.01f);
            holder.Add("Hanging Shield", 0.02f);
            holder.Add("J. Cola Light", 0.02f);
            holder.Add("Little Photos", 0.02f);
            holder.Add("Miner's Crest", 0.03f);
            holder.Add("Monster Danglers", 0.03f);
            holder.Add("Skull Poster", 0.02f);
            holder.Add("Wallflower Pal", 0.02f);
            holder.Add("World Map", 0.02f);
            holder.Add("Night Sky Decal #1", 0.02f);
            holder.Add("Night Sky Decal #2", 0.02f);
            holder.Add("Night Sky Decal #3", 0.02f);
            holder.Add("Pirate Flag", 0.02f);
            holder.Add("Small Wall Pumpkin", 0.02f);
            holder.Add("Strawberry Decal", 0.02f);
            holder.Add("Wall Pumpkin", 0.02f);
            holder.Add("Winter Tree Decal", 0.02f);
            holder.Add("Cloud Decal", 0.02f);
            holder.Add("Decorative Axe", 0.02f);
            holder.Add("Decorative Pitchfork", 0.02f);
            holder.Add("Lifesaver", 0.02f);
            holder.Add("Log Panel", 0.02f);
            holder.Add("Pyramid Decal", 0.02f);
            holder.Add("Starport Decal", 0.03f);
            holder.Add("Wood Panel", 0.02f);

            //FISHING TANKS
            holder.Add("Small Fish Tank", 0.045f);
            holder.Add("Modern Fish Tank", 0.055f);
            holder.Add("Large Fish Tank", 0.085f);
            holder.Add("Deluxe Fish Tank", 0.1f);
            holder.Add("Aquatic Sanctuary", 0.13f);

            //TORCHES
            holder.Add("Jungle Torch", 0.035f);
            holder.Add("Plain Torch", 0.035f);
            holder.Add("Stump Torch", 0.035f);

            //MISC
            holder.Add("China Cabinet", 0.105f);
            holder.Add("Ceramic Pillar", 0.025f);
            holder.Add("Gold Pillar", 0.035f);
            holder.Add("Industrial Pipe", 0.03f);
            holder.Add("Totem Pole", 0.055f);
            holder.Add("Decorative Bowl", 0.025f);
            holder.Add("Decorative Lantern", 0.04f);
            holder.Add("Globe", 0.055f);
            holder.Add("Model Ship", 0.055f);
            holder.Add("Small Crystal", 0.055f);
            holder.Add("Futan Bear", 0.085f);
            holder.Add("Decorative Trash Can", 0.005f);

            //OTHER DECORATIONS
            holder.Add("Bear Statue", 0.175f);
            holder.Add("Chicken Statue", 0.175f);
            holder.Add("Lg. Futan Bear", 0.175f);
            holder.Add("Obsidian Vase", 0.175f);
            holder.Add("Singing Stone", 0.175f);
            holder.Add("Skeleton Statue", 0.175f);
            holder.Add("Sloth Skeleton", 0.175f);
            holder.Add("Standing Geode", 0.175f);
            holder.Add("Butterfly Hutch", 0.175f);
            holder.Add("Leah's Sculpture", 0.175f);
            holder.Add("Sam's Boombox", 0.175f);
            holder.Add("Futan Rabbit", 0.175f);
            holder.Add("Small Junimo Plush", 0.175f);
            holder.Add("Green Serpent Statue", 0.175f);
            holder.Add("Purple Serpent Statue", 0.175f);
            holder.Add("Bobo Statue", 0.175f);
            holder.Add("Wumbus Statue", 0.175f);
            holder.Add("Junimo Plush", 0.175f);
            holder.Add("Gourmand Statue", 0.175f);
            holder.Add("Iridium Krobus", 0.175f);
            holder.Add("Squirrel Figurine", 0.175f);

            //SPECIAL ITEMS
            holder.Add("Basic Log", 0.025f);
            holder.Add("Log Section", 0.025f);
            holder.Add("Ornamental Hay Bale", 0.025f);
            holder.Add("Sign Of The Vessel", 0.025f);
            holder.Add("Wicked Statue", 0.045f);
            holder.Add("Big Green Cane", 0.025f);
            holder.Add("Big Red Cane", 0.025f);
            holder.Add("Green Canes", 0.025f);
            holder.Add("Red Canes", 0.025f);
            holder.Add("Mixed Cane", 0.025f);
            holder.Add("Lawn Flamingo", 0.025f);
            holder.Add("Plush Bunny", 0.055f);
            holder.Add("Seasonal Decor", 0.025f);
            holder.Add("Tub o' Flowers", 0.025f);
            holder.Add("Tea Set", 0.025f);
            holder.Add("Drum Block", 0.025f);
            holder.Add("Flute Block", 0.025f);
            holder.Add("Coffee Maker", 0.1f);
            holder.Add("Farm Computer", 0.075f);
            holder.Add("Mini-Jukebox", 0.075f);
            holder.Add("Telephone", 0.075f);
            holder.Add("Sewing Machine", 0.025f);
            holder.Add("Grave Stone", 0.025f);
            holder.Add("Stone Cairn", 0.025f);
            holder.Add("Stone Frog", 0.025f);
            holder.Add("Stone Junimo", 0.05f);
            holder.Add("Stone Owl", 0.025f);
            holder.Add("The Stone Owl", 0.2f);
            holder.Add("Stone Parrot", 0.025f);
            holder.Add("Suit Of Armor", 0.025f);
            holder.Add("??Foroguemon??", 0.085f);
            holder.Add("??HMTGF??", 0.085f);
            holder.Add("??Pinky Lemon??", 0.085f);
            holder.Add("Solid Gold Lewis", 0.05f);
            holder.Add("Statue Of Endless Fortune", 0.5f);
            holder.Add("Statue of Perfection", 0.1f);
            holder.Add("Statue Of True Perfection", 0.175f);
            holder.Add("Soda Machine", 0.075f);
            holder.Add("Stardew Hero Trophy", 0.1f);
            holder.Add("Junimo Kart Arcade System", 0.1f);
            holder.Add("Prairie King Arcade System", 0.1f);

            ItemDataBase = holder;
        }
    }
}
