GAME_DIR=${HOME}/GOG Games/Stardew Valley/game
MOD_DIR_CP=${GAME_DIR}/Mods/HatMouseLacey
MOD_DIR_SMAPI=${GAME_DIR}/Mods/HatMouseLacey_Core

install: smapi cp

smapi:
	cd SMAPI && dotnet build /clp:NoSummary
	install -m 644 LICENSE "${MOD_DIR_SMAPI}/"

cp:
	mkdir -p "${MOD_DIR_CP}"
	install -m 644 CP/content.json CP/manifest.json "${MOD_DIR_CP}/"
	/bin/cp -r CP/assets CP/compat CP/data CP/i18n "${MOD_DIR_CP}/"
	install -m 644 LICENSE "${MOD_DIR_CP}/"
	install -m 644 docs/nonlicensed.txt "${MOD_DIR_CP}/"

palettes: source/tint
	$< "source/house.png" CP/assets/house
	$< "source/storefront.png" CP/assets/house
	$< "source/hatmouselaceyStall.png" CP/assets/maps
	$< "source/hatmouselaceyInterior.png" CP/assets/maps
	$< "source/ellehouse.png" CP/assets/textures
	$< "source/ellestorefront.png" CP/assets/textures

source/tint: source/tint.c
	${CC} -o $@ $< -lpng

clean:
	rm -rf SMAPI/bin SMAPI/obj

uninstall:
	rm -rf "${MOD_DIR_CP}" "${MOD_DIR_SMAPI}"
