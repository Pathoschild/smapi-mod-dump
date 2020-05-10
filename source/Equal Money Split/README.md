# Equal Money Split

###### Split money earned between each player's wallet automatically!


#### Goal

The goal of this mod is to help the logistics of each player managing their own money without having to worry about how their spending affects the group.

**Money is only split if either of the following occur:**
1. An item is sold through the shop
2. Items are sold through the shipping bin at the end of the day

**The following triggers will  _not_ cause money to be split:**
1. A quest was completed
2. Money was delivered through the mail
3. An event gave the player money

---

#### How does the mod work?

##### The following events trigger the money distribution:

- If the player's inventory has changed and their current amount of money is greater than the previous known amount of money, which is updated _every_ game tick.
- When the day ends, if the value of the items in the player's shipping bin is greater than zero.

##### The following process updates the other players' money:

1. Each player establishes their own listener that constantly checks for any new network messages that are coming from this mod.
2. When one player earns money and meets the criteria to split the money, they send a SMAPI mod message to every other online player.
3. When the other players receive this SMAPI mod message with the specific target address for this mod, they deserialize the payload and update their money.

After the amount of money is calculated to be  

The current player's money is updated locally after sending the messages to other players.

##### How the algorithm works: 
The calculation is basic math that simply distributes the amount earned equally, but I have described it below for the sake of transparency. You can find the calculation in `MoneySplitUtil.cs`

1. The amount of money earned is acquired by checking the amount of money the client had in the previous game tick compared to how much they have now:  
`MoneyEarned = CurrentMoney - PreviousMoney`

2. Then the amount of money each player should earn is simply calculated by splitting that value by the number of players online:  
`MoneyToShare = MoneyEarned / NumberOfOnlinePlayers`

3. At this point the client sends a message to every other online player (excluding self) who update their money by adding the new value onto it:  
`CurrentMoney = CurrentMoney + MoneyToShare`

4. However, the original client still has the full amount of money they earned by selling the items. Thus they need their money reduced such that they only "earned" the same amount that the other players received:  
`CurrentMoney = CurrentMoney - MoneyEarned + MoneyToShare`

---



<sup>_Truthfully, I made the mod because _someone_ I play with liked to sell our crops without telling anyone and reaped the benefits of everyone's hard work_</sup>
