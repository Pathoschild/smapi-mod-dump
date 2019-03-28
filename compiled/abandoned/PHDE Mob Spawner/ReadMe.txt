PHDE MobSpawner - 1.2.0
This mod spawns as much as possible of the different kind of monsters, which are required for the "Protector Of The Valley" achievement. 

It is configurable on which mineshaft level which mob should spawn. The config file is created on the game boot up. 

:: REQUIREMENTS ::
* SMAPI

:: FEATURES ::
* Spawns Bats, Duggys, DustSpirits, Skeleton, GreenSlime and Shadowbrute in the mineshaft which you have configured

* you can configure now how many monster should spawn on each level
* syntax is: level:count
* for example 5:10 means on level 5 the monster will spawn 10 times (it may be limited by the mineshaft space so you cannot spawn 10000000 mobs in one mineshaft, all is limited by the mineshaft space)
* see also config.json.extended
* if no count is specified in the config the default spawn count for each monster will be 100
* if no level is specified the default level where monsters spawn is lvl 119