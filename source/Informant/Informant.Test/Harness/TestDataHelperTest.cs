/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System;
using NUnit.Framework;

namespace StardewTests.Harness; 

[TestFixture]
public class TestDataHelperTest {

    private record TestConfig {
        public string? Value { get; init; }
    }
    
    private TestDataHelper _classUnderTest = new();

    [SetUp]
    public void SetUp() {
        _classUnderTest = new();
    }

    [Test]
    public void ClearJsonFiles() {
        _classUnderTest.WriteJsonFile(Guid.NewGuid().ToString(), new TestConfig {
            Value = Guid.NewGuid().ToString(),
        });
        _classUnderTest.ClearJsonFiles();

        ReadJsonFile_Null();
    }
    
    [Test]
    public void ReadJsonFile_Null() {
        var key = Guid.NewGuid().ToString();
        var config = _classUnderTest.ReadJsonFile<TestConfig>(key);
        Assert.IsNull(config);
    }
    
    [Test]
    public void ReadJsonFile_Multiple() {
        var testKey = Guid.NewGuid().ToString();
        var testValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteJsonFile(testKey, new TestConfig {
            Value = testValue
        });
        var otherKey = Guid.NewGuid().ToString();
        var otherValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteJsonFile(otherKey,new TestConfig {
            Value = otherValue
        });
        
        var testConfig = _classUnderTest.ReadJsonFile<TestConfig>(testKey);
        Assert.NotNull(testConfig);
        Assert.AreEqual(testValue, testConfig!.Value);
        
        var otherConfig = _classUnderTest.ReadJsonFile<TestConfig>(otherKey);
        Assert.NotNull(otherConfig);
        Assert.AreEqual(otherValue, otherConfig!.Value);
    }
    
    [Test]
    public void WriteJsonFile_Null() {
        var testKey = Guid.NewGuid().ToString();
        var testValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteJsonFile(testKey, new TestConfig {
            Value = testValue
        });
        var otherKey = Guid.NewGuid().ToString();
        var otherValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteJsonFile(otherKey,new TestConfig {
            Value = otherValue
        });
        
        _classUnderTest.WriteJsonFile(testKey, (TestConfig?) null);
        
        var testConfig = _classUnderTest.ReadJsonFile<TestConfig>(testKey);
        Assert.IsNull(testConfig);
        
        var otherConfig = _classUnderTest.ReadJsonFile<TestConfig>(otherKey);
        Assert.NotNull(otherConfig);
        Assert.AreEqual(otherValue, otherConfig!.Value);
    }
    
    [Test]
    public void WriteJsonFile() {
        var key = Guid.NewGuid().ToString();
        var value = Guid.NewGuid().ToString();
        _classUnderTest.WriteJsonFile(key, new TestConfig {
            Value = value
        });
        
        var config = _classUnderTest.ReadJsonFile<TestConfig>(key);
        Assert.NotNull(config);
        Assert.AreEqual(value, config!.Value);
    }

    [Test]
    public void ClearSaveDatas() {
        _classUnderTest.WriteSaveData(Guid.NewGuid().ToString(), new TestConfig {
            Value = Guid.NewGuid().ToString(),
        });
        _classUnderTest.ClearSaveDatas();

        ReadSaveData_Null();
    }
    
    [Test]
    public void ReadSaveData_Null() {
        var key = Guid.NewGuid().ToString();
        var config = _classUnderTest.ReadSaveData<TestConfig>(key);
        Assert.IsNull(config);
    }
    
    [Test]
    public void ReadSaveData_Multiple() {
        var testKey = Guid.NewGuid().ToString();
        var testValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteSaveData(testKey, new TestConfig {
            Value = testValue
        });
        var otherKey = Guid.NewGuid().ToString();
        var otherValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteSaveData(otherKey,new TestConfig {
            Value = otherValue
        });
        
        var testConfig = _classUnderTest.ReadSaveData<TestConfig>(testKey);
        Assert.NotNull(testConfig);
        Assert.AreEqual(testValue, testConfig!.Value);
        
        var otherConfig = _classUnderTest.ReadSaveData<TestConfig>(otherKey);
        Assert.NotNull(otherConfig);
        Assert.AreEqual(otherValue, otherConfig!.Value);
    }
    
    [Test]
    public void WriteSaveData_Null() {
        var testKey = Guid.NewGuid().ToString();
        var testValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteSaveData(testKey, new TestConfig {
            Value = testValue
        });
        var otherKey = Guid.NewGuid().ToString();
        var otherValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteSaveData(otherKey,new TestConfig {
            Value = otherValue
        });
        
        _classUnderTest.WriteSaveData(testKey, (TestConfig?) null);
        
        var testConfig = _classUnderTest.ReadSaveData<TestConfig>(testKey);
        Assert.IsNull(testConfig);
        
        var otherConfig = _classUnderTest.ReadSaveData<TestConfig>(otherKey);
        Assert.NotNull(otherConfig);
        Assert.AreEqual(otherValue, otherConfig!.Value);
    }
    
    [Test]
    public void WriteSaveData() {
        var key = Guid.NewGuid().ToString();
        var value = Guid.NewGuid().ToString();
        _classUnderTest.WriteSaveData(key, new TestConfig {
            Value = value
        });
        
        var config = _classUnderTest.ReadSaveData<TestConfig>(key);
        Assert.NotNull(config);
        Assert.AreEqual(value, config!.Value);
    }
    
    [Test]
    public void ClearGlobalDatas() {
        _classUnderTest.WriteGlobalData(Guid.NewGuid().ToString(), new TestConfig {
            Value = Guid.NewGuid().ToString(),
        });
        _classUnderTest.ClearGlobalDatas();

        ReadGlobalData_Null();
    }
    
    [Test]
    public void ReadGlobalData_Null() {
        var key = Guid.NewGuid().ToString();
        var config = _classUnderTest.ReadGlobalData<TestConfig>(key);
        Assert.IsNull(config);
    }
    
    [Test]
    public void ReadGlobalData_Multiple() {
        var testKey = Guid.NewGuid().ToString();
        var testValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteGlobalData(testKey, new TestConfig {
            Value = testValue
        });
        var otherKey = Guid.NewGuid().ToString();
        var otherValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteGlobalData(otherKey,new TestConfig {
            Value = otherValue
        });
        
        var testConfig = _classUnderTest.ReadGlobalData<TestConfig>(testKey);
        Assert.NotNull(testConfig);
        Assert.AreEqual(testValue, testConfig!.Value);
        
        var otherConfig = _classUnderTest.ReadGlobalData<TestConfig>(otherKey);
        Assert.NotNull(otherConfig);
        Assert.AreEqual(otherValue, otherConfig!.Value);
    }
    
    [Test]
    public void WriteGlobalData_Null() {
        var testKey = Guid.NewGuid().ToString();
        var testValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteGlobalData(testKey, new TestConfig {
            Value = testValue
        });
        var otherKey = Guid.NewGuid().ToString();
        var otherValue = Guid.NewGuid().ToString();
        _classUnderTest.WriteGlobalData(otherKey,new TestConfig {
            Value = otherValue
        });
        
        _classUnderTest.WriteGlobalData(testKey, (TestConfig?) null);
        
        var testConfig = _classUnderTest.ReadGlobalData<TestConfig>(testKey);
        Assert.IsNull(testConfig);
        
        var otherConfig = _classUnderTest.ReadGlobalData<TestConfig>(otherKey);
        Assert.NotNull(otherConfig);
        Assert.AreEqual(otherValue, otherConfig!.Value);
    }
    
    [Test]
    public void WriteGlobalData() {
        var key = Guid.NewGuid().ToString();
        var value = Guid.NewGuid().ToString();
        _classUnderTest.WriteGlobalData(key, new TestConfig {
            Value = value
        });
        
        var config = _classUnderTest.ReadGlobalData<TestConfig>(key);
        Assert.NotNull(config);
        Assert.AreEqual(value, config!.Value);
    }
}