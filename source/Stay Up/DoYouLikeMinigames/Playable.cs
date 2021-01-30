/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Minigames;
using System;

namespace Su226.DoYouLikeMinigames {
  class CannotPlay : Exception {
    public CannotPlay(string key) : base(M.Helper.Translation.Get(key)) {}
  }

  abstract class Playable {
    public string name;

    public Playable(string key) {
      this.name = M.Helper.Translation.Get(key);
    }

    public abstract void Play();
  }

  class MinigamePlayable : Playable {
    public Func<IMinigame> game;

    public MinigamePlayable(string key, Func<IMinigame> game) : base(key) {
      this.game = game;
    }

    public override void Play() {
      Game1.currentMinigame = game();
    }
  }

  class ClubPlayable : MinigamePlayable {
    public ClubPlayable(string key, Func<IMinigame> game) : base(key, () => {
      if (M.Config.checkClubCard && !Game1.player.hasClubCard) {
        throw new CannotPlay("no_club_card");
      }
      return game();
    }) {}
  }

  class CalicoPlayable : ClubPlayable {
    public CalicoPlayable(string key, int bet, bool high) : base(key, () => {
      if (M.Config.checkQiCoin && bet != 0 && Game1.player.clubCoins < bet) {
        throw new CannotPlay("no_qi_coin");
      }
      return new CalicoJackExitable(bet, high);
    }) {}
  }

  class CranePlayable : MinigamePlayable {
    public CranePlayable(string key, Func<IMinigame> game) : base(key, () => {
      if (M.Config.checkCinema && !Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater")) {
        throw new CannotPlay("no_cinema");
      }
      return game();
    }) {}
  }

  class KartPlayable : MinigamePlayable {
    public KartPlayable(string key, Func<IMinigame> game) : base(key, () => {
      if (M.Config.checkSkullKey && !Game1.player.hasSkullKey) {
        throw new CannotPlay("no_skull_key");
      }
      return game();
    }) {}
  }

  class FairPlayable : MinigamePlayable {
    public FairPlayable(string key, Func<IMinigame> game) : base(key, () => {
      if (M.Config.checkFair && !Game1.player.hasOrWillReceiveMail("CF_Fair")) {
        throw new CannotPlay("no_fair");
      }
      return game();
    }) {}
  }
}