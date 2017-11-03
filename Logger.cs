// CardFileRdr - Copyright © 2016 John Oliver
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Text;
using System.IO;

namespace CardFileRdr {

	/// <summary>
	/// A simple common logger
	/// </summary>
	public sealed class Logger : IDisposable {

		/// <summary>
		/// Write formats
		/// </summary>
		public enum Wrtformats {
			/// <summary>
			/// Output number in base 10
			/// </summary>
			NATURAL,
			/// <summary>
			/// Output number as Hex
			/// </summary>
			HEX
		};

		/// <summary>
		/// Newline char
		/// </summary>
		public static string NL {
			get {
				return lF;
			}
		}
		private static string lF = "\n";

		private static string logSeparator = "________________________________________________________________________________";
		private string logfileName = "CardFileRdr.log";
		private StreamWriter  logFile;


		/// <summary>
		/// Construct a logger with a given log file name but do not open the file yet.
		/// Thus it may be referenced in code, but not used.
		/// </summary>
		/// <param name="inLogfile">Name and path of the logfile</param>
		public Logger( string inLogfile ) {
			if (!(inLogfile == null)) {
				logfileName = inLogfile;
			}
		}//Logger


		/// <summary>
		/// Open the log for writing.
		/// </summary>
		public void open() {
			logFile = new StreamWriter( logfileName, true, Encoding.UTF8 ); // true to append
			logFile.WriteLine( logSeparator );
			logFile.WriteLine( "Starting at: " + DateTime.Now.ToString() );
			logFile.Flush();
		}


		/// <summary>
		/// Explicitly close the log
		/// </summary>
		public void close() {
			if (logFile == null)
				return;
			logFile.Flush();
			logFile.Close();
			logFile = null;
		}

		/// <summary>
		/// Dispose of the object (override of IDisposable)
		/// </summary>
		public void Dispose() {
			close();
		}

		/// <summary>
		/// Write a new line to the log
		/// </summary>
		public void writeLn() {
			if (logFile == null)
				return;
			logFile.WriteLine();
			logFile.Flush();
		}


		/// <summary>
		/// Write a text line to the log
		/// </summary>
		/// <param name="logmsg">The text</param>
		public void writeLn( string logmsg ) {
			if (logFile == null)
				return;
			logFile.WriteLine( logmsg );
			logFile.Flush();
		}

		/// <summary>
		/// Write a text line to the log
		/// </summary>
		/// <param name="a">The char array</param>
		public void writeLn( char[] a ) {
			if (logFile == null)
				return;
			logFile.Write( a );
			writeLn();
			logFile.Flush();
		}


		/// <summary>
		/// Write text to the log
		/// </summary>
		/// <param name="logmsg">The text</param>
		public void write( string logmsg ) {
			if (logFile == null)
				return;
			logFile.Write( logmsg );
			logFile.Flush();
		}


		/// <summary>
		/// Write a char to the log
		/// </summary>
		/// <param name="c">The text</param>
		public void write( char c ) {
			if (logFile == null)
				return;
			logFile.Write( c );
			logFile.Flush();
		}


		/// <summary>
		/// Write a char array to the log
		/// </summary>
		/// <param name="a">The char array</param>
		public void write( char[] a ) {
			if (logFile == null)
				return;
			for (int i = 0; i < a.Length; i++) {
				logFile.Write( a[i] );
			}
			logFile.Flush();
		}


		/// <summary>
		/// Write a byte as hex to the log
		/// </summary>
		/// <param name="b">The byte</param>
		/// <param name="opf">Write format</param>
		public void write( byte b, Wrtformats opf ) {
			if (logFile == null)
				return;
			if (opf == Wrtformats.HEX) {
				logFile.Write( b.ToString( "X" ) );
			} else {
				logFile.Write( b );
			}
			logFile.Flush();
		}


		/// <summary>
		/// write a byte array to the log as hex
		/// </summary>
		/// <param name="ba">The byte array</param>
		/// <param name="opf">Write format</param>
		public void write( byte[] ba, Wrtformats opf ) {
			if (logFile == null)
				return;
			if (opf == Wrtformats.HEX) {
				string str = BitConverter.ToString( ba ).Replace( "-", " " );
				logFile.Write( str );
			} else {
				logFile.Write( ba );
			}
			logFile.Flush();
		}


		/// <summary>
		/// Write a UInt32 to log
		/// </summary>
		/// <param name="ui">The data</param>
		/// <param name="opf">Write format</param>
		public void write( UInt32 ui, Wrtformats opf ) {
			if (logFile == null)
				return;
			int len = 0;
			if (opf == Wrtformats.HEX) {
				string s = ui.ToString( "X" );
				logFile.Write( s );
				len = s.Length;
			} else {
				logFile.Write( ui );
				len = ui.ToString( "X" ).Length;
			}
			logFile.Flush();
		}


		/// <summary>
		/// Reset the byte count used by write(byte) (and write a newline)
		/// </summary>
		public void resetByteCount() {
			if (logFile == null)
				return;
			logFile.Write( NL );
			logFile.Flush();
		}

	}//class

}//namespace
