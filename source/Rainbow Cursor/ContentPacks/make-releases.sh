#!/bin/sh
# Copyright 2023 Jamie Taylor

basedir="$(dirname "$0")"
releasedir="Releases"

for cp in "$basedir"/*; do
    if test -r "$cp/manifest.json"; then
        ver="$(sed -nE -e 's/ *"Version" *: *"([0-9]+\.[0-9]+\.[0-9]+(\.[^"\. ]+)?)".*/\1/p' "$cp/manifest.json")"
        if test "$ver" == ""; then
            echo "Could not get version from $cp/manifest.json"
            continue
        fi
        cpname="$(basename "$cp")"
        zipname="$releasedir/$cpname $ver.zip"
        if test -e "$basedir/$zipname"; then
            echo "Release zip already exists for $cpname version $ver"
            continue
        fi
        echo "Creating release zip for $cpname version $ver"
        needs_license=
        if ! test -r "$cp/LICENSE"; then
            needs_license=non-empty
            ln -s ../../LICENSE "$cp/LICENSE"
        fi
        ( cd "$basedir" && zip -r "$zipname" "$cpname" --exclude \*/.DS_Store )
        if test -n $needs_license; then
            rm "$cp/LICENSE"
        fi
    fi
done

