public class Server : IDisposable
{
    private readonly Action<HttpListenerContext> _requestHandler;
    private readonly HttpListener _listener;
    private readonly Thread _listenerThread;

    public Server(Action<HttpListenerContext> requestHandler, string host = "localhost", string sufix = "/", int port = 8080)
    {
        _requestHandler = requestHandler;
        _listener = new HttpListener();

        _listener.Start();

        // See https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistenerprefixcollection.add?redirectedfrom=MSDN&view=net-6.0#remarks
        // for info about the host value.
        Url = new UriBuilder("http", host, port, sufix).ToString();
        _listener.Prefixes.Add(Url);

        _listenerThread = new Thread(HandleHttpRequests);
        _listenerThread.Start();
    }

    public string Url { get; }

    public void Dispose()
    {
        _listener.Stop();
        _listener.Close();
        _listenerThread.Join();
    }

    private void HandleHttpRequests()
    {
        while (_listener.IsListening)
        {
            try
            {
                var ctx = _listener.GetContext();
                _requestHandler(ctx);
            }
            catch (HttpListenerException)
            {
                // listener was stopped,
                // ignore to let the loop end and the method return
            }
            catch (ObjectDisposedException)
            {
                // the response has been already disposed.
            }
            catch (InvalidOperationException)
            {
                // this can occur when setting Response.ContentLength64, with the framework claiming that the response has already been submitted
                // for now ignore, and we'll see if this introduces downstream issues
            }
            catch (Exception) when (!_listener.IsListening)
            {
                // we don't care about any exception when listener is stopped
            }
        }
    }
}