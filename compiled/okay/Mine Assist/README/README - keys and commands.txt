Commands and parameters:
    UseItem: Directly use tool/weapon/items on toolbar. Tools almost all work well(fishing rod can cast). For weapons, sword works well but slingshot is somewhat complicated. Edible items and placeable items(including totem) work well, others are not tested.
        ItemName/Position: Required parameter. Indicate which item to use. Either by position or item name. If both are provided, Position will override ItemName. Since number of visible items is 12, the range of Posisiotn parameter should be 1~12. However, it's possible to set as 13~36. ItemName could be actual item name or "Weapon" to indicate any short weapon (not including slingshot or Scythe) or "Edible" to indicate any Edible items or basic tool name like "PickAxe" instead of "Copper PickAxe". If more than one item matches, the first one will be used.
        Condition:Optional parameter. If specify item by name, this parameter can add additional conditions to limit item selection. Possible values:
            "StaminaAtLeast <Stamina>"
            "StaminaAtMost <Stamina>"
            "HealthAtLeast <Health>"
            "HealthAtMost <Health>"
            "QualityAtLeast <Quality/UpgradeLevel/WeaponLevel>"
            "QualityAtMost <Quality/UpgradeLevel/WeaponLevel>"
            "PriceAtLeast <Price>"
            "PriceAtMost <Price>"
        Order:Optional parameter. For all items that satisfy Condition, indicate which one to use. Possible values:
            "StaminaLowest"
            "StaminaHighest"
            "HelthLowest"
            "HealthHighest"
            "QualityLowest"
            "QualityHighest"
            "PriceLowest"
            "PriceHighest"
        IsContinuous: Optional parameter. Some items can be used continuously while holding the action key (pickaxe, axe, scythe, edible items). By default, it's false.
        SpecialAction: Optional parameter. Invoke special action of weapon if possible. By default, it's false.
    Craft: Craft items
        ItemName: Required parameter.
        ToPosition: Optional parameter. If this parameter is set, the crafted item will placed to the fixed position if possible. If there is an item in that position, the item will be place to another empty slot.
    SwitchMod: Perform switch among modes.
        ModeName: Required parameter. Mode name to switch.
    OpenMenu: Open Journal Menu
    Pause: Pause the game, only host can do it.

Both modifyKeys and keys can be one of these(whcih defined in SMAPI):
    None
    Back
    Tab
    Enter
    Pause
    CapsLock
    Kana
    Kanji
    Escape
    ImeConvert
    ImeNoConvert
    Space
    PageUp
    PageDown
    End
    Home
    Left
    Up
    Right
    Down
    Select
    Print
    Execute
    PrintScreen
    Insert
    Delete
    Help
    D0
    D1
    D2
    D3
    D4
    D5
    D6
    D7
    D8
    D9
    A
    B
    C
    D
    E
    F
    G
    H
    I
    J
    K
    L
    M
    N
    O
    P
    Q
    R
    S
    T
    U
    V
    W
    X
    Y
    Z
    LeftWindows
    RightWindows
    Apps
    Sleep
    NumPad0
    NumPad1
    NumPad2
    NumPad3
    NumPad4
    NumPad5
    NumPad6
    NumPad7
    NumPad8
    NumPad9
    Multiply
    Add
    Separator
    Subtract
    Decimal
    Divide
    F1
    F2
    F3
    F4
    F5
    F6
    F7
    F8
    F9
    F10
    F11
    F12
    F13
    F14
    F15
    F16
    F17
    F18
    F19
    F20
    F21
    F22
    F23
    F24
    NumLock
    Scroll
    LeftShift
    RightShift
    LeftControl
    RightControl
    LeftAlt
    RightAlt
    BrowserBack
    BrowserForward
    BrowserRefresh
    BrowserStop
    BrowserSearch
    BrowserFavorites
    BrowserHome
    VolumeMute
    VolumeDown
    VolumeUp
    MediaNextTrack
    MediaPreviousTrack
    MediaStop
    MediaPlayPause
    LaunchMail
    SelectMedia
    LaunchApplication1
    LaunchApplication2
    OemSemicolon
    OemPlus
    OemComma
    OemMinus
    OemPeriod
    OemQuestion
    OemTilde
    ChatPadGreen
    ChatPadOrange
    OemOpenBrackets
    OemPipe
    OemCloseBrackets
    OemQuotes
    Oem8
    OemBackslash
    ProcessKey
    OemCopy
    OemAuto
    OemEnlW
    Attn
    Crsel
    Exsel
    EraseEof
    Play
    Zoom
    Pa1
    OemClear
    MouseLeft
    MouseRight
    MouseMiddle
    MouseX1
    MouseX2
    DPadUp
    DPadDown
    DPadLeft
    DPadRight
    ControllerStart
    ControllerBack
    LeftStick
    RightStick
    LeftShoulder
    RightShoulder
    BigButton
    ControllerA
    ControllerB
    ControllerX
    ControllerY
    LeftThumbstickLeft
    RightTrigger
    LeftTrigger
    RightThumbstickUp
    RightThumbstickDown
    RightThumbstickRight
    RightThumbstickLeft
    LeftThumbstickUp
    LeftThumbstickDown
    LeftThumbstickRight


