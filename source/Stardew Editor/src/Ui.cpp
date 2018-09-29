#include "Ui.hpp"

#include <boost/filesystem.hpp>
#include <imgui.h>
#include <imgui-sfml.h>
#include <SFML/Graphics/Sprite.hpp>
#include <SFML/Window/Mouse.hpp>
#include <util/File.hpp>
#include <util/String.hpp>

#include "Editor.hpp"
#include "ui/DialogueEditor.hpp"
#include "ui/EventEditor.hpp"
#include "ui/MapDisplay.hpp"
#include "ui/SoundPlayer.hpp"

Ui::Ui( Editor& theEditor )
:   editor( theEditor )
{
}

void Ui::update()
{
    ImGui::SFML::Update( editor.window, delta.restart() );

    mainMenu();
    other();
    for ( auto& module : modules )
    {
        module->update();
    }
}

void Ui::update( const sf::Event& event )
{
    ImGui::SFML::ProcessEvent( event );
}

void Ui::render( sf::RenderWindow& window )
{
    if ( firstUpdate )
    {
        modules.push_back( std::unique_ptr< UiModule >( mapDisplay     = new MapDisplay ( editor, ( * this ) ) ) );
        modules.push_back( std::unique_ptr< UiModule >( eventEditor    = new EventEditor( editor, ( * this ) ) ) );
        modules.push_back( std::unique_ptr< UiModule >( dialogueEditor = new DialogueEditor( editor, ( * this ) ) ) );
        modules.push_back( std::unique_ptr< UiModule >( soundPlayer    = new SoundPlayer( editor, ( * this ) ) ) );
        firstUpdate = false;
    }

    ImGui::Render();
}

bool Ui::isMouseOutside() const
{
    return !ImGui::IsMouseHoveringAnyWindow();
}

void Ui::showExport( const std::string& str, bool multi )
{
    exported = str;
    exportedMulti = multi;
}

void Ui::sendRefresh( Refresh::Type type )
{
    for ( auto& module : modules )
        module->refresh( type );
}

void Ui::mainMenu()
{
    if ( ImGui::BeginMainMenuBar() )
    {
        for ( auto& module : modules )
            module->menu();

        if ( ImGui::BeginMenu( "Other" ) )
        {
            bool wasShowingConfig = showingConfig;
            ImGui::MenuItem( "Show config window", nullptr, &showingConfig );
            if ( showingConfig != wasShowingConfig && !showingConfig )
            {
                editor.config.saveToFile( Editor::CONFIG_FILE );
            }

            bool selected = exported != "";
            ImGui::MenuItem( "Show export window", nullptr, &selected, selected );
            if ( !selected )
                exported = "";

            ImGui::EndMenu();
        }

        ImGui::EndMainMenuBar();
    }
}

void Ui::other()
{
    if ( exported != "" )
    {
        if ( ImGui::Begin( "Exported" ) )
        {
            if ( exportedMulti )
                ImGui::InputTextMultiline( "", &exported[ 0 ], exported.length(), ImVec2( 0, 0 ), ImGuiInputTextFlags_ReadOnly );
            else
                ImGui::InputText( "", &exported[ 0 ], exported.length(), ImGuiInputTextFlags_ReadOnly );
        }
        ImGui::End();
    }

    if ( showingConfig )
    {
        if ( ImGui::Begin( "Configuration" ) )
        {
            ImGui::InputText( "Content", &editor.config.content[ 0 ], 511 );
            //ImGui::InputText( "Data path", &editor.config.dataFolder[ 0 ], 511 );
            ImGui::InputText( "Extracted sounds", &editor.config.extractedSounds[ 0 ], 511 );
        }
        ImGui::End();
    }
}
