ChildConsoleCommands isn't an official mod, it's just a couple of console commands bundled together which make it easier to create new save files to test your CP mod on. It's absolutely not safe to use in a normal save file.

-> addchild
This should bring up a child naming menu (with gender customization). You can put in the name and gender of the child, and a new child will be added to your household.
You'll also get an error in your console. The only reason this command works is because SMAPI automatically recovers from the error it causes. Therefore, this command is NOT SAFE to use on non-testing save files.

-> agechild <child name>
Once you've added the child to your household, use this command to age your child to 54 days immediately. When you go to sleep and wake up the next morning, your child will be 55 days old and therefore a toddler. Any mod you use that tries to access the Child's birthday, like Lookup Anything, will throw an error because your child has been aged illegally. Therefore, this command is NOT SAFE to use on non-testing save files.
The default behavior of Child To NPC is for children to be 83 days old when it converts them to NPCs, so if you want it to happen sooner, you'll need to change the setting in the config.json for Child To NPC. I personally recommend using 56 days old, so you can go to sleep one more time to have the child converted to an NPC. (But you can do it however you want, you shouldn't run into problems.)