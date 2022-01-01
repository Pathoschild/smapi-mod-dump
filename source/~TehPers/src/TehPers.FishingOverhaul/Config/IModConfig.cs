/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    internal interface IModConfig
    {
        void Reset();

        void RegisterOptions(IGenericModConfigMenuApi configApi, IManifest manifest, ITranslationHelper translations);
    }
}