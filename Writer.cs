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
	/// A writer of data from a Cardile
	/// </summary>
	public interface Writer {

		/// <summary>
		/// Close and dispose of the writable object
		/// </summary>
		void close();

		/// <summary>
		/// Write a header, if appropriate
		/// </summary>
		void WriteHeader();

		/// <summary>
		/// Write a Card
		/// </summary>
		/// <param name="card">A Cardfile card</param>
		void WriteCard(Card card);

	}

}//nm
