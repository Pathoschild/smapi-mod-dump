using System;
using StardewModdingAPI;

namespace priceDrops
{
    internal class mailEditor : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\Mail");
        }

        // Initialize letters
        public void Edit<T>(IAssetData asset)
        {
            asset.AsDictionary<string, string>().Set("robin1", "Hey there @,^you've been such a good customer since you've arrived here in town. So I've decided to give you a " + ModEntry.DISC_1 + "% discount!^^See you soon,^   -Robin, your favorite carpenter");
            asset.AsDictionary<string, string>().Set("robin2", "Hi @,^since you've been such a good friend (and customer!) to me, I've decided to give you a " + ModEntry.DISC_2 + "% discount!^^See you soon,^   -Robin, your favorite carpenter (and friend!)");
            asset.AsDictionary<string, string>().Set("robin3", "Hi! I'm really happy to be your friend, @!^From now on, whenever you go shopping at my place, I'll give you " + ModEntry.DISC_3 + "% off!^^I hope you swing by soon! I'll get all your stuff built in no time!^   -Robin, your friend and favorite carpenter");
            asset.AsDictionary<string, string>().Set("robinMaru", "Hey @,^since we're family now, I thought I should give you another discount of " + ModEntry.BONUS_DISC + "% at my store. Take good care of my kid!^^Love,^   -Robin");
            asset.AsDictionary<string, string>().Set("robinSebastian", "Hey @,^since we're family now, I thought I should give you another discount of " + ModEntry.BONUS_DISC + "% at my store. Take good care of my kid!^^Love,^   -Robin");

            asset.AsDictionary<string, string>().Set("marnie1", "Hey there @,^thank you for being such a good customer. Since you're going to be a regular, I've decided to give you " + ModEntry.DISC_1 + "% off for all your next purchases!^^Have a nice day,^   -Marnie");
            asset.AsDictionary<string, string>().Set("marnie2", "Hello @, my Aunt made me write this because she's busy. Anyway, she wants you to know that you're really nice or something and that she's giving you another discount. Not even sure if we can afford it. Come buy some hay or something. It's " + ModEntry.DISC_2 + "% off.^   -Shane");
            asset.AsDictionary<string, string>().Set("marnie3", "Hey dear,^you've been a really good friend and I'm really glad we're neighbours! Let me tell you something. From now on, you get " + ModEntry.DISC_3 + "% off of all the things you buy from me!^^Talk to you soon,^   -Marnie");
            asset.AsDictionary<string, string>().Set("marnieShane", "Hey @,^thought I should write you a letter. I want you to get a discount on my chickens. I talked to my Aunt and discussed it with her and she said she's fine with it. So we'll give you " + ModEntry.BONUS_DISC + "% on the chickens.^^Have a nice day,^   -Shane");

            asset.AsDictionary<string, string>().Set("pierre1", "Dear valued customer,^Thanks for choosing 'Pierre's', your local produce shoppe! Your loyalty is very appreciated. We are happy to announce that from now on, you will receive a rebate of " + ModEntry.DISC_1 + "% whenever you visit our store. See you soon!^   -Pierre^^P.S. Sorry for the stock message, @. Enjoy!");
            asset.AsDictionary<string, string>().Set("pierre2", "Dear valued customer,^Thanks for choosing 'Pierre's', your local produce shoppe! Your loyalty is very appreciated. We are happy to announce that from now on, you will receive a rebate of " + ModEntry.DISC_2 + "% whenever you visit our store. See you soon!^   -Pierre^^P.S. Sorry for the stock message, @. I'm really busy these days. Enjoy!");
            asset.AsDictionary<string, string>().Set("pierre3", "Dear valued customer,^Thanks for choosing 'Pierre's', your local produce shoppe! Your loyalty is very appreciated. We are happy to announce that from now on, you will receive a rebate of " + ModEntry.DISC_3 + "% whenever you visit our store. See you soon!^   -Pierre^^P.S. Sorry for the stock message, @. How are you, by the way? I'm doing quite well myself, haha! Enjoy!");
            asset.AsDictionary<string, string>().Set("pierreAbigail", "Hey, @!^Since we're kind of family now, Caroline made me give you a " + ModEntry.BONUS_DISC + "% discount on the store.  Anyway, I know Abby is a little wild, but she has a good heart. Treat her well.^^Take care,^   -Pierre");
            asset.AsDictionary<string, string>().Set("pierreCaroline", "Hey, @, I didn't know you were best friends with my wife. She told me to give you a " + ModEntry.BONUS_DISC + "% discount in our shop yesterday. You better stop by and shop often from now on!^^See you soon,^   -Pierre");

            asset.AsDictionary<string, string>().Set("harvey1", "Hello, @!^You've been a valued customer at my clinic. So I've decided to give you a " + ModEntry.DISC_1 + "% discount for my OTC medicine. Have a nice day!^   -Harvey^^PS: Don't forget to cover your mouth when you sneeze! Wash your hands often!");
            asset.AsDictionary<string, string>().Set("harvey2", "Hello, @!^I'm so glad to have a friend in town. If you need tonics or remedies, just come by the clinic. I'll give you a " + ModEntry.DISC_2 + "% discount. See you soon!^   -Harvey^^PS: Don't forget to cover your mouth when you sneeze! Wash your hands often! If you need anything, just visit the clinic.");
            asset.AsDictionary<string, string>().Set("harvey3", "Hello, @!^You've been such a good friend to me since you've arrived in town. If you need anything, just visit the clinic. I can give you a check-up before you go into the mines if you want. Oh and you get a " + ModEntry.DISC_3 + "% discount for my OTC medicines.^^Take care.^   -Harvey^^PS: Don't forget to cover your mouth when you sneeze! Wash your hands often! Don't forget to eat well! Oh, and don't overwork yourself.");
            asset.AsDictionary<string, string>().Set("harveyMarried", "Hey darling, I'm writing from my office because I forgot to tell you yesterday. Since we're married now, I'd like you to have a " + ModEntry.BONUS_DISC + "% discount on my over-the-counter medicines. It's not much, but I hope it helps you when you run off to the mines again.^^Take care,^   -your husband");

            asset.AsDictionary<string, string>().Set("gus1", "Hello, @!^Since you've become a regular at my place, I'll give you " + ModEntry.DISC_1 + "% off! Come by the Stardrop Saloon when you need any refreshments. I make different dishes every week!^   -Gus");
            asset.AsDictionary<string, string>().Set("gus2", "Hey there, @,^thanks for being a loyal friend and customer. I really appreciate you coming by so often. Listening to your stories is never dull, haha! Anyway, I want to give you " + ModEntry.DISC_2 + "% off when you buy refreshments at the bar.^^See you soon!^   -Gus");
            asset.AsDictionary<string, string>().Set("gus3", "Hey, @!^I'm really glad you moved into town. Want to hear great news? I went through the numbers and am now able to give you your deserved " + ModEntry.DISC_3 + "% rebate for everything you purchase at my Saloon! I hope you come by more often from now on.^   -Your friend Gus");

            asset.AsDictionary<string, string>().Set("clint1", "Hello, @^thanks for coming by my shop so often. You know what, I'll give you " + ModEntry.DISC_1 + "% off from now on. If you need your tools upgraded, I'm your man. You know, being a blacksmith and all. Anyway. Bye.^   -Clint");
            asset.AsDictionary<string, string>().Set("clint2", "Hey, @^thanks for helping me with Emily the other day. Even though it didn't help. Anyway, I wanted to tell you that you'll get a " + ModEntry.DISC_2 + "% discount on your upgrades and my shop. Take care.^   -Clint");
            asset.AsDictionary<string, string>().Set("clint3", "Hey, @!^You're really hitting the mines! Thanks to you, my business is going quite well. So well actually, that I can give you " + ModEntry.DISC_3 + "% off from now on! Don't tell anyone. Take care.^   -Clint");

            asset.AsDictionary<string, string>().Set("sandy1", "Hello, hello, sweetie! I'm so glad you come by so often, It's so boring out here. Just me and the heat and those coconuts...^Anyway, I'll give you " + ModEntry.DISC_1 + "% off, so come by more often, and buy lots of seeds! Bye, kid!^   ~Sandy^^PS: Please say hi to Emily from me!");
            asset.AsDictionary<string, string>().Set("sandy2", "Hi!~ How are you, sweetie? I'm writing you this because you're such a good friend. See, I've been doing better thanks to you! Come and get your " + ModEntry.DISC_2 + "% off now! Just kidding. But seriously, it's so boring and hot here. Come visit me!^And buy a bunch of seeds!^   ~Sandy^^PS: Say hi to Emily! She hasn't been here in ages!");
            asset.AsDictionary<string, string>().Set("sandy3", "Hey, hey! It's your bestie Sandy from the desert!^Hope the valley is cooler than here, hehe!^You know that you're a valued customer and friend, right? You should come by more often. I'll give you a " + ModEntry.DISC_3 + "% discount! Nothing happens out here, it's depressing, really. Anyway, see you soon, honey!^   ~Sandy^^PS: Greet Emily for me, will you!");

            asset.AsDictionary<string, string>().Set("willy1", "Hey there, @,^thanks for dropping by so often. I've considered expanding my shop, but let's leave that for now. I've decided to give you a " + ModEntry.DISC_1 + "% discount since you've been such a loyal customer. Take care.^   -Willy");
            asset.AsDictionary<string, string>().Set("willy2", "Hey there, fellow fishing enthusiast! Glad you decided to come here. If you ever need a new fishing pole or bait, swing by my shop by the beach, I'll give you " + ModEntry.DISC_2 + "% off!^^Have fun fishing and good luck!^   -Willy");
            asset.AsDictionary<string, string>().Set("willy3", "Hello, @!^The ocean's been exciting the past few days, fishing-wise!^The local fishing-community has expanded quite a lot since you've arrived here. I want to thank you by giving you a " + ModEntry.DISC_3 + "% discount in my shop. Have fun fishing!^   -Willy");

            String dwarfDisc1 = "a tenth";
            String dwarfDisc2 = "a quarter";
            String dwarfDisc3 = "a half";

            if (ModEntry.DISC_1 == 10)
                dwarfDisc1 = "a tenth";
            else if (ModEntry.DISC_1 == 20)
                dwarfDisc1 = "a fifth";
            else if (ModEntry.DISC_1 == 25)
                dwarfDisc1 = "a quarter";
            else if (ModEntry.DISC_1 == 50)
                dwarfDisc1 = "a half";
            else if (ModEntry.DISC_1 == 75)
                dwarfDisc1 = "a three-quart";
            else
                dwarfDisc1 = ModEntry.DISC_1 + "%";

            if (ModEntry.DISC_2 == 10)
                dwarfDisc2 = "a tenth";
            else if (ModEntry.DISC_2 == 20)
                dwarfDisc2 = "a fifth";
            else if (ModEntry.DISC_2 == 25)
                dwarfDisc2 = "a quarter";
            else if (ModEntry.DISC_2 == 50)
                dwarfDisc2 = "a half";
            else if (ModEntry.DISC_2 == 75)
                dwarfDisc2 = "a three-quart";
            else
                dwarfDisc2 = ModEntry.DISC_2 + "%";

            if (ModEntry.DISC_3 == 10)
                dwarfDisc3 = "a tenth";
            else if (ModEntry.DISC_3 == 20)
                dwarfDisc3 = "a fifth";
            else if (ModEntry.DISC_3 == 25)
                dwarfDisc3 = "a quarter";
            else if (ModEntry.DISC_3 == 50)
                dwarfDisc3 = "a half";
            else if (ModEntry.DISC_3 == 75)
                dwarfDisc3 = "a three-quart";
            else
                dwarfDisc3 = ModEntry.DISC_3 + "%";

            asset.AsDictionary<string, string>().Set("dwarf1", "I don't trust you, but you're a good customer. Keep " + dwarfDisc1 + " when you buy at my shop in the mines.");
            asset.AsDictionary<string, string>().Set("dwarf2", "I have lots of things for your mining pleasure, human. You can keep " + dwarfDisc2 + ".");
            asset.AsDictionary<string, string>().Set("dwarf3", "Come by my shop any time. The mine is lonely. You can keep " + dwarfDisc3 + " of your money when shopping.");

            asset.AsDictionary<string, string>().Set("krobus1", "You are not like other humans. You are very friendly. I will give you " + dwarfDisc1 + " for that.^   -Krobus");
            asset.AsDictionary<string, string>().Set("krobus2", "You are special, human. I get few visitors, and you're the good kind. Keep " + dwarfDisc2 + " when you shop.^   -Krobus");
            asset.AsDictionary<string, string>().Set("krobus3", "You are a good ... nlakh. What do humans call it? I believe the right word is 'fiend'. Thank you, @. I will grant you " + dwarfDisc3 + " of the price.^   -Krobus");

            asset.AsDictionary<string, string>().Set("wizard1", "Greetings, young adept.^It is time. I have foreseen what must be done and that you will be in need of my help. In other words, I'll give you a " + ModEntry.DISC_1 + "% discount when the time comes.^   -M. Rasmodius, Wizard");
            asset.AsDictionary<string, string>().Set("wizard2", "Greetings, young adept.^The spirits have changed their minds. You are in need of a greater discount when the time has arrived! I'll give you " + ModEntry.DISC_2 + "% off.^   -M. Rasmodius, Wizard");
            asset.AsDictionary<string, string>().Set("wizard3", "Greetings, young adept.^My arcane powers have increased. I have foreseen that I must grant you " + ModEntry.DISC_3 + "% off in total if the prophecies are to be fulfilled. Beware!^   -M. Rasmodius, Wizard");

        }
    }
}