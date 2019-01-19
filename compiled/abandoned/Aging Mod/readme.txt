About
   This mod adds growing beard\hair and gray hair after year 50.
   You can still change hair color at wizard the hair will change back to gray every year however(or should).
   Beard growth changes every 2 seasons by default(summer and winter) but can be configured.
   The beard and hair growth stages can also be configured(inputing a new beard-Id value in the array).
   
   NOTE! you must have a hair\beard style that exist in the array if you want hair\beard growth!
    something completely different(or start a character with a hair not in the array) configure the config.json file!


Very rough outline on the ID's for beard\hair,  beard starts at -1(which is the non-accessory default \ 1), 
hair starts at 0(again, default 1 in the character screen). 
So if you want say hairstyle 15, you'd put 14.  if you want beard style 5, you'd put 3.


Configure the mod using the config.json file.

getOld
   > If true, Changes hair color to gray at year 40(and every year update after that), and light gray at 50. Finally white at 70.
       Default is true

FemaleGrowBeard
   > Self explanatory, allows female to grow beard too.
       Default is false

beardStages
   > A list of the various beard stages

keyPressShave
   > Allows shaving of the beard(revert to first style in beardStages) by pressing a button.
       default is false

shaveCost
   > The price of shaving by pressing the shavebutton(0 for free).
       default is 100

shaveKey
   > The key you press to shave
       default is .  | a.k.a period

chargeText
   > Text displayed after you've shaved and the game substract the money from you.

HairStagesMale
   > Same as beard stages, except with hair and for men.

HairStagesFemale
   > Look above

beardGrowSeasons
   > What seasons will push beard to next stage(between spring, summer, fall and winter), nothing will stop beard growth.
      default is summer and winter.

hairGrowSeasons
   > Look above

Installation instructions:
    Download then install SMAPI 
         > Src - https://github.com/Pathoschild/SMAPI/releases
    Unzip\Create folder named AgingMod in Stadrew Vallley\Mods   folder. Structure map:
    Stardew valley\Mods\AgingMod\|> AgingMod.dll
                                  |> Agingmod.pdb
                                  |> manifest.json