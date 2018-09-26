// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the DISCORDRPCNATIVE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// DISCORDRPCNATIVE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef DISCORDRPCNATIVE_EXPORTS
#define DISCORDRPCNATIVE_API __declspec(dllexport)
#else
#define DISCORDRPCNATIVE_API __declspec(dllimport)
#endif

// This class is exported from the DiscordRPC.Native.dll
class DISCORDRPCNATIVE_API CDiscordRPCNative {
public:
	CDiscordRPCNative(void);
};


//Example Variable
extern "C" DISCORDRPCNATIVE_API bool isConnected(void);
extern "C" DISCORDRPCNATIVE_API int readFrame(unsigned char* buffer, int length);
extern "C" DISCORDRPCNATIVE_API bool writeFrame(unsigned char* buffer, int length);
extern "C" DISCORDRPCNATIVE_API unsigned int open(char* pipename);
extern "C" DISCORDRPCNATIVE_API void close(void);
