#include "ui/DialogueEditor.hpp"

#include <boost/filesystem.hpp>
#include <imgui.h>
#include <xnb/DictionaryType.hpp>
#include <xnb/File.hpp>
#include <xnb/StringType.hpp>

#include "Editor.hpp"
#include "Ui.hpp"

DialogueEditor::DialogueEditor( Editor& theEditor, Ui& theUi )
:   editor( theEditor ),
    ui( theUi )
{
    dialogueFiles.clear();
    fs::path path = fs::path( editor.config.getContentFolder() ) / "Characters" / "Dialogue";
    if ( fs::exists( path ) )
    {
        for ( fs::directory_iterator it( path ); it != fs::directory_iterator(); ++it )
        {
            fs::path file = ( * it );
            if ( file.extension() == ".xnb" && file.stem() == file.stem().stem() )
            {
                dialogueFiles.insert( file.stem().string() );
            }
        }
    }
}

void DialogueEditor::menu()
{
    if ( ImGui::BeginMenu( "Dialogue" ) )
    {
        ImGui::MenuItem( "New - TODO", nullptr );
        ImGui::MenuItem( "Reload - TODO", nullptr );
        ImGui::MenuItem( "Export - TODO", nullptr );

        ImGui::MenuItem( "", nullptr );
        for ( auto& dialogue : dialogueFiles )
        {
            bool selected = currentDialogueFile == &dialogue;
            bool wasSelected = selected;
            ImGui::MenuItem( dialogue.c_str(), nullptr, &selected, true );
            if ( selected )
            {
                currentDialogueFile = &dialogue;
                if ( !wasSelected )
                {
                    // TODO
                }
            }
        }

        ImGui::EndMenu();
    }
}

void DialogueEditor::update()
{
}

void DialogueEditor::refresh( Refresh::Type type )
{
}
