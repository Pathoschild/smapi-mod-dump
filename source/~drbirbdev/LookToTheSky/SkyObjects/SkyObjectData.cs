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
using BirbCore.Extensions;
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
        TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite();
        sprite.acceleration = new Vector2(this.AccelerationX, this.AccelerationY);
        sprite.accelerationChange = new Vector2(this.AccelerationChangeX, this.AccelerationChangeY);
        sprite.alpha = this.Alpha;
        sprite.alphaFade = this.AlphaFade;
        sprite.alphaFadeFade = this.AlphaFadeFade;
        sprite.animationLength = this.AnimationLength;
        sprite.color = Utility.StringToColor(this.Color) ?? Microsoft.Xna.Framework.Color.White;
        sprite.delayBeforeAnimationStart = this.DelayBeforeAnimationStart;
        sprite.endFunction = null; // TODO: end function
        sprite.endSound = this.EndSound;
        sprite.extraInfoForEndBehavior = 0;
        sprite.flash = this.Flash;
        sprite.flicker = this.Flicker;
        sprite.holdLastFrame = this.HoldLastFrame;
        sprite.interval = this.Interval;
        sprite.motion = new Vector2(this.MotionX, this.MotionY);
        sprite.paused = this.Paused;
        sprite.pingPong = this.PingPong;
        sprite.pingPongMotion = this.PingPongMotion;
        sprite.pulse = this.Pulse;
        sprite.pulseAmount = this.PulseAmount;
        sprite.pulseTime = this.PulseTime;
        sprite.reachedStopCoordinate = null; // TODO: endfunction
        sprite.rotation = this.Rotation;
        sprite.rotationChange = this.RotationChange;
        sprite.scale = this.Scale;
        sprite.scaleChange = this.ScaleChange;
        sprite.scaleChangeChange = this.ScaleChangeChange;
        sprite.shakeIntensity = this.ShakeIntensity;
        sprite.shakeIntensityChange = this.ShakeIntensityChange;
        sprite.sourceRect = new Rectangle(this.SourceRectX, this.SourceRectY, this.SourceRectWidth, this.SourceRectHeight);
        sprite.sourceRectStartingPos = new Vector2(this.SourceRectStartingX, this.SourceRectStartingY);
        sprite.startSound = this.StartSound;
        sprite.stopAcceleratingWhenVelocityIsZero = this.StopAcceleratingWhenVelocityIsZero;
        sprite.texture = ModEntry.Instance.Helper.GameContent.Load<Texture2D>(this.TextureName);
        sprite.textureName = this.TextureName;
        sprite.ticksBeforeAnimationStart = this.TicksBeforeAnimationStart;
        sprite.totalNumberOfLoops = this.TotalNumberOfLoops;
        sprite.verticalFlipped = this.VerticalFlipped;
        sprite.xPeriodic = this.XPeriodic;
        sprite.xPeriodicLoopTime = this.XPeriodicLoopTime;
        sprite.xPeriodicRange = this.XPeriodicRange;
        sprite.xStopCoordinate = this.XStopCoordinate;
        sprite.yPeriodic = this.YPeriodic;
        sprite.yPeriodicLoopTime = this.YPeriodicLoopTime;
        sprite.yPeriodicRange = this.YPeriodicRange;
        sprite.yStopCoordinate = this.YStopCoordinate;
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
