/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace MoreGrass;

/// <summary>Represents a collection of a custom and base game sprites of grass for a season.</summary>
public class SpritePool
{
    /// <summary>The maximum number of grass sprites wide the atlas can be.</summary>
    /// <remarks>The size of a grass sprite is 15x20, meaning the atlas will be at most 1020 pixels wide.</remarks>
    private const int MaxAtlasWidth = 68;


    /*********
    ** Fields
    *********/
    /// <summary>Whether the default sprites should be including in the resulting sprite collection.</summary>
    private bool _IncludeDefaultGrass;

    /// <summary>The default grass sprites in the sprite pool.</summary>
    private readonly List<Texture2D> DefaultGrassSprites = [];

    /// <summary>The custom grass sprites in the sprite pool.</summary>
    private readonly List<GrassSprite> CustomGrassSprites = [];


    /*********
    ** Properties
    *********/
    /// <summary>Gets the number of sprites in the sprite pool.</summary>
    public int Count => IncludeDefaultGrass ? DefaultGrassSprites.Count + CustomGrassSprites.Count : CustomGrassSprites.Count;

    /// <summary>The sprite atlas of the sprite pool.</summary>
    public Texture2D Atlas { get; private set; }

    /// <summary>Whether the default sprites should be including in the resulting sprite collection.</summary>
    public bool IncludeDefaultGrass
    {
        get => _IncludeDefaultGrass || CustomGrassSprites.Count == 0;
        set => _IncludeDefaultGrass = value;
    }


    /*********
    ** Public Methods
    *********/
    /// <summary>Removes all the default grass sprites from the sprite pool.</summary>
    public void ClearDefaultGrass() => DefaultGrassSprites.Clear();

    /// <summary>Adds grass to the default part of the sprite pool.</summary>
    /// <param name="sprite">The sprite to add to the sprite pool.</param>
    public void AddDefaultGrass(Texture2D sprite)
    {
        if (sprite != null)
            DefaultGrassSprites.Add(sprite);
    }

    /// <summary>Adds grass to the custom part of the sprite pool.</summary>
    /// <param name="sprite">The sprite to add to the sprite pool.</param>
    public void AddCustomGrass(Texture2D sprite, List<string> whiteListedLocations, List<string> blackListedLocations)
    {
        if (sprite != null)
            CustomGrassSprites.Add(new GrassSprite(sprite, whiteListedLocations, blackListedLocations));
    }

    /// <summary>Retrieves a random sprite id.</summary>
    /// <param name="defaultOnly">Whether only default sprites should be picked.</param>
    /// <returns>A random sprite id.</returns>
    public int GetRandomSpriteId(bool defaultOnly) => Game1.random.Next(defaultOnly ? DefaultGrassSprites.Count : Count);

    /// <summary>Retrieves the offset into the atlas for a sprite id.</summary>
    /// <param name="spriteId">The sprite id to get the offset into the atlas of.</param>
    /// <returns>The offset in the atlas of the specified sprite id.</returns>
    public static Point GetOffsetFromSpriteId(int spriteId) => new(spriteId % MaxAtlasWidth * 15, spriteId / MaxAtlasWidth * 20);

    /// <summary>Regenerates the atlas.</summary>
    public void RegenerateAtlas()
    {
        Atlas?.Dispose();

        var rows = (Count / MaxAtlasWidth) + 1;
        var columns = rows > 1 ? MaxAtlasWidth : (Count % MaxAtlasWidth) + 1;

        Atlas = new Texture2D(Game1.graphics.GraphicsDevice, columns * 15, rows * 20);

        for (int spriteId = 0; spriteId < Count; spriteId++)
        {
            var grassSprite = GetSpriteById(spriteId);

            // some content packs have invalid sprite sizes that an old version of MoreGrass didn't pick up on
            // instead of rejecting them and breaking the content pack, we'll just work around them
            var spriteWidth = MathHelper.Min(15, grassSprite.Width);
            var spriteHeight = MathHelper.Min(20, grassSprite.Height);
            var grassData = new Color[spriteWidth * spriteHeight];

            grassSprite.GetData(0, new Rectangle(0, 0, spriteWidth, spriteHeight), grassData, 0, grassData.Length);

            var offset = GetOffsetFromSpriteId(spriteId);
            offset.Y += 20 - spriteHeight; // if a grass sprite is shorter than it should be, align it to the bottom,
                                           // instead of the top so the grass doesn't look like it's floating
            Atlas.SetData(0, new Rectangle(offset, new Point(spriteWidth, spriteHeight)), grassData, 0, grassData.Length);
        }
    }


    /*********
    ** Private Methods
    *********/
    /// <summary>Retrieves the sprite from an id.</summary>
    /// <param name="id">The id of the sprite.</param>
    /// <returns>The sprite with an id of <paramref name="id"/>.</returns>
    private Texture2D GetSpriteById(int id) => id < DefaultGrassSprites.Count ? DefaultGrassSprites[id] : CustomGrassSprites[id - DefaultGrassSprites.Count].Sprite;
}
