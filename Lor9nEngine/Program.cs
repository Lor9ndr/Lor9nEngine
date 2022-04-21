using Lor9nEngine;

AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

Application app  = new Application();
app.Run();


void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
{
    var e = (Exception)args.ExceptionObject;
    throw e;
}