using Iota.Lib.Api.Utils;
using System.Collections.Generic;

namespace Iota.Lib.Api.Core
{

    /// <summary>
    /// Represents the core API request 'AttachToTangle'.
    /// It is used to attach trytes to the tangle.
    /// </summary>
    public class AttachToTangleRequest : IotaRequest
    {
        int _minWeightMagnitude = Constants.MIN_WEIGHT_MAGNITUDE;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachToTangleRequest"/> class.
        /// </summary>
        /// <param name="trunkTransaction">The trunk transaction.</param>
        /// <param name="branchTransaction">The branch transaction.</param>
        /// <param name="trytes">The trytes.</param>
        /// <param name="minWeightMagnitude">The minimum weight magnitude.</param>
        public AttachToTangleRequest(string trunkTransaction, string branchTransaction, List<string> trytes, int minWeightMagnitude = Constants.MIN_WEIGHT_MAGNITUDE) : base(Core.Command.AttachToTangle)
        {
            TrunkTransaction = trunkTransaction;
            BranchTransaction = branchTransaction;
            Trytes = trytes;
            MinWeightMagnitude = minWeightMagnitude;

            if (Trytes == null)
                Trytes = new List<string>();
        }

        /// <summary>
        /// Proof of Work intensity.
        /// </summary>
        public int MinWeightMagnitude
        {
            get { return _minWeightMagnitude; }
            set
            {
                if (value > Constants.MIN_WEIGHT_MAGNITUDE)
                {
                    _minWeightMagnitude = value;
                }    
            }
        }

        /// <summary>
        /// Trunk transaction to approve.
        /// </summary>
        public string TrunkTransaction { get; set; }

        /// <summary>
        /// Branch transaction to approve.
        /// </summary>
        public string BranchTransaction { get; set; }

        /// <summary>
        /// List of trytes (raw transaction data) to attach to the tangle.
        /// </summary>
        public List<string> Trytes { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(MinWeightMagnitude)}: {MinWeightMagnitude}, {nameof(TrunkTransaction)}: {TrunkTransaction}, {nameof(BranchTransaction)}: {BranchTransaction}, {nameof(Trytes)}: {Trytes}";
        }
    }
}