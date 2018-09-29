#ifndef UI_HPP
#define UI_HPP

#include <map>
#include <set>
#include <SFML/System/Clock.hpp>
#include <string>

#include "ui/UiModule.hpp"

class Editor;

namespace sf
{
    class Event;
    class RenderWindow;
}

class DialogueEditor;
class EventEditor;
class MapDisplay;
class SoundPlayer;

class Ui
{
    public:
        Ui( Editor& theEditor );

        void update();
        void update( const sf::Event& event );
        void render( sf::RenderWindow& window );

        bool isMouseOutside() const;

        void showExport( const std::string& str, bool multi = true );

        void sendRefresh( Refresh::Type type );

        DialogueEditor* dialogueEditor;
        EventEditor* eventEditor;
        MapDisplay* mapDisplay;
        SoundPlayer* soundPlayer;

    private:
        Editor& editor;
        sf::Clock delta;
        bool firstUpdate = true;

        std::vector< std::unique_ptr< UiModule > > modules;

        bool showingConfig = false;

        std::string exported;
        bool exportedMulti = false;

        void mainMenu();
        void other();
};

#endif // UI_HPP
