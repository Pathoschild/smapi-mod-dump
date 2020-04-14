using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.IO;
using System.Media;
using System.Runtime.CompilerServices;

namespace PublicAccessTV
{
	public class Scene
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static Type CustomTVMod => ModEntry.CustomTVMod;

		public string message;
		public TemporaryAnimatedSprite background;
		public TemporaryAnimatedSprite overlay;

		// Don't use looping sound cues or long external sound assets, since
		// neither can be stopped at the end of scenes due to cross-platform
		// limitations (Windows and Android, respectively). Music tracks are
		// stopped at the appropriate time on all platforms.
		public string musicTrack;
		public string soundAsset;
		public string soundCue;

		public Action beforeAction;
		public Action afterAction;

		private TV currentTV;
		private Channel currentChannel;

		public Scene (string message, TemporaryAnimatedSprite background,
			TemporaryAnimatedSprite overlay = null)
		{
			this.message = message;
			this.background = background;
			this.overlay = overlay;
		}

		public void run (TV tv, Channel channel)
		{
			currentTV = tv;
			currentChannel = channel;

			if (beforeAction != null)
				beforeAction.Invoke ();

			Game1.changeMusicTrack (musicTrack ?? "none", false,
				Game1.MusicContext.Event);

			if (soundAsset != null &&
				Constants.TargetPlatform != GamePlatform.Android)
			{
				playCustomSound ($"{soundAsset}.wav");
			}
			else if (soundCue != null)
			{
				ICue cue = Game1.soundBank.GetCue (soundCue);
				if (soundCue == "distantTrain")
					cue.SetVariable ("Volume", 100f);
				cue.Play ();
			}


			Helper.Reflection.GetField<TemporaryAnimatedSprite> (tv, "screen")
				.SetValue (background);
			Helper.Reflection.GetField<TemporaryAnimatedSprite> (tv, "screenOverlay")
				.SetValue (overlay);
			Game1.drawObjectDialogue (Game1.parseText (message));

			Game1.afterDialogues = end;
		}

		private void end ()
		{
			if (afterAction != null)
				afterAction.Invoke ();
			
			currentChannel.runProgram (currentTV);
		}

		protected void playCustomSound (string filename)
		{
			string path = Path.Combine (Helper.DirectoryPath, "assets", filename);
			try
			{
				playSoundWithSoundPlayer (path);
			}
			catch (TypeLoadException)
			{
				// best effort, since Android lacks System.Media
			}
		}

		[MethodImplAttribute(MethodImplOptions.NoInlining)] 
		private void playSoundWithSoundPlayer (string path)
		{
			SoundPlayer sound = new SoundPlayer (path);
			sound.Play ();
		}
	}
}
