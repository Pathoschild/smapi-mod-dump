**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jaredlll08/HotbarHotswap**

----

# HotbarHotswap
This is a template repository that provides a base solution for developing Stardew Valley mods with [SMAPI](https://smapi.io) with Jenkins providing CI/CD.

When enabled, the zip file that is produced is uploaded to CurseForge.

## Developing
Nothing is changed with how you develop your mod, just develop your mod like your normally would, and it will be built by Jenkins.

## Requirements
The server requirements are:
* Linux Based (Tested on Ubuntu 20.04)
* Jenkins
* Docker
* Stardew Valley files in `/opt/stardewvalley` (the path can be changed in your Jenkinsfile)

## Configuration
The `build.json` file needs to be filled out, the game versions can be found on CurseForge, if you are developing for a version that isn't listed on CurseForge, reach out to them to get it listed!

### build.json Example and Spec

```json
{
  "curseProjectId": "297655", // The project ID of the project on CurseForge
  "version": "1.0.%build_number%", // Your mod's version, use %build_number% to replace it with the Jenkins build number
  "gameVersions": ["1.4.5", "1.4.4"], // Array of versions that this project supports
  "relations": [ // optional, the slug is the CurseForge project slug and the type can be one of ["embeddedLibrary", "incompatible", "optionalDependency", "requiredDependency", "tool"]
    {
      "slug": "bookcase",
      "type": "requiredDependency"
    }
  ],
  "changelogType": "text", // What format should the changelog be presented in, must be one of ["text", "html", "markdown"]
  "changelogFile": "changelog.md", // a path to the changelog file
  "releaseType": "release", // The type of release, must be one of ["alpha", "beta", "release"]
  "deploy": false // Should the file be uploaded to CurseForge, useful when you are making changes and don't want the file to be uploaded straight away
}
```

The manifest.json needs to be filled out as normal, however, the `"Version'` field should not be touched, it should be kept as `1.0.0-no-op`, this is so that Jenkins can replace it with the actual version in `build.json`.

To use the CurseForge api, you need an API key, you can get one from [here](https://www.curseforge.com/account/api-tokens).
Once you have the token, make a new file called `secrets.json`, and put the following:
```json
{
  "curseApiKey": ""
}
```
Once you have the file, add it as a secrets file credential on Jenkins, the credentials id should be called `mod_build_secrets`.

In the `Jenkinsfile`, change `modName` to be the name of your solution