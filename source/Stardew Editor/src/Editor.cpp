#include "Editor.hpp"

#include <boost/filesystem.hpp>
#include <imgui.h>
#include <imgui-sfml.h>
#include <SFML/Graphics/RenderWindow.hpp>
#include <SFML/Window/Event.hpp>

#include "Xnb.hpp"

namespace fs = boost::filesystem;

const char* Editor::CONFIG_FILE = "editor.cfg";

Editor::Editor( int argc, char* argv[] )
:   map( * this ),
    ui( * this ),
    gi( * this ),
    EXEC_PATH( fs::path( argv[ 0 ] ).parent_path().string() )
{
    auto configPath = fs::path( EXEC_PATH ) / CONFIG_FILE;
    if ( fs::exists( configPath ) )
        config.loadFromFile( configPath.string() );
    else config.saveToFile( CONFIG_FILE );

    loadTextureXnb( dialogueFont,  ( fs::path( config.getContentFolder() ) / "LooseSprites" / "font_bold" ).string() );

    refreshMapList();
    reloadSoundList();
}

Editor::~Editor()
{
    config.saveToFile( CONFIG_FILE );
}

void Editor::run()
{
    window.create( sf::VideoMode( 800, 600 ), "Stardew Editor" );
    window.setFramerateLimit( 60 );
    ImGui::SFML::Init( window );

    bool isRunning = true;
    while ( isRunning )
    {
        sf::Event event;
        while ( window.pollEvent( event ) )
        {
            if ( event.type == sf::Event::Closed )
            {
                isRunning = false;
            }

            ui.update( event );
            map.update( event );
        }

        ui.update();
        map.update();
        gi.update();

        window.clear( sf::Color::White );
        window.resetGLStates();
        map.render( window );
        ui.render( window );
        window.display();
    }

    ImGui::SFML::Shutdown();
}

void Editor::refreshMapList()
{
    std::ifstream file( ( fs::path( config.getDataFolder() ) / "locations.txt" ).string() );
    while ( true )
    {
        std::string line;
        std::getline( file, line );
        if ( !file )
            break;

        maps.insert( line );
    }

    ui.sendRefresh( Refresh::MapList );
}

void Editor::reloadSoundList()
{
    std::ifstream file( ( fs::path( config.getDataFolder() ) / "soundCues.txt" ).string() );
    if ( !file )
        return;

    sounds.clear();
    while ( true )
    {
        std::string line;
        std::getline( file, line );
        if ( !file )
            break;

        std::size_t eq = line.find( '=' );
        if ( eq == std::string::npos ) continue;

        std::string cue = line.substr( 0, eq );
        std::vector< std::string > inds = util::tokenize( line.substr( eq + 1 ), "," );

        sounds.insert( std::make_pair( cue, inds ) );
    }

    ui.sendRefresh( Refresh::Sounds );
}

std::vector< std::string > Editor::getSoundCueList() const
{
    std::vector< std::string > ret;
    for ( const auto& entry : sounds )
    {
        ret.push_back( entry.first );
    }
    return ret;
}

std::string Editor::getPathForSound( const std::string& cue ) const
{
    std::string snd = sounds.at( cue )[ rand() % sounds.at( cue ).size() ];
    return ( fs::path( config.getExtractedSounds() ) / ( snd + ".wav" ) ).string();
}
