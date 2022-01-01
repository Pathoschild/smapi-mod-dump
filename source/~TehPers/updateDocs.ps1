# Update schemas
Remove-Item -Path ./docs/**/schemas -Recurse -Force
dotnet run --project './src/TehPers.SchemaGen/TehPers.SchemaGen.csproj' -- ./docs
