# Contributing

You're welcome to do pull requests. :)

But bear in mind that this is my first ever C# project, and as such I only have a basic understanding of its various features and quirks.
So I would appreciate if you could add some details how your code works and what it does, so I can better understand what you did. Thanks!

# Troubleshooting

Some things I encountered when setting up the project.

## Error: "Can't import project"

Roslyn is in another castle. Just create a symlink:
```
/usr/lib/mono/msbuild/15.0/bin $ sudo ln -s /usr/lib/mono/msbuild/Current/bin/Roslyn/ Roslyn
```

Source: https://forum.manjaro.org/t/cannot-compile-with-msbuild-because-files-are-installed-in-wrong-location/107390

## Error "Can't find game path"

https://github.com/Pathoschild/SMAPI/blob/develop/docs/technical/mod-package.md#custom-game-path