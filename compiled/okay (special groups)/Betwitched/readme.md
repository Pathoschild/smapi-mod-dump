Betwitched
===========

A Stardew Valley Twitch mod.

Version 0.9.0

# Important Notice
This is a beta version! Use it at your own risk!

# Features
- connects to a twitch chat
- retrieves chat user names
- propose those names when naming babies or animals
- fallback to vanilla when there aren't enough chat users available
- different configurable name selection methods

optional:
- black listing e.g. bots
- farmer speech bubbles informing you about new subs
- new chat command to fill a farmers speech bubble, for free or for bits
- speech bubbles for all named in game characters with their chat content


# Details
Once a chat user either writes a message, subscribes, raids or is
otherwise public visible, his name is taken as a potential proposed
name in game. All user names are stored within a big list. This list
is discarded once the game ends.
Whenever a chat user subscribes, your farmer informs you about it
by an in game speech bubble and a customizable text. A new command
allows you to offer your chat to write into a farmers speech bubble too,
either for free or for bits.
Additionally for all in game animals or characters with a users name
a speech bubble with the according users chat messages opens up,
showing that message.

## Farmer Command
When enabled, a chat user can 'send' you a messer in a farmers speech bubble.
the commands format is quite simple:

  !farmer text to be shown

You can ask for a bit fee in order to use the command. Say the fee is 100 bits,
then the user has to type in

  cheer100 !farmer text to be shown.

Otherwise the user is informed that the number of bits he used for the command
isn't sufficient.

## Name selection methods
Currently the following methods are available:

### roundrobin
The user list is seen as a FIFO (first in, first out) list.
The user name on first position is proposed first and moved to
the end of the list. There is no randomness in user name selection.

### default behaviour
Beside of the overall user name list another list of already proposed
user names is kept. On user name proposal, a random user name is proposed
which is in the overall list but not in the already proposed user names list.
The new proposed user name is then also stored in the already proposed user
names list to prevent double selection.

When the already proposed user names list reaches its size limit, the
oldest entry is removed from the list when adding a new user name.
The size limit is configurable within the mods configuration by setting
"ProposedListLimit".

#### Edge Cases
When the number of overall available user names is less than the size limit
of the already proposed user names list, then it can result in having no
user names left for proposal. By setting the "ResetListWhenEmpty" in the
mods configuration to "true", the already proposed user names list is
cleared in that case.

# Restrictions
- Only names of those users are taken, who are public visible by
  sending chat messages, subscribing, donating subscriptions or
  raiding the observed chat.
- Currently there is no differentiation of gender. this results in
  potentially proposing a male name for a female baby and vice versa.

# Twitch Authorization
To allow the mod to act on your behalf, it needs your authorization.
Thankfully Twitch defines different privileges which you may grant based
on an apps task. It's not necessary to grant e.g. the right to ban users,
when it isn't in an apps scope, and you never should do so.
The mod uses oauth for authorization and it needs therefore an user
name and an authorization token.
There are several web sites which offer to create those. A nice site
where you can set all desired privileges is https://twitchtokengenerator.com/ .
At best select the _Bot_ option which just grants chat read and write
access. Specify the to be used user name in "TwitchUsername" and whatever
is shown behind _ACCESS TOKEN_ as TwitchAuthToken. You also have to specify
the channel to observe, when the user name and the channels name differ.

# Configuration
Explanation of configuration values in "config.json":

### Enabled
Enables or disables the mod in whole.
Valid values: true, false.
Default: true

### TwitchUsername
The user name to use for connecting to Twitch API. This name is
also taken as channel name when none is given. See also __Twitch Authorization__.

### TwitchAuthToken
The auth token to use for connecting to Twitch API.
See also __Twitch Authorization__.

### TwitchChannel
The channel to observe. If non is given, the value of "TwitchUsername" is taken.

### NamePrefix
In order to be able to differentiate between names taken by vanilla
or from twitch users, you may specify a prefix for Twitch user names.
When you specify e.g. "twitch." and a selected twitch user name is
"player", the proposed name results in "twitch.player".
Default: "Twitch."

### ResetListWhenEmpty
Valid values: true, false.
See __Edge Cases__ for more details.
Default: false

### SelectionMethod
Valid values: "roundrobin", "default".
See __Name selection methods__ for more details.
Default: "roundrobin"

### Blacklisted
Use this string array to specify user names which shouldn't be
proposed at all, e.g. bot names.
Default: ["StreamElements", "Nightbot", "Streamlabs", "Fossabot"]

### DisableSpeechBubbles
Valid values: true, false
Setting this to true will disable ALL mods in game speech bubbles.
Default: false

### SpeechBubbleOpacity
Valid values: any float from 0 to 1.
Defines the maximum opacity of speech bubbles.
Default: 0.9

### SpeechBubbleMsDuration
Duration in milliseconds of how long a speech bubble should be visible.
Default: 4000

### SpeechBubbleMsFadeInOut
Duration in milliseconds of speech bubble fading in and out.
Default: 500

### NewSubText
Text shown in a farmers speech bubble on new subs.
The term "{0}" is replaced with the users name.
Default: "Welcome onboard {0}!"

### ReSubText
Text shown in a farmers speech bubble on resubs.
The term "{0}" is replaced with the users name.
Default: "Enjoy your resub, {0}!"

### PrimePaidSubText
Text shown in a farmers speech bubble on paid subs?!
The term "{0}" is replaced with the users name.
Default: "{0}, thank you for your sub!"

### GiftedSubText
Text shown in a farmers speech bubble on gifted subs?!
The term "{0}" is replaced with the users name.
Default: "GiftedSub {0}!"

### CommunitySubText
Text shown in a farmers speech bubble on community subs?!
The term "{0}" is replaced with the users name.
Default: "CommunitySub {0}!"

### FarmerCmdMinBits
The minimum number of bits needed for the farmer command. When it's
negative, the command is disabled. When it's 0, everybody may use that
command (beware) and when it above 0, then it specifies the number of bits
which must be spent together with the command.
See _Farmer Command_ for more detail.
Default: -1

### FarmerCmdTooLessBits
Text shown to a user when he uses the farmers command but did not
spent sufficient bits.
The term "{0}" is replaced with the number of bits needed.
Default: "Not enough bits! You need to spend at least {0} bits for using this command"

# Version History

### Version 0.9.0
- Added speech bubble support
- Added farmer command
- Added in game sub notification
- Added speech bubbles for twitch namend in game characters
- Fixed bug: mod crashes when disabled
- Minor improvements

### Version 0.8.1
- Added better error handling in case of missing or wrong Twitch credentials.
- Corrected the default config.json
- Added Nexus id to manifest.json

### Version 0.8.0
Initial version


