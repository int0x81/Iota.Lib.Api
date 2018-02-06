using Iota.Lib.Model;
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
        Bundle Execute(Bundle transfer, string branchTip, string trunkTip, int minWeightMagnitutde = Constants.MIN_WEIGHT_MAGNITUDE);
    }
}
