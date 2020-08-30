<details>
  <summary><i>TRANSLATION INFO:</i></summary>
  
  - No translations are available yet.
</details>

---

# Dynamic Conversation Topics
*The villagers of Pelican Town are paying attention!*

## For Players
This mod adds new dialogue event keys, allowing NPCs to react to hundreds of events in your game in real-time. If you complete a quest for George, you might hear more about it from Evelyn, Alex, or Harvey next time you talk to them! If you see one of Sebastian's heart events, Sam, Robin, or Abigail might talk to you about it later. If you are creepy to Leah, Elliott will tell you off. Villagers will now congratulate you on your marriage and on the arrival of new children, too.

<details>
  <summary><i>NPC responds after some quest completion:</i></summary>
  
  ![Caption of the photo...](promo/PHOTO_NAME.png "Hover text!")
</details>

### Translation Support
No translations are available yet, but this mod is designed to support them! If you would like to help translate this mod, you can [contribute here](https://github.com/StardewModders/mod-translations/issues/31{{REPLACE_LINK}}). Submitted translations will be included in future mod updates.

## For Modders
All dialogue keys added by this mod are free to be used by other mods, too. Mod creators can use the same keys to overwrite or change any actual dialogues from this mod, add keyed dialogue entries for more NPC/event combinations, or make their own custom NPCs respond to all the same events in-game.

For example: DCT mod adds a new conversation topic (a.k.a. active dialogue event) when a player earns the "Artisan" achievement by crafting 30 different items. The dialogue key for this conversation topic is `"DCT.Achievement.Artisan"`. Other modders can add (or edit) a dialogue response for this key in literally any NPC's dialogue file.

<details>
  <summary><b>Existing NPC example:</b> <i>adding (or overwriting) DCT response dialogues</i></summary>

Inside the `content.json` `"Changes":` field for your CP content pack (this edits an NPC's dialogue file):

    {
      "LogName": "Snooty Leah Mod",
      "Action": "EditData",
      "Target": "Characters/Dialogue/Leah",
      "Entries": {
        "DCT.Achievement.Artisan": "You think you're really something, huh?$7#$b#You may have done a lot of crafting, but I'm still way better than you.$a",
          //Change Leah's response to the player earning the Artisan achievement
        "DCT.Event.Elliott8Heart": null,
          //Completely remove Leah's DCT response to Elliott's book reading
      }
    },

</details><br/>

<details>
  <summary><b>Custom NPC example:</b> <i>including DCT response dialogues for your character</i></summary>

Inside the `content.json` for your NPC content pack (this adds the dialogue file):

    {
      "LogName": "Bartholomew NPC",
      "Action": "Load",
      "Target": "Characters/Dialogue/Bartholomew",
      "FromFile": "assets/Dialogue/Bartholomew.json"
    },

Inside the `assets/Dialogue/Bartholomew.json` file with the character's other dialogues:

    {
      ...
      "summer_Sun8": "This is an example summer sunday dialogue for 8 hearts.$h",
      "fall_Mon": "This is an example monday dialogue for fall.",
      "Wed4": "This dialogue will be seen on Wednesdays at 4+ hearts.$s",
      ...
      "DCT.Quest.FishStew": "I stopped by the Saloon for a meal recently.#$b#The fish stew was amazing! Gus has really outdone himself.$h",
      "DCT.Achievement.Artisan": "I'm really impressed by your craftmanship!$u#$b#Maybe you could teach me a skill or two sometime?",
      ...
    }

</details></br>

Note: there is **no need to use `HasMod:` conditionals** with any patch that adds or edits DCT dialogue responses! If DCT is not installed, those dialogue entries will simply sit there and not be used. They will not cause any bugs or unexpected behaviour for users who have not installed Dynamic Conversation Topics.

### Achievements (40)
These dialogue keys allow new NPC dialogue responses once a player fulfills criteria for a new achievement.

<details>
  <summary>Click to expand list of achievement dialogue keys. <b>WARNING: SPOILERS</b></summary>

#### Money Achievements
`DCT.Achievement.Greenhorn` (earn 15k)  
`DCT.Achievement.Cowpoke` (earn 50k)  
`DCT.Achievement.Homesteader` (earn 250k)  
`DCT.Achievement.Millionaire` (earn 1 million)  
`DCT.Achievement.Legend` (earn 10 million)  

---
#### Friendship Achievements
`DCT.Achievement.ANewFriend` (1 villager at 5+ hearts)  
`DCT.Achievement.BestFriends` (1 villager at 10+ hearts)  
`DCT.Achievement.TheBelovedFarmer` (8 villagers at 10+ hearts)  
`DCT.Achievement.Cliques` (4 villagers at 5+ hearts)  
`DCT.Achievement.Networking` (10 villagers at 5+ hearts)  
`DCT.Achievement.Popular` (20 villagers at 5+ hearts)  

---
#### Cooking Achievements
`DCT.Achievement.Cook` (cook 10 different recipes)  
`DCT.Achievement.SousChef` (cook 25 different recipes)  
`DCT.Achievement.GourmetChef` (cook all recipes)  

---
#### Farmhouse Achievements
`DCT.Achievement.MovingUp` (1 house upgrade)  
`DCT.Achievement.LivingLarge` (2 house upgrades)  

---
#### Crafting Achievements
`DCT.Achievement.DIY` (craft 15 different items)  
`DCT.Achievement.Artisan` (craft 30 different items)  
`DCT.Achievement.CraftMaster` (craft all items)  

---
#### Fishing Achievements
`DCT.Achievement.Fisherman` (catch 10 different fish)  
`DCT.Achievement.OlMariner` (catch 24 different fish)  
`DCT.Achievement.MasterAngler` (catch every fish)  
`DCT.Achievement.MotherCatch` (catch 100 fish total)  

---
#### Museum Achievements
`DCT.Achievement.TreasureTrove` (donate 40 museum items)  
`DCT.Achievement.ACompleteCollection` (donate all museum items)  

---
#### Help Quest Achievements
`DCT.Achievement.Gofer` (complete 10 help quests)  
`DCT.Achievement.ABigHelp` (complete 40 help quests)  

---
#### Shipping Achievements
`DCT.Achievement.Polyculture` (ship 15 of each crop)  
`DCT.Achievement.Monoculture` (ship 300 of one crop)  
`DCT.Achievement.FullShipment` (ship 1 of everything)  

---
#### Steam Achievements
`DCT.Achievement.PrairieKing` (beat JOTPK)  
`DCT.Achievement.TheBottom` (reach mines level 120)  
`DCT.Achievement.LocalLegend` (restore Community Center)  
`DCT.Achievement.JojaCoMemberOfTheYear` (purchase all Joja developments)  
`DCT.Achievement.MysteryOfTheStardrops` (find every stardrop)  
`DCT.Achievement.FullHouse` (married with 2 kids)  
`DCT.Achievement.SingularTalent` (level 10 in 1 skill)  
`DCT.Achievement.MasterOfTheFiveWays` (level 10 in all skills)  
`DCT.Achievement.ProtectorOfTheValley` (complete all monster slayer goals)  
`DCT.Achievement.FectorsChallenge` (JOTPK deathless)  

---
</details>

### Festival Days (18)
These dialogue keys allow new NPC dialogue responses based around certain festival events. Dialogues may depend on the outcome of a festival.

<details>
  <summary>Click to expand list of festival dialogue keys. <b>WARNING: SPOILERS</b></summary><br/>

`DCT.Festival.EggHunt`  
`DCT.Festival.EggHunt-lose` (Abigail wins the egg hunt)  
`DCT.Festival.EggHunt-win` (you win the egg hunt)  

`DCT.Festival.FlowerDance`  
`DCT.Festival.FlowerDance-dance` (you participate with a partner in the Flower Dance)  
`DCT.Festival.FlowerDance-solo` (you don't dance with a partner)  

`DCT.Festival.Luau`  
`DCT.Festival.Luau-missing` (at least one player didn't add anything to the Soup)  
`DCT.Festival.Luau-best` (best soup ever)  
`DCT.Festival.Luau-good` (very pleasant soup)  
`DCT.Festival.Luau-neutral` (average soup)  
`DCT.Festival.Luau-bad` (disguting soup)  
`DCT.Festival.Luau-worst` (vile, poisonous soup)  
`DCT.Festival.Luau-shorts` (Lewis' shorts are in the soup)  

`DCT.Festival.Jellies`  

`DCT.Festival.GrangeDisplay`
`DCT.Festival.GrangeDisplay-lose` (Pierre wins the grange display contest)  
`DCT.Festival.GrangeDisplay-win` (you win the the grange display contest)  
`DCT.Festival.GrangeDisplay-shorts` (you put Lewis' shorts in your grange display)

`DCT.Festival.SpiritsEve`  

`DCT.Festival.IceFishing`  
`DCT.Festival.IceFishing-lose` (Willy wins the ice fishing contest)  
`DCT.Festival.IceFishing-player` (another player wins the ice fishing contest)  
`DCT.Festival.IceFishing-win` (you win the ice fishing contest)  

`DCT.Festival.WinterStar`  
`DCT.Festival.WinterStar-prep` (lasts from Winter 18-24 in the leadup to the Feast of the Winter Star)  
`DCT.Festival.WinterStar-gifts` (participate in the gift exchange)

---
</details>

### Heart Events (124) + 73
These dialogue keys allow new NPC dialogue responses after a player views an NPC's heart event. Some events with player choice have multiple keys, allowing NPCs to respond based on choices made by the player.

<details>
  <summary>Click to expand list of heart event dialogue keys. <b>WARNING: SPOILERS</b></summary>

#### Alex Heart Events
`DCT.Event.Alex2Heart` (Beach gridball catch) #CH question fork1 arrogantJosh  
- `DCT.Event.Alex2Heart-believe` (tell Alex you believe in him)  
- `DCT.Event.Alex2Heart-arrogant` (tell Alex he's really arrogant)  

`DCT.Event.Alex4Heart` (Town Dusty sad) #CH question fork1 didntHear  
- `DCT.Event.Alex4Heart-heard` (tell Alex you heard everything)  
- `DCT.Event.Alex4Heart-didnthear` (tell Alex you didn't overhear)  

`DCT.Event.Alex5Heart` (JoshHouse no books) #CH $q 57 $r 57 Event_books1 2 3  
`DCT.Event.Alex6Heart` (JoshHouse working out)  
`DCT.Event.Alex8Heart` (Beach mother died) #CH $q -1 #r event_box1 2 3 4  
`DCT.Event.Alex10Heart` (Saloon dinner date) #CH question fork1 rejectJosh  
- `DCT.Event.Alex10Heart-feelings` (tell Alex you have feelings for him)  
- `DCT.Event.Alex10Heart-reject` (reject Alex)  

`DCT.Event.Alex14Heart1` (Farm Alex asks for money for a secret project)  
- `DCT.Event.Alex14Heart1-cheapskate` (refuse to give Alex the money)      
- `DCT.Event.Alex14Heart1-givemoney` (give Alex 5,000g for his project)
`DCT.Event.Alex14Heart2` (Town a message pops up to check out the Saloon) - worldstate change  
`DCT.Event.Alex14Heart3` (Saloon Alex shows off the new sports room)  

---
#### Elliott Heart Events
`DCT.Event.Elliott2Heart` (ElliottHouse book genre) #CH $q 958699 $r event_idea1 2 3  
`DCT.Event.Elliott4Heart` (Saloon toast) #CH $q 28376 $r event_toast4 2 1 3  
`DCT.Event.Elliott6Heart` (Elliott piano) #CH question fork1 howLong question fork1 extrahelp  
- `DCT.Event.Elliott6Heart-wonderful` (tell Elliott his playing is wonderful)  
- `DCT.Event.Elliott6Heart-howlong` (ask Elliott how long he's been playing)  
- `DCT.Event.Elliott6Heart-hard` (tell Elliott that being a farmer is just as hard)  
- `DCT.Event.Elliott6Heart-extrahelp` (tell Elliott he should come live on the farm and help out)  

`DCT.Event.Elliott8Heart` (ArchaeologyHouse book reading)  
- `DCT.Event.Elliott8Heart` (mystery book)  
- `DCT.Event.Elliott8Heart` (romance book)  
- `DCT.Event.Elliott8Heart` (sci-fi book)  
`DCT.Event.Elliott10Heart` (Beach boat ride) #CH fork1 NoToElliott, $q $r event_boat1 2 fork tooBold  
- `DCT.Event.Elliott10Heart-no` (reject Elliott's offer of a boat ride) (end dialogue Elliott)  
- `DCT.Event.Elliott10Heart-happy` (tell Elliott you're trembling with happiness)  
- `DCT.Event.Elliott10Heart-reject` (tell Elliott you're uncomfortable) (end dialogue Elliott)  

`DCT.Event.Elliott14Heart1` (Farm book tour) addConversationTopic elliottGone (end dialogue Elliott) (+7 more)  
`DCT.Event.Elliott14Heart2` (FarmHouse return from book tour) (end dialogue Elliott)

---
#### Harvey Heart Events
`DCT.Event.Harvey2Heart` (JoshHouse George's checkup) #CH $q 84 $r event_george1 2  
`DCT.Event.Harvey4Heart` (Hospital doctor checkup) #CH $q 86 $r event_heart1 2 3  
`DCT.Event.Harvey6Heart` (SeedShop aerobics) #CH $q -1 $r event_aerobics1 2 (end dialogue Harvey)  
`DCT.Event.Harvey8Heart` (Hospital radio pilot) #CH question fork1 normal  
- `DCT.Event.Harvey8Heart-flustered` (ask Harvey why he's all flustered)  
- `DCT.Event.Harvey8Heart-normal` (pretend like everything's normal)  

`DCT.Event.Harvey10Heart` (Railroad balloon ride) #CH question fork1 afraid  
- `DCT.Event.Harvey10Heart-fun` (tell Harvey it looks like fun)  
- `DCT.Event.Harvey10Heart-afraid` (remind Harvey he's afraid of heights)  

`DCT.Event.Harvey14Heart` (FarmHouse pasta dinner) #CH quickQuestion quickQuestion  

---
#### Sam Heart Events
`DCT.Event.Sam2Heart` (SamHouse music genre) #CH $q 76 $r Event_band1 2 3 4  
`DCT.Event.Sam3Heart` (Beach vincent kent gone) #CH question null splitSpeak ~ (end dialogue Sam)  
`DCT.Event.Sam4Heart` (SamHouse egg drop) #CH $q 80 $r event_snack1 2 3 (end dialogue Sam)  
`DCT.Event.Sam6Heart` (Town skateboard) #CH question null splitSpeak Sam Lewis Sam  
`DCT.Event.Sam8Heart1` (Farm band invite) (end dialogue Sam)  
`DCT.Event.Sam8Heart2` (BusStop band show) cutscene bandFork poppy/heavy/techno/honkytonk (end dialogue Sam)  
- `DCT.Event.Sam8Heart2-poppy` (Sam's band plays cheerful pop music) (end dialogue Sam)   
- `DCT.Event.Sam8Heart2-heavy` (Sam's band plays experimental noise rock) (end dialogue Sam)   
- `DCT.Event.Sam8Heart2-techno` (Sam's band plays hi-energy dance music) (end dialogue Sam)   
- `DCT.Event.Sam8Heart2-honkytonk` (Sam's band plays honky-tonky country music) (end dialogue Sam)   

`DCT.Event.Sam10Heart` (Town sleep over) #CH question fork1 stayPut, fork1 rejectSam  
- `DCT.Event.Sam10Heart-stayput` (stay put in the bed after Jodi leaves)  
- `DCT.Event.Sam10Heart-closer` (get out of the bed, but move closer when Sam confesses feelings)  
- `DCT.Event.Sam10Heart-window` (get out of the bed, then reject Sam and head for the window)  

`DCT.Event.Sam14Heart1` (FarmHouse lazy) addConversationTopic samJob1 2  
`DCT.Event.Sam14Heart2` (FarmHouse job offer) #CH quickQuestion quickQuestion addConversationTopic samJob2 2  
`DCT.Event.Sam14Heart3` (FarmHouse song writing) addConversationTopic samJob3 3  
`DCT.Event.Sam14Heart4` (FarmHouse boombox)  

---
#### Sebastian Heart Events
`DCT.Event.Sebastian2Heart` (SebastianRoom computer) #CH question fork1 didntLeave switchEvent sebastianRoom resetVariable question fork1 decor switchEvent enterRobin resetVariable question fork1 noFriends  
- `DCT.Event.Sebastian2Heart-leave` (Sebastian looks busy: try to leave)  
- `DCT.Event.Sebastian2Heart-stayput` (Sebastian looks busy: stay put)  
- `DCT.Event.Sebastian2Heart-work` (ask Sebastian what he's working on)  
- `DCT.Event.Sebastian2Heart-decor` (compliment the decor in Sebastian's room)  
- `DCT.Event.Sebastian2Heart-career` (after Robin leaves: ask about his career goals)  
- `DCT.Event.Sebastian2Heart-nofriends` (after Robin leaves: ask why he doesn't see his friends)  

`DCT.Event.Sebastian4Heart` (Mountain motorcycle work) #CH $q -1 $r event_garage1 2 3  
`DCT.Event.Sebastian6Heart` (SebastianRoom game) #CH question chooseCharacter (fork warrior|fork healer) addMailReceived (choseWarrior | choseHealer | choseWizard) switchEvent opening
                               resetVariable question fork1 backEntrance resetVariable question fork1 ranAway resetVariable question fork0 swungWeapons addMailReceived killedSkeleton switchEvent sewer
                               resetVariable question fork1 wizardDoor switchEvent podRoom
                               resetVariable question fork0 leave addMailReceived destroyedPods switchEvent wizardDoor
                               resetVariable switchEvent Necromancer
                               resetVariable fork choseWizard finalBossWizard, fork choseWarrior finalBossWarrior question fork0 healedSam addMailReceived savedFriends switchEvent end
finalBossWizard: resetVariable question fork0 castBeam addMailReceived savedFriends switchEvent end
finalBossWarrior: resetVariable question fork0 chargeAhead addMailReceived savedFriends switchEvent end  
- `DCT.Event.Sebastian6Heart-scoreA` (best rating: killed skeleton, destroyed pods, and saved friends)   
- `DCT.Event.Sebastian6Heart-scoreB` (good rating: accomplished two of the objectives)   
- `DCT.Event.Sebastian6Heart-scoreC` (fair rating: accomplished one of the objectives)   
- `DCT.Event.Sebastian6Heart-scoreD` (poor rating: did not accomplish any objectives)  

`DCT.Event.Sebastian8Heart` (Beach boardwalk)  
`DCT.Event.Sebastian10Heart` (Mountain motorcycle ride) #CH $q -1 $r event_city1 2 3 4  
`DCT.Event.Sebastian14Heart1` (Mountain frog rescue) #CH quickQuestion addConversationTopic sebastianFrog 0  
`DCT.Event.Sebastian14Heart2` (FarmHouse terrarium) #CH quickQuestion quickQuestion  

---
#### Shane Heart Events
`DCT.Event.Shane2Heart` (Forest share beer)  
`DCT.Event.Shane4Heart` (AnimalShop passed out)  
`DCT.Event.Shane6Heart1` (Forest cliffs suicidal) #CH question shaneCliffs Event.cs.1760, 1761, 1763, 1764 (end invisible Shane)  
`DCT.Event.Shane6Heart2` (Farm apology, counselling) #CH $q -1 $r event_apologize1 2 3  
`DCT.Event.Shane7Heart` (AnimalShop sparkling water)  
`DCT.Event.Shane7HeartClintEmily2Heart` (Town Joja contest) (end dialogue Shane)  
`DCT.Event.Shane8Heart` (AnimalShop blue chickens)  
`DCT.Event.Shane10Heart1` (Farm gridball invite)  
`DCT.Event.Shane10Heart2` (BusStop gridball game) #CH $q -1 $r event_stadium1 2  
`DCT.Event.Shane14Heart1` (Town saloon) addConversationTopic shaneSaloon1
                                              (end dialogue Shane)  
`DCT.Event.Shane14Heart2` (Town confront) addConversationTopic shaneSaloon2
                                              (end dialogue Shane)  
`DCT.Event.Shane14Heart3` (Town arcade) #CH question fork0 wewereworried
                                              (end dialogue Shane)  
- `DCT.Event.Shane14Heart3-worried` (tell Shane you had been worried about him) (end dialogue Shane)  
- `DCT.Event.Shane14Heart3-sorry` (apologize to Shane for not believing him) (end dialogue Shane)  

---
#### Abigail Heart Events
`DCT.Event.Abigail2Heart` (SeedShop JOTPK co-op) #CH cutscene AbigailGame fork beatGame  
- `DCT.Event.Abigail2Heart-lostgame` (if you lose the game)  
- `DCT.Event.Abigail2Heart-beatgame` (if you and Abigail beat the level together)  

`DCT.Event.Abigail4Heart` (Mountain flute duet) #CH $q 32 Event_Rain_1 2 3  
`DCT.Event.Abigail6Heart` (Town graveyard) #CH $q 847951 Event_Grave1 2 3 4  
`DCT.Event.Abigail8Heart1` (SeedShop spirit board)  
`DCT.Event.Abigail8Heart2` (Farm apology)  
`DCT.Event.Abigail10Heart` (Mine scary bats) #CH $q 776589 $r Event_Cave2_1 2 $q 34 $r Event_Cave_1 2 3  
`DCT.Event.Abigail14Heart` (Backwoods monster attack) #CH quickQuestion  

---
#### Emily Heart Events
`DCT.Event.Emily2Heart` (HaleyHouse dreamscape)  
`DCT.Event.Emily4Heart` (Town parrot rescue)  
`DCT.Event.Emily6Heart` (HaleyHouse dance performance) #CH $q 213 $r event_dance1 2 1 (end dialogue Emily)  
`DCT.Event.Emily8Heart` (ManorHouse clothing therapy)  
`DCT.Event.Emily10Heart` (Woods camping trip)  
`DCT.Event.Emily14Heart1` (Farm fiber - Errand for your Wife quest hook) (end dialogue Emily)  
`DCT.Event.Emily14Heart2` (FarmHouse new outfit) (end dialogue Emily)  

---
#### Haley Heart Events
`DCT.Event.Haley2Heart` (HaleyHouse chores fight) #CH $q 45 $r Event_clean2 1 3 fork haleyWontDoIt
haleyWontDoIt:
- `DCT.Event.Haley2Heart-will` (Haley agrees to clean under the cushions)
- `DCT.Event.Haley2Heart-wont` (Haley won't do the cleaning)
`DCT.Event.Haley4Heart` (HaleyHouse open jar) #CH $q 47 $r Event_jar1 2  
`DCT.Event.Haley6Heart` (Beach lost bracelet) #CH $q $r Event_beach1 2 (end dialogue Haley)  
`DCT.Event.Haley8Heart` (Forest cow photos) (end dialogue Haley)  
`DCT.Event.Haley10Heart` (HaleyHouse dark room) #CH $q -1 Event_darkroom1 2 3 question haleyDarkRoom fork decorate fork leave (end dialogue Haley)
decorate: (end dialogue Haley)
leave: (end dialogue Haley)  
- `DCT.Event.Haley10Heart-decorate` (offer to help decorate the dark room)  
- `DCT.Event.Haley10Heart-leave` (make an excuse and leave)  
- `DCT.Event.Haley10Heart-kiss` (try to kiss Haley)  

`DCT.Event.Haley14Heart1` (Town cake idea) addConversationTopic haleyCakewalk1 0 (end dialogue Haley)  
`DCT.Event.Haley14Heart2` (FarmHouse request - Haley's Cake-Walk quest hook) #CH quickQuestion (end dialogue Haley) addConversationTopic haleyCakewalk2 0  
`DCT.Event.Haley14Heart3` (Town cake walk event)  

---
#### Leah Heart Events
`DCT.Event.Leah2Heart` (LeahHouse sculpting) #CH $q -1 $r event_sculpt1 2 3 (%fork playermale 3) fork creepySexualPass question fork1 fork internet addMailReceived LeahArtShowSuggestion (end dialogueWarpOut Leah)
creepySexualPass: (end dialogueWarpOut Leah)
internet: addMailReceived LeahInternet (end warpOut)  
- `DCT.Event.Leah2Heart-creepy` (make a creepy pass at Leah)  
- `DCT.Event.Leah2Heart-internet` (suggest selling her art on the internet)  
- `DCT.Event.Leah2Heart-artshow` (suggest holding an art show in town )  

`DCT.Event.Leah4Heart` (LeahHouse phone call) #CH $q 83 $r event_parents1 2 3 4 5 (%fork option 3 staying in city) fork angry fork LeahInternet internet2 fork LeahArtShowSuggestion artShowSuggest (end dialogueWarpOut Leah)
angry: (end warpOut)
internet2: (end warpOut)
artShowSuggest: (end warpOut)  
- `DCT.Event.Leah4Heart-angry` (tell Leah she would've been better off staying in the city)    
- `DCT.Event.Leah4Heart-internet` (Leah is getting a computer to sell art online like you suggested)  
- `DCT.Event.Leah4Heart-artshow` (Leah is preparing sculptures for the art show you suggested)  
- `DCT.Event.Leah4Heart-nosuggestion` (you were creepy in her 2-heart event but didn't make her angry in this one)  

`DCT.Event.Leah6HeartA` (Farm sculpture gift)  
`DCT.Event.Leah6HeartB` (Forest reach fruit) (end dialogue Leah)  
`DCT.Event.Leah8HeartA1` (Farm art show invite) (end dialogue Leah)  
`DCT.Event.Leah8HeartA2` (Town art show) (end dialogue Leah)  
`DCT.Event.Leah8HeartB` (LeahHouse art website)  
`DCT.Event.Leah10Heart` (Forest picnic ex) #CH fork LeahInternet choseInternet (in both forks:) question fork1 noPunch (end dialogue Leah) all endings  
- `DCT.Event.Leah10Heart-violent` (Kel shows up at the picnic and you punch him/her)  
- `DCT.Event.Leah10Heart-nopunch` (you don't punch Kel, and Leah does instead)  

`DCT.Event.Leah14Heart1` (Farm painting idea addConversationTopic leahPaint 0 (end dialogue Leah)  
`DCT.Event.Leah14Heart2` (Forest painting class) #CH quickQuestion addWorldState m_painting0, m_painting1, m_painting2 quickQuestion (end dialogue Leah) (can I insert with \\addConversationTopic country retro modern)  

---
#### Maru Heart Events
`DCT.Event.Maru2Heart` (ScienceHouse soil samples) #CH $q 15933 $r Event_Lab_Silence Event_Lab_Rat (Rat has the %fork) fork DadWeird (end Maru1)
DadWeird: (end Maru1)  
- `DCT.Event.Maru2Heart-silence` (don't mention what Demetrius said)  
- `DCT.Event.Maru2Heart-dadweird` (tell Maru her dad was being weird)  

`DCT.Event.Maru4Heart` (Hospital broken beaker) #CH $q 38 $r 38/39 Event_Hospital_1 2 3 fork toldTruth  
- `DCT.Event.Maru4Heart-blame` (tell Maru to blame the dropped sample on you)  
- `DCT.Event.Maru4Heart-truth` (tell her to scoop up the sample OR just say it was an accident)  

`DCT.Event.Maru6Heart` (Mountain telescope) #CH $q 40 Event_Space1 Event_Space2  
`DCT.Event.Maru8Heart` (ScienceHouse electrocuted) #CH $q 41 $r Event_Cut1 2  
`DCT.Event.Maru10Heart` (ScienceHouse MarILDA robot) #CH $q 18981 event_robot1 2 3 4 (%fork for #2, slave) fork BadAnswer
BadAnswer: $q 18982 $r event_robot_explain1 2 3  
- `DCT.Event.Maru10Heart-badanswer` (tell Maru that she should have made the robot her slave)  
- `DCT.Event.Maru10Heart-positive` (give literally any other answer)  

`DCT.Event.Maru14Heart1` (FarmHouse astronomy invite)  
`DCT.Event.Maru14Heart2` (Mountain astronomy event) #CH quickQuestion  

---
#### Penny Heart Events
`DCT.Event.Penny2Heart` (Town George mail) #CH $q 71 $r event_mail1 2 3 $q -1 $r event_old1 2 3 4  
`DCT.Event.Penny4Heart` (Trailer messy house)  
`DCT.Event.Penny6Heart` (Trailer new recipe) #CH $q 72 $r event_cook1 72 event_cook2 73 event_cook3 72 $p 72 chili de player | failure (end dialogue Penny)  
`DCT.Event.Penny8Heart` (Forest field trip) #CH $q -1 $r event_speaker_yes, yes, no (%fork) fork eventEnd (end dialogue Penny) question fork0 (farming, gathering) fork choseFarming question fork0 (minerals, fishing, lumber) fork choseMinerals (Vincent asks marry Penny/boyfriend) switchEvent fieldTripEnd
choseFarming: question fork0 (vegetables, animals) fork choseAnimals (Vincent asks Sam strong Penny) switchEvent fieldTripEnd
choseMinerals: (Vincent asks monsters) switchEvent fieldTripEnd
choseAnimals: (Vincent asks saddle cowboy) switchEvent fieldTripEnd
fieldTripEnd: $q -1 $r event_speaker_kids1 2 3 4 5 6 (end dialogue Penny)  
- `DCT.Event.Penny8Heart-hatekids` (refuse Penny's request and tell her you can't stand kids) (end dialogue Penny)  
- `DCT.Event.Penny8Heart-vegetables` (choose farming, vegetables; Vincent talks about Penny and Sam) (end dialogue Penny)  
- `DCT.Event.Penny8Heart-animals` (choose farming, animals; Vincent wants to be a cowboy) (end dialogue Penny)  
- `DCT.Event.Penny8Heart-minerals` (choose gathering, minerals; Vincent asks about goblins) (end dialogue Penny)  
- `DCT.Event.Penny8Heart-seafoodlumber` (choose gathering, seafood/lumber; Vincent asks about your love life) (end dialogue Penny)  

`DCT.Event.Penny10Heart` (BathHouse_Pool pool meeting) #CH $q -1 $r event_pool1 2 3 $q -1 $r event_pool4 5 (5 has the %fork) fork pennyHeartbroken (end warpOut)
pennyHeartbroken: (end warpOut)  
- `DCT.Event.Penny10Heart-feelings` (tell Penny you feel the same way)  
- `DCT.Event.Penny10Heart-reject` (reject Penny)  

`DCT.Event.Penny14Heart1` (FarmHouse redecorate question) #CH quickQuestion (3) quickQuestion (addConversationTopic pennyRedecorating 2\\addMailReceived pennyQuilt0 | addConversationTopic pennyRedecorating 2\\addMailReceived pennyQuilt1 | addConversationTopic pennyRedecorating 2\\addMailReceived pennyQuilt2 | addMailReceived noQuilt (end dialogue Penny)  
`DCT.Event.Penny14Heart2` (FarmHouse redecorate finished) #CH quickQuestion  

---
#### Bachelor(ette) Heart Events
`DCT.Event.Bachelors10HeartA` (Saloon dump guys) question fork1 fork choseToExplain (dump guys 3)
choseToExplain: resetVariable question fork2 fork crying (dump guys 4)
crying: (dump guys 4)  
- `DCT.Event.Bachelors10HeartA-wrong` (apologize and say what you did was wrong)  
- `DCT.Event.Bachelors10HeartA-blame` (choose to explain, then blame the guys or blame Pierre)  
- `DCT.Event.Bachelors10HeartA-crying` (choose to explain, then start crying)  

`DCT.Event.Bachelors10HeartB` (Saloon play pool)  
`DCT.Event.Bachelorettes10HeartA` (HaleyHouse dump girls) question fork1 fork choseToExplain (dump girls 3)
choseToExplain: resetVariable question fork2 fork lifestyleChoice (dump girls 4)
lifestyleChoice: (dump girls 4)  
- `DCT.Event.Bachelorettes10HeartA-wrong` (apologize and say what you did was wrong)  
- `DCT.Event.Bachelorettes10HeartA-blame` (choose to explain, then play dumb or blame Pierre)  
- `DCT.Event.Bachelorettes10HeartA-lifestyle` (choose to explain, then say it's just a lifestyle choice)  

`DCT.Event.Bachelorettes10HeartB` (HaleyHouse gossip)  

---
#### Other Villager Heart Events
`DCT.Event.Caroline2Heart` (Sunroom tea bushes) #CH quickQuestion a/b/c/d, Y/N  
`DCT.Event.Caroline6Heart` (SeedShop argues with Abigail) (end dialogue Abigail)  
`DCT.Event.Clint3Heart` (Saloon advice re. girls) #CH $q $r event_advice1 1 2 1  
`DCT.Event.Clint6Heart` (Town - from Forest - asks Emily out) (end dialogue Clint)  
`DCT.Event.Demetrius6Heart` (ScienceHouse tomato) #CH $q 59 $r Event_tomato1 2  
`DCT.Event.Dwarf50Point` (Sewer krobus fight)  
`DCT.Event.Evelyn4Heart` (JoshHouse baking cookies) #CH$q $r Event_cookies1 2  
`DCT.Event.George6Heart` (JoshHouse bookshelf wheelchair)  
`DCT.Event.Gus4HeartPam2Heart` (Saloon Pam pays her tab) #CH $q $r event_credit1 2  
`DCT.Event.Gus5Heart` (Farm mini-jukebox)  
`DCT.Event.JasVincent8Heart` (Forest spring onions)  
`DCT.Event.Jodi4Heart` (Farm Fish Casserole quest hook?) (end dialogue Jodi)  
`DCT.Event.Kent3Heart` (SamHouse popcorn) #CH $q $r event_popcorn1 2 3  
`DCT.Event.Krobus14Heart` (Beach sea monster ride)  
`DCT.Event.LewisMarnie6Heart` (Town gossip) #CH $q $r event_secret1 2  
`DCT.Event.Linus50Point` (Town george raccoons) #CH $y choices only  
`DCT.Event.Linus4Heart` (Mountain wild bait tent)  
`DCT.Event.Linus8Heart` (Mountain - from Robin's - farm) #CH question fork0 linusWell  
- `DCT.Event.Linus8Heart-well` (say you're just pleased to see that Linus is doing well)  
- `DCT.Event.Linus8Heart-livefarm` (invite Linus to live on the farm with you)  

`DCT.Event.Marnie3Heart` (Farm cave carrot Marnie's Request quest hook) (end dialogue Marnie)  
`DCT.Event.Pam9Heart` (Trailer_Big praying to Yoba) #CH question fork0 positive  
- `DCT.Event.Pam9Heart-hopeful` (say you're glad that Pam is feeling hopeful)  
- `DCT.Event.Pam9Heart-noyoba` (tell Pam that Yoba isn't real)  

`DCT.Event.Pierre6Heart` (SeedShop secret stash) #CH $q $r Event_naga1 2  
`DCT.Event.Robin6Heart` (ScienceHouse drum and flute block) #CH $q $r event_wood1 2  
`DCT.Event.Willy6Heart` (Beach crabs problem) addConversationTopic willyCrabs  

---
</details>

### Quests (55)
These dialogue keys allow new NPC dialogue responses after a player completes a quest. (Maybe when they start one, too?)

<details>
  <summary>Click to expand list of quest dialogue keys. <b>WARNING: SPOILERS</b></summary><br/>

`DCT.Quest.Introductions` (meet 28 villagers)  
`DCT.Quest.HowToWinFriends` (give someone a gift)  
`DCT.Quest.GettingStarted` (harvest a parsnip)  
`DCT.Quest.ToTheBeach` (visit Willy at the beach)  
`DCT.Quest.RaisingAnimals` (build a coop)  
`DCT.Quest.Advancement` (craft a scarecrow)  
`DCT.Quest.ExploreTheMine` (reach level 5 in the mines)  
`DCT.Quest.DeeperInTheMine` (reach level 40 in the mines)  
`DCT.Quest.ToTheBottom` (reach the bottom of the mines) \*\*also a Steam achievement  
`DCT.Quest.Archaeology.Part1` (visit Gunther at the museum)  
`DCT.Quest.Archaeology.Part2` (donate an item to the museum)  
`DCT.Quest.RatProblem` (examine the golden scroll in the Community Center)  
`DCT.Quest.MeetTheWizard` (enter the Wizard's tower after recieving letter)  
`DCT.Quest.ForgingAhead` (craft a furnace)  
`DCT.Quest.Smelting` (smelt a copper bar)  
`DCT.Quest.Initiation.Part1` (slay 10 slimes in the mines)  
`DCT.Quest.Initiation.Part2` (enter the Adventurer's Guild)  
`DCT.Quest.RobinsLostAxe` (return Robin's lost axe)  
`DCT.Quest.JodisRequest` (bring Jodi a cauliflower)  
`DCT.Quest.MayorsShorts` (return the mayor's shorts)  
`DCT.Quest.BlackberryBasket` (return Linus' berry basket)  
`DCT.Quest.MarniesRequest` (bring a cave carrot to Marnie's shop)  
`DCT.Quest.PamIsThirsty` (bring Pam a pale ale)  
`DCT.Quest.ADarkReagent` (bring the Wizard a void essence)  
`DCT.Quest.CowsDelight` (bring Marnie amaranth)  
`DCT.Quest.TheSkullKey` (find what the Skull Key is for)  
`DCT.Quest.CropResearch` (bring Demetrius a melon)  
`DCT.Quest.KneeTherapy` (bring George a hot pepper)  
`DCT.Quest.RobinsRequest` (bring Robin 10 hardwood)  
`DCT.Quest.QisChallenge` (reach level 25 of the skull cavern)  
`DCT.Quest.TheMysteriousQi.Part0` (put a battery pack in the bus tunnel)  
`DCT.Quest.TheMysteriousQi.Part1` (put a rainbow shell in the railroad box)  
`DCT.Quest.TheMysteriousQi.Part2` (put beets in the mayor's fridge)  
`DCT.Quest.TheMysteriousQi.Part3` (feed the sand dragon a solar essence)  
`DCT.Quest.TheMysteriousQi.Part4` (find the club card in your lumber pile)  
`DCT.Quest.CarvingPumpkins` (bring Caroline a pumpkin)  
`DCT.Quest.AWinterMystery` (find the shadowy figure)  
`DCT.Quest.StrangeNote` (bring maple syrup to the Secret Woods)  
`DCT.Quest.CrypticNote` (reach level 100 of the skull cavern)  
`DCT.Quest.FreshFruit` (bring Emily an apricot)  
`DCT.Quest.AquaticResearch` (bring Demetrius a pufferfish)  
`DCT.Quest.ASoldiersStar` (bring Kent a starfruit)  
`DCT.Quest.MayorsNeed` (bring Lewis truffle oil)  
`DCT.Quest.WantedLobster` (bring Gus a lobster)  
`DCT.Quest.PamNeedsJuice` (bring Pam a battery pack)  
`DCT.Quest.FishCasserole` (bring a largemouth bass to Jodi's house) \*\*quest hook is a Heart Event  
`DCT.Quest.CatchASquid` (bring Willy a squid)  
`DCT.Quest.FishStew` (bring Gus an Albacore)  
`DCT.Quest.PierresNotice` (bring Pierre sashimi)  
`DCT.Quest.ClintsAttempt` (bring Emily an amethyst)  
`DCT.Quest.AFavorForClint` (bring Clint an iron bar)  
`DCT.Quest.StaffOfPower` (bring the Wizard an iridium bar)  
`DCT.Quest.GrannysGift` (bring Evelyn a leek)  
`DCT.Quest.ExoticSpirits` (bring Gus a coconut)  
`DCT.Quest.CatchaLingcod` (bring Willy a lingcod)  
`DCT.Quest.DarkTalisman` (retrieve the dark talisman from the sewers and return with it to the railroad)  
`DCT.Quest.GoblinProblem` (retrieve the magic ink from the witch's hut and bring it to the Wizard)  

---
</details>

### Secret Notes (12) + 2
These dialogue keys allow new NPC dialogue responses after a player finishes a task from certain secret notes.

<details>
  <summary>Click to expand list of secret note dialogue keys. <b>WARNING: SPOILERS</b></summary><br/>

  // Note10 (see DCT.Quest.CrypticNote)
`DCT.Secret.Note13` (junimo plush by playground)  
`DCT.Secret.Note14` (stone junimo statue behind CC)  
`DCT.Secret.Note15` (pearl from mermaid show)  
`DCT.Secret.Note16` (railroad treasure chest)  
`DCT.Secret.Note17` (river strange doll green)  
`DCT.Secret.Note18` (desert strange doll yellow)  
`DCT.Secret.Note19` (solid gold Lewis statue)  
- `DCT.Secret.Note19-lewismad`(when you place the statue in town and Lewis gets mad)  
- `DCT.Secret.Note19-again`(when you do it again)  

`DCT.Secret.Note20` (special charm from bus driver)  
`DCT.Secret.Note21` (Marnie and Lewis in the bush)  
`DCT.Secret.Note22` (place battery pack in bus tunnel - here? or under Quests?)  
  // Note23 (see DCT.Quest.StrangeNote) may-pal serrup  
`DCT.Secret.Note25` (return necklace to Abigail or Caroline)  
- `DCT.Secret.Note25-abigail` (return necklace to Abigail)  
- `DCT.Secret.Note25-caroline` (return necklace to Caroline)

---
</details>

### Story Triggers
These dialogue keys allow new NPC dialogue responses after various in-game events or accomplishments not covered by the other categories.

<details>
  <summary>Click to expand list of story trigger dialogue keys. <b>WARNING: SPOILERS</b></summary><br/>

**Note:** this category is **very** subject to change, and I will add the easiest ones first.

`DCT.Story.MarriageNPC-{{SpouseName}}` (maybe have the topic custom by spouse, for better compatibility with Multiple Spouses mod? and easier congratulations after re-marriage?)  
`DCT.Story.MarriagePlayer` (get married to another player... 14 days?)  
`DCT.Story.DivorceNPC-{{SpouseName}}` (e.g. `DCT.Story.DivorceHaley` or `DCT.Story.DivorceBartholomew`; these are made custom so that you can write in-law and friend responses)  
`DCT.Story.DivorcePlayer` (get divorced from a player)  
`DCT.Story.WipeMemory{{SpouseName}}` (e.g. `DCT.Story.WipeMemoryHaley` or `DCT.Story.WipeMemoryBartholomew`; these are made custom so that you can write in-law and friend responses)  
`DCT.Story.FirstChild` (first child is born or adopted)  \*\*Can I distinguish between adoption and birth??
`DCT.Story.SecondChild` (second child is born or adopted)  
`DCT.Story.DoveKids` (turn your kids to doves :c) \*\*need to wipe all active \*Child topics and all past \*Child and DoveKids dialogue responses.  
`DCT.Story.AdoptPet` (adopt a pet from Marnie)  
- `DCT.Story.AdoptPet-cat` (adopt a cat)  
- `DCT.Story.AdoptPet-dog` (adopt a dog)  
- `DCT.Story.AdoptPet-nocat` (refuse to adopt the cat)  
- `DCT.Story.AdoptPet-nodog` (refuse to adopt the pet)  

`DCT.Story.RustySword` (Marlon gives you a rusty sword to use in the mines) (end dialogue Demetrius) 
`DCT.Story.DemetriusCave` (visit from Demetrius about using the cave) (end dialogue Demetrius)  
- `DCT.Story.DemetriusCave-mushrooms` (choose mushrooms)  
- `DCT.Story.DemetriusCave-bats` (choose fruit bats)  

`DCT.Story.EmilyCloth` (Emily visits to tell you about her sewing machine)  
`DCT.Story.JasVincentSewer` (Vincent and Jas are playing near the sewer door)  
`DCT.Story.RustyKey` (Gunther visits and gives you the Rusty Key)  
`DCT.Story.KentReturn` (Kent returns to the valley)  
`DCT.Story.SlimeHutch` (Marlon comes to talk about slimes)  
`DCT.Story.PierreSeeds` (Pierre will be selling new seed varieties next year)  
`DCT.Story.TrashBear-arrive` (trash bear arrives in Y3)  
`DCT.Story.TrashBear-feed` (give trash bear food)  
`DCT.Story.TrashBear-clean` (trash bear is appeased and cleans up the trash)  
`DCT.Story.Cellar` (third house upgrade)  
`DCT.Story.Rarecrows` (letter from Rarecrow Society & deluxe scarecrow recipe)  

`DCT.Story.DesertVisit` (first visit to the Desert)  
`DCT.Story.CasinoVisit` (first visit to the Casino)  
`DCT.Story.WoodsVisit` (first visit to the Secret Woods)  
`DCT.Story.RailroadVisit` (first visit to the Railroad)  
`DCT.Story.SpaVisit` (first visit to the Spa/Bathhouse)  
`DCT.Story.WitchVisit` (first visit to the Witch's Hut)  
`DCT.Story.SewerVisit` (first visit to the Sewer)  
`DCT.Story.CartVisit` (first visit to the Travelling Cart)??  
`DCT.Story.TraderVisit` (first visit to the Desert Trader)???  
`DCT.Story.MouseVisit` (first visit to the Hat Mouse)???  

`DCT.Story.MinesClear` (landslide blocking the mines is cleared Spring 5 Y1)  
`DCT.Story.RailroadClear` (earthquake clears the path to the railroad Summer 3 Y1)  
`DCT.Story.DwarfClear` (clear the rock blocking access to the Dwarf)  
`DCT.Story.WoodsClear` (clear the log blocking access to the Secret Woods)???  

`DCT.Story.GalaxySword` (you receive the galaxy sword)  
`DCT.Story.DwarfLanguage` (you learn the dwarf language)  

`DCT.Story.MutantCarp` (catch a Mutant Carp for the first time)  
`DCT.Story.Angler` (catch an Angler fish for the first time)  
`DCT.Story.Crimsonfish` (catch a Crimsonfish for the first time)  
`DCT.Story.Glacierfish` (catch a Glacierfish for the first time)  
`DCT.Story.Legend` (catch a Legend fish for the first time)  

#### More Ideas
Not sure yet if or how I'm going to implement these.
- Congradulations on high scores in games (definitely code needed here)
- Individual monster slayer goals
- New attractions to the museum (certain items?)
- respond to you dating someone (when relationship status changes? monitor it or Harmony patch the setter?)
- breakups - if you gave someone the wilted bonquet and their friends attack you for it 
- breakups (adults) "Aw, I'm sad you and __ broke up. You two were cute together."
- breakups "How dare you violate my friend!$a" "How dare you break my friends heart, you turd? >:'(" (depending if they are dating you or no?)
- More talk and events at the community center could be nice other than yayyy we used to all come here and have fun daily! no one enters in weeks when you fix it
- Wish more people talked about joja leaving
- 


- have more natural conversations such as having more options for you to respond to them
- react to slimes being into town 
- what if they noticed you ignoring them "Who are you again"
- talk about crops you sell in your shipping bin instead of just to Pierre
- Opinion on other people, which can get back to other people which then makes em annoyed at you
- Make Pam get mad when you beat her at ice fishing (Festival.IceFishing)


- Pierre is stocking new fertilizers (event 706, 707... mail triggers)
- Very first event Robin & Lewis? 60367 (do they have Introductions dialogue?) Vanilla!!!!

- Mail from mom, dad, tribune ids 68-76
- Emily mail trigger clothing therapy 2111194
- Emily mail trigger camping 2111294
- Grandpa candles 2146991 (nope)
- Alex mail trigger saloon date 2346091
- Sam mail trigger visit house 2346092
- Harvey mail trigger baloon ride 2346093
- Elliott mail trigger boat ride 2346094
- Elliott mail trigger book reading 2346095
- Penny mail trigger bath house 2346096
- Abigail mail trigger spirit board 2346097
- Pierre mail trigger new hours 3333094
- Elliott book tour mail triggers 3912126 3912127 3912128 3912129 3912130 3912131
- Bus fixed? (in vanilla) ...I think vanilla already has Conversation Topics for these?  
- PlayerKilled cutscenes (Events/Hospital and Events/Mine) can I add a counter?  
- First time passing out and/or dying?
- First chicken, duck, rabbit, dino, cow, goat, sheep, and pig (how to keep track?)  
- First stable/horse (which category does it fit?)  
- First coop (quest) + upgrades?, barn + upgrades?, silo, well, (big) shed, slime hutch (event), mill, fish pond, cabin(s), extra shipping bin(s)?  
- Community Upgrade (I think there's already a vanilla one for it?)  
- First Earth Obelisk, Water Obelisk, Desert Obelisk, Junimo hut and Gold Clock  
- Crafting/obtaining first sprinkler of any type?  
- Planting first fruit tree
- Tool Upgrades (so that Clint can ask about how they're working for you, maybe Robin too)
- First time cooking a recipe (eep probably too many of these...)
- Skill upgrades (comments on profession choice) hmmm
- Using the sewing machine and/or dye pots for the first time
- When you miss festivals entirely (they might ask where you were, or talk about what you missed)
- Opinion on other people, which can get back to other people which then makes em annoyed at you

---
</details>

## User Information
### COMPATIBILITY
- Stardew Valley v1.4 or later;
- Linux, Mac, Windows, and Android.
- Single-player and multiplayer. Can be installed by some OR all players - see Multiplayer section for details.

### INSTALLATION
- [Install the latest version of SMAPI.](https://smapi.io/)
- Download this mod from [Nexus](https://www.nexusmods.com/stardewvalley/mods/{{ADD_NUMBER}}) or the [GitHub Releases](https://github.com/Jonqora/StardewMods/releases) list.
- Unzip the mod and place the `DynamicConversationTopics` folder inside your `Mods` folder.
- Run the game using SMAPI.

### USING THE MOD
Once you install this mod, NPCs will begin to react to your new in-game activities and accomplishments. (**Note:** not every NPC reacts to each event. If you just saw someone's heart event, try talking to their friends and family members!)

## Multiplayer
Dynamic Conversation Topics is fully compatibile with multiplayer. This mod only affects gameplay for players who install it, and only the players who want to use this mod's features need to have it installed.


## Config Settings
After running SMAPI at least once with Dynamic Conversation Topics installed, a `config.json` file will be created inside the `DynamicConversationTopics` mod folder. Open it in any text editor to change your config settings for this mod.

**Optional:** Dynamic Conversation Topics includes [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (GMCM) support. If you download this optional mod, you can use a settings button in the Stardew Valley menu screen to change your DCT config while the game is running.


- **EnableAchievements:** Allows new NPC dialogue responses in the days after you fulfill criteria for a new achievement. Defaults to `true`.

- **EnableFestivals:** Allows new NPC dialogue responses in the days before or after certain festivals. Dialogues may depend on the outcome of any festival competition or event. Defaults to `true`.

- **EnableHeartEvents:** Allows new NPC dialogue responses in the days after you've seen someone's heart event. Dialogues may depend on the choices you made in the event. Defaults to `true`.

- **EnableQuests:** Allows new NPC dialogue responses in the days after you ((start?? or??)) complete a quest. Defaults to `true`.

- **EnableSecretNotes:** Allows new NPC dialogue responses in the days after you complete a task from certain secret notes. Defaults to `true`.

- **EnableStoryTriggers:** Allows new NPC dialogue responses in the days after various in-game events or accomplishments not covered by the other categories. Defaults to `true`.

- **DebugMode:** Uses more visible console logging and enables in-game alerts when new conversation topics are activated. Additionally labels all DCT response dialogues added by **this** mod (but not by other mods) with their conversation topic keys inside dialogue boxes (e.g. `DCT.Quest.FishStew`). This setting is pretty immersion-breaking, but useful for testing. Defaults to `false`.


## Notes
### ACKNOWLEDGEMENTS
* Much gratitude to ConcernedApe and [Pathoschild](https://www.nexusmods.com/stardewvalley/users/1552317?tab=user+files)!
* Thanks to those who provided help and support in the [Stardew Valley Discord](https://discordapp.com/invite/StardewValley) #making-mods channel.
* A particular thank you to {{people}} for helping beta-test this mod and give feedback on NPC dialogues!
* Many thanks to {{people}} for doing translations!

### SEE ALSO
* Help [translate](https://github.com/StardewModders/mod-translations/issues/31{{REPLACE_LINK}}) this mod to other languages
* Source code on [GitHub](https://github.com/Jonqora/StardewMods/tree/master/DynamicConversationTopics)
* Check out [my other mods](https://www.nexusmods.com/users/88107803?tab=user+files)!