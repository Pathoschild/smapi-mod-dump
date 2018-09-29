#include "integration/Packet.hpp"

#include "integration/PlayEventPacket.hpp"
#include "integration/ProtocolPacket.hpp"

Packet::~Packet()
{
}

std::unique_ptr< Packet > Packet::createPacket( sf::Uint8 id )
{
    std::unique_ptr< Packet > ret;
    
    switch ( id )
    {
        case Protocol : ret.reset( new ProtocolPacket()  ); break;
        case PlayEvent: ret.reset( new PlayEventPacket() ); break;
    }
    
    return std::move( ret );
}
