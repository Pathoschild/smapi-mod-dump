/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using HDPortraits.Patches;
using HDPortraits.Models;

namespace HDPortraits
{
    public class API : IHDPortraitsAPI
    {
        public string OverrideName { 
            get => PortraitDrawPatch.overrideName.Value; 
            set => PortraitDrawPatch.overrideName.Value = value; 
        }
        public void DrawPortrait(SpriteBatch b, NPC npc, int index, Rectangle region, Color? color = null, bool reset = false)
        {
            (Rectangle source, Texture2D tex) = GetTextureAndRegion(npc, index, Game1.currentGameTime.ElapsedGameTime.Milliseconds, reset);
            b.Draw(tex, region, source, color ?? Color.White);
        }
        public void DrawPortrait(SpriteBatch b, NPC npc, int index, Point position, Color? color = null, bool reset = false)
        {
            DrawPortrait(b, npc, index, new Rectangle(position, new(256, 256)), color, reset);
        }
        public void DrawPortrait(SpriteBatch b, string name, string suffix, int index, Rectangle region, Color? color = null, bool reset = false)
        {
            (Rectangle source, Texture2D tex) = GetTextureAndRegion(name, suffix, index, Game1.currentGameTime.ElapsedGameTime.Milliseconds, reset);
            b.Draw(tex, region, source, color ?? Color.White);
        }
        public void DrawPortrait(SpriteBatch b, string name, string suffix, int index, Point position, Color? color = null, bool reset = false)
        {
            DrawPortrait(b, name, suffix, index, new Rectangle(position, new(256, 256)), color, reset);
        }
        public void DrawPortraitOrOverride(SpriteBatch b, NPC npc, int index, Rectangle region, Color? color = null, bool reset = false)
        {
            if (npc is not null)
                DrawPortrait(b, npc, index, region, color, reset);
            else if(PortraitDrawPatch.overrideName.Value is not null)
                DrawPortrait(b, PortraitDrawPatch.overrideName.Value, null, index, region, color, reset);
        }
        public void DrawPortraitOrOverride(SpriteBatch b, NPC npc, int index, Point position, Color? color = null, bool reset = false)
        {
            DrawPortraitOrOverride(b, npc, index, new Rectangle(position, new(256, 256)), color, reset);
        }
        public string GetEventPortraitFor(NPC npc)
        {
            return npc.uniquePortraitActive ? PortraitDrawPatch.EventOverrides.Value.GetValueOrDefault(npc.Name, null) : null;
        }
        public (Rectangle, Texture2D) GetTextureAndRegion(NPC npc, int index, int elapsed = -1, bool reset = false)
        {
            if(npc.uniquePortraitActive)
                return GetTextureAndRegion(npc.Name, PortraitDrawPatch.EventOverrides.Value[npc.Name], index, elapsed, reset);
            return GetTextureAndRegion(npc.getTextureName(), PortraitDrawPatch.GetSuffix(npc), index, elapsed, reset);
        }
        public (Rectangle, Texture2D) GetTextureAndRegion(string name, string suffix, int index, int elapsed = -1, bool reset = false)
        {
            var path = $"Portraits/{name}{(suffix is not null ? '_' + suffix : null)}";
            if (!Utils.TryLoadAsset<Texture2D>(path, out var tex))
                throw new ContentLoadException($"Default portrait '{path}' does not exist or could not be loaded! Do you have a typo or missing asset?");

            if (!ModEntry.TryGetMetadata(name, suffix, out var metadata))
                return (Game1.getSourceRectForStandardTileSheet(tex, index, 64, 64), tex);

            if (reset)
                metadata.Animation?.Reset();

            PortraitDrawPatch.lastLoaded.Value.Add(metadata);

            Texture2D texture = metadata.overrideTexture.Value ?? tex;
            Rectangle rect = (metadata.Animation != null) ?
                metadata.Animation.GetSourceRegion(texture, metadata.Size, index, elapsed) :
                Game1.getSourceRectForStandardTileSheet(texture, index, metadata.Size, metadata.Size);
            return (rect, texture);
        }
        public void ReloadData()
        {
            ModEntry.ReloadData();
        }
    }
}
