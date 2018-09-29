#include "integration/PlayEventPacket.hpp"

#include <SFML/Network/Packet.hpp>

void PlayEventPacket::write( sf::Packet& out )
{
    out << location << data;
}

void PlayEventPacket::process( Editor& editor, GameIntegration& gi, sf::Packet& in )
{
}
