/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using DecidedlyShared.Logging;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace DecidedlyShared.Input;

public static class InputEvents
{
    private static Action<string, LogLevel> logCallback;
    private static List<KeyWatch> keyWatches; // TODO: Fix watch duplication bug.
    private static List<ButtonWatch> gamepadWatches;
    private static List<MouseWatch> mouseWatches;
    private static KeyboardState currentKeyboardState;
    private static KeyboardState previousKeyboardState;
    private static GamePadState currentGamepadState;
    private static GamePadState previousGamepadState;
    private static MouseState currentMouseState;
    private static MouseState previousMouseState;
    private static bool isInitialised = false;

    public static void InitInput(IModHelper helper, Action<string, LogLevel> logCallback)
    {
        if (!isInitialised)
        {
            keyWatches = new List<KeyWatch>();
            gamepadWatches = new List<ButtonWatch>();
            helper.Events.GameLoop.UpdateTicked += GameLoopOnUpdateTicked;
            InputEvents.logCallback = logCallback;
            InputEvents.isInitialised = true;
        }
    }

    private static void GameLoopOnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!isInitialised)
            return;

        currentGamepadState = Game1.input.GetGamePadState();
        currentKeyboardState = Game1.input.GetKeyboardState();
        currentMouseState = Game1.input.GetMouseState();

        for (int i = 0; i < keyWatches.Count; i++)
        {
            switch (keyWatches[i].Type)
            {
                case KeyPressType.Hold:
                    if (currentKeyboardState[keyWatches[i].Key] == KeyState.Down &&
                        previousKeyboardState[keyWatches[i].Key] == KeyState.Down)
                        InvokeAction(keyWatches[i]);
                    break;
                case KeyPressType.Press:
                    if (currentKeyboardState[keyWatches[i].Key] == KeyState.Down &&
                        previousKeyboardState[keyWatches[i].Key] == KeyState.Up)
                        InvokeAction(keyWatches[i]);
                    break;
                case KeyPressType.Released:
                    if (currentKeyboardState[keyWatches[i].Key] == KeyState.Up &&
                        previousKeyboardState[keyWatches[i].Key] == KeyState.Down)
                        InvokeAction(keyWatches[i]);
                    break;
            }
        }

        for (int i = 0; i < gamepadWatches.Count; i++)
        {
            switch (gamepadWatches[i].Type)
            {
                case KeyPressType.Hold:
                    if (currentGamepadState.IsButtonDown(gamepadWatches[i].Button) &&
                        previousGamepadState.IsButtonDown(gamepadWatches[i].Button))
                        InvokeAction(gamepadWatches[i]);
                    break;
                case KeyPressType.Press:
                    if (currentGamepadState.IsButtonDown(gamepadWatches[i].Button) &&
                        previousGamepadState.IsButtonUp(gamepadWatches[i].Button))
                        InvokeAction(gamepadWatches[i]);
                    break;
                case KeyPressType.Released:
                    if (currentGamepadState.IsButtonUp(gamepadWatches[i].Button) &&
                        previousGamepadState.IsButtonDown(gamepadWatches[i].Button))
                        InvokeAction(gamepadWatches[i]);
                    break;
            }
        }

        // for (int i = 0; i < mouseWatches.Count; i++)
        // {
        //     switch (mouseWatches[i].Type)
        //     {
        //         case KeyPressType.Hold:
        //             switch (mouseWatches[i].button)
        //             {
        //                 case MouseButton.Left:
        //                     if (previousMouseState.LeftButton == ButtonState.Pressed &&
        //                         currentMouseState.LeftButton == ButtonState.Pressed)
        //                         InvokeAction(gamepadWatches[i]);
        //
        //                     break;
        //                 case MouseButton.Right:
        //                     if (previousMouseState.RightButton == ButtonState.Pressed &&
        //                         currentMouseState.RightButton == ButtonState.Pressed)
        //                         InvokeAction(gamepadWatches[i]);
        //
        //                     break;
        //                 case MouseButton.Middle:
        //                     if (previousMouseState.MiddleButton == ButtonState.Pressed &&
        //                         currentMouseState.MiddleButton == ButtonState.Pressed)
        //                         InvokeAction(gamepadWatches[i]);
        //
        //                     break;
        //             }
        //
        //             break;
        //         case KeyPressType.Press:
        //             switch (mouseWatches[i].button)
        //             {
        //                 case MouseButton.Left:
        //                     if (previousMouseState.LeftButton == ButtonState.Released &&
        //                         currentMouseState.LeftButton == ButtonState.Pressed)
        //                         InvokeAction(gamepadWatches[i]);
        //
        //                     break;
        //                 case MouseButton.Right:
        //                     if (previousMouseState.RightButton == ButtonState.Released &&
        //                         currentMouseState.RightButton == ButtonState.Pressed)
        //                         InvokeAction(gamepadWatches[i]);
        //
        //                     break;
        //                 case MouseButton.Middle:
        //                     if (previousMouseState.MiddleButton == ButtonState.Released &&
        //                         currentMouseState.MiddleButton == ButtonState.Pressed)
        //                         InvokeAction(gamepadWatches[i]);
        //
        //                     break;
        //             }
        //
        //             break;
        //         case KeyPressType.Released:
        //             switch (mouseWatches[i].button)
        //             {
        //                 case MouseButton.Left:
        //                     if (previousMouseState.LeftButton == ButtonState.Pressed &&
        //                         currentMouseState.LeftButton == ButtonState.Released)
        //                         InvokeAction(gamepadWatches[i]);
        //
        //                     break;
        //                 case MouseButton.Right:
        //                     if (previousMouseState.RightButton == ButtonState.Pressed &&
        //                         currentMouseState.RightButton == ButtonState.Released)
        //                         InvokeAction(gamepadWatches[i]);
        //
        //                     break;
        //                 case MouseButton.Middle:
        //                     if (previousMouseState.MiddleButton == ButtonState.Pressed &&
        //                         currentMouseState.MiddleButton == ButtonState.Released)
        //                         InvokeAction(gamepadWatches[i]);
        //
        //                     break;
        //             }
        //
        //             break;
        //     }
        // }

        previousGamepadState = currentGamepadState;
        previousKeyboardState = currentKeyboardState;
        previousMouseState = currentMouseState;
    }

    private static void InvokeAction(KeyWatch watch)
    {
        if (watch.Callback != null)
            watch.Callback.Invoke();
        else
            logCallback.Invoke($"The callback for {watch.Key.ToString()} was null.", LogLevel.Error);
    }

    private static void InvokeAction(ButtonWatch watch)
    {
        if (watch.Callback != null)
            watch.Callback.Invoke();
        else
            logCallback.Invoke($"The callback for {watch.Button.ToString()} was null.", LogLevel.Error);
    }

    public static bool RegisterEvent(KeyPressType type, Keys key, Action? callback,
        Action<string, LogLevel>? logCallback)
    {
        if (callback == null)
            return false;

        keyWatches.Add(new KeyWatch(key, type, callback, logCallback));

        return true;
    }

    public static bool RegisterEvent(KeyPressType type, Buttons button, Action? callback,
        Action<string, LogLevel>? logCallback)
    {
        if (callback == null)
            return false;

        gamepadWatches.Add(new ButtonWatch(button, type, callback, logCallback));

        return true;
    }

    public static bool RegisterEvent(KeyPressType type, MouseButton button, Action? callback,
        Action<string, LogLevel>? logCallback)
    {
        if (callback == null)
            return false;

        mouseWatches.Add(new MouseWatch(button, type, callback, logCallback));

        return true;
    }

    public static bool RegisterEvent(KeyPressType type, SButton button, Action? callback,
        Action<string, LogLevel>? logCallback)
    {
        bool isKeyboard, isGamepad;

        isKeyboard = button.TryGetKeyboard(out Keys key);
        isGamepad = button.TryGetController(out Buttons gamepadButton);

        if (isKeyboard)
            RegisterEvent(type, key, callback, logCallback);
        if (isGamepad)
            RegisterEvent(type, gamepadButton, callback, logCallback);

        logCallback.Invoke("Could not parse input as either keyboard or gamepad button.", LogLevel.Error);
        return false;
    }
}

public enum KeyPressType
{
    Press,
    Hold,
    Released
}
