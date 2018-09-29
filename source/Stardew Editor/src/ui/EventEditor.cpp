#include "ui/EventEditor.hpp"

#include <boost/filesystem.hpp>
#include <imgui.h>
#include <imgui_internal.h> // I'm so terrible
#include <xnb/DictionaryType.hpp>
#include <xnb/File.hpp>
#include <xnb/StringType.hpp>

#include "Editor.hpp"
#include "Ui.hpp"

namespace
{
    bool wasItemActive( int check = -1 )
    {
        ImGuiContext* ctx = ImGui::GetCurrentContext();
        ImGuiWindow* window = ctx->CurrentWindow;
        auto id = check == -1 ? window->DC.LastItemId : check;
        return ctx->ActiveIdPreviousFrame == id && ctx->ActiveId != id;
    }
}

EventEditor::EventEditor( Editor& theEditor, Ui& theUi )
:   editor( theEditor ),
    ui( theUi )
{
    eventFiles.clear();
    fs::path path = fs::path( editor.config.getContentFolder() ) / "Data" / "Events";
    if ( fs::exists( path ) )
    {
        for ( fs::directory_iterator it( path ); it != fs::directory_iterator(); ++it )
        {
            fs::path file = ( * it );
            if ( file.extension() == ".xnb" && file.stem() == file.stem().stem() )
            {
                eventFiles.insert( file.stem().string() );
            }
        }
    }

    reloadPreconditionTypes();
}

void EventEditor::menu()
{
    if ( ImGui::BeginMenu( "Events" ) )
    {
        if ( ImGui::BeginMenu( "New" ) )
        {
            bool selected = false;
            ImGui::MenuItem( "w/ ID", nullptr, &selected );
            if ( selected )
            {
                dummy = Event::Data();
                dummy.id = 99999;
                active = &dummy;
                editor.map.clearActors();
            }

            selected = false;
            ImGui::MenuItem( "Named", nullptr, &selected );
            if ( selected )
            {
                dummy = Event::Data();
                dummy.branchName = "...";
                active = &dummy;
                editor.map.clearActors();
            }

            ImGui::EndMenu();
        }

        if ( ImGui::BeginMenu( "Reload" ) )
        {
            bool refresh = false;
            ImGui::MenuItem( "Precondition types", nullptr, &refresh );
            if ( refresh )
            {
                reloadPreconditionTypes();
            }

            refresh = false;
            ImGui::MenuItem( "Current map events", nullptr, &refresh );
            if ( refresh )
            {
                loadEventList( editor.map.getCurrentMap() );
            }

            ImGui::EndMenu();
        }

        if ( ImGui::BeginMenu( "Export" ) )
        {
            bool selected = false;
            ImGui::MenuItem( "Current", nullptr, &selected, active != nullptr );
            if ( selected )
            {
                ui.showExport( active->toGameFormat(), false );
            }

            selected = false;
            ImGui::MenuItem( "All", nullptr, &selected );
            if ( selected )
            {
                /*
                std::stringstream ss;
                std::ifstream file( ( fs::path( editor.config.getDataFolder() ) / "eventHeader.txt" ).string() );
                while ( true )
                {
                    std::string line;
                    std::getline( file, line );
                    if ( !file )
                        break;
                    ss << line << std::endl;
                }
                for ( const auto& entry : events )
                    ss << "    " << entry.second.toGameFormat() << " #!String" << std::endl;
                for ( const auto& entry : eventBranches )
                    ss << "    " << entry.second.toGameFormat() << " #!String" << std::endl;
                ss << std::endl;

                ui.showExport( ss.str() );
                */

                fs::path path = fs::path( editor.config.getExportFolder() ) / "Data" / "Events";
                if ( !fs::exists( path ) )
                    fs::create_directories( path );

                xnb::File file;
                file.data.reset( new xnb::DictionaryData( xnb::STRING_TYPE, xnb::STRING_TYPE ) );
                xnb::DictionaryData* dict = static_cast< xnb::DictionaryData* >( file.data.get() );

                for ( const auto& entry : events )
                    dict->data.emplace( new xnb::StringData( entry.second.toGameFormatKey() ), new xnb::StringData( entry.second.toGameFormatValue() ) );
                for ( const auto& entry : eventBranches )
                    dict->data.emplace( new xnb::StringData( entry.second.toGameFormatKey() ), new xnb::StringData( entry.second.toGameFormatValue() ) );

                if ( !file.writeToFile( ( path / ( editor.map.getCurrentMap() + ".xnb" ) ).string() ) )
                    util::log( "[ERROR] Failed to save exported XNB.\n" );
            }

            ImGui::EndMenu();
        }

        ImGui::MenuItem( "", nullptr );

        for ( auto& event : events )
        {
            bool selected = active == &event.second;
            bool wasSelected = selected;
            ImGui::MenuItem( util::toString( event.first ).c_str(), nullptr, &selected, true );
            if ( selected )
            {
                active = &event.second;
                if ( !wasSelected )
                {
                    editor.map.clearActors();
                    for ( const Event::Actor& actor : event.second.actors )
                    {
                        editor.map.addActor( actor.name.c_str(), actor.pos, actor.facing );
                    }
                }
            }
        }

        for ( auto& event : eventBranches )
        {
            bool selected = active == &event.second;
            bool wasSelected = selected;
            ImGui::MenuItem( event.first.c_str(), nullptr, &selected, true );
            if ( selected )
            {
                active = &event.second;
                if ( !wasSelected )
                {
                    editor.map.clearActors();
                    for ( const Event::Actor& actor : event.second.actors )
                    {
                        editor.map.addActor( actor.name.c_str(), actor.pos, actor.facing );
                    }
                }
            }
        }

        ImGui::EndMenu();
    }
}

void EventEditor::update()
{
    if ( !active )
        return;

    info();
    preconditions();
    actors();
    commands();
}

void EventEditor::refresh( Refresh::Type type )
{
    if ( type == Refresh::Map )
        loadEventList( editor.map.getCurrentMap() );
}

void EventEditor::info()
{
    ImGui::SetNextWindowPos( ImVec2( 25, 25 ), ImGuiSetCond_Appearing );
    ImGui::SetNextWindowSize( ImVec2( 250, 125 ), ImGuiSetCond_Appearing );
    if ( ImGui::Begin( "Event Info" ) )
    {
        if ( active->id != -1 )
        {
            ImGui::InputInt( "Event ID", &active->id );
            if ( wasItemActive( ImGui::GetCurrentWindow()->GetID( "Event ID" ) ) ||
                 !ImGui::IsItemActive() && active->oldId != active->id )
            {
                Event::Data data = ( * active );
                auto it = events.find( active->oldId );
                if ( it != events.end() )
                    events.erase( it );
                data.oldId = data.id;
                events.insert( std::make_pair( data.id, data ) );
                active = &events[ data.id ];
            }
            ImGui::InputText( "Music", &active->music[ 0 ], 31 );
            ImGui::InputInt2( "Viewport", &active->viewport.x );

            if ( ImGui::Button( "Play" ) )
                editor.gi.playEvent( active->toGameFormatValue() );
        }
        else
        {
            ImGui::InputText( "Event Name", &active->branchName[ 0 ], 31 );
            if ( wasItemActive() ||
                 !ImGui::IsItemActive() && active->oldBranchName != active->branchName )
            {
                Event::Data data = ( * active );
                auto it = eventBranches.find( active->oldBranchName.c_str() );
                if ( it != eventBranches.end() )
                    eventBranches.erase( it );
                data.oldBranchName = data.branchName;
                eventBranches.insert( std::make_pair( data.branchName.c_str(), data ) );
                active = &eventBranches[ data.branchName.c_str() ];
            }
        }

    }
    ImGui::End();
}

void EventEditor::preconditions()
{
    if ( active->id == -1 )
        return;

    ImGui::SetNextWindowPos( ImVec2( 25, 150 ), ImGuiSetCond_Appearing );
    ImGui::SetNextWindowSize( ImVec2( 250, 200 ), ImGuiSetCond_Appearing );
    if ( ImGui::Begin( "Event Preconditions" ) )
    {
        int precNum = 0;
        for ( auto precIt = active->preconditions.begin(); precIt != active->preconditions.end(); ++precIt, ++precNum )
        {
            Event::Precondition& prec = ( * precIt );
            const Event::PreconditionType& type = Event::PreconditionType::types[ prec.type ];
            int selPrec = std::find( precTypeLabels.begin(), precTypeLabels.end(), type.label ) - precTypeLabels.begin();
            int oldSelPrec = selPrec;
            ImGui::Combo( util::format( "Type##prec$", precNum ).c_str(), &selPrec, precTypeLabelsStr.c_str() );
            if ( selPrec != oldSelPrec )
            {
                for ( const auto& checkType : Event::PreconditionType::types )
                {
                    if ( checkType.second.label == precTypeLabels[ selPrec ] )
                    {
                        prec = Event::Precondition::init( checkType.second );
                        break;
                    }
                }
                continue;
            }

            std::size_t iVal = 0;
            for ( std::size_t i = 0; i < type.paramTypes.size(); ++i, ++iVal )
            {
                switch ( type.paramTypes[ i ] )
                {
                    case Event::ParamType::Integer:
                        {
                            int x = util::fromString< int >( prec.params[ iVal ] );
                            int oldX = x;
                            ImGui::InputInt( util::format( "$##prec$param$", type.paramLabels[ i ], precNum, i ).c_str(), &x );
                            if ( x != oldX )
                                prec.params[ iVal ] = util::toString( x );
                        }
                        break;

                    case Event::ParamType::Double:
                        {
                            float x = util::fromString< float >( prec.params[ iVal ] );
                            float oldX = x;
                            ImGui::InputFloat( util::format( "$##prec$param$", type.paramLabels[ i ], precNum, i ).c_str(), &x );
                            if ( x != oldX )
                                prec.params[ iVal ] = util::toString( x );
                        }
                        break;

                    case Event::ParamType::Bool:
                        {
                            bool x = prec.params[ iVal ] == "true";
                            bool oldX = x;
                            ImGui::Checkbox( util::format( "$##prec$param$", type.paramLabels[ i ], precNum, i ).c_str(), &x );
                            if ( x != oldX )
                                prec.params[ iVal ] = x ? "true" : "false";
                        }
                        break;

                    case Event::ParamType::String:
                    case Event::ParamType::Unknown:
                        {
                            prec.params[ iVal ].resize( 32, '\0' );
                            ImGui::InputText( util::format( "$##prec$param$", type.paramLabels[ i ], precNum, i ).c_str(), &prec.params[ iVal ][ 0 ], 31 );
                        }
                        break;

                    case Event::ParamType::EnumOne:
                        {
                            int selEnum = std::find( type.enumValues.begin(), type.enumValues.end(), prec.params[ iVal ] ) - type.enumValues.begin();
                            int oldSelEnum = selEnum;
                            ImGui::Combo( util::format( "##prec$param$", precNum, i ).c_str(), &selEnum, &enumValuesStr[ type.id ][ 0 ] );
                            if ( selEnum != oldSelEnum )
                                prec.params[ iVal ] = type.enumValues[ selEnum ];
                        }
                        break;

                    case Event::ParamType::EnumMany:
                        {
                            for ( std::size_t e = 0; e < type.enumValues.size(); ++e )
                            {
                                const std::string& val = type.enumValues[ e ];
                                auto valIt = std::find( prec.params.begin(), prec.params.end(), val );
                                bool sel = valIt != prec.params.end();
                                bool oldSel = sel;
                                ImGui::Checkbox( util::format( "$##prec$param$val$", val, precNum, i, e ).c_str(), &sel );
                                if ( sel != oldSel )
                                {
                                    if ( sel )
                                        prec.params.push_back( val );
                                    else
                                        prec.params.erase( valIt );
                                }
                            }
                        }
                        break;

                    case Event::ParamType::Position:
                        {
                            sf::Vector2i x( util::fromString< int >( prec.params[ iVal ] ), util::fromString< int >( prec.params[ iVal + 1 ] ) );
                            sf::Vector2i oldX;
                            ImGui::InputInt2( util::format( "$##prec$param$", type.paramLabels[ i ], precNum, i ).c_str(), &x.x );
                            if ( x != oldX )
                            {
                                prec.params[ i ] = util::toString( x.x );
                                prec.params[ i + 1 ] = util::toString( x.y );
                            }
                            ++iVal;
                        }
                        break;
                }
            }
            if ( ImGui::Button( util::format( "Delete precondition##prec$", precNum ).c_str() ) )
            {
                active->preconditions.erase( precIt );
                break;
            }
            ImGui::Separator();
        }
        if ( ImGui::Button( "New precondition" ) )
        {
            active->preconditions.push_back( Event::Precondition::init( Event::PreconditionType::types.begin()->second ) );
        }
    }
    ImGui::End();
}

void EventEditor::actors()
{
    if ( active->id == -1 )
        return;

    ImGui::SetNextWindowPos( ImVec2( 25, 375 ), ImGuiSetCond_Appearing );
    ImGui::SetNextWindowSize( ImVec2( 250, 200 ), ImGuiSetCond_Appearing );
    if ( ImGui::Begin( "Event Actors" ) )
    {
        int actorNum = 0;
        for ( auto actorIt = active->actors.begin(); actorIt != active->actors.end(); ++actorIt, ++actorNum )
        {
            Event::Actor& actor = ( * actorIt );
            Actor* mapActor = editor.map.getActor( actor.oldName.c_str() );
            actor.name.resize( 32, '\0' );
            ImGui::InputText( util::format( "Name##actor$", actorNum ).c_str(), &actor.name[ 0 ], 31 );
            if ( wasItemActive() )
            {
                if ( mapActor )
                    editor.map.removeActor( actor.oldName.c_str() );
                editor.map.addActor( actor.name.c_str(), actor.pos, actor.facing );
                actor.oldName = actor.name;
            }
            if ( ImGui::InputInt2( util::format( "Position##actor$", actorNum ).c_str(), &actor.pos.x ) )
            {
                if ( mapActor )
                    mapActor->setPosition( actor.pos );
            }
            if ( ImGui::InputInt( util::format( "Facing##actor$", actorNum ).c_str(), &actor.facing ) )
            {
                actor.facing = actor.facing % 4;
                if ( mapActor )
                    mapActor->setFacing( actor.facing );
            }

            if ( ImGui::Button( util::format( "Delete actor##actor$", actorNum ).c_str() ) )
            {
                active->actors.erase( actorIt );
                if ( mapActor )
                    editor.map.removeActor( actor.name.c_str() );
                break;
            }

            ImGui::Separator();
        }

        if ( ImGui::Button( "New actor" ) )
        {
            active->actors.push_back( Event::Actor() );
        }
    }
    ImGui::End();
}

void EventEditor::commands()
{
    ImGui::SetNextWindowPos( ImVec2( editor.window.getSize().x - 25 - 250, 25 ), ImGuiSetCond_Appearing );
    ImGui::SetNextWindowSize( ImVec2( 250, 500 ), ImGuiSetCond_Appearing );
    if ( ImGui::Begin( "Event Commands" ) )
    {
        int cmdNum = 0;
        for ( auto cmdIt = active->commands.begin(); cmdIt != active->commands.end(); ++cmdIt, ++cmdNum )
        {
            Event::Command& command = ( * cmdIt );
            command.cmd.resize( 512, '\0' );
            ImGui::InputText( util::format( "Command##cmd$", cmdNum ).c_str(), &command.cmd[ 0 ], 511 );

            if ( ImGui::Button( util::format( "Delete command##cmd$", cmdNum ).c_str() ) )
            {
                active->commands.erase( cmdIt );
                break;
            }
            ImGui::SameLine();
            if ( cmdIt != active->commands.begin() )
            {
                if ( ImGui::Button( util::format( "Up##cmd$", cmdNum ).c_str(), ImVec2( 50, 0 ) ) )
                {
                    auto tmpIt = cmdIt; --tmpIt;
                    std::swap( ( * tmpIt ), ( * cmdIt ) );
                }
            }
            else ImGui::Dummy( ImVec2( 50, 0 ) );
            auto tmpIt = cmdIt; ++tmpIt;
            if ( tmpIt != active->commands.end() )
            {
                ImGui::SameLine();
                if ( ImGui::Button( util::format( "Down##cmd$", cmdNum ).c_str(), ImVec2( 50, 0 ) ) )
                {
                    std::swap( ( * tmpIt ), ( * cmdIt ) );
                }
            }
            ImGui::Separator();
        }

        if ( ImGui::Button( "New command" ) )
        {
            active->commands.push_back( Event::Command() );
        }
    }
    ImGui::End();
}


void EventEditor::loadEventList( const std::string& map )
{
    events.clear();
    eventBranches.clear();
    active = nullptr;

    xnb::File file;
    if ( !file.loadFromFile( ( fs::path( editor.config.getContentFolder() ) / "data" / "Events" / ( map + ".xnb" ) ).string() ) )
        return;

    const xnb::DictionaryData* data = dynamic_cast< const xnb::DictionaryData* >( file.data.get() );
    if ( data == nullptr )
    {
        util::log( "Event data must be a dictionary! $\n", file.data->toString() );
        return;
    }

    for ( const auto& pair : data->data )
    {
        const xnb::StringData* keyData = dynamic_cast< const xnb::StringData* >( pair.first.get() );
        const xnb::StringData* valueData = dynamic_cast< const xnb::StringData* >( pair.second.get() );
        if ( keyData == nullptr || valueData == nullptr )
        {
            util::log( "Bad event data type? $ $\n", pair.first->toString(), pair.second->toString() );
            continue;
        }

        Event::Data event = Event::Data::fromGameFormat( keyData->value, valueData->value );
        if ( event.id == -1 && event.branchName == "" )
        {
            util::log( "Bad event data? $ $\n", pair.first->toString(), pair.second->toString() );
            continue;
        }

        if ( event.id != -1 )
        {
            events.insert( std::make_pair( event.id, event ) );
        }
        else
        {
            eventBranches.insert( std::make_pair( event.branchName.c_str(), event ) );
        }
    }
}

void EventEditor::reloadPreconditionTypes()
{
    Event::PreconditionType::types = Event::PreconditionType::loadTypes( ( fs::path( editor.config.getDataFolder() ) / "preconditions.txt" ).string() );
    precTypeLabels.clear();
    for ( const auto& type : Event::PreconditionType::types )
    {
        precTypeLabels.push_back( type.second.label );

        if ( type.second.enumValues.size() > 0 )
        {
            std::string str = "";
            for ( const std::string& val : type.second.enumValues )
            {
                str += val + '\0';
            }
            str += '\0';

            enumValuesStr[ type.second.id ] = str;
        }
    }

    precTypeLabelsStr = "";
    for ( const std::string& label : precTypeLabels )
    {
        precTypeLabelsStr += label + '\0';
    }
    precTypeLabelsStr += '\0';
}
