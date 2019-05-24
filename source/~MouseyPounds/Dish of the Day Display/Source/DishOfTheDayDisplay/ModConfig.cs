namespace DishOfTheDayDisplay
{
    class ModConfig
    {
            public bool Show_Chat_Message { get; set; } = true;
            public bool Place_Sign { get; set; } = true;
            public string Sign_Type { get; set; } = "wood";
            public string Sign_Location_Map_Name { get; set; } = "Town";
            public int Sign_Location_Tile_X { get; set; } = 39;
            public int Sign_Location_Tile_Y { get; set; } = 70;
    }
}
