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
using System.Runtime.Serialization;

namespace CardFileRdr {

	/// <summary>
	/// Exception arising from CardFileRdr - Generic base class for this app's exceptions
	/// </summary>
	[Serializable]
	public class ExnCardFileRdr : Exception {

		/// <summary>
		/// Exception with default message
		/// </summary>
		public ExnCardFileRdr() : base() { }

		/// <summary>
		/// Exception with parameterised message
		/// </summary>
		/// <param name="msg">Text</param>
		public ExnCardFileRdr( string msg ) : base( msg ) { }

		/// <summary>
		/// Chained exception with parameterised message and exception.
		/// </summary>
		/// <param name="msg">Text</param>
		/// <param name="ex">Chained exception</param>
		public ExnCardFileRdr( string msg, Exception ex ) : base( msg, ex ) { }

		/// <summary>
		/// Make exception serializable
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ExnCardFileRdr( SerializationInfo info, StreamingContext context ) : base(info, context) { }

	}//c

}//n
