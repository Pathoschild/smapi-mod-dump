**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/andyruwruw/stardew-valley-water-bot**

----

# v1.2.1

🐛 Bug Fix

- Fixed bug when trying to water after all crops are already watered.

# v1.2.0

🚀 New Feature

- Translations for different languages added.

🐛 Bug Fix

- Fixed bug when calculating best path through groups

# v1.1.1

🚀 New Feature

- Bot now works on any location (Greenhouse included).
- Any action button set in options now works, not just right-click.

🐛 Bug Fix

- Fixed bug where groups of only unwalkable crops broke the bot.

💅 Polish

- If all crops are grouped, skips TSP algorithm.

# v1.1.0

🚀 New Feature

- Doesn't start bot when harvesting plants and accidentally holding Watering Can.
- Doesn't water crops that are ready to harvest, unless they are crops that regrow after harvest.
- Refills water instantly if needed, instead of walking to the first group at the start.
- Update keys linked to Nexus and Github.

🐛 Bug Fix

- Works with all Watering Can upgrades.
- Correctly finds standing place next to refillable tile.
- Finding the nearest refillable tile should and take less memory.
- Checks for accidental empty groupings.
- Now exits task if now suitable refill tile is reachable.

💅 Polish

- Removal of unnecessary imports.
- Provided console feedback on major events.

# v1.0.0

🚀 New Feature

- Waters your crops upon right-clicking a crop.
- Refills Watering Can when nessisary.
