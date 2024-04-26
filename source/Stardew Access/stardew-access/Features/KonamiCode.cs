/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using System.Collections.Generic;
using System.Linq; // Add this to use SequenceEqual

namespace stardew_access.Features;

using Translation;
using StardewValley;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;

internal class KonamiCode : FeatureBase
{
    private int _tracker = 0;
    private static KonamiCode? instance;
    public new static KonamiCode Instance
    {
        get
        {
            instance ??= new KonamiCode();
            return instance;
        }
    }

    public override void Update(object? sender, UpdateTickedEventArgs e)
    {}

    public override bool OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        bool cheater = CheckKeyPress(e.Button);
        if (cheater)
        {
            EnableCheats();
            _tracker = 0;
        }
        return cheater;
    }

    public bool CheckKeyPress(SButton button)
    {
        // Increment or reset the tracker based on the current button pressed and the current value of _tracker
        switch (button)
        {
            case SButton.Up when _tracker < 2:
                _tracker++;
                break;
            case SButton.Down when (_tracker == 2 || _tracker == 3):
                _tracker++;
                break;
            case SButton.Left when (_tracker == 4 || _tracker == 6): // Adding 0 for Left as a starting point for flexibility
                _tracker++;
                break;
            case SButton.Right when (_tracker == 5 || _tracker == 7):
                _tracker++;
                break;
            case SButton.B when _tracker == 8:
                _tracker++;
                break;
            case SButton.A when _tracker == 9:
                _tracker++;
                break;
            case SButton.Enter when _tracker == 10:
                _tracker++;
                break;
            case SButton.Escape when _tracker == 11:
                _tracker++;
                MainClass.ModHelper!.Input.Suppress(button);
                return true;
            default:
                _tracker = 0; // Reset to 0 for any invalid sequence
                break;
        }
        return false;
    }

    private static void EnableCheats()
    {
        Program.enableCheats = true;
        Game1.player.Money = 1000000;
        Game1.player.maxHealth = 100000;
        Game1.player.maxStamina.Value = 100000;
        Game1.player.health = Game1.player.maxHealth;
        Game1.player.stamina = Game1.player.maxStamina.Value;
        Game1.player.temporarilyInvincible = true;
        Game1.player.temporaryInvincibilityTimer = -1000000000;
        
        Game1.player.clearBackpack();
        Game1.player.increaseBackpackSize(36);
        Game1.player.hasRustyKey = false;
        Game1.player.hasSkullKey = true;
        Game1.player.hasSpecialCharm = true;
        Game1.player.hasDarkTalisman = true;
        Game1.player.hasMagicInk = true;
        Game1.player.hasClubCard = true;
        Game1.player.canUnderstandDwarves = true;
        Game1.player.hasMagnifyingGlass = true;

        Game1.player.addItemToInventory(ItemRegistry.Create("(T)IridiumAxe"));
        Game1.player.addItemToInventory(ItemRegistry.Create("(T)IridiumHoe"));
        Game1.player.addItemToInventory(ItemRegistry.Create("(T)IridiumWateringCan"));
        Game1.player.addItemToInventory(ItemRegistry.Create("(T)IridiumPickaxe"));
        Game1.player.addItemToInventory(ItemRegistry.Create("(T)AdvancedIridiumRod"));
        Game1.player.addItemToInventory(ItemRegistry.Create("(T)IridiumPan"));
        Game1.player.addItemToInventory(ItemRegistry.Create("(W)66"));
        Game1.player.addItemToInventory(ItemRegistry.Create("(O)TentKit"));
        Game1.player.addItemToInventory(ItemRegistry.Create("(W)62"));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)288", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)388", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)709", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)390", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)330", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)334", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)335", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)336", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)337", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)910", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)338", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)428", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)152", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)153", 999));
        Game1.player.addItemToInventory(ItemRegistry.Create("(o)789"));

        Game1.player.Equip(ItemRegistry.Create<Ring>("(O)527"), Game1.player.leftRing);
        Game1.player.Equip(ItemRegistry.Create<Ring>("(O)527"), Game1.player.rightRing);

        WaitAndSpeak();
    }

    private static async void WaitAndSpeak()
    {
        await Task.Delay(500);
        if (Game1.soundBank != null) Game1.playSound("bobber_progress", 500);
        MainClass.ScreenReader.Say(Translator.Instance.Translate("feature-speak_cheater"), true);
    }
}