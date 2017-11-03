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

namespace CardFileRdr {

	/// <summary>
	/// A card from a cardfile
	/// </summary>
	public class Card {

		private string title = String.Empty;
		private string data = String.Empty;

		/// <summary>
		/// Card data
		/// </summary>
		public string Data {
			get {
				return data;
			}

			set {
				if (value == null) {
					data = string.Empty;
				} else {
					data = value;
				}
			}
		}


		/// <summary>
		/// The index title text
		/// </summary>
		public string Title {
			get {
				return title;
			}

			set {
				if (value == null) {
					title = string.Empty;
				} else {
					title = value;
				}
			}
		}

		/// <summary>
		/// Initialise the data
		/// </summary>
		public void init() {
			title = String.Empty;
			data = String.Empty;
		}

	}
}
