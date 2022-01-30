/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

namespace SDV.Shared.Abstractions
{
  public interface IAnimationFrameWrapper
  {
    IAnimationFrameWrapper AddFrameAction(
      AnimatedSpriteWrapper.endOfAnimationBehavior callback);

    IAnimationFrameWrapper AddFrameEndAction(
      AnimatedSpriteWrapper.endOfAnimationBehavior callback);
  }
}