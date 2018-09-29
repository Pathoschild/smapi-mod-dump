#ifndef DIALOGUEEDITOR_HPP
#define DIALOGUEEDITOR_HPP

#include <map>
#include <set>
#include <string>
#include <vector>

//#include "game/Dialogue.hpp"
#include "ui/UiModule.hpp"

class Editor;
class Ui;

class DialogueEditor : public UiModule
{
    public:
        DialogueEditor( Editor& theEditor, Ui& theUi );

        virtual void menu() override;
        virtual void update() override;

        virtual void refresh( Refresh::Type type ) override;

    private:
        Editor& editor;
        Ui& ui;

        std::set< std::string > dialogueFiles;
        const std::string* currentDialogueFile = nullptr;
};

#endif // DIALOGUEEDITOR_HPP
