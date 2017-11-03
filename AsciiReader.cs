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

using System.IO;

namespace CardFileRdr {

	/// <summary>
	/// A Char Reader for Ascii encodings
	/// </summary>
	class AsciiReader : CharReader {

		private BinaryReader br;

		public AsciiReader(BinaryReader ibr) {
			br = ibr;
		}


		/// <summary>
		/// Read one char
		/// </summary>
		/// <returns>The char</returns>
		public char ReadChar() {
			return br.ReadChar();
		}


		/// <summary>
		/// Read some chars
		/// </summary>
		/// <param name="num">The number of chars to read</param>
		/// <returns>The chars</returns>
		public char[] ReadChars(int num) {
			char[] ca = new char[num];
			ca = br.ReadChars(num);
			return ca;
		}
	}
}
