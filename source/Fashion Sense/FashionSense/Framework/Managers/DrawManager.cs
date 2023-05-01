/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Models.Appearances;
using FashionSense.Framework.Models.Appearances.Accessory;
using FashionSense.Framework.Models.Appearances.Generic;
using FashionSense.Framework.Models.Appearances.Hair;
using FashionSense.Framework.Models.Appearances.Hat;
using FashionSense.Framework.Models.Appearances.Pants;
using FashionSense.Framework.Models.Appearances.Shirt;
using FashionSense.Framework.Models.Appearances.Shoes;
using FashionSense.Framework.Models.Appearances.Sleeves;
using FashionSense.Framework.Models.General;
using FashionSense.Framework.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using static StardewValley.FarmerSprite;

namespace FashionSense.Framework.Managers
{
    internal class DrawManager
    {
        private SpriteBatch _spriteBatch { get; }
        private FarmerRenderer _farmerRenderer { get; }
        private SkinToneModel _skinToneModel { get; }
        private Texture2D _baseTexture { get; }
        private Rectangle _farmerSourceRectangle { get; }
        private Rectangle _shirtSourceRectangle { get; }
        private Rectangle _dyedShirtSourceRectangle { get; }
        private Rectangle _accessorySourceRectangle { get; }
        private Rectangle _hatSourceRectangle { get; }
        private Dictionary<AppearanceModel, AnimationModel> _appearanceTypeToAnimationModels { get; }
        private AnimationFrame _animationFrame { get; }
        private bool _areColorMasksPendingRefresh { get; }
        private bool _isDrawingForUI { get; }
        private bool _hideSleeves { get; }
        private Color _overrideColor { get; }
        private Vector2 _position { get; }
        private Vector2 _origin { get; }
        private Vector2 _positionOffset { get; }
        private Vector2 _rotationAdjustment; // Purposely omitting { get; set; } as certain vanilla draw methods modify this value
        private int _facingDirection { get; }
        private int _currentFrame { get; }
        private float _scale { get; }
        private float _rotation { get; }

        internal float LayerDepth { get; set; }

        public DrawManager(SpriteBatch spriteBatch, FarmerRenderer farmerRenderer, SkinToneModel skinToneModel, Texture2D baseTexture, Rectangle farmerSourceRectangle, Rectangle shirtSourceRectangle, Rectangle dyedShirtSourceRectangle, Rectangle accessorySourceRectangle, Rectangle hatSourceRectangle, Dictionary<AppearanceModel, AnimationModel> appearanceTypeToAnimationModels, AnimationFrame animationFrame, Color overrideColor, Vector2 position, Vector2 origin, Vector2 positionOffset, Vector2 rotationAdjustment, int facingDirection, int currentFrame, float scale, float rotation, bool areColorMasksPendingRefresh, bool isDrawingForUI, bool hideSleeves)
        {
            _spriteBatch = spriteBatch;
            _farmerRenderer = farmerRenderer;
            _skinToneModel = skinToneModel;
            _baseTexture = baseTexture;
            _farmerSourceRectangle = farmerSourceRectangle;
            _shirtSourceRectangle = shirtSourceRectangle;
            _dyedShirtSourceRectangle = dyedShirtSourceRectangle;
            _accessorySourceRectangle = accessorySourceRectangle;
            _hatSourceRectangle = hatSourceRectangle;
            _appearanceTypeToAnimationModels = appearanceTypeToAnimationModels;
            _animationFrame = animationFrame;
            _overrideColor = overrideColor;
            _position = position;
            _origin = origin;
            _positionOffset = positionOffset;
            _rotationAdjustment = rotationAdjustment;
            _facingDirection = facingDirection;
            _currentFrame = currentFrame;
            _scale = scale;
            _rotation = rotation;
            _areColorMasksPendingRefresh = areColorMasksPendingRefresh;
            _isDrawingForUI = isDrawingForUI;
            _hideSleeves = hideSleeves;
        }

        public void DrawLayers(Farmer who, List<LayerData> layers)
        {
            foreach (var layer in layers)
            {
                if (layer.IsHidden)
                {
                    continue;
                }

                if (layer.IsVanilla)
                {
                    DrawVanillaLayer(who, layer);
                }
                else
                {
                    DrawCustomLayer(who, layer);
                }
            }
        }

        private void DrawVanillaLayer(Farmer who, LayerData layer)
        {
            switch (layer.AppearanceType)
            {
                case AppearanceContentPack.Type.Player:
                    DrawPlayerVanilla(who);
                    break;
                case AppearanceContentPack.Type.Pants:
                    DrawPantsVanilla(who);
                    break;
                case AppearanceContentPack.Type.Sleeves:
                    DrawSleevesVanilla(who);
                    break;
                case AppearanceContentPack.Type.Shirt:
                    DrawShirtVanilla(who);
                    break;
                case AppearanceContentPack.Type.Accessory:
                    DrawAccessoryVanilla(who);
                    break;
                case AppearanceContentPack.Type.Hair:
                    DrawHairVanilla(who);
                    break;
                case AppearanceContentPack.Type.Hat:
                    DrawHatVanilla(who);
                    break;
                case AppearanceContentPack.Type.Shoes:
                    // Purposely leaving blank, as vanilla shoes are handled in DrawPatch
                    break;
            }
        }

        private void DrawCustomLayer(Farmer who, LayerData layer)
        {
            switch (layer.AppearanceType)
            {
                case AppearanceContentPack.Type.Pants:
                case AppearanceContentPack.Type.Shirt:
                case AppearanceContentPack.Type.Accessory:
                case AppearanceContentPack.Type.Hair:
                case AppearanceContentPack.Type.Hat:
                case AppearanceContentPack.Type.Shoes:
                    DrawAppearance(who, layer);
                    break;
                case AppearanceContentPack.Type.Sleeves:
                    DrawSleevesCustom(who, layer);
                    break;
            }
        }

        #region Vanilla draw methods
        private void DrawPlayerVanilla(Farmer who)
        {
            // Check if the player's legs need to be hidden
            var adjustedBaseRectangle = _farmerSourceRectangle;
            if (AppearanceHelpers.ShouldHideLegs(who, _facingDirection) && !(bool)who.swimming)
            {
                switch (who.FarmerSprite.CurrentFrame)
                {
                    case 2:
                    case 16:
                    case 54:
                    case 57:
                    case 62:
                    case 66:
                    case 84:
                    case 90:
                    case 124:
                    case 125:
                        adjustedBaseRectangle.Height -= 6;
                        break;
                    case 6:
                    case 7:
                    case 9:
                    case 19:
                    case 21:
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                    case 43:
                    case 45:
                    case 55:
                    case 59:
                    case 61:
                    case 64:
                    case 68:
                    case 72:
                    case 74:
                    case 76:
                    case 94:
                    case 95:
                    case 97:
                    case 99:
                    case 105:
                        adjustedBaseRectangle.Height -= 8;
                        break;
                    case 11:
                    case 17:
                    case 20:
                    case 22:
                    case 23:
                    case 49:
                    case 50:
                    case 53:
                    case 56:
                    case 60:
                    case 69:
                    case 70:
                    case 71:
                    case 73:
                    case 75:
                    case 112:
                        adjustedBaseRectangle.Height -= 9;
                        break;
                    case 51:
                    case 106:
                        adjustedBaseRectangle.Height -= 12;
                        break;
                    case 52:
                        adjustedBaseRectangle.Height -= 11;
                        break;
                    case 77:
                        adjustedBaseRectangle.Height -= 10;
                        break;
                    case 107:
                    case 113:
                        adjustedBaseRectangle.Height -= 14;
                        break;
                    case 117:
                        adjustedBaseRectangle.Height -= 13;
                        break;
                    default:
                        adjustedBaseRectangle.Height -= 7;
                        break;
                }

                if (who.isMale)
                {
                    adjustedBaseRectangle.Height -= 1;
                }
            }

            // Draw the player's base texture
            _spriteBatch.Draw(_baseTexture, _position + _origin + _positionOffset, adjustedBaseRectangle, _overrideColor, _rotation, _origin, 4f * _scale, _animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, IncrementAndGetLayerDepth());

            // Vanilla swim draw logic
            if (!FarmerRenderer.isDrawingForUI && (bool)who.swimming)
            {
                if (who.currentEyes != 0 && who.FacingDirection != 0 && (Game1.timeOfDay < 2600 || (who.isInBed.Value && who.timeWentToBed.Value != 0)) && ((!who.FarmerSprite.PauseForSingleAnimation && !who.UsingTool) || (who.UsingTool && who.CurrentTool is FishingRod)))
                {
                    _spriteBatch.Draw(_baseTexture, _position + _origin + _positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4 + 20 + ((who.FacingDirection == 1) ? 12 : ((who.FacingDirection == 3) ? 4 : 0)), FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + 40), new Rectangle(5, 16, (who.FacingDirection == 2) ? 6 : 2, 2), _overrideColor, 0f, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                    _spriteBatch.Draw(_baseTexture, _position + _origin + _positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4 + 20 + ((who.FacingDirection == 1) ? 12 : ((who.FacingDirection == 3) ? 4 : 0)), FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + 40), new Rectangle(264 + ((who.FacingDirection == 3) ? 4 : 0), 2 + (who.currentEyes - 1) * 2, (who.FacingDirection == 2) ? 6 : 2, 2), _overrideColor, 0f, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                }

                // Exiting early from this method, as copied from the vanilla logic
                return;
            }

            // Draw blinking / eyes closed animation, if conditions are met
            FishingRod fishing_rod;
            if (who.currentEyes != 0 && _facingDirection != 0 && (Game1.timeOfDay < 2600 || (who.isInBed.Value && who.timeWentToBed.Value != 0)) && ((!who.FarmerSprite.PauseForSingleAnimation && !who.UsingTool) || (who.UsingTool && who.CurrentTool is FishingRod)) && (!who.UsingTool || (fishing_rod = who.CurrentTool as FishingRod) == null || fishing_rod.isFishing))
            {
                int x_adjustment = 5;
                x_adjustment = (_animationFrame.flip ? (x_adjustment - FarmerRenderer.featureXOffsetPerFrame[_currentFrame]) : (x_adjustment + FarmerRenderer.featureXOffsetPerFrame[_currentFrame]));
                switch (_facingDirection)
                {
                    case 1:
                        x_adjustment += 3;
                        break;
                    case 3:
                        x_adjustment++;
                        break;
                }

                x_adjustment *= 4;
                _spriteBatch.Draw(_baseTexture, _position + _origin + _positionOffset + new Vector2(x_adjustment, FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + ((who.IsMale && who.FacingDirection != 2) ? 36 : 40)), new Rectangle(5, 16, (_facingDirection == 2) ? 6 : 2, 2), _overrideColor, 0f, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                _spriteBatch.Draw(_baseTexture, _position + _origin + _positionOffset + new Vector2(x_adjustment, FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + ((who.FacingDirection == 1 || who.FacingDirection == 3) ? 40 : 44)), new Rectangle(264 + ((_facingDirection == 3) ? 4 : 0), 2 + (who.currentEyes - 1) * 2, (_facingDirection == 2) ? 6 : 2, 2), _overrideColor, 0f, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
            }
        }

        private void DrawPantsVanilla(Farmer who)
        {
            Rectangle pants_rect = new Rectangle(_farmerSourceRectangle.X, _farmerSourceRectangle.Y, _farmerSourceRectangle.Width, _farmerSourceRectangle.Height);
            pants_rect.X += _farmerRenderer.ClampPants(who.GetPantsIndex()) % 10 * 192;
            pants_rect.Y += _farmerRenderer.ClampPants(who.GetPantsIndex()) / 10 * 688;

            if (!who.IsMale)
            {
                pants_rect.X += 96;
            }

            _spriteBatch.Draw(FarmerRenderer.pantsTexture, _position + _origin + _positionOffset, pants_rect, _overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetPantsColor()) : _overrideColor, _rotation, _origin, 4f * _scale, _animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, IncrementAndGetLayerDepth());
        }

        private void DrawSleevesVanilla(Farmer who)
        {
            // Get the sleeves model, if applicable
            SleevesModel sleevesModel = null;
            if (who.modData.ContainsKey(ModDataKeys.CUSTOM_SLEEVES_ID) && FashionSense.textureManager.GetSpecificAppearanceModel<SleevesContentPack>(who.modData[ModDataKeys.CUSTOM_SLEEVES_ID]) is SleevesContentPack sleevesPack && sleevesPack != null)
            {
                sleevesModel = sleevesPack.GetSleevesFromFacingDirection(_facingDirection);
            }

            // Handle the vanilla sleeve / arm drawing, if a custom sleeve model isn't given
            if (sleevesModel is null && _hideSleeves is false)
            {
                DrawSlingshotVanilla(who);
            }
        }

        private void DrawSlingshotVanilla(Farmer who)
        {
            _spriteBatch.Draw(_baseTexture, _position + _origin + _positionOffset + who.armOffset, new Rectangle(_farmerSourceRectangle.X + (_animationFrame.secondaryArm ? 192 : 96), _farmerSourceRectangle.Y, _farmerSourceRectangle.Width, _farmerSourceRectangle.Height), _overrideColor, _rotation, _origin, 4f * _scale, _animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, IncrementAndGetLayerDepth());

            // Handle drawing slingshot
            if (who.usingSlingshot is false || who.CurrentTool is not Slingshot)
            {
                return;
            }

            Slingshot slingshot = who.CurrentTool as Slingshot;
            Point point = Utility.Vector2ToPoint(slingshot.AdjustForHeight(Utility.PointToVector2(slingshot.aimPos.Value)));
            int mouseX = point.X;
            int y = point.Y;
            int backArmDistance = slingshot.GetBackArmDistance(who);

            Vector2 shoot_origin = slingshot.GetShootOrigin(who);
            float frontArmRotation = (float)Math.Atan2((float)y - shoot_origin.Y, (float)mouseX - shoot_origin.X) + (float)Math.PI;
            if (Game1.options.useLegacySlingshotFiring is false)
            {
                frontArmRotation -= (float)Math.PI;
                if (frontArmRotation < 0f)
                {
                    frontArmRotation += (float)Math.PI * 2f;
                }
            }

            switch (_facingDirection)
            {
                case 0:
                    _spriteBatch.Draw(_baseTexture, _position + new Vector2(4f + frontArmRotation * 8f, -44f), new Rectangle(173, 238, 9, 14), Color.White, 0f, new Vector2(4f, 11f), 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth() + ((_facingDirection != 0) ? 5.9E-05f : (-0.0005f)));
                    break;
                case 1:
                    {
                        _spriteBatch.Draw(_baseTexture, _position + new Vector2(52 - backArmDistance, -32f), new Rectangle(147, 237, 10, 4), Color.White, 0f, new Vector2(8f, 3f), 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth() + ((_facingDirection != 0) ? 5.9E-05f : 0f));
                        _spriteBatch.Draw(_baseTexture, _position + new Vector2(36f, -44f), new Rectangle(156, 244, 9, 10), Color.White, frontArmRotation, new Vector2(0f, 3f), 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth() + ((_facingDirection != 0) ? 1E-08f : 0f));
                        int slingshotAttachX = (int)(Math.Cos(frontArmRotation + (float)Math.PI / 2f) * (double)(20 - backArmDistance - 8) - Math.Sin(frontArmRotation + (float)Math.PI / 2f) * -68.0);
                        int slingshotAttachY = (int)(Math.Sin(frontArmRotation + (float)Math.PI / 2f) * (double)(20 - backArmDistance - 8) + Math.Cos(frontArmRotation + (float)Math.PI / 2f) * -68.0);
                        Utility.drawLineWithScreenCoordinates((int)(_position.X + 52f - (float)backArmDistance), (int)(_position.Y - 32f - 4f), (int)(_position.X + 32f + (float)(slingshotAttachX / 2)), (int)(_position.Y - 32f - 12f + (float)(slingshotAttachY / 2)), _spriteBatch, Color.White);
                        break;
                    }
                case 3:
                    {
                        _spriteBatch.Draw(_baseTexture, _position + new Vector2(40 + backArmDistance, -32f), new Rectangle(147, 237, 10, 4), Color.White, 0f, new Vector2(9f, 4f), 4f * _scale, SpriteEffects.FlipHorizontally, IncrementAndGetLayerDepth() + ((_facingDirection != 0) ? 5.9E-05f : 0f));
                        _spriteBatch.Draw(_baseTexture, _position + new Vector2(24f, -40f), new Rectangle(156, 244, 9, 10), Color.White, frontArmRotation + (float)Math.PI, new Vector2(8f, 3f), 4f * _scale, SpriteEffects.FlipHorizontally, IncrementAndGetLayerDepth() + ((_facingDirection != 0) ? 1E-08f : 0f));
                        int slingshotAttachX = (int)(Math.Cos(frontArmRotation + (float)Math.PI * 2f / 5f) * (double)(20 + backArmDistance - 8) - Math.Sin(frontArmRotation + (float)Math.PI * 2f / 5f) * -68.0);
                        int slingshotAttachY = (int)(Math.Sin(frontArmRotation + (float)Math.PI * 2f / 5f) * (double)(20 + backArmDistance - 8) + Math.Cos(frontArmRotation + (float)Math.PI * 2f / 5f) * -68.0);
                        Utility.drawLineWithScreenCoordinates((int)(_position.X + 4f + (float)backArmDistance), (int)(_position.Y - 32f - 8f), (int)(_position.X + 26f + (float)slingshotAttachX * 4f / 10f), (int)(_position.Y - 32f - 8f + (float)slingshotAttachY * 4f / 10f), _spriteBatch, Color.White);
                        break;
                    }
                case 2:
                    _spriteBatch.Draw(_baseTexture, _position + new Vector2(4f, -32 - backArmDistance / 2), new Rectangle(148, 244, 4, 4), Color.White, 0f, Vector2.Zero, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth() + ((_facingDirection != 0) ? 5.9E-05f : 0f));
                    Utility.drawLineWithScreenCoordinates((int)(_position.X + 16f), (int)(_position.Y - 28f - (float)(backArmDistance / 2)), (int)(_position.X + 44f - frontArmRotation * 10f), (int)(_position.Y - 16f - 8f), _spriteBatch, Color.White);
                    Utility.drawLineWithScreenCoordinates((int)(_position.X + 16f), (int)(_position.Y - 28f - (float)(backArmDistance / 2)), (int)(_position.X + 56f - frontArmRotation * 10f), (int)(_position.Y - 16f - 8f), _spriteBatch, Color.White);
                    _spriteBatch.Draw(_baseTexture, _position + new Vector2(44f - frontArmRotation * 10f, -16f), new Rectangle(167, 235, 7, 9), Color.White, 0f, new Vector2(3f, 5f), 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth() + ((_facingDirection != 0) ? 5.9E-05f : 0f));
                    break;
            }
        }

        private void DrawShirtVanilla(Farmer who)
        {
            switch (_facingDirection)
            {
                case 0:
                    if (!who.bathingClothes)
                    {
                        _spriteBatch.Draw(FarmerRenderer.shirtsTexture, _position + _origin + _positionOffset + new Vector2(16f * _scale + (float)(FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4), (float)(56 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4) + (float)(int)_farmerRenderer.heightOffset * _scale), _shirtSourceRectangle, _overrideColor.Equals(Color.White) ? Color.White : _overrideColor, _rotation, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                        _spriteBatch.Draw(FarmerRenderer.shirtsTexture, _position + _origin + _positionOffset + new Vector2(16f * _scale + (float)(FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4), (float)(56 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4) + (float)(int)_farmerRenderer.heightOffset * _scale), _dyedShirtSourceRectangle, _overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : _overrideColor, _rotation, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                    }
                    break;
                case 1:
                    if (_rotation == -(float)Math.PI / 32f)
                    {
                        _rotationAdjustment.X = 6f;
                        _rotationAdjustment.Y = -2f;
                    }
                    else if (_rotation == (float)Math.PI / 32f)
                    {
                        _rotationAdjustment.X = -6f;
                        _rotationAdjustment.Y = 1f;
                    }
                    if (!who.bathingClothes)
                    {
                        _spriteBatch.Draw(FarmerRenderer.shirtsTexture, _position + _origin + _positionOffset + _rotationAdjustment + new Vector2(16f * _scale + (float)(FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4), 56f * _scale + (float)(FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4) + (float)(int)_farmerRenderer.heightOffset * _scale), _shirtSourceRectangle, _overrideColor.Equals(Color.White) ? Color.White : _overrideColor, _rotation, _origin, 4f * _scale + ((_rotation != 0f) ? 0f : 0f), SpriteEffects.None, IncrementAndGetLayerDepth());
                        _spriteBatch.Draw(FarmerRenderer.shirtsTexture, _position + _origin + _positionOffset + _rotationAdjustment + new Vector2(16f * _scale + (float)(FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4), 56f * _scale + (float)(FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4) + (float)(int)_farmerRenderer.heightOffset * _scale), _dyedShirtSourceRectangle, _overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : _overrideColor, _rotation, _origin, 4f * _scale + ((_rotation != 0f) ? 0f : 0f), SpriteEffects.None, IncrementAndGetLayerDepth());
                    }
                    break;
                case 2:
                    if (!who.bathingClothes)
                    {
                        _spriteBatch.Draw(FarmerRenderer.shirtsTexture, _position + _origin + _positionOffset + new Vector2(16 + FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, (float)(56 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4) + (float)(int)_farmerRenderer.heightOffset * _scale - (float)(who.IsMale ? 0 : 0)), _shirtSourceRectangle, _overrideColor.Equals(Color.White) ? Color.White : _overrideColor, _rotation, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth() + 1.5E-07f);
                        _spriteBatch.Draw(FarmerRenderer.shirtsTexture, _position + _origin + _positionOffset + new Vector2(16 + FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, (float)(56 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4) + (float)(int)_farmerRenderer.heightOffset * _scale - (float)(who.IsMale ? 0 : 0)), _dyedShirtSourceRectangle, _overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : _overrideColor, _rotation, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth() + 1.5E-07f);
                    }
                    break;
                case 3:
                    {
                        if (_rotation == -(float)Math.PI / 32f)
                        {
                            _rotationAdjustment.X = 6f;
                            _rotationAdjustment.Y = -2f;
                        }
                        else if (_rotation == (float)Math.PI / 32f)
                        {
                            _rotationAdjustment.X = -5f;
                            _rotationAdjustment.Y = 1f;
                        }
                        if (!who.bathingClothes)
                        {
                            _spriteBatch.Draw(FarmerRenderer.shirtsTexture, _position + _origin + _positionOffset + _rotationAdjustment + new Vector2(16 - FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, 56 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + (int)_farmerRenderer.heightOffset), _shirtSourceRectangle, _overrideColor.Equals(Color.White) ? Color.White : _overrideColor, _rotation, _origin, 4f * _scale + ((_rotation != 0f) ? 0f : 0f), SpriteEffects.None, IncrementAndGetLayerDepth() + 1.5E-07f);
                            _spriteBatch.Draw(FarmerRenderer.shirtsTexture, _position + _origin + _positionOffset + _rotationAdjustment + new Vector2(16 - FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, 56 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + (int)_farmerRenderer.heightOffset), _dyedShirtSourceRectangle, _overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : _overrideColor, _rotation, _origin, 4f * _scale + ((_rotation != 0f) ? 0f : 0f), SpriteEffects.None, IncrementAndGetLayerDepth() + 1.5E-07f);
                        }
                        break;
                    }
            }
        }

        private void DrawAccessoryVanilla(Farmer who)
        {
            if ((int)who.accessory >= 0)
            {
                switch (who.facingDirection.Value)
                {
                    case 0:
                        return;
                    case 1:
                        _spriteBatch.Draw(FarmerRenderer.accessoriesTexture, _position + _origin + _positionOffset + _rotationAdjustment + new Vector2(FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, 8 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + (int)_farmerRenderer.heightOffset - 4), _accessorySourceRectangle, (_overrideColor.Equals(Color.White) && (int)who.accessory < 6) ? ((Color)who.hairstyleColor) : _overrideColor, _rotation, _origin, 4f * _scale + ((_rotation != 0f) ? 0f : 0f), SpriteEffects.None, IncrementAndGetLayerDepth());
                        break;
                    case 2:
                        _spriteBatch.Draw(FarmerRenderer.accessoriesTexture, _position + _origin + _positionOffset + _rotationAdjustment + new Vector2(FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, 8 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + (int)_farmerRenderer.heightOffset - 4), _accessorySourceRectangle, (_overrideColor.Equals(Color.White) && (int)who.accessory < 6) ? ((Color)who.hairstyleColor) : _overrideColor, _rotation, _origin, 4f * _scale + ((_rotation != 0f) ? 0f : 0f), SpriteEffects.None, IncrementAndGetLayerDepth());
                        break;
                    case 3:
                        _spriteBatch.Draw(FarmerRenderer.accessoriesTexture, _position + _origin + _positionOffset + _rotationAdjustment + new Vector2(-FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, 4 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + (int)_farmerRenderer.heightOffset), _accessorySourceRectangle, (_overrideColor.Equals(Color.White) && (int)who.accessory < 6) ? ((Color)who.hairstyleColor) : _overrideColor, _rotation, _origin, 4f * _scale + ((_rotation != 0f) ? 0f : 0f), SpriteEffects.FlipHorizontally, IncrementAndGetLayerDepth());
                        break;
                }
            }
        }

        private void DrawHairVanilla(Farmer who)
        {
            int hair_style = who.getHair();
            HairStyleMetadata hair_metadata = Farmer.GetHairStyleMetadata(who.hair.Value);
            if (who != null && who.hat.Value != null && who.hat.Value.hairDrawType.Value == 1 && hair_metadata != null && hair_metadata.coveredIndex != -1)
            {
                hair_style = hair_metadata.coveredIndex;
                hair_metadata = Farmer.GetHairStyleMetadata(hair_style);
            }

            Texture2D hairTexture = FarmerRenderer.hairStylesTexture;
            Rectangle hairstyleSourceRect = new Rectangle(hair_style * 16 % FarmerRenderer.hairStylesTexture.Width, hair_style * 16 / FarmerRenderer.hairStylesTexture.Width * 96, 16, 32);
            if (hair_metadata != null)
            {
                hairTexture = hair_metadata.texture;
                hairstyleSourceRect = new Rectangle(hair_metadata.tileX * 16, hair_metadata.tileY * 16, 16, 32);
            }

            switch (_facingDirection)
            {
                case 0:
                    hairstyleSourceRect.Offset(0, 64);
                    _spriteBatch.Draw(hairTexture, _position + _origin + _positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + 4 + ((who.IsMale && hair_style >= 16) ? (-4) : ((!who.IsMale && hair_style < 16) ? 4 : 0))), hairstyleSourceRect, _overrideColor.Equals(Color.White) ? ((Color)who.hairstyleColor) : _overrideColor, _rotation, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                    break;
                case 1:
                    hairstyleSourceRect.Offset(0, 32);
                    _spriteBatch.Draw(hairTexture, _position + _origin + _positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + ((who.IsMale && (int)who.hair >= 16) ? (-4) : ((!who.IsMale && (int)who.hair < 16) ? 4 : 0))), hairstyleSourceRect, _overrideColor.Equals(Color.White) ? ((Color)who.hairstyleColor) : _overrideColor, _rotation, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                    break;
                case 2:
                    _spriteBatch.Draw(hairTexture, _position + _origin + _positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + ((who.IsMale && (int)who.hair >= 16) ? (-4) : ((!who.IsMale && (int)who.hair < 16) ? 4 : 0))), hairstyleSourceRect, _overrideColor.Equals(Color.White) ? ((Color)who.hairstyleColor) : _overrideColor, _rotation, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                    break;
                case 3:
                    bool flip2 = true;
                    if (hair_metadata != null && hair_metadata.usesUniqueLeftSprite)
                    {
                        flip2 = false;
                        hairstyleSourceRect.Offset(0, 96);
                    }
                    else
                    {
                        hairstyleSourceRect.Offset(0, 32);
                    }
                    _spriteBatch.Draw(hairTexture, _position + _origin + _positionOffset + new Vector2(-FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + ((who.IsMale && (int)who.hair >= 16) ? (-4) : ((!who.IsMale && (int)who.hair < 16) ? 4 : 0))), hairstyleSourceRect, _overrideColor.Equals(Color.White) ? ((Color)who.hairstyleColor) : _overrideColor, _rotation, _origin, 4f * _scale, flip2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, IncrementAndGetLayerDepth());
                    break;
            }
        }

        private void DrawHatVanilla(Farmer who)
        {
            if (who.hat.Value != null && !who.bathingClothes)
            {
                bool flip = who.FarmerSprite.CurrentAnimationFrame.flip;
                if (who.hat.Value.isMask && _facingDirection == 0)
                {
                    Rectangle mask_draw_rect = _hatSourceRectangle;
                    mask_draw_rect.Height -= 11;
                    mask_draw_rect.Y += 11;
                    _spriteBatch.Draw(FarmerRenderer.hatsTexture, _position + _origin + _positionOffset + new Vector2(0f, 44f) + new Vector2(-8 + ((!flip) ? 1 : (-1)) * FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + ((!who.hat.Value.ignoreHairstyleOffset) ? FarmerRenderer.hairstyleHatOffset[(int)who.hair % 16] : 0) + 4 + (int)_farmerRenderer.heightOffset), mask_draw_rect, Color.White, _rotation, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                    mask_draw_rect = _hatSourceRectangle;
                    mask_draw_rect.Height = 11;
                    _spriteBatch.Draw(FarmerRenderer.hatsTexture, _position + _origin + _positionOffset + new Vector2(-8 + ((!flip) ? 1 : (-1)) * FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + ((!who.hat.Value.ignoreHairstyleOffset) ? FarmerRenderer.hairstyleHatOffset[(int)who.hair % 16] : 0) + 4 + (int)_farmerRenderer.heightOffset), mask_draw_rect, who.hat.Value.isPrismatic ? Utility.GetPrismaticColor() : Color.White, _rotation, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                }
                else
                {
                    _spriteBatch.Draw(FarmerRenderer.hatsTexture, _position + _origin + _positionOffset + new Vector2(-8 + ((!flip) ? 1 : (-1)) * FarmerRenderer.featureXOffsetPerFrame[_currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[_currentFrame] * 4 + ((!who.hat.Value.ignoreHairstyleOffset) ? FarmerRenderer.hairstyleHatOffset[(int)who.hair % 16] : 0) + 4 + (int)_farmerRenderer.heightOffset), _hatSourceRectangle, who.hat.Value.isPrismatic ? Utility.GetPrismaticColor() : Color.White, _rotation, _origin, 4f * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                }
            }
        }
        #endregion

        #region Custom draw methods
        private void DrawAppearance(Farmer who, LayerData layer)
        {
            var model = layer.AppearanceModel;
            var modelPack = model.Pack;

            // Adjust color if needed
            Color? colorOverride = null;
            Color modelColor = layer.Colors.Count == 0 ? Color.White : layer.Colors[0];
            if (model.DisableGrayscale)
            {
                colorOverride = Color.White;
            }
            else if (model.IsPrismatic)
            {
                colorOverride = Utility.GetPrismaticColor(speedMultiplier: model.PrismaticAnimationSpeedMultiplier);
            }

            // Get any positional offset
            Position positionOffset = GetPositionOffset(model);

            // Get any feature offset
            var featureOffset = GetFeatureOffset(_facingDirection, _currentFrame, _scale, _farmerRenderer, model, who);
            if (model is SleevesModel || model is PantsModel || model is ShoesModel || model is HairModel)
            {
                featureOffset.Y -= who.IsMale ? 4 : 0;
            }

            _spriteBatch.Draw(modelPack.Texture, GetScaledPosition(_position, model, _isDrawingForUI) + _origin + _positionOffset + featureOffset, GetSourceRectangle(model, _appearanceTypeToAnimationModels), model.HasColorMask() ? Color.White : colorOverride is not null ? colorOverride.Value : modelColor, _rotation, _origin + new Vector2(positionOffset.X, positionOffset.Y), model.Scale * _scale, model.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, IncrementAndGetLayerDepth());

            if (model.HasColorMask())
            {
                DrawColorMask(_spriteBatch, modelPack, model, _areColorMasksPendingRefresh, GetScaledPosition(_position, model, _isDrawingForUI) + _origin + _positionOffset + featureOffset, GetSourceRectangle(model, _appearanceTypeToAnimationModels), colorOverride, layer.Colors, _rotation, _origin + new Vector2(positionOffset.X, positionOffset.Y), model.Scale * _scale, IncrementAndGetLayerDepth());
            }
            if (model.HasSkinToneMask())
            {
                DrawSkinToneMask(_spriteBatch, modelPack, model, _skinToneModel, _areColorMasksPendingRefresh, GetScaledPosition(_position, model, _isDrawingForUI) + _origin + _positionOffset + featureOffset, GetSourceRectangle(model, _appearanceTypeToAnimationModels), model.HasColorMask() ? Color.White : colorOverride is not null ? colorOverride.Value : modelColor, _rotation, _origin + new Vector2(positionOffset.X, positionOffset.Y), model.Scale * _scale, IncrementAndGetLayerDepth());
            }
        }

        private void DrawSleevesCustom(Farmer who, LayerData layer)
        {
            var sleevesModel = layer.AppearanceModel as SleevesModel;
            var sleevesModelPack = sleevesModel.Pack as SleevesContentPack;

            // Adjust color if needed
            var modelColor = layer.Colors.Count == 0 ? Color.White : layer.Colors[0];
            if (sleevesModel.DisableGrayscale)
            {
                modelColor = Color.White;
            }
            else if (sleevesModel.IsPrismatic)
            {
                modelColor = Utility.GetPrismaticColor(speedMultiplier: sleevesModel.PrismaticAnimationSpeedMultiplier);
            }

            // Get any positional offset
            Position positionOffset = GetPositionOffset(sleevesModel);

            // Get any feature offset
            var featureOffset = GetFeatureOffset(_facingDirection, _currentFrame, _scale, _farmerRenderer, sleevesModel, who);
            featureOffset.Y -= who.IsMale ? 4 : 0; // Manually adjusting for male sleeves

            DrawSleevesCustom(who, sleevesModel, sleevesModelPack, modelColor, positionOffset, featureOffset, GetSourceRectangle(sleevesModel, _appearanceTypeToAnimationModels));
            if (_appearanceTypeToAnimationModels.TryGetValue(sleevesModel, out var animationModel) is true && animationModel is not null)
            {
                foreach (var subFrame in animationModel.SubFrames.Where(s => s.Handling is SubFrame.Type.Normal))
                {
                    DrawSleevesCustom(who, sleevesModel, sleevesModelPack, modelColor, positionOffset, featureOffset, GetSourceRectangle(sleevesModel, _appearanceTypeToAnimationModels, subFrame));
                }

                var slingshotFrontArmFrame = animationModel.SubFrames.FirstOrDefault(s => s.Handling is SubFrame.Type.SlingshotBackArm);
                var slingshotBackArmFrame = animationModel.SubFrames.FirstOrDefault(s => s.Handling is SubFrame.Type.SlingshotFrontArm);

                if (slingshotFrontArmFrame is not null || slingshotBackArmFrame is not null)
                {
                    DrawSlingshotCustom(who, sleevesModel, sleevesModelPack, _areColorMasksPendingRefresh, positionOffset, featureOffset, modelColor, GetSourceRectangle(sleevesModel, _appearanceTypeToAnimationModels, slingshotBackArmFrame), GetSourceRectangle(sleevesModel, _appearanceTypeToAnimationModels, slingshotFrontArmFrame));
                }
            }
        }

        private void DrawSleevesCustom(Farmer who, SleevesModel sleevesModel, SleevesContentPack sleevesModelPack, Color modelColor, Position positionOffset, Vector2 featureOffset, Rectangle customSleevesSourceRect)
        {
            _spriteBatch.Draw(sleevesModelPack.Texture, GetScaledPosition(_position + who.armOffset, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset, customSleevesSourceRect, sleevesModel.HasColorMask() ? Color.White : modelColor, _rotation, _origin + new Vector2(positionOffset.X, positionOffset.Y), sleevesModel.Scale * _scale, sleevesModel.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, IncrementAndGetLayerDepth());

            if ((sleevesModel.HasColorMask() || sleevesModel.HasShirtToneMask()) && sleevesModel.UseShirtColors)
            {
                // Get the shirt model, if applicable
                ShirtModel shirtModel = null;
                if (who.modData.ContainsKey(ModDataKeys.CUSTOM_SHIRT_ID) && FashionSense.textureManager.GetSpecificAppearanceModel<ShirtContentPack>(who.modData[ModDataKeys.CUSTOM_SHIRT_ID]) is ShirtContentPack sPack && sPack != null)
                {
                    shirtModel = sPack.GetShirtFromFacingDirection(_facingDirection);
                }

                if (shirtModel is not null && shirtModel.SleeveColors is not null)
                {
                    DrawSleeveColorMask(_spriteBatch, sleevesModelPack, sleevesModel, shirtModel, _areColorMasksPendingRefresh, who, GetScaledPosition(_position + who.armOffset, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset, customSleevesSourceRect, modelColor, _rotation, _origin + new Vector2(sleevesModel.BodyPosition.X, sleevesModel.BodyPosition.Y), sleevesModel.Scale * _scale, IncrementAndGetLayerDepth());
                }
                else
                {
                    DrawSleeveColorMaskVanilla(_spriteBatch, sleevesModelPack, sleevesModel, _areColorMasksPendingRefresh, who, GetScaledPosition(_position + who.armOffset, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset, customSleevesSourceRect, modelColor, _rotation, _origin + new Vector2(sleevesModel.BodyPosition.X, sleevesModel.BodyPosition.Y), sleevesModel.Scale * _scale, IncrementAndGetLayerDepth());
                }
            }
            else
            {
                DrawColorMask(_spriteBatch, sleevesModelPack, sleevesModel, _areColorMasksPendingRefresh, GetScaledPosition(_position + who.armOffset, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset, customSleevesSourceRect, modelColor, new List<Color>(), _rotation, _origin + new Vector2(positionOffset.X, positionOffset.Y), sleevesModel.Scale * _scale, IncrementAndGetLayerDepth());
            }

            if (sleevesModel.HasSkinToneMask())
            {
                DrawSkinToneMask(_spriteBatch, sleevesModelPack, sleevesModel, _skinToneModel, _areColorMasksPendingRefresh, GetScaledPosition(_position + who.armOffset, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset, customSleevesSourceRect, modelColor, _rotation, _origin + new Vector2(positionOffset.X, positionOffset.Y), sleevesModel.Scale * _scale, IncrementAndGetLayerDepth());
            }
        }

        private void DrawSlingshotCustom(Farmer who, SleevesModel sleevesModel, SleevesContentPack sleevesContentPack, bool areColorMasksPendingRefresh, Position positionOffset, Vector2 featureOffset, Color modelColor, Rectangle frontArmSourceRectangle, Rectangle backArmSourceRectangle)
        {
            _spriteBatch.Draw(_baseTexture, _position + _origin + _positionOffset + who.armOffset, new Rectangle(_farmerSourceRectangle.X + (_animationFrame.secondaryArm ? 192 : 96), _farmerSourceRectangle.Y, _farmerSourceRectangle.Width, _farmerSourceRectangle.Height), _overrideColor, _rotation, _origin, 4f * _scale, _animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, IncrementAndGetLayerDepth());

            // Handle drawing slingshot
            if (who.usingSlingshot is false || who.CurrentTool is not Slingshot)
            {
                return;
            }

            Slingshot slingshot = who.CurrentTool as Slingshot;
            Point point = Utility.Vector2ToPoint(slingshot.AdjustForHeight(Utility.PointToVector2(slingshot.aimPos.Value)));
            int mouseX = point.X;
            int y = point.Y;
            int backArmDistance = slingshot.GetBackArmDistance(who);

            Vector2 shoot_origin = slingshot.GetShootOrigin(who);
            float frontArmRotation = (float)Math.Atan2((float)y - shoot_origin.Y, (float)mouseX - shoot_origin.X) + (float)Math.PI;
            if (Game1.options.useLegacySlingshotFiring is false)
            {
                frontArmRotation -= (float)Math.PI;
                if (frontArmRotation < 0f)
                {
                    frontArmRotation += (float)Math.PI * 2f;
                }
            }

            // Collect textures to draw, if needed
            if (sleevesContentPack.CollectiveMaskTexture is null || areColorMasksPendingRefresh)
            {
                List<Texture2D> textures = new List<Texture2D>()
                {
                    sleevesContentPack.ColorMaskTextures is not null ? sleevesContentPack.ColorMaskTextures[0] : null,
                    sleevesContentPack.ShirtToneTexture is not null ? sleevesContentPack.ShirtToneTexture : null,
                    sleevesContentPack.SkinMaskTexture is not null ? sleevesContentPack.SkinMaskTexture : null
                };

                Color[] data = new Color[sleevesContentPack.Texture.Width * sleevesContentPack.Texture.Height];
                sleevesContentPack.Texture.GetData(data);
                foreach (var texture in textures)
                {
                    if (texture is null)
                    {
                        continue;
                    }

                    Color[] subData = new Color[texture.Width * texture.Height];
                    texture.GetData(subData);

                    for (int i = 0; i < data.Length; i++)
                    {
                        if (subData[i] != Color.Transparent)
                        {
                            data[i] = subData[i];

                            if (texture == sleevesContentPack.ColorMaskTextures[0])
                            {
                                data[i] = Color.Lerp(data[i], modelColor, 0.5f);
                            }
                        }
                    }
                }

                Texture2D collectiveTexture = new Texture2D(Game1.graphics.GraphicsDevice, sleevesContentPack.Texture.Width, sleevesContentPack.Texture.Height);
                collectiveTexture.SetData(data);
                sleevesContentPack.CollectiveMaskTexture = collectiveTexture;
            }

            // Draw the slingshot
            switch (_facingDirection)
            {
                case 0:
                    _spriteBatch.Draw(sleevesContentPack.CollectiveMaskTexture, GetScaledPosition(_position, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset + new Vector2((frontArmRotation * 8f) - 12f, -44f), frontArmSourceRectangle, Color.White, 0f, _origin + new Vector2(positionOffset.X, positionOffset.Y), sleevesModel.Scale * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                    break;
                case 1:
                    {
                        _spriteBatch.Draw(sleevesContentPack.CollectiveMaskTexture, GetScaledPosition(_position, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset + new Vector2(-backArmDistance, 0f), backArmSourceRectangle, Color.White, 0f, _origin + new Vector2(positionOffset.X, positionOffset.Y), sleevesModel.Scale * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                        _spriteBatch.Draw(sleevesContentPack.CollectiveMaskTexture, GetScaledPosition(_position, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset + new Vector2(36f, -40f), frontArmSourceRectangle, Color.White, frontArmRotation, _origin + new Vector2(positionOffset.X, positionOffset.Y), sleevesModel.Scale * _scale, SpriteEffects.None, IncrementAndGetLayerDepth() - 0.1f);
                        int slingshotAttachX = (int)(Math.Cos(frontArmRotation + (float)Math.PI / 2f) * (double)(20 - backArmDistance - 8) - Math.Sin(frontArmRotation + (float)Math.PI / 2f) * -68.0);
                        int slingshotAttachY = (int)(Math.Sin(frontArmRotation + (float)Math.PI / 2f) * (double)(20 - backArmDistance - 8) + Math.Cos(frontArmRotation + (float)Math.PI / 2f) * -68.0);
                        Utility.drawLineWithScreenCoordinates((int)(_position.X + 52f - (float)backArmDistance), (int)(_position.Y - 32f - 4f), (int)(_position.X + 32f + (float)(slingshotAttachX / 2)), (int)(_position.Y - 32f - 12f + (float)(slingshotAttachY / 2)), _spriteBatch, Color.White);
                        break;
                    }
                case 3:
                    {
                        _spriteBatch.Draw(sleevesContentPack.CollectiveMaskTexture, GetScaledPosition(_position, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset + new Vector2(backArmDistance, 0f), backArmSourceRectangle, Color.White, 0f, _origin + new Vector2(positionOffset.X, positionOffset.Y), sleevesModel.Scale * _scale, SpriteEffects.FlipHorizontally, IncrementAndGetLayerDepth());
                        _spriteBatch.Draw(sleevesContentPack.CollectiveMaskTexture, GetScaledPosition(_position, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset + new Vector2(24f, -36f), frontArmSourceRectangle, Color.White, frontArmRotation + (float)Math.PI, _origin + new Vector2(positionOffset.X, positionOffset.Y) + new Vector2(16f, 0f), sleevesModel.Scale * _scale, SpriteEffects.FlipHorizontally, IncrementAndGetLayerDepth() - 0.1f);
                        int slingshotAttachX = (int)(Math.Cos(frontArmRotation + (float)Math.PI * 2f / 5f) * (double)(20 + backArmDistance - 8) - Math.Sin(frontArmRotation + (float)Math.PI * 2f / 5f) * -68.0);
                        int slingshotAttachY = (int)(Math.Sin(frontArmRotation + (float)Math.PI * 2f / 5f) * (double)(20 + backArmDistance - 8) + Math.Cos(frontArmRotation + (float)Math.PI * 2f / 5f) * -68.0);
                        Utility.drawLineWithScreenCoordinates((int)(_position.X + 4f + (float)backArmDistance), (int)(_position.Y - 32f - 8f), (int)(_position.X + 26f + (float)slingshotAttachX * 4f / 10f), (int)(_position.Y - 32f - 8f + (float)slingshotAttachY * 4f / 10f), _spriteBatch, Color.White);
                        break;
                    }
                case 2:
                    _spriteBatch.Draw(sleevesContentPack.CollectiveMaskTexture, GetScaledPosition(_position, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset + new Vector2(4f, -backArmDistance / 2), backArmSourceRectangle, Color.White, 0f, _origin + new Vector2(positionOffset.X, positionOffset.Y), sleevesModel.Scale * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                    Utility.drawLineWithScreenCoordinates((int)(_position.X + 16f), (int)(_position.Y - 28f - (float)(backArmDistance / 2)), (int)(_position.X + 44f - frontArmRotation * 10f), (int)(_position.Y - 16f - 8f), _spriteBatch, Color.White);
                    Utility.drawLineWithScreenCoordinates((int)(_position.X + 16f), (int)(_position.Y - 28f - (float)(backArmDistance / 2)), (int)(_position.X + 56f - frontArmRotation * 10f), (int)(_position.Y - 16f - 8f), _spriteBatch, Color.White);
                    _spriteBatch.Draw(sleevesContentPack.CollectiveMaskTexture, GetScaledPosition(_position, sleevesModel, _isDrawingForUI) + _origin + _positionOffset + featureOffset + new Vector2(24f - frontArmRotation * 10f, 0f), frontArmSourceRectangle, Color.White, 0f, _origin + new Vector2(positionOffset.X, positionOffset.Y), sleevesModel.Scale * _scale, SpriteEffects.None, IncrementAndGetLayerDepth());
                    break;
            }
        }

        internal static void DrawColorMask(SpriteBatch b, AppearanceContentPack appearancePack, AppearanceModel appearanceModel, bool areColorMasksPendingRefresh, Vector2 position, Rectangle sourceRect, Color? colorOverride, List<Color> colors, float rotation, Vector2 origin, float scale, float layerDepth)
        {
            if (appearancePack.ColorMaskTextures is null || areColorMasksPendingRefresh)
            {
                var colorMaskTextures = new List<Texture2D>();
                for (int x = 0; x < appearanceModel.ColorMaskLayers.Count; x++)
                {
                    Color[] data = new Color[appearancePack.Texture.Width * appearancePack.Texture.Height];
                    appearancePack.Texture.GetData(data);
                    Texture2D maskedTexture = new Texture2D(Game1.graphics.GraphicsDevice, appearancePack.Texture.Width, appearancePack.Texture.Height);

                    for (int i = 0; i < data.Length; i++)
                    {
                        if (!appearanceModel.IsMaskedColor(data[i], layerIndexToCheck: x))
                        {
                            data[i] = Color.Transparent;
                        }
                    }

                    maskedTexture.SetData(data);
                    colorMaskTextures.Add(maskedTexture);
                }
                appearancePack.ColorMaskTextures = colorMaskTextures;
            }

            for (int t = 0; t < appearancePack.ColorMaskTextures.Count; t++)
            {
                var colorToUse = Color.White;
                if (colorOverride is not null)
                {
                    colorToUse = colorOverride.Value;
                }
                else if (colors.Count > t)
                {
                    colorToUse = colors[t];
                }
                b.Draw(appearancePack.ColorMaskTextures[t], position, sourceRect, colorToUse, rotation, origin, scale, appearanceModel.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
            }
        }

        private void CreateSleeveMask(Farmer who, SleevesModel sleevesModel, ShirtModel shirtModel, ref Color[] data)
        {
            Color firstSleeveColor;
            Color secondSleeveColor;
            Color thirdSleeveColor;
            if (shirtModel is null)
            {
                var shirtSleeveColors = AppearanceHelpers.GetVanillaShirtSleeveColors(who, _farmerRenderer);
                firstSleeveColor = shirtSleeveColors[0];
                secondSleeveColor = shirtSleeveColors[1];
                thirdSleeveColor = shirtSleeveColors[2];
            }
            else
            {
                firstSleeveColor = shirtModel.GetSleeveColor(0);
                secondSleeveColor = shirtModel.GetSleeveColor(1);
                thirdSleeveColor = shirtModel.GetSleeveColor(2);
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (sleevesModel.ShirtToneMask is not null)
                {
                    if (sleevesModel.IsShirtToneMaskColor(data[i]) is false)
                    {
                        data[i] = Color.Transparent;
                        continue;
                    }

                    if (data[i] == sleevesModel.ShirtToneMask.Darkest && (shirtModel is null || shirtModel.HasSleeveColorAtLayer(0)))
                    {
                        data[i] = firstSleeveColor;
                    }
                    else if (data[i] == sleevesModel.ShirtToneMask.Medium && (shirtModel is null || shirtModel.HasSleeveColorAtLayer(1)))
                    {
                        data[i] = secondSleeveColor;
                    }
                    else if (data[i] == sleevesModel.ShirtToneMask.Lightest && (shirtModel is null || shirtModel.HasSleeveColorAtLayer(2)))
                    {
                        data[i] = thirdSleeveColor;
                    }
                }
                else if (sleevesModel.ColorMaskLayers is not null)
                {
                    if (sleevesModel.IsMaskedColor(data[i], true) is false)
                    {
                        data[i] = Color.Transparent;
                        continue;
                    }

                    if (data[i] == sleevesModel.GetColorMaskByIndex(layerIndex: 0, maskIndex: 0) && (shirtModel is null || shirtModel.HasSleeveColorAtLayer(0)))
                    {
                        data[i] = firstSleeveColor;
                    }
                    else if (data[i] == sleevesModel.GetColorMaskByIndex(layerIndex: 0, maskIndex: 1) && (shirtModel is null || shirtModel.HasSleeveColorAtLayer(1)))
                    {
                        data[i] = secondSleeveColor;
                    }
                    else if (data[i] == sleevesModel.GetColorMaskByIndex(layerIndex: 0, maskIndex: 2) && (shirtModel is null || shirtModel.HasSleeveColorAtLayer(2)))
                    {
                        data[i] = thirdSleeveColor;
                    }
                }
            }
        }

        private void DrawSleeveColorMask(SpriteBatch b, SleevesContentPack sleevesPack, SleevesModel sleevesModel, ShirtModel shirtModel, bool areColorMasksPendingRefresh, Farmer who, Vector2 position, Rectangle sourceRect, Color color, float rotation, Vector2 origin, float scale, float layerDepth)
        {
            if (sleevesPack.ShirtToneTexture is null || areColorMasksPendingRefresh)
            {
                Color[] data = new Color[sleevesPack.Texture.Width * sleevesPack.Texture.Height];
                sleevesPack.Texture.GetData(data);
                Texture2D maskedTexture = new Texture2D(Game1.graphics.GraphicsDevice, sleevesPack.Texture.Width, sleevesPack.Texture.Height);

                CreateSleeveMask(who, sleevesModel, shirtModel, ref data);

                maskedTexture.SetData(data);
                sleevesPack.ShirtToneTexture = maskedTexture;
            }

            b.Draw(sleevesPack.ShirtToneTexture, position, sourceRect, Color.White, rotation, origin, scale, sleevesModel.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        }

        private void DrawSleeveColorMaskVanilla(SpriteBatch b, SleevesContentPack sleevesPack, SleevesModel sleevesModel, bool areColorMasksPendingRefresh, Farmer who, Vector2 position, Rectangle sourceRect, Color color, float rotation, Vector2 origin, float scale, float layerDepth)
        {
            if (sleevesPack.ShirtToneTexture is null || areColorMasksPendingRefresh)
            {
                Color[] data = new Color[sleevesPack.Texture.Width * sleevesPack.Texture.Height];
                sleevesPack.Texture.GetData(data);
                Texture2D maskedTexture = new Texture2D(Game1.graphics.GraphicsDevice, sleevesPack.Texture.Width, sleevesPack.Texture.Height);

                CreateSleeveMask(who, sleevesModel, shirtModel: null, ref data);

                maskedTexture.SetData(data);
                sleevesPack.ShirtToneTexture = maskedTexture;
            }

            b.Draw(sleevesPack.ShirtToneTexture, position, sourceRect, Color.White, rotation, origin, scale, sleevesModel.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        }

        internal static void DrawSkinToneMask(SpriteBatch b, AppearanceContentPack appearancePack, AppearanceModel appearanceModel, SkinToneModel skinTone, bool areColorMasksPendingRefresh, Vector2 position, Rectangle sourceRect, Color color, float rotation, Vector2 origin, float scale, float layerDepth)
        {
            if (appearancePack.SkinMaskTexture is null || areColorMasksPendingRefresh)
            {
                Color[] data = new Color[appearancePack.Texture.Width * appearancePack.Texture.Height];
                appearancePack.Texture.GetData(data);
                Texture2D maskedTexture = new Texture2D(Game1.graphics.GraphicsDevice, appearancePack.Texture.Width, appearancePack.Texture.Height);

                for (int i = 0; i < data.Length; i++)
                {
                    if (!appearanceModel.IsSkinToneMaskColor(data[i]))
                    {
                        data[i] = Color.Transparent;
                    }
                    else if (appearanceModel.SkinToneMasks is not null)
                    {
                        if (appearanceModel.SkinToneMasks.LightTone is not null && data[i] == appearanceModel.SkinToneMasks.Lightest)
                        {
                            data[i] = skinTone.Lightest;
                        }
                        else if (appearanceModel.SkinToneMasks.MediumTone is not null && data[i] == appearanceModel.SkinToneMasks.Medium)
                        {
                            data[i] = skinTone.Medium;
                        }
                        else if (appearanceModel.SkinToneMasks.DarkTone is not null && data[i] == appearanceModel.SkinToneMasks.Darkest)
                        {
                            data[i] = skinTone.Darkest;
                        }
                    }
                }

                maskedTexture.SetData(data);
                appearancePack.SkinMaskTexture = maskedTexture;
            }

            b.Draw(appearancePack.SkinMaskTexture, position, sourceRect, appearanceModel.DisableSkinGrayscale ? Color.White : color, rotation, origin, scale, appearanceModel.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        }
        #endregion

        #region Helper methods
        private Position GetPositionOffset(AppearanceModel model)
        {
            switch (model)
            {
                case PantsModel pantsModel:
                    return pantsModel.BodyPosition;
                case ShoesModel shoesModel:
                    return shoesModel.BodyPosition;
                case ShirtModel shirtModel:
                    return shirtModel.BodyPosition;
                case AccessoryModel accessoryModel:
                    return accessoryModel.HeadPosition;
                case HairModel hairModel:
                    return hairModel.HeadPosition;
                case SleevesModel sleevesModel:
                    return sleevesModel.BodyPosition;
                case HatModel hatModel:
                    return hatModel.HeadPosition;
            }
            return new Position();
        }

        private Rectangle GetSourceRectangle(AppearanceModel model, Dictionary<AppearanceModel, AnimationModel> appearanceTypeToAnimationModels, SubFrame subFrame = null)
        {
            var size = AppearanceHelpers.GetModelSize(model);
            Rectangle sourceRectangle = new Rectangle(model.StartingPosition.X, model.StartingPosition.Y, size.Width, size.Length);

            if (appearanceTypeToAnimationModels.TryGetValue(model, out var animation) is false || animation is null)
            {
                return sourceRectangle;
            }

            return AppearanceHelpers.GetAdjustedSourceRectangle(animation, model.Pack, sourceRectangle, subFrame);
        }

        private Vector2 GetFeatureOffset(int facingDirection, int currentFrame, float scale, FarmerRenderer renderer, AppearanceModel model, Farmer who)
        {
            // Determine if sprite his flipped
            bool flip = false;
            if (model is HairModel)
            {
                flip = true;
            }
            else if (model is HatModel && who.FarmerSprite.CurrentAnimationFrame.flip)
            {
                flip = true;
            }

            // Establish the base offset
            Vector2 offset = Vector2.Zero;

            // Return without any further modifications if DisableNativeOffset is true
            if (model.DisableNativeOffset)
            {
                return offset;
            }

            var type = model.GetPackType();
            if (type is AppearanceContentPack.Type.Hat)
            {
                return new Vector2(-8 + ((!flip) ? 1 : (-1)) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 4 + (int)renderer.heightOffset.Value);
            }

            if (type is AppearanceContentPack.Type.Shirt)
            {
                switch (facingDirection)
                {
                    case 0:
                        return new Vector2(16f * scale + (float)(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float)(56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)renderer.heightOffset.Value * scale);
                    case 1:
                        return new Vector2(16f * scale + (float)(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), 56f * scale + (float)(FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)renderer.heightOffset.Value * scale);
                    case 2:
                        return new Vector2(16 + FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, (float)(56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)renderer.heightOffset.Value * scale);
                    case 3:
                        return new Vector2(16 - FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)renderer.heightOffset.Value);
                }
            }
            else if (type is not AppearanceContentPack.Type.Sleeves)
            {
                switch (facingDirection)
                {
                    case 0:
                        offset = new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4);
                        break;
                    case 1:
                        offset = new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 4 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4);
                        break;
                    case 2:
                        offset = new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4);
                        break;
                    case 3:
                        offset = new Vector2(-FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, (flip ? 4 : 0) + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4);
                        break;
                }
            }

            if (type is AppearanceContentPack.Type.Accessory or AppearanceContentPack.Type.AccessorySecondary or AppearanceContentPack.Type.AccessoryTertiary)
            {
                switch (facingDirection)
                {
                    case 0:
                    case 1:
                        break;
                    case 2:
                    case 3:
                        offset.Y += 4;
                        break;
                }

                offset.Y += renderer.heightOffset.Value;
            }

            return offset;
        }

        private Vector2 GetScaledPosition(Vector2 position, AppearanceModel model, bool isDrawingForUI)
        {
            if (isDrawingForUI)
            {
                position.Y += (4f - model.Scale) * 32;
            }

            return position;
        }

        private float IncrementAndGetLayerDepth()
        {
            LayerDepth += 0.0001f;
            return LayerDepth;
        }
        #endregion
    }
}
