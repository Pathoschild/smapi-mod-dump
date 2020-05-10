echo "Checking dependencies..."
npm i
echo "Compiling tools..."
tsc
echo "Compiling JSON..."
node ./tools/make-json.js ./src/content.jsonc ./src/manifest.json
echo "Copying assets..."
cp -r ./src/Characters ./bin/