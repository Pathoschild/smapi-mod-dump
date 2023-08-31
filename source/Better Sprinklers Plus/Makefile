.ONESHELL:
.SILENT:

ifneq (,$(wildcard ./.env))
    include .env
    export
endif

SOURCE="${HOME}/.steam/debian-installation/steamapps/common/Stardew Valley/"
DEST="./stardew"

tasks/copy-stardew:
	if [ ! -d ${DEST} ]; then \
		if [ -d ${SOURCE} ]; then \
			echo "stardew found"; \
			cp -r ${SOURCE} ${DEST}; \
		else
			echo "Stardew not found, change it in Makefile"; \
			exit 1; \
		fi
	else \
		echo "Already copied"; \
	fi
.PHONY: tasks/copy-stardew

tasks/bump-version:
	docker compose run smapi ./set-version.sh
.PHONY: tasks/bump-version

plugin/build: docker/build-image/release
	docker compose run smapi ./build.sh
.PHONY: plugin/build

docker/shell:
	docker compose run smapi bash
.PHONY: docker/build-image/dev

docker/build-image/dev: tasks/copy-stardew
	docker build --tag jamescodesthings/smapi:latest --build-arg PASSWORD --progress=plain --no-cache .
.PHONY: docker/build-image/dev

docker/build-image/release: tasks/copy-stardew
	docker build --tag jamescodesthings/smapi:latest --build-arg PASSWORD .
.PHONY: docker/build-image/release

docker/build-image: docker/build-image/release
.PHONY: docker/build-image

docker/publish-image: docker/build-image/release
    docker push jamescodesthings/smapi:tagname
.PHONY: docker/publish-image