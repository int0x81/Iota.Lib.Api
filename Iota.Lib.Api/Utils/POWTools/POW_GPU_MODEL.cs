using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iota.Lib.Utils
{
    class POW_GPU_Model : IPoWComputer
    {
        public static bool IsSystemSupported()
        {
            //GPU based proof-of-work not implemented yet
            return false;
        }

        bool IPoWComputer.IsSystemSupported()
        {
            return IsSystemSupported();
        }

        public string Search(string rawTransaction, int threadsAvail, int minWeightMagnitutde)
        {
            throw new NotImplementedException("GPU based proof-of-work not implemented yet");
        }
    }
}