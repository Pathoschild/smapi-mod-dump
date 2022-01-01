**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/mus-candidus/ImagEd**

----

# ImagEd

Mod for [Stardew Valley](http://stardewvalley.net/) which provides non-interactive image recoloring. Requires [ContentPatcher](https://www.nexusmods.com/stardewvalley/mods/1915).

Provides the token **ImageEd/Recolor** which takes an image to recolor and optionally a mask and a desaturation mode. The final image loaded by ContentPatcher is created as following:

- The source image is loaded, either from content pack or from game content. 
- A sub image of the source image is extracted using the given mask. If no mask is given, a copy is created.
- The extracted sub image is optionally desaturized using the given mode (HSV, HSL, HSI or Luma, see [HSL_and_HSV](https://en.wikipedia.org/wiki/HSL_and_HSV) for details on the algorithms).
- The desaturized image is multiplied with the given color.
- The recolored image is optionally flipped.
- The result is written to disk so it can loaded by ContentPatcher.

**ImagEd/Recolor** takes 7 arguments:

1. Content pack that uses recoloring. This is usually the content pack that contains the current config.json but a custom CP token doesn't have that information so we must provide it.
2. Asset name.
3. Source image or "gamecontent" to load a vanilla asset.
4. Mask image or "none". Mask is supposed to be a grayscale image so we always desaturate it (mode is DesaturateLuma).
5. Blend color in #RRGGBB format ([Hex triplet](https://en.wikipedia.org/wiki/Web_colors#Hex_triplet)). The non-standard format #RRGGBBAA is supported to set an alpha value.
6. Desaturation mode: DesaturateHSV, DesaturateHSL, DesaturateHSI, DesaturateLuma or None. This is an optional argument, default is None.
7. Flip mode: FlipHorizontally, FlipVertically, FlipBoth or None. This is an optional argument, default is None. Attention: This feature is not useful in most cases, especially not for NPCs. The whole image is flipped so if you flip a sprite sheet all sprite indexes are wrong afterwards. Nonetheless you can do cool things with flipping. You have been warned :-)

Example:

    {
        "Action": "EditImage",
        "Target": "Characters/Emily",
        "FromFile": "{{Candidus42.ImagEd/Recolor: Candidus42.EmilyDressRecolor, Characters/Emily, gamecontent, assets/Characters/Emily_dress_mask.png, #FFFF00, DesaturateHSV}}",
        "PatchMode": "Overlay"
    }
