// DiscordRPC.Native.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <windows.h>
#include "DiscordRPC.Native.h"

HANDLE pipe;
bool isOpen;

extern "C" DISCORDRPCNATIVE_API bool isConnected()
{
	/*
	bool connected = handle != nullptr && PipeConnected(handle);
	*/
	return pipe != INVALID_HANDLE_VALUE && isOpen;
}

extern "C" DISCORDRPCNATIVE_API bool readFrame(unsigned char* buffer, int length)
{
	/*
	if (!PeekNamedPipe(handle)) return false;
	Read(handle, buffer, 0, length);
	*/

	return false;
}

extern "C" DISCORDRPCNATIVE_API bool writeFrame(unsigned char* buffer, int length)
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
	//Creates a connection to the pipe
	pipe = ::CreateFileA(pipename, GENERIC_READ | GENERIC_WRITE, 0, nullptr, OPEN_EXISTING, 0, nullptr);
	if (pipe != INVALID_HANDLE_VALUE) 
	{
		isOpen = true;
		return true;
	}

	//We have a error, better find out why.
	auto lasterr = GetLastError();

	//Pipe wasnt found
	if (lasterr == ERROR_FILE_NOT_FOUND)
		return false;

	//Pipe is just busy
	if (lasterr == ERROR_PIPE_BUSY) 
	{
		if (!WaitNamedPipeA(pipename, 10000))
		{
			//Failed to open, terminate
			return false;
		}
		else 
		{
			//Attempt to open it again
			return open(pipename);
		}
	}

	//Some other magically error occured
	return false;
}

// This is the constructor of a class that has been exported.
// see DiscordRPC.Native.h for the class definition
CDiscordRPCNative::CDiscordRPCNative()
{
    return;
}
