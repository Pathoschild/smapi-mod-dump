import os
import glob

files = glob.glob("../*/bin/Release/net5.0/*.zip")
print(files)

for f in files:
    name = os.path.basename(f)
    if os.path.exists(name):
        print("Skipping", name)
    else:
        print("Moving", name)
        os.rename(f, name)
