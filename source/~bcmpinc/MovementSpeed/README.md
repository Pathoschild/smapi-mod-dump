# Movement Speed
Changes the player's movement speed and charging time of the hoe and watering can.

## Config
* MovementSpeedMultiplier: The movement speed is multiplied by this amount. The default is 1.5, meaning 50% faster movement. 
* ToolChargeDelay: Time required for charging the hoe or watering can in ms. Normally this is 600ms. The default is 600/1.5 = 400, meaning 50% faster charging.

## Known bugs
none

## Changes
#### 0.4:
* Fixed incompatibility with Mouse Move Mode mod.
* Setting `MovementSpeedMultiplier: 1` or `ToolChargeDelay: 600` will disable the associated patch.
