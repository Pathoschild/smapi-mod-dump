# Internationalization

## Description
A tool for translating mods that use SMAPI's I18n API. It allows you to translate 
those mods live-ish in-game (including this one).

This mod is intended to make translating mods more accessible. It was originally 
designed to make it easier to update existing translation files. 
Now it also handles the JSON encoding aspects, so you don't have to. 

The editor indicates which of the mods have full/partial/no translation and what 
languages a mod has been translated into. Red indicates no translation. Yellow 
means there is one. Green indicates that the mod is fully translated for that 
language.

It runs from within the game, which means that it also updates the translations 
live (unless they're copied/cached by the mod). 
However the editor needs to be opened inside a browser, which has better support 
for text editing.

The editor has support for custom languages. If you have a custom language mod
installed, or if any of the mods have a translation for a language, then that
language will be selectable inside the editor.

## Development
This mod should be fully functional to translate existing mods that use the 
SMAPI i18n API. However its still in 'early access'. I don't know if there's any
bugs, missing features, or how people use this mod. So please let me know. I'd
love to hear from people how you use it, so I can help better integrate it into
your workflow.

## Usage
First make sure that the mod you want to translate has an i18n folder. If it does
not, this mod won't be able to help you translate it. Install that mod and make
sure that SMAPI loads it when you start the game.

You also need to install this translation mod. It has no dependencies other than 
SMAPI. After starting the game, go to [http://localhost:8018](http://localhost:8018). 
Choose what mod you want to translate and the language you want to translate to.

Under "new translation" you can start editing right away. Click a text and type 
its translation. This will directly update the translation in-game too. Although
this is still temporary. Restarting the game at this point will revert all your 
changes.

Once you're done, click save to store your translations within your game files. 
Then they will be loaded next time you start Stardew Valley. Or click download 
to download the translation file to your computer. Then maybe share it with the 
mod author.

Note: if you accidentally close the game while you were editing a language file,
the download button will still work.

## Changes
#### 0.3:
* French and Vietnamese translations.

#### 0.2:
* Fix bug with not parsing escaped quotes in strings correctly.
* Localization support: it can now translate itself.
* Progress indicator for current file.

#### 0.1:
* First release

## ToDo:
Stuff that may or may not be added in a future release. Let me know if you're 
particularly interested in one of these.

* Option to add new entries.
* Filter lines (error/untranslated/search query)
* Metadata like translation author name, update date, SMAPI & mod version.
* An in-game button to open the editor.
* Validation of {{tag}} usage.
* Upload file from disk (for easier importing).
* Preview using in-game bitmap font
* Add working directory setting to automatically save to git projects as well.
* Maybe integrate [translation summary script](https://github.com/Pathoschild/StardewScripts/blob/main/create-translation-summary/create%20translation%20summary.linq)
* A standalone version?
