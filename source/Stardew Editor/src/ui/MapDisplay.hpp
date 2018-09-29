#ifndef MAPDISPLAY_HPP
#define MAPDISPLAY_HPP

#include "ui/UiModule.hpp"

class Editor;
class Ui;

class MapDisplay : public UiModule
{
    public:
        MapDisplay( Editor& theEditor, Ui& theUi );
        
        virtual void menu() override;
        virtual void update() override;
        
    private:
        Editor& editor;
        Ui& ui;
};

#endif // MAPDISPLAY_HPP
