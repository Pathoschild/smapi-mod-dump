/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ylsama/RightClickMoveMode
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley;
using StardewValley.Tools;

namespace MouseMoveMode
{
    class DecompiliedGame1
    {
        public static Farmer player { get; private set; }
        public static GameLocation currentLocation { get; private set; }
        public static bool wasMouseVisibleThisFrame { get; private set; }
        public static bool eventUp { get; private set; }
        public static xTile.Dimensions.Rectangle viewport { get; private set; }
        public static bool fadeToBlack { get; private set; }
        public static object oldKBState { get; private set; }
        public static InputState input { get; private set; }
        public static MouseState oldMouseState { get; private set; }
        public static object oldPadState { get; private set; }
        public static float mouseCursorTransparency { get; private set; }

        private static Event CurrentEvent;

        public static bool isCheckingNonMousePlacement { get; private set; }
        public static Options options { get; private set; }
        public static bool dialogueUp { get; private set; }
        public static LocalizedContentManager content { get; private set; }

        public static void init()
        {
            player = Game1.player;
            currentLocation = Game1.player.currentLocation;
            wasMouseVisibleThisFrame = Game1.wasMouseVisibleThisFrame;
            eventUp = Game1.eventUp;
            viewport = Game1.viewport;
            fadeToBlack = Game1.fadeToBlack;

            mouseCursorTransparency = Game1.mouseCursorTransparency;
            CurrentEvent = Game1.CurrentEvent;
            isCheckingNonMousePlacement = !IsPerformingMousePlacement();
            options = Game1.options;
            dialogueUp = Game1.dialogueUp;
            content = Game1.content;
        }

        private static int getMouseX()
        {
            return Game1.getMouseX();
        }

        private static int getMouseY()
        {
            return Game1.getMouseY();
        }

        private static int getOldMouseX()
        {
            return Game1.getOldMouseX();
        }

        private static int getOldMouseY()
        {
            return Game1.getOldMouseY();
        }

        private static bool didPlayerJustRightClick(bool ignoreNonMouseHeldInput)
        {
            return Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: ignoreNonMouseHeldInput);
        }

        private static bool isFestival()
        {
            return Game1.isFestival();
        }

        private static bool tryToCheckAt(Vector2 grabTile, Farmer who)
        {
            return Game1.tryToCheckAt(grabTile, who);
        }
        private static bool IsPerformingMousePlacement()
        {
            return Game1.IsPerformingMousePlacement();
        }

        private static bool isOneOfTheseKeysDown(KeyboardState state, InputButton[] keys)
        {
            return Game1.isOneOfTheseKeysDown(state, keys);
        }

        private static bool playSound(string cueName, System.Nullable<int> pitch = null)
        {
            return Game1.playSound(cueName, pitch);
        }

        private static void addHUDMessage(HUDMessage message)
        {
            Game1.addHUDMessage(message);
        }

        public static bool pressActionButtonMod(Vector2 grabTile, bool forceNonDirectedTile = false)
        {
            init();
            if (!player.UsingTool && (!eventUp || (currentLocation.currentEvent != null && currentLocation.currentEvent.playerControlSequence)) && !fadeToBlack)
            {
                if (wasMouseVisibleThisFrame && currentLocation.animals.Length > 0)
                {
                    Vector2 mousePosition = new Vector2(getOldMouseX() + viewport.X, getOldMouseY() + viewport.Y);
                    if (Utility.withinRadiusOfPlayer((int)mousePosition.X, (int)mousePosition.Y, 1, player))
                    {
                        if (currentLocation.CheckPetAnimal(mousePosition, player))
                        {
                            return true;
                        }
                        if (didPlayerJustRightClick(ignoreNonMouseHeldInput: true) && currentLocation.CheckInspectAnimal(mousePosition, player))
                        {
                            return true;
                        }
                    }
                }
                Vector2 cursorTile = grabTile;
                bool non_directed_tile = false;
                if (forceNonDirectedTile || !wasMouseVisibleThisFrame || mouseCursorTransparency == 0f || !Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, player))
                {
                    grabTile = player.GetGrabTile();
                    non_directed_tile = true;
                }
                bool was_character_at_grab_tile = false;
                if (eventUp && !isFestival())
                {
                    CurrentEvent?.receiveActionPress((int)grabTile.X, (int)grabTile.Y);
                    return false;
                }
                if (tryToCheckAt(grabTile, player))
                {
                    return false;
                }
                if (player.isRidingHorse())
                {
                    player.mount.checkAction(player, player.currentLocation);
                    return false;
                }
                if (!player.canMove)
                {
                    return false;
                }
                if (!was_character_at_grab_tile && player.currentLocation.isCharacterAtTile(grabTile) != null)
                {
                    was_character_at_grab_tile = true;
                }
                bool isPlacingObject = false;
                if (player.ActiveObject != null && !(player.ActiveObject is Furniture))
                {
                    if (player.ActiveObject.performUseAction(currentLocation))
                    {
                        player.reduceActiveItemByOne();
                        return false;
                    }
                    int stack = player.ActiveObject.Stack;
                    isCheckingNonMousePlacement = !IsPerformingMousePlacement();
                    if (non_directed_tile)
                    {
                        isCheckingNonMousePlacement = true;
                    }
                    //if (isOneOfTheseKeysDown(currentKBState, options.actionButton))
                    //{
                    //    isCheckingNonMousePlacement = true;
                    //}
                    Vector2 valid_position = Utility.GetNearbyValidPlacementPosition(player, currentLocation, player.ActiveObject, (int)grabTile.X * 64 + 32, (int)grabTile.Y * 64 + 32);
                    if (!isCheckingNonMousePlacement && player.ActiveObject is Wallpaper && Utility.tryToPlaceItem(currentLocation, player.ActiveObject, (int)cursorTile.X * 64, (int)cursorTile.Y * 64))
                    {
                        isCheckingNonMousePlacement = false;
                        return true;
                    }
                    if (Utility.tryToPlaceItem(currentLocation, player.ActiveObject, (int)valid_position.X, (int)valid_position.Y))
                    {
                        isCheckingNonMousePlacement = false;
                        return true;
                    }
                    if (!eventUp && (player.ActiveObject == null || player.ActiveObject.Stack < stack || player.ActiveObject.isPlaceable()))
                    {
                        isPlacingObject = true;
                    }
                    isCheckingNonMousePlacement = false;
                }
                if (!isPlacingObject && !was_character_at_grab_tile)
                {
                    grabTile.Y += 1f;
                    if (player.FacingDirection >= 0 && player.FacingDirection <= 3)
                    {
                        Vector2 normalized_offset2 = grabTile - player.Tile;
                        if (normalized_offset2.X > 0f || normalized_offset2.Y > 0f)
                        {
                            normalized_offset2.Normalize();
                        }
                        if (Vector2.Dot(Utility.DirectionsTileVectors[player.FacingDirection], normalized_offset2) >= 0f && tryToCheckAt(grabTile, player))
                        {
                            return false;
                        }
                    }
                    if (!eventUp && player.ActiveObject is Furniture furniture3)
                    {
                        furniture3.rotate();
                        playSound("dwoop");
                        return false;
                    }
                    grabTile.Y -= 2f;
                    if (player.FacingDirection >= 0 && player.FacingDirection <= 3 && !was_character_at_grab_tile)
                    {
                        Vector2 normalized_offset = grabTile - player.Tile;
                        if (normalized_offset.X > 0f || normalized_offset.Y > 0f)
                        {
                            normalized_offset.Normalize();
                        }
                        if (Vector2.Dot(Utility.DirectionsTileVectors[player.FacingDirection], normalized_offset) >= 0f && tryToCheckAt(grabTile, player))
                        {
                            return false;
                        }
                    }
                    if (!eventUp && player.ActiveObject is Furniture furniture2)
                    {
                        furniture2.rotate();
                        playSound("dwoop");
                        return false;
                    }
                    grabTile = player.Tile;
                    if (tryToCheckAt(grabTile, player))
                    {
                        return false;
                    }
                    if (!eventUp && player.ActiveObject is Furniture furniture)
                    {
                        furniture.rotate();
                        playSound("dwoop");
                        return false;
                    }
                }

                if (!player.isEating && player.ActiveObject != null && !dialogueUp && !eventUp && !player.canOnlyWalk && !player.FarmerSprite.PauseForSingleAnimation && !fadeToBlack && player.ActiveObject.Edibility != -300 && didPlayerJustRightClick(ignoreNonMouseHeldInput: true))
                {
                    if (player.team.SpecialOrderRuleActive("SC_NO_FOOD"))
                    {
                        MineShaft obj = player.currentLocation as MineShaft;
                        if (obj != null && obj.getMineArea() == 121)
                        {
                            addHUDMessage(new HUDMessage(content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"), 3));
                            return false;
                        }
                    }
                    if (player.hasBuff("25") && player.ActiveObject != null && !player.ActiveObject.HasContextTag("ginger_item"))
                    {
                        addHUDMessage(new HUDMessage(content.LoadString("Strings\\StringsFromCSFiles:Nauseous_CantEat"), 3));
                        return false;
                    }
                    player.faceDirection(2);
                    player.itemToEat = player.ActiveObject;
                    player.FarmerSprite.setCurrentSingleAnimation(304);
                    if (Game1.objectData.TryGetValue(player.ActiveObject.ItemId, out var objectData))
                    {
                        currentLocation.createQuestionDialogue((objectData.IsDrink && player.ActiveObject.preserve.Value != Object.PreserveType.Pickle) ? content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3159", player.ActiveObject.DisplayName) : content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3160", player.ActiveObject.DisplayName), currentLocation.createYesNoResponses(), "Eat");
                    }
                    return false;
                }
            }

            // if (player.CurrentTool is MeleeWeapon && player.CanMove && !player.canOnlyWalk && !eventUp && !player.onBridge && didPlayerJustRightClick(ignoreNonMouseHeldInput: true))
            // {
            //     ((MeleeWeapon)player.CurrentTool).animateSpecialMove(player);
            //     return false;
            // }
            return true;
        }

        public bool canPlace()
        {
            var checkCanPlace = false;
            checkCanPlace |= Utility.playerCanPlaceItemHere(currentLocation, player.CurrentItem, getMouseX() + viewport.X, getMouseY() + viewport.Y, player);
            checkCanPlace |= Utility.isThereAnObjectHereWhichAcceptsThisItem(currentLocation, player.CurrentItem, getMouseX() + viewport.X, getMouseY() + viewport.Y)
                    && Utility.withinRadiusOfPlayer(getMouseX() + viewport.X, getMouseY() + viewport.Y, 1, player);
            return checkCanPlace;
        }
    }
}
