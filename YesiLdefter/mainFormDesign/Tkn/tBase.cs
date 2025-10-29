using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class tBase : IDisposable
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


}
