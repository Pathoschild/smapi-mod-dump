#ifndef GLOBAL_HPP
#define GLOBAL_HPP

#include <util/Logger.hpp>
#include <SFML/Config.hpp>

namespace boost
{
    namespace filesystem
    {
        class path;
    }
}
namespace fs = boost::filesystem;

constexpr unsigned int TILE_SIZE = 16;

#ifdef SFML_SYSTEM_WINDOWS
std::string getRegistryValue( const std::string& key, const std::string& value );
#endif

#endif // GLOBAL_HPP
