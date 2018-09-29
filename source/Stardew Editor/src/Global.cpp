#include "Global.hpp" // Not needed but whatever

#include<iostream>
#include <windows.h>

std::string getRegistryValue( const std::string& key, const std::string& value )
{
    HKEY hkey;
    auto err = RegOpenKeyEx( HKEY_LOCAL_MACHINE, key.c_str(), 0, KEY_READ, &hkey );
    if ( err != ERROR_SUCCESS )
        return "";

    unsigned long size = 512;
    unsigned char buffer[ size ];
    err = RegQueryValueEx( hkey, value.c_str(), 0, nullptr, &buffer[ 0 ], &size );
    RegCloseKey( hkey );

    std::string ret = reinterpret_cast< char* >( buffer );
    return ret;
}
