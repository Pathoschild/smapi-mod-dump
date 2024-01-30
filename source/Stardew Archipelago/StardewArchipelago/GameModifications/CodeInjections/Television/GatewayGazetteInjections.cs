/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.GameModifications.EntranceRandomizer;
using StardewArchipelago.Serialization;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace StardewArchipelago.GameModifications.CodeInjections.Television
{
    internal class GatewayGazetteInjections
    {
        private const int GAZETTE_CHANNEL = 1492;
        private const string GAZETTE_INTRO =
            "Welcome back to the Gateway Gazette! The bi-weekly show where brave adventurers explore the strange topology of the world around us!";
        private const string GAZETTE_EPISODE =
            "On today's episode, our agent {0} has traversed from {1} and discovered... {2}! They came back safe and sound to share this wonderful knowledge with us!";

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static EntranceManager _entranceManager;
        private static ArchipelagoStateDto _state;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, EntranceManager entranceManager, ArchipelagoStateDto state)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _entranceManager = entranceManager;
            _state = state;
        }

        private static IReflectedField<int> GetCurrentChannelField(TV tv)
        {
            // private int currentChannel;
            return _modHelper.Reflection.GetField<int>(tv, "currentChannel");
        }

        // public virtual void selectChannel(Farmer who, string answer)
        public static void SelectChannel_SelectGatewayGazetteChannel_Postfix(TV __instance, Farmer who, string answer)
        {
            try
            {
                var channel = answer.Split(' ')[0];
                if (channel != TVInjections.GATEWAY_GAZETTE_KEY)
                {
                    return;
                }

                var currentChannelField = GetCurrentChannelField(__instance);
                currentChannelField.SetValue(GAZETTE_CHANNEL);

                SetGazetteScreen(__instance);

                Game1.drawObjectDialogue(Game1.parseText(GAZETTE_INTRO));
                Game1.afterDialogues = __instance.proceedToNextScene;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SelectChannel_SelectGatewayGazetteChannel_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public virtual void proceedToNextScene()
        public static void ProceedToNextScene_GatewayGazette_Postfix(TV __instance)
        {
            try
            {
                var currentChannelField = GetCurrentChannelField(__instance);
                if (currentChannelField.GetValue() != GAZETTE_CHANNEL)
                {
                    return;
                }

                // private TemporaryAnimatedSprite screenOverlay;
                var screenOverlayField = _modHelper.Reflection.GetField<TemporaryAnimatedSprite>(__instance, "screenOverlay");
                if (screenOverlayField.GetValue() == null)
                {
                    PlayGazetteEpisode(__instance);
                    screenOverlayField.SetValue(new TemporaryAnimatedSprite { alpha = 1E-07f });
                }
                else
                {
                    __instance.turnOffTV();
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ProceedToNextScene_GatewayGazette_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void PlayGazetteEpisode(TV __instance)
        {
            SetGazetteScreen(__instance);
            var random = new Random((int)(Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed));
            var agentName = Community.AllNames[random.Next(Community.AllNames.Length)];
            var entrancesNotChecked = _entranceManager.ModifiedEntrances.Keys.Where(x => !_state.EntrancesTraversed.Contains(x)).ToArray();
            if (!entrancesNotChecked.Any())
            {
                entrancesNotChecked = _entranceManager.ModifiedEntrances.Keys.ToArray();
            }
            var entranceToReveal = entrancesNotChecked[random.Next(entrancesNotChecked.Length)];
            var friendlyEntranceName = GetFriendlyMapName(entranceToReveal);
            var destinationInternalName = _entranceManager.ModifiedEntrances[entranceToReveal];
            var destinationFriendlyName = GetFriendlyDestinationName(destinationInternalName);
            Game1.drawObjectDialogue(Game1.parseText(string.Format(GAZETTE_EPISODE, agentName, friendlyEntranceName, destinationFriendlyName)));
            Game1.afterDialogues = __instance.proceedToNextScene;
        }

        private static string GetFriendlyDestinationName(string destinationInternalName)
        {
            var friendlyDestinationName = destinationInternalName.Split(EntranceManager.TRANSITIONAL_STRING).Last().Split(" ").Last().Split("|").First();
            return GetFriendlyMapName(friendlyDestinationName);
        }

        private static string GetFriendlyMapName(string mapName)
        {
            var friendlyMapName = mapName.Replace("Custom_", "");
            for (var i = friendlyMapName.Length - 1; i > 0; i--)
            {
                if (char.IsUpper(friendlyMapName[i]) && char.IsLetter(friendlyMapName[i]) && !char.IsWhiteSpace(friendlyMapName[i - 1]))
                {
                    friendlyMapName = friendlyMapName.Insert(i, " ");
                }
                if (friendlyMapName[i] == '|')
                {
                    do
                    {
                        friendlyMapName = friendlyMapName.Remove(i, 1);
                    } while (i < friendlyMapName.Length && char.IsDigit(friendlyMapName[i]));
                }
            }
            return friendlyMapName;
        }

        private static void SetGazetteScreen(TV __instance)
        {
            // private TemporaryAnimatedSprite screen;
            var screenField = _modHelper.Reflection.GetField<TemporaryAnimatedSprite>(__instance, "screen");
            var tvSprite = CreateGatewayGazetteTvSprite(__instance);
            screenField.SetValue(tvSprite);
        }

        private static TemporaryAnimatedSprite CreateGatewayGazetteTvSprite(TV tv)
        {
            var screenRectangle = new Rectangle(413, 305, 42, 28);
            var interval = 150f;
            var length = 2;
            var loops = 999999;
            var screenPosition = tv.getScreenPosition();
            var flicker = false;
            var flipped = true;
            var layerDepth = (float)((tv.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06);
            var fade = 0.0f;
            var scale = tv.getScreenSizeModifier();
            var tvSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", screenRectangle, interval, length, loops, screenPosition, flicker, flipped, layerDepth, fade,
                Color.White, scale, 0.0f, 0.0f, 0.0f);
            return tvSprite;
        }
    }
}
