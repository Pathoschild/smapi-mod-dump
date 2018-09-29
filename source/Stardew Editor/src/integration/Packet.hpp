#ifndef PACKET_HPP
#define PACKET_HPP

#include <istream>
#include <memory>
#include <ostream>
#include <SFML/Config.hpp>

namespace sf
{
    class Packet;
}

class Editor;
class GameIntegration;

class Packet
{
    public:
        virtual ~Packet();
        
        virtual void write( sf::Packet& out ) = 0;
        virtual void process( Editor& editor, GameIntegration& gi, sf::Packet& in ) = 0;
        
        enum Id : sf::Uint8
        {
            Protocol = 0,
            PlayEvent = 1,
        };
        
        static std::unique_ptr< Packet > createPacket( sf::Uint8 id );
};

#endif // PACKET_HPP
