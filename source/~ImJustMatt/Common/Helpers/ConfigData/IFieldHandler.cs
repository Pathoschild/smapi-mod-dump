/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using Common.Integrations.GenericModConfigMenu;
using StardewModdingAPI;

namespace Helpers.ConfigData
{
    internal interface IFieldHandler
    {
        bool CanHandle(IField field);
        object GetValue(object instance, IField field);
        void SetValue(object instance, IField field, object value);
        void CopyValue(IField field, object source, object target);
        void RegisterConfigOption(IManifest manifest, GenericModConfigMenuIntegration modConfigMenu, object instance, IField field);
    }
}