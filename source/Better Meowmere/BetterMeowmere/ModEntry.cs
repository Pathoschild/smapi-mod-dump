/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/BetterMeowmere
**
*************************************************/

using System.Collections.Generic;
using System.Collections;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using StardewValley.GameData.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using xTile.Dimensions;

namespace BetterMeowmere;

public class ModEntry
    : Mod
{
    private ModConfig config;
    private string[] AcceptedValues = { "All", "Some", "None" };
    public override void Entry(IModHelper helper)
    {
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        helper.Events.Content.AssetRequested += this.AssetRequested;
        helper.Events.GameLoop.GameLaunched += this.GameLaunched;
        helper.Events.GameLoop.Saving += this.Saving;
        helper.Events.GameLoop.DayStarted += this.DayStarted;

        try
        {
            this.config = helper.ReadConfig<ModConfig>();
            if (AcceptedValues.Contains(this.config.ProjectileSound) == false)
            {
                this.config.ProjectileSound = "All";
            }
        }
        catch
        {
            this.config = new ModConfig();
            this.Monitor.Log("Failed to parse config file, default options will be used.", LogLevel.Warn);
        }

        MeowmereProjectile.Initialise(this.Helper, this.config);
    }

    private void GameLaunched(object sender, GameLaunchedEventArgs e) 
    { 
        this.BuildConfigMenu();
    }

    private void BuildConfigMenu()
    {
        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) 
        { 
            return; 
        }

        void ApplyChanges()
        {
            this.Helper.GameContent.InvalidateCache("Data\\Weapons");
            this.Helper.WriteConfig(this.config);
        }

        // register mod
        configMenu.Register(
            mod: this.ModManifest,
            reset: () => this.config = new ModConfig(),
            save: () => ApplyChanges()
        );
        configMenu.AddTextOption(
                ModManifest,
                name: () => "Projectile Sounds",
                tooltip: () => "\"None\" No sounds.\n\"Some\" No sound when bouncing off walls.\n\"All\" Meow!",
                allowedValues: AcceptedValues,
                getValue: () => config.ProjectileSound,
                setValue: value => config.ProjectileSound = value
            );
        configMenu.AddBoolOption(
                ModManifest,
                name: () => "Projectile Is Secondary Attack",
                tooltip: () => "Shoot the cat projectile using the secondary attack (true) or the primary attack (false).",
                getValue: () => config.ProjectileIsSecondaryAttack,
                setValue: value => config.ProjectileIsSecondaryAttack = value
            );
        configMenu.AddBoolOption(
                ModManifest,
                name: () => "Buff Attack",
                tooltip: () => "Increase the damage of the meowmere blade (and projectile to a lesser extent). Only change this if you're fully committed to having an overpowered sword!",
                getValue: () => config.BuffAttack,
                setValue: value => config.BuffAttack = value
            );

    }

    private void ApplyDamageChanges(bool revert)
    {
        this.Helper.GameContent.InvalidateCache("Data\\Weapons");
        var inventory = Game1.player.Items;
        if (inventory != null)
        {
            foreach( var item in inventory)
            {
                if (item is MeleeWeapon && item.Name == "Meowmere")
                {
                    var meowmere = item as MeleeWeapon;
                    if (meowmere != null && revert == true)
                    {
                        meowmere.minDamage.Value = 20;
                        meowmere.maxDamage.Value = 20;
                    }
                    else if (meowmere != null && revert == false)
                    {
                        meowmere.minDamage.Value = 120;
                        meowmere.maxDamage.Value = 120;
                    }                    
                }
            }
        }
    }

    private void Saving(object sender, SavingEventArgs e)
    {
        ApplyDamageChanges(true);
    }

    private void DayStarted(object sender, DayStartedEventArgs e)
    {
        if (config.BuffAttack == true)
        {
            ApplyDamageChanges(false);
        }
        else
        {
            ApplyDamageChanges(true);
        }
    }

    private void AssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("Data\\Weapons"))
        {
            if(this.config.BuffAttack == true)
            {
                e.Edit(asset =>
                {
                    var data = asset.Data as Dictionary<string, WeaponData>;
                    if (data != null)
                    {
                        data["65"].MinDamage = 120;
                        data["65"].MaxDamage = 120;
                    }
                });
            }
            else
            {
                e.Edit(asset =>
                {
                    var data = asset.Data as Dictionary<string, WeaponData>;
                    if (data != null)
                    {
                        data["65"].MinDamage = 20;
                        data["65"].MaxDamage = 20;
                    }
                });
            }

        }
    }

    private void ShootProjectile(Farmer user)
    {
        int bounces = 4;
        Random random = new Random();
        var soundtoplay = "terraria_meowmere";
        if (config.ProjectileSound != "None")
        {
            Game1.currentLocation.playSound(soundtoplay);
        }        
        string bouncesound = soundtoplay;

        if (this.config.ProjectileSound != "All")
        {
            bouncesound = "";
        }

        Vector2 velocity1 = TranslateVector(new Vector2(0, 10), user.FacingDirection);
        Vector2 startPos1 = TranslateVector(new Vector2(0, 96), user.FacingDirection);
        int damage = this.config.BuffAttack == true ? random.Next(50, 70) : random.Next(20, 40);
        Game1.currentLocation.projectiles.Add(new MeowmereProjectile(damage, velocity1.X, velocity1.Y, user.Position + new Vector2(0, -64) + startPos1, bounces, 6, bouncesound, user.currentLocation, user));
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (Context.IsWorldReady == false || Context.IsPlayerFree == false)
            return;

        var user = Game1.player;
        if (user.CurrentTool?.Name != "Meowmere")
        {
            return;
        }

        if (e.Button.IsUseToolButton() == true && this.config.ProjectileIsSecondaryAttack == false)
        {
            ShootProjectile(user);
        }

        else if (this.config.ProjectileIsSecondaryAttack == true)
        {
            if ((!e.Button.IsActionButton()) || (MeleeWeapon.defenseCooldown > 0))
            {
                return;
            }
            ShootProjectile(user);
        }

    }

    public static Vector2 TranslateVector(Vector2 vector, int facingDirection)
    {
        float outx = vector.X;
        float outy = vector.Y;
        switch (facingDirection)
        {
            case 2:
                break;
            case 3:
                outx = -vector.Y;
                outy = vector.X;
                break;
            case 0:
                outx = -vector.X;
                outy = -vector.Y;
                break;
            case 1:
                outx = vector.Y;
                outy = -vector.X;
                break;
        }
        return new Vector2(outx, outy);
    }
}