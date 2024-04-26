/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace RandomNPC
{
    internal class RNPCDialogue
    {
        public string gender;
        public string situation;
        public string mood;
        public string friendship;
        public string personality;
        public string age;
        public string manners;
        public string anxiety;
        public string optimism;

        public RNPCDialogue(string dialogueString)
        {
            string[] dialogueArray = dialogueString.Split('/');
            this.age = dialogueArray[0];
            this.manners = dialogueArray[1];
            this.anxiety = dialogueArray[2];
            this.optimism = dialogueArray[3];
            this.gender = dialogueArray[4];
            this.mood = dialogueArray[5];
            this.friendship = dialogueArray[5];
        }

    }
}