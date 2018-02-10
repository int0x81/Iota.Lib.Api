namespace Iota.Lib.Utils
{
    /// <summary>
    /// The Iota units 
    /// </summary>
    public enum IotaUnit
    {
        /// <summary>
        /// The corresponding value is in Iota. Same as 'None' (<see cref="None"/>)
        /// </summary>
        Iota = 0,

        /// <summary>
        ///  The corresponding value is in Iota. Same as 'Iota' (<see cref="Iota"/>)
        /// </summary>
        None = 0,

        /// <summary>
        /// 10^3
        /// </summary>
        Kilo = 3,

        /// <summary>
        /// 10^6
        /// </summary>
        Mega = 6,
        
        /// <summary>
        /// 10^9
        /// </summary>
        Giga = 9,

        /// <summary>
        /// 10^12
        /// </summary>
        Terra = 12,

        /// <summary>
        /// 10^15
        /// </summary>
        Peta = 15,
    }
}