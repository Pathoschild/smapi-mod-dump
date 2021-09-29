import os
import glob
import re

files = glob.glob("*/*.csproj")

for f in files:
    name = os.path.basename(os.path.dirname(f))
    print("Bumping", name)
    with open(f) as file:
        text = file.read()

    # Bump api version
    text = re.sub('net452', 'net5.0', text)

    # Bump build package version
    text = re.sub('"3.4.0"', '"3.4.0-beta.20210813"', text)

    with open(f, "w") as file:
        file.write(text)
