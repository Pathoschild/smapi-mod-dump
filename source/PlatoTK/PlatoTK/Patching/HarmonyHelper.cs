/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatoTK.Network;
using StardewValley;

namespace PlatoTK.Patching
{
    internal class HarmonyHelper : InnerHelper, IHarmonyHelper
    {
        public HarmonyInstance HarmonyInstance { get; }
        private static HashSet<MethodInfo> TracedMethods = new HashSet<MethodInfo>();
        internal static HashSet<TracedObject> TracedObjects;
        internal static HashSet<TypeForwarding> TracedTypes = new HashSet<TypeForwarding>();
        internal static HashSet<TypeForwarding> LinkedConstructors = new HashSet<TypeForwarding>();

        public HarmonyHelper(IPlatoHelper helper)
            : base(helper)
        {

            if (TracedObjects == null)
            {
                TracedObjects = new HashSet<TracedObject>();
                Plato.ModHelper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
            }

            if (HarmonyInstance == null)
                HarmonyInstance = HarmonyInstance.Create($"Plato.HarmonyHelper.{Plato.ModHelper.ModRegistry.ModID}");

        }

        public Texture2D GetDrawHandle(string id, Func<ITextureDrawHandler, bool> handler, Texture2D texture)
        {
            SpriteBatchPatches.InitializePatch();
            return new PlatoTexture(id, handler, texture, texture?.GraphicsDevice);
        }

        public Texture2D GetDrawHandle(string id, Func<ITextureDrawHandler, bool> handler, int width, int height, GraphicsDevice graphicsDevice = null)
        {
            SpriteBatchPatches.InitializePatch();
            return new PlatoTexture(id, handler, width, height, graphicsDevice ?? Game1.graphics.GraphicsDevice);
        }

        public Texture2D GetDrawHandle<TData>(string id, TData data, Func<ITextureDrawHandler<TData>, bool> handler, Texture2D texture)
        {
            SpriteBatchPatches.InitializePatch();
            return new PlatoTexture<TData>(id, data, handler, texture, texture?.GraphicsDevice);
        }

        public Texture2D GetDrawHandle<TData>(string id, TData data, Func<ITextureDrawHandler, bool> handler, int width, int height, GraphicsDevice graphicsDevice = null)
        {
            SpriteBatchPatches.InitializePatch();
            return new PlatoTexture<TData>(id, data, handler, width, height, graphicsDevice ?? Game1.graphics.GraphicsDevice);
        }

        public void PatchTileDraw(string id, Texture2D targetTexture, Func<Texture2D, bool> predicate, Texture2D patch, Rectangle? sourceRectangle, int index, int tileWidth = 16, int tileHeight = 16)
        {
            id = $"{Plato.ModHelper.ModRegistry.ModID}.{id}";
            SpriteBatchPatches.DrawPatches.RemoveWhere(p => p.Id == id);
            PatchAreaDraw(id, targetTexture, predicate, patch, sourceRectangle, Game1.getSourceRectForStandardTileSheet(targetTexture, index, tileWidth, tileHeight));
        }

        public void PatchAreaDraw(string id, Texture2D targetTexture, Func<Texture2D,bool> predicate, Texture2D patch, Rectangle? sourceRectangle, Rectangle? targetTileArea)
        {
            id = $"{Plato.ModHelper.ModRegistry.ModID}.{id}";
            SpriteBatchPatches.DrawPatches.RemoveWhere(p => p.Id == id);
            SpriteBatchPatches.DrawPatches.Add(new AreaDrawPatch(id, targetTexture, predicate, patch, targetTileArea.HasValue ? () => targetTileArea.Value : (Func<Rectangle>)null, sourceRectangle));
            SpriteBatchPatches.InitializePatch();
        }

        public void PatchTileDraw(string id, Texture2D targetTexture, Func<Texture2D,bool> predicate, Texture2D patch, Rectangle? sourceRectangle, Func<int> getIndex, int tileWidth = 16, int tileHeight = 16)
        {
            id = $"{Plato.ModHelper.ModRegistry.ModID}.{id}";
            SpriteBatchPatches.DrawPatches.RemoveWhere(p => p.Id == id);
            SpriteBatchPatches.DrawPatches.Add(new AreaDrawPatch(id, targetTexture, predicate, patch, () => Game1.getSourceRectForStandardTileSheet(targetTexture, getIndex(), tileWidth, tileHeight), sourceRectangle));
            SpriteBatchPatches.InitializePatch();
        }

        public void PatchAreaDraw(string id, Texture2D targetTexture, Func<Texture2D, bool> predicate, Texture2D patch, Rectangle? sourceRectangle, Func<Rectangle> getTargetTileArea)
        {
            id = $"{Plato.ModHelper.ModRegistry.ModID}.{id}";
            SpriteBatchPatches.DrawPatches.RemoveWhere(p => p.Id == id);
            SpriteBatchPatches.DrawPatches.Add(new AreaDrawPatch(id, targetTexture, predicate, patch, getTargetTileArea, sourceRectangle));
            SpriteBatchPatches.InitializePatch();
        }

        public void RemoveDrawPatch(string id)
        {
            SpriteBatchPatches.DrawPatches.RemoveWhere(p => p.Id == id);
        }
        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            foreach (var obj in TracedObjects)
                if (obj.Target is ILinked linked)
                    linked.OnUnLink(Plato, obj.Original);

            TracedObjects.Clear();
        }

        public void UnlinkObjects(object original, object target = null)
        {
            TracedObjects.RemoveWhere(o =>
            {
                bool remove = (o.Original == original || original == null) && (o.Target == target || target == null);
                if(remove && o.Target is ILinked linked)
                        linked.OnUnLink(Plato, o.Original);

                return remove;
            });
        }

        public bool TryGetLink(object linkedObject, out object target)
        {
            if (TracedObjects.FirstOrDefault(t => t.Original == linkedObject) is TracedObject traced)
            {
                target = traced;
                return true;
            }
            target = null;
            return false;
        }
      
        public void LinkContruction<TOriginal, TTarget>()
        {
            if (LinkedConstructors.Any(l => l.FromType == typeof(TOriginal) && l.ToType == typeof(TTarget)))
                return;

            LinkedConstructors.Add(new TypeForwarding(typeof(TOriginal), typeof(TTarget), Plato, null));

            if (LinkedConstructors.Count(l => l.FromType == typeof(TOriginal)) > 1)
                return;

            foreach (ConstructorInfo constructor in 
                typeof(TOriginal)
                .GetConstructors())
            {
                List<Type> patchParameters = new List<Type>();
                List<Type> genericParameters = new List<Type>();

                patchParameters.Add(typeof(TOriginal));
                genericParameters.Add(typeof(TOriginal));
                patchParameters.Add(typeof(MethodInfo));


                patchParameters.AddRange(constructor.GetParameters().Select(p => p.ParameterType));
                genericParameters.AddRange(constructor.GetParameters().Select(p => p.ParameterType));

                if (AccessTools.GetDeclaredMethods(typeof(ConstructorPatches)).FirstOrDefault(m => m.Name == nameof(ConstructorPatches.ConstructorPatch) && m.IsGenericMethod && m.GetParameters().Length == patchParameters.Count) is MethodInfo sMethod)
                {
                    var gMethod = sMethod.MakeGenericMethod(genericParameters.ToArray());
                    var hMethod = new HarmonyMethod(gMethod);

                    HarmonyInstance.Patch(
                        original: constructor,
                        postfix: hMethod
                        );
                }
            }
        }

        public void LinkTypes(Type originalType, Type targetType, object targetForAllInstances = null)
        {
            if (TracedTypes.Any(tt => tt.FromType == originalType && tt.ToType == targetType))
                return;

            TracedTypes.Add(new TypeForwarding(originalType, targetType, Plato, targetForAllInstances));

            foreach (MethodInfo tmethod in AccessTools.GetDeclaredMethods(targetType))
            {
                if (AccessTools.Method(originalType,tmethod.Name, tmethod.GetParameters().Select(p => p.ParameterType).ToArray(), null) is MethodInfo method && method.GetParameters() is ParameterInfo[] parameters)
                {
                    if (TracedMethods.Contains(method))
                        continue;
                    else
                        TracedMethods.Add(method);

                    bool hasReturnType = method.ReturnType != Type.GetType("System.Void");
                    List<Type> patchParameters = new List<Type>();
                    List<Type> genericParameters = new List<Type>();

                    if (hasReturnType)
                    {
                        genericParameters.Add(method.ReturnType);
                        patchParameters.Add(typeof(object).MakeByRefType());
                    }

                    genericParameters.Add(originalType);

                    string patchName = hasReturnType ? nameof(MethodPatches.ForwardMethodPatch) : nameof(MethodPatches.ForwardMethodPatchVoid);

                    patchParameters.Add(typeof(object));
                    patchParameters.Add(typeof(MethodInfo));
                    patchParameters.AddRange(method.GetParameters().Select(p => typeof(object)));
                    genericParameters.AddRange(method.GetParameters().Select(p => p.ParameterType));

                    if (AccessTools.GetDeclaredMethods(typeof(MethodPatches)).FirstOrDefault(m => m.Name == patchName && m.IsGenericMethod && m.GetParameters().Length == patchParameters.Count) is MethodInfo sMethod)
                    {
                        HarmonyInstance.Patch(
                            original: method,
                            prefix: new HarmonyMethod(sMethod.MakeGenericMethod(genericParameters.ToArray()))
                            );
                    }

                }
            }

        }

        public bool ForwardMethodVoid(object __instance, MethodInfo __originalMethod, params object[] args)
        {
            return MethodPatches.ForwardMethodVoid(__instance, __originalMethod, args);
        }

        public bool ForwardMethod(ref object __result, object __instance, MethodInfo __originalMethod, params object[] args)
        {
            return MethodPatches.ForwardMethod(ref __result, __instance, __originalMethod, args);
        }


        public bool LinkObjects(object original, object target)
        {
            if (original == null || target == null)
                return false;

            if (target is ILinked linked1 && !linked1.CanLinkWith(original))
                return false;

            if (target is ILinked linked2)
            {
                linked2.Link = new Link(original, target, Plato);
                linked2.OnLink(Plato, original);
            }

            if (TracedObjects.Any(t => t.Original == original && t.Target == target))
                return false;

            Type targetType = target.GetType();
            Type originalType = original.GetType();

            TracedObjects.Add(new TracedObject(original, target, Plato));

            LinkTypes(originalType, targetType, null);
            return true;
        }

        
    }
}
