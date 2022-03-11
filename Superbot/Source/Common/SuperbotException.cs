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

namespace Infosys.Solutions.Superbot.Infrastructure.Common
{
   public class SuperbotException : System.Exception, ISerializable
    {
        public SuperbotException()
            : base()
        {

    }
    public SuperbotException(string message)
            : base(message)
        {

    }
    public SuperbotException(string message, Exception inner)
            : base(message, inner)
        {

    }
    protected SuperbotException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        // Add implementation.
    }
}

[Serializable]
public class SuperbotCriticalException : System.Exception, ISerializable
{
    public SuperbotCriticalException() : base()
    {

    }
    public SuperbotCriticalException(string message) : base(message)
    {

    }
    public SuperbotCriticalException(string message, Exception inner) : base(message, inner)
    {

    }
    protected SuperbotCriticalException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // Add implementation.
    }
}

[Serializable]
public class SuperbotDataItemNotFoundException : System.Exception, ISerializable
{
    public SuperbotDataItemNotFoundException() : base()
    {

    }
    public SuperbotDataItemNotFoundException(string message) : base(message)
    {

    }
    public SuperbotDataItemNotFoundException(string message, Exception inner) : base(message, inner)
    {

    }
    protected SuperbotDataItemNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // Add implementation.
    }

}

/// <summary>
/// The exception to be rasied in case validation fails for input request data
/// </summary>
[Serializable]
public class SuperbotValidationException : System.Exception, ISerializable
{
    public SuperbotValidationException()
        : base()
    {

    }
    public SuperbotValidationException(string message)
        : base(message)
    {

    }
    public SuperbotValidationException(string message, Exception inner)
        : base(message, inner)
    {

    }
    protected SuperbotValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // Add implementation.
    }

}

}

