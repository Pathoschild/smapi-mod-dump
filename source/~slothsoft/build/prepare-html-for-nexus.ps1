# -----------------------------------------------------------------------------------------------------------------
# The Nexus description is different from our normal README.md, so change the HTML before converting
# -----------------------------------------------------------------------------------------------------------------

$htmlFile=$args[0]
$htmlFileSnip=$args[1]

$html = [string]::Join("`n", (gc $htmlFile -encoding utf8)) # Note: all line separators are \n !

$deinstallationGuide = $html -replace "(?ms)<h1(.*)<h3 id=""removing",'<h3 id=""removing' -replace "(?ms)/ol>(.*)",'/ol>'

# Removes the header info of the readme file
$html = $html -replace "(?ms)</h1>(.*?)</ul>",'</h1>'
# Removes the TOC and installation and deinstallation guides
$html = $html -replace "<p><p>",'<p>'
$html = $html -replace "(?ms)<p><strong>Content(.*?)<h3 id=""using-the-mod(.*?)</h3>",''
# Removes images entirely
$html = $html -replace '<img(.*?)src="readme/(.*?)"(.*?)width="(.*?)"(.*?)>(\n)+','' 
# Removes anchor links
$html = $html -replace '<a href="#(.*?)">(.*?)</a>','$2'
# Link correct license file
$html = $html -replace 'href="LICENSE"','href="https://github.com/slothsoft/stardew-challenger/blob/main/LICENSE"'
# Link correct Challenger file
$html = $html -replace 'href="Challenger','href="https://github.com/slothsoft/stardew-challenger/blob/main/Challenger'
$html = $html -replace 'href="readme','href="https://github.com/slothsoft/stardew-challenger/blob/main/readme'
# Link the documentation for the config file
$html = $html -replace "(?ms)The <em>config.json</em> with all entries(.*?)</table>","The <em>config.json</em> with all entries is documented <a href=""https://github.com/slothsoft/stardew-challenger/blob/main/README.md#configuration"">here</a>.</p>"
# Remove the versions section
$html = $html -replace "(?ms)<h3 id=""versions(.*?)</p>(\n)+",''
# Replace the translation table with link
$html = $html -replace "(?ms)<h2 id=""translator-guide","$deinstallationGuide`n<h2 id=""translator-guide"
$html = $html -replace "(?ms)<table(.*?)German(.*?)</table>","<p>More information can be found <a href=""https://github.com/slothsoft/stardew-challenger/blob/main/README.md#translator-guide"">here</a>.</p>"
# Add link to GitHub
$html = $html -replace "dev-notes.md"">here</a>.</p>", "dev-notes.md"">here</a>.</p>`n<p>The source code for this mod is on <a href=""https://github.com/slothsoft/stardew-challenger"">GitHub</a>.</p>"

# Move headers one category up
$html = $html -replace '<([/])*h3','<$1h2'
$html = $html -replace '<([/])*h4','<$1h3'

# Make some changes to get better BBCode
$html = $html -replace "<h2","`n`n<h2"
$html = $html -replace "<h3","`n<h3"
$html = $html -replace "<p><p>",'<p>'

$html| Out-File -encoding utf8 $htmlFileSnip