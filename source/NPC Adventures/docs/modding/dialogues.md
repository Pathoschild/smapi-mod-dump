**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/PurrplingMod**

----

# Dialogues

## Dialogue strings source

Dialogue strings is loaded from:

- `assets/Dialogues/<NPCName>.json` for general companion dialogues
- `assets/Dialogues/<NPCName>Spouse.json` for spouse variant of dialogues

If you acces dialogues from code, acces it via mod's `ContentLoader` without the `assets/` prefix and `.json` suffix.

```yaml
Dialogues/Abigail
Dialogues/AbigailSpouse
Dialogues/Maru
...
```

## Ask and recruit dialogues

```yaml
companionAccepted  # NPC accepted a companion request
companionRejected  # NPC rejected a companion request 
companionRejectedNight  # NPC rejected request and it's night right now
companionDismiss  # NPC companion was released from companion recruitment
companionDismissAuto  # NPC companion was automatic released (due to night after 22h)
companionRecruited  # NPC companion was recruited
```

NOTE: All listed ask&recruit dialogue keys has a variant with suffix `_Spouse` or `_Dating` for marriage/spouse or dating dialogues.

**Example:**

```yaml
"companionAccepted": "Do you want to go for an adventure with me?$u#$b# Well @, what are you waiting for? Let's go!$h",
"companionAccepted_Spouse": "Adventure? Oh, of course I will @!$l#$b#I hope we can delve into the mines today.$h",
"companionRejected": "Sounds interesting, but I cannot go. Sorry.$s",
"companionRejectedNight": "Sorry, it's too late to go exploring today.$s",
"companionRejectedNight_Spouse": "Sorry @ I'm feeling tired. I'll be going to bed soon.$s",
"companionDismiss": "Thanks for bringing me with you @, it was exciting.$h#$b#I hope to do it again sometime. See ya.$h",
"companionDismiss_Spouse": "I had a great time @.$h#$b#But it's time for me to head back to the farm now, stay safe. I love you <$h",
"companionDismissAuto": "Hey @, it's getting late and I'm tired.#$b# I'm going to go home now. If you ever need an adventure partner, Goodnight.$h",
"companionDismissAuto_Spouse": "It's time to go.#$b#You should go to bed soon too honey.$2",
"companionRecruited": "Can we go to the mines @?$9 I've always wanted to explore them.$6",
"companionRecruited_Spouse": "I can't wait to go on an adventure with you @!$h#Shall we go into the mines today?$6",
```

## Suggestion dialogues

```js
{
  "companionSuggest": "<companion suggest dialogue>#$q -1 -1#<question for farmer>#$r -1 0 Yes#<yes farmer answer text>#$r -1 0 No#<no farmer answer text>",
  "companionSuggest_Yes": "<companion's reaction to farmer accepted>",
  "companionSuggest_No": "<companion's reaction to farmer rejected>",
}
```

**Example**

```js
{
  "companionSuggest": "I'm getting the itch for an adventure.#$q -1 -1#Hey @, wanna tag team for a little bit?#$r -1 0 Yes#Sure, lets find some trouble!#$r -1 0 No#Hmmmm, probably not today. Don't count me out next time though.",
  "companionSuggest_Yes": "I knew you'd be down! Where should we go first?$h",
  "companionSuggest_No": "Boo. Guess I'll need to find some fun with someone else today.",
}
```

## Skill specific dialogues

NOTE: All listed skill specific dialogue keys has a variant with suffix `_Spouse` or `_Dating` for marriage/spouse or dating dialogues.

### Doctor dialogues

```yaml
heal # NPC say this dialogue line after they heals a farmer
nomedkits # NPC say this dialogue when farmer try to ask for heal, but NPC has no medkits (speak only once)
```

**Example**

```js
{
  "heal": "Let's get you patched up.$3",
  "nomedkits": "I'll have to stop by Harvey's Clinic, I haven't got any more bandages.$6",
}
```

### Forager dialogues

```yaml
giveForages # This dialogue is shown when forager gives collected forages to farmer
farmerRunAway # If farmer run away, forager yell this dialogue
```

**Example**

```js
{
  "farmerRunAway": "Hey, @! Wait for me!",
  "giveForages": "@, I found some good stuff! Here, take this.",
}
```

## Location various dialogues

The dialogue key for companion for specific game location has this format:

```yaml
companion_<location> # Standard location dialogue. Can be spoken only once per day (companion recruitment session). Can be variated
companionEnter_<location> # Standard location dialogue triggered only when companion enter location
companionOnce_<location> # This kind of dialogue will be spoken only once in game save. This dialogue key can't be variated (no variant can be used)
companionOnceEnter_<location> # Once dialogue triggered only when companion enter location
companionRepeat_<location> # This dialogues can be repeated every enter to location or every game time changed. Can be variated.
companionRepeatEnter_<location> # Repeat dialogue triggered only when companion enter location
```

The `location` must be a valid game location name starts with capital letter.

All thoose companion location dialogues are generated when farmer and companion entered a location or game time changed in that location. If you want specify dialogue which is generated only when player and companion enter location, define key variant with `Enter` base suffix. All dialogues can be variated except the `*Once*` base key variant. Every line can be pushed to companion dialogue stack when player and companion enters a location or game time changed (ten minutes update). Only the dialogues which base of key contaiins part `Enter` can be pushed only when player and companion enters a location, not in game time was changed.

**Example:**

```yaml
companion_Town
companion_Farm
companionRepeatEnter_Farm
companion_Mine
companionEnter_Mine
companion_Mountain
companionOnce_Mountain # this dialogue will be spoken only once in game
```

### Once spoken location dialogues

This kind of dialogues will be spoken only once per game save and will be never repeated. This dialogue has a priority over ordinary location dialogues and can't be variated (see variations below). If both `companion_<location>` and `companionOnce_<location>` are defined for the same location, then the **companionOnce** will be shown during first interaction with the NPC in mentioned location. No other dialogue will be added to the stack. After leaving that location and entering it again, dialogue companion will be loaded, dialogue **companion** will be loaded and can be spoken repeately when we enter again.

NOTE: Only once spoken dialogues **can't be variated**.

### Repeat dialogues

Standard location dialogue can be shown only once per day (companion recruit session). Dialogue lines which key contains a part called `Repeat` can be shown many times per day (in companion session). I recommend use Repeat in combination with `Enter` base key suffix. It's better use repeat dialogues only when companion enter location than everytime. Restrict to enter location only you avoid wtf dialogue repeats when game time was changed (ten minutes update).

### Dialogue variations

Location dialogues can be variated. We can specify a dialog for some location, season, day, week day, Day/Night, specific Weather, Friendship heart level and etc.

The variation keys has following formats and will be checked in this order:

```yaml
companion_<location>_<time>_<condition>[<friendship>]
companion_<location>_<condition>[<friendship>]
companion_<location>_<time>[<friendship>]
companion_<location>[<friendship>]
```

The `<time>` is a time based dialogue key (eg. spring, Monday, and some time variations), `<condition>` is a game state condition (Player is married, is night, Rainy weather and etc) and `<friendship>` is a friendship heart level.

**For example:**

```yaml
companion_Town_fall_Monday_Rainy8
companion_Town_fall_Rainy
companion_Town_Night6
companion_Town_Night
companion_Town4
companion_Town
```

#### Time based dialogues

Time based dialogues has following format checked in this order:

```yaml
*_<season>_<dayOfMonth>
*_<season>_<dayOfWeek>
*_<dayOfWeek>
*_<season>
```

**Example:**

```yaml
*_spring_4
*_fall_Tuesday
*_Saturday
*_Summer
```

#### Condition based dialogues

Dialogues can be conditioned by game state. Conditions are:

- Is player married or dating with this NPC?
- Is day or night?
- Whats is a weather? Rainy? Sunny or something else?
- and combinations of this conditions

This keys has a following format and checked in this order:

```yaml
*_Night_<weather>_Spouse
*_<weather>_Spouse
*_Spouse
*_Night_<weather>_Dating
*_<weather>_Dating
*_Dating
*_Night_<weather>
*_Night
*_<weather>
```

Key `<weather>` can use this values: `Rainy`, `Snowy`, `Stormy`, `Cloudy` and `Sunny`.

### The friendship

Dialogues can be conditioned by friendship heart level. Friendship dialogue is checked decreased in scale: `12`, `8`, `6`, `4` and `2` hearts.
If friendship heart level for this NPC is betwen two numbers in scale, then using nearest lower than scale anchor. For example: With Abigail I has a **7 hearts**. Gamme use existing dialogue for **6 hearts**. If this dialogue is not defined, then Game try use **4 hearts** dialogue. If this exists, then use it. If game not found any friendship level dialogue, then fallback to a dialogue without friendship.

**Example:**

```yaml
companion_Town8
companion_Town_Spring4
companion_Town_fall_Spouse8
companion_Town_fall_Spouse
```

## The format

All dialogues has a vanilla game dialogue format. For more info see [Official SDV wiki](https://stardewvalleywiki.com/Modding:Dialogue#Format)

## Speech bubbles

You can define, what NPC say in speech bubble above head. Speech bubles is defined in `assets/Strings/SpeechBuubles.json` (`Strings/SpeechBubbles` for accessing from code). Bubbles has this format:

```yaml
ambient_<location>_<NPC>
```

**Example:**

```yaml
ambient_Mine_Abigail
ambient_Town_Maru
```

NPC in bubble can mention a player name. In dialogue text use `{0}` for player name. Bubbles is not supporting SDV dialogue format, because **speech bubbles is not a dialogues.**

```yaml
Mine_Abigail: "Taking me to mines, {0}?"  # Abigail says: "Taking me to mines, Ellen?" if playername is Ellen
```

Speech bubbles **can't be variated**. Only location and npc name is suppported. We can randomize it. See bellow.

### Fight bubbles

**Format**
```yaml
fight_<NPC>
```

We can specify fight bubble speechs. This bubbles can be displayed above NPC's head randomly. For NPCs with *warrior* profession is higher chance to display it instead of other NPCs.

**Example**
```yaml
fight_Abigail
fight_Elliott
fight_Maru~0
fight_Maru~1
```

## Randomize dialogues

You can randomize dialogue. If you can suffix with `~<number|word|character>` dialogue key, then game select random dialogue text with specified key.

**Example:**

```yaml
ambient_Mine_Abigail: "Be blessed my sword!",
ambient_Mine_Abigail~1: "I love adventure in mines!",
ambient_Mine_Abigail~2: "Taking me to mines, {0}?",
ambient_Mine_Abigail~3: "I < it!",
```

If game requests a dialogue with key `Mine_Abigail`, then a random text is selected. In this case game selects a random dialogue text and view it in speech bubble above Abigail's head.

**You can randomize any dialogue or speech bubble**

### Random chance dialogues

You can define which chance has dialogue to be show. Add at end of key `^<number in %>` to define what chance has this line to be pushed into NPC's dialogue stack.

**Example**

```yaml
companion_Mine^30 # 30% chance to be added this dialogue to stack
```

**NOTE:** Chance dialogues (`^`) can't be combined with random dialogues (`~`). You can use only one of this effects or none of them.

## Use it in content packs

For create or edit dialogues via content packs, add to your content.json file in contentpack folder:

```json
{
  "Format": "1.3",
  "Changes": [
    {
      "Target": "Dialogue/Elliott",
      "FromFile": "assets/dialogue/elliott.json"
    },
    {
      "Target": "Dialogue/MyOwnNpc",
      "FromFile": "assets/dialogue/myownnpc.json"
    },
    {
      "Target": "Strings/SpeechBubbles",
      "FromFile": "assets/strings/mybubbles.json"
    }
  ]
}
```
