
*********************************************
************** Config settings **************
*********************************************

///////////////////////
// tilestoassumewarp //
///////////////////////

If you click within x tiles of a reachable warp point then the player will run to the warp. This is useful as some warps are off the map and so not reachable with clicking otherwise. Must be a whole number.

///////////////////////////
// overrideHorseDismount //
///////////////////////////

If this is set to true then using the primary or secondary activatin button will not dismount you from the horse. Must be set to true or false.

/////////////////////
/ endPointPrecision /
/////////////////////

endPointPrecision should be an whole number more than 0, it sets how many pixels you should be from each target point before stopping. If you set it too low then Click2Move will automatically increase it to stop any overstepping the target jitters. It is capped at 16 pixels as at lower accuracy than this the player will not follow the path accurately enough to get around corners.
////////////////////
// activationType //
////////////////////

activationType can have the following settings:
----------------------------------------------
"Single" this means you press the primary activation button once in order to activate click to move
----------------------------------------------
"Hold X" where X is a whole number. This means you hold the primary activation button for X game ticks to acticate click to move (there are around 60 game ticks a second).
----------------------------------------------
"Multiple" this means you have to have both the primary and secondary activation buttons down at the same time in order to activate click to move.
----------------------------------------------
"Double X" where X is a whole number. This means you have to double click the primary activation button for X game ticks to acticate click to move (there are around 60 game ticks a second).
----------------------------------------------
If X is not present then it will default to 30 ticks. If this setting is not done correctly then Click2Move will not run.

/////////////////////////
/primaryActivationButton/
/////////////////////////

This is the button that will activate Click2Move. 

///////////////////////////
/secondaryActivationButton/
///////////////////////////

This is the secondary button that will activate Click2Move - it's only relavent if you've selected multiple for activation type. 

/////////////////
/ allowAutoGate /
/////////////////

If true then click2Move will allow paths to go through gates - automatically opening them as they are approached and closing them after passing through. Gates that were open already will be left open. If a path stops on the same tile as a gate then the gate will be left open.

****************************************
************** Known bugs **************
****************************************
----------
It is possible to activate click to move in a way that causes the horse to be mounted immediately after click2move has calculated a route - this route may not be wide enough for the horse
----------

***********************************************
************** Upcoming features **************
***********************************************
----------
Visual representation of the path or end destination
