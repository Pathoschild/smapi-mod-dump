--Set Up--

-Open your mod folder
-Open manifest.json
-Add to dependencies : 
    {
      "UniqueID": "mooseybrutale.spriteextender",
      "IsRequired": "false"
    }
-Launch SMAPI
-If you have GMCM installed, go to settings and add the number of sprites you are adding to the sprite sheet
    -If you are only using one gender, you only need to set one gender. Leaving it at 0 will be fine.
    -If you do not have GMCM, these changes can be made in your mod folder in the now added SSXConfig.json file but will require a game restart to take effect.
-Load or start new save

The mod will automatically add dummy values for the amount of sprites you specified (first time only).
These values all default to [0,0](Format: [x-offset, y-offset])
Your sprite frames should now be usable!

--Changing Sprite Offset--

If your farmer's hair, clothing and accessories do not line up when using your sprite, you need to change the offset settings.
You can do this by going into the SSXConfig.json file and editing the pair for the sprite or by editing it in the SMAPI console using the setFrameOffset command.

--Console Commands--

setFrameOffset - allows you to modify x and y offset for the indicated sprite
    Changes made will automatically be saved to your SSXConfig.json file and loaded anytime someone loads your mod.
    Format: setFrameOffset <frame index> <x-offset> <y-offset>
        <frame index> - The frame number of the sprite you want to edit. MUST BE GREATER THAN 125!
        <x-offset> The number of pixels to horizontally offset the farmer's hair and clothes (positive right, negative left)
        <y-offset> The number of pixels to vertically offset the farmer's hair and clothes (positive down, negative up)

getFrameOffset - allows you to see what the current offset settings are for the indicated sprite
    <frame index> - The frame number of the sprite you want to check. MUST BE GREATER THAN 125!