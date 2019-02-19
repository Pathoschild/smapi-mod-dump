using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Json;

namespace TehPers.CoreMod.Api {
    public interface ICoreApi {
        /// <summary>The mod that owns this core API object.</summary>
        IMod Owner { get; }

        /// <summary>The content source associated with the owner of this core API. Content loaded through this will be loaded from the owner's mod folder.</summary>
        IContentSource ContentSource { get; }

        /// <summary>The translation helper associated with the owner of this core API. Translations loaded through this will be loaded from the owner's mod folder.</summary>
        ICoreTranslationHelper TranslationHelper { get; }

        /// <summary>The drawing API which can be used to override draw calls to <see cref="SpriteBatch"/>.</summary>
        IDrawingApi Drawing { get; }

        /// <summary>The item API which can be used to create new items in the game.</summary>
        IItemApi Items { get; }

        /// <summary>The JSON API which can be used to create and read commented JSON files.</summary>
        IJsonApi Json { get; }

        // /// <summary>The contnet pack API which can be used to read complex content packs.</summary>
        // TODO: IContentPackApi ContentPacks { get; }
    }
}
