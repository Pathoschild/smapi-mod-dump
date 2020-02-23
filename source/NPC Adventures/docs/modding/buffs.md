# Buffs

## Define buffs

Asset `Data/Buffs`

### Stat buffs

```js
{
  "<npcName>": "<farming>/<fishing>/<mining>/<digging>/<luck>/<foraging>/<crafting>/<maxStamina>/<magneticRadius>/<speed>/<defense>/<attack>"
}
```

NOTE: All values is integer.

**Example**
```js
{
  "Abigail": "0/0/0/0/1/0/0/0/0/1/0/1",
  "Alex": "0/0/0/0/0/0/0/0/0/1/0/2",
  "Elliott": "0/3/0/0/0/0/0/0/0/0/0/0",
  "Emily": "0/0/2/0/0/0/0/0/0/0/0/0",
  // ...
}
```

### Prosthetic (changeable) buffs

This buffs can be changed in-game by press `G` key.

```js
{
  "<npcName>~<index>": "...", // value is same like stat buffs
}
```

## Define buff description

Asset `Strings/Buffs`

```js
{
  "<npcName>": "<text>"
}
```

TIP: You can use `#` for new line

**Example:**

```js
{
  "Abigail": "Hanging out with Abigail get's your adventurer's blood pumping!#You gain +1 Speed, +1 Luck and +1 Attack.",
  "Alex": "Alex's fighting spirit seems to awaken yours as well.#You gain +1 Speed and +2 Attack.",
  "Elliott": "Truly a descendant of Thoreau himself, there's nobody quite#like Elliott to sit down and fish with next to a tranquil pond.#You gain +3 to your Fishing skill.",
  "Emily": "Emily and her powerful spirit will aid you while mining.#She grants you +2 Mining.",
  "Haley": "When Haley's around, good things just seem to happen more often.#Your Luck is increased +2!",
  "Harvey": "Dr. Harvey is the master of both preventative and reactive medicine.#You gain +3 Defense.",
  "Leah": "Leah always seems to smell like chopped wood and fall mushrooms.#You gain +2 Foraging while hanging out with her.",
  "Maru": "Maru's been working on some prosthetics that can enhance various abilities!#You gain +1 to any stat. Use the '%button' key to cycle your current prosthetic.",
  "Penny": "Penny's a true homesteader, and she's more than willing to help out on the farm!#You gain +3 Farming with her at your side.",
  "Sam": "Sam likes to live fast (but hopefully he won't die TOO young)# You gain +2 Speed",
  "Sebastian": "Hmmmm. Something about Sebastian's buff seems familiar...#You gain +1 Speed, +1 Luck and +1 Attack",
  "Shane": "Shane may shun organic ingredients in favor of frozen pizza, but he's still good with chickens.#You gain +3 Farming with Shane."
}
```
