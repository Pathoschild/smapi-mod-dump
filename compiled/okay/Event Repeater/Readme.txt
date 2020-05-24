*************************************************************************************************************************************************
*																																				*
*													MissCoriel's Event Repeater																	*
*																																				*
*													  Explainations and Tips																	*
*																																				*
*************************************************************************************************************************************************


[Table of Contents:]

A. Introduction
B. Explaination of Mod use
B1. Repeating Events
B2. Repeating Mail
B3. Repeating Responses
C. Console Commands
D. Tips for Modders!
E. Special Thanks/Credits
-=+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++=-
[A. Introduction:]

Grettings Farmers, Fishers, and Miners!  I am MissCoriel and I am very happy that you have chosen to use Event Repeater!  This mod was inspired
with my own desire to make Stardew Valley have certain events and even mail repeated so as to increase your immersion in the story.  A little about
myself, I have always messed around in games and tried to see what I can change.  I used to make weapons for Starsiege back in the early 2000's and
even made my own cars for Need for Speed III for the PC.  I started to mess with xnb files to Stardew and spent a year trying to learn about how
Dialogue, Events, even NPCs worked.  When I joined the official Discord and met the modding community, I was happy to find encouragement and even
more I am proud to help people learn and grow as they make mods of their dreams.  That's what Modding is about, Dreams.  At first, I was told it 
would be quite a chore to make events repeatable.  One day out of random, I decided to get to it.  With a bit of help with experienced SMAPI modders,
Event Repeater has become Reality!  I hope that this mod will allow you to follow your dreams and create the mod we all deserve. <3

-=+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++=-
[B. Explaination of Mod Use:]

Event Repeater is simple and straight to the point.  It merely stores the [EventID]s and [MailID]s that the game has seen.  It will check Content Patcher (CP) Mods for
the appropriate fields and if you seen an [EventID] or [MailID], Event Repeater shall remove them from the seen list when you go to bed.  

In order for Event Repeater to work, you must add this to your CP mod's manifest.json:

"Dependencies": [
   {
      "UniqueID": "misscoriel.eventrepeater",
      "IsRequired": true,
   },
],


[B1. Repeating Events:]

As of version 4 of Event Repeater, It has been intergrated in Content Patcher.  There is no extra mod and manifest.json you need anymore.  
Simply add the following to your CP Mod's content.json:

 "RepeatEvents": [
	[EventID],
 
	],

[EventID] is the ID of the Event you wish to Repeat.  If you downloaded my Test Mod the [EventID] is 69696969.
This is what that would look like:

	"RepeatEvents": [
	69696969,

	],
A comma is placed after each [EventID] with the exception of the last (though it won't complain if you do). 

When the next day starts, you will see the following in the [SMAPI] console: New Day, Forget events!

This is a generalizeation of the action in the background.  If you need, you can check the [Trace Log] in the Error Log to get a more specific explaination:

	[21:48:00 TRACE Event Repeater] Forgetting event id: 10000
	[21:48:00 TRACE Event Repeater] Forgetting event id: 69696969

This means that Event Repeater found the numbers 10000 and 69696969 in the "RepeatEvents": [] field.  They can come from any CP mod.  Those events were
removed when the new day starts.

[B2. Repeating Mail]:

The ability of Mail being repeatable was added in Event Repeater version 2.  The process is similar to Repeating Events:

	"RepeatMail": [
	[MailID],

	],

[MailID] is the ID of the mail message you wish Event Repeater to forget.  Let's use the [MailID] "Dobson" as our example:

	"RepeatMail": [
	"Dobson",
	
	],

This will tell Event Repeater to remove "Dobson" from the seen mail list when the next day starts.  Be aware that it will take more to repeat mail messages.

Let's say "Dobson" is mailed on a Tuesday when you're two hearts with Haley.  The mail event would be as follows:

	"454545/f Haley 500/d Mon Wed Thu Fri Sat Sun/x Dobson": "null",

When the mail is in the mailbox, you will have seen the [EventID] 454545.  This means in order to repeat "Dobson", you  must also repeat 454545:

	"RepeatEvents": [
	454545,
	
	],

	"RepeatMail": [
	"Dobson",
	
	],
This will tell Event Repeater to remove both the [EventID] and [MailID] from your seen list.  This will mean the next [Tue] the event will play again; so long as 
you still qualify for the event conditions.

With the use of Repeating Mail, you can have loved ones send you gifts and beyond (Read Tips for Modders for more).

[B3. Repeating Response IDs:]

Version 5 of Event Repeater gives you the power to remove responses to questions you have been asked in either dialogue or events.  Like the others, it's straight foward:

	"RepeatResponse": [
	[ResponseID],
	]

The [ResponseID] is a numerical value added to a question ($q) and response ($r) string.  Here is an example of what It would look like:

	speak Penny \"I don't know if we talked yesterday...#$b#$q 690001 null#Did we test the new response yet?#$r 690001 0 repeat_yes#Yes.. it works#$r 690002 0 repeat_no#Not yet.. let's try tomorrow!\"/

This is how it would look within an event.  See the Stardew Valley Wiki Modding: Event_data page for more information.
Take note the the question string referrs to the number 690001 and 690002 within it.  These are response IDs in which it will be stored and used when called for.
The $q and $r can be used in day to day dialogue:

	"Mon": "Did you know that I have a sword at my house?#$b#$q 1000/1001 swordplay#Would you like to see it?#$r 1000 10 swordYes#Sure!#$r 1001 -10 swordNO#Heck no!",
	"swordYes": "Great!! Maybe we can meet up later when Dad is not watching.$1",
	"swordNo": "Well I didn't want to show it to you anyways.$5",
	"swordplay": "$p 1000#It's real shiny.$1|Not that you care.$5",

This indicates that Response ID 1000 is a positive response, while 1001 is a Negative. When the question is asked it will store the [ResponseID] and divert to "swordplay".. thus:

	<If 1000:>
		"Did you know that I have a sword at my house?"After pressing the button to move on: "It's real Shiny."

	<If 1001:>
		"Did  you know that I have a sword at my house?"After Pressing the button to move on: "Not that you care."

Being able to forget either case is knowing the [ResponseID]:

	"RepeatResponse": [
	690001,
	690002,
	1000,
	1001,
	]

Every morning, Event Repeater will remove the listed [ResponseIDs].  This means the game will ask you the same question again for a response.
	
-=+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++=-
[C. Console Commands:]

Event Repeater is a powerful tool for Modding both for a player and a tester!  Using these console commands goes beyond the content.json and 
allows you to test events more than once during a single day:

  {showevents}

This Console Command simply shows you all the events you've seen:

	"Events seen: 60367, [EventID], [EventID],"

Above would be how the output would look, where [EventID] would be any events seen.  [EventID] 60367 is the initial event when you step off the bus.  It is not repeatable
as it uses coding that will possibly crash or corrupt your save.  DO NOT ATTEMPT TO REPEAT IT!

	{eventforget}

This Console Command is used to manually remove [EventID]s for testing and evaluation purposes.  Let's say you've seen event 34 (This is Penny's 2 heart event) and would like
to try a different branch of a question fork.  Simply use the command: eventforget 34
This will cause the Console to output the following:

	"Forgeting event id: 34"

This means that if you meet the requirements to the event, you may simply walk out and back into the map and the event will start again.
This can also work for repeating Mailing events if you know the [EventID] that mailed it.

	{showmail}

This will work just like showevents, except this is for [MailID]s.  You will note that some special [MailID]s will appear such as
"Haley_Introduction"  These can be repeated as well, however I have not personally looked into the full application of this.
Like showevents, showmail will show all [MailID]s in the order they were seen.

	{mailforget}

This works just like eventforget, except this works for [MailID]s.  Remember, if you wish to repeat mail manually you need both the [MailID] and the [EventID] that sent the mail.

	{sendme}

This was implemented in Event Repeater secret 3.0 release.  This command tells Event Repeater to send the Mail with the indicated [MailID]: sendme Dobson

The SMAPI console will then, if the ID is correct, will return this:

	"Check Mail Tomorrow!  Sending: Dobson"

Tomorrow morning, the mail with the [MailID] "Dobson" will appear in your mailbox.  This is good to test mail formatting and item receiving.

	{showresoponse}

This is for Advanced Modders who know how to find specific [ResponseIDs].  It will show only the ID numbers and no other context. If an Event is repeated, you may see the same ID more than once.

	{responseforget}

This is a Manual forget command to remove a [ResponseID] from the list.  

	{responseadd}

This is for Debugging!  Adds a response ID.  It will not know if the ID is a valid one.. so be careful!


-=+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++=-
[D. Tips for Modders:]

I.  Use Event Conditions:

	Events can regulate themselves on how many times they can repeat.  If you wish to have a weekly repeatable event, simply use the 'd' condition:
	/d Mon Wed Fri Sat Sun/- This means the event will only play on [Tue] and [Thu].  You can use the 'u' command to specify a specific day of the season.
	so /u 20/ would indicate that the 20th day in each season the event would play.  You DO NOT need to put 'd' and 'u' on the same event.
	
II. Event Variance; Random chances:

	Using the event condition 'r' allows you to set a probability on if the event happens or not.  An event with /r 0.3/ has only a 30% chance to play.
	This is a good way to make rare events.  
	More importantly, you can use Event Repeater to make variable events.
	Simply make 2 events with the same [EventID] and both with a 'r' condition:

	"50000000/d Tue Wed Thu Fri Sat Sun/z winter/r 0.5": <instance 1>
	"50000000/d Tue Wed Thu Fri Sat Sun/z winter/r 0.3": <Instance 2>

	This will mean that when you enter the map with the [EventID] 50000000, it will roll for each event, the successful roll will be the event seen.  This means the losing roll will NOT 
	be seen on this day.  When the conditions are met again, they will roll against each other again.

III. Stop Repeatables; Useing Tokens

	Content Patcher has plenty of powerful patching abilities.  You can use them to stop and even start Repeating events at your desire.
	When you create an event, simply use a "When" condition through Content Patcher to determine if an event is present or not:
	{
		"LogName": "Dating Haley",
		"Action": "EditData",
		"Target": "Data/Events/Beach",
		"When": {
			"Relationship:Haley": "Dating",
			},
		"Entries": {
			"983980348/f Haley 2000/t 1800 2100": [Event goes here]
		},

	This means so long as you're dating Haley, This event will be able to happen everyday at 6pm- 9pm.  When you get engaged, or no longer dating Haley, This event will become invalid
	and no longer play as it's not there to play.
	
This section may grow over time.  For now, these are the basics of which I have used with Event Repeater.  If any new techniques are found, let me know and I'll add them in.

-=+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++=-
[E. Special Thanks:]

The Event Repeater would of not of been realized without the help of the people in this list:

	-Elizabeth Ann Coriel ~ My darling wife who supported her crazy wife (me) though all the frustration.
	-Pathoschild ~ For not only creating SMAPI, but helping me in coding as he is senpai (notice me!).
	-Bwdy ~ For helping me pull the concept together and making it possible for me to figure out.
	-Minerva ~ The first (and currently only) adopter of Event Repeater
	-The Modding Community Discord ~ too many names to mention for all the help and support they give.  Truly the best community I have ever known.

