/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.GUI;
using ArsVenefici.Framework.Spells;
using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using SpaceCore;
using ArsVenefici.Framework.Skill;
using SpaceShared.APIs;
using SpaceCore.Events;
using ArsVenefici.Framework.Spells.Effects;
using System.Reflection.PortableExecutable;

namespace ArsVenefici
{
    public class ModEntry : Mod
    {

        public static IModHelper helper;
        public ModConfig Config;

        public static IManaBarApi Mana;

        public SpellPartManager spellPartManager;
        public SpellPartIconManager spellPartIconManager;

        public Events eventsHandler;

        private static Texture2D ManaBg;
        private static Texture2D ManaFg;

        /// <summary>The active effects, spells, or projectiles which should be updated or drawn.</summary>
        public readonly IList<IActiveEffect> ActiveEffects = new List<IActiveEffect>();

        /// <remarks>This should only be accessed through <see cref="GetSpellBook"/> or <see cref="Extensions.GetSpellBook"/> to make sure an updated instance is retrieved.</remarks>
        private static readonly IDictionary<long, SpellBook> SpellBookCache = new Dictionary<long, SpellBook>();

        /// <summary>The ID of the event in which the player learns magic from the Wizard.</summary>
        //public static int LearnedMagicEventId { get; } = 90002;
        public int LearnedWizardryEventId { get; } = 9918172;

        /// <summary>The number of mana points gained per magic level.</summary>
        public int ManaPointsPerLevel { get; } = 100;

        /// <summary>Whether the current player learned wizardry.</summary>
        public bool LearnedWizardy => Game1.player?.eventsSeen?.Contains(LearnedWizardryEventId.ToString()) == true ? true : false;

        public static Skill Skill;
        public const string MsgCast = "HeyImAmethyst.ArsVenifici.Cast";
        public static Random RandomGen = new Random();

        public bool isSVEInstalled;

        public override void Entry(IModHelper helper)
        {
            ModEntry.helper = helper;

            InitializeClasses();

            LoadAssets();

            this.Config = this.Helper.ReadConfig<ModConfig>();
            SetUpEvents();

            CheckIfSVEIsInstalled();

            SpaceCore.Skills.RegisterSkill(ModEntry.Skill = new Skill(this));
        }

        /// <summary>
        /// Initializes the needed classes for the mod.
        /// </summary>
        private void InitializeClasses()
        {
            spellPartManager = new SpellPartManager(this);
            spellPartIconManager = new SpellPartIconManager(this);
            eventsHandler = new Events(this);
        }

        private static void LoadAssets()
        {
            ModEntry.ManaBg = helper.ModContent.Load<Texture2D>("assets/farmer/manabg.png");

            Color manaCol = new Color(0, 48, 255);
            ModEntry.ManaFg = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            ModEntry.ManaFg.SetData(new[] { manaCol });
        }

        /// <summary>
        /// Sets up the events for the mod.
        /// </summary>
        private void SetUpEvents()
        {
            helper.Events.Input.ButtonPressed += eventsHandler.OnButtonPressed;

            helper.Events.Display.RenderingHud += eventsHandler.OnRenderingHud;
            helper.Events.Display.RenderedHud += eventsHandler.OnRenderedHud;
            helper.Events.Display.MenuChanged += eventsHandler.OnMenuChanged;
            helper.Events.Display.RenderedWorld += eventsHandler.OnRenderedWorld;

            helper.Events.GameLoop.GameLaunched += eventsHandler.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += eventsHandler.OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += eventsHandler.OnUpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicking += eventsHandler.OnOneSecondUpdateTicking;

            helper.Events.Player.Warped += eventsHandler.OnWarped;

            SpaceEvents.OnItemEaten += eventsHandler.OnItemEaten;
            Networking.RegisterMessageHandler(MsgCast, eventsHandler.OnNetworkCast);
        }

        /// <summary>Fix the player's mana pool to match their skill level if needed.</summary>
        /// <param name="player">The player to fix.</param>
        /// <param name="overrideWizardryLevel">The wizardry skill level, or <c>null</c> to get it from the player.</param>
        public void FixManaPoolIfNeeded(Farmer player, int? overrideWizardryLevel = null)
        {
            // skip if player hasn't learned wizardry
            if (!LearnedWizardy && overrideWizardryLevel is not > 0)
                return;

            // get wizardry info
            int wizardryLevel = overrideWizardryLevel ?? player.GetCustomSkillLevel(Skill.WizardrySkillId);
            
            SpellBook spellBook = Game1.player.GetSpellBook();

            // fix mana pool
            {
                int expectedPoints = wizardryLevel * ManaPointsPerLevel;
                if (player.GetMaxMana() < expectedPoints)
                {
                    player.SetMaxMana(expectedPoints);
                    player.AddMana(expectedPoints);
                }
            }
        }

        /// <summary>Get a self-updating view of a player's magic metadata.</summary>
        /// <param name="player">The player whose spell book to get.</param>
        public static SpellBook GetSpellBook(Farmer player)
        {
            if (!ModEntry.SpellBookCache.TryGetValue(player.UniqueMultiplayerID, out SpellBook book) || !object.ReferenceEquals(player, book.Player))
                ModEntry.SpellBookCache[player.UniqueMultiplayerID] = book = new SpellBook(player);

            return book;
        }

        /// <summary>
        /// Checks if the mod Stardew Valley Expanded is currently installed.
        /// </summary>
        private void CheckIfSVEIsInstalled()
        {
            isSVEInstalled = Helper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP");
            Monitor.Log($"Satrdew Valley Expanded Sense Installed: {isSVEInstalled}", LogLevel.Trace);
        }
    }
}