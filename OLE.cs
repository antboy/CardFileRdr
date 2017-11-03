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
	/// Represents an Object Linking and Embedding object.
	/// The size of the OLE object is not stored anywhere within the .CRD file.
	/// To find the end of the OLE object, it must be read from the file and parsed.
	///
	/// The OLE object's format description is documented in Appendix C of
	/// "Object Linking and Embedding Programmer's Reference" version 1.0, published by Microsoft Press,
	/// and also in the Microsoft Windows Software Development Kit (SDK) "Programmer's Reference, Volume 1: Overview,"
	/// Chapter 6, Object Storage Format.
	/// </summary>
	class OLE {
		// n.b.  The aged reference literature above should be read with assumption that their "long" is a 32-bit integer.

		private BinaryReader  br; // Reader for the file containing the OLE
		private CharReader  chrd; // chr reader for a given encoding
		private bool  isEcho = false;
		private string  nuls = "";


		/// <summary>
		/// Construct an OLE object
		/// </summary>
		/// <param name="br">Reader for the file containing the OLE</param>
		/// <param name="chrd">Chr reader of given encoding</param>
		/// <param name="doEcho">Echo bytes read to log?</param>
		public OLE( BinaryReader br, CharReader chrd, bool doEcho ) {
			this.br = br;
			this.chrd = chrd;
			isEcho = doEcho;
		}//ctor


		/// <summary>
		/// Navigates the OLE to read it to the end of it.  After, the file position will be at the next byte
		/// following the OLE.
		/// </summary>
		/// <returns>Position</returns>The BaseStream.Position of the last byte of the OLE object plus one
		public long parseOLE() {
			try {
				address( "Byte number at start of OLE obj ID" );

				read4bytesU( br, "Obj ID: ", true ); // Obj ID

				if (isVersion1()) {                                        // If OLE version is 1.0
					debugMsg( "OLE format: ", false );
					UInt32 Format = read4bytesU( br, nuls, false );        // 1==> Linked, 2==> Embedded, 3==> Static
					debugMsg( "", true );

					eatBytes( read4bytesU( br, nuls, false ) );            // Class String

					if (Format == 3) {                                     // Static object
						read4bytesU( br, nuls, false );                    // Width in mmhimetric.
						read4bytesU( br, nuls, false );                    // Height in mmhimetric.
						eatBytes( read4bytesU( br, nuls, false ) );        // Presentation data size and data itself.
					} else {                                               // Embedded or linked objects.
						eatBytes( read4bytesU( br, nuls, false ) );        // Topic string.
						eatBytes( read4bytesU( br, nuls, false ) );        // Item string.
						if (Format == 2) {                                 // Embedded object.
							eatBytes( read4bytesU( br, nuls, false ) );    // Native data and its size.
							SkipPresentationObj();                         // Read and eat the presentation object.
						} else {                                           // Linked object.
							eatBytes( read4bytesU( br, nuls, false ) );    // Network name.
							read4bytesU( br, nuls, false );                // Network type and net driver version.
							read4bytesU( br, nuls, false );                // Link update options.
							SkipPresentationObj();                         // Read and eat the presentation object.
						}//if format 2
					}//if format 3
				}//v1
			} catch (Exception e) {
				CardFile.Log.writeLn( "OLE error: " + e.ToString() );
			}
			return br.BaseStream.Position;
		}//m()


		private void SkipPresentationObj() {
			if (isEcho) {
				CardFile.Log.writeLn();
			}

			if (isVersion1()) {                                          // If the version is 1.0
				UInt32 formatId = read4bytesU( br, nuls, false );        // Format ID
				if (formatId == 0) { // no presentation object
					return;
				}
				if (getOLEClass()) {
					read4bytesU( br, nuls, false );                      // Width in mmhimetric.
					read4bytesU( br, nuls, false );                      // Height in mmhimetric.
					eatBytes( read4bytesU( br, nuls, false ) );          // Presentation data size and data itself
				} else {
					if (read4bytesU( br, nuls, false ) == 0) {           // if Clipboard format value is NULL
						eatBytes( read4bytesU( br, nuls, false ) );      // Read Clipboard format name.
					}
					eatBytes( read4bytesU( br, nuls, false ) );          // Presentation data size and data itself.
				}
			}//v1
		}//m()


		// read but discard bytes
		private void eatBytes( UInt32 cnt ) {
			byte[] ba = new byte[cnt];
			ba = br.ReadBytes( (int)cnt );
			if (isEcho) {
				CardFile.Log.write( string.Format( ", eatBytes({0}): ", cnt ) );
				CardFile.Log.write( ba, Logger.Wrtformats.HEX );
			}
		}//m()


		// OLE type
		private bool getOLEClass() {
			bool res = false;
			const int MAX_LEN_PIC_TYPE = 13; // max length of longest type string (METAFILEPICT) + nul terminator
			char[] charA = new char[MAX_LEN_PIC_TYPE];

			UInt32 clen = read4bytesU( br, "Class string: ", true ); // length of class string expected (incl nul terminator)
			if (clen > MAX_LEN_PIC_TYPE) {
				return res; // know it won't be an expected class
			}

			address( "Byte number at start of OLE Class string" );

			int i = 0;
			char c = chrd.ReadChar();
			while (!(c == 0)) {
				charA[i++] = c;
				c = chrd.ReadChar();
				if (i > MAX_LEN_PIC_TYPE - 1) {
					break;
				}
			}// while ends with i being length of string read

			string str = new string( charA, 0, i );// i is length
			if (isEcho) {
				debugMsg( string.Format( "OLE Class string: {0}", str ), true );
			}

			if (str.Equals( "METAFILEPICT" ) || str.Equals( "BITMAP" ) || str.Equals( "DIB" )) {
				debugMsg( "OLE Class string matched", true );
				res = true;
			}
			return res;
		}//m()


		// OLE version
		private bool isVersion1() {
			bool result = false;

			UInt32 oleVer = read4bytesU( br, "OLE version: ", true );

			// 1 for NT 3.51, 1.5 for Win 3.1
			if (oleVer == 0x0001 || oleVer == 0x0501) {
				result = true;
			} else {
				throw new ExnCardFileRdr( "OLE object found with version number not in (1, 1.5) so not known/handled" );
			}

			return result;
		}//m()


		// Read 4 unsigned bytes - an OLE 'long'
		private UInt32 read4bytesU( BinaryReader br, string prefix, bool doWriteLn ) {
			UInt32 ui = br.ReadUInt32();
			if (isEcho) {
				CardFile.Log.write( prefix );
				CardFile.Log.write( " Uint32: x" );
				CardFile.Log.write( ui, Logger.Wrtformats.HEX );
				if (doWriteLn) {
					CardFile.Log.writeLn();
				}
			}
			return ui;
		}


		// trace
		private void debugMsg( string str, bool doWriteLn ) {
			if (isEcho) {
				CardFile.Log.write( str );
				if (doWriteLn) {
					CardFile.Log.writeLn();
				}
			}
		}


		// Hex address of current file position
		// string is text to say what the address is of
		private void address( string str ) {
			if (isEcho) {
				CardFile.Log.writeLn( string.Format( "{0}: x{1,8:X}", str, br.BaseStream.Position ) );
			}
		}//m()

	}//OLE
}//nm
