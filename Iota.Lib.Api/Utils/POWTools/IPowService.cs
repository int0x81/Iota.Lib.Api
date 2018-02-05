using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Iota.Lib.Utils
{
    /// <summary>
    /// Defines all functions a proof of work service has to implement
    /// </summary>
    interface IPowService
    {
        string Execute(string rawTransaction, int minWeightMagnitutde);
    }
}
