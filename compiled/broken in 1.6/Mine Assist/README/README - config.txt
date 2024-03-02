This mod intend to add quick key bindings to assist mining. But as a general key binding mod, you can also use it in other situations.

Deleting config.json will make the mod regenerate the missing files with default values. This is useful when updating the mod.
Feel free to modify config.json all you'd like. The format is as follows:

 - isEnable: true or false. Determine whether the mod should actually do anything but load the configs. You can set this to false rather than delete the mod if you're trying to find mod conflicts.
 
 - overrideTrigger: true or false. SMAPI will raise button click event only when trigger is pushed to a certain depth but the game will respond to trigger when it is pushed even a little bit. Enable this will make sure the game will not process trigger events, only the bindings here will respond to the trigger key.
 
 - triggerClickedThreshold: A digit between 0~1. Dtermine how depth shoud you push trigger keys to execute binding command. This take effect only when overrideTrigger is true.
 
 - modes: Key bindings are assiciated with active mode so that you can switch between key settings under various situations, when the game start, it will try to activate "default" mode.
    - <mode name>: Each mode has its name. It will be used in parameter of "SwitchMod" command. "default" mode will be loaded when the game start.
        - modifyKeys: An array of modifyKeys. modifyKey can conbime with other keys. modifyKey alone can also mapped to a command and will be executed when the key is released and no combination binding have been triggered.
        - cmds: Key to Commands mapping list.
            - modifyKey: The modify key used in single key biding. Use "None" if you do not intend to use modify keys. Possbile keys are listed in "README - keys and commands.txt".
            - key: The action key used in single key biding. Hold modifyKey while press this key will invoke the related command. Possbile keys are listed in "README - keys and commands.txt".
            - cmd: Which command to invoke. Can be one of "UseItem", "Craft", or "SwitchMod".
            - par: Command parameter. Command and paramter list are in "README - keys and commands.txt".
