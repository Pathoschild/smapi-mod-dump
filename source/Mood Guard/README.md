# Mood Guard
A Stardew Valley mod that fixes multiple problems with animal happiness.

As of version 1.1, this mod currently fixes the following two bugs:
* After 6pm, animals' happiness drains every 10 minutes that the animals are inside and you stay awake
* When you have a relevant profession (Coopmaster or Shepard) and pet a matching animal with nearly full happiness, that animal becomes unhappy

These two fixes can be enabled or disabled individually.

# Config

The mod config can be edited by changing the file `config.json`. By default, the file looks like this:
```json
{
    "NightFix": {
        "Enabled": true,
        "Mode": "Standard"
    },
    "ProfessionFix": {
        "Enabled": true
    }
}
```

## NightFix Options

Changing `Enabled` to `false` will disable the fix for post-6pm happiness drain.

There are three different modes that you can choose from:
* `Standard` - Animals will no longer lose happiness from you being awake after 6pm
* `Increased` - Animals will gain happiness by being inside after 6pm
* `Maximized` - Animals' happiness will continually be set to maximum (255)

## ProfessionFix Options

Changing `Enabled` to `false` will disable the fix for profession-based happiness overflow

# Known Bugs

When the profession fix activates, it will occasionally cause the animal's status window to appear before it shows the heart/frustration bubble.

# Download
Downloads can currently be found at the [project releases](https://github.com/YonKuma/NightChicken/releases).
