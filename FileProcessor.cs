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
	/// A file reader
	/// </summary>
	class FileProcessor {
		private bool doTrace = false; // output debug info?
		private Writer wrtr; // The object to which to write the cardfile data


		/// <summary>
		/// Construct a FileProcessor
		/// </summary>
		/// <param name="doTrace">Debug trace on?</param>
		/// <param name="wrtr">The object which exports cardile data</param>
		public FileProcessor( bool doTrace, Writer wrtr ) {
			this.doTrace = doTrace;
			this.wrtr = wrtr;
		}


		/// <summary>
		/// The format of the CardFile as defined my Microsoft.
		///     MGC may have graphic objects inside file but not OLE
		///     RRG may have OLE objects
		///     DKO is in Unicode format
		/// </summary>
		private enum CFformats { MGC, RRG, DKO };

		private CharReader chrd; // Char reader for a given encoding


		/// <summary>
		/// Read a binary file - invoked from CardFile class
		/// </summary>
		/// <param name="inStrm">The input stream to be read</param>
		public void readBinFile( Stream inStrm ) {
			using (BinaryReader br = new BinaryReader( inStrm, Encoding.ASCII )) {
				chrd = new AsciiReader( br );
				readIndexes( br );

			}//using
		}//readBinFile


		private void readIndexes( BinaryReader br ) {
			// File header fields
			CFformats fmt;
			char[] rchars = br.ReadChars( 3 ); // get cardfile format chars e.g. MGC // always ASCII encoded even in 'Unicode' file
			string st1 = new string( rchars );
			try {
				fmt = (CFformats)Enum.Parse( typeof( CFformats ), st1, true ); // true means case-sensitive
			} catch (ArgumentException) {
				string msg = string.Format( "Unknown card file type: {0}: Invalid CardFile, expect MGC, RRG or DKO", st1 );
				CardFile.Log.writeLn( msg );
				throw new ExnNoSuchCardFileType( msg );
			}

			CardFile.Log.write( "Cardfile format: " + fmt.ToString() + ", " );

			UInt16 numCards; // count of cards in card file, from file header
			UInt32 lastObjID; // ID of last object in file

			switch (fmt) // use the appropriate code for the file header
			{
				case CFformats.MGC:
					numCards = br.ReadUInt16(); // count of cards in file
					CardFile.Log.writeLn( "Num Cards: " + numCards.ToString() );
					break;

				case CFformats.RRG:
					lastObjID = br.ReadUInt32(); // last obj
					debug( "Last Obj ID: " + lastObjID.ToString() );

					numCards = br.ReadUInt16(); // count of cards in file
					CardFile.Log.writeLn( "Num Cards: " + numCards.ToString() );
					break;

				case CFformats.DKO:
					chrd = new UCS2Reader( br ); // switch to UCS-2 encoded chars

					lastObjID = br.ReadUInt32(); // last obj
					debug( "Last Obj ID: " + lastObjID.ToString() );

					numCards = br.ReadUInt16(); // count of cards in file
					CardFile.Log.writeLn( "Num Cards: " + numCards.ToString() );
					break;

				default:
					string msg = "Warn: readBinFile: unknown card file type: " + fmt.ToString();
					CardFile.Log.writeLn( msg );
					throw new ExnNoSuchCardFileType( msg );

#pragma warning disable CS0162 // Unreachable code detected
					break;
#pragma warning restore CS0162 // Unreachable code detected
			}//switch

			// Index entries - each is 52 bytes long
			// One index entry per card, in following byte format
			//  0 -  5 : null bytes (reserved for future MS use)
			//  6 -  9 : absolute position of card data in file
			//      10 : Flag (00)
			// 11 - 50 : Index title text content, terminated by a null byte. When Unicode -> 2n+1
			//      51 : Last byte, null - for when index text line is full hence no null terminator there

			const int nullBytes = 6;
			const int partIdxLen = 12; // length of index excluding title text
			int idxTxtCharLen = 40; // max single-byte chars of index title text
			int idxTxtByteLen = 40; // max bytes of index title text

			if (fmt == CFformats.DKO) {
				idxTxtByteLen *= 2;
				idxTxtByteLen++;
			}
			int idxLen = idxTxtByteLen + partIdxLen; // index byte length

			char[] rdata = new char[idxTxtCharLen + 1]; // big enough to hold index line text plus null terminator
			byte[] bdata = new byte[nullBytes]; // check for expected null bytes

			Card card = new Card();

			// For each index entry
			for (int i = 0; i < numCards; i++) {
				long startPos = br.BaseStream.Position; // save index start pos'n
				debugAddr( br, "\nIndex start pos'n" );

				bdata = br.ReadBytes( nullBytes ); // null bytes - check they are null for valid cardfile
				foreach (byte b in bdata) {
					if (b != 0) {
						throw new ExnInvalidCardfile( "Expected null bytes not found - Invalid Cardfile" );
					}
				}

				long dataPos = br.ReadInt32(); // pos'n of card data in file

				debug( string.Format( "Card Data Pos'n: x{0:X4} ", dataPos ) );

				br.ReadByte(); // flag byte - discard

				CardFile.Log.write( "\nCard index title: " );

				int cIdx = 0; // count chars in index text
				char ic = chrd.ReadChar(); // 1st char of text content of index line
				while (!(ic == 0)) {
					if (cIdx > (idxTxtCharLen)) {
						CardFile.Log.writeLn( string.Format( "\nreadBinFile: index has more than {0} chars - not a proper card file?", idxTxtCharLen ) );
					}
					//CardFile.Log.write(ic);
					rdata[cIdx++] = ic;
					ic = chrd.ReadChar(); // 1st char of text content of index line
				}
				//CardFile.Log.writeLn();
				string iStr = "";
				if (cIdx > 0) {
					iStr = new string( rdata, 0, cIdx ); // last arg is length

				}
				CardFile.Log.writeLn( iStr ); // write index title
				card.Title = iStr;
				;

				debugLn( String.Format( "\nIndex bytes read: {0:D}", br.BaseStream.Position - startPos ) );

				// Now the card data
				br.BaseStream.Position = dataPos; // set file pos'n for card data from index
				debugAddr( br, "\nFile position" );

				// Get the card data
				switch (fmt) {
					case CFformats.MGC:
						card.Data = mgcData( br );
						break;
					case CFformats.RRG:
						card.Data = rrgData( br );
						break;
					case CFformats.DKO:
						card.Data = dkoData( br );
						break;
				}//switch

				wrtr.WriteCard( card ); // write card data
				card.init();

				br.BaseStream.Position = startPos + idxLen; // set to start of next index pos'n

			}//for index entries

			wrtr.close(); // close writer

		}//readIndexes


		// Get card data for MGC format file
		private String mgcData( BinaryReader br ) {
			// Read the file but for text only, ignore Graphics

			// format when either Graphics (G), Text (T) or both G&T
			// Byte numbers
			// G & T        T       G
			// 0 : 1        0:1     0 : 1       Length of graphic bitmap (lob)
			// 2 : 3                2 : 3       Width
			// 4 : 5                4 : 5       Height
			// 6 : 7                6 : 7       X-co-ord
			// 8 : 9                8 : 9       Y-co-ord
			// 10 : eob             10 : eob    Bitmap Graphic
			// eob+1 :
			//     +2       2:3     eob+1 :
			//                          +2      Length of text entry (lot)
			// eob +3 :
			//    end       4:lot   eob +3 :
			//                           end    Text data

			Int16 bitmapLen = br.ReadInt16(); // 1st two bytes of card data
			Int16 num = br.ReadInt16();   // 2nd two bytes - either text length (T) or bitmap width (G & GT)

			String txtData = String.Empty;

			if (bitmapLen + num == 0) {
				CardFile.Log.writeLn( "Card with no data" );
			} else if (bitmapLen == 0 && num > 0) { // Text only card

				debugLn( "Text only card" );
				Int16 txtLen = num;

				txtData = new string( chrd.ReadChars( txtLen ), 0, txtLen ); // last arg is length

				debugLn( "Card Text Length: " + txtLen );
				CardFile.Log.writeLn( "Card Data:" );
				CardFile.Log.writeLn( txtData );
				debugLn( "End of Text only card\n" );
			} else { // Graphic data, maybe text too

				CardFile.Log.writeLn( "Has Graphic" );
				debugLn( "Bitmap length: " + bitmapLen );

				Int16 bitmapWidth = num;
				Int16 bitmapHeight = br.ReadInt16();
				Int16 Xoffset = br.ReadInt16();
				Int16 Yoffset = br.ReadInt16();
				br.ReadBytes( bitmapLen ); // read but discard bitmap, to advance file ptr
				Int16 txtLen = br.ReadInt16(); // text length field

				//char[] txtData = chrd.ReadChars(txtLen);
				txtData = new string( chrd.ReadChars( txtLen ), 0, txtLen ); // last arg is length

				CardFile.Log.writeLn( "Card data: " );
				CardFile.Log.writeLn( txtData );
				debugLn( "End of Text only card\n" );

			}//if

			return txtData;

		}//mgcData


		// RRG data
		private String rrgData( BinaryReader br ) {
			// format when either Graphics (G), Text (T) or both G&T
			// Byte numbers
			// G & T        T       G
			// 0 : 1        0:1     0 : 1       object flag, null if none
			// 2 : 5                2 : 5       unique object ID
			// 6 : x                6 : x       OLE object
			// x+1:x+2              x+1:x+2     DIB char width
			// x+3:x+4              x+3:x+4     DIB char height
			// x+5:x+6              x+5:x+6     X-coord U/L
			// x+7:x+8              x+7:x+8     Y-coord U/L
			// x+9:x+10             x+9:x+10    X-coord B/R
			// x+11:x+12            x+11:x+12   Y-coord B/R
			// x+13:x+14            x+13:x+14   embedded=0,linked=1,static=2
			// x+15:x+16    2:3     x+15:x+16   text length t
			// x+17:x+17+t-1 4:4+t-1 x+17:x+17+t-1  text data

			UInt16 hasOLE = br.ReadUInt16();

			if (hasOLE == 1) {
				CardFile.Log.writeLn( "Has an OLE" );

				OLE ole = new OLE( br, chrd, doTrace );
				ole.parseOLE();

				debugLn( string.Format( "\nBytes in file: x{0,8:X}", br.BaseStream.Length ) );
				debugAddr( br, "Byte number after OLE" );

				if (br.BaseStream.Position > br.BaseStream.Length - 1) {
					throw new ExnCardFileRdr( "Internal OLE Error: Will attempt to read beyond end of file" );
				}
				br.ReadBytes( 14 );//bytes after OLE, before text len
			}

			UInt16 txtLen = br.ReadUInt16();

			string txtData = new string( chrd.ReadChars( txtLen ), 0, txtLen ); // last arg is length

			CardFile.Log.writeLn( "Card data: " );
			CardFile.Log.writeLn( txtData );

			return txtData;

		}//m()


		//dko data
		private String dkoData( BinaryReader br ) {
			return rrgData( br ); // same as rrg
		}


		// debug tracing
		private void debug( string str ) {
			if (doTrace) {
				CardFile.Log.write( str );
			}
		}//m()


		// debug tracing
		private void debugLn( string str ) {
			if (doTrace) {
				CardFile.Log.writeLn( str );
			}
		}//m()


		// Hex address of current file position
		// string is text to say what the address is of
		private void debugAddr( BinaryReader br, string str ) {
			if (doTrace) {
				CardFile.Log.writeLn( string.Format( "{0}: x{1,8:X}", str, br.BaseStream.Position ) );
			}
		}//m()

	}//class
}//namespace
