/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Reflection;

namespace RadialMenu
{
    internal class KeybindActivator
    {
        private readonly IInputHelper inputHelper;
        private readonly FieldInfo currentInputStateField;
        private readonly MethodInfo overrideButtonMethod;

        public KeybindActivator(IInputHelper inputHelper)
        {
            this.inputHelper = inputHelper;
            // We can't use ReflectionHelper here because SMAPI blocks reflection on itself.
            currentInputStateField = inputHelper.GetType().GetField(
                "CurrentInputState",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
            // We don't really need the input state object at construction time, but since we don't
            // have direct access to its type, we have to use the object to get it.
            var currentInputState = GetCurrentInputState();
            overrideButtonMethod = currentInputState.GetType().GetMethod(
                "OverrideButton",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                [typeof(SButton), typeof(bool)])!;
        }

        public void Activate(Keybind keybind)
        {
            if (!keybind.IsBound)
            {
                return;
            }
            var inputState = GetCurrentInputState();
            // Overrides are transient, because the input state itself is transient and recreated on
            // every frame. Therefore we don't need to remove the override; rather, if we wanted to
            // "hold" the button down, we'd need to keep doing this on every subsequent frame.
            foreach (var button in keybind.Buttons)
            {
                overrideButtonMethod.Invoke(inputState, [button, true]);
            }
        }

        private object GetCurrentInputState()
        {
            return ((Delegate)(currentInputStateField.GetValue(inputHelper)!)).DynamicInvoke()!;
        }
    }
}
