using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.IO
{
	/// <summary>
	/// Pipe Client used to communicate with Discord.
	/// </summary>
	public interface INamedPipeClient : IDisposable
	{

		/// <summary>
		/// The logger for the Pipe client to use
		/// </summary>
		ILogger Logger { get; set; }

		/// <summary>
		/// Is the pipe client currently connected?
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// The pipe the client is currently connected too
		/// </summary>
		int ConnectedPipe { get; }

		/// <summary>
		/// Attempts to connect to the pipe. If 0-9 is passed to pipe, it should try to only connect to the specified pipe. If -1 is passed, the pipe will find the first available pipe.
		/// </summary>
		/// <param name="pipe">If -1 is passed, the pipe will find the first available pipe, otherwise it connects to the pipe that was supplied</param>
		/// <returns></returns>
		bool Connect(int pipe);
		
		/// <summary>
		/// Reads a frame if there is one available. Returns false if there is none. This should be non blocking (aka use a Peek first).
		/// </summary>
		/// <param name="frame">The frame that has been read. Will be <code>default(PipeFrame)</code> if it fails to read</param>
		/// <returns>Returns true if a frame has been read, otherwise false.</returns>
		bool ReadFrame(out PipeFrame frame);

		/// <summary>
		/// Writes the frame to the pipe. Returns false if any errors occur.
		/// </summary>
		/// <param name="frame">The frame to be written</param>
		bool WriteFrame(PipeFrame frame);
		
		/// <summary>
		/// Closes the connection
		/// </summary>
		void Close();
        
	}
}
