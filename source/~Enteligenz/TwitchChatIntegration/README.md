**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Enteligenz/StardewMods**

----

# Twitch Chat Integration

Uses a Twitch chat bot to read messages from Twitch chat and displays them inside of the Stardew Valley chat.
Every message sender is assigned some color that their messages will be displayed in for better readability,
though these colors are not unique to each person.
In multiplayer, the Twitch messages will only be sent to the chat of the mod user, the other players won't see them.

## Usage
You will need to provide a Twitch account that the chat bot can use, you can use the one you use to stream or a separate one.
To log in the bot needs a user name and an OAuth token, which you can create here: https://twitchapps.com/tmi/.
Then you need to fill in this information along with the name of the channel you want to read the chat from into the file ``config.json``
inside of the mod folder that is created when first starting the game with this mod installed.

## Acknowledgements

The idea for this mod comes from [RyanGoods](https://github.com/StardewModders/mod-ideas/issues/1047).

The code for the Twitch bot comes from [this article](https://medium.com/swlh/writing-a-twitch-bot-from-scratch-in-c-f59d9fed10f3),
I seriously couldn't have made this mod without it.
