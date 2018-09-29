#include "Map.hpp"

#include <boost/filesystem.hpp>
#include <SFML/Graphics.hpp>
#include <xnb/File.hpp>
#include <xnb/TextureType.hpp>
#include <xnb/TbinType.hpp>

#include "Editor.hpp"

Map::Map( Editor& theEditor )
:   editor( theEditor )
{
    sf::Image gridImage;
    gridImage.create( TILE_SIZE, TILE_SIZE, sf::Color( 0, 0, 0, 0 ) );
    for ( int i = 0; i < TILE_SIZE * TILE_SIZE; ++i )
    {
        int ix = i % TILE_SIZE;
        int iy = i / TILE_SIZE;

        if ( ix == 0 || iy == 0 || ix == TILE_SIZE - 1 || iy == TILE_SIZE - 1 )
        {
            gridImage.setPixel( ix, iy, sf::Color( 0, 0, 0, 64 ) );
        }
    }
    grid.loadFromImage( gridImage );
    grid.setRepeated( true );
}

void Map::update()
{
    if ( firstUpdate )
    {
        view = sf::View( sf::FloatRect( 0, 0, editor.window.getSize().x, editor.window.getSize().y ) );

        firstUpdate = false;
    }

    if ( dragging && !sf::Mouse::isButtonPressed( sf::Mouse::Left ) )
    {
        dragging = false;
    }
}

void Map::update( const sf::Event& event )
{
    if ( event.type == sf::Event::EventType::MouseButtonPressed && editor.ui.isMouseOutside() )
    {
        if ( event.mouseButton.button == sf::Mouse::Left )
        {
            switch ( clickMode )
            {
                case Panning:
                    dragging = true;
                    dragFrom = pixelToWorld( sf::Vector2i( event.mouseButton.x, event.mouseButton.y ) );
                    break;
            }
        }
    }
    else if ( event.type == sf::Event::EventType::MouseWheelScrolled && editor.ui.isMouseOutside() )
    {
        sf::Vector2f worldPos = editor.window.mapPixelToCoords( sf::Vector2i( event.mouseWheelScroll.x, event.mouseWheelScroll.y ), view );

        if ( event.mouseWheelScroll.delta < 0 )
            view.zoom( 1.1 );
        else
            view.zoom( 0.9 );

        sf::Vector2f newWorldPos = editor.window.mapPixelToCoords( sf::Vector2i( event.mouseWheelScroll.x, event.mouseWheelScroll.y ), view );
        view.move( worldPos - newWorldPos );
    }
    else if ( event.type == sf::Event::EventType::Resized )
    {
        sf::Vector2f oldCenter = view.getCenter();
        view = sf::View( sf::FloatRect( 0, 0, editor.window.getSize().x, editor.window.getSize().y ) );
        view.setCenter( oldCenter );
    }
}

void Map::render( sf::RenderWindow& window )
{
    window.setView( view );

    if ( dragging )
    {
        sf::Vector2i mouse = sf::Vector2i( sf::Mouse::getPosition( window ) );
        sf::Vector2f dragTo = pixelToWorld( mouse );
        view.move( dragFrom - dragTo );
        dragFrom = pixelToWorld( mouse );
    }

    if ( current != "" )
    {
        window.draw( spr );

        sf::Vertex v[ 4 ];
        v[ 0 ] = sf::Vertex( sf::Vector2f( 0, 0 ), sf::Vector2f( 0, 0 ) );
        v[ 1 ] = sf::Vertex( sf::Vector2f( tex.getSize().x, 0 ), sf::Vector2f( tex.getSize().x, 0 ) );
        v[ 2 ] = sf::Vertex( sf::Vector2f( tex.getSize() ), sf::Vector2f( tex.getSize() ) );
        v[ 3 ] = sf::Vertex( sf::Vector2f( 0, tex.getSize().y ), sf::Vector2f( 0, tex.getSize().y ) );
        window.draw( &v[ 0 ], 4, sf::PrimitiveType::Quads, &grid );
    }
    for ( auto& entry : actors )
    {
        entry.second.render( window );
    }
}

void Map::changeCurrentMap( const std::string& map_ )
{
    xnb::File file;
    if ( !file.loadFromFile( ( fs::path( editor.config.getContentFolder() ) / "Maps" / ( map_ + ".xnb" ) ).string() ) )
    {
        util::log( "[ERROR] Couldn't load map XNB\n" );
        return;
    }
    auto data = dynamic_cast< xnb::TbinData* >( file.data.get() );
    tbin::Map map;
    std::istringstream strm = std::istringstream( data->data );
    if ( !map.loadFromStream( strm ) )
    {
        util::log( "[ERROR] Couldn't load map tbin\n" );
        return;
    }

    std::map< std::string, sf::Texture > texTs;
    for ( auto its = map.tilesheets.begin(); its != map.tilesheets.end(); ++its )
    {
        xnb::File xnbImage;
        xnbImage.loadFromFile( ( fs::path( editor.config.getContentFolder() ) / "Maps" / ( its->image + ".xnb" ) ).string() );
        xnb::Texture2DData* data = dynamic_cast< xnb::Texture2DData* >( xnbImage.data.get() );

        texTs[ its->image ].loadFromImage( data->data[ 0 ] );
    }

    sf::RenderTexture renTex;
    renTex.create( map.layers[ 0 ].layerSize.x * map.layers[ 0 ].tileSize.x, map.layers[ 0 ].layerSize.y * map.layers[ 0 ].tileSize.y );
    renTex.clear( sf::Color::Black );
    for ( auto itl = map.layers.begin(); itl != map.layers.end(); ++itl )
    {
        if ( !itl->visible || itl->id == "Paths" )
            continue;

        for ( std::size_t i = 0; i < itl->tiles.size(); ++i )
        {
            std::size_t ix = i % itl->layerSize.x;
            std::size_t iy = i / itl->layerSize.x;
            const tbin::Tile& tile = itl->tiles[ i ];
            if ( tile.isNullTile() )
                continue;

            tbin::TileSheet* found = nullptr;
            for ( auto its = map.tilesheets.begin(); its != map.tilesheets.end(); ++its )
            {
                if ( its->id == tile.tilesheet || ( tile.animatedData.frames.size() > 0 && its->id == tile.animatedData.frames[0].tilesheet ) )
                {
                    found = &( * its );
                    break;
                }
            }

            auto ti = tile.staticData.tileIndex;
            if ( tile.animatedData.frames.size() > 0 )
            {
                ti = tile.animatedData.frames[ 0 ].staticData.tileIndex;
            }

            int sx = found->sheetSize.x;// / found->tileSize.x;
            int sy = found->sheetSize.y;// / found->tileSize.y;

            sf::Sprite spr( texTs[ found->image ] );
            spr.setTextureRect( sf::IntRect( ti % sx * found->tileSize.x, ti / sx * found->tileSize.y, found->tileSize.x, found->tileSize.y ) );
            spr.setPosition( ix * itl->tileSize.x, iy * itl->tileSize.y );
            renTex.draw( spr );
        }
    }
    renTex.display();

    tex.loadFromImage( renTex.getTexture().copyToImage() );

    current = map_;
    spr.setTexture( tex, true );
    spr.setPosition( 0, 0 );
    actors.clear();
}

std::string Map::getCurrentMap() const
{
    return current;
}

sf::Vector2f Map::pixelToWorld( sf::Vector2i pixel ) const
{
    return editor.window.mapPixelToCoords( pixel, view );
}

void Map::clearActors()
{
    actors.clear();
}

Actor& Map::addActor( const std::string& name, sf::Vector2i pos, int facing )
{
    auto it = actors.insert( std::make_pair( name, Actor( editor, name, pos, facing ) ) );
    it.first->second.init();
    return it.first->second;
}

Actor* Map::getActor( const std::string& name )
{
    auto it = actors.find( name );
    if ( it == actors.end() )
        return nullptr;
    return &it->second;
}

const Actor* Map::getActor( const std::string& name ) const
{
    auto it = actors.find( name );
    if ( it == actors.end() )
        return nullptr;
    return &it->second;
}

void Map::removeActor( const std::string& name )
{
    auto it = actors.find( name );
    if ( it != actors.end() )
        actors.erase( it );
}

std::vector< std::string > Map::getActorList() const
{
    std::vector< std::string > ret;
    for ( const auto& actor : actors )
        ret.push_back( actor.first );
    return ret;
}
