// DiscordRPC.Native.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "DiscordRPC.Native.h"
#include <windows.h>

HANDLE pipe;
bool isOpen;

extern "C" DISCORDRPCNATIVE_API bool isConnected()
{
	/*
	bool connected = handle != nullptr && PipeConnected(handle);
	*/
	return isOpen && pipe != INVALID_HANDLE_VALUE;
}

extern "C" DISCORDRPCNATIVE_API int readFrame(unsigned char* buffer, int length)
{
	/*
	if (!PeekNamedPipe(handle)) return false;
	Read(handle, buffer, 0, length);
	*/

	if (length == 0) { return -1; }
	if (!isConnected()) { return -2; }

	//Prepare how many bytes we have read
	DWORD bytesAvailable = 0;

	//Attempt to peek at the available pipe
	if (::PeekNamedPipe(pipe, nullptr, 0, nullptr, &bytesAvailable, nullptr)) 
	{
		//Check if we have bytes available to read
		if (bytesAvailable > 0) 
		{
			//Read the bytes. 
			//TODO: Make the Bytes Read appart of the output
			DWORD bytesToRead = (DWORD)length;
			DWORD bytesRead = 0;

			//Attempt to read the bytes
			if (::ReadFile(pipe, buffer, bytesToRead, &bytesRead, nullptr) == TRUE)
			{
				return bytesRead;
			}
			else 
			{
				//We failed to read anything, close the pipe (broken)
				close();
				return -3;
			}
		}
		else 
		{
			//We failed to read as there were no bytes available
			return 0;
		}
	}
	else 
	{
		//We have failed to peek. The pipe is probably broken
		close();
		return -5;
	}
}

extern "C" DISCORDRPCNATIVE_API bool writeFrame(unsigned char* buffer, int length)
{
	/*
	Write(handle, buffer, 0, length);
	*/
	if (length == 0) 
	{
		return true;
	}

	if (!isConnected()) 
	{
		return false;
	}

	//Prepare the size
	const DWORD bytesLength = (DWORD)length;
	DWORD bytesWritten = 0;

	//Write and return its success
	bool success = ::WriteFile(pipe, buffer, bytesLength, &bytesWritten, nullptr);
	return success && bytesWritten == bytesLength;
}

extern "C" DISCORDRPCNATIVE_API void close()
{
	/*
	Close(handle);
	*/
	::CloseHandle(pipe);
	pipe = INVALID_HANDLE_VALUE;
	isOpen = false;
}


extern "C" DISCORDRPCNATIVE_API unsigned int open(char* pipename) 
{
	//Creates a connection to the pipe
	pipe = ::CreateFileA(pipename, GENERIC_READ | GENERIC_WRITE, 0, nullptr, OPEN_EXISTING, 0, nullptr);
	if (pipe != INVALID_HANDLE_VALUE) 
	{
		isOpen = true;
		return 0;
	}

	//We have a error, better find out why.
	auto lasterr = GetLastError();
	
	//Pipe wasnt found
	if (lasterr == ERROR_FILE_NOT_FOUND)
		return lasterr;

	//The path wasnt found?
	if (lasterr == ERROR_PATH_NOT_FOUND)
		return lasterr;

	//Pipe is just busy
	if (lasterr == ERROR_PIPE_BUSY) 
	{
		if (!WaitNamedPipeA(pipename, 1000))
		{
			//Failed to open, terminate
			return lasterr;
		}
		else 
		{
			//Attempt to open it again
			return open(pipename);
		}
	}

	//Some other magically error occured
	return lasterr;
}

// This is the constructor of a class that has been exported.
// see DiscordRPC.Native.h for the class definition
CDiscordRPCNative::CDiscordRPCNative()
{
    return;
}
