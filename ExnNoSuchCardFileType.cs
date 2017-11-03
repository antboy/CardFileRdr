﻿// CardFileRdr - Copyright © 2016 John Oliver
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
	/// Type of card file read from file is not recognised
	/// </summary>
	[Serializable]
	class ExnNoSuchCardFileType : ExnCardFileRdr {

		public ExnNoSuchCardFileType() : base() { }

		public ExnNoSuchCardFileType( string msg ) : base( msg ) { }

		public ExnNoSuchCardFileType( string msg, Exception ex ) : base( msg, ex ) { }

		protected ExnNoSuchCardFileType( SerializationInfo info, StreamingContext context ) : base( info, context ) { }
	}
}
