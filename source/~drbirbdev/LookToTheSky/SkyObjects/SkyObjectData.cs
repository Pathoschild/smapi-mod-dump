/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace LookToTheSky.SkyObjects;
public class SkyObjectData
{
    public string OnExitLeft { get; set; }
    public string OnExitRight { get; set; }
    public string OnExitTop { get; set; }
    public string OnExitBottom { get; set; }
    public string OnExitSide { get; set; }
    public string OnExit { get; set; }

    public float AccelerationX { get; set; } = 0f;
    public float AccelerationY { get; set; } = 0f;
    public float AccelerationChangeX { get; set; } = 0f;
    public float AccelerationChangeY { get; set; } = 0f;
    public float Alpha { get; set; } = 1f;
    public float AlphaFade { get; set; } = 0f;
    public float AlphaFadeFade { get; set; } = 0f;
    public int AnimationLength { get; set; } = 0;
    public string Color { get; set; } = "white";
    public int DelayBeforeAnimationStart { get; set; } = 0;
    public string EndSound { get; set; } = "";
    public bool Flash { get; set; } = false;
    public bool Flicker { get; set; } = false;
    public bool HoldLastFrame { get; set; } = false;
    public float InitialPositionYMax { get; set; } = 1f;
    public float InitialPositionYMin { get; set; } = 0f;
    public float Interval { get; set; } = 100f;
    public float MotionX { get; set; } = 0f;
    public float MotionY { get; set; } = 0f;
    public bool Paused { get; set; } = false;
    public bool PingPong { get; set; } = false;
    public int PingPongMotion { get; set; } = -1;
    public bool Pulse { get; set; } = false;
    public float PulseAmount { get; set; } = 0f;
    public float PulseTime { get; set; } = 1.1f;
    public float Rotation { get; set; } = 0f;
    public float RotationChange { get; set; } = 0f;
    public float Scale { get; set; } = 4f;
    public float ScaleChange { get; set; } = 0f;
    public float ScaleChangeChange { get; set; } = 0f;
    public float ShakeIntensity { get; set; } = 0f;
    public float ShakeIntensityChange { get; set; } = 0f;
    public int SourceRectX { get; set; } = 0;
    public int SourceRectY { get; set; } = 0;
    public int SourceRectWidth { get; set; } = 16;
    public int SourceRectHeight { get; set; } = 16;
    public int SourceRectStartingX { get; set; } = 0;
    public int SourceRectStartingY { get; set; } = 0;
    public string StartSound { get; set; } = "";
    public bool StopAcceleratingWhenVelocityIsZero { get; set; } = false;
    public string TextureName { get; set; } = "LooseSprites/Cursors";
    public int TicksBeforeAnimationStart { get; set; } = 0;
    public int TotalNumberOfLoops { get; set; } = 0;
    public bool VerticalFlipped { get; set; } = false;
    public bool XPeriodic { get; set; } = false;
    public float XPeriodicLoopTime { get; set; } = 0f;
    public float XPeriodicRange { get; set; } = 0f;
    public int XStopCoordinate { get; set; } = -1;
    public bool YPeriodic { get; set; } = false;
    public float YPeriodicLoopTime { get; set; } = 0f;
    public float YPeriodicRange { get; set; } = 0f;
    public int YStopCoordinate { get; set; } = -1;

    public TemporaryAnimatedSprite GetSprite()
    {
        TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite
        {
            acceleration = new Vector2(this.AccelerationX, this.AccelerationY),
            accelerationChange = new Vector2(this.AccelerationChangeX, this.AccelerationChangeY),
            alpha = this.Alpha,
            alphaFade = this.AlphaFade,
            alphaFadeFade = this.AlphaFadeFade,
            animationLength = this.AnimationLength,
            color = Utility.StringToColor(this.Color) ?? Microsoft.Xna.Framework.Color.White,
            delayBeforeAnimationStart = this.DelayBeforeAnimationStart,
            endFunction = null, // TODO: end function
            endSound = this.EndSound,
            extraInfoForEndBehavior = 0,
            flash = this.Flash,
            flicker = this.Flicker,
            holdLastFrame = this.HoldLastFrame,
            interval = this.Interval,
            motion = new Vector2(this.MotionX, this.MotionY),
            paused = this.Paused,
            pingPong = this.PingPong,
            pingPongMotion = this.PingPongMotion,
            pulse = this.Pulse,
            pulseAmount = this.PulseAmount,
            pulseTime = this.PulseTime,
            reachedStopCoordinate = null, // TODO: endfunction
            rotation = this.Rotation,
            rotationChange = this.RotationChange,
            scale = this.Scale,
            scaleChange = this.ScaleChange,
            scaleChangeChange = this.ScaleChangeChange,
            shakeIntensity = this.ShakeIntensity,
            shakeIntensityChange = this.ShakeIntensityChange,
            sourceRect = new Rectangle(this.SourceRectX, this.SourceRectY, this.SourceRectWidth, this.SourceRectHeight),
            sourceRectStartingPos = new Vector2(this.SourceRectStartingX, this.SourceRectStartingY),
            startSound = this.StartSound,
            stopAcceleratingWhenVelocityIsZero = this.StopAcceleratingWhenVelocityIsZero,
            texture = ModEntry.Instance.Helper.GameContent.Load<Texture2D>(this.TextureName),
            textureName = this.TextureName,
            ticksBeforeAnimationStart = this.TicksBeforeAnimationStart,
            totalNumberOfLoops = this.TotalNumberOfLoops,
            verticalFlipped = this.VerticalFlipped,
            xPeriodic = this.XPeriodic,
            xPeriodicLoopTime = this.XPeriodicLoopTime,
            xPeriodicRange = this.XPeriodicRange,
            xStopCoordinate = this.XStopCoordinate,
            yPeriodic = this.YPeriodic,
            yPeriodicLoopTime = this.YPeriodicLoopTime,
            yPeriodicRange = this.YPeriodicRange,
            yStopCoordinate = this.YStopCoordinate
        };
        return sprite;
    }

    public static Farmer GetPlayer(string str)
    {
        if (str is null)
        {
            return null;
        }
        else if (str == "Current")
        {
            return Game1.player;
        }
        else if (str == "Host")
        {
            return Game1.MasterPlayer;
        }
        else if (long.TryParse(str, out long id))
        {
            return Game1.getFarmer(id);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Try to get a Character.
    /// This can be a player, animal, horse, pet, monster, etc.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Character GetCharacter(string str)
    {
        Character player = GetPlayer(str);
        if (player != null)
        {
            return player;
        }
        else if (str == "Horse")
        {
            return Utility.findHorseForPlayer(Game1.player.UniqueMultiplayerID);
        }
        else if (str == "Pet")
        {
            return Game1.player.getPet();
        }
        else if (Game1.farmAnimalData.ContainsKey(str))
        {
            Character animal = null;

            Utility.ForEachLocation((location) =>
            {
                foreach (FarmAnimal farmAnimal in location.animals.Values)
                {
                    if (farmAnimal.type.Value == str)
                    {
                        animal = farmAnimal;
                        return false;
                    }
                }
                return true;
            });
            if (animal is not null)
            {
                return animal;
            }
        }

        if (Game1.getCharacterFromName(str, true) is not null)
        {
            return Game1.getCharacterFromName(str, true);
        }
        return null;
    }

    public static class DefaultEndBehaviorHandlers
    {


        public static void SpawnSprite(TemporaryAnimatedSprite sprite, string[] args)
        {

        }

        public static void ChangeFields(TemporaryAnimatedSprite sprite, string[] args)
        {

        }
    }

    public static readonly Dictionary<string, TemporaryAnimatedSprite.endBehavior> EndFunctions = new()
    {
        {"SpawnItem", (i) =>
        {

        } }
    };
}
