# Stardew-Mods

## Custom Quest Expiration
Change the Daily Quest expiration (make it never expire, expire after 5 days, etc)

## TillableGround
Make any tile tillable (so you can use your hoe on it)

## Better Junimos
Allow your junimos (from junimo huts) to automatically plant seeds, fertilize, and much more!

### *BetterJunimosApi*

#### Custom Abilities
```
RegisterJunimoAbility(IJunimoAbility junimoAbility) // add a custom Junimo ability
```

```
/*
 * Provides abilities for Junimos 
 */    
public interface IJunimoAbility {
    /*
     * What is the name of this ability 
     */
    String AbilityName();

    /*
     * Is the action available at the position? E.g. is the crop ready to harvest
     */
    bool IsActionAvailable(Farm farm, Vector2 pos);

    /*
     * Action to take if it is available, return false if action failed
     */
    bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Chest chest);

    /*
     * Does this action require an item (SObject.SeedsCategory, etc)?
     * Return 0 if no item needed        
     */
    int RequiredItem();
}
```

#### Other Functions
```
GetJunimoHutMaxRadius() // int: max radius

GetJunimoHutMaxJunimos() // int: max junimos

GetJunimoAbilities() // Dictionary<string, bool>: enabled abilities

GetWereJunimosPaidToday() // bool: did the user pay the junimos today
```
