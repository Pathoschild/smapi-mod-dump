param (
[string]$ModFolder,
[string]$BuildOutput
)

if ((Test-Path $ModFolder))
{
	Remove-Item $ModFolder\* -Recurse
} 
else
{
	New-Item $ModFolder -ItemType Directory
}
$files = -join($BuildOutput, "\*")
#copy $files $modfolder

$exclude = @('*.pdb','Lidgren.Network.dll', 'Newtonsoft.Json.dll', 'xTile.dll', 'Steamworks.NET.dll')
Copy-Item $files $ModFolder -Recurse -Force -Exclude $exclude