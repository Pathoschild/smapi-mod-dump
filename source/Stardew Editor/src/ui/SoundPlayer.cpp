#include "ui/SoundPlayer.hpp"

#include <algorithm>
#include <imgui.h>
#include <SFML/System/Sleep.hpp>

#include "Editor.hpp"

SoundPlayer::SoundPlayer( Editor& theEditor, Ui& theUi )
:   editor( theEditor ),
    ui( theUi )
{
    refreshList();
}

void SoundPlayer::menu()
{
    if ( ImGui::BeginMenu( "Sounds" ) )
    {
        if ( ImGui::BeginMenu( "Reload" ) )
        {
            bool refresh = false;
            ImGui::MenuItem( "Sound list", nullptr, &refresh );
            if ( refresh )
            {
                editor.reloadSoundList();
            }
            
            ImGui::EndMenu();
        }
        
        bool selected = isShowing();
        ImGui::MenuItem( "Player", nullptr, &selected );
        if ( selected != isShowing() )
        {
            if ( selected ) show();
            else hide();
        }
        
        ImGui::EndMenu();
    }
}

void SoundPlayer::update()
{
    if ( !showing )
        return;
    
    ImGui::SetNextWindowSize( ImVec2( 200, 100 ), ImGuiSetCond_Appearing );
    if ( ImGui::Begin( "Sound Player" ) )
    {
        int sel = std::find( choices.begin(), choices.end(), current ) - choices.begin();
        int oldSel = sel;
        ImGui::Combo( "Cue", &sel, choicesStr.c_str() );
        
        if ( sel != oldSel )
        {
            playing.stop();
            current = choices[ sel ];
        }
        
        if ( current != "" )
        {
            float at = playing.getPlayingOffset().asSeconds();
            float oldAt = at;
            ImGui::SliderFloat( "Time", &at, 0, playing.getDuration().asSeconds() );
            if ( at != oldAt )
            {
                if ( at != playing.getDuration().asSeconds() )
                    playing.setPlayingOffset( sf::seconds( at ) );
                else playing.stop();
            }
            
            bool isPlaying = playing.getStatus() == sf::Music::Playing;
            std::string str = isPlaying ? "Stop" : "Play";
            if ( ImGui::Button( str.c_str() ) )
            {
                if ( isPlaying )
                    playing.pause();
                else if ( playing.getStatus() == sf::Music::Paused )
                    playing.play();
                else
                {
                    std::string path = editor.getPathForSound( current );
                    if ( !playing.openFromFile( path ) )
                        util::log( "[WARN] Failed to open '$' as sf::Music.\n", path );
                    else playing.play();
                }
            }
        }
    }
    ImGui::End();
}

void SoundPlayer::refresh( Refresh::Type type )
{
    if ( type == Refresh::Sounds )
        refreshList();
}

bool SoundPlayer::isShowing() const
{
    return showing;
}

void SoundPlayer::show()
{
    showing = true;
}

void SoundPlayer::hide()
{
    showing = false;
}

std::string SoundPlayer::getCurrentName() const
{
    return current;
}

sf::Time SoundPlayer::getCurrentProgress() const
{
    return playing.getPlayingOffset();
}

sf::Time SoundPlayer::getCurrentDuration() const
{
    return playing.getDuration();
}

void SoundPlayer::refreshList()
{
    choices = editor.getSoundCueList();
    if ( std::find( choices.begin(), choices.end(), current ) == choices.end() )
    {
        current = "";
        playing.stop();
    }
    
    choicesStr = "";
    for ( const std::string& choice : choices )
    {
        choicesStr += choice + '\0';
    }
    choicesStr += '\0';
}
