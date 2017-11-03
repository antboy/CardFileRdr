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
	/// A char reader of UCS-2 (like UTF-16 but always 2 bytes) encodings
	/// </summary>
	class UCS2Reader : CharReader {

		private BinaryReader br;

		// Data for ReadChar (1)
		private byte[] bytes2;
		private byte BSIZE = 2; // bytes per char
		private byte CSIZE = 1; // char 1
		private char[] ca1; // array of 1 char


		public UCS2Reader(BinaryReader ibr) {
			br = ibr;
		}


		/// <summary>
		/// Read one char
		/// </summary>
		/// <returns>The char</returns>
		public char ReadChar() {
			if (bytes2 == null) {
				bytes2 = new byte[BSIZE];
				ca1 = new char[CSIZE];
			}
			bytes2 = br.ReadBytes(BSIZE);
			ca1 = Encoding.Unicode.GetChars(bytes2);
			return ca1[CSIZE-1];
		}


		/// <summary>
		/// Read some chars
		/// </summary>
		/// <param name="num">The number of chars to read.</param>
		/// <returns>The chars in an array.</returns>
		public char[] ReadChars(int num) {

			byte[]  mbytes = new byte[num * 2];//UCS-2
			mbytes = br.ReadBytes(num * 2);//UCS-2

			char[]  mchars = new char[num];
			mchars = Encoding.Unicode.GetChars(mbytes);
			return mchars;
		}

	}//class
}//nm
