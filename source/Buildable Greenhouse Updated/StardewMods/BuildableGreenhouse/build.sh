python=`cat <<HEREDOC
import markdown
import re

file = markdown.markdown(open("README.md", 'r', encoding='utf-8-sig').read())

mainStack = []
stackTag = []

tag = ''
text = ''

ifTag = False

for char in file:
    if(char == '<'):
        stackTag.append(char)
        ifTag = True
        if(text != ''):
            mainStack.append(text)
            text = ''
    elif(char == '>'):
        stackTag.append(char)
        while(len(stackTag) > 0):
            tag += stackTag.pop()
        tag = tag[::-1] 
        mainStack.append(tag)    
        tag = ''
        ifTag = False
    else:
        if(ifTag):
            stackTag.append(char)
        else:
            text += char

nexus = open("bbcode.txt", 'w');

for token in mainStack:
    tagPattern = re.compile('(?<=<)(.*?)(?=>)')
    mo = tagPattern.search(token)
    if(mo != None):
        tag = mo.group()
        if(tag == 'h1'):
            nexus.write('[size=6]')
        elif(tag == '/h1' or tag == '/h4'):
            nexus.write('[/size]\n')
        elif(tag == 'h4'):
            nexus.write('[size=4]')
        elif(tag == 'p' or tag == '/li' or tag == 'pre' or tag == '/pre'):
            nexus.write('')
        elif(tag == '/p' or tag == 'br'):
            nexus.write('\n')
        elif(tag == 'li'):
            nexus.write('[*]')
        elif(tag == 'ul'):
            nexus.write('[list]')
        elif(tag == '/ul'):
            nexus.write('[/list]')
        elif(tag == 'code'):
            nexus.write('[code]')
        elif(tag == '/code'):
            nexus.write('[/code]')
        elif(tag == '/a'):
            nexus.write('[/url]')
        elif 'href' in tag:
            hrefPattern = re.compile(r'"(.*?)"')
            mo = hrefPattern.search(tag)
            nexus.write(f'[url={mo.group()}]')
        else:
            print("Warning {} is missing translation".format(tag))
    else:
        nexus.write(token)
        
nexus.close()
HEREDOC
`

python3 -c "$python"