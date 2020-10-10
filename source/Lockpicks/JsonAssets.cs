/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bwdymods/SDV-Lockpicks
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Lockpicks
{
    class JsonAssets
    {
        Assembly JA = null;
        Type JaMod = null;
        object JaModInstance = null;
        MethodInfo RegisterObjectMethod = null;
        Type ObjectDataType = null;
        StardewModdingAPI.Mod Mod = null;
        public bool IsHappy = false;

        public JsonAssets(StardewModdingAPI.Mod mod)
        {
            Mod = mod;
            JA = GetAssemblyByName("JsonAssets");
            if (JA == null)
            {
                Mod.Monitor.Log("Could not locate the JsonAssets assembly, mod will not be functional.", LogLevel.Warn);
                return;
            }
            JaMod = JA.GetType("JsonAssets.Mod");
            if (JaMod == null)
            {
                Mod.Monitor.Log("Could not locate the JsonAssets mod class, mod will not be functional.", LogLevel.Warn);
                return;
            }
            var i = JaMod.GetField("instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if(i == null)
            {
                Mod.Monitor.Log("Could not locate the JsonAssets mod instance field, mod will not be functional.", LogLevel.Warn);
                return;
            }
            JaModInstance = i.GetValue(null);
            if (JaModInstance == null)
            {
                Mod.Monitor.Log("Could not locate the JsonAssets mod instance, mod will not be functional.", LogLevel.Warn);
                return;
            }
            RegisterObjectMethod = JaMod.GetMethod("RegisterObject", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (RegisterObjectMethod == null)
            {
                Mod.Monitor.Log("Could not locate the JsonAssets RegisterObject function, mod will not be functional.", LogLevel.Warn);
                return;
            }
            ObjectDataType = JA.GetType("JsonAssets.Data.ObjectData");
            if (ObjectDataType == null)
            {
                Mod.Monitor.Log("Could not locate the JsonAssets ObjectData class, mod will not be functional.", LogLevel.Warn);
                return;
            }
            IsHappy = true;
        }
        
        public int? GetObjectId(string name)
        {
            return Mod.Helper.ModRegistry.GetApi<IApi>("spacechase0.JsonAssets").GetObjectId(name);
        }

        public void RegisterObject(string name, string desc, string texture, string purchaseFrom, int category, int price, int purchasePrice)
        {
            if (!IsHappy) return;
            int stage = 0;
            try
            {
                var obj = Activator.CreateInstance(ObjectDataType);
                stage = 1;
                SetVariable(ObjectDataType, obj, "texture", Mod.Helper.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(texture, ContentSource.ModFolder));
                stage = 2;
                if (purchaseFrom != null)
                {
                    SetVariable(ObjectDataType, obj, "PurchaseFrom", purchaseFrom);
                    SetVariable(ObjectDataType, obj, "CanPurchase", true);
                }
                stage = 3;
                SetVariable(ObjectDataType, obj, "Category", category);
                stage = 4;
                SetVariable(ObjectDataType, obj, "Price", price);
                stage = 5;
                SetVariable(ObjectDataType, obj, "PurchasePrice", purchasePrice);
                stage = 6;
                SetVariable(ObjectDataType, obj, "Name", name);
                stage = 7;
                SetVariable(ObjectDataType, obj, "Description", desc);
                stage = 8;

                RegisterObjectMethod.Invoke(JaModInstance, new object[] { Mod.ModManifest, Convert.ChangeType(obj, ObjectDataType) });

                stage = 9;
            }
            catch (Exception e)
            {
                Mod.Monitor.Log("JsonAssets RegisterObject has failed at stage " + stage + ": " + e, LogLevel.Error);
            }
        }

        public void SetVariable(Type t, object obj, string field, object value)
        {
            //try a field first
            var a1 = t.GetField(field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if(a1 != null)
            {
                a1.SetValue(obj, value);
                return;
            }
            //otherwise try a property
            var a2 = t.GetProperty(field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if(a2 != null)
            {
                a2.SetValue(obj, value);
                return;
            }
            //welp guess it didn't work
            Mod.Monitor.Log("Failed to set ObjectData field: " + field, LogLevel.Error);
            throw new Exception("Failed to set ObjectData field: " + field);
        }

        Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

    }

    public interface IApi
    {
        int GetObjectId(string name);
        int GetCropId(string name);
        int GetFruitTreeId(string name);
        int GetBigCraftableId(string name);
        int GetHatId(string name);
        int GetWeaponId(string name);
        int GetClothingId(string name);
    }
}
