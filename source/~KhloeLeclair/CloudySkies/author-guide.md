**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

← [README](README.md)

Well, look who blew in. Howdy, farmer! Looking to make some custom weather
types for your mod? You've come to the right place!


> Note: This isn't done yet! Sorry.


## Contents

* [Getting Started](#getting-started)
* [What is a Weather Type?](#what-is-a-weather-type)
* [What is a Weather Flag?](#what-is-a-weather-flag)
* [Custom Weather Type](#custom-weather-type)
* [Effects](#effects)
  * [Buff](#buff)
  * [Modify Health](#modifyhealth)
  * [Modify Stamina](#modifystamina)
  * [Trigger](#trigger)
* [Layers](#layers)
  * [Color](#color)
  * [Debris](#debris)
  * [Rain](#rain)
  * [Snow](#snow)
  * [Texture Scroll](#texturescroll)
* [How Do I Make My Weather Happen?](#how-do-i-make-my-weather-happen)
  * [Custom Weather Totems](#custom-weather-totems)
* [Commands](#commands)
* [Mod Data / Custom Fields](#mod-data-custom-fields)
* [Game State Queries](#game-state-queries)
* [Content Patcher Tokens](#content-patcher-tokens)


## Getting Started

Cloudy Skies does not currently support content packs of its own. Instead, I
expect you to use [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915)
to load your custom data.

> Don't know how to use Content Patcher? Head over to check out their
> [excellent documentation](https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/docs/author-guide.md#readme)
> then! They do a much better job than I ever could.

Specifically, you'll be loading textures and using the `EditData` action with
the target:
```
Mods/leclair.cloudyskies/WeatherData
```


## What is a Weather Type?

A weather type is, if you'll forgive the tautology, a type of weather. The base
game has eight weather types:

* `Sun`
* `Rain`
* `Wind`
* `Storm`
* `Snow`
* `Festival`
* `Wedding`
* (and one added in 1.6, not named because it's a spoiler)

Really though, it's more like six. `Festival` and `Wedding` are both duplicates
of `Sun`, just with a little flavor for the occasion.

The Stardew 1.6 update added the ability to use custom Ids for the
weather, but it didn't make it possible to actually *do anything* with those
custom Ids. That's what this mod is for.

In so far as this mod is concerned, a weather type is any behavior associated
with a specific weather flag or weather Id. We make it possible to set up
a lot of those behaviors, and set up the necessary data for other mods to
contribute their own behaviors. (As an example, you could use FTM to spawn
custom foragables in certain weather.)


## What is a Weather Flag?

A weather flag is slightly different than a weather type. Internally, the
game has flags it checks rather than directly looking at the current
weather type's Id. These flags are pretty self explanatory:

* `IsRaining`
* `IsSnowing`
* `IsLightning`
* `IsDebrisWeather` (Debris is the internal name for Wind)
* (and one added in 1.6, not named because it's a spoiler)

The `Storm` weather, for example, sets both the `IsRaining` and
`IsLightning` flags to be true.


## Custom Weather Type

Alright, awesome! So how do you *make a custom weather type*? Glad you asked!
You just need to add an entry to the `Mods/leclair.cloudyskies/WeatherData`
asset with your new weather type's unique Id, and any other data to go with it.

Specifically, each custom weather type supports the following properties:

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr><th colspan=2>Identity</th></tr>
<tr>
<td><code>Id</code></td>
<td>

**Required.** The unique Id of your weather type. This must be unique, and is
used elsewhere to reference this specific weather type.

You should include your mod's Unique Id in this for best results to ensure
there aren't any collisions. When using Content Patcher, you can do this
easily using the `{{ModId}}` token. Example: `{{ModId}}_Sandstorm`

</td>
</tr>
<tr>
<td><code>DisplayName</code></td>
<td>

**Required.** A human readable name to display to the player when this weather
type is referenced by name. This is a [tokenizable string](https://stardewvalleywiki.com/Modding:Tokenizable_strings).

</td>
</tr>
<tr><th colspan=2>Icon</th></tr>
<tr>
<td><code>IconTexture</code></td>
<td>

*Optional.* The name of a texture containing this weather type's icon. The
icon is displayed on the in-game HUD next to the clock when this is the
active weather type in the current location. This may also be used by
other mods, such as UI Info Suite or Almanac.

This should be a texture you've loaded. As an example, you could use
the value `Mods/{{ModId}}/SandstormIcon` and then elsewhere have
the content block:

`{
	"Action": "Load",
	"Target": "Mods/{{ModId}}/SandstormIcon",
	"FromFile": "assets/SandstormIcon.png"
}`

</td>
</tr>
<tr>
<td><code>IconSource</code></td>
<td>

*Optional.* The top-left corner of your weather's icon in the provided
texture. You only supply the X and Y position. An icon is always 12 by 8 pixels.

Default: `{"X": 0, "Y": 0}`

</td>
</tr>
<tr><th colspan=2>Television</th></tr>
<tr>
<td><code>TVTexture</code></td>
<td>

*Optional.* The name of a texture containing this weather type's TV animation.
The animation may be displayed when a player checks the forecast using their
television. This may also be used by other mods.

Like `IconTexture`, this should be a texture you've loaded. It can even be
the same texture, if you use `TVSource` and `IconSource`.

</td>
</tr>
<tr>
<td><code>TVSource</code></td>
<td>

*Optional.* The top-left corner of the first frame of your weather type's TV
animation. Each frame is always 13 by 13 pixels, and the frames are always
laid out as a horizontal strip.

Default: `{"X": 0, "Y": 0}`

</td>
</tr>
<tr>
<td><code>TVFrames</code></td>
<td>

*Optional.* The number of frames your weather type's TV animation has.

Default: `4`

</td>
</tr>
<tr>
<td><code>Forecast</code></td>
<td>

*Optional.* The message to display to the player when they check the
forecast using their TV and this weather type is coming up.
This is a [tokenizable string](https://stardewvalleywiki.com/Modding:Tokenizable_strings).

Example: `"Winds across the desert have blown up thick clouds of sand. Best to stay inside if you can."`

</td>
</tr>
<tr>
<td><code>ForecastByContext</code></td>
<td>

*Optional.* A dictionary letting you override the `Forecast` string for
specific location contexts. This is an object where each key is the Id
of a location context, and the value is a `Forecast` for that context.

Example: `{"Island": "A cloud of dust is blowing towards the island from a desert on a far off continent."}`

</td>
</tr>
<tr><th colspan=2>Weather Totem Behavior</th></tr>
<tr>
<td><code>TotemMessage</code></td>
<td>

*Optional.* A message to display to the player when they use a custom weather
totem for this weather type. If this is not set, the default message used
by the base game's Rain Totem will appear.
This is a [tokenizable string](https://stardewvalleywiki.com/Modding:Tokenizable_strings).

</td>
</tr>
<tr>
<td><code>TotemSound</code></td>
<td>

*Optional.* An [audio cue](https://stardewvalleywiki.com/Modding:Audio) to
play when the player uses a custom weather totem for this weather type. If
this is not set, no sound will play. The base game's Rain Totem uses the
sound: `thunder`

</td>
</tr>
<tr>
<td><code>TotemAfterSound</code></td>
<td>

*Optional.* An [audio cue](https://stardewvalleywiki.com/Modding:Audio) to
play after the player uses a custom weather totem for this weather type. This
plays approximately two seconds after `TotemSound`. If this is not set, no
sound will play. The base game's Rain Totem uses the sound: `rainsound`

</td>
</tr>
<tr>
<td><code>TotemScreenTint</code></td>
<td>

*Optional.* A color to flash the screen when the player uses a custom weather
totem for this weather type. If this is not set, the screen will not flash a
color. The base game's Rain Totem uses the color: `slateblue`

</td>
</tr>
<tr>
<td><code>TotemParticleTexture</code></td>
<td>

*Optional.* The texture name of a texture to display as particles when the
player uses a custom weather totem for this weather type. If this is not set,
no extra particles will be displayed. The base game's Rain Totem uses the
texture: `LooseSprites\Cursors`

</td>
</tr>
<tr>
<td><code>TotemParticleSource</code></td>
<td>

*Optional.* The region of the `TotemParticleTexture` to use when displaying
custom totem particles for this weather type. If this is not set, the entire
texture will be used. The base game's Rain Totem uses the region
`648, 1045, 52, 33` of the `LooseSprites\Cursors` texture.

</td>
</tr>
<tr><th colspan=2>Behavior - Music</th></tr>
<tr>
<td><code>MusicOverride</code></td>
<td>

*Optional.* An [audio cue](https://stardewvalleywiki.com/Modding:Audio) that,
when set, will play in place of the normal in-game music when this weather
condition is active. This mimics the behavior of the base game's raining
weather flag, which plays a rain sound.

Example: `rain`

</td>
</tr>
<tr>
<td><code>MusicFrequencyOutside</code></td>
<td>

*Optional.* The frequency that `MusicOverride` should play at when the player
is standing in an outdoors location. This does not affect all audio cues.

Default: `100`

</td>
</tr>
<tr>
<td><code>MusicFrequencyInside</code></td>
<td>

*Optional.* The frequency that `MusicOverride` should play at when the player
is standing in an indoors location. This does not affect all audio cues. The
base game's raining weather flag uses this with a value of `15` to change
the pitch of the rain sound when you are indoors.

Default: `100`

</td>
</tr>
<tr><th colspan=2>Behavior - Weather Flags</th></tr>
<tr>
<td><code>IsRaining</code></th>
<td>

*Optional.* Whether or not this weather type should apply the `IsRaining`
weather flag.

Default: `false`

</td>
</tr>
<tr>
<td><code>IsRaining</code></th>
<td>

*Optional.* Whether or not this weather type should apply the `IsRaining`
weather flag.

Default: `false`

</td>
</tr>
<tr>
<td><code>IsSnowing</code></th>
<td>

*Optional.* Whether or not this weather type should apply the `IsSnowing`
weather flag.

Default: `false`

</td>
</tr>
<tr>
<td><code>IsLightning</code></th>
<td>

*Optional.* Whether or not this weather type should apply the `IsLightning`
weather flag.

Default: `false`

</td>
</tr>
<tr>
<td><code>IsDebrisWeather</code></th>
<td>

*Optional.* Whether or not this weather type should apply the `IsDebrisWeather`
weather flag.

Default: `false`

</td>
</tr>
<tr>
<td><code>IsGreenRain</code></th>
<td>

*Optional.* Whether or not this weather type should apply the `IsGreenRain`
weather flag.

> Note: You should be careful not to change this value mid-day, as doing
> so may cause the game world to be left in an odd state where temporary
> changes triggered by a green rain day are not reverted.

Default: `false`

</td>
</tr>
<tr><th colspan=2>Behavior - Other</th></tr>
<tr>
<td><code>UseNightTiles</code></td>
<td>

*Optional.* If this is set to true, this weather type will cause maps to
display their night tiles even during the day and to have darkened windows,
similar to how the base game's `IsRaining` weather flag behaves.

For the sake of flexibility, this has been moved into a separate flag for
custom weather types.

Default: `false`

</td>
</tr>
<tr>
<td><code>SpawnCritters</code></td>
<td>

*Optional.* Whether or not critters should be allowed to spawn during this
weather type. Critters are the small animals you'll see around maps, like
birds or squirrels, along with moving cloud shadows and frogs.

Default: `true`

</td>
</tr>
<tr>
<td><code>SpawnFrogs</code></td>
<td>

*Optional.* Whether or not frogs should be allowed to spawn during this
weather type. If this is not set, the game will use the default logic and
check the `IsRaining` weather flag.

</td>
</tr>
<tr>
<td><code>SpawnClouds</code></td>
<td>

*Optional.* Whether or not cloud shadow critters should attempt to spawn.
Yes, those occasional shadows you'll see moving around are technically
critters. If this is not set, the game will use the default logic
which checks for sunny days in summer.

</td>
</tr>
<tr><th colspan=2>Screen Tinting</th></tr>
<td><code>AmbientColor</code></td>
<td>

*Optional.* The ambient color that should be used for lighting when this
weather type is active. In the base game, this is only used if the
`IsRaining` weather flag is applied, in which case it gets the
value: `255, 200, 80`.

> Note: You can use hex or color names here, and not just `R, G, B` values.

</td>
</tr>
<tr>
<td><code>LightingTint</code></td>
<td>

*Optional.* If set, this color will be drawn to the screen in lighting mode
during the Draw Lightmap phase of world rendering. In the base game, this
is only used if the `IsRaining` weather flag is applied, in which case it
uses the value `orangered`.

> Note: You can use hex or color names here, and not just `R, G, B` values.

</td>
</tr>
<tr>
<td><code>LightingTintOpacity</code></td>
<td>

*Optional.* An opacity to use with the `LightingTint` color. Setting this
here will pre-multiply the alpha as the game expects. You will likely want
to use this rather than messing with the alpha value of `LightingTint` to
have it behave how you expect.

In the base game, this is only used if the `IsRaining` weather flag is
applied, in which case it uses the value `0.45`.

You may wish to use `Layers` rather than these lighting properties for
more flexibility.

</td>
</tr>
<tr>
<td><code>PostLightingTint</code></td>
<td>

*Optional.* If set, this color is drawn to the screen in normal mode after
the lighting phase of world rendering. In the base game, this is only used
if the `IsRaining` weather flag is applied, in which case it uses the
value `blue`. If the `IsGreenRain` flag is applied, it instead uses the
value `0, 120, 150`

</td>
</tr>
<tr>
<td><Code>PostLightingTintOpacity</code></td>
<td>

*Optional.* An opacity to use with the `PostLightingTint` color. Setting
this will pre-multiply the alpha as the game expects. You will likely want
to use this rather than messing with the alpha value of `PostLightingTint`
to have it behave how you expect.

In the base game, this is only used if the `IsRaining` weather flag is
applied, in which case it will use the value `0.2`. If the `IsGreenRain`
flag is applied, it instead uses the value `0.22`

</td>
</tr>
<tr><th colspan=2>The Good Stuff</th></tr>
<tr>
<td><code>Effects</code></td>
<td>

*Optional.* A list of [Effects](#effects) that should apply to the player
while they are in a location with this weather type.

</td>
</tr>
<tr>
<td><code>Layers</code></td>
<td>

*Optional.* A list of [Layers](#layers) that should render when the
current location has this weather type.

</td>
</tr>
</table>


## Effects

An effect is something that applies to the player while they are in a location
with a given weather type. This is specifically for things that affect players,
and not for other arbitrary things that could happen.

For making other things happen, you can make suggestions, but you might want
to look into other mods and triggers.

Each `Effect` has the following values:

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr><th colspan=2>Identity</th></tr>
<tr>
<td><code>Id</code></td>
<td>

**Required.** The Id of this effect. This only needs to be unique within the
weather type containing it.

</td>
</tr>
<tr>
<td><code>Type</code></td>
<td>

**Required.** The type of effect. Valid options are:
* `Buff`
* `ModifyHealth`
* `ModifyStamina`
* `Trigger`

More types may be added in the future, or by C# mods (in the future).

</td>
</tr>
<tr>
<td><code>Rate</code></td>
<td>

*Optional.* How often should this effect update, in ticks. Each second is
`60` ticks.

Default: `60`

</td>
</tr>
<tr><th colspan=2>Conditional Effects</th></tr>
<tr>
<td><code>Condition</code></td>
<td>

*Optional.* A [game state query](https://stardewvalleywiki.com/Modding:Game_state_queries)
to determine whether or not this effect should be active. If this is not set,
the effect will always be active.

These conditions are only reevaluated upon location change, an event starting,
or the in-game hour changing.

</td>
</tr>
<tr>
<td><code>Group</code></td>
<td>

*Optional.* An optional group for this effect. Only one effect in a group can
be active at a time. Specifically: the first effect in the effect list that
passes its `Condition` will be active and all others in the group will be
skipped.

</td>
</tr>
</table>

All other effect values are specific to their individual `Type`s, as follows:


### `Buff`


### `ModifyHealth`

The `ModifyHealth` effect will either damage or heal the player. This can be
used to, for example, damage the player if they're caught outside in a
particularly nasty bit of weather like acid rain, or a snowstorm, or volcanic
heat, etc. Or maybe there's a sacred grove with special weather that heals
the player? Anything is possible.

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr>
<td><code>Amount</code></td>
<td>

**Required.** The amount to change the player's health by. Setting this to
a negative value will damage them, and setting this to a positive value will
heal them.

Note that when damaging players, the player will get temporary invulnerability
to further damage so you may as to increase the time between damage ticks to
avoid effectively making them immune to other dangers on the map because of
the weather damage.

There is no such rate limitation when healing the player.

</td>
</tr>
<tr>
<td><code>MinValue</code></td>
<td>

*Optional.* The minimum value that the player's health can reach.

> Please note that, due to the random nature of applying damage, the player's
> health may dip under this value. If you want to use it to stop the player
> from dying to your damage, you should set it to a higher value like `10`
> rather than `1` to prevent an unlucky damage roll from killing them.

Default: `0`

</td>
</tr>
<tr>
<td><code>MaxValue</code></td>
<td>

*Optional.* The maximum value that the player's health can reach when they
are being healed. Unlike the `MinValue`, this should be perfectly reliable.
Unfortunately, there is no way to set it based on the player's maximum
health at this time aside from that, if this value is greater than the
player's maximum health, it will be reduced to the player's maximum health.

Default: `2147483647`

</td>
<tr>
<td><code>Chance</code></td>
<td>

*Optional.* The chance that this effect applies on any given update. You can
use this to make the effect only occasionally damage/heal the player. This is
a number from `0.0` to `1.0`, where `1.0` is a 100% chance and `0.0` is a
0% chance.

Default: `1.0`

</td>
</tr>
</table>


### `ModifyStamina`

The `ModifyStamina` effect will either drain or fill the player's stamina. This
can be used to, for example, make the player lose stamina while they're caught
outside in a hostile bit of weather like extreme winds or a sand storm.

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr>
<td><code>Amount</code></td>
<td>

**Required.** The amount to change the player's stamina by. Setting this to
a negative value will drain it, and setting this to a positive value will
fill it.

</td>
</tr>
<tr>
<td><code>MinValue</code></td>
<td>

*Optional.* The minimum value that the player's stamina can reach.

Default: `0`

</td>
</tr>
<tr>
<td><code>MaxValue</code></td>
<td>

*Optional.* The maximum value that the player's stamina can reach.
Unfortunately, there is no way to set it based on the player's maximum
stamina at this time aside from that, if this value is greater than the
player's maximum stamina, it will be reduced to the player's maximum stamina.

Default: `2147483647`

</td>
<tr>
<td><code>Chance</code></td>
<td>

*Optional.* The chance that this effect applies on any given update. You can
use this to make the effect only occasionally drain/fill the player's stamina.
This is a number from `0.0` to `1.0`, where `1.0` is a 100% chance and `0.0`
is a 0% chance.

Default: `1.0`

</td>
</tr>
</table>


### `Trigger`


## Layers

A Layer is something that draws to the screen while in a location with a
given weather type. You can compose multiple layers to create complex effects.

Each `Layer` has the following values:

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr><th colspan=2>Identity</th></tr>
<tr>
<td><code>Id</code></td>
<td>

**Required.** The Id of this layer. This only needs to be unique within the
weather type containing it.

</td>
</tr>
<tr>
<td><code>Type</code></td>
<td>

**Required.** The type of layer. Valid options are:
* `Color`
* `Debris`
* `Rain`
* `Snow`
* `TextureScroll`

More types may be added in the future, or by C# mods (in the future).

</td>
</tr>
<tr>
<td><code>Mode</code></td>
<td>

*Optional.* The blending mode for this layer. There are two choices, and
they significantly change how the layer is drawn to the screen:

* `Normal`

	This blending mode just draws things on top of other things as you
	would expect. Normally, so to speak.

* `Lighting`

	This blending mode functions the same way the game handles lightmap
	drawing. Specifically, the color blending function is called
	'ReverseSubtract'. Rather than adding values together, you're
	subtracting, basically.

	You'll probably just want to experiment with this. It can be great
	for things like moving shadows from clouds in the sky.


> Note: You should try to group all your layers with the same
> modes together for the best performance.

Default: `Normal`

</td>
</tr>
<tr><th colspan=2>Conditional Layers</th></tr>
<tr>
<td><code>Condition</code></td>
<td>

*Optional.* A [game state query](https://stardewvalleywiki.com/Modding:Game_state_queries)
to determine whether or not this layer should be visible. If this is not set,
the layer will always be visible.

These conditions are only reevaluated upon location change, an event starting,
or the in-game hour changing.

</td>
</tr>
<tr>
<td><code>Group</code></td>
<td>

*Optional.* An optional group for this layer. Only one layer in a group can
be visible at a time. Specifically: the first layer in the `Layers` list that
passes its `Condition` will be visible and all others in the group will be
skipped.

</td>
</tr>
</table>

All other layer properties are specific to their individual `Type`s, as follows:


### `Color`


### `Debris`


### `Rain`


### `Snow`


### `TextureScroll`


## How Do I Make My Weather Happen?

Just adding a custom weather type using Cloudy Skies isn't enough to actually
make your weather happen in the game. You need to tell the game when and where
it should be applied, and that's done in one of two ways:

* Use a location context's `WeatherConditions` list to make the weather type
  occur naturally in a location context.

  See the [1.6 migration guide](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Custom_location_contexts)
  for more.

* Use a custom weather totem to override tomorrow's weather specifically.


### Custom Weather Totems

To make a custom weather totem, you'll need to add a custom object to the
game. In 1.6, the best way to do this is by editing the `Data/Objects`
list to add your custom item, and then editing a shop, crafting recipe,
event, etc. to give the player a way to obtain your item.

The important thing is that your custom weather totem has a `CustomField`
in its `Data/Objects` entry with the key `leclair.cloudyskies/WeatherTotem`
and a value with the desired weather type's Id.


## Commands

Cloudy Skies has the following console commands:


### `cs_reload`

Invalidate the cached effects and layers, causing them to be
reinitialized. Useful if working on C# stuff that changes how
effects or layers work.


### `cs_list`

List all the known weather types, including how many effects
and layers they have, which locations they are active in, and
which locations they will be active in tomorrow.


### `cs_tomorrow [weatherId]`

Change tomorrow's weather in your current location to use the
weather type with the provided Id. This acts as though you used
a custom weather totem.

> Note: This obeys the weather totem permissions, and will not
> override forced weather likes `Festival` and `Wedding`.


## Mod Data / Custom Fields

### `leclair.cloudyskies/WeatherTotem`

**Targets:**
* `modData` on individual item instances
* `CustomFields` in `Data/Objects`

This can be used either as `modData` or by setting a custom field
on an object. Either way, the value should be the Id of whatever
weather type you want the totem to change tomorrow's weather to.
When the player uses the totem, it will act like a Rain Totem,
but for whatever weather is specified.


### `leclair.cloudyskies/AllowTotem:[ID]`

**Target**:
* `CustomFields` in `Data/LocationContexts`

This can be used to control whether or not a custom weather totem
for the weather type with the Id `[ID]` can be used to override
tomorrow's weather in the relevant location context.

The value should be `true` or `false`


## Game State Queries

Cloudy Skies adds the following game state queries to the game:


### `CS_LOCATION_IGNORE_DEBRIS_WEATHER <location>`

Whether the [given location](https://stardewvalleywiki.com/Modding:Game_state_queries#Target_location)
is flagged to ignore debris weather. You can use this query to hide your
`Debris` layers in such locations.


### `CS_WEATHER_IS_RAINING <location>`

Whether the weather at the [given location](https://stardewvalleywiki.com/Modding:Game_state_queries#Target_location)
has the `IsRaining` weather flag.


### `CS_WEATHER_IS_SNOWING <location>`

Whether the weather at the [given location](https://stardewvalleywiki.com/Modding:Game_state_queries#Target_location)
has the `IsSnowing` weather flag.


### `CS_WEATHER_IS_LIGHTNING <location>`

Whether the weather at the [given location](https://stardewvalleywiki.com/Modding:Game_state_queries#Target_location)
has the `IsLightning` weather flag.


### `CS_WEATHER_IS_DEBRIS <location>`

Whether the weather at the [given location](https://stardewvalleywiki.com/Modding:Game_state_queries#Target_location)
has the `IsDebrisWeather` weather flag.


### `CS_WEATHER_IS_GREEN_RAIN <location>`

Whether the weather at the [given location](https://stardewvalleywiki.com/Modding:Game_state_queries#Target_location)
has the `IsGreenRain` weather flag.


## Content Patcher Tokens

### `leclair.cloudyskies/Weather`

This token represents the weather flags of the current location.
Possible values:

* `Raining`
* `Snowing`
* `Lightning`
* `Debris`
* `GreenRain`
* `Sunny` (present is none of the previous value are present)
* `Music` (present is `MusicOverride` is set)
* `NightTiles` (present if `UseNightTiles` is set)
