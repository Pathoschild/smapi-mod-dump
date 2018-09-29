#ifndef MAP_HPP
#define MAP_HPP

#include <map>
#include <SFML/Graphics/Sprite.hpp>
#include <SFML/Graphics/Texture.hpp>
#include <SFML/Graphics/View.hpp>
#include <string>
#include <tbin/Map.hpp>
#include <vector>

#include "Actor.hpp"

class Editor;

namespace sf
{
    class Event;
    class RenderWindow;
}

class Map
{
    public:
        Map( Editor& editor );

        void update();
        void update( const sf::Event& event );
        void render( sf::RenderWindow& window );

        void changeCurrentMap( const std::string& map );
        std::string getCurrentMap() const;

        enum ClickMode
        {
            Panning,
        } clickMode = Panning;

        sf::Vector2f pixelToWorld( sf::Vector2i pixel ) const;

        void clearActors();
        Actor& addActor( const std::string& name, sf::Vector2i pos, int facing );
        Actor* getActor( const std::string& name );
        const Actor* getActor( const std::string& name ) const;
        void removeActor( const std::string& name );
        std::vector< std::string > getActorList() const;

    private:
        Editor& editor;
        bool firstUpdate = true;

        sf::View view;
        std::string current;
        sf::Texture tex;
        sf::Sprite spr;

        sf::Texture grid;

        std::map< std::string, Actor > actors;

        bool dragging = false;
        sf::Vector2f dragFrom;
};

#endif // MAP_HPP
