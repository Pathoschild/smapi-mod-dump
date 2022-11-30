/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System.Collections.Generic;

namespace StardewTests.Harness; 

public class TestDataHelper : IDataHelper {

    private readonly Dictionary<string, dynamic> _jsonFiles = new();
    private readonly Dictionary<string, dynamic> _saveDatas = new();
    private readonly Dictionary<string, dynamic> _globalDatas = new();
    
    public void ClearJsonFiles() {
        _jsonFiles.Clear();
    }
    
    public TModel? ReadJsonFile<TModel>(string path) where TModel : class {
        return (TModel?) _jsonFiles.GetValueOrDefault(path);
    }

    public void WriteJsonFile<TModel>(string path, TModel? data) where TModel : class {
        if (data == null) {
            _jsonFiles.Remove(path);
        } else {
            _jsonFiles[path] = data;
        }
    }

    public void ClearSaveDatas() {
        _saveDatas.Clear();
    }
    
    public TModel? ReadSaveData<TModel>(string path) where TModel : class {
        return (TModel?) _saveDatas.GetValueOrDefault(path);
    }

    public void WriteSaveData<TModel>(string path, TModel? data) where TModel : class {
        if (data == null) {
            _saveDatas.Remove(path);
        } else {
            _saveDatas[path] = data;
        }
    }
    
    public void ClearGlobalDatas() {
        _globalDatas.Clear();
    }
    
    public TModel? ReadGlobalData<TModel>(string path) where TModel : class {
        return (TModel?) _globalDatas.GetValueOrDefault(path);
    }

    public void WriteGlobalData<TModel>(string path, TModel? data) where TModel : class {
        if (data == null) {
            _globalDatas.Remove(path);
        } else {
            _globalDatas[path] = data;
        }
    }
}