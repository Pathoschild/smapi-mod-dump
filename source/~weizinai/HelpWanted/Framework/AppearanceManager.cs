/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;

namespace weizinai.StardewValleyMod.HelpWanted.Framework;

internal class AppearanceManager
{
    private readonly ModConfig config;

    private const string PadTexturePath = "aedenthorn.HelpWanted/Pad";
    private const string PinTexturePath = "aedenthorn.HelpWanted/Pin";

    private readonly Texture2D defaultPadTexture;
    private readonly Texture2D defaultPinTexture;

    public AppearanceManager(IModHelper helper, ModConfig config)
    {
        this.config = config;
        this.defaultPadTexture = helper.ModContent.Load<Texture2D>("Assets/Pad.png");
        this.defaultPinTexture = helper.ModContent.Load<Texture2D>("Assets/Pin.png");
    }

    public Texture2D GetPinTexture(string target, string questType)
    {
        // 获取特定NPC和特定任务类型的任务的自定义Pin纹理
        var texture = this.GetTexture(PinTexturePath + "/" + target + "/" + questType);
        if (texture != null) return texture;

        // 获取特定NPC的任务的自定义Pin纹理
        texture = this.GetTexture(PinTexturePath + "/" + target);
        if (texture is not null) return texture;

        // 获取特定任务类型的任务的自定义Pin纹理
        texture = this.GetTexture(PinTexturePath + "/" + questType);
        if (texture is not null) return texture;

        // 获取自定义Pin纹理
        texture = this.GetTexture(PinTexturePath);
        return texture ?? this.defaultPinTexture;
    }

    public Texture2D GetPadTexture(string target, string questType)
    {
        // 获取特定NPC和特定任务类型的任务的自定义Pad纹理
        var texture = this.GetTexture(PadTexturePath + "/" + target + "/" + questType);
        if (texture is not null) return texture;

        // 获取特定NPC的任务的自定义Pad纹理
        texture = this.GetTexture(PadTexturePath + "/" + target);
        if (texture is not null) return texture;

        // 获取特定任务类型的任务的自定义Pad纹理
        texture = this.GetTexture(PadTexturePath + "/" + questType);
        if (texture is not null) return texture;

        // 获取自定义Pad纹理
        texture = this.GetTexture(PadTexturePath);
        return texture ?? this.defaultPadTexture;
    }

    private Texture2D? GetTexture(string path)
    {
        // 获取特定NPC或特定任务类型的任务的不同自定义纹理,如果纹理存在,则随机返回一个
        var textures = new List<Texture2D>();
        try
        {
            for (var i = 1;; i++) textures.Add(Game1.content.Load<Texture2D>(path + "/" + i));
        }
        catch
        {
            // ignored
        }

        if (textures.Any())
        {
            return Game1.random.ChooseFrom(textures);
        }

        // 获取特定NPC或特定任务类型的任务的自定义纹理
        try
        {
            return Game1.content.Load<Texture2D>(path);
        }
        catch
        {
            // ignored
        }

        return null;
    }

    public Color GetRandomColor()
    {
        var random = Game1.random;
        return new Color((byte)random.Next(this.config.RandomColorMin, this.config.RandomColorMax),
            (byte)random.Next(this.config.RandomColorMin, this.config.RandomColorMax),
            (byte)random.Next(this.config.RandomColorMin, this.config.RandomColorMax));
    }
}