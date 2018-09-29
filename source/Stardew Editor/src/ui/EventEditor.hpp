#ifndef EVENTEDITOR_HPP
#define EVENTEDITOR_HPP

#include <map>
#include <set>
#include <string>
#include <vector>

#include "game/Event.hpp"
#include "ui/UiModule.hpp"

class Editor;
class Ui;

class EventEditor : public UiModule
{
    public:
        EventEditor( Editor& theEditor, Ui& theUi );
        
        virtual void menu() override;
        virtual void update() override;
        
        virtual void refresh( Refresh::Type type ) override;
        
    private:
        Editor& editor;
        Ui& ui;
        
        std::set< std::string > eventFiles;
        std::map< int, Event::Data > events;
        std::map< std::string, Event::Data > eventBranches;
        Event::Data* active = nullptr;
        Event::Data dummy;
        
        std::vector< std::string > precTypeLabels;
        std::string precTypeLabelsStr;
        std::map< char, std::string > enumValuesStr;
        
        void loadEventList( const std::string& map );
        void reloadPreconditionTypes();
        
        void info();
        void preconditions();
        void actors();
        void commands();
};

#endif // EVENTEDITOR_HPP
