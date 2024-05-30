**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Smoked-Fish/ParticleFramework**

----

### Particle Effect Variables
###### **key**: The unique key for the particle effect
###### **type**: The type of thing to target; one of "object", "furniture", "npc", "location", "hat", "shirt", "boots", "pants", "tool", "ring"
###### **name**: The qualified item id or name of the target
###### **follow**: If set to true, the particles will match the movment of the player or npc
###### **movementType**: How particles move; one of "none", "away", "towards", "up", "down", "left", "right", "random"
###### **movementSpeed**: How many pixels a particle moves per tick
###### **acceleration**: How many pixels/tick the movement speed increases per tick
###### **frameSpeed**: How many animation frames change per tick (see textures below)
###### **restrictOuter**: Whether particles disappear when they move outside the field (default false)
###### **restrictInner**: Whether particles disappear when they move inside the inner boundary (default false)
###### **belowOffset**: If set to > 0, allows particles to move behind the thing by the given layer depth offset (default -1)
###### **aboveOffset**: If set to > 0, allows particles to move in front of the thing by the given layer depth offset (default 0.001)
###### **minRotationRate**: If set to > 0, allows particles to rotate around their center
###### **maxRotationRate**: If set to > 0, allows particles to rotate around their center
###### **minAlpha**: Allows transparency (default: 1)
###### **maxAlpha**: Allows transparency (default: 1)
###### **particleWidth**: The width of each particle on the sprite sheet
###### **particleHeight**: The height of each particle on the sprite sheet
###### **fieldOuterRadius**: If set to > 0, specifies a circular particle spawn field and sets the outer radius of the field
###### **fieldInnerRadius**: If set to > 0, sets the inner radius of the field
###### **fieldOuterWidth**: If fieldOuterRadius is set to 0, designates the outer width of the rectangular particle field
###### **fieldOuterHeight**: If fieldOuterRadius is set to 0, designates the outer height of the rectangular particle field
###### **fieldInnerWidth**: If set to > 0, sets the inner width of the rectangular particle field
###### **fieldInnerHeight**: If set to > 0, sets the inner height of the rectangular particle field
###### **fieldOffsetX**: For locations, specifies the X coordinate of the field center on the map; for everything else, specifies the field X offset from the thing's center
###### **fieldOffsetY**: For locations, specifies the Y coordinate of the field center on the map; for everything else, specifies the field Y offset from the thing's center
###### **maxParticles**: Sets the maximum number of simultaneous particles in a field
###### **minLifespan**: Minimum number of ticks a particle lives
###### **maxLifespan**: Maximum number of ticks a particle lives
###### **minParticleScale**: Minimum scale of each particle in the field
###### **maxParticleScale**: Maximum scale of each particle in the field
###### **particleChance**: The chance (0 to 1) that a new particle will be created, checked each tick
###### **spriteSheetPath**: The to this particle's spritesheet (check examples below).




### Content Patcher Example
```
{
	"Format": "2.0.0",
	"Changes": [
		{
			"Action": "EditData",
			"Target": "Mods/Espy.ParticleFramework/dict",
			"Entries": {
				"Espy.CPParticleFramework/darkThronePuffer": {
					"type":"furniture",
					"name":"(F)64",
					"movementType":"random",
					"movementSpeed": 2,
					"frameSpeed": 0.1,
					"acceleration":0,
					"minRotationRate":0.1,
					"maxRotationRate":0.1,
					"particleWidth":57,
					"particleHeight":58,
					"fieldInnerRadius":64,
					"fieldOuterRadius":128,
					"fieldOffsetX": 16,
					"fieldOffsetY": -32,
					"minParticleScale": 0.2,
					"maxParticleScale": 1,
					"maxParticles": 30,
					"minLifespan": 40,
					"maxLifespan": 80,
					"spriteSheetPath": "Mods/Espy.ParticleFramework/pufferchick"
				}
			}
		},
		{
			"Action": "Load",
			"Target": "Mods/Espy.ParticleFramework/pufferchick",
			"FromFile": "assets/pufferchick.png"
		}
	]
}
```

[Stardew Valley Vanilla IDs](https://mateusaquino.github.io/stardewids/) can be useful for finding ids. Items in the "Objects" tab need (O) in the id.

**Don't change "Target" unless you add the new target to the dictList with the Api.\
The entry key must be unique.**

### C# Example
```c#

private IParticleFrameworkApi _particleFrameworkApi;
private ParticleEffectData particleEffectData;

private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
{
        _particleFrameworkApi = Helper.ModRegistry.GetApi<IParticleFrameworkApi>("Espy.ParticleFramework");
        particleEffectData = new ParticleEffectData()
        {
                key = "Espy.CPParticleFramework/darkThronePuffer",
                type = "furniture",
                name = "(F)64",
                movementType = "random",
                movementSpeed = 2,
                frameSpeed = 0.1f,
                acceleration = 0,
                minRotationRate = 0.1f,
                maxRotationRate = 0.1f,
                particleWidth = 57,
                particleHeight = 58,
                fieldInnerRadius = 64,
                fieldOuterRadius = 128,
                fieldOffsetX = 16,
                fieldOffsetY = -32,
                minParticleScale = 0.2f,
                maxParticleScale = 1,
                maxParticles = 30,
                minLifespan = 40,
                maxLifespan = 80,
                spriteSheetPath = "assets/pufferchick.png"
        };
        _particleFrameworkApi.LoadEffect(particleEffectData);
}
```

You can find the [Api Interface](https://github.com/Smoked-Fish/ParticleFramework/blob/main/Framework/Interfaces/Internal/IParticleFrameworkApi.cs) here
