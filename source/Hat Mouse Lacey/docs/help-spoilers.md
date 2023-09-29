**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ichortower/HatMouseLacey**

----

# THIS FILE CONTAINS SPOILERS
Read at your own risk.

Broadly, spoilers get more damaging as you scroll.

...

...

...

...

## I can't find Lacey. What is her schedule?

Most days, she is in her cabin or on the big island in the river nearby. (*If Stardew Valley Expanded is installed, she stays on the west/northwest bank of the river instead of using the big island, to avoid Andy's house*)

On Tuesdays, she goes shopping at Pierre's.

On Fridays, she visits town, then goes to the saloon.

If the Community Center is rebuilt, then on Wednesdays she goes there to use
the crafts room, then visits the saloon before going home.

On rainy days, or in winter, she stays in her cabin all morning, then goes to
the saloon for the evening.

She visits the Night Market on Winter 15 *and* Winter 17.

## Something is wrong with the Forest map around Lacey's house.

This mod adds a SMAPI console command `lacey_map_repair`. Try running that in
your SMAPI terminal and see if that helps (this function runs itself
automatically if needed, so it shouldn't be necessary to do this).

If it still looks wrong to you, please open a ticket so I can take a look at
the problem. Please provide your mod list and a screenshot if you do this.

## How do I trigger Lacey's heart events?

* *2 hearts*\
Enter her house when she's there.
* *4 hearts*\
Enter the saloon when she's there.
* *6 hearts*\
Enter the forest on a sunny day when she's there.
* *8 hearts*\
Enter town when she's there.
* *(the secret event)*\
After receiving the letter, enter town between 12 noon and 1 pm.
* *10 hearts*\
After receiving the letter, enter her house after 8 pm when she's there
(not on a festival day).
* *14 hearts*\
Enter the bus stop *from town* on a sunny day, not in winter, between 3 pm
and 6 pm.

## What are Lacey's gift tastes?

* **Love**: Morel, Fiddlehead Fern, Radish, Artichoke, Coffee, Cheese
* **Like**: Wheat, Corn, Rice, Unmilled Rice, Hardwood, *Fruits*, *Vegetables*
* **Neutral**: *Gems*, *Seeds*
* **Dislike**: Pale Ale, Beer, Wine, Mead, Juice, Rabbit's Foot, *Milks*, *Animal Products*
* **Hate**: none

These are her specified tastes. Anything not listed uses the Universal taste
lists.

## I heard Lacey reacts to the hat you're wearing, but she's not doing it. What's wrong?

You have to see her 2-heart event first. She also only reacts to each hat once;
if you already showed her your hat, you don't get the reaction again.

There isn't currently a way to see (in game) which hats you've already shown
her. Maybe a future update will include this.

## I got Lacey to 8 hearts and tried to give her a bouquet, but she turned me down. What gives?

I bet you said some mean things to her in her heart events, and she's probably
mad at you about it.

Go get a good night's sleep. You'll feel better in the morning.

## Is that a reference to...?
Here's a list of intentional references to other media:

* *"But how can we know for sure unless we try for ourselves?"*\
This is Maduin's line from Final Fantasy 6.
* *"Winter may be beautiful, but don't freeze!"*\
Toad (of *Frog and Toad*) says a line like this ("... but bed is much better.")
in *Frog and Toad All Year*.
* *"I wish I were a little bit taller."*\
[I wish I was a baller.](https://www.youtube.com/watch?v=cmXZOI7cM0M)
I wish I had a girl who looked good, I would call her.
* *"Those are supposed to protect you from radiation, right?
... Do they work?"*\
*The Simpsons*, S07E02: ["The goggles do nothing!"](https://frinkiac.com/caption/S07E02/966381)
* *"Hmm. Looking for wabbits?"*\
Elmer Fudd.
* *"Ooh, imagine a ghost having that instead of a head. Spooky!"*\
This is about the Headless Horseman wielding a jack-o'-lantern.
* *"You look like a cultist. Don't start caw-cawing or anything weird like
that, okay?"*\
The Cultist enemy (and headpiece!) in [Slay the Spire](https://www.megacrit.com/)
says "ca-caw!"
* *"I have to say, you look a little ridiculous. Are you going to take off and
fly around?"*\
Calvin's [propeller beanie](https://assets.amuniversal.com/4753ecc0deca013171a6005056a9545d).
* *"Is that tuna I smell? Or maybe it's salmon..."*\
Hobbes.
* *"Scurvy! Man the cannons!"*\
*Dodgeball* (2004) "... Steve? Steve the pirate. Scurvy!"
* *"Looking at this hat makes me hungry for spinach."*\
Popeye.
* *"Doesn't it make you feel merry?"*\
Robin Hood (and his merry men).
* *Quest title "Hats for the Hat Mouse"*\
[Blood for the Blood God](https://knowyourmeme.com/memes/blood-for-the-blood-god)
(*Warhammer 40k*).

And here are some intentional gags at Stardew Valley's expense:

* *"Sometimes I feel like I'm running out of new things to say."*\
This is about being a video game character with limited available dialogue
options. You are bound to get repeats eventually.
* *"I'm pretty sure someone's spiked [the fruit punch] before."*\
It was Pam.
* *"Have you figured out how they decide on the flower queen? Nobody will tell
me the rules."*\
The actual rule is "it's Haley, if she's dancing", but also it's not really a
thing.
* *"This town has a strange definition of 'potluck'."*\
I, the author, am delivering this line.

## What happened to Lacey's family?

See below.

## Why aren't there any other mice?

There is a canon (i.e. my headcanon) answer for these two questions. I have
deliberately not specified it, so that your headcanon can take its place. That
will probably be more satisfying to you than reading mine.

## Our kids don't look right: my config was wrongly set. How do I fix it?

This mod includes a console command `mousify_child` which you can use to change
your kids. In the SMAPI console window, type `mousify_child <child's name>
<number>` and press Enter to run it. This will transform your child permanently
(but you can always run it again). This doesn't change the child's gender or
age, just its mouseness.

Specify the `<number>` for the child you want:

* `-1`: normal human child (uses the vanilla asset, so other mods may change that texture)
* `0`: gray mouse child
* `1`: brown mouse child

More colors may be added in the future.
