**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/DynamicDialogues**

----

# Custom questions


Questions are loaded from `mistyspring.dynamicdialogues/Questions/<NPC name>`. Once a NPC has nothing else to talk about, you can ask them questions (if any exist).
Like with dialogues, these need a key (it's only used in the case errors are found, so the name doesn't matter).

You can add multiple questions.

## Contents

* [Adding questions](#adding-questions)

  * [Example 1](#example-1)

  * [Example 2](#example-2)
-----------

## Adding questions


name |Required| description
-----|---|--------- 
Question |Yes| Text the question will have.
Answer |Yes| NPC's answer.
MaxTimesAsked\* |No| Max times you can ask this question. If 0, it'll count as infinite.
Location |No| The question will only appear when in this location.
From |No| The hour the question *can* begin being added at.
To |No| Limit time for adding the question.
EventToStart|No|Event to start after you ask the question.
QuestToStart|No|Quest to add after you ask the question.
CanRepeatEvent\*\*|No|If the event can be repeated.

\*= If you include a quest (or event), `MaxTimesAsked` must be 1.

\*\*= Once per day.

Template:

```
"nameForPatch": {
          "Question": ,
          "Answer": ,
          "MaxTimesAsked": ,
          "Location": ,
          "From": ,
          "To": 
        },
```

Just remove any fields you won't be using.
**Note:** If you don't want the question to appear every day, use CP's "When" field.

------------

### Example 1

This will add a question, which can only be asked twice (per day), and only when in Elliott's cabin.
```
{
      "Action": "EditData",
      "Target": "mistyspring.dynamicdialogues/Questions/Elliott",
      "Entries": {
        "rainyday": {
          "Question": "What do you think of rainy days?",
          "Answer": "My, they're quite gloomy....$2",
          "Location": "ElliottHouse",
          "MaxTimesAsked": 2
        }
      },
      "When":{
        "Weather":"Rain",
        "Hearts:Elliott": "{{Range: 3, 14}}"
      }
    },

```

------------

### Example 2

This will add "template" questions for sandy.
The first one can be asked forever, but the second one will only appear once.
```
{
      "Action":"EditData",
      "Target":"mistyspring.dynamicdialogues/Questions/Sandy",
      "Entries":{
        "questionTest1": {
          "Question": "Hey, this is a test question.",
          "Answer": "This is an answer."
        },
        "questionTest2": {
          "Question": "How about a limited question?",
          "Answer": "Never thought of it.",
          "MaxTimesAsked": 1
        }
      }
    }
```
