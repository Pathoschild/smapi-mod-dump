#ifndef GAMEINTEGRATION_HPP
#define GAMEINTEGRATION_HPP

#include <SFML/Network/TcpListener.hpp>
#include <SFML/Network/TcpSocket.hpp>

class Editor;

class GameIntegration
{
    public:
        static constexpr sf::Uint16 PORT = 24700;
        
        GameIntegration( Editor& theEditor );
        
        void update();
        
        void playEvent( const std::string& eventValueStr );
    
    private:
        Editor& editor;
        sf::TcpListener listener;
        sf::TcpSocket socket;
        bool failedListening = false;
        
        bool validated = false;
        
        friend class ProtocolPacket;
        
        void listen();
        void send( sf::Packet& packet );
};

#endif // GAMEINTEGRATION_HPP
