// DiscordRPC.Native.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "DiscordRPC.Native.h"

extern "C" DISCORDRPCNATIVE_API bool isConnected()
{
	/*
	bool connected = handle != nullptr && PipeConnected(handle);
	*/
	return false;
}

extern "C" DISCORDRPCNATIVE_API bool readFrame(int* buffer, int length)
{
	/*
	if (!PeekNamedPipe(handle)) return false;
	Read(handle, buffer, 0, length);
	*/

	return false;
}

extern "C" DISCORDRPCNATIVE_API bool writeFrame(int* buffer, int length)
{
	/*
	Write(handle, buffer, 0, length);
	*/
	return false;
}

extern "C" DISCORDRPCNATIVE_API void close()
{
	/*
	Close(handle);
	*/
}

extern "C" DISCORDRPCNATIVE_API bool open(char* pipename) 
{
	/*
	handle = OpenFile(pipename);
	*/
	return false;
}

// This is the constructor of a class that has been exported.
// see DiscordRPC.Native.h for the class definition
CDiscordRPCNative::CDiscordRPCNative()
{
    return;
}
