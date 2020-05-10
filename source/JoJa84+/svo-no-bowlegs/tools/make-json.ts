import fs from 'fs';
import path from 'path';

interface Json extends Object {
	[key: string]: any;
}

process.argv.forEach((arg: string, i: number): void => {
	if (i >= 2) {
		const parsedPath: path.ParsedPath = path.parse(arg);
		const jsonStr: string = ((): string => {
			if (parsedPath.ext === ".jsonc") {
				const jsoncStr: string = fs.readFileSync(arg, "utf8");
				let toReturn: string = "";
				jsoncStr.split("\n").forEach((line: string): void => {
					toReturn += `\n${line.replace(/(?<!:)\/\/.+$/gm, "")}`;
				});
				return toReturn;
			} else {
				return fs.readFileSync(arg, "utf8");
			}
		})();
		const jsonObj: Json = JSON.parse(jsonStr);
		if (jsonObj["$schema"] != null) {
			delete jsonObj["$schema"];
		}
		fs.writeFileSync(path.join("bin/", `${parsedPath.name}.json`), JSON.stringify(jsonObj));
	}
});
