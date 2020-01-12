require('child_process').exec("git describe --tags --long HEAD", function (err, stdout) {
    let [tag, commits, hash] = stdout.split('-');

    const fs = require('fs');

    let rawdata = fs.readFileSync('manifest.json');
    let manifest = JSON.parse(rawdata.toString().trim());
    let oldVersion = manifest['Version'];
    let [base, suffix] = oldVersion.split('-');
    let [major, minor] = base.split('.');

    if (!suffix) {
        ++minor;
    }

    let newVersion = `${major}.${minor}.0-nightbuild.${commits || 0}.${hash}`.replace("\n", "");

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
