fileName = "fireworkScript"
fileIn = open(fileName + ".json","r",encoding="utf-8")
fileOut = open(fileName + "Parsed.txt","w",encoding="utf-8")

scriptLines = fileIn.readlines()

for i, line in enumerate(scriptLines):
	# Skip comments
	if line.startswith("//"):
		continue

	# Strip whitespace and remove double spaces
	line = ' '.join(line.split())

	# Strip leading and trailing quotation marks
	if line.startswith("\"") and line.endswith("\""):
		line = line[1:-1]

	# Warn and skip broken event lines
	if not line.endswith("/") and i != len(scriptLines) - 1:
		print(line)
		print("Warning: broken event line detected, must end in /")
		continue

	# Save
	fileOut.write(line)

fileIn.close()
fileOut.close()