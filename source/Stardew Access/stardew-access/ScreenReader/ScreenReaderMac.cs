/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

/*
    MacOS libspeak library used:
    https://github.com/Flameborn/libspeak
*/

using System.Runtime.InteropServices;
using System.Reflection;

namespace stardew_access.ScreenReader
{
    public class ScreenReaderMac : IScreenReader
    {
        // The speaker instance
        private static IntPtr speaker;
        //Stuff for the runloop thread
        private readonly CancellationTokenSource cts = new();
        private Thread? rt;
        //Speech queue for interrupt
        private static readonly Queue<string> speechQueue = new();

        // DidFinishSpeaking callback for interrupt
        readonly dfs_callback fscb = new(DoneSpeaking);

        // Dylib imports
        ///////////
        // Speaker
        //
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void init_speaker();
        #pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
        [DllImport("libspeak", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        #pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
        private static extern void speak(String text);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_voice(Int32 index);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 available_voices_count();
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_language(Int32 index);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 available_languages_count();
        #pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
        [DllImport("libspeak",  CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        #pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
        private static extern void get_voice_name(UInt32 idx, String pszOut);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_volume(Single volume);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern Single get_volume();
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_rate(Single rate);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern Single get_rate();
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void stop();
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void cleanup_speaker();
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void mainloop_speaker(IntPtr speaker);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern Boolean is_speaking(IntPtr speaker);

        ///////////////
        // Speaker OO
        //
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr make_speaker();
        [DllImport("libspeak", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void speak_with(IntPtr speaker, [MarshalAs(UnmanagedType.LPUTF8Str)] String text);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_voice_with(IntPtr speaker, Int32 index);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_volume_with(IntPtr speaker, Single volume);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern Single get_volume_with(IntPtr speaker);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_rate_with(IntPtr speaker, Single rate);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern Single get_rate_with(IntPtr speaker);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void stop_with(IntPtr speaker);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void cleanup_with(IntPtr speaker);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void wsw_callback(String p1);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void wsp_callback(Int16 p1);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void dfs_callback();

        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void register_will_speak_word_callback(IntPtr speaker, [MarshalAs(UnmanagedType.FunctionPtr)] wsw_callback cb);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void register_will_speak_phoneme_callback(IntPtr speaker, [MarshalAs(UnmanagedType.FunctionPtr)] wsp_callback cb);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void register_did_finish_speaking_callback(IntPtr speaker, [MarshalAs(UnmanagedType.FunctionPtr)] dfs_callback cb);

        /////////////////
        // Recognizer OO
        //
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr make_listener();
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void start_listening(IntPtr listener);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void stop_listening(IntPtr listener);
        #pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
        [DllImport("libspeak", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        #pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
        private static extern void add_command(IntPtr listener, String command);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void cleanup_listener(IntPtr listener);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void mainloop_listener(IntPtr listener);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern Boolean is_listening(IntPtr listener);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void drc_callback(String p1);
        [DllImport("libspeak", CallingConvention = CallingConvention.Cdecl)]
        private static extern void register_did_recognize_command_callback(IntPtr listener, drc_callback cb);



        private string menuPrefixText = "";
        private string prevMenuPrefixText = "";
        private string menuSuffixText = "";
        private string prevMenuSuffixText = "";
        private string menuPrefixNoQueryText = "";
        private string menuSuffixNoQueryText = "";

        public string PrevTextTile
        {
            get => prevTextTile;
            set => prevTextTile = value;
        }

        public string PrevMenuQueryText
        {
            get => prevMenuText;
            set => prevMenuText = value;
        }

        public string MenuPrefixText
        {
            get => menuPrefixText;
            set => menuPrefixText = value;
        }

        public string MenuSuffixText
        {
            get => menuSuffixText;
            set => menuSuffixText = value;
        }

        public string MenuPrefixNoQueryText
        {
            get => menuPrefixNoQueryText;
            set => menuPrefixNoQueryText = value;
        }

        public string MenuSuffixNoQueryText
        {
            get => menuSuffixNoQueryText;
            set => menuSuffixNoQueryText = value;
        }

        public string prevText = "", prevTextTile = "", prevChatText = "", prevMenuText = "";
        private bool resolvedDll = false;

        private static void SpeakLoop(object? obj)
        {
            if (obj == null) return;

            CancellationToken ct = (CancellationToken)obj;

            while (!ct.IsCancellationRequested)
            {
                mainloop_speaker(speaker);
                Thread.Sleep(20);
            }
        }

        public void InitializeScreenReader()
        {
            if (!resolvedDll) {
                // Set the path to load libspeak.dylib from via a resolver
                NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
                resolvedDll = true;
            }

            Log.Info("Initializing screen reader");
            speaker = make_speaker();
            rt = new Thread(new ParameterizedThreadStart(SpeakLoop));
            rt.Start(cts.Token);
            register_did_finish_speaking_callback(speaker, fscb);
            set_rate_with(speaker, MainClass.Config.MacSpeechRate);
        }

        public void CloseScreenReader()
        {
            cts.Cancel();
            rt?.Join();
            cts.Dispose();
            cleanup_with(speaker);
        }

        public void Say(string text, bool interrupt)
        {
            if (text == null) return;
            if (string.IsNullOrWhiteSpace(text)) return;
            if (!MainClass.Config.TTS) return;

            #if DEBUG
            Log.Verbose($"Speaking(interrupt: {interrupt}) = {text}");
            #endif

            if (interrupt)
            {
                speechQueue.Clear();
                speak_with(speaker, text);
            }
            else
            {
                speechQueue.Enqueue(text);
            }
        }

        public void SayWithChecker(string text, bool interrupt)
        {
            if (text == null) return;
            if (text != prevText)
            {
                Say(text, interrupt);
                prevText = text;
            }
        }

        public void SayWithMenuChecker(string text, bool interrupt)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (prevMenuText == text && prevMenuSuffixText == MenuSuffixText && prevMenuPrefixText == MenuPrefixText)
                return;

            prevMenuText = text;
            prevMenuSuffixText = MenuSuffixText;
            prevMenuPrefixText = MenuPrefixText;
            Say($"{MenuPrefixNoQueryText}{MenuPrefixText}{text}{MenuSuffixText}{MenuSuffixNoQueryText}", interrupt);
            MenuPrefixNoQueryText = "";
            MenuSuffixNoQueryText = "";
        }

        public void SayWithChatChecker(string text, bool interrupt)
        {
            if (text == null) return;
            if (text != prevChatText)
            {
                Say(text, interrupt);
                prevChatText = text;
            }
        }

        public void SayWithTileQuery(string text, int x, int y, bool interrupt)
        {
            string query = $"{text} x:{x} y:{y}";

            if (prevTextTile != query)
            {
                prevTextTile = query;
                Say(text, interrupt);
            }
        }

        private static void DoneSpeaking()
        {
            if (speechQueue.Count != 0)
            {
                speak_with(speaker, speechQueue.Dequeue());
            }
        }

        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            // libraryName is the name provided in DllImport i.e., [DllImport(libraryName)]
            if (libraryName != "libspeak") return IntPtr.Zero;
            if (MainClass.ModHelper is null) return IntPtr.Zero;
            
            string dylibPath = Path.Combine(MainClass.ModHelper.DirectoryPath, "libraries", "macos", "libspeak.dylib");
            return NativeLibrary.Load(dylibPath, assembly, searchPath);
        }
    }
}
