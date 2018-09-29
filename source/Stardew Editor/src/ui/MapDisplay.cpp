#include "ui/MapDisplay.hpp"

#include <imgui.h>
#include <SFML/Window/Mouse.hpp>

#include "Editor.hpp"

MapDisplay::MapDisplay( Editor& theEditor, Ui& theUi )
:   editor( theEditor ),
    ui( theUi )
{
}

void MapDisplay::menu()
{
        if ( ImGui::BeginMenu( "Maps" ) )
        {
            if ( ImGui::BeginMenu( "Reload" ) )
            {
                bool refresh = false;
                ImGui::MenuItem( "Map list", nullptr, &refresh );
                if ( refresh )
                {
                    editor.refreshMapList();
                }
                
                ImGui::EndMenu();
            }
            ImGui::MenuItem( "", nullptr );
            
            for ( const auto& map : editor.maps )
            {
                bool selected = editor.map.getCurrentMap() == map;
                ImGui::MenuItem( map.c_str(), nullptr, &selected/*, eventFiles.find( map ) != eventFiles.end()*/ );
                if ( selected && editor.map.getCurrentMap() != map )
                {
                    editor.map.changeCurrentMap( map );
                    ui.sendRefresh( Refresh::Map );
                }
            }
            
            ImGui::EndMenu();
        }
}

void MapDisplay::update()
{
    // Toolbar
    int style = ImGuiWindowFlags_NoMove | ImGuiWindowFlags_NoResize;
    style |= ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags_NoScrollbar;
    style |= ImGuiWindowFlags_NoCollapse | ImGuiWindowFlags_NoSavedSettings;
    
    ImGui::SetNextWindowPos( ImVec2( -4, editor.window.getSize().y - 28 ) );
    ImGui::SetNextWindowSize( ImVec2( editor.window.getSize().x + 16, 28 ) );
    if ( ImGui::Begin( "", nullptr, style ) )
    {
        std::string tilePos = "";
        std::string pixelPos = "";
        if ( ui.isMouseOutside() )
        {
            sf::Vector2f pixel = editor.map.pixelToWorld( sf::Mouse::getPosition( editor.window ) );
            sf::Vector2i tile( pixel.x / TILE_SIZE, pixel.y / TILE_SIZE );
            if ( pixel.x < 0 ) tile.x -= 1;
            if ( pixel.y < 0 ) tile.y -= 1;
            
            tilePos = util::format( "($, $)", tile.x, tile.y );
            pixelPos = util::format< int, int >( "($, $)", pixel.x, pixel.y );
        }
        ImGui::Text( ( "Tile: " + tilePos ).c_str() );
        ImGui::SameLine( 150 );
        ImGui::Text( ( "Pixel: " + pixelPos ).c_str() );
    }
    ImGui::End();
}
