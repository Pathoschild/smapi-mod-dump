param (
	[string]$DirectoryToZip,
	[string]$ZipName,
	[string]$ZipTarget
)

function ZipFiles( $zipfilename, $sourcedir )
{
	Add-Type -Assembly System.IO.Compression.FileSystem
	Write-Host "Creating zipfile $($zipfilename) of directory $($sourcedir)"
	$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
	[System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir, $zipfilename, $compressionLevel, $true)
}
$zippath = -join($ZipTarget, $ZipName, ".zip")

if (Test-Path $zippath)
{
	Remove-Item $zippath
}
if (!(Test-Path $ZipTarget))
{
	New-Item $ZipTarget -ItemType Directory
}

ZipFiles -zipfilename $zippath -sourcedir $DirectoryToZip






