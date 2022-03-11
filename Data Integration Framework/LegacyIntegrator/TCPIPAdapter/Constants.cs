internal static class Constants
{

    internal enum ConnectionMode
    {
        Standard,
        Enhanced
    }


    /// <summary>
    /// Should be used to indicate true in configuration files
    /// </summary>
    public const string TrueIndicator = "ON";

    /// <summary>
    /// Should be used to indicate false in configuration files
    /// </summary>
    public const string FalseIndicator = "OFF";


    public const string MessageSeperator = ",";


    public const string UserIdTls = "MainframeUserId";
    public const string PasswordTls = "MainframePassword";

    public const int Ebcdic_Code_Page_Number = 500;



    //internal static class ConfigurationElements
    //{
    //    /// <summary>
    //    /// Possible values Standard/Enhanced
    //    /// Depends on what server is executing on the mainframe
    //    /// </summary>
    //    public const string Mode = "Mode";

    //    /// <summary>
    //    /// Connection pooling if any to be used to communicate with the m/f
    //    /// </summary>
    //    public const string Connection = "Connection";

    //    /// <summary>
    //    /// 4 characters indicating the transaction id to be executed for the concurrent server.
    //    /// Can be null if Mode is Enhanced 
    //    /// </summary>
    //    public const string TransactionID = "TransactionID";

    //    /// <summary>
    //    /// The cobol program id which will handle the requests
    //    /// </summary>
    //    public const string HostProgID = "HostProgID";

    //    /// <summary>
    //    /// Possible values ON/OFF
    //    /// </summary>
    //    public const string EnableTrace = "EnableTrace";

    //    /// <summary>
    //    /// Possible values ON/OFF
    //    /// </summary>
    //    public const string EnablePerformanceCounters = "EnablePerformanceCounters";

    //    /// <summary>
    //    /// IP Address of the mainframe to connect to
    //    /// </summary>
    //    public const string IPAddress = "IPAddress";

    //    /// <summary>
    //    /// The port number on which the mainframe is listening.
    //    /// </summary>
    //    public const string PortNumber = "Port";

    //}

}