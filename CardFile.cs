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
using System.IO;

namespace CardFileRdr {

	/// <summary>
	/// This class reads a CardFile and exports it using the supplied Writer object.
	/// </summary>
	public class CardFile {

		private bool doTrace; // logging?

		// Logging is a security risk if the Cardfile contains sensitive information
		// so it should only be enabled for debugging with test data.
		private static Logger log; // The logger

		/// <summary>
		/// Common logger
		/// </summary>
		public static Logger Log {
			get {
				return log;
			}
		}


		// The writer of the card file data that was read
		private Writer wrtr;


		/// <summary>
		/// Construct a Cardfile object
		/// </summary>
		/// <param name="pDoTrace">Set logging on?</param>
		/// <param name="logFileName">Name of log file when logging used (or empty string when not)</param>
		/// <param name="wrtr">The object performing the writing of the data read</param>
		public CardFile( bool pDoTrace, string logFileName, Writer wrtr ) {
			if (logFileName == null) {
				throw new ArgumentNullException( "CardFile(): null logFileName received" );
			}
			if (wrtr == null) {
				throw new ArgumentNullException( "CardFile(): null wrtr received" );
			}

			log = new Logger( String.Empty ); // Always need a valid log object even if not logging
			doTrace = pDoTrace;
			if (doTrace) {
				log = new Logger( logFileName );
				log.open();
			}
			this.wrtr = wrtr;
		}


		/// <summary>
		/// Process the CardFile - read it and write it out
		/// </summary>
		/// <param name="inStrm">The card file to be read</param>
		public void process( Stream inStrm ) {
			try {

				FileProcessor fp = new FileProcessor( doTrace, wrtr );
				fp.readBinFile( inStrm );

			} catch (Exception ex) {
				Log.writeLn( ex.ToString() );
				throw;
			} finally {
				Log.close();
			}//try

		}//process

	}//class
}//namespace
