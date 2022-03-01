**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments**

----

Ginger-Island Specific Dialogue Keys
=========================================

[← back to readme](../../README.md) <br/>
[← back to main guide](./README.MD)

Characters who have island schedules and are not currently on `IslandSouth` can call upon vacation dialogue keys in the usual dialogue file for each character
`Characters/Dialogue/<NPCname>`. These dialogue keys are said at most once per day.

Vacation keys are inserted just after the location-specific dialogue keys and follow the general form `<season><base><flair><remainder>`. Like the location keys, the season-specific variants are checked first.

The `<base>` is `Resort_Approach` before 1200. After 1800, the base is `Resort_Left` when the character is outside (ie, when they're traveling away from Ginger Island). When they're inside, the base is `Resort_Left_<location>` - ie `Resort_Left_Saloon`.

For `<flair>`: **either** `_<ResortGroup>` **or** `_marriage`, with `<ResortGroup>` being checked first. `<ResortGroup>` is used whenever the specific group is picked to go to the resort. (For example, so `Resort_Approach_PierreFamily` will only be called if the group `PierreFamily` was picked). A list of resort groups included with this mod can be found [here](https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments/blob/master/Ginger%20Island%20Mainland%20Adjustments/assets/defaultGroupings.json). Mods can add more by patching `Mods/atravita_Ginger_Island_Adjustments_groups`. 

Married NPCs will also check for base keys with `_marriage` appended to it, but will use non-marriage specific dialogue keys if a married variant is not available.

Next, you can limit the times the key will be used with the `<remainder>`. Remainders are checked in the following order:

1. `<base>_<ShortDayOfWeek>`. IE - `Resort_Left_Fri`
2. `<base><hearts>`, where `<hearts>` is any **even** number - the code will use the highest value it can find where the friendship requirement is satisfied. IE - `Resort_Left6`
3. `<base>`. IE `Resort_Left`

#### Examples of keys

* `winterResort_Left_Saloon_Fri` - said at the Saloon, in winter on Fridays, after coming back from the Resort.
* `winterResort_Left_Saloon_marriage_Fri` - said by married characters, at the Saloon, in winter on Fridays, after coming back from the Resort. (If this key is omitted, the married character will say the `winterResort_Left_Saloon_Fri` dialogue.
* `winterResort_Left_Fri` - said *outside only*, after coming back from the Resort, in winter on Fridays.
* `winterResort_Left` - said *outside only*, after coming back from the resort in winter.
* `Resort_Left` - always said *outside only* after coming back from the resort.
* `Resort_Approach` - said before heading to the resort in the morning, always, indoors or outdoors.

(Why is this not consistent? It's because characters in general do not do things before heading to Ginger Island, while `GIRemainder` schedule keys allow for characters to do things *after* returning.)

### Additional Resort keys.

* ~~`Resort_Antisocial`: Said while standing on IslandSouthEast.~~ **Temporarily removed until I figure out why NPCs sometimes vanish after going to IslandSouthEast**
* `Resort_Pier`: Said while standing on the pier, facing right.
* `Resort_IslandNorth`: Said by NPCs on IslandNorth.
* `Resort_Adventure`: Said by NPCs headed off towards either the volcano or Professor Snail's tent.
* `Resort_Volcano`: Said by NPCs standing near the volcano.
* `Resort_Tent`: Said by NPCs standing Professor Snail's tent.
* `Resort_Bartend`: While bartending.
* `Resort_Fish`: While fishing (off the pier on IslandSouth). Requires a fishing animation.

The vanilla resort keys are as following, documented here for easy access

* `Resort_Bar`: said while standing next to the bar, drinking.
* `Resort_Towel`: said while standing or lying on a towel. (Note: musicians also use this while playing music)
* `Resort_Dance`: said while dancing.
* `Resort_Wander`: said while wandering around in the northern part of IslandSouth.
* `Resort_Umbrella`: said while standing under an umbrella.
* `Resort_Chair`: said while standing in front of various chairs scatterd around IslandSouth.
* `Resort_Shore`: said while standing at the shore.
* `Resort_Entering`: said before 1200 while at the Resort.
* `Resort_Leaving`: said after 1800 while alt the Resort.
* `Resort`: fallback, said while on the Island South map.


### Marriage keys

The keys `GILeave_<NPCname>` and `GIReturn_<NPCname>` will be used by married characters, similar to how `FunLeave_<NPCname>` and `FunReturn_<NPCname>` work. Examples can be found [here](https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments/blob/master/%5BCP%5D%20Ginger%20Island%20Mainland%20Adjustments/assets/default/marriage_dialogue.json).