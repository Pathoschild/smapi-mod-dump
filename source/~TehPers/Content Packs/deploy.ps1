<# Deploys a content pack folder to the mods directory. #>
param(
    # The content pack folder
    [Parameter(Mandatory=$true)]
    [System.IO.DirectoryInfo] $srcDir,
    # The mods directory
    [Parameter(Mandatory=$true)]
    [System.IO.DirectoryInfo] $modsDir
)

# Copy the folder
$outDirName = $srcDir.Name
Copy-Item -LiteralPath $srcDir.FullName $modsDir -Force -Recurse -Verbose
