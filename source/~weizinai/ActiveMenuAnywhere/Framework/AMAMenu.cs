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
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

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
    private ClickableTextureComponent downArrow = null!;
    private ClickableComponent title = null!;
    private ClickableTextureComponent upArrow = null!;

    public AMAMenu(MenuTabID menuTabID, IModHelper helper)
    {
        this.helper = helper;
        this.Init(menuTabID);
        this.ResetComponents();
        this.SetOptions();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        // arrow
        if (this.upArrow.containsPoint(x, y) && this.currentPage > 0) this.currentPage--;
        if (this.downArrow.containsPoint(x, y) && this.options.Count - (this.currentPage + 1) * OptionsPerPage > 0) this.currentPage++;

        // tab
        var tab = this.tabs.FirstOrDefault(tab => tab.containsPoint(x, y));
        if (tab != null) Game1.activeClickableMenu = new AMAMenu(this.GetTabID(tab), this.helper);

        // option
        for (var i = 0; i < OptionsPerPage; i++)
        {
            var optionsIndex = i + this.currentPage * OptionsPerPage;
            if (this.optionSlots[i].containsPoint(x, y) && optionsIndex < this.options.Count)
            {
                this.options[optionsIndex].ReceiveLeftClick();
                break;
            }
        }
    }

    public override void performHoverAction(int x, int y)
    {
        for (var i = 0; i < OptionsPerPage; i++)
            if (this.optionSlots[i].containsPoint(x, y) && i + this.currentPage * OptionsPerPage < this.options.Count)
                this.optionScales[i] = 0.9f;
            else
                this.optionScales[i] = 1f;
    }

    public override void draw(SpriteBatch spriteBatch)
    {
        // Draw shadow
        DrawHelper.DrawShadow();

        // Draw background
        drawTextureBox(spriteBatch, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White);

        // Draw title
        DrawHelper.DrawTitle(this.title.bounds.X, this.title.bounds.Y, this.title.name, Align.Center);

        // Draw arrows
        this.upArrow.draw(spriteBatch);
        this.downArrow.draw(spriteBatch);

        // Draw tabs
        this.tabs.ForEach(tab => DrawHelper.DrawTab(tab.bounds.X + tab.bounds.Width, tab.bounds.Y, Game1.smallFont, tab.name, Align.Right, this.GetTabID(tab) == this.currentMenuTabID ? 0.7f : 1f));

        // Draw options
        this.DrawOption();

        // Draw Mouse
        this.drawMouse(spriteBatch);
    }

    private void Init(MenuTabID menuTabID)
    {
        this.width = InnerWidth + borderWidth * 2;
        this.height = InnerHeight + borderWidth * 2;
        this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.width / 2;
        this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.height / 2;

        this.currentMenuTabID = menuTabID;
    }

    private void ResetComponents()
    {
        // Add title
        const int titleOffsetY = -64;
        this.title = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + titleOffsetY, 0, 0),
            "ActiveMenuAnywhere");

        // Add arrows
        this.AddArrows();

        // Add tabs
        this.AddTabs();

        // Add optionSlots
        this.AddOptionSlots();
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
        return new Rectangle(this.innerDrawPosition.x + i * 200, this.innerDrawPosition.y + j * 200, 200, 200);
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
        var tabPosition = (x: this.xPositionOnScreen - tabSize.width, y: this.yPositionOnScreen + tabOffset.y);

        return new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * index, tabSize.width - tabOffset.x, tabSize.height);
    }

    private void AddArrows()
    {
        const float scale = 4f;
        var offset = (x: 8, y: (int)(12 * scale / 2));
        this.upArrow = new ClickableTextureComponent(
            new Rectangle(this.xPositionOnScreen + this.width + offset.x, this.yPositionOnScreen - offset.y, (int)(11 * scale), (int)(12 * scale)),
            Game1.mouseCursors, new Rectangle(421, 459, 11, 12), scale);
        this.downArrow = new ClickableTextureComponent(
            new Rectangle(this.xPositionOnScreen + this.width + offset.x, this.yPositionOnScreen + this.height - offset.y, (int)(11 * scale),
                (int)(12 * scale)),
            Game1.mouseCursors, new Rectangle(421, 472, 11, 12), scale);
    }

    private void AddTabs()
    {
        var i = 0;
        this.tabs.Clear();
        this.tabs.AddRange(new[]
        {
            new ClickableComponent(this.GetTabRectangle(i++), I18n.Tab_Farm(), MenuTabID.Farm.ToString()),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.Tab_Town(), MenuTabID.Town.ToString()),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.Tab_Mountain(), MenuTabID.Mountain.ToString()),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.Tab_Forest(), MenuTabID.Forest.ToString()),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.Tab_Beach(), MenuTabID.Beach.ToString()),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.Tab_Desert(), MenuTabID.Desert.ToString()),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.Tab_GingerIsland(), MenuTabID.GingerIsland.ToString())
        });
        if (this.helper.ModRegistry.Get("FlashShifter.SVECode") != null) this.tabs.Add(new ClickableComponent(this.GetTabRectangle(i++), I18n.Tab_SVE(), MenuTabID.SVE.ToString()));
        if (this.helper.ModRegistry.Get("Rafseazz.RidgesideVillage") != null) this.tabs.Add(new ClickableComponent(this.GetTabRectangle(i), I18n.Tab_RSV(), MenuTabID.RSV.ToString()));
    }

    private void SetOptions()
    {
        this.options.Clear();
        switch (this.currentMenuTabID)
        {
            case MenuTabID.Farm:
                this.options.AddRange(new BaseOption[]
                {
                    new TVOption(this.GetSourceRectangle(0), this.helper),
                    new ShippingBinOption(this.GetSourceRectangle(1), this.helper)
                });
                break;
            case MenuTabID.Town:
                this.options.AddRange(new BaseOption[]
                {
                    new BillboardOption(this.GetSourceRectangle(0)),
                    new SpecialOrderOption(this.GetSourceRectangle(1)),
                    new CommunityCenterOption(this.GetSourceRectangle(2)),
                    new PierreOption(this.GetSourceRectangle(3)),
                    new ClintOption(this.GetSourceRectangle(4)),
                    new GusOption(this.GetSourceRectangle(5)),
                    new JojaShopOption(this.GetSourceRectangle(6)),
                    new PrizeTicketOption(this.GetSourceRectangle(7)),
                    new BooksellerOption(this.GetSourceRectangle(8)),
                    new KrobusOption(this.GetSourceRectangle(9)),
                    new StatueOption(this.GetSourceRectangle(10)),
                    new HarveyOption(this.GetSourceRectangle(11)),
                    new TailoringOption(this.GetSourceRectangle(12)),
                    new DyeOption(this.GetSourceRectangle(13)),
                    new IceCreamStandOption(this.GetSourceRectangle(14)),
                    new AbandonedJojaMartOption(this.GetSourceRectangle(15))
                });
                break;
            case MenuTabID.Mountain:
                this.options.AddRange(new BaseOption[]
                {
                    new RobinOption(this.GetSourceRectangle(0)),
                    new DwarfOption(this.GetSourceRectangle(1)),
                    new MonsterOption(this.GetSourceRectangle(2), this.helper),
                    new MarlonOption(this.GetSourceRectangle(3))
                });
                break;
            case MenuTabID.Forest:
                this.options.AddRange(new BaseOption[]
                {
                    new MarnieOption(this.GetSourceRectangle(0)),
                    new TravelerOption(this.GetSourceRectangle(1)),
                    new HatMouseOption(this.GetSourceRectangle(2)),
                    new WizardOption(this.GetSourceRectangle(3)),
                    new RaccoonOption(this.GetSourceRectangle(4), this.helper)
                });
                break;
            case MenuTabID.Beach:
                this.options.AddRange(new BaseOption[]
                {
                    new WillyOption(this.GetSourceRectangle(0)),
                    new BobberOption(this.GetSourceRectangle(1)),
                    new NightMarketTraveler(this.GetSourceRectangle(2)),
                    new DecorationBoatOption(this.GetSourceRectangle(3)),
                    new MagicBoatOption(this.GetSourceRectangle(4))
                });
                break;
            case MenuTabID.Desert:
                this.options.AddRange(new BaseOption[]
                {
                    new SandyOption(this.GetSourceRectangle(0)),
                    new DesertTradeOption(this.GetSourceRectangle(1)),
                    new CasinoOption(this.GetSourceRectangle(2)),
                    new FarmerFileOption(this.GetSourceRectangle(3)),
                    new BuyQiCoinsOption(this.GetSourceRectangle(4)),
                    new ClubSellerOption(this.GetSourceRectangle(5))
                });
                break;
            case MenuTabID.GingerIsland:
                this.options.AddRange(new BaseOption[]
                {
                    new QiSpecialOrderOption(this.GetSourceRectangle(0)),
                    new QiGemShopOption(this.GetSourceRectangle(1)),
                    new QiCatOption(this.GetSourceRectangle(2), this.helper),
                    new IslandTradeOption(this.GetSourceRectangle(3)),
                    new IslandResortOption(this.GetSourceRectangle(4)),
                    new VolcanoShopOption(this.GetSourceRectangle(5)),
                    new ForgeOption(this.GetSourceRectangle(6))
                });
                break;
            case MenuTabID.SVE:
                this.options.AddRange(new BaseOption[]
                {
                    new SophiaOption(this.GetSourceRectangle(0))
                });
                break;
            case MenuTabID.RSV:
                this.options.AddRange(new BaseOption[]
                {
                    new RSVQuestBoardOption(this.GetSourceRectangle(0)),
                    new RSVSpecialOrderOption(this.GetSourceRectangle(1)),
                    new IanOption(this.GetSourceRectangle(2)),
                    new PaulaOption(this.GetSourceRectangle(3)),
                    new LorenzoOption(this.GetSourceRectangle(4)),
                    new JericOption(this.GetSourceRectangle(5)),
                    new KimpoiOption(this.GetSourceRectangle(6)),
                    new PikaOption(this.GetSourceRectangle(7)),
                    new LolaOption(this.GetSourceRectangle(8)),
                    new NinjaBoardOption(this.GetSourceRectangle(10)),
                    new JoiOption(this.GetSourceRectangle(11)),
                    new MysticFalls1Option(this.GetSourceRectangle(12)),
                    new MysticFalls2Option(this.GetSourceRectangle(13)),
                    new MysticFalls3Option(this.GetSourceRectangle(14)),
                });
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddOptionSlots()
    {
        for (var i = 0; i < OptionsPerPage; i++) this.optionSlots.Add(new ClickableComponent(this.GetBoundsRectangle(i), ""));
    }

    private void DrawOption()
    {
        var spriteBatch = Game1.spriteBatch;
        for (var i = 0; i < OptionsPerPage; i++)
        {
            var optionsIndex = i + this.currentPage * OptionsPerPage;
            // 选项槽的位置和大小
            var bounds = this.optionSlots[i].bounds;
            // 根据缩放调整纹理绘制的位置和大小
            var size = (width: (int)(bounds.Width * this.optionScales[i]), height: (int)(bounds.Height * this.optionScales[i]));
            var position = (x: bounds.X + (bounds.Width - size.width) / 2, y: bounds.Y + (bounds.Height - size.height) / 2);
            // 如果选项绘制完成,则停止绘制
            if (optionsIndex >= this.options.Count) break;
            // 绘制纹理
            drawTextureBox(spriteBatch, ModEntry.Textures[this.currentMenuTabID], this.options[optionsIndex].SourceRect,
                position.x, position.y, size.width, size.height, Color.White);
            // 绘制标签
            var x = bounds.X + bounds.Width / 2;
            var y = bounds.Y + bounds.Height / 3 * 2;
            DrawHelper.DrawTab(x, y, Game1.smallFont, this.options[optionsIndex].Name, Align.Center);
        }
    }
}