/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

require('child_process').exec("git describe --tags --long HEAD", function (err, stdout) {
    let [tag, commits, hash] = stdout.split('-');

    const fs = require('fs');

    let rawdata = fs.readFileSync('manifest.json');
    let manifest = JSON.parse(rawdata.toString().trim());
    let oldVersion = manifest['Version'];
    let [base] = oldVersion.split('+');

    let newVersion = `${base}+nightbuild.${commits || 0}.${hash}`.replace("\n", "");

    console.log("Version in origin manifest:", oldVersion);
    console.log("Version described by git:", stdout);
    console.log("Possible nightbuild version:", newVersion);

    if (commits > 0) {
        console.log(`Head is ${commits} commit(s) ahead of the last tag ${tag}.`);
        console.log("Updating version to nightbuild...");
        manifest['Version'] = newVersion;
    } else {
        console.log(`No update version. Head is a last tag ${tag}`);
    }

    fs.writeFileSync('manifest.json', JSON.stringify(manifest, null, 2));
    console.log("Done!");
});
