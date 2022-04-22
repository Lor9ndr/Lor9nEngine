using Lor9nEngine;

AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

using Application app = new Application();


void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
{
    var e = (Exception)args.ExceptionObject;
    throw e;
}