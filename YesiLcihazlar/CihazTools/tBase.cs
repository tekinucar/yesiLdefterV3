using System;

public class tBase : IDisposable
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


}
