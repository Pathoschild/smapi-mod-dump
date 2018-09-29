using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Timers;

namespace MusicNameSeeker
{
    public class Class1 :Mod
    {
        public List<char> charList;
        public static char[] name;
        public List<string> successfulMusic;

        public List<string> processedMusic;
        public List<string> failedCues;
        public static System.Timers.Timer aTimer;




        public override void Entry(IModHelper helper)
        {
            base.Entry(helper);

            StardewModdingAPI.Events.PlayerEvents.LoadedGame += PlayerEvents_LoadedGame;
            StardewModdingAPI.Command.RegisterCommand("whereami", "see where the name is").CommandFired += Class1_CommandFired;
            processedMusic = new List<string>();
            successfulMusic = new List<string>();
            failedCues = new List<string>();
            name = new char[16];
            charList = new List<char>();
            charList.Add( Convert.ToChar(0));
            charList.Add('0');
            charList.Add('1');
            charList.Add('2');
            charList.Add('3');
            charList.Add('4');
            charList.Add('5');
            charList.Add('6');
            charList.Add('7');
            charList.Add('8');
            charList.Add('9');
            charList.Add('A');
            charList.Add('B');
            charList.Add('C');
            charList.Add('D');
            charList.Add('E');
            charList.Add('F');
            charList.Add('G');
            charList.Add('H');
            charList.Add('I');
            charList.Add('J');
            charList.Add('K');
            charList.Add('L');
            charList.Add('M');
            charList.Add('N');
            charList.Add('O');
            charList.Add('P');
            charList.Add('Q');
            charList.Add('R');
            charList.Add('S');
            charList.Add('T');
            charList.Add('U');
            charList.Add('V');
            charList.Add('W');
            charList.Add('X');
            charList.Add('Y');
            charList.Add('Z');

            charList.Add('a');
            charList.Add('b');
            charList.Add('c');
            charList.Add('d');
            charList.Add('e');
            charList.Add('f');
            charList.Add('g');
            charList.Add('h');
            charList.Add('i');
            charList.Add('j');
            charList.Add('k');
            charList.Add('l');
            charList.Add('m');
            charList.Add('n');
            charList.Add('o');
            charList.Add('p');
            charList.Add('q');
            charList.Add('r');
            charList.Add('s');
            charList.Add('t');
            charList.Add('u');
            charList.Add('v');
            charList.Add('w');
            charList.Add('x');
            charList.Add('w');
            charList.Add('z');

            charList.Add(' ');
            charList.Add('_');
            charList.Add('-');
            charList.Add('!');
            charList.Add('\'');


        }

        private void Class1_CommandFired(object sender, StardewModdingAPI.Events.EventArgsCommand e)
        {
            string s = "";
            foreach (var v in name)
            {
                if (Convert.ToInt32(v) == 0)
                {
                    // Log.AsyncC("Null char. Skipping");
                    continue;
                }
                s += v;
            }
            Log.AsyncC(s);
        }

        private void PlayerEvents_LoadedGame(object sender, StardewModdingAPI.Events.EventArgsLoadedGameChanged e)
        {

            // SetTimer();
            // calculator();

         string[]s=   File.ReadAllLines(Path.Combine(Helper.DirectoryPath,"StardewRawHexDump.txt"));

            string master="";
            foreach(var v in s)
            {
               string g= v.Remove(0, 61);
               g= g.Trim(' ');
                Log.AsyncC(g);
                master += g;
            }

            string[] parsed = master.Split('.');

            foreach(var v in parsed)
            {
                string f = v.Replace(" ", string.Empty);
               Log.AsyncC(f);
                successfulMusic.Add(f);
            }

            Log.AsyncM("Parsed " + successfulMusic.Count + " audio files!");

            File.WriteAllLines(Path.Combine(Helper.DirectoryPath, "StardewRawCueNames.txt"),successfulMusic);
            //aTimer.Dispose(); 


            foreach (var v in successfulMusic)
            {
                try
                {
                    Game1.playSound(v);
                    Log.AsyncG("Successfully played song: " + v);
                    processedMusic.Add(v);
                }
                catch (Exception r)
                {
                    Log.AsyncC(r);
                    failedCues.Add(v);
                }
            }
            File.WriteAllLines(Path.Combine(Helper.DirectoryPath, "StardewWorkingCueNames.txt"), processedMusic);
            File.WriteAllLines(Path.Combine(Helper.DirectoryPath, "StardewFailedCueNames.txt"), failedCues);
        }


        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(1000 *60); //fire every minute
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(System.Object source, ElapsedEventArgs e)
        {
            string s = "";
            foreach (var v in name)
            {
                if (Convert.ToInt32(v) == 0)
                {
                    // Log.AsyncC("Null char. Skipping");
                    continue;
                }
                s += v;
            }
            Log.AsyncC("Current Position in Search: "+s);
        }

        public void calculator()
        {


            for(int one =0; one < charList.Count; one++)
            {
                for (int two = 0; two < charList.Count; two++)
                {
                    for (int three = 0; three < charList.Count; three++)
                    {
                        for (int four = 0; four < charList.Count; four++)
                        {

                            for (int five = 0; five < charList.Count; five++)
                            {
                                for (int six = 0; six < charList.Count; six++)
                                {
                                    for (int seven = 0; seven < charList.Count; seven++)
                                    {
                                        for (int eight = 0; eight < charList.Count; eight++)
                                        {
                                            for (int nine = 0; nine < charList.Count; nine++)
                                            {
                                                for (int ten = 0; ten < charList.Count; ten++)
                                                {
                                                    for (int eleven = 0; eleven < charList.Count; eleven++)
                                                    {
                                                        for (int twelve = 0; twelve < charList.Count; twelve++)
                                                        {

                                                            for (int thirteen = 0;thirteen  < charList.Count; thirteen++)
                                                            {
                                                                for (int fourteen = 0; fourteen < charList.Count; fourteen++)
                                                                {
                                                                    for (int fifteen = 0; fifteen < charList.Count; fifteen++)
                                                                    {
                                                                        for (int sixteen = 0; sixteen < charList.Count; sixteen++)
                                                                        {
                                                                            name[0] = charList.ElementAt(one);
                                                                            name[1] = charList.ElementAt(two);
                                                                            name[2] = charList.ElementAt(three);
                                                                            name[3] = charList.ElementAt(four);
                                                                            name[4] = charList.ElementAt(five);
                                                                            name[5] = charList.ElementAt(six);
                                                                            name[6] = charList.ElementAt(seven);
                                                                            name[7] = charList.ElementAt(eight);
                                                                            name[8] = charList.ElementAt(nine);
                                                                            name[9] = charList.ElementAt(ten);
                                                                            name[10] = charList.ElementAt(eleven);
                                                                            name[11] = charList.ElementAt(twelve);
                                                                            name[12] = charList.ElementAt(thirteen);
                                                                            name[13] = charList.ElementAt(fourteen);
                                                                            name[14] = charList.ElementAt(fifteen);
                                                                            name[15] = charList.ElementAt(sixteen);

                                                                            string s = "";
                                                                            foreach (var v in name)
                                                                            {
                                                                                if (Convert.ToInt32(v) == 0)
                                                                                {
                                                                                   // Log.AsyncC("Null char. Skipping");
                                                                                    continue;
                                                                                }
                                                                                s += v;
                                                                            }
                                                                            try
                                                                            {
                                                                               
                                                                                Game1.playSound(s);
                                                                                Log.AsyncG("Sucessful sound! " + s);
                                                                                successfulMusic.Add(s);
                                                                            }
                                                                            catch(Exception e)
                                                                            {
                                                                             //   Log.AsyncR("Failed sound. " + s);
                                                                            }

                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            File.WriteAllLines("musicList.txt", successfulMusic);
        }

    }
}
