const assetsDir = __dirname + "/../NpcAdventure/assets"
const suffix = "json";
const supportedLocales = [
  "pt-BR",
  "fr-FR",
  "zh-CN",
];
const knownContents = [
  "Data/Events",
  "Data/Quests",
  "Dialogue/Abigail",
  "Dialogue/Alex",
  "Dialogue/Elliott",
  "Dialogue/Emily",
  "Dialogue/Haley",
  "Dialogue/Harvey",
  "Dialogue/Leah",
  "Dialogue/Maru",
  "Dialogue/Penny",
  "Dialogue/Sam",
  "Dialogue/Sebastian",
  "Dialogue/Shane",
  "Strings/Buffs",
  "Strings/Mail",
  "Strings/SpeechBubbles",
  "Strings/Strings",
];

function readOriginal(asset) {
  const fs = require("fs");
  const path = require("path");
  const file = fs.readFileSync(path.resolve(assetsDir, asset + ".json"));
  const json = JSON.parse(file.toString().trim());

  return json;
}

function seekForMissing(locale, asset) {
  const missing = [];
  const fs = require("fs");
  const path = require("path");
  const json = readOriginal(asset);
  const localizedFilePath = path.resolve(assetsDir, asset + `.${locale}.json`);

  if (!fs.existsSync(localizedFilePath)) {
    console.log(`W01 (untranslated-asset): Asset '${asset}' is not translated into '${locale}'`);
    return missing.concat(Object.keys(json) || []);
  }

  const localizedFile = fs.readFileSync(path.resolve(assetsDir, asset + `.${locale}.json`));
  const localizedJson = JSON.parse(localizedFile.toString().trim());

  for (let key of Object.keys(json)) {
    if (!Object.keys(localizedJson).includes(key)) {
      console.log(`W02 (untranslated-key): Missing '${locale}' localization for key '${key}' in asset '${asset}'`);
      missing.push(key);
    }
  }

  return missing;
}

function coverage(asset, misingKeysCount) {
  const keyCount = Object.keys(readOriginal(asset)).length;

  return {
    total: keyCount,
    covered: keyCount - misingKeysCount,
  };
}

function walk(locale, contents) {
  const report = [];
  for (let asset of contents) {
    try {
      const missing = seekForMissing(locale, asset);
      report.push({asset, missing, coverage: coverage(asset, missing.length)})
    } catch (error) {
      report.push({asset, missing: [], error: error.message})
    }
  }

  return report;
}

function generateStats(fullReport) {
  const stats = [];

  for (let report of fullReport.reports) {
    const total = report.analysis.reduce((ac, cur) => ac + cur.coverage.total, 0);
    const covered = report.analysis.reduce((ac, cur) => ac + cur.coverage.covered, 0)

    stats.push({
      locale: report.locale,
      coverage: {
        total,
        covered,
        percentage: covered / total
      }
    });
  }

  return stats;
}

function analyze() {
  const report = {
    date: new Date(),
    title: "NPC Advenures localization coverage report",
    reports: [],
    stats: null,
  };

  console.log("Analyzing assets...");

  for (let locale of supportedLocales) {
    report.reports.push({locale, analysis: walk(locale, knownContents)});
  }

  console.log("Generating stats...");
  report.stats = generateStats(report);

  return report;
}

const analysis = analyze();

for (let stat of analysis.stats) {
  console.log(`Locale: ${stat.locale} - Covered ${stat.coverage.covered} entries of ${stat.coverage.total} (${Number(stat.coverage.percentage * 100).toFixed(2)}%)`);
}

require("fs").writeFileSync("report.json", JSON.stringify(analysis));
console.log("Report written to report.json");

