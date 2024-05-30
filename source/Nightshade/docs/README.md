**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ichortower/Nightshade**

----

# Nightshade: in-game color reshader

![The player is standing in Pelican Town with the Nightshade config menu open,
showing the slider color settings in use to recolor the
game](images/example_summer.png)

This mod adds a highly configurable pixel shader to the game, allowing you to
adjust saturation, lightness, contrast, and color balance live during gameplay.
It is intended to serve as an alternative to [ReShade](https://reshade.me/),
for those players who can't (or would prefer not to) use that program. I doubt
it has as many features as ReShade, but since it runs within the game, it can
do a few extra things, like apply independently to the world and UI layers, and
automatically apply different profiles depending on the in-game season.

It also includes a depth-of-field ("tilt-shift") shader, for fun. That one is
not as configurable, though.

This mod is named after [the family of flowering
plants](https://en.wikipedia.org/wiki/Solanaceae).

## Special Thanks

A very big thank you goes out to my lovely beta testers, who agreed to try out
this mod before it was ready for general release, and who helped me find bugs
and make improvements:

- burntcheese.
- .cozy.with.zoe
- ellipzist
- .huntersouls
- hylianprincess
- jessicanekos
- .logophile
- pokkky
- riotgirl5989
- shalassa

## Installation

Nightshade is just a SMAPI mod, so you install it like any other: unzip the mod
into your Mods folder and start playing.

Of course, you will need to have SMAPI 4.0+ and Stardew Valley 1.6+.

## Configuration

Nightshade hooks into [Generic Mod Config
Menu](https://www.nexusmods.com/stardewvalley/mods/5098) if you have it
installed. However, the submenu only allows you to configure the keybinding
which will open Nightshade's custom config menu (implementing my own menu was
required in order to 1. avoid blocking too much of the screen, and 2. allow
values to affect the game's render in real time).

The default keybinding is H. Press it during gameplay to open the menu:

![A shot of the Nightshade menu, showing all of its sliders and buttons in
detail](images/example_menu.png)

There are a lot of controls. Here is an explanation of how they work.

* **Colorize World/Colorize UI** \
    You can independently toggle whether the color settings apply to the world
    layer and the UI layer. They will use the same profile; in a future update,
    I plan to allow them to use different ones.
* **Colorize By Season** \
    Nightshade saves four color profiles in its config.json. If Colorize By
    Season is enabled, the profiles will be mapped, in order, to Spring,
    Summer, Fall, and Winter (the tab titles will change to reflect this). In
    the appropriate season, that profile will be applied automatically.

    Note that at the title screen, the game's current season is Spring, so
    that profile will apply if this setting is toggled on.
* **Profile Switcher** \
    Switch between color profiles. Whichever one you select will be rendered
    live while the menu is open, even if Colorize By Season is on and it
    doesn't match the current season. If Colorize By Season is disabled, the
    selected profile will be saved (see Save) as the active profile and will
    apply at all times.
* **Saturation/Lightness/Contrast** \
    Adjust the overall saturation of colors, overall darkness/lightness of the
    game, and overall color contrast. These sliders apply equally to all
    colors.
* **Color Balance (CMY/RGB)** \
    These nine sliders allow you to adjust the balance between the primary
    RGB colors and their complements (CMY). Each group represents one pair
    (cyan-red, magenta-green, and yellow-blue), and in each group are three
    sliders, controlling (from top to bottom) shadows, midtones, and
    highlights, respectively, giving you fine-grained control.
* **Revert** \
    When you open the config menu, the current state of the four color profiles
    is saved in memory. At any time, clicking this button will return the
    current profile to that saved state.
* **Clear** \
    Resets all color sliders in the current profile to zero.
* **Copy** \
    Copies the current state of the color sliders to an in-memory buffer.
* **Paste** \
    Applies the contents of the in-memory buffer to the current color profile.
* **Depth of Field** \
    Enables or disables the depth of field shader.
* **Field** \
    Controls how much of the screen is fully in focus (not blurred). This is a
    minimum amount, since the player's position affects the field too (see the
    Depth of Field section for details).
* **Intensity** \
    Controls how strong the blur effect is, at maximum. After leaving the
    field, the blur gradually intensifies until it reaches this maximum.
* **Save** \
    This button saves the current settings to your config.json, including the
    slider positions on all four color profiles. Whichever profile tab is
    currently selected will be set as the active profile. Leaving the menu
    without having used this button will revert everything to its prior state.

    Even if you save, your screen may still change when you leave the menu;
    if Colorize By Season is on, the active profile index is not used in favor
    of the current active season. If you are viewing a profile from another
    season when you exit, the mod will immediately switch to the one matching
    the season.

If you would like, you **can** edit Nightshade's config.json by hand. However,
please note that the values in the config are mostly floating-point, even
though the UI presents most of them as integers.


## Depth of Field

The depth of field shader is not too sophisticated, but here's how it works.

The blur effect is a two-pass gaussian blur, which is identical to a one-pass
version but more performant. Since it is running in-game, it is able to use
the player's position to determine where on screen should be blurred. This
means that if you move to the top or bottom of the screen, that portion will
come into focus as you get closer (but the other end of the screen will not
become more blurred).

The blur intensity increases gradually once the Field threshold is crossed.
The growth is quadratic, so the very low blur amounts right at the threshold
may not be noticeable and may cause the field to appear wider than expected.
After reaching the maximum allowed Intensity, the blur will stop getting
stronger and will persist until reaching the edge of the screen.

The blur applies only to the world layer, and not to the UI. Unfortunately, a
few pieces of UI are rendered by the game on the world layer, so the shader
blurs them (see Known Issues). I hope to address that in a future update.


## Compatibility

This mod should be compatible with almost everything, since all it does is
alter the rendering logic to add the shaders, which is an uncommon technique.
This means it should work with all existing recolors and retextures, all mods
that render conventionally (Fashion Sense, Alternative Textures, etc.), and all
mod-added assets. However, any mods that similarly mess with the game's render
cycle or render in nonstandard ways are probably not compatible.

I don't have a list of such mods, so please let me know if you find an
incompatible one.


## Known Issues

During skippable cutscenes, the Skip button in the lower corner of the screen
is rendered on the world layer, which means the depth of field shader will
blur it if enabled. I hope to patch this in a future update.

The mine/skull cavern level indicator in the upper corner of the screen also
suffers this problem. I hope to patch this one as well.


## Roadmap

Features planned for (near) future updates:

* Allow different color profiles to apply to the world and UI layers.
* Allow saving and loading of color presets from a subdirectory of json files.
* Bundle some presets with the mod.
* Implement a locking feature on the color balance groups to make them easier
    to adjust in concert.
* Add a preview toggle button for the colorizer, to allow you to quickly a/b
    test a set of changes.
* Find a good way to open Nightshade's menu from within GMCM, instead of the
    little text blurb telling you about it.

A feature I would like to add, in an ideal world (don't count on it):

* Adding enough of a depth map to the game that the depth of field effect can
    look more convincing.


## Performance

This mod adds two pixel shaders, and to employ them it adds up to seven
full-screen draw calls per frame, four of which run the shaders (the remaining
three are just blits, to get pixels to another texture). These draws and
shaders have a cost, so if your computer is using an integrated or weak GPU
like mine, you may drop frames with everything enabled.

In this case, your best bet is to disable shaders you can live without: turning
off either colorizer layer saves one shader draw and one blit, and turning off
depth of field saves two shader draws. The depth of field shader is more costly
to run than the colorizer, so I recommend disabling that first if your
performance suffers.
