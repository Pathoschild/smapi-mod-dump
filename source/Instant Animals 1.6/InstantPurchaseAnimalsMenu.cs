/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/siweipancc/InstantAnimals
**
*************************************************/

using System;
using System.Collections.Generic;
using InstantAnimals.wrapper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.GameData.FarmAnimals;
using StardewValley.Menus;
using xTile.Dimensions;
using MRectangle = Microsoft.Xna.Framework.Rectangle;
using SObject = StardewValley.Object;

namespace InstantAnimals;

public sealed class InstantPurchaseAnimalsMenu : IClickableMenu
{
    // public const int region_okButton = 101;
    //
    // public const int region_doneNamingButton = 102;
    //
    // public const int region_randomButton = 103;
    //
    // public const int region_namingBox = 104;

    private static int _menuHeight = 320;

    private static int _menuWidth = 448;

    private readonly List<List<ClickableTextureComponent>> _animalsToPurchase = new();

    private readonly ClickableTextureComponent? _okButton;

    private readonly ClickableTextureComponent _doneNamingButton;

    private readonly ClickableTextureComponent _randomButton;

    private ClickableTextureComponent? _hovered;

    private readonly ClickableTextureComponent _upButton;

    private readonly ClickableTextureComponent _downButton;

    private bool _onFarm;

    private bool _currentDoNamingAnimal;

    private bool _freeze;

    private FarmAnimal? _animalBeingPurchased;

    private readonly TextBox _textBox;

    private readonly TextBoxEvent _e;

    private Building? _newAnimalHome;

    private int _priceOfAnimal;

    private const bool ReadOnly = false;

    private ulong _latestId;

    private readonly IDictionary<string, string> _strings;

    private readonly int[] _okBounds;

    private int _pageNum;

    private readonly int _pageMax;


    private readonly List<SObject> _stock;

    private int _friendship;
    private readonly IModHelper _helper;
    private readonly IDictionary<string, FarmAnimalData> _farmAnimalData;

    // ReSharper disable once NotAccessedField.Local
    private readonly ClickableComponent _textBoxCc;


    private bool _adults;

    public void SetAdult(bool x)
    {
        if (_adults == x)
        {
            return;
        }

        Logger.Info($"change _adults: from {_adults} to {x}");
        _adults = x;
        ToggleAgeTexture();
    }

    public InstantPurchaseAnimalsMenu(
        (List<SObject>, IDictionary<string, string>,
            IDictionary<string, FarmAnimalData>) p, IModHelper helper)
        : base(
            Game1.uiViewport.Width / 2 - _menuWidth / 2 - borderWidth * 2,
            (Game1.uiViewport.Height - _menuHeight - borderWidth * 2) / 4,
            _menuWidth + borderWidth * 2,
            _menuHeight + borderWidth)
    {
        (_stock, _strings, this._farmAnimalData) = p;
        _adults = true;
        height += 128;
        _pageMax = _stock.Count / 9;
        this._helper = helper;
        for (int j = 0; j <= _pageMax; j++)
        {
            List<ClickableTextureComponent> page = new List<ClickableTextureComponent>();
            for (int i = 0 + j * 9; i < Math.Min(_stock.Count, (j + 1) * 9); i++)
            {
                string name = _stock[i].Name;
                FarmAnimalData animalData = _farmAnimalData[name];
                var texture2D = LoadAgedTexture2D(animalData);
                page.Add(new ClickableTextureComponent(string.Concat(_stock[i].salePrice()),
                    new MRectangle(xPositionOnScreen + borderWidth * 2 + i % 3 * 75 * 2,
                        yPositionOnScreen + spaceToClearTopBorder +
                        borderWidth / 2 + i % 9 / 3 * 90, 64, 64), null, name, texture2D,
                    new MRectangle(1, 1, animalData.SpriteWidth, animalData.SpriteHeight), 2f,
                    _stock[i].Type == null)
                {
                    item = _stock[i],
                    myID = i % 9,
                    rightNeighborID = i % 9 % 3 == 2 ? -1 : i % 9 + 1,
                    leftNeighborID = i % 9 % 3 == 0 ? -1 : i % 9 - 1,
                    downNeighborID = i % 9 + 3,
                    upNeighborID = i % 9 - 3
                });
            }

            _animalsToPurchase.Add(page);
        }

        this._upButton = new ClickableTextureComponent(
            new MRectangle(xPositionOnScreen + width,
                yPositionOnScreen + borderWidth / 2, 44, 48), Game1.mouseCursors,
            new MRectangle(421, 459, 11, 12), 4f);
        this._downButton = new ClickableTextureComponent(
            new MRectangle(xPositionOnScreen + width,
                yPositionOnScreen + height - 4 - borderWidth / 2, 44, 48),
            Game1.mouseCursors, new MRectangle(421, 472, 11, 12), 4f);

        _okBounds = new[]
        {
            xPositionOnScreen + width + 64,
            yPositionOnScreen + height - 4 - borderWidth
        };
        this._okButton = new ClickableTextureComponent(
            new MRectangle(_okBounds[0], _okBounds[1], 64, 64), Game1.mouseCursors,
            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f)
        {
            myID = 101,
            upNeighborID = 103,
            leftNeighborID = 103
        };
        this._randomButton = new ClickableTextureComponent(
            new MRectangle(xPositionOnScreen + width + 51 + 64,
                Game1.uiViewport.Height / 2, 64, 64), Game1.mouseCursors,
            new MRectangle(381, 361, 10, 10), 4f)
        {
            myID = 103,
            downNeighborID = 101,
            rightNeighborID = 101
        };

        _menuHeight = 320;
        _menuWidth = 448;
        this._textBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor)
        {
            X = Game1.uiViewport.Width / 2 - 192,
            Y = Game1.uiViewport.Height / 2,
            Width = 256,
            Height = 192
        };
        this._e = TextBoxEnter;
        this._textBoxCc = new ClickableComponent(new MRectangle(this._textBox.X, this._textBox.Y, 192, 48),
            "")
        {
            myID = 104,
            rightNeighborID = 102,
            downNeighborID = 101
        };
        this._randomButton = new ClickableTextureComponent(
            new MRectangle(this._textBox.X + this._textBox.Width + 64 + 48 - 8,
                Game1.uiViewport.Height / 2 + 4, 64, 64), Game1.mouseCursors,
            new MRectangle(381, 361, 10, 10), 4f)
        {
            myID = 103,
            leftNeighborID = 102,
            downNeighborID = 101,
            rightNeighborID = 101
        };
        this._doneNamingButton = new ClickableTextureComponent(
            new MRectangle(this._textBox.X + this._textBox.Width + 32 + 4,
                Game1.uiViewport.Height / 2 - 8, 64, 64), Game1.mouseCursors,
            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
        {
            myID = 102,
            rightNeighborID = 103,
            leftNeighborID = 104,
            downNeighborID = 101
        };
        if (Game1.options.SnappyMenus)
        {
            populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }
    }

    /// <summary>
    /// 根据年龄加载不同的材质.
    /// </summary>
    /// <param name="animalData"></param>
    /// <returns></returns>
    private Texture2D LoadAgedTexture2D(FarmAnimalData animalData)
    {
        string texture = animalData.Texture;
        if (!_adults && animalData.BabyTexture != null)
        {
            if (animalData.BabyTexture == null)
            {
                Logger.Debug($"this animal: {animalData.DisplayName} has no BabyTexture");
            }
            else
            {
                texture = animalData.BabyTexture;
            }
        }

        Texture2D texture2D = Game1.content.Load<Texture2D>(texture);
        return texture2D;
    }

    public override bool shouldClampGamePadCursor()
    {
        return this._onFarm;
    }

    public override void snapToDefaultClickableComponent()
    {
        currentlySnappedComponent = getComponentWithID(0);
        this.snapCursorToCurrentSnappedComponent();
    }

    private void TextBoxEnter(TextBox sender)
    {
        if (!this._currentDoNamingAnimal)
        {
            return;
        }

        if (Game1.activeClickableMenu == null || Game1.activeClickableMenu is not InstantPurchaseAnimalsMenu)
        {
            this._textBox.OnEnterPressed -= this._e;
        }
        else if (sender.Text.Length >= 1)
        {
            if (this._animalBeingPurchased == null)
            {
                return;
            }

            if (Utility.areThereAnyOtherAnimalsWithThisName(sender.Text))
            {
                // 名称不可用
                Game1.showRedMessage(_strings["PurchaseAnimalsMenu.cs.11308"]);
                return;
            }


            GameLocation? indoorsValue = this._newAnimalHome?.indoors.Value;
            if (indoorsValue is not AnimalHouse animalHouse)
            {
                return;
            }

            this._textBox.OnEnterPressed -= this._e;
            this._animalBeingPurchased.Name = sender.Text;
            this._animalBeingPurchased.displayName = sender.Text;
            // this._animalBeingPurchased.displayType = this._animalBeingPurchased.Name;
            this._animalBeingPurchased.home = this._newAnimalHome;
            this._animalBeingPurchased.setRandomPosition(this._animalBeingPurchased?.home?.indoors.Value);
            animalHouse.animals.Add(this._animalBeingPurchased!.myID.Value,
                this._animalBeingPurchased);
            animalHouse.animalsThatLiveHere.Add(this._animalBeingPurchased
                .myID
                .Value);
            this._newAnimalHome = null;
            this._currentDoNamingAnimal = false;
            Game1.player.Money -= this._priceOfAnimal;
            this.SetUpForReturnAfterPurchasingAnimal();
        }
    }

    private void SetUpForReturnAfterPurchasingAnimal()
    {
        {
            this._okButton!.bounds.X = xPositionOnScreen + width + 4;
            Game1.displayHUD = true;
            Game1.displayFarmer = true;
            this._freeze = false;
            this._textBox.OnEnterPressed -= this._e;
            this._textBox.Selected = false;
            Game1.viewportFreeze = false;
            exitThisMenu();
            Game1.player.forceCanMove();
            this._freeze = false;
        }
    }

    // ReSharper disable PossibleLossOfFraction
    private void SetUpForAnimalPlacement()
    {
        Game1.currentLocation.cleanupBeforePlayerExit();
        Game1.displayFarmer = false;
        Game1.currentLocation.resetForPlayerEntry();
        Game1.currentLocation.cleanupBeforePlayerExit();
        this._onFarm = true;
        this._freeze = false;
        this._okButton!.bounds.X = Game1.uiViewport.Width - 128;
        this._okButton.bounds.Y = Game1.uiViewport.Height - 128;
        Game1.displayHUD = false;
        Game1.viewportFreeze = true;
        Game1.viewport.Location =
            new Location((int)(Game1.player.Tile.X * Game1.tileSize - Game1.viewport.Width / 2),
                (int)(Game1.player.Tile.Y * Game1.tileSize - Game1.viewport.Height / 2));
        Game1.panScreen(0, 0);
    }

    private void SetUpForReturnToShopMenu()
    {
        this._freeze = false;
        Game1.displayFarmer = true;
        {
            this._onFarm = false;
            Game1.player.viewingLocation.Value = null;
            this._okButton!.bounds.X = _okBounds[0];
            this._okButton.bounds.Y = _okBounds[1];
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            this._currentDoNamingAnimal = false;
            this._textBox.OnEnterPressed -= this._e;
            this._textBox.Selected = false;
            if (Game1.options.SnappyMenus)
            {
                this.snapToDefaultClickableComponent();
            }
        }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (Game1.IsFading() || this._freeze)
        {
            return;
        }

        if (this._okButton != null && this._okButton.containsPoint(x, y) && this.readyToClose())
        {
            if (this._onFarm)
            {
                this.SetUpForReturnToShopMenu();
                Game1.playSound("smallSelect");
            }
            else
            {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }
        }

        if (this._onFarm)
        {
            Vector2 clickTile =
                new Vector2((int)((Utility.ModifyCoordinateFromUIScale(x) + Game1.viewport.X) / 64f),
                    (int)((Utility.ModifyCoordinateFromUIScale(y) + Game1.viewport.Y) / 64f));
            Building selection = Game1.RequireLocation<Farm>("Farm").getBuildingAt(clickTile);
            if (selection != null && !this._currentDoNamingAnimal)
            {
                if (this._animalBeingPurchased == null)
                {
                    throw new Exception("_animalBeingPurchased should not be null");
                }

                // check live in type requirement
                bool canLiveIn =
                    selection.GetData().ValidOccupantTypes
                        .Contains(this._animalBeingPurchased.buildingTypeILiveIn.Value);
                if (canLiveIn)
                {
                    // is that full ?
                    if ((selection.indoors.Value as AnimalHouse)!.isFull())
                    {
                        Logger.Info($"selected location is full: {selection.indoors.Value.DisplayName}");
                        Game1.showRedMessage(_strings["PurchaseAnimalsMenu.cs.11321"]);
                    }
                    else if (this._animalBeingPurchased.type.Value != "2")
                    {
                        this._currentDoNamingAnimal = true;
                        this._newAnimalHome = selection;
                        this._animalBeingPurchased.makeSound();
                        this._textBox.OnEnterPressed += this._e;
                        this._textBox.Text = this._animalBeingPurchased.displayName;
                        Game1.keyboardDispatcher.Subscriber = this._textBox;
                        if (Game1.options.SnappyMenus)
                        {
                            currentlySnappedComponent = getComponentWithID(104);
                            this.snapCursorToCurrentSnappedComponent();
                        }
                    }
                    else if (Game1.player.Money >= this._priceOfAnimal)
                    {
                        this._newAnimalHome = selection;
                        this._animalBeingPurchased.home = this._newAnimalHome;
                        this._animalBeingPurchased.setRandomPosition(this._animalBeingPurchased.home.indoors.Value);
                        (this._newAnimalHome.indoors.Value as AnimalHouse)!.animals.Add(
                            this._animalBeingPurchased.myID.Value, this._animalBeingPurchased);
                        (this._newAnimalHome.indoors.Value as AnimalHouse)!.animalsThatLiveHere.Add(
                            this._animalBeingPurchased.myID.Value);
                        this._newAnimalHome = null;
                        this._currentDoNamingAnimal = false;
                        this._animalBeingPurchased.makeSound();
                        Game1.player.Money -= this._priceOfAnimal;
                        // 非常好！我马上送小{0}去她的新家。
                        var msg = string.Format(_strings["PurchaseAnimalsMenu.cs.11314"],
                            this._animalBeingPurchased.displayType);
                        Game1.addHUDMessage(new HUDMessage(
                            msg, 3500f));
                    }
                    else if (Game1.player.Money < this._priceOfAnimal)
                    {
                        Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                    }

                    return;
                }

                // not failed  cannot live in.
                Logger.Info(
                    $"animal: {_animalBeingPurchased!.Name} cannot live in : {selection.indoors.Value.DisplayName}");
                Game1.showRedMessage(string.Format(_strings["PurchaseAnimalsMenu.cs.11326"],
                    this._animalBeingPurchased.displayType));
                return;
            }

            if (this._currentDoNamingAnimal)
            {
                if (this._doneNamingButton.containsPoint(x, y))
                {
                    this.TextBoxEnter(this._textBox);
                    Game1.playSound("smallSelect");
                }
                else if (this._currentDoNamingAnimal && this._randomButton.containsPoint(x, y))
                {
                    if (this._animalBeingPurchased == null)
                    {
                        throw new Exception("_animalBeingPurchased should not be null");
                    }

                    this._animalBeingPurchased.Name = Dialogue.randomName();
                    this._animalBeingPurchased.displayName = this._animalBeingPurchased.Name;
                    this._textBox.Text = this._animalBeingPurchased.displayName;
                    this._randomButton.scale = this._randomButton.baseScale;
                    Game1.playSound("drumkit6");
                }

                this._textBox.Update();
            }
        }

        if (this._upButton.containsPoint(x, y) && _pageNum > 0)
        {
            _pageNum--;
            if (Game1.options.SnappyMenus)
            {
                populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }

            return;
        }

        if (this._downButton.containsPoint(x, y) && _pageNum < _pageMax)
        {
            _pageNum++;
            if (Game1.options.SnappyMenus)
            {
                populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }

            return;
        }

        foreach (ClickableTextureComponent c in this._animalsToPurchase[_pageNum])
        {
            if (!ReadOnly && c.containsPoint(x, y) && (c.item as SObject)?.Type == null)
            {
                int price = c.item.salePrice();
                if (Game1.player.Money >= price)
                {
                    SetUpForAnimalPlacement();
                    Game1.playSound("smallSelect");
                    this._onFarm = true;
                    while (this._animalBeingPurchased == null ||
                           !this._animalBeingPurchased.type.Value.Equals(c.hoverText))
                    {
                        this._animalBeingPurchased =
                            new FarmAnimal(c.hoverText, GetNewId(), Game1.player.UniqueMultiplayerID);
                    }

                    if (_adults)
                    {
                        MakeAdult(this._animalBeingPurchased);
                    }

                    this._animalBeingPurchased.friendshipTowardFarmer.Value = _friendship * 200;
                    this._priceOfAnimal = price;
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage(_strings["PurchaseAnimalsMenu.cs.11325"], 3500f));
                }
            }
        }
    }

    public override void receiveScrollWheelAction(int direction)
    {
        base.receiveScrollWheelAction(direction);
        if (!this._onFarm && !Game1.dialogueUp && !Game1.IsFading())
        {
            if (direction > 0 && _friendship < 5)
            {
                _friendship++;
            }
            else if (direction < 0 && _friendship > 0)
            {
                _friendship--;
            }
        }
    }

    public override bool overrideSnappyMenuCursorMovementBan()
    {
        if (this._onFarm)
        {
            return !this._currentDoNamingAnimal;
        }

        return false;
    }

    public override void receiveGamePadButton(Buttons b)
    {
        base.receiveGamePadButton(b);
        if (b != Buttons.B || Game1.globalFade || !this._onFarm || !this._currentDoNamingAnimal)
        {
            return;
        }

        this.SetUpForReturnToShopMenu();
        Game1.playSound("smallSelect");
    }

    public override void receiveKeyPress(Keys key)
    {
        while (true)
        {
            if (Game1.globalFade || this._freeze)
            {
                return;
            }

            if (!Game1.globalFade && this._onFarm)
            {
                if (!this._currentDoNamingAnimal)
                {
                    if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose() &&
                        !Game1.IsFading())
                    {
                        this.SetUpForReturnToShopMenu();
                    }
                    else if (!Game1.options.SnappyMenus)
                    {
                        if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                        {
                            Game1.panScreen(0, 4);
                        }
                        else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                        {
                            Game1.panScreen(4, 0);
                        }
                        else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                        {
                            Game1.panScreen(0, -4);
                        }
                        else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                        {
                            Game1.panScreen(-4, 0);
                        }
                    }
                }
                else if (Game1.options.SnappyMenus)
                {
                    if (!this._textBox.Selected &&
                        Game1.options.doesInputListContain(Game1.options.menuButton, key))
                    {
                        this.SetUpForReturnToShopMenu();
                        Game1.playSound("smallSelect");
                    }
                    else if (!this._textBox.Selected ||
                             !Game1.options.doesInputListContain(Game1.options.menuButton, key))
                    {
                        continue;
                    }
                }
            }
            else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !Game1.IsFading())
            {
                if (this.readyToClose())
                {
                    Game1.player.forceCanMove();
                    Game1.exitActiveMenu();
                    Game1.playSound("bigDeSelect");
                }
            }
            else if (Game1.options.SnappyMenus)
            {
                continue;
            }

            break;
        }
    }

    public override void update(GameTime time)
    {
        base.update(time);
        if (!this._onFarm || this._currentDoNamingAnimal)
        {
            return;
        }

        int mouseX = Game1.getOldMouseX(ui_scale: false) + Game1.viewport.X;
        int mouseY = Game1.getOldMouseY(ui_scale: false) + Game1.viewport.Y;
        if (mouseX - Game1.viewport.X < 64)
        {
            Game1.panScreen(-8, 0);
        }
        else if (mouseX - (Game1.viewport.X + Game1.viewport.Width) >= -64)
        {
            Game1.panScreen(8, 0);
        }

        if (mouseY - Game1.viewport.Y < 64)
        {
            Game1.panScreen(0, -8);
        }
        else if (mouseY - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
        {
            Game1.panScreen(0, 8);
        }

        Keys[] pressedKeys = Game1.oldKBState.GetPressedKeys();
        foreach (Keys key in pressedKeys)
        {
            this.receiveKeyPress(key);
        }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
        this._hovered = null;
        if (Game1.IsFading() || this._freeze)
        {
            return;
        }

        this._upButton.scale = this._upButton.containsPoint(x, y)
            ? Math.Min(4.5f, this._upButton.scale + 0.25f)
            : Math.Max(4f, this._upButton.scale - 0.25f);

        this._downButton.scale = this._downButton.containsPoint(x, y)
            ? Math.Min(4.5f, this._downButton.scale + 0.25f)
            : Math.Max(4f, this._downButton.scale - 0.25f);

        if (this._okButton != null)
        {
            this._okButton.scale = this._okButton.containsPoint(x, y)
                ? Math.Min(1.1f, this._okButton.scale + 0.05f)
                : Math.Max(1f, this._okButton.scale - 0.05f);
        }

        if (this._onFarm)
        {
            if (!this._currentDoNamingAnimal)
            {
                Vector2 clickTile =
                    new Vector2((int)((Utility.ModifyCoordinateFromUIScale(x) + Game1.viewport.X) / 64f),
                        (int)((Utility.ModifyCoordinateFromUIScale(y) + Game1.viewport.Y) / 64f));

                Farm f = Game1.RequireLocation<Farm>("Farm");
                foreach (Building building in f.buildings)
                {
                    building.color = Color.White;
                }

                Building selection = f.getBuildingAt(clickTile);
                if (selection != null)
                {
                    if (this._animalBeingPurchased == null)
                    {
                        throw new Exception("_animalBeingPurchased should not be null");
                    }

                    if (selection.buildingType.Value.Contains(this._animalBeingPurchased.buildingTypeILiveIn
                            .Value) && !(selection.indoors.Value as AnimalHouse)!.isFull())
                    {
                        selection.color = Color.LightGreen * 0.8f;
                    }
                    else
                    {
                        selection.color = Color.Red * 0.8f;
                    }
                }
            }

            // if (this._doneNamingButton != null)
            // {
            this._doneNamingButton.scale = this._doneNamingButton.containsPoint(x, y)
                ? Math.Min(1.1f, this._doneNamingButton.scale + 0.05f)
                : Math.Max(1f, this._doneNamingButton.scale - 0.05f);
            // }

            this._randomButton.tryHover(x, y, 0.5f);
            return;
        }

        foreach (ClickableTextureComponent c in this._animalsToPurchase[_pageNum])
        {
            if (c.containsPoint(x, y))
            {
                c.scale = Math.Min(c.scale + 0.05f, c.baseScale < 3 ? 3.1f : 4.1f);

                this._hovered = c;
            }
            else
            {
                c.scale = Math.Max(c.baseScale < 3 ? 3f : 4f, c.scale - 0.025f);
            }
        }
    }


    public override void draw(SpriteBatch b)
    {
        if (!this._onFarm && !Game1.dialogueUp && !Game1.IsFading())
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            if (_adults)
            {
                SpriteText.drawStringWithScrollBackground(b, "Adult " + _strings["PurchaseAnimalsMenu.cs.11354"],
                    xPositionOnScreen + 96, yPositionOnScreen);
            }
            else
            {
                SpriteText.drawStringWithScrollBackground(b, "Baby " + _strings["PurchaseAnimalsMenu.cs.11354"],
                    xPositionOnScreen + 96, yPositionOnScreen);
            }

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height,
                speaker: false, drawOnlyBox: true);
            Game1.dayTimeMoneyBox.drawMoneyBox(b);
            foreach (ClickableTextureComponent c in this._animalsToPurchase[_pageNum])
            {
                c.draw(b, (c.item as SObject)?.Type != null ? Color.Black * 0.4f : Color.White,
                    0.87f);
            }

            if (_pageNum != 0)
            {
                this._upButton.draw(b);
            }

            if (_pageMax > 0 && _pageNum < _pageMax)
            {
                this._downButton.draw(b);
            }

            for (int hearts = 0; hearts < 5; hearts++)
            {
                b.Draw(Game1.mouseCursors,
                    new Vector2(xPositionOnScreen + borderWidth * 2 + hearts * 64,
                        yPositionOnScreen + height - 64 - borderWidth / 2),
                    new MRectangle(211, 428, 7, 6),
                    _friendship <= hearts ? Color.Black * 0.35f : Color.White, 0f, Vector2.Zero, 6f,
                    SpriteEffects.None, 0.88f);
            }
        }
        else if (!Game1.IsFading() && this._onFarm)
        {
            if (this._animalBeingPurchased == null)
            {
                throw new Exception("_animalBeingPurchased should not be null");
            }

            // "选择一个{0}来安置你的新{1}"
            string s = string.Format(_strings["PurchaseAnimalsMenu.cs.11355"],
                this._animalBeingPurchased.displayHouse, this._animalBeingPurchased.displayType);
            SpriteText.drawStringWithScrollBackground(b, s,
                Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16);
            if (this._currentDoNamingAnimal)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                Game1.drawDialogueBox(Game1.uiViewport.Width / 2 - 256, Game1.uiViewport.Height / 2 - 192 - 32, 512,
                    192, speaker: false, drawOnlyBox: true);
                // "给你的新动物取名字： "
                Utility.drawTextWithShadow(b, _strings["PurchaseAnimalsMenu.cs.11357"], Game1.dialogueFont,
                    new Vector2(Game1.uiViewport.Width / 2 - 256 + 32 + 8, Game1.uiViewport.Height / 2 - 128 + 8),
                    Game1.textColor);
                this._textBox.Draw(b);
                this._doneNamingButton.draw(b);
                this._randomButton.draw(b);
            }
        }

        if (!Game1.IsFading() && this._okButton != null)
        {
            this._okButton.draw(b);
        }

        if (this._hovered != null)
        {
            if ((this._hovered.item as SObject)?.Type != null)
            {
                drawHoverText(b,
                    Game1.parseText((this._hovered.item as SObject)?.Type, Game1.dialogueFont, 320),
                    Game1.dialogueFont);
            }
            else
            {
                string displayName = GetAnimalTitle(this._hovered.hoverText);
                SpriteText.drawStringWithScrollBackground(b, displayName,
                    xPositionOnScreen + spaceToClearSideBorder + 64,
                    yPositionOnScreen + height + -32 + spaceToClearTopBorder / 2 + 8,
                    "Truffle Pig");
                SpriteText.drawStringWithScrollBackground(b,
                    "$" + string.Format(_strings["LoadGameMenu.cs.11020"], this._hovered.item.salePrice()),
                    xPositionOnScreen + spaceToClearSideBorder + 128,
                    yPositionOnScreen + height + 64 + spaceToClearTopBorder / 2 + 8,
                    "$99999999g", (Game1.player.Money >= this._hovered.item.salePrice()) ? 1f : 0.5f);
                string description = GetAnimalDescription(this._hovered.hoverText);
                drawHoverText(b, Game1.parseText(description, Game1.smallFont, 320), Game1.smallFont,
                    0, 0, -1, displayName);
            }
        }

        Game1.mouseCursorTransparency = 1f;
        drawMouse(b);
    }

    private long GetNewId()
    {
        ulong seqNum = ((this._latestId & 0xFF) + 1) & 0xFF;
        ulong nodeId = (ulong)Game1.player.UniqueMultiplayerID;
        nodeId = (nodeId >> 32) ^ (nodeId & 0xFFFFFFFFu);
        nodeId = ((nodeId >> 16) ^ (nodeId & 0xFFFF)) & 0xFFFF;
        ulong timestamp = (ulong)(DateTime.Now.Ticks / 10000);
        this._latestId = (timestamp << 24) | (nodeId << 8) | seqNum;
        return (long)this._latestId;
    }

    private void ToggleAgeTexture()
    {
        for (int page = 0; page <= _pageMax; page++)
        {
            int idxInPage = 0;
            for (int i = page * 9; i < Math.Min(_stock.Count, (page + 1) * 9); i++)
            {
                ClickableTextureComponent textureComponent = _animalsToPurchase[page][idxInPage++];
                string name = _stock[i].Name;
                FarmAnimalData animalData = _farmAnimalData[name];
                Texture2D texture2D = LoadAgedTexture2D(animalData);
                this._helper.Reflection.GetField<Texture2D>(textureComponent, "texture").SetValue(texture2D);
            }
        }

        if (!Game1.options.SnappyMenus)
        {
            return;
        }

        populateClickableComponentList();
        this.snapToDefaultClickableComponent();
    }

    private static void MakeAdult(FarmAnimal animal)
    {
        if (animal.isAdult())
        {
            return;
        }

        int daysToMature = animal.GetAnimalData().DaysToMature;

        animal.age.Value = daysToMature;

        animal.Sprite.LoadTexture("Animals\\" + animal.type.Value);
        // if (animal.type.Value.Contains("Sheep"))
        // {
        //     animal.currentProduce.Value = "O(440)";
        // }

        animal.daysSinceLastLay.Value = 99;
        animal.reload(animal.home);
    }

    private string GetAnimalTitle(string name)
    {
        string displayName = FarmAnimal.GetDisplayName(name);
        return displayName ?? name;
    }

    private string GetAnimalDescription(string name)
    {
        string description = FarmAnimal.GetShopDescription(name);
        FarmAnimalData data = _farmAnimalData[name];
        if (description != null)
        {
            return description;
        }

        string house = data.House;
        string living = house switch
        {
            "Coop" => _strings["PurchaseAnimalsMenu.cs.11335"],
            "Barn" => _strings["PurchaseAnimalsMenu.cs.11344"],
            _ => ""
        };

        description = GetAnimalTitle(name) +
                      Environment.NewLine + living;

        return description;
    }
}