#include "Xnb.hpp"

#include <SFML/Graphics/Texture.hpp>
#include <xnb/File.hpp>
#include <xnb/TextureType.hpp>

void loadTextureXnb( sf::Texture& tex, const std::string& path )
{
    xnb::File file;
    if ( !file.loadFromFile( path + ".xnb" ) )
    {
        util::log( "[ERROR] Failed to load XNB: $\n", path );
        return;
    }
    
    auto texData = dynamic_cast< const xnb::Texture2DData* >( file.data.get() );
    if ( texData == nullptr )
    {
        util::log( "[ERROR] XNB was not a texture: $\n", path );
        return;
    }
    
    tex.loadFromImage( texData->data[ 0 ] );
}
