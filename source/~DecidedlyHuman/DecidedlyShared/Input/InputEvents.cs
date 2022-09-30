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
    private static Action<string> logCallback;
    private static List<KeyWatch> keyWatches;
    private static List<ButtonWatch> gamepadWatches;
    private static KeyboardState currentKeyboardState;
    private static KeyboardState previousKeyboardState;
    private static GamePadState currentGamepadState;
    private static GamePadState previousGamepadState;
    private static MouseState currentMouseState;
    private static MouseState previousMouseState;
    private static bool isInitialised = false;

    public static void InitInput(IModHelper helper, Action<string> logCallback)
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

        previousGamepadState = currentGamepadState;
        previousKeyboardState = currentKeyboardState;
    }

    private static void InvokeAction(KeyWatch watch)
    {
        if (watch.Callback != null)
            watch.Callback.Invoke();
        else
            logCallback.Invoke($"The callback for {watch.Key.ToString()} was null.");
    }

    private static void InvokeAction(ButtonWatch watch)
    {
        if (watch.Callback != null)
            watch.Callback.Invoke();
        else
            logCallback.Invoke($"The callback for {watch.Button.ToString()} was null.");
    }

    public static bool RegisterEvent(KeyPressType type, Keys key, Action? callback, Action<string>? logCallback)
    {
        if (callback == null)
            return false;

        keyWatches.Add(new KeyWatch(key, type, callback, logCallback));

        return true;
    }

    public static bool RegisterEvent(KeyPressType type, Buttons button, Action? callback, Action<string>? logCallback)
    {
        if (callback == null)
            return false;

        gamepadWatches.Add(new ButtonWatch(button, type, callback, logCallback));

        return true;
    }
}

public enum KeyPressType
{
    Press,
    Hold,
    Released
}
