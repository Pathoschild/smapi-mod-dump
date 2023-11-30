# FIXME revert to GOG location after 1.6 release
GAME_DIR=${HOME}/.local/share/Steam/steamapps/common/Stardew Valley
MOD_DIR=${GAME_DIR}/Mods/NPCGeometry

install: smapi

smapi:
	dotnet build
	install -m 644 LICENSE "${MOD_DIR}"

clean:
	rm -rf bin obj

uninstall:
	rm -rf "${MOD_DIR}"
