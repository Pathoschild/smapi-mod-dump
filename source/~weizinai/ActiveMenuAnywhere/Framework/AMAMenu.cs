/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using ActiveMenuAnywhere.Framework.Options;
using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework;

internal class AMAMenu : IClickableMenu
{
    private const int InnerWidth = 600;
    private const int InnerHeight = 600;

    private const int OptionsPerPage = 9;
    private readonly IModHelper helper;

    private readonly (int x, int y) innerDrawPosition =
        (x: Game1.uiViewport.Width / 2 - InnerWidth / 2, y: Game1.uiViewport.Height / 2 - InnerHeight / 2);

    private readonly List<BaseOption> options = new();
    private readonly List<float> optionScales = new(OptionsPerPage) { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
    private readonly List<ClickableComponent> optionSlots = new();
    private readonly List<ClickableComponent> tabs = new();

    private MenuTabID currentMenuTabID;

    private int currentPage;
    private ClickableTextureComponent downArrow;
    private ClickableComponent title;
    private ClickableTextureComponent upArrow;

    public AMAMenu(MenuTabID menuTabID, IModHelper helper)
    {
        this.helper = helper;
        Init(menuTabID);
        ResetComponents();
        SetOptions();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        // arrow
        if (upArrow.containsPoint(x, y) && currentPage > 0) currentPage--;
        if (downArrow.containsPoint(x, y) && options.Count - (currentPage + 1) * OptionsPerPage > 0) currentPage++;

        // tab
        var tab = tabs.FirstOrDefault(tab => tab.containsPoint(x, y));
        if (tab != null) Game1.activeClickableMenu = new AMAMenu(GetTabID(tab), helper);

        // option
        for (var i = 0; i < OptionsPerPage; i++)
        {
            var optionsIndex = i + currentPage * OptionsPerPage;
            if (optionSlots[i].containsPoint(x, y) && optionsIndex < options.Count)
            {
                options[optionsIndex].ReceiveLeftClick();
                break;
            }
        }
    }

    public override void performHoverAction(int x, int y)
    {
        for (var i = 0; i < OptionsPerPage; i++)
            if (optionSlots[i].containsPoint(x, y) && i + currentPage * OptionsPerPage < options.Count)
                optionScales[i] = 0.9f;
            else
                optionScales[i] = 1f;
    }

    public override void draw(SpriteBatch spriteBatch)
    {
        // Draw shadow
        DrawHelper.DrawShadow();

        // Draw background
        drawTextureBox(spriteBatch, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

        // Draw title
        DrawHelper.DrawTitle(title.bounds.X, title.bounds.Y, title.name, Align.Center);

        // Draw arrows
        upArrow.draw(spriteBatch);
        downArrow.draw(spriteBatch);

        // Draw tabs
        tabs.ForEach(tab => DrawHelper.DrawTab(tab.bounds.X + tab.bounds.Width, tab.bounds.Y, Game1.smallFont, tab.name, Align.Right,
            GetTabID(tab) == currentMenuTabID ? 0.7f : 1f));

        // Draw options
        DrawOption();

        // Draw Mouse
        drawMouse(spriteBatch);
    }

    private void Init(MenuTabID menuTabID)
    {
        width = InnerWidth + borderWidth * 2;
        height = InnerHeight + borderWidth * 2;
        xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
        yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;

        currentMenuTabID = menuTabID;
    }

    private void ResetComponents()
    {
        // Add title
        const int titleOffsetY = -64;
        title = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2, yPositionOnScreen + titleOffsetY, 0, 0),
            "ActiveMenuAnywhere");

        // Add arrows
        AddArrows();

        // Add tabs
        AddTabs();

        // Add optionSlots
        AddOptionSlots();
    }

    private MenuTabID GetTabID(ClickableComponent tab)
    {
        if (!Enum.TryParse(tab.label, out MenuTabID tabID))
            throw new InvalidOperationException($"Couldn't parse tab name '{tab.label}'.");
        return tabID;
    }

    private Rectangle GetBoundsRectangle(int index)
    {
        var i = index % 3;
        var j = index / 3;
        return new Rectangle(innerDrawPosition.x + i * 200, innerDrawPosition.y + j * 200, 200, 200);
    }

    private Rectangle GetSourceRectangle(int index)
    {
        var i = index % 3;
        var j = index / 3;
        return new Rectangle(i * 200, j * 200, 200, 200);
    }

    private Rectangle GetTabRectangle(int index)
    {
        var tabOffset = (x: 4, y: 16);
        var tabSize = (width: 100, height: 48);
        var tabPosition = (x: xPositionOnScreen - tabSize.width, y: yPositionOnScreen + tabOffset.y);

        return new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * index, tabSize.width - tabOffset.x, tabSize.height);
    }

    private void AddArrows()
    {
        const float scale = 4f;
        var offset = (x: 8, y: (int)(12 * scale / 2));
        upArrow = new ClickableTextureComponent(
            new Rectangle(xPositionOnScreen + width + offset.x, yPositionOnScreen - offset.y, (int)(11 * scale), (int)(12 * scale)),
            Game1.mouseCursors, new Rectangle(421, 459, 11, 12), scale);
        downArrow = new ClickableTextureComponent(
            new Rectangle(xPositionOnScreen + width + offset.x, yPositionOnScreen + height - offset.y, (int)(11 * scale),
                (int)(12 * scale)),
            Game1.mouseCursors, new Rectangle(421, 472, 11, 12), scale);
    }

    private void AddTabs()
    {
        var tabOffset = (x: 4, y: 16);
        var tabSize = (width: 100, height: 48);
        var tabPosition = (x: xPositionOnScreen - tabSize.width, y: yPositionOnScreen + tabOffset.y);

        var i = 0;
        tabs.Clear();
        tabs.AddRange(new[]
        {
            new ClickableComponent(GetTabRectangle(i++), I18n.Tab_Farm(), MenuTabID.Farm.ToString()),
            new ClickableComponent(GetTabRectangle(i++), I18n.Tab_Town(), MenuTabID.Town.ToString()),
            new ClickableComponent(GetTabRectangle(i++), I18n.Tab_Mountain(), MenuTabID.Mountain.ToString()),
            new ClickableComponent(GetTabRectangle(i++), I18n.Tab_Forest(), MenuTabID.Forest.ToString()),
            new ClickableComponent(GetTabRectangle(i++), I18n.Tab_Beach(), MenuTabID.Beach.ToString()),
            new ClickableComponent(GetTabRectangle(i++), I18n.Tab_Desert(), MenuTabID.Desert.ToString()),
            new ClickableComponent(GetTabRectangle(i++), I18n.Tab_GingerIsland(), MenuTabID.GingerIsland.ToString())
        });
        if (helper.ModRegistry.Get("FlashShifter.SVECode") != null)
            tabs.Add(new ClickableComponent(GetTabRectangle(i++), I18n.Tab_SVE(), MenuTabID.SVE.ToString()));
        if (helper.ModRegistry.Get("Rafseazz.RidgesideVillage") != null)
            tabs.Add(new ClickableComponent(GetTabRectangle(i), I18n.Tab_RSV(), MenuTabID.RSV.ToString()));
    }

    private void SetOptions()
    {
        options.Clear();
        switch (currentMenuTabID)
        {
            case MenuTabID.Farm:
                options.AddRange(new BaseOption[]
                {
                    new TVOption(GetSourceRectangle(0), helper),
                    new ShippingBinOption(GetSourceRectangle(1), helper)
                });
                break;
            case MenuTabID.Town:
                options.AddRange(new BaseOption[]
                {
                    new BillboardOption(GetSourceRectangle(0)),
                    new SpecialOrderOption(GetSourceRectangle(1)),
                    new CommunityCenterOption(GetSourceRectangle(2)),
                    new PierreOption(GetSourceRectangle(3)),
                    new ClintOption(GetSourceRectangle(4)),
                    new GusOption(GetSourceRectangle(5)),
                    new JojaShopOption(GetSourceRectangle(6)),
                    new PrizeTicketOption(GetSourceRectangle(7)),
                    new BooksellerOption(GetSourceRectangle(8)),
                    new KrobusOption(GetSourceRectangle(9)),
                    new StatueOption(GetSourceRectangle(10)),
                    new HarveyOption(GetSourceRectangle(11)),
                    new TailoringOption(GetSourceRectangle(12)),
                    new DyeOption(GetSourceRectangle(13)),
                    new IceCreamStandOption(GetSourceRectangle(14)),
                    new AbandonedJojaMartOption(GetSourceRectangle(15))
                });
                break;
            case MenuTabID.Mountain:
                options.AddRange(new BaseOption[]
                {
                    new RobinOption(GetSourceRectangle(0)),
                    new DwarfOption(GetSourceRectangle(1)),
                    new MonsterOption(GetSourceRectangle(2), helper),
                    new MarlonOption(GetSourceRectangle(3))
                });
                break;
            case MenuTabID.Forest:
                options.AddRange(new BaseOption[]
                {
                    new MarnieOption(GetSourceRectangle(0)),
                    new TravelerOption(GetSourceRectangle(1)),
                    new HatMouseOption(GetSourceRectangle(2)),
                    new WizardOption(GetSourceRectangle(3)),
                    new RaccoonOption(GetSourceRectangle(4), helper)
                });
                break;
            case MenuTabID.Beach:
                options.AddRange(new BaseOption[]
                {
                    new WillyOption(GetSourceRectangle(0)),
                    new BobberOption(GetSourceRectangle(1)),
                    new NightMarketTraveler(GetSourceRectangle(2)),
                    new DecorationBoatOption(GetSourceRectangle(3)),
                    new MagicBoatOption(GetSourceRectangle(4))
                });
                break;
            case MenuTabID.Desert:
                options.AddRange(new BaseOption[]
                {
                    new SandyOption(GetSourceRectangle(0)),
                    new DesertTradeOption(GetSourceRectangle(1)),
                    new CasinoOption(GetSourceRectangle(2)),
                    new FarmerFileOption(GetSourceRectangle(3)),
                    new BuyQiCoinsOption(GetSourceRectangle(4)),
                    new ClubSellerOption(GetSourceRectangle(5))
                });
                break;
            case MenuTabID.GingerIsland:
                options.AddRange(new BaseOption[]
                {
                    new QiSpecialOrderOption(GetSourceRectangle(0)),
                    new QiGemShopOption(GetSourceRectangle(1)),
                    new QiCatOption(GetSourceRectangle(2), helper),
                    new IslandTradeOption(GetSourceRectangle(3)),
                    new IslandResortOption(GetSourceRectangle(4)),
                    new VolcanoShopOption(GetSourceRectangle(5)),
                    new ForgeOption(GetSourceRectangle(6))
                });
                break;
            case MenuTabID.SVE:
                options.AddRange(new BaseOption[]
                {
                    new SophiaOption(GetSourceRectangle(0))
                });
                break;
            case MenuTabID.RSV:
                options.AddRange(new BaseOption[]
                {
                    new RSVQuestBoardOption(GetSourceRectangle(0), helper),
                    new RSVSpecialOrderOption(GetSourceRectangle(1), helper),
                    new IanOption(GetSourceRectangle(2), helper),
                    new PaulaOption(GetSourceRectangle(3), helper),
                    new LorenzoOption(GetSourceRectangle(4)),
                    new JericOption(GetSourceRectangle(5)),
                    new KimpoiOption(GetSourceRectangle(6)),
                    new PikaOption(GetSourceRectangle(7)),
                    new LolaOption(GetSourceRectangle(8)),
                    new NinjaBoardOption(GetSourceRectangle(10)),
                    new JoiOption(GetSourceRectangle(11)),
                    new MysticFalls1Option(GetSourceRectangle(12)),
                    new MysticFalls2Option(GetSourceRectangle(13)),
                    new MysticFalls3Option(GetSourceRectangle(14)),
                });
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddOptionSlots()
    {
        for (var i = 0; i < OptionsPerPage; i++) optionSlots.Add(new ClickableComponent(GetBoundsRectangle(i), ""));
    }

    private void DrawOption()
    {
        var spriteBatch = Game1.spriteBatch;
        for (var i = 0; i < OptionsPerPage; i++)
        {
            var optionsIndex = i + currentPage * OptionsPerPage;
            // 选项槽的位置和大小
            var bounds = optionSlots[i].bounds;
            // 根据缩放调整纹理绘制的位置和大小
            var size = (width: (int)(bounds.Width * optionScales[i]), height: (int)(bounds.Height * optionScales[i]));
            var position = (x: bounds.X + (bounds.Width - size.width) / 2, y: bounds.Y + (bounds.Height - size.height) / 2);
            // 如果选项绘制完成,则停止绘制
            if (optionsIndex >= options.Count) break;
            // 绘制纹理
            drawTextureBox(spriteBatch, ModEntry.Textures[currentMenuTabID], options[optionsIndex].SourceRect,
                position.x, position.y, size.width, size.height, Color.White);
            // 绘制标签
            var x = bounds.X + bounds.Width / 2;
            var y = bounds.Y + bounds.Height / 3 * 2;
            DrawHelper.DrawTab(x, y, Game1.smallFont, options[optionsIndex].Name, Align.Center);
        }
    }
}