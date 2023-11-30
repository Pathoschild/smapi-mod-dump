**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ichortower/NPCGeometry**

----

# NPC Geometry

A mod for Stardew Valley 1.6+.

Allows several hardcoded NPC metrics to be overridden by custom data fields in
the `Data/Characters` asset.

This is useful if you're like me and you made [a custom
NPC](https://github.com/ichortower/HatMouseLacey) who is an adult but is very
short; the base game uses heuristics based on age and gender to calculate
positions, and they don't work out.

## Important Considerations

This mod **will have some of its functionality crippled by SpaceCore**, a
widely-used dependency mod. This mod uses Harmony transpilers to patch the game
code; two important segments of code go into the method `NPC.draw`.

SpaceCore applies a Harmony prefix patch to `NPC.draw` which prevents all
subsequent prefixes and all transpilers from executing. I have no control over
this and cannot work around it without breaking SpaceCore in turn.

If you also use SpaceCore in 1.6, `BreatheRect` will not work at all and
`EmoteHeight` will not work in normal gameplay (event emotes may still work).

## How To Use

This mod doesn't have any visible effect on its own: it injects code into a few
existing game methods to replace position-calculating logic with custom values,
if found. Currently, this allows other mods to specify the following:

1. `BreatheRect`: The position and size of the rectangle that animates to show
    the NPC breathing.
2. `EmoteHeight`: The height offset to use when rendering emote bubbles.
3. `ShadowScale`: The scaling factor to apply to the NPC's shadow.

By default, these things are hardcoded or use heuristic calculations based on
the NPC's age and gender. With this mod installed, these routines will try to
read from the character's `CustomFields` dictionary, and use values there if
they are found.

### BreatheRect

To set a custom breathing rectangle, set the custom field
`ichortower.NPCGeometry/BreatheRect` to a slash-delimited string containing
four integer fields: X, Y, Width, and Height, in that order. For example:

    "CustomFields": {
      "ichortower.NPCGeometry/BreatheRect": "5/25/6/3"
    }

These values are pixels in the spritesheet frame (big 4x pixels, not screen
pixels) and are relative to the top-left corner of the frame, which is usually
16x32.  The X and Y values are indexes, so "0/0" is the top-left-most pixel.

### EmoteHeight

To change the rendering position of emote bubbles (both in events and in normal
gameplay), set the custom field `ichortower.NPCGeometry/EmoteHeight` to an
integer. This value will be used to position the emote bubbles vertically, and
will not use any of the age or gender heuristics.

    "CustomFields": {
      "ichortower.NPCGeometry/EmoteHeight": "23"
    }

Event and non-event emote rendering are handled separately by the game. By
default, they don't yield the same results (events use age and gender and also
a slightly different base offset). If you set this field, the emotes should
appear at the same height in both situations: it is intended to look like the
bottom pixel of the emote starts at the offset in the field (when counting from
the bottom of the NPC's sprite frame).

### ShadowScale

To change the scale of the shadow the NPC casts, set the custom field
`ichortower.NPCGeometry/ShadowScale` to a floating-point value.

    "CustomFields": {
      "ichortower.NPCGeometry/ShadowScale": "0.85"
    }

This is used as an additional scale multiplier when drawing the NPC's shadow,
so 1.0 is normal size, 0.5 is half, 2.0 is double, etc.

### Scale (future? not implemented)

In the future, it may be possible to change the scale at which an NPC is
rendered. I have left it out of this mod so far because simply setting the
Scale property on an NPC doesn't seem fully-baked (it often causes strange
rendering errors).

I may be able to provide a field for this later, but it will require more
patches to support.

