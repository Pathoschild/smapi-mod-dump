/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class AnimationFrameWrapper : IAnimationFrameWrapper
  {
    public AnimationFrameWrapper(FarmerSprite.AnimationFrame animationFrame) => GetBasType = animationFrame;
    public FarmerSprite.AnimationFrame GetBasType { get; }

    public IAnimationFrameWrapper AddFrameAction(AnimatedSpriteWrapper.endOfAnimationBehavior callback) => null;

    public IAnimationFrameWrapper AddFrameEndAction(AnimatedSpriteWrapper.endOfAnimationBehavior callback) => null;
  }
}
