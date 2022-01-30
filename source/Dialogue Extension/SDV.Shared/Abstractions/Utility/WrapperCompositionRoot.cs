/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using LightInject;

namespace SDV.Shared.Abstractions.Utility
{
  public class WrapperCompositionRoot : ICompositionRoot
  {
    public void Compose(IServiceRegistry serviceRegistry)
    {
      serviceRegistry
        .Register<IAnimatedSpriteWrapper, AnimatedSpriteWrapper>()
        .Register<IAnimationFrameWrapper, AnimationFrameWrapper>()
        .Register<ICharacterWrapper, CharacterWrapper>()
        .Register<IFarmerSpriteWrapper, FarmerSpriteWrapper>()
        .Register<IGameLocationWrapper, GameLocationWrapper>()
        .Register<IGameWrapper, GameWrapper>()
        .Register<IHorseWrapper, HorseWrapper>()
        .Register<IModDataDictionaryWrapper, ModDataDictionaryWrapper>()
        .Register<INPCWrapper, NPCWrapper>()
        .Register<IWrapperFactory, WrapperFactory>();
    }
  }
}