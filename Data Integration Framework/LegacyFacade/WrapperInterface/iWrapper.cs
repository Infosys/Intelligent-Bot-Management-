/****************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file contains the IWrapper interface to be used by custom wrappers.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Infosys.Lif.LegacyParameters;

namespace Infosys.Lif.WrapperInterface
{
	/// <summary>
	/// All Wrapper classes need to implement from iWrapper.
	/// </summary>
	public interface IWrapper
	{
		/// <summary>
		/// This is the Extract Method to retrieve the individual responses
		/// from the response string
		/// </summary>
		/// <param name="hostResponse">The response string</param>
		/// <returns>String array containing responses</returns>
		string[] Extract(string hostResponse);

		/// <summary>
		/// This method creates a request from individual requests
		/// appending header and footer information
		/// </summary>
		/// <param name="inputRequest">string array containing individual request strings</param>
		/// <param name="UserName">userName to be added in Header information</param>
		/// <param name="requestParameters">Parameters collection containing Wrapper information</param>
		/// <returns></returns>
		string CreateRequest(string[] inputRequest, RequestParameters requestParameters);
	}

}
