using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PredictiveCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Movies;
using StardewValley.Objects;
using System.IO;

namespace PublicAccessTV
{
	public class MoviesChannel : Channel
	{
		public MoviesChannel ()
			: base ("movies")
		{
			Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "movies_craneGame.png"));
		}

		internal override bool isAvailable =>
			base.isAvailable && Movies.IsAvailable;

		internal override void show (TV tv)
		{
			MoviePrediction prediction = Movies.PredictForDate (Utilities.Now ());

			TemporaryAnimatedSprite screenBackground = loadSprite (tv,
				"MovieTheaterScreen_TileSheet", new Rectangle (31, 0, 162, 108));
			TemporaryAnimatedSprite hostOverlay = loadSprite (tv,
				"MovieTheater_TileSheet", new Rectangle (240, 160, 16, 26),
				positionOffset: new Vector2 (18f, 2f), overlay: true);

			// Opening scene: the concessionaire greets the viewer.
			string hostName =
				Helper.ModRegistry.IsLoaded ("Lemurkat.NPCJuliet")
					? "Juliet"
					: Helper.ModRegistry.IsLoaded ("FlashShifter.StardewValleyExpandedCP")
						? "Claire"
						: Helper.Translation.Get ("movies.host.generic");
			queueScene (new Scene (Helper.Translation.Get ("movies.opening",
				new { host = hostName }), screenBackground, hostOverlay)
				{ soundCue = "Cowboy_Secret" });

			// Current movie poster, title and description
			queueScene (new Scene (Helper.Translation.Get ("movies.current", new
				{
					title = prediction.currentMovie.Title,
					description = prediction.currentMovie.Description,
				}), loadMoviePoster (tv, prediction.currentMovie))
				{ musicTrack = prediction.currentMovie.Scenes[0].Music });

			// Lobby advertisement. If the crane game is available, it is
			// promoted; otherwise, the concession stand is promoted.
			if (prediction.craneGameAvailable)
			{
				string assetName = Helper.Content.GetActualAssetKey
					(Path.Combine ("assets", "movies_craneGame.png"));
				TemporaryAnimatedSprite craneGame = loadSprite (tv, assetName,
					new Rectangle (0, 0, 94, 63));
				TemporaryAnimatedSprite craneFlash = loadSprite (tv, assetName,
					new Rectangle (94, 0, 94, 63), 250f, 2, new Vector2 (),
					true, true);
				queueScene (new Scene (Helper.Translation.Get ("movies.lobby.craneGame"),
					craneGame, craneFlash) { musicTrack = "crane_game" });
			}
			else
			{
				queueScene (new Scene (Helper.Translation.Get ("movies.lobby.concession"),
					loadSprite (tv, "MovieTheater_TileSheet", new Rectangle (2, 3, 84, 56)))
					{ soundAsset = "movies_concession", musicTrack =
						(Constants.TargetPlatform == GamePlatform.Android)
							? "movieTheater" : null });
			}

			// Upcoming movie poster, title and description.
			queueScene (new Scene (Helper.Translation.Get ("movies.next", new
				{
					season = Utility.getSeasonNameFromNumber
						(prediction.firstDateOfNextMovie.SeasonIndex),
					title = prediction.nextMovie.Title,
					description = prediction.nextMovie.Description,
				}), loadMoviePoster (tv, prediction.nextMovie))
				{ musicTrack = prediction.nextMovie.Scenes[0].Music });

			// Closing scene: the concessionaire signs off.
			queueScene (new Scene (Helper.Translation.Get ("movies.closing"),
				screenBackground, hostOverlay));

			runProgram (tv);
		}

		private TemporaryAnimatedSprite loadMoviePoster (TV tv, MovieData movie)
		{
			return loadSprite (tv, "LooseSprites\\Movies",
				new Rectangle (15, 128 * movie.SheetIndex, 92, 61));
		}
	}
}
