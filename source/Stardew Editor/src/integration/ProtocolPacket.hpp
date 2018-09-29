#ifndef PROTOCOLPACKET_HPP
#define PROTOCOLPACKET_HPP

#include "integration/Packet.hpp"

class ProtocolPacket : public Packet
{
    public:
        static constexpr sf::Uint32 PROTOCOL_VERSION = 0;
        
        sf::Uint32 version = PROTOCOL_VERSION;
        
        virtual void write( sf::Packet& out );
        virtual void process( Editor& editor, GameIntegration& gi, sf::Packet& in );
};

#endif // PROTOCOLPACKET_HPP
