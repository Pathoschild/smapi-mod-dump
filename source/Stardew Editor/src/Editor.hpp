#ifndef EDITOR_HPP
#define EDITOR_HPP

#include <set>
#include <SFML/Graphics/RenderWindow.hpp>
#include <SFML/Graphics/Texture.hpp>
#include <string>

#include "Config.hpp"
#include "integration/GameIntegration.hpp"
#include "Map.hpp"
#include "Ui.hpp"

namespace sf
{
    class RenderWindow;
}

class Editor
{
    public:
        static const char* CONFIG_FILE;
        
        Editor( int argc, char* argv[] );
        ~Editor();
        
        void run();
        
        Config config;
        sf::RenderWindow window;
        Map map;
        Ui ui;
        GameIntegration gi;
        
        sf::Texture dialogueFont;
        
        void refreshMapList();
        std::set< std::string > maps;
        
        void reloadSoundList();
        std::vector< std::string > getSoundCueList() const;
        std::string getPathForSound( const std::string& cue ) const;
    
    private:
        const std::string EXEC_PATH;
        
        std::map< std::string, std::vector< std::string > > sounds;
};

#endif // EDITOR_HPP
