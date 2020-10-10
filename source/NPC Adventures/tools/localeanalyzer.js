/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

const assetsDir = __dirname + "/../assets"
const officialLocaleDir = __dirname + "/../locale"
const unofficialLocaleDir = __dirname + "/../unofficial/locale";
const excludedLocaleDir = __dirname + "/../Contrib/Locale";
const suffix = "json";
const supportedLocales = [
  {code: "pt-BR", name: "Portuguese", dir: officialLocaleDir, official: true},
  {code: "es-ES", name: "Spanish", dir: officialLocaleDir, official: true},
  {code: "ko-KR", name: "Korean", dir: officialLocaleDir, official: true},
  {code: "ja-JP", name: "Japanese", dir: unofficialLocaleDir, official: false},
  {code: "ru-RU", name: "Russian", dir: unofficialLocaleDir, official: false},
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
  const file = fs.readFileSync(path.resolve(assetsDir, asset + `.${suffix}`));
  const json = JSON.parse(file.toString().trim().replace(/\s/g, ''));

  return json;
}

function seekForMissing(locale, asset) {
  const missing = [];
  const fs = require("fs");
  const path = require("path");
  const json = readOriginal(asset);
  const localizedFilePath = path.resolve(locale.dir, locale.code.toLowerCase(), `${asset}.${suffix}`);

  if (!fs.existsSync(localizedFilePath)) {
    console.log(`W01 (untranslated-asset): Asset '${asset}' is not translated into '${locale.code}'`);
    return missing.concat(Object.keys(json) || []);
  }

  const localizedFile = fs.readFileSync(localizedFilePath);
  const localizedJson = JSON.parse(localizedFile.toString().trim().replace(/\s/g, ''));

  for (let key of Object.keys(json)) {
    if (!Object.keys(localizedJson).includes(key)) {
      console.log(`W02 (untranslated-key): Missing '${locale.code}' localization for key '${key}' in asset '${asset}'`);
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
      try {
        const json = readOriginal(asset);
        const missing = Object.keys(json) || [];

        report.push({ asset, missing: [], error: error.message, coverage: coverage(asset, missing.length) });
        console.log(`E01 (general-error): An error occured while analysing locale '${locale.code}' asset '${asset}' - ${error.message}`);
      } catch (error) {
        console.error(`Can't analyze covarage for original asset '${asset}, because is broken:`);
        console.error(`    ${error.message}`);
        console.error(`Coverage report generator aborted.`);
        process.exit(4);
      }
    }
  }

  return report;
}

function generateStats(fullReport) {
  const stats = [];

  for (let report of fullReport.reports) {
    //console.log(report.analysis);

    const total = report.analysis.reduce((ac, cur) => ac + cur.coverage.total || 0, 0);
    const covered = report.analysis.reduce((ac, cur) => ac + cur.coverage.covered || 0, 0)

    stats.push({
      locale: report.locale,
      label: report.label,
      official: report.official,
      failed: !!report.analysis.find(cur => cur.error),
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
    title: "NPC Adventures localization coverage report",
    reports: [],
    stats: null,
  };

  console.log("Analyzing assets...");

  for (let locale of supportedLocales) {
    report.reports.push({
      locale: locale.code, 
      label: locale.name, 
      official: locale.official, 
      analysis: walk(locale, knownContents)
    });
  }

  console.log("Generating stats...");
  report.stats = generateStats(report);

  return report;
}

function mark(stat) {
  if (stat.failed) {
    return "\x1b[1m\x1b[31mF\x1b[0m";
  }

  if (stat.coverage.percentage <= 0.5) {
    return "\x1b[1m\x1b[41m!\x1b[0m"
  }

  if (stat.coverage.percentage < 0.8) {
    return "\x1b[1m\x1b[33m!\x1b[0m"
  }

  return " ";
}

const analysis = analyze();

for (let stat of analysis.stats) {
  console.log(` ${mark(stat)} Locale: ${!stat.official ? "~" : ""}${stat.label} (${stat.locale}) - Covered ${stat.coverage.covered} entries of ${stat.coverage.total} (${Number(stat.coverage.percentage * 100).toFixed(2)}%)`);
}

const reportFile = process.argv[2] || "report.json";
require("fs").writeFileSync(reportFile, JSON.stringify(analysis));
console.log("\n ~  unofficial/community localization");
console.log(`Report written to ${reportFile}`);
