/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundsPatcher.Utility
{
    public class Utilities
    {
        private const string SoundCues = "archaeo/axchop/leafrustle/axe/babblingBrook/backpackIN/barrelBreak/clam_tone/bigDeSelect/bigDrums/bigSelect/boulderBreak/boulderCrack/breakingGlass/breathin/breathout/busDriveOff/cacklingWitch/cameraNoise/cancel/cast/cat/cavedrip/objectiveComplete/clank/clubhit/clubloop/achievement/clubSmash/clubswipe/cluck/coin/communityCenter/cow/cowboy_dead/cowboy_explosion/Cowboy_Footstep/cowboy_gopher/Cowboy_gunshot/Cowboy_monsterDie/cowboy_monsterhit/cowboy_powerup/cowboy_gunload/Cowboy_Secret/cracklingFire/crafting/crickets/cricketsAmbient/croak/crow/crystal/cut/daggerswipe/batFlap/batScreech/death/debuffHit/debuffSpell/dialogueCharacter/dialogueCharacterClose/dirtyHit/discoverMineral/distantTrain/dog_bark/dog_pant/dogs/dogWhining/doorClose/doorCreak/doorCreakReverse/doorOpen/dropItemInWater/drumkit0/drumkit1/drumkit2/drumkit3/drumkit4/drumkit5/drumkit6/Duck/Duggy/dustMeep/throw/dwoop/explosion/dwop/bubbles/EarthMine/fall_day_ambient/fastReel/fireball/fishBite/fishEscape/FishHit/fishingRodBend/fishSlap/flameSpell/flameSpellHit/flute/flybuzzing/FrostMine/furnace/fuse/ghost/give_gift/glug/goat/grassyStep/stoneStep/thudStep/eat/grunt/gulp/purchaseRepeat/hammer/harvest/healSound/heavyEngine/hitEnemy/hoeHit/Hospital_Ambient/jingle1/junimoMeep1/keyboardTyping/Lava_Ambient/LavaMine/Majestic/toyPiano/Meteorite/Milking/minecartLoop/money/moneyDial/monsterdead/mouseClick/newArtifact/newRecipe/newRecord/New Snow/nightTime/ocean/openBox/openChest/ow/owl/parrot/parry/phone/Pickup_Coin15/pickUpItem/pool_ambient/potterySmash/purchase/purchaseClick/questcomplete/rabbit/rain/rainsound/reward/roadnoise/robotBLASTOFF/robotSoundEffects/rockGolemDie/rockGolemHit/rockGolemSpawn/rooster/sandyStep/seeds/scissors/seagulls/secret1/select/sell/serpentDie/serpentHit/shadowDie/shadowHit/shadowpeep/shiny4/sheep/Ship/boop/pig/shwip/fallDown/SinWave/sipTea/skeletonHit/skeletonStep/skeletonDie/slime/slimedead/slimeHit/slingshot/slosh/slowReel/smallSelect/snowyStep/SpringBirds/spring_day_ambient/spring_night_ambient/springsongs/stairsdown/stardrop/crit/stoneCrack/stumpCrack/summer_day_ambient/swordswipe/throwDownITem/thunder/thunder_small/tinyWhip/button1/toolCharge/toolSwap/pullItemFromWater/trainLoop/getNewSpecialItem/trainWhistle/trashcan/trashcanlid/treecrack/treethud/UFO/Upper_Ambient/wand/warrior/wateringCan/waterSlosh/whistle/wind/winter_day_ambient/woodWhack/woodyHit/woodyStep/bob/yoba";
        private const string SongCues = "50s/AbigailFlute/AbigailFluteDuet/aerobics/breezy/bugLevelLoop/caldera/Cavern/christmasTheme/Cloth/CloudCountry/cowboy_boss/cowboy_outlawsong/Cowboy_OVERWORLD/Cowboy_singing/Cowboy_undead/crane_game/crane_game_fast/junimoKart/junimoKart_ghostMusic/junimoKart_mushroomMusic/junimoKart_slimeMusic/junimoKart_whaleMusic/Crystal Bells/desolate/distantBanjo/echos/elliottPiano/EmilyDance/EmilyDream/EmilyTheme/end_credits/event1/event2/fall1/fall2/fall3/fallFest/fieldofficeTentMusic/FlowerDance/FrogCave/grandpas_theme/gusviolin/harveys_theme_jazz/heavy/honkytonky/Icicles/IslandMusic/jaunty/jojaOfficeSoundscape/junimoStarSong/kindadumbautumn/libraryTheme/MainTheme/MarlonsTheme/marnieShop/mermaidSong/moonlightJellies/movie_classic/movie_nature/movie_wumbus/movieTheater/movieTheaterAfter/musicboxsong/Near The Planet Core/night_market/Of Dwarves/Overcast/PIRATE_THEME/PIRATE_THEME(muffled)/playful/poppy/ragtime/sad_kid/sadpiano/Saloon1/sam_acoustic1/sam_acoustic2/sampractice/Secret Gnomes/SettlingIn/shaneTheme/shimmeringbastion/spaceMusic/spirits_eve/spring1/spring2/spring3/springtown/starshoot/submarine_song/summer1/summer2/summer3/SunRoom/sweet/tickTock/tinymusicbox/title_night/tribal/VolcanoMines1/VolcanoMines2/wavy/wedding/winter1/winter2/winter3/WizardSong/woodsTheme/XOR";

        public static Dictionary<string, bool> GetSoundsDict()
        {
            Dictionary<string, bool> soundDict = new();

            string[] cues = SoundCues.Split('/');
            cues = cues.OrderBy(x => x).ToArray();

            foreach (string cue in cues)
                soundDict.Add(cue, false);

            return soundDict;
        }

        public static Dictionary<string, bool> GetSongsDict()
        {
            Dictionary<string, bool> songDict = new();

            string[] songs = SongCues.Split('/');
            songs = songs.OrderBy(x => x).ToArray();

            foreach (string song in songs)
                songDict.Add(song, false);

            return songDict;
        }

        public static void AddAllSongsToJukebox()
        {
            if (Game1.player != null) 
            {
                string[] songs = SongCues.Split('/');
                foreach (string song in songs)
                    Game1.player.songsHeard.Add(song);
            }

            ModEntry.IMonitor.Log("Added every song to the jukebox", LogLevel.Info);
        }
    }
}
