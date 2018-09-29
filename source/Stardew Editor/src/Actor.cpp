#include "Actor.hpp"

#include <boost/filesystem.hpp>

#include "Config.hpp"
#include "Editor.hpp"
#include "Xnb.hpp"

namespace
{
    int shirtSpot[] = { 0, 8, 24, 16 };
}

Actor::Actor( Editor& theEditor, const std::string& theName, sf::Vector2i thePos, int theFacing )
:   editor( theEditor ),
    name( theName ),
    tilePos( thePos ),
    facing( theFacing )
{
}

std::string Actor::getName() const
{
    return name;
}

void Actor::setPosition( sf::Vector2i thePos )
{
    tilePos = thePos;
    spr.setPosition( tilePos.x * TILE_SIZE, tilePos.y * TILE_SIZE );
    if ( farmerData )
    {
        farmerData->arms.setPosition( spr.getPosition() );
        farmerData->legs.setPosition( spr.getPosition() );
        farmerData->shirt.setPosition( spr.getPosition() );
        farmerData->shirt.move( 4, -1 );
        farmerData->hair.setPosition( spr.getPosition() );
    }
}

sf::Vector2i Actor::getPosition() const
{
    return tilePos;
}

void Actor::setFacing( int theFacing )
{
    facing = theFacing % 4;
    if ( facing < 0 )
        facing += 4;
    
    spr.setTextureRect( sf::IntRect( 0, facing * 32, 16, 32 ) );
    if ( farmerData )
    {
        if ( facing == 3 )
            spr.setTextureRect( sf::IntRect( 16, 1 * 32, -16, 32 ) );
        sf::IntRect rect = spr.getTextureRect();
        
        farmerData->arms.setTextureRect( sf::IntRect( rect.left + 6 * 16, rect.top, rect.width, rect.height ) );
        farmerData->legs.setTextureRect( sf::IntRect( rect.left + 18 * 16, rect.top, rect.width, rect.height ) );
        
        farmerData->shirt.setTextureRect( sf::IntRect( 0, shirtSpot[ facing ], 8, 8 ) );
        farmerData->hair.setTextureRect( sf::IntRect( 0, facing * 32, 16, 32 ) );
        if ( facing == 3 )
        {
            farmerData->hair.setTextureRect( sf::IntRect( 16, 1 * 32, -16, 32 ) );
        }
    }
}

int Actor::getFacing() const
{
    return facing;
}

void Actor::render( sf::RenderWindow& window )
{
    window.draw( spr );
    if ( farmerData )
    {
        window.draw( farmerData->arms );
        window.draw( farmerData->legs );
        window.draw( farmerData->shirt );
        window.draw( farmerData->hair );
    }
}

void Actor::init()
{
    fs::path path( editor.config.getContentFolder() );
    path /= "Characters";
    if ( name.length() >= 6 && name.substr( 0, 6 ) == "farmer" )
        path = path / "Farmer" / "farmer_base";
    else path /= name + "";
    
    loadTextureXnb( tex, path.string() );
    spr.setTexture( tex );
    spr.setPosition( tilePos.x * TILE_SIZE, tilePos.y * TILE_SIZE );
    spr.setTextureRect( sf::IntRect( 0, facing * 32, 16, 32 ) );
    spr.setOrigin( 0, 16 );
    
    if ( name.length() >= 6 && name.substr( 0, 6 ) == "farmer" )
    {
        if ( facing == 3 )
            spr.setTextureRect( sf::IntRect( 16, 1 * 32, -16, 32 ) );
        sf::IntRect rect = spr.getTextureRect();
        
        farmerData.reset( new FarmerData() );
        farmerData->arms = spr;
        farmerData->arms.setTextureRect( sf::IntRect( rect.left + 6 * 16, rect.top, rect.width, rect.height ) );
        farmerData->legs = spr;
        farmerData->legs.setTextureRect( sf::IntRect( rect.left + 18 * 16, rect.top, rect.width, rect.height ) );
        farmerData->legs.setColor( sf::Color( 46, 85, 183 ) );
        loadTextureXnb( farmerData->shirtTex, ( path.parent_path() / "shirts" ).string() );
        farmerData->shirt.setTexture( farmerData->shirtTex );
        farmerData->shirt.setPosition( spr.getPosition() );
        farmerData->shirt.move( 4, -1 );
        farmerData->shirt.setTextureRect( sf::IntRect( 0, shirtSpot[ facing ], 8, 8 ) );
        loadTextureXnb( farmerData->hairTex, ( path.parent_path() / "hairstyles" ).string() );
        farmerData->hair.setTexture( farmerData->hairTex );
        farmerData->hair.setPosition( spr.getPosition() );
        farmerData->hair.move( 0, 1 );
        farmerData->hair.setTextureRect( sf::IntRect( 0, facing * 32, 16, 32 ) );
        if ( facing == 3 )
        {
            farmerData->hair.setTextureRect( sf::IntRect( 16, 1 * 32, -16, 32 ) );
        }
        farmerData->hair.setOrigin( 0, 16 );
        farmerData->hair.setColor( sf::Color( 193, 90, 50 ) );
    }
}
