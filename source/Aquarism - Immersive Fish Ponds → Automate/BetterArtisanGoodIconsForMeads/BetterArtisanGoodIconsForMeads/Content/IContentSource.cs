/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

using StardewModdingAPI;

namespace BetterArtisanGoodIconsForMeads.Content;

internal interface IContentSource
{
	T Load<T>(string path);

	IManifest GetManifest();
}
