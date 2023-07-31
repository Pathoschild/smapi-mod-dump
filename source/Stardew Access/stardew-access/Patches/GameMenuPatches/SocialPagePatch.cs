/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class SocialPagePatch
    {
        internal static string socialPageQuery = "";

        internal static void DrawPatch(SocialPage __instance, List<ClickableTextureComponent> ___sprites, int ___slotPosition, List<string> ___kidsNames)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                for (int i = ___slotPosition; i < ___slotPosition + 5; i++)
                {
                    if (i >= ___sprites.Count)
                        continue;

                    if (__instance.names[i] is string && NarrateNPCDetails(__instance, i, ___kidsNames, x, y))
                    {
                        return;
                    }
                    else if (__instance.names[i] is long && NarrateFarmerDetails(__instance, i, ___sprites, x, y))
                    {
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateNPCDetails(SocialPage __instance, int i, List<string> ___kidsNames, int x, int y)
        {
            if (!__instance.characterSlots[i].bounds.Contains(x, y))
                return false;
            
            string name = $"{__instance.names[i] as string}";
            int heartLevel = Game1.player.getFriendshipHeartLevelForNPC(name);
            bool datable = SocialPage.isDatable(name);
            Friendship friendship = __instance.getFriendship(name);
            int giftsThisWeek = friendship.GiftsThisWeek;
            bool hasTalked = Game1.player.hasPlayerTalkedToNPC(name);
            bool spouse = friendship.IsMarried();
            bool housemate = spouse && SocialPage.isRoommateOfAnyone(name);
            ___kidsNames.Add("Robin");
            ___kidsNames.Add("Pierre");
            ___kidsNames.Add("Caroline");
            ___kidsNames.Add("Jodi");
            ___kidsNames.Add("Kent");
            ___kidsNames.Add("George");
            ___kidsNames.Add("Evelyn");
            ___kidsNames.Add("Demetrius");

            string toSpeak = $"{name}";

            if (!hasTalked)
            {
                toSpeak = $"{toSpeak}, not talked yet";
            }


            if (datable | housemate)
            {
                string text2 = (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.pt) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635") : ((__instance.getGender(name) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').First() : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').Last());
                if (housemate)
                {
                    text2 = Game1.content.LoadString("Strings\\StringsFromCSFiles:Housemate");
                }
                else if (spouse)
                {
                    text2 = ((__instance.getGender(name) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637"));
                }
                else if (__instance.isMarriedToAnyone(name))
                {
                    text2 = ((__instance.getGender(name) == 0) ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC") : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC"));
                }
                else if (!Game1.player.isMarried() && friendship.IsDating())
                {
                    text2 = ((__instance.getGender(name) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640"));
                }
                else if (__instance.getFriendship(name).IsDivorced())
                {
                    text2 = ((__instance.getGender(name) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643"));
                }

                toSpeak = $"{toSpeak}, {text2}";
            }

            if (!__instance.getFriendship(name).IsMarried() && ___kidsNames.Contains(name))
            {
                toSpeak = $"{toSpeak}, married";
            }

            if (spouse)
            {
                toSpeak = $"{toSpeak}, spouse";
            }
            else if (friendship.IsDating())
            {
                toSpeak = $"{toSpeak}, dating";
            }

            toSpeak = $"{toSpeak}, {heartLevel} hearts, {giftsThisWeek} gifts given this week.";

            if (socialPageQuery != toSpeak)
            {
                socialPageQuery = toSpeak;
                MainClass.ScreenReader.Say(toSpeak, true);
            }
            return true;
        }

        private static bool NarrateFarmerDetails(SocialPage __instance, int i, List<ClickableTextureComponent> ___sprites, int x, int y)
        {
            long farmerID = (long)__instance.names[i];
            Farmer farmer = Game1.getFarmerMaybeOffline(farmerID);
            if (farmer == null)
                return false;

            int gender = (!farmer.IsMale) ? 1 : 0;
            ClickableTextureComponent clickableTextureComponent = ___sprites[i];
            if (!clickableTextureComponent.containsPoint(x, y))
                return false;
            
            Friendship friendship = Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, farmerID);
            bool spouse = friendship.IsMarried();
            string toSpeak = "";

            string text2 = (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.pt) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635") : ((gender == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').First() : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').Last());
            if (spouse)
            {
                text2 = ((gender == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637"));
            }
            else if (farmer.isMarried() && !farmer.hasRoommate())
            {
                text2 = ((gender == 0) ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC") : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC"));
            }
            else if (!Game1.player.isMarried() && friendship.IsDating())
            {
                text2 = ((gender == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640"));
            }
            else if (friendship.IsDivorced())
            {
                text2 = ((gender == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643"));
            }

            toSpeak = $"{farmer.displayName}, {text2}";

            if (socialPageQuery != toSpeak)
            {
                socialPageQuery = toSpeak;
                MainClass.ScreenReader.Say(toSpeak, true);
            }
            return true;
        }

        internal static void Cleanup()
        {
            socialPageQuery = "";
        }
    }
}
