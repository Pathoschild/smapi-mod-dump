/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Pathfinding;

namespace AnotherClickMovementExperience;

public class ModEntry : Mod
{
    private static bool isPathing;
    // private Character fakeCharacter;
    private PathFindController controller;
    private TemporaryAnimatedSprite sprite;


    public override void Entry(IModHelper helper)
    {
        helper.Events.Input.ButtonPressed += this.InputOnButtonPressed;
        helper.Events.Input.ButtonPressed += (sender, args) =>
        {
            if (args.Button == SButton.OemTilde)
            {
                Vector2 tile = Game1.currentCursorTile;
                GameLocation here = Game1.currentLocation;
            }
        };
        helper.Events.GameLoop.UpdateTicking += this.GameLoopOnUpdateTicking;
        helper.Events.Content.AssetRequested += this.ContentOnAssetRequested;

        var harmony = new Harmony(this.ModManifest.UniqueID);

        // All of our Harmony patches to disable interactions while in build mode.
        harmony.Patch(
            AccessTools.Method(typeof(Game1), "UpdateControlInput"),
            new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.UpdateControlInput_Prefix)));
    }

    private void ContentOnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("Mods/DecidedlyHuman/ACME/ClickAnimation"))
        {
            e.LoadFromModFile<Texture2D>("assets/click-to-move-animation.png", AssetLoadPriority.Low);
        }
    }

    private static bool UpdateControlInput_Prefix(Game1 __instance, GameTime time)
    {
        return !isPathing;
    }

    private void GameLoopOnUpdateTicking(object? sender, UpdateTickingEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        if (isPathing)
        {
            // Vector2 oldLocation = Game1.player.Position;
            // this.controller.update(Game1.currentGameTime);
            // Vector2 newLocation = Game1.player.Position;
            //
            // // 0: Up
            // // 1: Right
            // // 2: Down
            // // 3: Left
            // if (oldLocation.X < newLocation.X)
            // {
            //     Game1.player.faceDirection(1);
            //     Game1.player.FarmerSprite.faceDirection(1);
            // }
            // if (oldLocation.X > newLocation.X)
            // {
            //     Game1.player.faceDirection(3);
            //     Game1.player.FarmerSprite.faceDirection(3);
            // }
            // if (oldLocation.Y < newLocation.Y)
            // {
            //     Game1.player.faceDirection(2);
            //     Game1.player.FarmerSprite.faceDirection(2);
            // }
            // if (oldLocation.Y > newLocation.Y)
            // {
            //     Game1.player.faceDirection(0);
            //     Game1.player.FarmerSprite.faceDirection(0);
            // }
            //
            // Game1.player.animateInFacingDirection(Game1.currentGameTime);
            // Game1.player.updateMovementAnimation(Game1.currentGameTime);
            // Game1.player.Sprite.

            // Game1.player.Position = this.fakeCharacter.Position;
        }

        if (this.controller is null || this.controller.pathToEndPoint is null || this.controller.pathToEndPoint.Count < 1)
        {
            isPathing = false;
            if (Game1.currentLocation.temporarySprites.Contains(this.sprite))
                Game1.currentLocation.temporarySprites.Remove(this.sprite);
            // Game1.freezeControls = false;
        }
    }

    private void InputOnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady) return;
        if (Game1.activeClickableMenu is not null) return;
        if (e.Button != SButton.MouseRight) return;

        isPathing = false;
        if (Game1.currentLocation.temporarySprites.Contains(this.sprite))
            Game1.currentLocation.temporarySprites.Remove(this.sprite);

        Point playerPosition = Game1.player.TilePoint;
        Point targetPosition = Game1.currentCursorTile.ToPoint();

        // This doesdn't seem to work quite right. It seems to work differently going to the left or right.
        // var points = PathFindController.FindPathOnFarm(
        //     playerPosition, targetPosition,
        //     Game1.currentLocation, 500);

        // this.fakeCharacter = new Character(new AnimatedSprite("LooseSprites/Cursors"), Game1.player.Position, 10, "");
        this.controller = new PathFindController(Game1.player, Game1.currentLocation, targetPosition, 3);
        this.controller.endBehaviorFunction = (character, location) =>
        {
            isPathing = false;
            if (Game1.currentLocation.temporarySprites.Contains(this.sprite))
                Game1.currentLocation.temporarySprites.Remove(this.sprite);
        };

        this.sprite = new TemporaryAnimatedSprite(
            "Mods/DecidedlyHuman/ACME/ClickAnimation",
            new Rectangle(0, 0, 64, 80),
            0.05f,
            10,
            2000,
            Game1.currentCursorTile * 64 - new Vector2(0, 20),
            false,
            false);
        Game1.player.controller = this.controller;
        // Game1.player.setRunning(true);
        // Game1.freezeControls = true;
        // Game1.displayHUD = true;
        isPathing = true;
        Game1.currentLocation.temporarySprites.Add(this.sprite);

        // if (points is null || points.Count < 1)
        //     return;
        //
        // while (!Game1.player.getTileLocationPoint().Equals(targetPosition))
        // {
        //     if (points.Count < 1)
        //         break;
        //
        //     Point location = Game1.player.getTileLocationPoint();
        //
        //     Vector2 moveDelta = Vector2.Lerp(location.ToVector2(), points.Pop().ToVector2(), 1f);
        //     Game1.player.Position = moveDelta * 64;
        // }
    }
}
