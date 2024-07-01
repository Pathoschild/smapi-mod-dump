GAME_DIR=${HOME}/GOG Games/Stardew Valley/game
MOD_DIR=${GAME_DIR}/Mods/SecretNoteFramework

install: smapi

smapi:
	dotnet build /clp:NoSummary
	install -m 644 LICENSE "${MOD_DIR}"

clean:
	rm -rf bin obj

uninstall:
	rm -rf "${MOD_DIR}"
