#!/usr/bin/env sh

if [ ! -d "$GAME_PATH" ]; then
  echo "Extracting to $GAME_PATH"
  7z x /app/stardew.7z -o/app/stardew -p"${PASSWORD}" -y
else
  echo "already extracted"
fi

echo "Restoring"
dotnet restore

echo "Building"
dotnet build

echo "Copying plugin to root"
cp -f BetterSprinklersPlus/bin/Debug/net6.0/*.zip ./
