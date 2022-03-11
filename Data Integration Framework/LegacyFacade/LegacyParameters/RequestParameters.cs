/****************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file contains a Name Value Pair to be used by serializers and wrappers.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace Infosys.Lif.LegacyParameters
{

	/// <summary>
	/// 
	/// </summary>
	public class RequestParameters 
	{
		private NameValueCollection requestCollection;


		public RequestParameters()
		{
			RequestCollection = new NameValueCollection(); 
		}

		public NameValueCollection RequestCollection
		{
			get
			{
				return requestCollection;
			}
			set
			{
				requestCollection = value;
			}
		}



		
}
}
