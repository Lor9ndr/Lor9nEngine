using Lor9nEngine;

using NLog;

ILogger logger = LogManager.GetCurrentClassLogger();
AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
logger.Info("Application started...");
using Application app = new Application();
logger.Info("Application exited...");

void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
{
    var e = (Exception)args.ExceptionObject;
    logger.Fatal(e, "An unexpected exception has occured");
    throw e;
}