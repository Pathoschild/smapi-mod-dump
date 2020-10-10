/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Events;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services.Internal;
using Igorious.StardewValley.DynamicAPI.Utils;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class ClassMapperService
    {
        #region Constants

        private static readonly EventInfo TimeOfDayChangedEventInfo = new EventInfo(typeof(TimeEvents), nameof(TimeEvents.TimeOfDayChanged));

        #endregion

        #region Singleton Access

        private ClassMapperService()
        {
            PlayerEvents.LoadedGame += (s, e) =>
            {
                if (!ActivateMapping()) return;
                Log.ImportantInfo("Activation after loading...");
                ConvertToSmartInWorld();
            };

            TimeEvents.TimeOfDayChanged += (s, e) =>
            {
                if (Game1.timeOfDay != 610) return;
                if (!ActivateMapping()) return;
                Log.ImportantInfo("FORCED ACTIVATION!");
                ConvertToSmartInWorld();
            };

            SavingEvents.BeforeSaving += () =>
            {
                if (!DeactivateMapping()) return;
                Log.ImportantInfo("Deactivation before saving...");
                DelayTimeOfDayChanged();
                ConvertToRawInWorld(); // Allow use native serializer.
            };

            SavingEvents.AfterSaving += () =>
            {
                if (!ActivateMapping()) return;
                Log.ImportantInfo("Activation after saving...");
                ConvertToSmartInWorld();
                RestoreTimeOfDayChanged();
            };

            InventoryEvents.ActiveObjectChanged += OnObjectChanged;
            InventoryEvents.CraftedObjectChanged += OnObjectChanged;
            PlayerEvents.InventoryChanged += OnInventoryChanged;
        }

        private void OnInventoryChanged(object s, EventArgsInventoryChanged e)
        {
            if (!IsActivated || !e.Added.Any()) return;

            var sw = Stopwatch.StartNew();
            var inventory = Game1.player.Items;
            foreach (var addedItemInfo in e.Added)
            {
                var addedItem = addedItemInfo.Item as Object;
                if (!IsRawObject(addedItem)) continue;

                var index = inventory.FindIndex(i => i == addedItem);
                inventory[index] = ToSmartObject(addedItem);
            }
            Log.InfoAsync($"Convertion ('smart') in inventory finished: {sw.ElapsedMilliseconds} ms");
        }

        private static ClassMapperService _instance;

        public static ClassMapperService Instance => _instance ?? (_instance = new ClassMapperService());

        #endregion

        #region Private Data

        private bool IsActivated { get; set; }

        private readonly Dictionary<int, Type> _itemTypeMap = new Dictionary<int, Type>();
        private readonly Dictionary<int, Type> _craftableTypeMap = new Dictionary<int, Type>();
        private readonly Dictionary<int, DynamicTypeInfo> _dynamicCraftableTypeMap = new Dictionary<int, DynamicTypeInfo>();

        private readonly Dictionary<Type, ConstructorBase> _itemTypeCtorCache = new Dictionary<Type, ConstructorBase>();
        private readonly Dictionary<Type, ConstructorBase> _craftableTypeCtorCache = new Dictionary<Type, ConstructorBase>();
        private readonly Dictionary<Type, ConstructorBase> _dynamicCraftableCtorCache = new Dictionary<Type, ConstructorBase>();

        private IReadOnlyList<Delegate> DelayedTimeOfDayChangedHandlers { get; set; }

        #endregion

        #region	Properties

        public IReadOnlyDictionary<int, Type> ItemTypeMap => _itemTypeMap;
        public IReadOnlyDictionary<int, Type> CraftableTypeMap => _craftableTypeMap;
        public IReadOnlyDictionary<int, DynamicTypeInfo> DynamicCraftableTypeMap => _dynamicCraftableTypeMap;

        #endregion

        #region	Public Methods

        /// <summary>
        /// Map game Item ID to specific class.
        /// </summary>
        public void MapItem<TObject>(DynamicID<ItemID> id) where TObject : ISmartObject, new()
        {
            _itemTypeMap.Add(id, typeof(TObject));
            if (!_itemTypeCtorCache.ContainsKey(typeof(TObject)))
            {
                _itemTypeCtorCache.Add(typeof(TObject), new Constructor<TObject>());
            }
        }

        /// <summary>
        /// Map game Craftable ID to specific class.
        /// </summary>
        public void MapCraftable<TObject>(DynamicID<CraftableID> id) where TObject : ISmartObject, new()
        {
            _craftableTypeMap.Add(id, typeof(TObject));
            if (!_craftableTypeCtorCache.ContainsKey(typeof(TObject)))
            {
                _craftableTypeCtorCache.Add(typeof(TObject), new Constructor<TObject>());
            }
        }

        /// <summary>
        /// Map game Craftable ID to specific class.
        /// </summary>
        public void MapCraftable(DynamicTypeInfo dynamicTypeInfo)
        {
            _dynamicCraftableTypeMap.Add(dynamicTypeInfo.ClassID, dynamicTypeInfo);
            if (!_dynamicCraftableCtorCache.ContainsKey(dynamicTypeInfo.BaseType))
            {
                _dynamicCraftableCtorCache.Add(dynamicTypeInfo.BaseType, new Constructor<int, Object>(dynamicTypeInfo.BaseType));
            }
        }

        /// <summary>
        /// Get mapped Craftable ID for specific class.
        /// </summary>
        public int GetCraftableID<TObject>() where TObject : ISmartObject
        {
            var result = _craftableTypeMap.FirstOrDefault(kv => kv.Value == typeof(TObject));
            if (result.Value == null) Log.Error($"Type {typeof(TObject).Name} is not registered!");
            return result.Key;
        }

        /// <summary>
        /// Get mapped ID for specific class.
        /// </summary>
        public int GetItemID<TObject>() where TObject : ISmartObject
        {
            var result = _itemTypeMap.FirstOrDefault(kv => kv.Value == typeof(TObject));
            if (result.Value == null) Log.Error($"Type {typeof(TObject).Name} is not registered!");
            return result.Key;
        }

        public Object ToSmartObject(Object rawObject)
        {
            if (!IsRawObject(rawObject)) return rawObject;

            try
            {
                var smartObject = rawObject.bigCraftable
                    ? CraftableToSmartObject(rawObject)
                    : ObjectToSmartObject(rawObject);
                Cloner.Instance.CopyData(rawObject, smartObject);
                return smartObject;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                throw;
            }
        }

        #endregion

        #region	Auxiliary Methods

        #region Handlers

        private void OnObjectChanged(ObjectEventArgs args)
        {
            if (!IsActivated || !IsRawObject(args.Object)) return;
            args.Object = ToSmartObject(args.Object);
        }

        private void OnLocationObjectsChanged(object sender, EventArgsLocationObjectsChanged e)
        {
            if (!IsActivated) return;
            var sw = Stopwatch.StartNew();
            Traverser.Instance.TraverseLocationLight(Game1.currentLocation, ToSmartObject);
            Log.InfoAsync($"Activation of objects (cause: changes in {Game1.currentLocation?.Name}) finished: {sw.ElapsedMilliseconds} ms");
        }

        private void OnCurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
        {
            if (!IsActivated) return;
            var sw = Stopwatch.StartNew();
            Traverser.Instance.TraverseLocationDeep(Game1.currentLocation, ToSmartObject);
            Log.Info($"Activation of objects(cause: entered to {Game1.currentLocation?.Name}) finished: {sw.ElapsedMilliseconds} ms");
        }

        #endregion

        #region Conversion

        private Object CraftableToSmartObject(Object rawObject)
        {
            var craftableID = rawObject.ParentSheetIndex;

            Type type;
            if (_craftableTypeMap.TryGetValue(craftableID, out type))
            {
                var ctor = _craftableTypeCtorCache[type];
                return (Object)ctor.Invoke();
            }

            DynamicTypeInfo dynamicTypeInfo;
            if (_dynamicCraftableTypeMap.TryGetValue(craftableID, out dynamicTypeInfo))
            {
                var ctor = _dynamicCraftableCtorCache[dynamicTypeInfo.BaseType];
                return (Object)ctor.Invoke(dynamicTypeInfo.ClassID);
            }

            Log.Error($"Can't find mapping for CraftableID={craftableID}.");
            return rawObject;
        }

        private Object ObjectToSmartObject(Object rawObject)
        {
            var itemID = rawObject.ParentSheetIndex;
            var type = _itemTypeMap[itemID];
            var ctor = _itemTypeCtorCache[type];
            return (Object)ctor.Invoke();
        }

        public Object ToRawObject(Object smartObject)
        {
            if (!(smartObject is ISmartObject)) return smartObject;
            var rawObject = (smartObject is Chest) ? new Chest() : (smartObject.GetColor() != null) ? new ColoredObject() : new Object();
            Cloner.Instance.CopyData(smartObject, rawObject);
            return rawObject;
        }

        private void ConvertToRawInWorld()
        {
            var sw = Stopwatch.StartNew();
            Log.Info("Deactivating objects in world...");
            Traverser.Instance.TraverseLocations(l => Traverser.Instance.TraverseLocationDeep(l, ToRawObject));
            Traverser.Instance.TraverseInventory(Game1.player, ToRawObject);
            Log.Info($"Convertion ('raw') finished: {sw.ElapsedMilliseconds} ms");
        }

        private void ConvertToSmartInWorld()
        {
            var sw = Stopwatch.StartNew();
            Log.Info("Activating objects in world...");
            Traverser.Instance.TraverseLocations(l => Traverser.Instance.TraverseLocationDeep(l, ToSmartObject));
            Traverser.Instance.TraverseInventory(Game1.player, ToSmartObject);
            Log.Info($"Convertion ('smart') finished: {sw.ElapsedMilliseconds} ms");
        }

        #endregion

        #region Checks

        private bool IsRawObject(Object o)
        {
            return (o != null) && (IsRawCraftable(o) || IsRawItem(o));
        }

        private bool IsRawCraftable(Object o)
        {
            if (!o.bigCraftable) return false;
            var craftableID = o.ParentSheetIndex;

            Type type;
            if (_craftableTypeMap.TryGetValue(craftableID, out type))
            {
                return (o.GetType() != type);
            }

            DynamicTypeInfo dynamicTypeInfo;
            if (_dynamicCraftableTypeMap.TryGetValue(craftableID, out dynamicTypeInfo))
            {
                return (o.GetType() != dynamicTypeInfo.BaseType);
            }

            return false;
        }

        private bool IsRawItem(Object o)
        {
            if (o.bigCraftable) return false;
            var itemID = o.ParentSheetIndex;

            Type type;
            if (_itemTypeMap.TryGetValue(itemID, out type))
            {
                return (o.GetType() != type);
            }

            return false;
        }

        #endregion

        #region Activation

        public void ForceDeactivation()
        {
            DeactivateMapping();
            ConvertToRawInWorld();
        }

        private bool ActivateMapping()
        {
            if (IsActivated) return false;
            IsActivated = true;
            LocationEvents.CurrentLocationChanged += OnCurrentLocationChanged;
            LocationEvents.LocationObjectsChanged += OnLocationObjectsChanged;
            Log.ImportantInfo("Class mapping activated.");
            return true;
        }

        private bool DeactivateMapping()
        {
            if (!IsActivated) return false;
            IsActivated = false;
            LocationEvents.LocationObjectsChanged -= OnLocationObjectsChanged;
            LocationEvents.CurrentLocationChanged -= OnCurrentLocationChanged;
            Log.ImportantInfo("Class mapping deactivated.");
            return true;
        }

        #endregion

        #region Delay Events

        private void DelayTimeOfDayChanged()
        {
            DelayedTimeOfDayChangedHandlers = DelayEventHandlers(TimeOfDayChangedEventInfo, h => h.Method.Module.Name == "FarmAutomation.ItemCollector.dll");
        }

        private void RestoreTimeOfDayChanged()
        {
            RestoreEventHandlers(TimeOfDayChangedEventInfo, DelayedTimeOfDayChangedHandlers);
            foreach (var handler in DelayedTimeOfDayChangedHandlers)
            {
                try
                {
                    handler.Method.Invoke(handler.Target, new object[] { null, new EventArgsIntChanged(600, 600) });
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            }
            DelayedTimeOfDayChangedHandlers = null;
        }

        private static IReadOnlyList<Delegate> DelayEventHandlers(EventInfo eventInfo, Func<Delegate, bool> filter)
        {
            var handlers = eventInfo.Type.GetEventHandlers(eventInfo.EventName);
            var automationHandlers = handlers.Where(filter).ToList();
            foreach (var handler in automationHandlers)
            {
                eventInfo.Type.RemoveEventHandler(eventInfo.EventName, handler);
            }
            return automationHandlers;
        }

        private static void RestoreEventHandlers(EventInfo eventInfo, IReadOnlyList<Delegate> delayedHandlers)
        {
            foreach (var handler in delayedHandlers)
            {
                eventInfo.Type.AddEventHandler(eventInfo.EventName, handler);
            }
        }

        #endregion

        #endregion
    }
}