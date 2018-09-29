#include "integration/ProtocolPacket.hpp"

#include <SFML/Network/Packet.hpp>

#include "integration/GameIntegration.hpp"

void ProtocolPacket::write( sf::Packet& out )
{
    out << version;
}

void ProtocolPacket::process( Editor& editor, GameIntegration& gi, sf::Packet& in )
{
    in >> version;
    if ( !in )
        return;
    
    if ( version != PROTOCOL_VERSION )
    {
        util::log( "[WARN] Received bad protocol version: $\n", static_cast< int >( version ) );
        gi.socket.disconnect();
    }
    else
    {
        util::log( "[INFO] Correct protocol version received.\n" );
        gi.validated = true;
    }
}
