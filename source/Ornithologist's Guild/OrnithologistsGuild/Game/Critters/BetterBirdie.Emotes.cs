/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using Microsoft.Xna.Framework;

namespace OrnithologistsGuild.Game.Critters
{
	public partial class BetterBirdie
	{
        public bool isEmoting;

        public int currentEmote;

        public int currentEmoteFrame;

        public float emoteInterval;

        private bool emoteFading;

        public void updateEmote(GameTime time)
        {
            if (!isEmoting)
            {
                return;
            }
            emoteInterval += time.ElapsedGameTime.Milliseconds;
            if (emoteFading && emoteInterval > 20f)
            {
                emoteInterval = 0f;
                currentEmoteFrame--;
                if (currentEmoteFrame < 0)
                {
                    emoteFading = false;
                    isEmoting = false;
                }
            }
            else if (!emoteFading && emoteInterval > 20f && currentEmoteFrame <= 3)
            {
                emoteInterval = 0f;
                currentEmoteFrame++;
                if (currentEmoteFrame == 4)
                {
                    currentEmoteFrame = currentEmote;
                }
            }
            else if (!emoteFading && emoteInterval > 250f)
            {
                emoteInterval = 0f;
                currentEmoteFrame++;
                if (currentEmoteFrame >= currentEmote + 4)
                {
                    emoteFading = true;
                    currentEmoteFrame = 3;
                }
            }
        }

        public virtual void doEmote(int whichEmote)
        {
            if (!isEmoting)
            {
                isEmoting = true;
                currentEmote = whichEmote;
                currentEmoteFrame = 0;
                emoteInterval = 0f;
            }
        }

        public void stopEmote()
        {
            emoteFading = false;
            isEmoting = false;
        }
	}
}

