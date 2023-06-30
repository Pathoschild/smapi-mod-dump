**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jltaylor-us/StardewGMCMOptions**

----

# Stardew Valley GMCM Options

A Stardew Valley SMAP mod that provides additional complex option types for use with Generic Mod Config Menu (GMCM).

You can find this mod on [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/10505)
if you prefer.

The complex options currently supported are:
* [Color Picker](#color-picker)
* [Image Picker](#image-picker)

## How to Use

**For end users:** Mod authors must code their mods to use GMCM Options (much like they had to code their mods to use GMCM).
There is nothing for you to do except install GMCM Options like every other SMAPI mod.

**For mod authors:**

Copy the [API](StardewGMCMOptions/IGMCMOptionsAPI.cs) into your project.
Use GMCM as normal.  When you want to insert one of the complex options provided by GMCM Options, call the appropriate
method from the GMCM Options API.  That's it!

See [Example.cs](StardewGMCMOptions/Example.cs) for a working example.
(If you want to actually see it work in the game, for real, type `gmcmoptions-example` in the SMAPI console to enable it.)

## Complex Options Provided by GMCM Options

### Color Picker

The color picker option supports choosing colors with or without an Alpha (transparency) channel in any of the following
styles:
* RGB sliders
* HSV color wheel
* HSL color wheel

An option can be a single picker style:
![](doc/color-picker-single.png)
Or display multiple picker styles at once:
![](doc/color-picker-multi.png)

When multiple styles are shown for the same option, updates are synchronized between all of the different picker
styles.
![video](doc/color-picker-interactions.mov)

### Image Picker

The image picker option is really an "array index picker" where each index is rendered as an image
(or custom draw function) that you supply.  The underlying value of the option is the `uint` of the
selected index.

Label and arrow location are configurable.

The API provides both a full interface where you can supply a custom drawing function, and a simplified
interface where you need only supply an array of tuples describing the images to be drawn and their labels.

![video](doc/image-picker-attire.mov)

### Horizontal Separator

The horizontal separator option renders a horizontal line in the menu.  It
supports configurable height, padding, alignment, color, and shadow color
(all of which have default values and can be left out).  The "simple"
version allows specifying a fixed values, while the full version allows
for dynamically changing width and color.

![video](doc/horizontal-separator.mov)

### Dynamic Paragraph

A dynamic paragraph reflects changes in its text even while the GMCM
window is open.  (It could, for example, react to changes in other options
and sometimes return an empty string for its text.  A dynamic paragraph
also supports styled text.

![](doc/styled-text.png)

![video](doc/dynamic-styled-text.mov)

Styled text supports simple HTML-like markup for specifying text
formatting.  The text must be valid XML fragment(s).  (I.e., if the text
were enclosed in an XML tag, the result must be a valid XML document.)
Styled text should not have any newlines in the text (at least as of this release).

The following tags are supported (and all others are removed from the text
before it is displayed in the menu).

#### `<b>` - bold text

Text appearing inside a `<b>` tag (i.e., between `<b>` and its matching
`</b>`) is rendered in **bold**.

Example: `some <b>bold</b> text`

#### `<color>` - text color

Text appearing inside a `<color>` tag is colored according to the color
specified in the tag.  The color can be specified either via the `name`
attribute, or the combination of `r`, `g`, and `b` attributes.  (If the
`name` attribute is present then any other attributes are ignored.  If
none of the aforementioned attributes are present then the tag is
ignored.)

Colors specified by name must use a name that is one of the fields of
`Microsoft.Xna.Framework.Color`.  Case is important, but as a convenience,
if the name is not found then the renderer will look for a that name with
its first character uppercased.  Example: `<color
name="blue">true</color>`

Colors specified by RGB value must have at least one of the `r`, `g`, or
`b` attributes.  The attribute values must be whole numbers between 0
and 255.  Any missing or malformed attributes are treated as 0.  Example:
`<color r="255" g="255" b="255">white</color>`

----
## See also
* [Release Notes](doc/ReleaseNotes.md)
