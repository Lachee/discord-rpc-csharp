// DiscordRPC.Native.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "DiscordRPC.Native.h"

extern "C" DISCORDRPCNATIVE_API bool isConnected(int* buffer, int length)
{
	return false;
}

extern "C" DISCORDRPCNATIVE_API bool readFrame(int* buffer, int length)
{
	return false;
}

extern "C" DISCORDRPCNATIVE_API bool writeFrame(int* buffer, int length)
{
	return false;
}

extern "C" DISCORDRPCNATIVE_API bool close()
{
	return false;
}

extern "C" DISCORDRPCNATIVE_API bool open(char* pipename) 
{
	return false;
}

// This is the constructor of a class that has been exported.
// see DiscordRPC.Native.h for the class definition
CDiscordRPCNative::CDiscordRPCNative()
{
    return;
}
