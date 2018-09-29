# AutomatedDoors
Automated Doors 

Automatically open and close buildings doors on your farm.
Defaults to not open in winter, and at 0620 / 1810 open and close.

Example config format:
```json
{
  "TimeDoorsOpen": 620,
  "TimeDoorsClose": 1810,
  "OpenOnRainyDays": false,
  "OpenInWinter": false,
  "Buildings": {
    "Andrei": {
      "Coop94036": true,
      "Barn74036": true
    }
  }
}

```


---

## Authors:

* Original: [Phate](http://community.playstarbound.com/threads/smapi-0-37-automatic-open-close-animal-doors-at-specific-time.109455/)
* Maintainer: Andrew Zah - [azah](https://andrewzah.com)
