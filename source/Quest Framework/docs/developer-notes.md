**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/QuestFramework**

----

# Quest Framework developer notes

## Cloning the repository

### Clone via common StardewModsSolution repository

1. Clone PurrplingCat's [StardewModsSolution](https://github.com/purrplingcat/StardewModsSolution)
2. In cloned repo run script `init.sh` in native linux bash, WSL or GitBash on Windows (MinGW)
3. Enter QuestFramework directory `cd QuestFramework`
4. Now you can read the code, build and make changes
5. Make changes and commit them
6. If you need create fork, do it on GitHub and then add your fork as remote to your cloned QuestFramework repo
7. Push changes

### Clone directly

1. Clone this repository
2. Clone [PurrplingCore](https://github.com/purrplingcat/PurrplingCore)
3. Make changes and try it build (You probably need to create solution `.sln` file in parent directory for build)
4. Commit your changes
5. If you need, fork this repository
6. Add your fork as remote
7. Push your changes (to your fork)

## Project dependencies

- [SMAPI](https://smapi.io) (as compiled binary installed in your system. See instructions on SMAPI website)
- [PurrplingCore](https://github.com/purrplingcat/PurrplingCore) (shared project)
