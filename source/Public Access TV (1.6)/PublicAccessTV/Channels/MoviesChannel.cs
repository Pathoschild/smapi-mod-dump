/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/PredictiveMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Movies;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using System.IO;

namespace PublicAccessTV
{
	public class MoviesChannel : Channel
	{
		public MoviesChannel ()
			: base ("movies")
		{
			Helper.ModContent.Load<Texture2D>
				(Path.Combine ("assets", "movies_craneGame.png"));
		}

		internal override bool isAvailable =>
			Movies.IsAvailable;

		internal override void show (TV tv)
		{
			Movies.Prediction prediction = Movies.PredictForDate (SDate.Now ());

			TemporaryAnimatedSprite screenBackground = loadSprite (tv,
				"Maps\\MovieTheaterScreen_TileSheet", new Rectangle (31, 0, 162, 108));

			string hostName = null;
			TemporaryAnimatedSprite portrait = null;

			if (Helper.ModRegistry.IsLoaded ("LemurKat.JulietHouse.NPC"))
			{
				hostName = "Juliet";
				portrait = loadPortrait (tv, "Juliet", 0, 0);
			}
			else if (Helper.ModRegistry.IsLoaded ("FlashShifter.StardewValleyExpandedCP"))
			{
				hostName = "Claire";
				portrait = loadPortrait (tv, "Claire", 1, 0);
			}

			hostName ??= Helper.Translation.Get ("movies.host.generic");
			portrait ??= loadSprite (tv,
				"Maps\\MovieTheater_TileSheet", new Rectangle (240, 160, 16, 26),
				positionOffset: new Vector2 (18f, 2f), overlay: true);

			// Opening scene: the concessionaire greets the viewer.
			queueScene (new Scene (Helper.Translation.Get ("movies.opening",
				new { host = hostName }), screenBackground, portrait)
			{ soundCue = "Cowboy_Secret" });

			// Current movie poster, title and description
			queueScene (new Scene (Helper.Translation.Get ("movies.current", new
			{
				title = TokenParser.ParseText(prediction.currentMovie.Title),
				description = TokenParser.ParseText(prediction.currentMovie.Description),
			}), loadMoviePoster (tv, prediction.currentMovie))
			{ musicTrack = prediction.currentMovie.Scenes[0].Music });

			// Lobby advertisement. If the crane game is available, it is
			// promoted; otherwise, the concession stand is promoted.
			if (prediction.craneGameAvailable)
			{
				string assetName = Helper.ModContent.GetInternalAssetName
                    (Path.Combine ("assets", "movies_craneGame.png")).Name;
				TemporaryAnimatedSprite craneGame = loadSprite (tv, assetName,
					new Rectangle (0, 0, 94, 63));
				TemporaryAnimatedSprite craneFlash = loadSprite (tv, assetName,
					new Rectangle (94, 0, 94, 63), 250f, 2, new Vector2 (),
					true, true);
				queueScene (new Scene (Helper.Translation.Get ("movies.lobby.craneGame"),
					craneGame, craneFlash)
				{ musicTrack = "crane_game" });
			}
			else
			{
				queueScene (new Scene (Helper.Translation.Get ("movies.lobby.concession"),
					loadSprite (tv, "Maps\\MovieTheater_TileSheet", new Rectangle (2, 3, 84, 56)),
					portrait)
				{
                    // workaround, see comments from Scene.cs playSoundWithSoundPlayer()
                    // soundAsset = "movies_concession",
					// musicTrack =
					// 	(Constants.TargetPlatform == GamePlatform.Android)
					// 		? "movieTheater" : null
					musicTrack = "movieTheater"
				});
			}

			// Upcoming movie poster, title and description.
			queueScene (new Scene (Helper.Translation.Get ("movies.next", new
			{
				season = Utility.getSeasonNameFromNumber
						(prediction.firstDateOfNextMovie.SeasonIndex),
				title = TokenParser.ParseText(prediction.nextMovie.Title),
				description = TokenParser.ParseText(prediction.nextMovie.Description),
			}), loadMoviePoster (tv, prediction.nextMovie))
			{ musicTrack = prediction.nextMovie.Scenes[0].Music });

			// Closing scene: the concessionaire signs off.
			queueScene (new Scene (Helper.Translation.Get ("movies.closing"),
				screenBackground, portrait));

			runNextScene (tv);
		}

		private TemporaryAnimatedSprite loadMoviePoster (TV tv, MovieData movie)
		{
			return loadSprite (tv, "LooseSprites\\Movies",
				new Rectangle (15, 128 * movie.SheetIndex, 92, 61));
		}
	}
}
