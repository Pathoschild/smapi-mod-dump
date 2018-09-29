#ifndef PLAYEVENTPACKET_HPP
#define PLAYEVENTPACKET_HPP

#include "integration/Packet.hpp"

class PlayEventPacket : public Packet
{
    public:
        std::string location;
        std::string data;
        
        virtual void write( sf::Packet& out );
        virtual void process( Editor& editor, GameIntegration& gi, sf::Packet& in );
};

#endif // PLAYEVENTPACKET_HPP
