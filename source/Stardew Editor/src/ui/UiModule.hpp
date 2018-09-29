#ifndef UIMODULE_HPP
#define UIMODULE_HPP

#include "RefreshType.hpp"

class UiModule
{
    public:
        virtual ~UiModule();
        
        virtual void menu() = 0;
        virtual void update() = 0;
        
        virtual void refresh( Refresh::Type type );
};

#endif // UIMODULE_HPP
