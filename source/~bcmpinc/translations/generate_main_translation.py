import os
import glob
import re

VERSION = "7.1"

out = open("default.json", "w", encoding="UTF-8")

print("{", file=out)
for f in glob.glob("../*/i18n/default.json"):
    name = os.path.basename(os.path.dirname(os.path.dirname(f)))
    print("Gathering", name)
    with open(f, encoding="UTF-8-sig") as file:
        text = file.read().strip()
        print('  ',name,": ",text[:-1],"  },", sep='', file=out)
print("}", file=out)
out.close()
