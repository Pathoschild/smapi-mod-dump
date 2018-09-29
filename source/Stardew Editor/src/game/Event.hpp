#ifndef EVENT_HPP
#define EVENT_HPP

#include <map>
#include <SFML/System/Vector2.hpp>
#include <string>
#include <vector>

namespace Event
{
    static_assert( offsetof( sf::Vector2i, x ) + sizeof( int ) == offsetof( sf::Vector2i, y ), "Too lazy to make proper array, so actor.pos must be like one" );
    
    enum ParamType : char
    {
        Integer = 'i',
        Double = 'd',
        Bool = 'b',
        String = 's',
        EnumOne = 'e',
        EnumMany = 'E',
        
        Position = 'p',
        
        Unknown = '?',
    };
    
    struct PreconditionType
    {
        char id;
        std::vector< ParamType > paramTypes;
        std::vector< std::string > paramLabels;
        std::string label;
        
        /// We're assuming there is only one enum type per precondition, for now.
        std::vector< std::string > enumValues; 
        
        static std::map< char, PreconditionType > types;
        static std::map< char, PreconditionType > loadTypes( const std::string& path );
    };
    
    struct Precondition
    {
        char type;
        std::vector< std::string > params;
        
        static Precondition init( PreconditionType type );
    };
    
    struct Actor
    {
        std::string name;
        sf::Vector2i pos = sf::Vector2i( 0, 0 );
        int facing = 0;
        
        std::string oldName;
    };
    
    struct Command
    {
        std::string cmd;
        std::vector< std::string > params;
    };
    
    struct Data
    {
        Data();
        
        // Either id or branch name.
        int id = -1;
        std::string branchName;
        
        std::vector< Precondition > preconditions;
        
        std::string music;
        sf::Vector2i viewport;
        std::vector< Actor > actors;
        
        std::vector< Command > commands;
        
        static Data fromGameFormat( const std::string& line );
        static Data fromGameFormat( const std::string& key, const std::string& value );
        std::string toGameFormat() const;
        std::string toGameFormatKey() const;
        std::string toGameFormatValue() const;
        
        int oldId;
        std::string oldBranchName;
    };
}

#endif // EVENT_HPP
