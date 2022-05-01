**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/evfredericksen/StardewSpeak**

----

## Creating a new release

* Update the *Unreleased* section of `CHANGELOG.md` with the latest version and commit.

* Run the `StardewSpeak\lib\speech-client\build.py` script with the Python virtual environment active. This will bundle the python code, build the C#, and create the release zip at `StardewSpeak\bin\Release\StardewSpeak {version}.zip`.

* Fill out the [new release form](https://github.com/evfredericksen/StardewSpeak/releases/new) with the version number as the title and the changelog notes (and any other comments) as the description. Upload the zip created in the previous step. Click Publish release.

* Update the latest release link in `readme.md` to point the newly uploaded zip.  Bump the manifest version to the next release number and add a new *Unreleased* section to `CHANGELOG.md`.