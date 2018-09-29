call %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe "C:\Users\owner\Documents\Visual Studio 2015\Projects\github\StardewValleyMods\Revitalize\Revitalize\Revitalize.sln"
call xnb_node.cmd pack RevitalizeProjectDecompiled RevitalizeProjectCompiled
xcopy /e /v /y RevitalizeProjectCompiled "C:\Users\owner\Documents\Visual Studio 2015\Projects\github\StardewValleyMods\Revitalize\RevitalizeProjectCompiled\"
xcopy /e /v /y RevitalizeProjectDecompiled "C:\Users\owner\Documents\Visual Studio 2015\Projects\github\StardewValleyMods\Revitalize\RevitalizeProjectDecompiled\"
xcopy /e /v /y RevitalizeProjectCompiled "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Content"
SET COPYCMD=/Y
copy "C:\Users\owner\Desktop\games\Stardew_stuff\XNB\UpdateAndRunStardew.bat" "C:\Users\owner\Documents\Visual Studio 2015\Projects\github\StardewValleyMods\Revitalize\UpdateAndRunStardew.bat"
cd "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\"
start "StardewModdingAPI" "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\StardewModdingAPI.exe"