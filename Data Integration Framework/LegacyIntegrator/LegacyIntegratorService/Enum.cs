/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyIntegratorService
{
	/// <summary>
	/// Communication type 
	/// </summary>
	public enum CommunicationType
	{
		//Synchronous 
		Sync,
		//Asynchronous
		Async
	}

	/// <summary>
	/// Represent type of connection model used.
	/// </summary>
	public enum ConnectionModelType
	{
		// Connection pooling
		ConnectionPool,
		// No connection pooling
		None
	}

	/// <summary>
	/// Represent persistent property for a message
	/// </summary>
	public enum MessagePersistence
	{
		// Persistent 
		Persistent,
		// Non persistent 
		NonPersistent,
		// as per queue definition
		Default
	}
}
