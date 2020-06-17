~Nice Messages Version 1.2.1~

This mod is meant to add flavour text to your morning routines in Stardew Valley!

###FEATURES###

---Included messages---
Messages are based on the season and current weather of the day! An array of various messages are
included in the mod for you to enjoy! You may also decide the % chance that these messages may appear.
(By default, set to 100% chance of a message each morning.)

---Customization---

/\__CONFIG OPTIONS__/\

HOW TO CHANGE PERCENT (%) CHANCE A MESSAGE WILL APPEAR IN THE MORNING
 1. Open the Nice Messages mod folder
 2. Open 'config.json' in a plain text editor.
    a. Notepad works just fine, Notepad++ if you have it.
 3. Change the number to the percent chance you want to see a message happen, between 1 and 100

 HOW TO CHANGE THE AMOUNT OF TIME A MESSAGE WILL BE DISPLAYED ON SCREEN
 1. Open the Nice Messages mod folder
 2. Open 'config.json' in a plain text editor.
 3. Change the msgFadeOutTimer option:
    - This number is done in milliseconds 1 second = 1000 milliseconds. Take the number of seconds 
      you want to delay the message and multiply it by 1000
    - EXAMPLE: 5.5 seconds = 5500

/\__MESSAGE OPTIONS__/\

HOW TO ADD/REMOVE/CUSTOMIZE MESSAGES
 1. Open the Nice Messages mod folder
 2. Open unifiedMessages.json.
 3.
    In this file you will see all the included messages in this format: 
                                "key":
                                [
                                "value"
                                ]

    ..:::EXAMPLE:::..
    
    }
        "spring/sunny":
	    [
        "msg1",
        "msg2"
	    ],

	    "spring/windy":
	    [
        "msg3",
        "msg4",
        "msg5"
	    ],

	    "spring/rain":
	    [
        "msg6",
        "msg7"
	    ]
    
    }

    Keys: The program looks for these particular keys. Changing them will render those messages unreachable. Please do not change them.
          If a key is not working, please refer to the below chart to ensure that the keys are correct:
        
        * VALD KEYS ********************************************************************
        * spring/sunny         summer/sunny        fall/sunny          winter/sunny    *
        * spring/windy                             fall/windy                          *
        * spring/rain          summer/rain         fall/rain                           *
        * spring/lightning     summer/lightning    fall/lightning                      *
        *                                                              winter/snow     *
        * ******************************************************************************
   
   Values: These are the messages the program reads. Messages may be added, changed, or removed so long as they
           follow these rules:

        1. All messages must be in "quotation marks"
        2. ALL messages must be between the [square brackets] for their respective season
        3. Unless the message is the final one in a season, all messages must be followed by a comma (,).
            --This is telling the program that these are sperate messages in a list. The last message without a comma
              tells the program that the list has ended.


~~~~~~~~Afterword~~~~~~~~~~~~
Thank you for downloading and trying my mod! It really means a lot to me that people are actually enjoying something I've made!
I hope this mod has made your game a little more fun!

Happy Farming,
--Nori
