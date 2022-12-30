**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ribeena/StardewValleyMods**

----

← [author guide](../author-guide.md)

## Contents
* [Introduction](#introduction)
* [The Colors](#the-colors)

## Introduction
Dynamic Bodies uses a shader to fastly swap 25 colors to use selections. In order to make this
simpler, rather than loading those colors from the Farmer Sprite, these are standardised. You
can use the [palette image](../../assets/Character/palette_skin.png) to help you get the colors right.

## The Colors
<img src="../../assets/Character/palette_skin.png" width="250" style="image-rendering: pixelated;">
Here are all 25 colors and a brief explanation of what they are for.

| Color         | Description |
| :-----------: |-------------|
| Hex: `#4a0c06` | Dark shirt |
| Hex: `#701718` | Medium shirt |
| Hex: `#8e1f0c` | Light shirt |
| Hex: `#ff00ff` | Make transparent (good for noses) |
| Hex: `#6b003a` | Dark skin |
| Hex: `#e06b65` | Medium Skin |
| Hex: `#f9ae89` | Light skin |
| Hex: `#a63650` | Medium Dark skin (new!) - automatically made by blending Dark and Medium skin colors. |
| Hex: `#ec8c77` | Medium Light skin (new!) - automatically made by blending Light and Medium skin colors. |
| Hex: `#b5394e` | Alternate dark skin color |
| Hex: `#c8495b` | Alternate medium skin color |
| Hex: `#db5969` | Alternate light skin color |
| Hex: `#3d1123` | Shoes dark |
| Hex: `#5b1f24` | Shoes medium dark |
| Hex: `#77291a` | Shoes medium light |
| Hex: `#ad471b` | Shoes light |
| Hex: `transparent` | Pants color (used internally only) |
| Hex: `#4a4542` | Eyelash/eyebrow color - new color, make sure to draw over those eyelashes with this |
| Hex: `#fffdfc` | Sclera light (whites of the eyes) |
| Hex: `#bea8a8` | Sclera dark |
| Hex: `#682b0f` | Left eye light |
| Hex: `#2d1206` | Left eye dark |
| Hex: `#0f0a08` | Pupil (?) - currently unused |
| Hex: `#0f1768` | Right eye light |
| Hex: `#0a062d` | Right eye dark |