# Configuration documentation

The configuration consist of an associative array of key -> subarray in the JSON format.

## rule key

- A number : The nth of the prize in the list
- "infinite" : The rule that will be applied for each prize after the maximum configured, for example if my JSON contain 5 prizes + an "infinite" rule, when i pull the 6th, 7th, etc... prize, the rule applied will be the "infinite" one each time.

If there is no "infinite" rule or if there is no key in the array corresponding to the current prize we try to get, the native rule will be used, so you can patch only one prize if you want.

## prizeType

- simple : A simple unique item identifier
- list : A list of unique items identifiers, comma separated
- query : An item query, please see the documentation at : https://stardewvalleywiki.com/Modding:Item_queries

## itemRequest

- The current item request, depending of the prizeType value it must be an unique item identifier, a list of uniques items identifier or an item query

## quantity

- The quantity of item to be given as prize, if integer it give the quantity, if string you can define a range which the mod pick randomly between like : "1-5"

## perItemCondition

- The per item condition to apply to filter items returned by the item query when using it, else leave it empty
