# Changelog

## 1.0.0-beta.1

### Common changes

- Fixed saving game
- Added new global conditions: *MinDaysPlayed*, *MaxDaysPlayed*, *DaysPlayed* and *IsFarmerMarried*
- Added support for daily quests (day limited quests)

### For Content Pack modders

- You can define days left for quests (daily quests)
- Added token for use objects from JsonAssets in quest triggers. 
(example for ItemDelivery quest type: `Willy {{ja:Fish Oil}}`)

### For SMAPI modders

- You can define days left for quests (daily quests) via SMAPI mod-provided API
- Added trigger parser-loader in CustomQuest class

## 1.0.0-beta

- First release
