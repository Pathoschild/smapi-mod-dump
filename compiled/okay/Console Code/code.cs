foreach ( var item in Game1.player.items )
{
    if ( item == null )
        continue;
    
	Game1.player.money += item.salePrice();
	Game1.player.removeItemFromInventory( item );
}