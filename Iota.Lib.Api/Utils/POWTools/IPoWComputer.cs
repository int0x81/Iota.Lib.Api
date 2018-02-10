using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iota.Lib.Utils
{
    interface IPoWComputer
    {
        bool IsSystemSupported();
        string Search(string rawTransaction, int threadsAvail, int minWeightMagnitutde);
    }
}