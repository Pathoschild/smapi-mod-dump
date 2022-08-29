#!/bin/bash

if uname -r | grep -q "microsoft" 
then
    FACTIORIO_PATH=$APPDATA"/Factorio/mods/"
else
    FACTIORIO_PATH=~/.factorio/mods
fi

INFO_JSON=info.json

JQ_ARGN=.name
JQ_RESPONSEN=$(cat $INFO_JSON | jq -r "$JQ_ARGN")
JQ_ARGV=.version
JQ_RESPONSEV=$(cat $INFO_JSON | jq -r "$JQ_ARGV")

MOD_NAME=$JQ_RESPONSEN
ZIP_NAME="$MOD_NAME""_""$JQ_RESPONSEV"".zip"

python=`cat <<HEREDOC
import zipfile, os

def zipDir(path, ziph):
    for root, dirs, files in os.walk(path):
        for file in files:
            if(file != "build.sh" and file != '$ZIP_NAME'):
                print("Adding:\t{}".format(os.path.relpath(os.path.join(root, file))))
                ziph.write(os.path.join(root, file), os.path.relpath(os.path.join(root, file), os.path.join(path, '..')))

with zipfile.ZipFile('$ZIP_NAME', 'w', zipfile.ZIP_DEFLATED) as ziph:
    zipDir('.', ziph)
    print("Done:\t{}".format('$ZIP_NAME'))
HEREDOC
`

python3 -c "$python"

if [ -f $FACTIORIO_PATH$ZIP_NAME ]; then
    rm $FACTIORIO_PATH$ZIP_NAME
fi
mv $ZIP_NAME $FACTIORIO_PATH