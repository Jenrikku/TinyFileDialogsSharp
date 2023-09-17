cmake_minimum_required(VERSION 3.9)
project(tinyfiledialogs VERSION 3.14.0)

add_library(${PROJECT_NAME} SHARED tinyfiledialogs.c tinyfiledialogs.h)
set_target_properties(${PROJECT_NAME} PROPERTIES VERSION ${PROJECT_VERSION})
