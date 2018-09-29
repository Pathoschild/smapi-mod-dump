#ifndef UI_SOUNDPLAYER_HPP
#define UI_SOUNDPLAYER_HPP

#include <SFML/Audio/Music.hpp>
#include <string>
#include <vector>

#include "ui/UiModule.hpp"

class Editor;
class Ui;

class SoundPlayer : public UiModule
{
    public:
        SoundPlayer( Editor& theEditor, Ui& theUi );
        
        virtual void menu() override;
        virtual void update() override;
        
        virtual void refresh( Refresh::Type type );
        
        bool isShowing() const;
        void show();
        void hide();
        
        std::string getCurrentName() const;
        sf::Time getCurrentProgress() const;
        sf::Time getCurrentDuration() const;
    
    private:
        Editor& editor;
        Ui& ui;
        
        bool showing = false;
        
        std::vector< std::string > choices;
        std::string choicesStr;
        
        std::string current;
        sf::Music playing;
        
        void refreshList();
};

#endif // UI_SOUNDPLAYER_HPP
