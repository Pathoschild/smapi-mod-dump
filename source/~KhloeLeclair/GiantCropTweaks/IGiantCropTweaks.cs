/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

namespace Leclair.Stardew.GiantCropTweaks;


/// <summary>
/// Controls how Giant Crop Tweaks decides whether a giant crop
/// should re-plant the original crop beneath it when harvested.
/// </summary>
public enum ReplantBehavior {
	/// <summary>Never re-plant.</summary>
	Never,
	/// <summary>Always re-plant.</summary>
	Always,
	/// <summary>If the original crop is a regrowing crop, re-plant.</summary>
	WhenRegrowing
};

public interface IExtraGiantCropData {

	/// <summary>
	/// Must match the ID of an existing <see cref="GiantCrops"/>
	/// instance in <c>Data/GiantCrops</c>.
	/// </summary>
	string Id { get; set; }

	#region Behaviors

	/// <summary>
	/// Whether or not this giant crop can grow if the source crop
	/// is fully grown, but has been harvested and has
	/// not yet regrown. By default, this is true, but you may
	/// want to turn this down for regrowable crops with high
	/// chances of growing giant.
	/// </summary>
	bool CanGrowWhenNotFullyRegrown { get; set; }

	/// <summary>
	/// Whether or not this giant crop should be replanted when harvested.
	/// By default, giant crops will be replanted if the base crop is
	/// configured to re-grow.
	/// </summary>
	ReplantBehavior ShouldReplant { get; set; }

	#endregion

	#region Colors

	/// <summary>
	/// An optional list of colors to apply to this giant crop. The color
	/// will be used for rendering the overlay texture, and may also be
	/// used to color the harvest items.
	/// </summary>
	List<Color>? Colors { get; set; }

	/// <summary>
	/// If this is set to true, we will attempt to pick this giant crop's
	/// color based on the <see cref="CropData.TintColors"/> of the
	/// crop matching <see cref="GiantCropData.FromItemId"/>. This
	/// overrides <see cref="Colors"/> if set.
	/// </summary>
	bool UseBaseCropTintColors { get; set; }

	/// <summary>
	/// When this giant crop is harvested, any items with item Ids in
	/// this list will be converted to colored items using this giant
	/// crop's color.
	/// </summary>
	List<string>? HarvestItemsToColor { get; set; }

	/// <summary>
	/// If this is true, each individual entry in <see cref="GiantCropData.HarvestItems"/>
	/// will have its color randomized to allow more than one
	/// color of item to drop. Only takes effect to items listed
	/// in <see cref="HarvestItemsToColor"/>.
	/// </summary>
	bool RandomizeHarvestItemColors { get; set; }

	#endregion

	#region Overlay Texture

	/// <summary>
	/// An optional texture for an overlay. If this is set, a second layer
	/// will be drawn with this texture.
	/// </summary>
	string? OverlayTexture { get; set; }

	/// <summary>
	/// Whether or not the overlay should be drawn as prismatic. If this is
	/// true, the overlay color will not be used for rendering.
	/// </summary>
	bool OverlayPrismatic { get; set; }

	/// <summary>
	/// The top-left pixel position of the overlay sprite within the texture.
	/// If this is null, <see cref="GiantCropData.TexturePosition"/> will
	/// be used instead.
	/// </summary>
	Point? OverlayPosition { get; set; }

	/// <summary>
	/// The size of the overlay sprite, in tiles. If this is null,
	/// <see cref="GiantCropData.TileSize"/> will be used instead.
	/// </summary>
	Point? OverlaySize { get; set; }

	/// <summary>
	/// The number of tiles the overlay should be offset from the
	/// base sprite.
	/// </summary>
	Point OverlayOffset { get; set; }

	#endregion

}


public interface IExtraGiantCropDataEditor : IDictionary<string, IExtraGiantCropData> {

	IExtraGiantCropData GetOrCreate(string key);


}


public interface IGiantCropTweaks {

	/// <summary>
	/// Create a new <see cref="IExtraGiantCropData"/> instance. Note that this
	/// does not add it to the data dictionary, but merely creates an instance.
	/// To edit the dictionary, use the asset requested event and the method
	/// here to get an editor.
	/// </summary>
	/// <returns>A new, blank data model.</returns>
	IExtraGiantCropData CreateNew();

	/// <summary>
	/// Get an editor for editing GCT's data dictionary. This is intended to
	/// be used within the asset requested event.
	/// </summary>
	/// <param name="assetData">The asset data you get when editing an
	/// asset in the asset requested event.</param>
	/// <returns>A dictionary-like interface for editing the asset.</returns>
	IExtraGiantCropDataEditor GetEditor(IAssetData assetData);

	/// <summary>
	/// Get all the entries in the loaded data dictionary.
	/// </summary>
	IEnumerable<KeyValuePair<string, IExtraGiantCropData>> GetData();

	/// <summary>
	/// Try to get the data for a specific giant crop, if it exists.
	/// </summary>
	/// <param name="key">The crop Id to search for.</param>
	/// <param name="data">The crop data, if it exists.</param>
	/// <returns>Whether or not it was found.</returns>
	bool TryGetData(string key, [NotNullWhen(true)] out IExtraGiantCropData? data);


}
