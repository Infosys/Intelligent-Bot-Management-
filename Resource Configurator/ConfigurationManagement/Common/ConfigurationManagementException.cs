/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.ConfigurationManager.Infrastructure.Common
{
   public class ConfigurationManagementException : System.Exception, ISerializable
    {
        public ConfigurationManagementException()
            : base()
        {

    }
    public ConfigurationManagementException(string message)
            : base(message)
        {

    }
    public ConfigurationManagementException(string message, Exception inner)
            : base(message, inner)
        {

    }
    protected ConfigurationManagementException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        // Add implementation.
    }
}

[Serializable]
public class ConfigurationManagementCriticalException : System.Exception, ISerializable
{
    public ConfigurationManagementCriticalException() : base()
    {

    }
    public ConfigurationManagementCriticalException(string message) : base(message)
    {

    }
    public ConfigurationManagementCriticalException(string message, Exception inner) : base(message, inner)
    {

    }
    protected ConfigurationManagementCriticalException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // Add implementation.
    }
}

[Serializable]
public class ConfigurationManagementDataItemNotFoundException : System.Exception, ISerializable
{
    public ConfigurationManagementDataItemNotFoundException() : base()
    {

    }
    public ConfigurationManagementDataItemNotFoundException(string message) : base(message)
    {

    }
    public ConfigurationManagementDataItemNotFoundException(string message, Exception inner) : base(message, inner)
    {

    }
    protected ConfigurationManagementDataItemNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // Add implementation.
    }

}

/// <summary>
/// The exception to be rasied in case validation fails for input request data
/// </summary>
[Serializable]
public class ConfigurationManagementValidationException : System.Exception, ISerializable
{
    public ConfigurationManagementValidationException()
        : base()
    {

    }
    public ConfigurationManagementValidationException(string message)
        : base(message)
    {

    }
    public ConfigurationManagementValidationException(string message, Exception inner)
        : base(message, inner)
    {

    }
    protected ConfigurationManagementValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // Add implementation.
    }

}

}

