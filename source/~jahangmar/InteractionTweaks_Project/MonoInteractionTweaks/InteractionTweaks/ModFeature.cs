//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using StardewModdingAPI;
namespace InteractionTweaks
{
    public abstract class ModFeature
    {
        protected static IMonitor Monitor;
        protected static IModHelper Helper;
        protected static InteractionTweaksConfig Config;

        public static void Init(ModEntry modEntry)
        {
            Monitor = modEntry.Monitor;
            Helper = modEntry.Helper;
            Config = modEntry.GetConfig();
        }

        public static void Enable()
        {

        }

        public static void Disable()
        {

        }

        public static string GetTrans(string key) => Helper.Translation.Get(key);
        public static string GetTrans(string key, object tokens) => Helper.Translation.Get(key, tokens);
    }
}
