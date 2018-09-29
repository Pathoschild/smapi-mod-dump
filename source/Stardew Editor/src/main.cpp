#include "Editor.hpp"

#include <xnb/File.hpp>

int main( int argc, char* argv[] )
{
    util::Logger::setName( "Main log", "log.txt" );
    
    Editor editor( argc, argv );
    editor.run();
}
