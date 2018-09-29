#include "GameIntegration.hpp"

#include <SFML/Network/Packet.hpp>
#include <util/String.hpp>

#include "Editor.hpp"
#include "integration/Packet.hpp"
#include "integration/PlayEventPacket.hpp"

constexpr sf::Uint16 GameIntegration::PORT;

GameIntegration::GameIntegration( Editor& theEditor )
:   editor( theEditor )
{
    listener.setBlocking( false );
    socket.setBlocking( false );
    listen();
}

void GameIntegration::update()
{
    if ( socket.getLocalPort() == 0 )
    {
        if ( failedListening )
            return;
        
        if ( listener.getLocalPort() == 0 )
            listen();
        
        if ( listener.accept( socket ) == sf::Socket::Done )
        {
            util::log( "[INFO] Received connection.\n" );
            listener.close();
            validated = false;
        }
        else return;
    }
    
    sf::Packet packetData;
    if ( socket.receive( packetData ) == sf::Socket::Done )
    {
        sf::Uint8 id;
        packetData >> id;
        if ( !packetData )
            return;
        
        auto packet = Packet::createPacket( id );
        if ( packet )
            packet->process( editor, ( * this ), packetData );
    }
    
    if ( socket.getLocalPort() == 0 )
    {
        util::log( "[INFO] Lost connection.\n" );
    }
}

void GameIntegration::playEvent( const std::string& eventValueStr )
{
    if ( socket.getLocalPort() == 0 )
        return;
    
    util::log( "[INFO] Sending play event command '$' to game.\n", eventValueStr );
    
    PlayEventPacket toSend;
    toSend.location = editor.map.getCurrentMap();
    toSend.data = eventValueStr;
    
    sf::Packet packet;
    packet << static_cast< sf::Uint8 >( Packet::PlayEvent );
    toSend.write( packet );
    send( packet );
}

void GameIntegration::listen()
{
    util::log( "Listening...\n" );
    if ( listener.listen( PORT ) != sf::Socket::Done )
    {
        util::log( util::format( "[WARN] Failed to listen to port $!\n", PORT ) );
        failedListening = true;
    }
    else failedListening = false;
}

void GameIntegration::send( sf::Packet& packet )
{
    socket.setBlocking( true );
    while ( true )
    {
        auto stat = socket.send( packet );
        if ( stat == sf::Socket::Partial )
            continue;
        else
        {
            if ( stat == sf::Socket::Disconnected )
            {
                util::log( "[WARN] Lost connection!\n", stat );
            }
            else if ( stat != sf::Socket::Done )
                util::log( "[WARN] Error sending packet: $\n", stat );
            break;
        }
    }
    socket.setBlocking( false );
}
