**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/SpousesIsland**

----

"DynamicTokens": [
        //random (used for spouse schedule)
        {
            "Name": "Random_Map",
            "Value": "{{Random: Custom_GiCave, Custom_GiForest, Custom_GiRiver, Custom_GiClearance, Custom_IslandSW, Custom_GiHut, Custom_GiForestEnd, Custom_GiRBeach}}",
            "When": {
                "HasMod": ""
            }
        },
        {
            "Name": "Random_Tile",
            "Value": "{{Random: {{Range: 9, 22}}}}, {{Random: {{Range: 10, 16}}}}",
            "When": {
                "Random_Map": "Custom_GiCave"
            }
        },
        {
            "Name": "Random_Tile",
            "Value": "{{Random: {{Range: 11, 29}}}}, {{Random: {{Range: 21, 31}}}}",
            "When": {
                "Random_Map": "Custom_GiForest"
            }
        },
        {
            "Name": "Random_Tile",
            "Value": "{{Random: {{Range: 15, 34}}}}, {{Random: {{Range: 6, 11}}}}",
            "When": {
                "Random_Map": "Custom_GiRiver"
            }
        },
        {
            "Name": "Random_Tile",
            "Value": "{{Random: {{Range: 11, 22}}}}, {{Random: {{Range: 13, 26}}}}",
            "When": {
                "Random_Map": "Custom_GiClearance"
            }
        },
        {
            "Name": "Random_Tile",
            "Value": "{{Random: {{Range: 10, 37}}}}, {{Random: {{Range: 17, 24}}}}",
            "When": {
                "Random_Map": "Custom_IslandSW"
            }
        },
        {
            "Name": "Random_Tile",
            "Value": "{{Random: {{Range: 1, 7}}}}, {{Random: {{Range: 6, 8}}}}",
            "When": {
                "Random_Map": "Custom_GiHut"
            }
        },
        {
            "Name": "Random_Tile",
            "Value": "{{Random: {{Range: 9, 25}}}}, {{Random: {{Range: 25, 31}}}}",
            "When": {
                "Random_Map": "Custom_GiForestEnd"
            }
        },
        {
            "Name": "Random_Tile",
            "Value": "{{Random: {{Range: 27, 35}}}}, {{Random: {{Range: 6, 23}}}}",
            "When": {
                "Random_Map": "Custom_GiRBeach"
            }
        }
    ]
