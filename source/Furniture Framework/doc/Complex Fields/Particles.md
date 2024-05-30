**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# How to define Custom Particles

Custom Particles have a lot of fields, but don't be scared: most of them are not required.  
If you need an example, the `Custom Cauldron` Furniture in the Example Pack has all its fields defined (even the optional fields for completeness).

## Source Image

Like the Source Image of a Furniture, a Particle can have its own .png (or be stored in the same spritesheet as the Furniture). This field is required.

## Source Rect

Like the [Source Rect](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#source-rect) of a Furniture, this tells the game which part of your image should be used for a single Particle. If making an animated Particle (see lower), this should be the Rectangle around the first frame.  
If omitted, the Source Rect will be the size of the given image.

## Emission Interval

This field defines how much time passes between each Particle is emitted, it is mesured in whole milliseconds (no decimals). This field is required.  
Setting this interval too low (less than 100) might cause issues because of the number of Particles that would be created.

## Spawn Rect

This field is another Rectangle, but this one defines the area of the sprite where the Particles will spawn. This field is required.  
Be carefull, the Spawn Rect of a Particle is relative to the sprite, not to the whole spritesheet.  
This field might be changed to be directionnal in the future.

## Depths

This is a list of decimal numbers from which the game will randomly pick to draw each Particle. A depth of 0 is at the front of the Furniture and a depth of Collisions.Height is at the back of the Furniture.  
If you did not define any [Layers](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#layers) for this Furniture, you probably don't have to worry about it.  

If omitted, every Particle will spawn with a depth of 0.

## Speed

This is the initial speed of spawned Particles. This field is a Vector field, which means it has 2 properties:
- "X" the horizontal speed
- "Y" the vertical speed

When properly defined it looks like this:
```json
"Speed": {"X": 0, "Y": -0.5}
```

It defaults to (0, 0) (the Particle do not move).

Note: the vertical axis Y is directed downwars, which means that a Particule with a negative Y speed will go up.

## Rotations

This is a list of decimal numbers from which the game will randomly pick to draw each Particle. This field is in **Radians**, not in degrees.

If omitted, every Particle will spawn upright.

## Rotation Speeds

This is a list of decimal numbers from which the game will randomly pick to draw each Particle. The unit is unknown, but 0.05 is already quite fast for reference.

If omitted, every Particle will spawn without rotating.

## Scaling

The `Scale` is a decimal number defining the scale of Particles. Defaults to 1.

The `Scale Change` is a decimal number defining the rate at which the Particles will change scale. If it's positive, the Particles will grow and vice-versa. Defaults to 0.

## Color

The `Color` is the _Name_ of a color that you want to apply to your particles. See [here](https://learn.microsoft.com/en-us/dotnet/api/system.drawing.color?view=net-8.0#properties) for a list of accepted color names (R, G, B and A are not accepted). Defaults to White (will not change the color of the Particle).

## Transparency

`Alpha` is a decimal number defining the transparency of Particles, 0 means fully transparent and 1 means fully opaque. Defaults to 1.

`Alpha Fade` is a decimal number defining the rate at which the transparency of Particles decreases. Defaults to 0 (the transparency doesn't change).

## Animation Properties

You can make animated Particles by using these fields:
- The "Frame Count" is the number of frames in your animation.
- The "Frame Duration" is how long each frame will last, measured in milliseconds.
- The "Loop Count" is how many times your animation will loop (untested).
- "Hold Last Frame" (true or false) will stay on the last frame of your animation one it has finished playing and looping.
- "Flicker" (true or false) will make your animation flicker (untested).

## Mode Options

There are a few more options defining when a Particle will spawn:
- `Emit When On` (true or false)
- `Emit When Off` (true or false)

If the Furniture is not [Toggleable](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#toggle), it will be "Off" forever.

## Burst

To reproduce the effect happening when a Cauldron is turned on, you can set `Burst` to `true`, it will create a semi-hardcoded burst of particles upon toggleing or placing depending on the Mode options.  
This feature is intended for Particles with non-zeros `Speed` and `Alpha Fade`.