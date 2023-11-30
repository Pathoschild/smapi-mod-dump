GAME_DIR=${HOME}/GOG Games/Stardew Valley/game
MOD_DIR_CP=${GAME_DIR}/Mods/HatMouseLacey
MOD_DIR_SMAPI=${GAME_DIR}/Mods/HatMouseLacey_Core

CONVERT_PNGS=source/house.png \
	     source/storefront.png \
	     source/hatmouselaceyStall.png \
	     source/hatmouselaceyInterior.png \
	     source/ellehouse.png \
	     source/ellestorefront.png

install: smapi cp

smapi:
	cd SMAPI && dotnet build
	install -m 644 LICENSE "${MOD_DIR_SMAPI}/"

cp:
	mkdir -p "${MOD_DIR_CP}"
	install -m 644 CP/content.json CP/manifest.json "${MOD_DIR_CP}/"
	/bin/cp -r CP/assets CP/compat CP/data CP/i18n "${MOD_DIR_CP}/"
	install -m 644 LICENSE "${MOD_DIR_CP}/"
	install -m 644 docs/nonlicensed.txt "${MOD_DIR_CP}/"

palettes: source/tint
	@for img in ${CONVERT_PNGS}; do \
		./source/tint "$$img" CP/assets; \
	done

source/tint: source/tint.c
	${CC} -o $@ $< -lpng

clean:
	rm -rf SMAPI/bin SMAPI/obj

uninstall:
	rm -rf "${MOD_DIR_CP}" "${MOD_DIR_SMAPI}"
