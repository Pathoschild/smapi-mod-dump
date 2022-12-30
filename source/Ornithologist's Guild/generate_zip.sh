#/bin/bash

pwd=$(pwd)

# # Move release ZIPs
mkdir /tmp/og_release
find ./*/bin/Release -iname '*Ornithologists Guild*.zip' -exec mv "{}" /tmp/og_release \;

cd /tmp/og_release

# # Get original filename
fn=$(find . -iname 'O*.zip' -exec echo "{}" \;)

# # Extract release ZIPs
find . -iname '*.zip' -exec unzip "{}" -d /tmp/og_release \;
rm /tmp/og_release/*.zip
chmod -R 755 .

# # Copy final zip
zip -r "$fn" ./*
mv "$fn" "$pwd/"

# # Clean up
rm -rf /tmp/og_release