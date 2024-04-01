/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Models;
using ItemExtensions.Models.Contained;
using Microsoft.Xna.Framework;

namespace ItemExtensions.Additions;

public static class DefaultContent
{
    //private static readonly Color[] DefaultColors = { Color.Tomato, Color.Purple, Color.Green, Color.Yellow, Color.CornflowerBlue, Color.Wheat};
    
    internal static Dictionary<string, FarmerAnimation> GetAnimations()
    {
        Dictionary<string, FarmerAnimation> result = new()
        {
            { "base_drink", Drink() },
            { "base_drink_like", LikeDrink() },
            { "base_badfood", BadFood() },
            { "base_eat", Eat() },
            { "base_eat_like", LikeEat() },
        };

        result.Add("base_jelly_blue", Jar(Color.CornflowerBlue));
        result.Add("base_jelly_green", Jar(Color.Green));
        result.Add("base_jelly_lightblue", Jar(Color.LightBlue));
        result.Add("base_jelly_lightgreen", Jar(Color.LightGreen));
        result.Add("base_jelly_orange", Jar(Color.Orange));
        result.Add("base_jelly_pink", Jar(Color.Pink));
        result.Add("base_jelly_purple", Jar(Color.Purple));
        result.Add("base_jelly_red", Jar(Color.Tomato));
        //result.Add($"base_jelly_wheat", Jar(Color.Wheat));
        result.Add("base_jelly_yellow", Jar(Color.Yellow));
        
        return result;
    }

    private static FarmerAnimation Drink()
    {
        var frames = new[]
        {
            new FarmerFrame { Frame = 0, Duration = 200 },
            new FarmerFrame { Frame = 18, Duration = 150 },
            new FarmerFrame { Frame = 26, Duration = 200 },
            new FarmerFrame { Frame = 25, Duration = 300 },
            new FarmerFrame { Frame = 68, Duration = 150 },
            new FarmerFrame { Frame = 86, Duration = 200 },
            new FarmerFrame { Frame = 103, Duration = 200 },
            new FarmerFrame { Frame = 86, Duration = 200 }
        };
        var food = new FoodAnimation
        {
            Duration = 500,
            Delay = 350,
            StartSound = "dwop",
            EndSound = "gulp",
            Crunch = false,
            Offset = new Vector2(25, 40),
            Flip = true,
            Scale = 0.8f,
            Speed = new Vector2(0.0f, 0.02f),
            Motion = new Vector2(0.0f, -0.9f),
            StopX = 0,
            StopY = -5
        };

        return new FarmerAnimation
        {
            Animation = frames,
            Food = food
        };
    }

    private static FarmerAnimation LikeDrink()
    {
        var baseAnim = Drink();
        var newAnimation = baseAnim.Animation.ToList();
        newAnimation.Add(new FarmerFrame
        {
            Frame = 102,
            Duration = 500,
            SecondaryArm = false,
            Flip = false,
            HideArm = false
        });
        
        baseAnim.Animation = newAnimation.ToArray();
        baseAnim.Emote = 20;

        return baseAnim;
    }

    private static FarmerAnimation BadFood()
    {
        var frames = new[]
        {
            new FarmerFrame { Frame = 95, SecondaryArm = true, Duration = 450},
            new FarmerFrame { Frame = 75 },
            new FarmerFrame { Frame = 87, Duration = 600 },
            new FarmerFrame { Frame = 88 },
            new FarmerFrame { Frame = 87 },
            new FarmerFrame { Frame = 88 },
            new FarmerFrame { Frame = 0 },
            new FarmerFrame { Frame = 104, Duration = 300 },
            new FarmerFrame { Frame = 105, Duration = 300 }
        };

        var food = new FoodAnimation
        {
            Duration = 300,
            Delay = 300,
            Crunch = true,
            Offset = new Vector2(0, -15),
            Flip = false,
            Scale = 0.8f,
            Speed = new Vector2(0.2f, 0.8f),
            Motion = new Vector2(1.2f, 1.4f),
            StopX = 50,
            StopY = 64
        };

        return new FarmerAnimation
        {
            Animation = frames,
            Food = food
        };
    }

    private static FarmerAnimation Eat()
    {
        var frames = new[]
        {
            new FarmerFrame { Frame = 0, Duration = 200 },
            new FarmerFrame { Frame = 18, Duration = 150 },
            new FarmerFrame { Frame = 26, Duration = 200 },
            new FarmerFrame { Frame = 25, Duration = 300 },
            new FarmerFrame { Frame = 68, Duration = 150 },
            new FarmerFrame { Frame = 86, Duration = 200 },
            new FarmerFrame { Frame = 103, Duration = 200 },
            new FarmerFrame { Frame = 86, Duration = 200 }
        };
        var food = new FoodAnimation
        {
            Duration = 500,
            Delay = 350,
            StartSound = "dwop",
            EndSound = "gulp",
            Crunch = true,
            Offset = new Vector2(25, 40),
            Flip = true,
            Scale = 0.8f,
            Speed = new Vector2(0.0f, 0.02f),
            Motion = new Vector2(0.0f, -0.9f),
            StopX = 0,
            StopY = -5
        };

        return new FarmerAnimation
        {
            Animation = frames,
            Food = food
        };
    }

    private static FarmerAnimation LikeEat()
    {
        var baseAnim = Eat();
        var newAnimation = baseAnim.Animation.ToList();
        newAnimation.Add(new FarmerFrame
        {
            Frame = 102,
            Duration = 500,
            SecondaryArm = false,
            Flip = false,
            HideArm = false
        });
        
        baseAnim.Animation = newAnimation.ToArray();
        baseAnim.Emote = 20;

        return baseAnim;
    }
    
    private static FarmerAnimation Jar(Color color)
    {
        var frames = new[]
        {
            new FarmerFrame { Frame = 0, Duration = 100 },
            new FarmerFrame { Frame = 3, Duration = 150 },
            new FarmerFrame { Frame = 55, Duration = 200 },
            new FarmerFrame { Frame = 54, Duration = 300 },
            new FarmerFrame { Frame = 70, Duration = 150 },
            new FarmerFrame { Frame = 24, Duration = 400, SecondaryArm = true },
            
            new FarmerFrame { Frame = 90, Duration = 200 },
            new FarmerFrame { Frame = 91, Duration = 200 },
            new FarmerFrame { Frame = 92, Duration = 200 },
            new FarmerFrame { Frame = 93, Duration = 200 },
            new FarmerFrame { Frame = 94, Duration = 200 }
        };
        var food = new FoodAnimation
        {
            Duration = 600,
            Delay = 100,
            Crunch = true,
            Offset = new Vector2(10, -40),
            Flip = false,
            Scale = 0.8f,
            Speed = new Vector2(0.0f, 0.03f),
            Motion = new Vector2(0.5f, 0.9f),
            StopX = 0,
            StopY = 7
        };

        return new FarmerAnimation
        {
            Animation = frames,
            Food = food
        };
    }
}