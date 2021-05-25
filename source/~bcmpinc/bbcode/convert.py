import os
import glob
import re

files = glob.glob("../*/README.md")

for f in files:
    name = os.path.basename(os.path.dirname(f))
    print("Converting", name)
    with open(f) as file:
        text = file.read()

    # Convert links
    text = re.sub("\[(.+?)\]\((.+?)\)", "[url=\\2]\\1[/url]", text)

    # Convert headers
    text = re.sub("^#### (.+)", "[i]\\1[/i]", text, flags=re.MULTILINE)
    text = re.sub("^## (.+)", "[size=4][b]\\1[/b][/size]", text, flags=re.MULTILINE)
    text = re.sub("^# (.+)", "", text, flags=re.MULTILINE).strip()

    # Convert markup
    text = re.sub("[*][*](.+?)[*][*]", "[b]\\1[/b]", text)
    text = re.sub("[*](.+?)[*]", "[i]\\1[/i]", text)
    text = re.sub("[`](.+?)[`]", "[code]\\1[/code]", text)

    # Convert lists
    text = re.sub("^[*](.+)", "[list]\n[*]\\1\n[/list]", text, flags=re.MULTILINE)
    text = re.sub("\[/list\]\n\[list\]\n", "", text)

    # Remove extraneous line breaks
    text = re.sub("\n+\[list\]", "[list]", text)
    text = re.sub("\[/list\]\n+", "[/list]", text)

    with open(name+".txt", "w") as file:
        file.write(text)
        
