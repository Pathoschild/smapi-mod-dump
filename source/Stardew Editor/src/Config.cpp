#include "Config.hpp"

#include <boost/filesystem.hpp>
#include <cstdlib>
#include <fstream>
#include <util/String.hpp>

namespace
{
    const char* OPT_CONTENT = "content";
    const char* OPT_EXPORT = "export";
    const char* OPT_DATA = "data";
    const char* OPT_EXTRACTED_SOUNDS = "extracted_sounds";
}

Config::Config()
{
    reset();
}

bool Config::loadFromFile( const std::string& path )
{
    std::ifstream file( path );
    if ( !file )
        return false;

    Config old = ( * this );
    reset();

    while ( true )
    {
        std::string line;
        std::getline( file, line );
        if ( !file )
            break;

        std::size_t eq = line.find( '=' );
        if ( eq == std::string::npos)
            continue;

        std::string opt = line.substr( 0, eq );
        std::string val = line.substr( eq + 1 );

        if ( opt == OPT_CONTENT )
            content = val;
        else if ( opt == OPT_EXPORT )
            exportFolder = val;
        else if ( opt == OPT_DATA )
            dataFolder = val;
        else if ( opt == OPT_EXTRACTED_SOUNDS )
            extractedSounds = val;
    }

    content.resize( 512, '\0' );
    exportFolder.resize( 512, '\0' );
    dataFolder.resize( 512, '\0' );
    extractedSounds.resize( 512, '\0' );

    return true;
}

bool Config::saveToFile( const std::string& path ) const
{
    std::ofstream file( path, std::ofstream::trunc );
    if ( !file )
        return false;

    file << util::format( "$=$\n", OPT_CONTENT, content.c_str() );
    file << util::format( "$=$\n", OPT_EXPORT, exportFolder.c_str() );
    file << util::format( "$=$\n", OPT_DATA, dataFolder.c_str() );
    file << util::format( "$=$\n", OPT_EXTRACTED_SOUNDS, extractedSounds.c_str() );

    return true;
}

void Config::setContentFolder( const std::string& path )
{
    content = path;
    content.resize( 512, '\0' );
}

std::string Config::getContentFolder() const
{
    return content.c_str();
}

void Config::setExportFolder( const std::string& path )
{
    exportFolder = path;
    exportFolder.resize( 512, '\0' );
}

std::string Config::getExportFolder() const
{
    return exportFolder.c_str();
}

void Config::setDataFolder( const std::string& path )
{
    dataFolder = path;
    dataFolder.resize( 512, '\0' );
}

std::string Config::getDataFolder() const
{
    return dataFolder.c_str();
}

void Config::setExtractedSounds( const std::string& theExtractedSounds )
{
    extractedSounds = theExtractedSounds;
    extractedSounds.resize( 512, '\0' );
}

std::string Config::getExtractedSounds() const
{
    return extractedSounds.c_str();
}

void Config::reset()
{
    content = "./Content";
    #if defined( SFML_SYSTEM_WINDOWS )
    if ( fs::exists( "C:\\Program Files (x86)\\GalaxyClient\\Games\\Stardew Valley\\Content" ) )
        content = "C:\\Program Files (x86)\\GalaxyClient\\Games\\Stardew Valley\\Content";
    else if ( fs::exists( "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Stardew Valley\\Content" ) )
        content = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Stardew Valley\\Content";
    else
    {
        std::string steam = getRegistryValue( "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 413150", "InstallLocation" );
        std::string gog = getRegistryValue( "SOFTWARE\\WOW6432Node\\GOG.com\\Games\\1453375253", "PATH" );
        if ( steam != "" && fs::exists( steam ) )
            content = steam + "\\Content";
        else if ( gog != "" && fs::exists( gog ) )
            content = gog + "\\Content";
    }
    #elif defined( SFML_SYSTEM_MACOS )
        std::string home = std::getenv( "HOME" );
        if ( fs::exists( "/Applications/Stardew Valley.app/Contents/MacOS/Content" ) )
            content = "/Applications/Stardew Valley.app/Contents/MacOS/Content";
        else if ( fs::exists( home + "/Library/Application Support/Steam/steamapps/common/Stardew Valley/Contents/MacOS/Content" ) )
            content = home + "/Library/Application Support/Steam/steamapps/common/Stardew Valley/Contents/MacOS/Content";
    #else
        if ( fs::exists( home + "/GOG Games/Stardew Valley/game/Content" ) )
            content = home + "/GOG Games/Stardew Valley/game/Content";
        else if ( fs::exists( home + "/.steam/steam/steamapps/common/Stardew Valley/Content" ) )
            content = home + "/.steam/steam/steamapps/common/Stardew Valley/Content";
        else if ( fs::exists( home + "/.local/share/Steam/steamapps/common/Stardew Valley/Content" ) )
            content = home + "/.local/share/Steam/steamapps/common/Stardew Valley/Content";
    #endif
    exportFolder = "./export";
    dataFolder = "./data";
    extractedSounds = "./sounds";

    content.resize( 512, '\0' );
    dataFolder.resize( 512, '\0' );
    extractedSounds.resize( 512, '\0' );
}
