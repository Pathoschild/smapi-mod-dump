------
|NOTES
------
When assigning new keyboard key or controller button codes to activate the sprint buff open the 
SprintMod.json in notepad and find "SprintKey" or "SprintKeyForControllers" and replace values
after the colon. 

For example, if you want to use the E key as the key code should look like "E" or more specifically 
the entire line should look like:

"SprintKey": "E",

Another example, if you want to use the A button on your XBox controller as the sprint key then
the button code should be "A" or more specifically the entire line should look like:

"SprintKeyForControllers": "A"

---------
|WARNINGS
---------

I haven't tested ANY of the controller codes. I have no idea if they work. I'll work on it when 
I am able to.

I haven't tested what happens when a keyboard code or controller code is already assigned. My 
guess is that the mod won't work.

-----------------------------
|Accepted Keyboard Key Codes:
-----------------------------
There are a few special cases for keyboard keys. You can only specify if you want to use either
control keys or either shift keys. In addition, to use either control key use "17" and in order
to use either shift key use "16". Quotes are required for the mod to pick up the correct key.

The following key ranges or keys are accepted:

Alphanumeric (eg: "A" or "9")
LeftAlt
RightAlt
Space
PageUp
PageDown
End
Home
Insert
Delete
Left
Up
Right
Down
NumPad<#> (eg: "NumPad0" or "NumPad9")
Multiply
Add
Subtract
Decimal
Divide
17 (Lets you use either left or right control)
16 (Lets you use either left or right shift)

----------------------------------
|Accepted Controller Button Codes
----------------------------------
Quotes should surround the button code of your choice.

A
B
Back
BigButton (no idea what this means)
DPadDown
DPadLeft
DPadRight
DPadUp
LeftShoulder
LeftStick (As in click left stick down)
LeftThumbstickDown
LeftThumbstickLeft
LeftThumbstickRight
LeftThumbstickUp
LeftTrigger
RightShoulder
RightStick (As in click right stick down)
RightThumbstickDown
RightThumbstickLeft
RightThumbstickRight
RightThumbstickUp
RightTrigger
Start
X
Y