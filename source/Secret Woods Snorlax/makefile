GAME_DIR=${HOME}/GOG Games/Stardew Valley/game
MOD_DIR=${GAME_DIR}/Mods/SecretWoodsSnorlax

install: smapi

smapi:
	dotnet build

clean:
	rm -rf bin obj

uninstall:
	rm -rf "${MOD_DIR}"
