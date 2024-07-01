**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ichortower/SecretNoteFramework**

----

# Secret Note Framework - Author Guide

This document explains how to use this mod to add your custom Secret Notes to
Stardew Valley.

## Contents

* [Introduction](#introduction)
* [Adding Notes](#adding-notes)
  * [Content Patcher example](#content-patcher-example)
  * [Image Notes](#image-notes)
  * [Custom Items](#custom-items)
* [How Modded Notes Work](#how-modded-notes-work)
  * [Spawning](#spawning)
  * [Collecting](#collecting)
  * [Querying Notes](#querying-notes)
  * [Marking Notes as Seen](#marking-notes-as-seen)
  * [Debugging](#debugging)


## Introduction

Secret Note Framework works for other mods by providing a data asset for them
to edit. At this time, other mods are expected to use Content Patcher or
SMAPI's Content API to perform their edits; in the future, I may provide
content pack support and/or a C# API, but those are not supported yet.

In addition to providing a way to add secret notes without fear of conflicts
(or of running out of space on the collections page), this mod also lets you do
a few advanced things, like:

* specify complex eligibility conditions on a note-by-note basis with game
  state queries
* determine which location context(s) your notes appear in, so you can (e.g.)
  restrict your mod's notes to spawn only in your mod's area
* use different (custom) items to represent different sets of notes, as
  desired
* specify your own assets for note content and formatting, including image
  notes
* set any number of trigger actions to be run when a note is first read

Let's begin!

(**Please note**: I recommend using i18n for text content in your mods. The
examples in this guide omit its use, for clarity of purpose.)


## Adding Notes

The main goal of the framework is to add notes, so here's what that looks like.
The asset to target with your edits is:

```
Mods/ichortower.SecretNoteFramework/Notes
```

The asset is a `string->object` dictionary. The `string` keys are note IDs, and
should be [unique string
IDs](https://stardewvalleywiki.com/Modding:Common_data_field_types#Unique_string_ID),
like most 1.6-era data items. The model (object) has the following fields:

<table>

<tr>
<th>Field</th>
<th>Type</th>
<th>Purpose</th>
</tr>

<tr>
<td><code>Contents</code></td>
<td>string</td>
<td>
  
The text content of your note. Obviously, it is required for text notes.
Formatting is the same as vanilla mail and secret notes: `^` is used for line
breaks, and `@` will be replaced with the player's name. You can also use the
[special mail format
codes](https://stardewvalleywiki.com/Modding:Mail_data#Custom_mail_formatting)
`[letterbg]` and `[textcolor]`, but it is preferred to use the other fields for
that instead (the fields will supersede the in-band format codes).

*Default:* `""`

</td>
</tr>

<tr>
<td><code>Title</code></td>
<td>string</td>
<td>

The note's title, which will be displayed on the hover tooltip in the
collections menu. Vanilla secret notes generate a title from their integer id
(e.g.  "Secret Note #15"), but you can specify whatever you would like. If not
specified, the tooltip will display `???`.

*Default:* `null`

</td>
</tr>

<tr>
<td><code>Conditions</code></td>
<td>string</td>
<td>

A [game state query](https://stardewvalleywiki.com/Modding:Game_state_queries)
specifying the conditions for this note to be available to spawn. If null or
empty, the note will be available without restriction, although the player will
still need the Magnifying Glass in order to find secret notes in the first
place.

Conditions are evaluated at the start of each game day, so after fulfilling the
conditions for a note to be available, the player will need to sleep a day in
order for the note to be able to appear.

This mod adds [a query](#querying-notes) for checking whether a modded secret
note has been seen by a player, which is useful for ordering your notes.

*Default:* `null`

</td>
</tr>

<tr>
<td><code>LocationContext</code></td>
<td>string</td>
<td>

This string specifies one or more location contexts where the note is able to
appear. Vanilla journal scraps spawn only in the `Island` context (i.e. on
Ginger Island); secret notes spawn anywhere else. You can specify any value(s)
here, including modded contexts, so for example if a mod adds a mountain area
with a separate context, you can define notes which spawn only there.

Specify any number of context names, separated by commas (e.g. `Default,
Desert`), to allow a note to spawn in any of them. Alternately, you can specify
one context name preceded by a `!` to specify any location *except* that one:
the default value is `!Island`, mimicking vanilla's secret notes.

*Default:* `!Island`

</td>
</tr>

<tr>
<td><code>ObjectId</code></td>
<td>string</td>
<td>

A qualified or unqualified object ID from `Data/Objects`. This is analogous to
the vanilla items "Secret Note" `(O)79` and "Journal Scrap" `(O)842`: this item
will be created as debris, and when the player uses it, one of the notes
matching it will be read (see [Spawning](#spawning), under how notes work, for
more details). This item's sprite will also be displayed in the collections
page to represent this note, either grayed out (unread) or normal (read).

If this field is `null` (the default), the mod will use its default secret note
object. For best results, you should leave it as default or specify a [custom
item](#custom-items).

*Default:* `null`

</td>
</tr>

<tr>
<td><code>NoteTexture</code></td>
<td>string</td>
<td>

A game asset path indicating the note background texture to use when displaying
the note. This is equivalent to specifying an asset via the [special mail format
code](https://stardewvalleywiki.com/Modding:Mail_data#Custom_mail_formatting)
`[letterbg]`, except that this field will take precedence if both are provided.
Since secret notes use the same game code as mail, all of the same formatting
caveats apply to the asset you specify here.

*Default:* `null`

</td>
</tr>

<tr>
<td><code>NoteTextureIndex</code></td>
<td>integer</td>
<td>

An integer specifying the index in the `NoteTexture` to use as a background.
This is equivalent to (but takes precedence over) specifying an index via
`[letterbg]`, as with NoteTexture.

*Default:* `0`

</td>
</tr>

<tr>
<td><code>NoteTextColor</code></td>
<td>string</td>
<td>

A string specifying what color to use to render the note's text, equivalent to
using the format code `[textcolor]` (but overriding it, as usual). This can be
any of the 10 acceptable vanilla color names:

`black`, `blue`, `red`, `purple`, `white`, `orange`, `green`, `cyan`, `gray`, `jojablue`

... or, you can specify any RGB color you like by using the form `rgb(<r>, <g>,
<b>)`, where r, g, and b are integers from 0 to 255. For example:

```
"NoteTextColor": "rgb(88, 34, 44)",
```

*Default:* `null`

</td>
</tr>

<tr>
<td><code>NoteImageTexture</code></td>
<td>string</td>
<td>

A game asset path indicating the texture to use when loading an image for an
[image note](#image-notes) (see that section for more details).
Note images are 64x64 pixels and are read in order, left-to-right and
top-to-bottom, just like other spritesheets; but be aware that there is a
hardcoded offset for the image of the piece of tape holding the image inside
the note (193/65, 14x21), so you should not use the index containing that.

If `null`, the default secret notes image texture
(`TileSheets/SecretNotesImages.png`) will be used.

*Default:* `null`

</td>
</tr>

<tr>
<td><code>NoteImageTextureIndex</code></td>
<td>integer</td>
<td>

An integer specifying the index in the `NoteImageTexture` to use for the note
image. Unlike `NoteTextureIndex`, the default value here is `-1`, since the
LetterViewerMenu itself uses this value to control rendering; as a result,
**this value controls whether your note is an image note (>= 0) or a text note
(-1)**.

*Default:* `-1`

</td>
</tr>

<tr>
<td><code>ActionsOnFirstRead</code></td>
<td>array(string)</td>
<td>

This array of strings specifies what [trigger
actions](https://stardewvalleywiki.com/Modding:Trigger_actions) should be run
when the player reads this note by using the item from inventory (i.e. when the
note is added to the collection, versus when re-reading the note from the
collections menu).

The actions are run when the letter menu is closed.

**Note**: if you use this mod's trigger action to [mark this note as
seen](#marking-notes-as-seen) (adding it to a player's collection), these
actions **will not** run. Likewise, if you mark a seen note as unseen and it is
collected again, the actions will run again when the player uses the item and
views the note.

*Default:* `[]`

</td>
</tr>

</table>

Most of these fields are optional: you can create a fully-working text note by
specifying only `Contents` (although including a `Title` is nice to do), or an
image note with only `NoteImageTextureIndex` (although you ought to also
specify `NoteImageTexture` and use your own asset).


### Content Patcher example

A Content Patcher patch to add a secret note to a mod might look like this:

```js
{
  "Target": "Mods/ichortower.SecretNoteFramework/Notes",
  "Action": "EditData",
  "Entries": {
    "{{ModId}}_SecretNote01": {
      "Contents": "I sure hope nobody finds this! It's full of embarrassing secrets.",
      "Title": "TOP SECRET DIARY",
      "Conditions": "PLAYER_HEARTS Current {{YourNpc}} 4",
      "ActionsOnFirstRead": [
        "AddMail Current {{ModId}}_Mail_HowDareYouFindMyDiary tomorrow"
      ]
    }
  }
}
```

This patch creates a note which is available only after reaching 4 hearts with
`{{YourNpc}}`. When it is found and read, it sends a mail to the current
player for the next day, presumably to scold them for reading the diary.


### Image Notes

You can create image secret notes (like the picture of Marnie, or the secret
dig locations) by specifying any value `0` or greater for
`NoteImageTextureIndex` in your note's data.  When this value is >= 0, the
`NoteImageTexture` (or the vanilla texture, if unspecified) will be loaded, and
this offset in the texture will be displayed (this applies to both the hover
tooltip and the inside of the letter). This image behaves exactly like the
vanilla secret notes texture (`Data/SecretNotesImages`), as follows.

Each note image in the texture is 64x64 pixels, the same size as a character
portrait. They are read left-to-right and top-to-bottom, like this:

```
0   1   2   3
4   5   6   7
8   9  10  11
etc.
```

The recommended minimum size for this image is 256x128 (four columns and two
rows), because the LetterViewerMenu loads the texture for the piece of tape
from this asset at the hardcoded offset (193, 65) and size (14, 21). In the
layout above, this corresponds to **index 7**: if your image is wider, the
affected index will change accordingly.

This means that you should not use that index for your notes, and should
instead draw your tape image there (consult the vanilla asset for reference).
Of course, if you don't want the tape piece to appear on your notes, you can
leave that area blank or make your texture smaller.

Here's how you might set up an image note via Content Patcher:

```js
{
  "Target": "Mods/ichortower.SecretNoteFramework/Notes",
  "Action": "EditData",
  "Entries": {
    "{{ModId}}_SecretNote_TreasureMap": {
      "Title": "Blackgull's Map",
      "NoteImageTexture": "Mods/{{ModId}}/SecretNotesImages",
      "NoteImageTextureIndex": 3
    }
  }
},

{
  "Target": "Mods/{{ModId}}/SecretNotesImages",
  "Action": "Load",
  "FromFile": "assets/{{TargetWithoutPath}}.png"
}
```

### Custom Items

Using custom items for your secret notes lets you add a little extra *je ne
sais quoi* to your mod, and it helps your notes stand out from the vanilla
notes as well as those added by other mods. You can use as many different
`ObjectId`s as you want, creating groups of notes with related meaning.

Adding an item to accompany your note is pretty simple. Here's a Content
Patcher example adding a note which can be found after earning the Sous Chef
achievement. When found, it gives the player a new cooking recipe:

```js
{
  "Target": "Mods/ichortower.SecretNoteFramework/Notes",
  "Action": "EditData",
  "Entries": {
    "{{ModId}}_Note_CookingSecrets": {
      "Contents": "YOUR TEXT HERE: explain the top-secret cooking knowledge",
      "Title": "Cooking Secrets",
      "ObjectId": "(O){{ModId}}_Object_CookingSecrets",
      "Conditions": "PLAYER_HAS_ACHIEVEMENT Current 16", // Sous Chef
      "ActionsOnFirstRead": [
        "MarkCookingRecipeKnown Current {{ModId}}_SecretFamilyRecipe"
      ]
    }
  }
},

{
  "Target": "Data/Objects",
  "Action": "EditData",
  "Entries": {
    "{{ModId}}_Object_CookingSecrets": {
      "Name": "TornPageCookingSecrets",
      "DisplayName": "[LocalizedText Strings\\Objects:{{ModId}}_Object_CookingSecrets_Name]",
      "Description": "[LocalizedText Strings\\Objects:{{ModId}}_Object_CookingSecrets_Description]",
      "Type": "asdf",
      "Category": 0,
      "Price": 1,
      "Texture": "Mods/{{ModId}}/TornPageCookingSecrets",
      "SpriteIndex": 0,
      "Edibility": -300
    }
  }
},

{
  "Target": "Strings/Objects",
  "Action": "EditData",
  "Entries": {
    "{{ModId}}_Object_CookingSecrets_Name": "Torn Cookbook Page",
    "{{ModId}}_Object_CookingSecrets_Description": "It's a page torn from an old cookbook. It's in bad shape, but still legible."
  }
},

{
  "Target": "Mods/{{ModId}}/TornPageCookingSecrets",
  "Action": "Load",
  "FromFile": "assets/{{TargetWithoutPath}}.png"
}
```

I don't think your objects are *required* to be of `"Type": "asdf"` and
`"Category": 0`, but that's how the vanilla secret note items are and I
recommend copying them.

When adding notes, it is recommended to either omit `ObjectId` (and let the
framework use its own default object) or to use the ID of an object you are
adding to the game. When an object is connected to notes via `ObjectId`, this
framework automatically checks for the object id in its postfix patch to the
method `Object.performUseAction`, triggering the addition of the note to your
collection if it finds a match. This is why you should not use existing items:
they may already have code attached to them, and the note check may never be
run as a result.


## How Modded Notes Work

Broadly, the notes added to the `Mods/ichortower.SecretNoteFramework/Notes`
asset behave in the same way that [vanilla secret notes
do](https://stardewvalleywiki.com/Secret_Notes); this section explains what
that means in detail.


### Spawning

This mod adds a subsequent check for modded notes which is performed only after
the base game has already attempted to spawn a note. If a vanilla note was
spawned, there is a 50% chance that this mod's check will attempt to replace
it, or else do nothing. If no vanilla note was spawned, the check proceeds
normally but is less likely to succeed (the goal here is to avoid increasing
the frequency of generated notes too much).

The check has the same chance as the vanilla notes, but taking into account
only notes which are available to spawn (based on their `Conditions` and
`LocationContext` fields): a linear scale, from 80% if none have been found to
12% if only one remains unseen. If not rolling to replace a vanilla note, the
starting chance is cut in half, so the range becomes 40% to 12%.

When a note is spawned, its `ObjectId` field is checked to generate the
inventory item. Like with vanilla secret notes, the note has not truly been
selected yet: that occurs only when the item is used, to read the note. On use,
the note item will randomly choose from unread notes that use its ID (i.e. the
set of notes that have this item for their `ObjectId`).

If no note is available to read when the item is used, it will disappear from
inventory and display a message (in English, it's "The note crumbled to
dust..."). This shouldn't happen unless the player cheated the note items into
their inventory; or if they found a note and didn't use it for a while, and in
the meantime all notes of its type became unavailable; or something of that
nature.


### Collecting

Notes seen by each player are saved in the Farmer's modData, under the
following key:

```
ichortower.SecretNoteFramework/NotesSeen
```

When opening the collections menu, this mod adds any notes the player has seen
(drawn normally) and any notes that are eligible to spawn (grayed out), just
like vanilla secret notes. **Notes which are not eligible to spawn and have not
been seen will not appear**.

There is no specific limit to the number of notes that can be added. The code
which adds the modded notes to the Collections menu accounts for pagination,
so if you have a lot of modded notes, more pages will be added as needed, just
like the Mail tab.


### Querying Notes

#### Via Game State Query

If you need to know whether a given note has been read, you can use the
following [game state
query](https://stardewvalleywiki.com/Modding:Game_state_queries) added by this
mod:

```
ichortower.SecretNoteFramework_PLAYER_HAS_MOD_NOTE <player> <note_id>
```

Like most game state queries, the `<player>` argument can be [any specified
player](https://stardewvalleywiki.com/Modding:Game_state_queries#Target_player):
`Any`, `All`, `Current`, `Host`, `Target`, or a unique multiplayer ID.

This query is specific to notes added via this framework, since they are stored
separately from the vanilla notes (in the farmer's modData, instead of in the
dedicated secret notes field).

The expected use is to create note sequences, by making each later note require
the previous one, or to gate a set of notes behind having first acquired a
"key" note, or similar; but you can use it in any situation, like shop
conditions, or whether a character attends your wedding, or anything else that
strikes your fancy. Just remember that note conditions are only evaluated at
the start of each day, so note chains will require multiple days to complete.

For example, two notes might look like this:

```js
{
  "Target": "Mods/ichortower.SecretNoteFramework/Notes",
  "Action": "EditData",
  "Entries": {
    "{{ModId}}_SecretNote_Part1": {
      "Contents": "Part 1 of my debut mystery novel!",
      "Title": "Mystery Part 1"
    },
    "{{ModId}}_SecretNote_Part2": {
      "Contents": "And now, the thrilling conclusion!",
      "Title": "Mystery Part 2",
      "Conditions": "ichortower.SecretNoteFramework_PLAYER_HAS_MOD_NOTE Current {{ModId}}_SecretNote_Part1"
    },
  }
}
```

With this setup, the first note is available as soon as the player has access
to Secret Notes, but the second one is not; starting from the next day after
finding the first one, the second becomes available.

#### Via Content Patcher

This mod also adds a Content Patcher token:

```
{{ichortower.SecretNoteFramework/HasModNote}}
```

This token works just like `{{HasFlag}}`: it returns a comma-separated list of
all modded notes seen by the current player (at this time, no other specified
players are supported. I may add this in the future, if it's useful and
feasible). You can use it in the same way:

```js
"When": {
  "ichortower.SecretNoteFramework/HasModNote": "MyNoteId"
}

"When": {
  "ichortower.SecretNoteFramework/HasModNote |contains=MyNoteId": true
}
```

Be mindful of your patch's [update
rate](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md#update-rate),
as usual, when relying on this token.


### Marking Notes as Seen

This mod adds a [trigger
action](https://stardewvalleywiki.com/Modding:Trigger_actions) which you can
use to mark notes as seen (or unseen) directly, without the player having to
find the note or use the item.

```
ichortower.SecretNoteFramework_MarkModNoteSeen <player> <note id> [true/false]
```

`<player>` should be one of `Current`, `Host`, `All`, or a unique multiplayer
ID. The third argument is optional and may be set to `false` in order to mark a
note as *unread* instead of as read (removing it from the player's collection,
instead of adding it).

Like the GSQ and the CP token, this trigger works exclusively on modded notes
and will not affect vanilla ones.

**Important Note**: when you use this trigger action to mark a note as read,
its actions listed under `ActionsOnFirstRead` **will not** be executed. If you
need to execute them, you should rely on the player to find and read the note,
or you should duplicate them in the context you are using to run this action.

Likewise, marking a note as unread will allow it to be collected again, which
will cause its `ActionsOnFirstRead` to execute an additional time.


### Debugging

This mod includes a SMAPI console command which is intended to help authors
iterate quickly when creating notes, much like Content Patcher's `patch reload`
and `patch update`. It is:

```
snf_reload <target>
```

Where `<target>` should be one of `data`, `check`, or `full` (or `help`, or
omit it, in order to see the usage notes directly in the console).

* **data**: cache-invalidate and reload the notes data asset.
* **check**: reevaluate the `Conditions` fields on all notes, rebuilding the
  list of notes eligible to spawn.
* **full**: reload the notes data asset, then reevaluate note conditions (like
  running "data" followed by "check").
