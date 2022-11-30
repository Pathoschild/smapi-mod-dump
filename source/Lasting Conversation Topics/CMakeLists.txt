/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ModOrganizer2/modorganizer-basic_games
**
*************************************************/

cmake_minimum_required(VERSION 3.16)

project(basic_games LANGUAGES NONE)
set(project_type python_module_plugin)
set(enable_warnings OFF)
set(create_translations OFF)

if(DEFINED DEPENDENCIES_DIR)
    include(${DEPENDENCIES_DIR}/modorganizer_super/cmake_common/project.cmake)
	include(${DEPENDENCIES_DIR}/modorganizer_super/cmake_common/src.cmake)
else()
	include(../cmake_common/project.cmake)
	include(../../cmake_common/src.cmake)
endif()
