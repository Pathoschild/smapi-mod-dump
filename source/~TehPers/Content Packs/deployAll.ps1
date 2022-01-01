<# Deploys al content pack folders to the mods directory. #>
param(
    # The mods directory
    [Parameter(Mandatory=$true)]
    [System.IO.DirectoryInfo] $modsDir
)

$items = Get-ChildItem -Path . -Directory
foreach ($item in $items) {
    Write-Host "Deploying $item"
    ./deploy.ps1 -srcDir $item -modsDir $modsDir
}
