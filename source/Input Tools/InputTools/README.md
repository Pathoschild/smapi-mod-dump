**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/sagittaeri/StardewValleyMods**

----

# Stardew Valley Input Tools

A Stardew Valley SMAPI mod to provide an array of tools to make it easier to
handle inputs across mouse, keyboard and controller.

## For mod users

This mod is mostly meant for other modders as a modding tool, but it does do one
thing for users:

* Mod Config: immediately force hides the mouse cursor when controller is used to improve
  gamepad experience (cursor reappears when moved with mouse or gamepad right stick). Defaults yes.

### Compatibility

* Works with Stardew Valley 1.5 on Linux/Mac/Windows.
* Works with keyboard, mouse, and most gamepads
* Works in single player and multiplayer.
* No known incompatibilities.

### Installation

Follow the usual installation proceedure for SMAPI mods:
1. Install [SMAPI](https://smapi.io)
2. Download the latest realease of this mod and unzip it into the `Mods` directory
3. Run the game using SMAPI

## For mod developers

This mod provides an API for the following:
* Allow event handlers (`ButtonPairPressed += MyDelegate`) and per tick polling (`IsButtonPairPressed(CtrlSpace)`) to check if a button pair (e.g. ctrl+space) is Pressed, Held or Released
* Direct methods and event handlers for Confirm, Cancel, Alt, Menu, MoveRight, MoveDown, MoveLeft and MoveUp input actions supporting mouse, keyboard and gamepad
* A way define and use custom input actions (e.g. Jump, NavigateDown, CustomConfirm, etc) and assign it to mouse, keyboard and gamepad
* Check which input device was most recently used and get event updates whenever it changes
* Get a string from the player using an automatially-created and cleaned up Virtual Keyboard
* Get a custom keybinding from the user, both single-button and button-pair
* Get the corrected location for item placement tile when gamepad is used
* Group input event handling and per-tick polling into an "Input Layer" which can be turned off and on
* In more complex systems, create multiple Input Layers and dictate their behaviour e.g. whether or not to allow input events from a layer to propagate to the layer below

For the full API, see [`IInputToolsAPI.cs`](https://github.com/sagittaeri/StardewValleyMods/blob/main/InputTools/IInputToolsAPI.cs).

### Concepts
#### Button Pairs
Button pairs are when two buttons are pushed at the same time and are stored as `Tuple<SButton, SButton>`. For example:
```cs
// Define ctrl+space
Tuple<SButton, SButton> ctrlSpace = new Tuple<SButton, SButton>(SButton.LeftControl, SButton.Space);

this.InputToolsAPI.Global.ButtonPairPressed +=  new EventHandler<Tuple<SButton, SButton>>((s, e) =>
{
    // This line is entered whenever any button pair is pressed
    if (e == ctrlSpace)
    {
        // ctrl+space is pressed - do something cool
    }
});
```
Note that this defines `Ctrl+Space` and not `Space+Ctrl` - the order in the tuple has to match the keystroke.

#### Actions
Actions are events that are triggered by an input or a set of inputs. For example, the Jump action could be triggered by "Space" on the keyboard and "X" on the gamepad and so on. A good way to implement cross-device support to your mod is to use Actions. For example:
```cs
// Add custom action "Jump"
this.InputToolsAPI.RegisterAction("Jump", SButton.Space, SButton.ControllerX);
this.InputToolsAPI.RegisterAction("Jump", new Tuple<SButton, SButton>(SButton.MouseLeft, SButton.MouseRight));

this.InputToolsAPI.Global.ActionPressed +=  new EventHandler<string>((s, e) =>
{
    // This line is entered whenever any custom action is pressed
    if (e == "Jump")
    {
        // Do jump!
    }
});
```
This says `Jump` is triggered in three ways; (1) `Space` on keyboard; (2) `X` on gamepad; and (3) `Left+Right` mouse clicks. Then it uses an event handler to listen for all custom action input events.

#### Input Layers and the Stack
This is for advanced usage of Input Tools. If you're only using it for small stuff, you can just use the default layer `InputToolsAPI.Global` to do all your input work.

An Input Layer is an object which contains all the input events and per-tick polling methods. You can deactivate a layer to pause input handling which can be useful especially if you require multiple different sets of input handling, many of them conflicting. For example, the Escape button can have different uses depending on the state of the game, so you can create an Input Layer for each state for input handling.

Additionally, the layers are grouped into a "Stack" i.e. they are like UI popups: when they are created, they are added on top of the stack. Each layer can determine whether or not to allow input events to pass down to the layer below in the stack, which is useful for UI popups where you don't want the Escape button to close all the popups at once, just the one on top.

An example:

```cs
// Define an Input Layer for the initial state
IInputToolsAPI.IInputLayer normalLayer = this.InputToolsAPI.CreateLayer("Normal");
normalLayer.ButtonPressed += new EventHandler<SButton>((s, e) =>
{
    // In this context, s = "Normal", and e is an SButton enum
    if (e == SButton.Escape)
    {
        // Perform action in normal state
    }
});

normalLayer.LayerUpdateTicked += new EventHandler<UpdateTickedEventArgs>((s, e) =>
{
    // This is like a normal UpdateTicked event, except it's specific for this layer
    if (normalLayer.IsButtonPairPressed(new Tuple<SButton, SButton>(SButton.LeftShift, SButton.Enter)))
    {
        // When LShift+Enter is pressed, switch to the Popup layer
        IInputToolsAPI.GetLayer("Popup").SetBlock(IInputToolsAPI.BlockBehavior.Block);
        IInputToolsAPI.GetLayer("Popup").SetActive(true);

        // While Popup layer is active and blocking, Normal layer will no longer receive button events
        // Additionally, normalLayer.IsButtonPairPressed() etc will always return false since the Popup layer
        // above is preventing input events from reaching the Normal layer
    }
};

// Define the Input Layer for the popup. Note that layers are always created on top of the stack.
// The default layer, InputToolsAPI.Global, is the exception, which is always above the entire stack,
// which means if InputToolsAPI.Global is set to Block, no other layer will receive input events
IInputToolsAPI.IInputLayer popupLayer = this.InputToolsAPI.CreateLayer("Popup");
popupLayer.ButtonPressed += new EventHandler<SButton>((s, e) =>
{
    // In this context, s = "Popup", and e is an SButton enum
    if (e == SButton.Escape)
    {
        // Deactivate this layer and allow inputs to pass below to Normal layer again
        IInputToolsAPI.GetLayer(s).SetBlock(IInputToolsAPI.BlockBehavior.PassBelow);
        IInputToolsAPI.GetLayer(s).SetActive(false);
    }
});

// By default, created layers are active and will block inputs, so turn them off and allow inputs to pass down
popupLayer.SetBlock(IInputToolsAPI.BlockBehavior.PassBelow);
popupLayer.SetActive(false);

```

### Mod Integration

To start integrating this mod into your own, you'll first need to define the interface of the whole API or just functions you wish to use. An example for a simple interface with only one function:
```cs
public interface IInputToolsAPI
{
    public IInputLayer Global { get; }
    public interface IInputLayer
    {
        public Vector2 GetPlacementTile();
    }
}
```
Note: `IInputLayer Global` object is the default Input Layer, and for most use-cases you'll need it.

If you want to implement the entire API instead, just copy this file onto your project: [`IInputToolsAPI.cs`](https://github.com/sagittaeri/StardewValleyMods/blob/main/InputTools/IInputToolsAPI.cs)

* *Pros of implementing just the functions you need:* less chances of the integration breaking due to API changes.
* *Pros of implementing the whole API:* Intellisense can help you discover functions you need easier.

The following method inside your `ModEntry.cs` is recommended for a soft integration:
```cs
private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
{
    try
    {
        this.InputToolsAPI = this.Helper.ModRegistry.GetApi<InputTools.IInputToolsAPI>("Sagittaeri.InputTools");
    }
    catch (Exception exception)
    {
        this.Monitor.Log($"Failed to load Sagittaeri.InputTools. Reason: {exception.Message}", LogLevel.Error);
    }
    if (this.InputToolsAPI != null)
    {
        this.Monitor.Log("Loaded Sagittaeri.InputTools successfully - controller will be supported", LogLevel.Debug);
    }
}
```
This will allow any integration issues to fail gracefully, and you only need to check that `InputToolsAPI != null` before using the tool.

For general information on how to use another mod's API in your mod,
see the [Mod Integration](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations)
page on the Stardew Valley Wiki.
