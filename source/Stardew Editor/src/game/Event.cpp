#include "Event.hpp"

#include <algorithm>
#include <cstdlib>
#include <fstream>
#include <sstream>
#include <util/String.hpp>

namespace
{
    std::vector< Event::Precondition > parsePreconditions( const std::string& str )
    {
        std::vector< Event::Precondition > ret;
        
        auto tokens = util::tokenize( str, "/" );
        for ( const std::string& token : tokens )
        {
            Event::Precondition prec;
            prec.type = token[ 0 ];
            
            const auto& paramTypes = Event::PreconditionType::types[ prec.type ].paramTypes;
            if ( paramTypes.size() > 0 )
            {
                prec.params = util::tokenize( token.substr( 2 ), " " );
                if ( prec.params.size() != paramTypes.size() &&
                     std::find( paramTypes.begin(), paramTypes.end(), Event::ParamType::EnumMany ) == paramTypes.end() )
                {
                    //util::log( "[WARN] Precondition parameters for \"$\" doesn't match its type. ($ vs $)\n", token, prec.params.size(), Event::PreconditionType::types[ prec.type ].paramTypes.size() );
                }
            }
            
            ret.push_back(  prec );
        }
        
        return ret;
    }
    
    std::vector< Event::Actor > parseActors( const std::string& str )
    {
        std::vector< Event::Actor > ret;
        
        auto tokens = util::tokenize( str, " " );
        for ( std::size_t i = 0; i < tokens.size() / 4; ++i )
        {
            Event::Actor actor;
            actor.name = tokens[ i * 4 + 0 ];
            actor.name.resize( 32, '\0' );
            actor.pos = sf::Vector2i( util::fromString< int >( tokens[ i * 4 + 1 ] ), util::fromString< int >( tokens[ i * 4 + 2 ] ) );
            actor.facing = util::fromString< int >( tokens[ i * 4 + 3 ] );
            actor.oldName = actor.name;
            
            ret.push_back( actor );
        }
        
        return ret;
    }
    
    std::vector< Event::Command > parseCommands( const std::string& str, int skip )
    {
        std::vector< Event::Command > ret;
        
        int i = 0;
        auto tokens = util::tokenize( str, "/" );
        for ( const std::string& token : tokens )
        {
            if ( i++ < skip ) continue;
            
            Event::Command toAdd;
            
            std::size_t space = token.find( ' ' );
            toAdd.cmd = token;//token.substr( 0, space );
            /*
            if ( space != std::string::npos )
            {
                toAdd.params = util::tokenize( token.substr( space + 1 ), " " );
            }
            */
            
            ret.push_back( toAdd );
        }
        
        return ret;
    }
}

namespace Event
{
    std::map< char, PreconditionType > PreconditionType::types = std::map< char, PreconditionType >();
    std::map< char, PreconditionType > PreconditionType::loadTypes( const std::string& path )
    {
        std::map< char, PreconditionType > ret;
        
        std::ifstream file( path );
        if ( !file )
            return ret;
        
        while ( true )
        {
            std::string str;
            std::getline( file, str );
            if ( !file )
                break;
            
            std::size_t eq1 = str.find( '=' );
            std::size_t eq2 = eq1 != std::string::npos ? str.find( '=', eq1 + 1 ) : std::string::npos;
            if ( eq1 != 1 || eq2 == std::string::npos )
                continue;
            
            PreconditionType prec;
            prec.id = str[ 0 ];
            prec.label = str.substr( eq2 + 1 );
            std::string params = str.substr( eq1 + 1, eq2 - eq1 - 1 );
            
            int paramCount = params[ 0 ] - '0';
            for ( int param = 0, i = 1; param < paramCount; ++param )
            {
                ParamType type = static_cast< ParamType >( params[ i ] );
                
                std::size_t end = params.find( ';', i + 1 );
                if ( end == std::string::npos )
                    continue;
                std::string vals = params.substr( i + 1, end - i - 1 );
                if ( type == ParamType::EnumOne || type == ParamType::EnumMany )
                {
                    prec.enumValues = util::tokenize( vals, "," );
                    prec.paramLabels.push_back( "" );
                }
                else
                {
                    prec.paramLabels.push_back( vals );
                }
                i = end + 1;
                prec.paramTypes.push_back( type );
            }
            
            ret.insert( std::make_pair( prec.id, prec ) );
        }
        
        return ret;
    }
    
    Precondition Precondition::init( PreconditionType type )
    {
        Precondition prec;
        prec.type = type.id;
        
        prec.params.clear();
        prec.params.resize( type.paramTypes.size() );
        for ( std::size_t param = 0, currParamVal = 0; param < type.paramTypes.size(); ++param )
        {
            Event::ParamType paramType = type.paramTypes[ param ];
            switch ( paramType )
            {
                case Event::ParamType::Integer:
                case Event::ParamType::Double:
                    prec.params[ currParamVal++ ] = "0";
                    break;
                
                case Event::ParamType::Bool:
                    prec.params[ currParamVal++ ] = "false";
                    break;
                
                case Event::ParamType::String:
                case Event::ParamType::Unknown:
                    prec.params[ currParamVal++ ] = "";
                    break;
                
                case Event::ParamType::EnumOne:
                    prec.params[ currParamVal++ ] = type.enumValues[ 0 ];
                    break;
                
                case Event::ParamType::EnumMany:
                    prec.params.pop_back();
                    break;
                    
                case Event::ParamType::Position:
                    prec.params[ currParamVal++ ] = "0";
                    prec.params.insert( prec.params.begin() + currParamVal++, "0" );
                    break;
            }
        }
        
        return prec;
    }
    
    Data::Data()
    {
        branchName.resize( 32, '\0' );
        music.resize( 32, '\0' );
    }
    
    Data Data::fromGameFormat( const std::string& line )
    {
        std::size_t i = 0;
        for ( ; i < line.length(); ++i )
        {
            if ( !std::isspace( line[ i ] ) )
                break;
        }
        
        std::size_t colon = line.find( ':', i );
        if ( colon == std::string::npos )
            return Data();
        std::string key = line.substr( i, colon - i );
        
        std::string value;
        bool esc = false;
        for ( i = line.find( '"', colon + 1 ) + 1; i < line.length(); ++i )
        {
            char c = line[ i ];
            if ( c == '"' && !esc )
                break;
            else if ( c == '\\' && !esc )
                esc = true;
            else
            {
                value += c;
                esc = false;
            }
        }
        
        return std::move( fromGameFormat( key, value ) );
    }
    
    Data Data::fromGameFormat( const std::string& key, const std::string& value )
    {
        Data data;
        
        std::size_t slash = key.find( '/' );
        if ( slash == std::string::npos )
        {
            data.branchName = key;
            data.branchName.resize( 32,'\0' );
            data.commands = parseCommands( value, 0 );
        }
        else
        {
            data.id = util::fromString< int >( key.substr( 0, slash ) );
            data.preconditions = parsePreconditions( key.substr( slash + 1 ) );
            
            std::size_t s1 = value.find( '/' );
            std::size_t sp = value.find( ' ', s1 + 1 );
            std::size_t s2 = value.find( '/', s1 + 1 );
            std::size_t s3 = value.find( '/', s2 + 1 );
            data.music = value.substr( 0, s1 );
            data.music.resize( 32,'\0' );
            data.viewport = sf::Vector2i( util::fromString< int >( value.substr( s1 + 1, sp - s1 - 1 ) ), 
                                          util::fromString< int >( value.substr( sp + 1, s2 - sp - 1 ) ) );
            data.actors = parseActors( value.substr( s2 + 1, s3 - s2 - 1 ) );
            data.commands = parseCommands( value, 3 );
        }
        
        data.oldId = data.id;
        data.oldBranchName = data.branchName;
        
        return data;
    }
    
    std::string Data::toGameFormat() const
    {
        std::stringstream ss;
        if ( id != -1 )
        {
            ss << id;
            for ( const Precondition& prec : preconditions )
            {
                ss << '/' << prec.type;
                for ( const std::string& param : prec.params )
                    ss << ' ' << param.c_str();
            }
        }
        else
        {
            ss << branchName.c_str();
        }
        ss << ": \"";
        if ( id != -1 )
        {
            ss << music.c_str() << '/' << viewport.x << ' ' << viewport.y << '/';
            bool firstActor = true;
            for ( const Actor& actor : actors )
            {
                if ( !firstActor )
                    ss << ' ';
                firstActor = false;
                ss << actor.name.c_str() << ' ' << actor.pos.x << ' ' << actor.pos.y << ' ' << actor.facing;
            }
            ss << '/';
        }
        bool firstCmd = true;
        for ( const Command& cmd : commands )
        {
            if ( !firstCmd )
                ss << '/';
            firstCmd = false;
            
            std::string c = cmd.cmd.c_str();
            for ( std::size_t i = c.find( '"' ); i != std::string::npos; i = c.find( '"', i + 2 ) )
                c.replace( i, 1, "\\\"" );
            ss << c;
            
            bool firstParam = true;
            for ( const std::string& param : cmd.params )
            {
                if ( !firstParam )
                    ss << ' ';
                firstParam = false;
                
                std::string p = param.c_str();
                
                bool quoted = false;
                if ( p.find( '"' ) != std::string::npos )
                {
                    quoted = true;
                    for ( std::size_t i = p.find( '"' ); i != std::string::npos; i = p.find( '"', i + 2 ) )
                        p.replace( i, 1, "\\\"" );
                }
                
                if ( quoted ) ss << '"';
                ss << p;
                if ( quoted ) ss << '"';
            }
        }
        ss << "\"";
        return ss.str();
    }
    std::string Data::toGameFormatKey() const
    {
        std::stringstream ss;
        if ( id != -1 )
        {
            ss << id;
            for ( const Precondition& prec : preconditions )
            {
                ss << '/' << prec.type;
                for ( const std::string& param : prec.params )
                    ss << ' ' << param.c_str();
            }
        }
        else
        {
            ss << branchName.c_str();
        }
        return ss.str();
    }
    std::string Data::toGameFormatValue() const
    {
        std::stringstream ss;
        if ( id != -1 )
        {
            ss << music.c_str() << '/' << viewport.x << ' ' << viewport.y << '/';
            bool firstActor = true;
            for ( const Actor& actor : actors )
            {
                if ( !firstActor )
                    ss << ' ';
                firstActor = false;
                ss << actor.name.c_str() << ' ' << actor.pos.x << ' ' << actor.pos.y << ' ' << actor.facing;
            }
            ss << '/';
        }
        bool firstCmd = true;
        for ( const Command& cmd : commands )
        {
            if ( !firstCmd )
                ss << '/';
            firstCmd = false;
            
            std::string c = cmd.cmd.c_str();
            for ( std::size_t i = c.find( '"' ); i != std::string::npos; i = c.find( '"', i + 2 ) )
                c.replace( i, 1, "\"" );
            ss << c;
            
            bool firstParam = true;
            for ( const std::string& param : cmd.params )
            {
                if ( !firstParam )
                    ss << ' ';
                firstParam = false;
                
                std::string p = param.c_str();
                
                bool quoted = false;
                if ( p.find( '"' ) != std::string::npos )
                {
                    quoted = true;
                    for ( std::size_t i = p.find( '"' ); i != std::string::npos; i = p.find( '"', i + 2 ) )
                        p.replace( i, 1, "\"" );
                }
                
                if ( quoted ) ss << '"';
                ss << p;
                if ( quoted ) ss << '"';
            }
        }
        return ss.str();
    }
}
