**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/slothsoft/stardew-challenger**

----

# Challenger

_A Mod for [Stardew Valley](https://www.stardewvalley.net/)_

[![MIT Licence](https://img.shields.io/github/license/jenkinsci/java-client-api.svg?label=License)](LICENSE)

- **Author:** [Stef Schulz](mailto:s.schulz@slothsoft.de)
- **Repository:** <https://github.com/slothsoft/stardew-challenger>
- **Open Issues:** <https://github.com/slothsoft/stardew-challenger/issues>

A mod that makes playing a challenge again. It allows you to select a challenge from a list, 
which restricts some parts of the game and lets you focus on another part.

<img alt="Screenshot" src="readme/screenshot.png" width="300"/>

**Content of this ReadMe:**

- **[User Manual](#user-manual)**
    - [Prerequisites](#prerequisites)
    - [Installing](#installing)
    - [Removing the Mod](#removing-the-mod)
    - [Using the Mod](#using-the-mod)
        - [No Challenge](#no-challenge)
        - [Brewery](#brewery)
        - [Hermit](#hermit)
        - [No Capitalist](#no-capitalist)
        - [Vineyard](#vineyard)
    - [FAQ](#faq)
    - [Configuration](#configuration)
    - [Versions](#versions)
- **[Translator Guide](#translator-guide)**
- **[Developer Notes](#developer-notes)**
- **[License](#license)**



## User Manual

### Prerequisites

You need **[SMAPI](https://smapi.io/)** to run any kind of mods. And of course you need
**[Stardew Valley](https://www.stardewvalley.net/)**.


### Installing

1. Install [SMAPI](https://smapi.io/)
1. Unzip the latest _[Challenger.zip](https://github.com/slothsoft/stardew-challenger/releases/latest)_ to the _Mods/_ folder
1. If you want to use spacechase0's [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098), this mod automatically hooks into its menu
1. If you want to use Pathoschild's [Automate](https://www.nexusmods.com/stardewvalley/mods/1063), unzip the _[ChallengerAutomate.zip](https://github.com/slothsoft/stardew-challenger/releases/latest)_, too



### Removing the Mod

<img alt="Screenshot" align="right" src="readme/screenshot_revert.png" width="100">

1. Just remove the _Challenger/_ folder (and _ChallengerAutomate/_ if you have it).
1. If you have a **Magical Challenge Object**™ on your map, it should revert to a pink plush bunny
1. If you experienced bugs with the mod, please take the time to [report them](https://github.com/slothsoft/stardew-challenger/issues)



### Using the Mod

The mod is automatically present in the game. You will get an mail with the **Magical Challenge Object**™
the next time you wake up.

<img alt="Screenshot Mail" src="readme/screenshot_mail.png" width="300"/>

Using the attached object in its unmodified form will open the challenge menu. You can also open the menu using the
key [K] (you can customize the key in either the mod's config file or the [GenericModConfigMenu](https://www.nexusmods.com/stardewvalley/mods/5098)).

<img alt="Screenshot Mail" src="readme/screenshot_menu.png" width="300"/>

This will display all the restrictions you have to face - but you might get a little something extra to
make the challenge possible. The **Magical Challenge Object**™ will turn into this "something extra" once you
start the challenge and can be used as any other object of the kind it turns into.

After the challenge was finished, nothing really happens. The restrictions stay in place, but at least you get a congratulatory mail.
[The plan](https://github.com/slothsoft/stardew-challenger/issues/76) is to get a reward in future versions.

This mod brings the following challenges:

- [No Challenge](#no-challenge)
- [Brewery](#brewery)
- [Hermit](#hermit)
- [No Capitalist](#no-capitalist)
- [Vineyard](#vineyard)

#### No Challenge

- default value without any restrictions - for when you are sick and tired of following rules


#### Brewery

**Goal:** To earn gold by selling beer [2.500.000 for easy, 5.000.000 for medium or 10.000.000 for hard]

- rice juice is renamed to rice beer
- only beers (including the new rice beer) count towards the goal
- you can only buy seeds for plants that make beer (which includes rice)
- you cannot build animal buildings, i.e. coop, barn and silo
- [easy and medium] you get a keg to start you off


#### Hermit

**Goal:** [easy and medium] To finish the community center / [hard] To reach perfection

- You cannot leave the farm, except on Sundays
- [easy] you get a seed maker


#### No Capitalist

**Goal:** To finish the community center

- You cannot buy from Pierre or the Joja Mart
- [hard] You cannot buy from Clint either (upgrading tools and opening geodes at Clint's is still possible though)
- [easy] You get a seed maker to tide you over your imminent lack of seeds


#### Vineyard

**Goal:** To earn gold by selling wine [2.500.000 for easy, 5.000.000 for medium or 10.000.000 for hard]

- rice juice is renamed to rice wine
- only wine and mead (and the new rice wine) count towards the goal
- you can only buy seeds for plants that make wine (which includes rice)
- you cannot build animal buildings, i.e. coop, barn and silo
- [easy and medium] you get a keg to start you off


### FAQ

#### Can I disable / pause the challenges?

Yes, absolutely. You can just select "no challenge" in the challenge menu at any time to 
pause the challenge. 
So if not buying from Clint is no fun for you, you can just pause the challenge, buy, 
and then select the challenge again. Your progress might not get counted if the challenge 
is paused, but you're not losing the current progress either.

#### The bunny turned into a Seedmaker and now I can't open the menu?

You don't need the bunny, you can also press [K] to open the same menu. Then when you select 
another challenge, the seedmaker will change into another object (or back to the bunny again).

#### Does the mod change the inventory / farm?

Your inventory and the farm will not be changed, only the **Magical Challenge Object**™
might change. This mod is only applied to current interactions. 

#### Can you start challenges mid-playthrough?

You can start the challenges at any time, and you can even pause them (by selecting
"no challenge" in the menu) whenever you feel like it.



### Configuration

If you wish to configure this mod, it's advised to install spacechase0's
[Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098). Then there will be a config
page.

<img alt="Screenshot" src="readme/screenshot_config.png" width="300"/>

But if you wish to change the _config.json_ file manually, you can also do that. The _config.json_ with all
entries is:

```json
{
     "ButtonOpenMenu": "K"
}
```

| Key in JSON      | Config Name | Default Value | Explanation                                                                                       |
|------------------|-------------|---------------|---------------------------------------------------------------------------------------------------|
| `ButtonOpenMenu` | Menu Button | K             | The button to use when you want to open the menu to select the challenge or observe your progress |



### Versions

| Version | Issues                                                                          | Changes                                                                                                                                                                                                                                  |
|---------|---------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Future  | [Issues](https://github.com/slothsoft/stardew-challenger/milestone/4)           |                                                                                                                                                                                                                                          |
| 0.5.0   | [Issues](https://github.com/slothsoft/stardew-challenger/milestone/10?closed=1) | - add difficulty selection<br>- re-balancing of challenges using this difficulty feature<br>- basic multiplayer support (only the host can change the challenge, but all players (with this mod) contribute to it)<br>- various bugfixes |
| 0.4.3   | [Issues](https://github.com/slothsoft/stardew-challenger/milestone/9?closed=1)  | Release blockers II                                                                                                                                                                                                                      |
| 0.4.2   | [Issues](https://github.com/slothsoft/stardew-challenger/milestone/8?closed=1)  | Release blockers                                                                                                                                                                                                                         |
| 0.4.1   | [Issues](https://github.com/slothsoft/stardew-challenger/milestone/7?closed=1)  | 2nd alpha test bugfixes                                                                                                                                                                                                                  |
| 0.4.0   | [Issues](https://github.com/slothsoft/stardew-challenger/milestone/6?closed=1)  | After play testing version 0.2.x and 0.3.x it turned out the challenges have some mayor flaws - so mod was revamped to make it playable                                                                                                  |
| 0.3.1   | [Issues](https://github.com/slothsoft/stardew-challenger/milestone/3?closed=1)  | Fix of the most glaring bugs                                                                                                                                                                                                             |
| 0.3.0   | [Issues](https://github.com/slothsoft/stardew-challenger/milestone/5?closed=1)  | More challenges                                                                                                                                                                                                                          |
| 0.2.0   | [Issues](https://github.com/slothsoft/stardew-challenger/milestone/2?closed=1)  | Alpha test                                                                                                                                                                                                                               |
| 0.1.0   | [Issues](https://github.com/slothsoft/stardew-challenger/milestone/1?closed=1)  | Proof of concept                                                                                                                                                                                                                         |

_(All issues can be found [here](https://github.com/slothsoft/stardew-challenger/issues).)_



## Translator Guide

Right now, not all translations are provided. If you want to help, translate either file in the _[i18n/](Challenger/i18n/)_
and test it right in your Stardew Valley (the same folder exists in your _Mods/_ folder). You can than provide it
to me via [pull request](https://github.com/slothsoft/stardew-challenger/fork) or [email](mailto:s.schulz@slothsoft.de).

| Key       | Language  | Translated? |
|-----------|-----------|-------------|
| `default` | English   |  ✅  |
| `de`      | German    |  ✅  |
| `es`      | Spanish   |             |
| `fr`      | French    |             |
| `hu`      | Hungarian |             |
| `it`      | Italian   |             |
| `ko`      | Korean    |             |
| `pt`      | Portugese |             |
| `ru`      | Russian   |             |
| `tr`      | Turkish   |             |
| `zh`      | Chinese   |             |



## Developer Notes

Can be found [here](readme/dev-notes.md).


## License

This project is licensed under the MIT License - see the [MIT license](LICENSE) for details.