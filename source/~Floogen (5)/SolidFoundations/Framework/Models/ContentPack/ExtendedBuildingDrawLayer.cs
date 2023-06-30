/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Models.ContentPack.Actions;
using SolidFoundations.Framework.Utilities.Backport;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Models.ContentPack
{

    // TODO: When using SDV v1.6, this class should inherit StardewValley.GameData.BuildingDrawLayer
    public class ExtendedBuildingDrawLayer : BuildingDrawLayer
    {
        public bool HideBaseTexture { get; set; }
        public bool DrawBehindBase { get; set; }
        public List<Sequence> Sequences { get; set; }
        public string Condition { get; set; }
        public string[] ModDataFlags { get; set; }
        public string[] SkinFilter { get; set; }

        private int _cachedTime;
        private int _elapsedTime;
        private int _currentSequenceIndex;
        private Random _random;

        public Rectangle GetSourceRect(int time, GenericBuilding building)
        {
            if (Sequences is null || Sequences.Count <= _currentSequenceIndex)
            {
                return base.GetSourceRect(time);
            }
            else if (_cachedTime is default(int))
            {
                _cachedTime = time;
                return base.GetSourceRect();
            }

            if (_random is null)
            {
                _random = new Random(Game1.dayOfMonth * 321 + building.tileX.Value * building.tileY.Value + building.tilesWide.Value * building.tilesHigh.Value);
            }

            var sequence = Sequences[_currentSequenceIndex];
            if (_elapsedTime > sequence.GetDuration())
            {
                _elapsedTime = 0;
                _currentSequenceIndex = GetNextValidFrame(building, _currentSequenceIndex);
                sequence = Sequences[_currentSequenceIndex];
                sequence.RefreshDuration(_random);

                // Execute any ModifyFlags
                if (sequence.ModifyFlags is not null)
                {
                    SpecialAction.HandleModifyingBuildingFlags(building, sequence.ModifyFlags);
                }
                if (sequence.PlaySound is not null)
                {
                    SpecialAction.HandlePlayingSound(building, sequence.PlaySound);
                }
            }
            _elapsedTime += time - _cachedTime;
            _cachedTime = time;

            var sourceRect = base.GetSourceRect();
            if (this.FramesPerRow < 0)
            {
                sourceRect.X += sourceRect.Width * sequence.Frame;
            }
            else
            {
                sourceRect.X += sourceRect.Width * (sequence.Frame % this.FramesPerRow);
                sourceRect.Y += sourceRect.Height * (sequence.Frame / this.FramesPerRow);
            }
            return sourceRect;
        }

        public int GetNextValidFrame(GenericBuilding building, int startingValue = 0)
        {
            var currentIndex = startingValue;
            if (currentIndex + 1 < Sequences.Count)
            {
                currentIndex = currentIndex + 1;
            }
            else
            {
                currentIndex = 0;
            }

            var sequence = Sequences[currentIndex];
            if (building.ValidateConditions(sequence.Condition, sequence.ModDataFlags) is false)
            {
                currentIndex = GetNextValidFrame(building, currentIndex);
            }

            return currentIndex;
        }
    }
}
