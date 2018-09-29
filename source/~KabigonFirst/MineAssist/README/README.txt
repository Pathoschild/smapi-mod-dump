This mod intend to add quick key bindings to assist mining. But as a general key binding mod, you can also use it in other situations.

Deleting config.json will make the mod regenerate the missing files with default values. This is useful when updating the mod.
Feel free to modify config.json all you'd like. The format is as follows:

 - isEnable: Whether the mod should actually do anything but load the configs. You can set this to false rather than delete the mod if you're trying to find mod conflicts.
 
 - modes: Key bindings are assiciated with active mode so that you can switch between key settings under various situation, when the game start, it will try to activate "default" mode.
    - <mode name>: Each mode has its name. It will be used in parameter of "SwitchMod" command.
        - modifyKeys: An array of modifyKeys. modifyKey won't invoke and action but can only conbime with other keys.
        - cmds: Key to Commands mapping list.
            - modifyKey: The modify key used in single key biding. Use "None" if not intend to use modify keys. Possbile keys are listed in "README - keys and commands.txt".
            - key: The action key used in single key biding. Hold modifyKey while press this key will invoke the related command. Possbile keys are listed in "README - keys and commands.txt".
            - cmd: Which command to invoke. Can be one of "UseItem", "Craft", or "SwitchMod".
            - par: Command parameter. Command and paramter list are in "README - keys and commands.txt".
