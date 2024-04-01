import os
import glob
import re

VERSION = "7.1"

for f in glob.glob("*/manifest.json"):
    name = os.path.basename(os.path.dirname(f))
    print("Bumping", name)
    with open(f) as file:
        text = file.read()

    # Bump mod version
    #text = re.sub('"Version": "[\d.]+",', '"Version": "'+VERSION+'",', text)

    # Bump api version
    text = re.sub('"MinimumApiVersion": "[\d.]+",', '"MinimumApiVersion": "4.0.0",', text)

    # Bump StardewHack version
    #text = re.sub('("UniqueID": "bcmpinc[.]StardewHack",\s+"MinimumVersion":) "[\d.]+"', '\\1 "'+VERSION+'"', text)

    with open(f, "w") as file:
        file.write(text)


for f in glob.glob("*/*.csproj"):
    name = os.path.basename(os.path.dirname(f))
    print("Bumping", name)
    with open(f) as file:
        text = file.read()

    # Bump solutionversion
    #text = re.sub('<Version>\d[.]\d', '<Version>' + VERSION, text)

    with open(f, "w") as file:
        file.write(text)
      
