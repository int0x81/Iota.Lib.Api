using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Iota.Lib.Test")]

namespace Iota.Lib.Api.Utils
{
    /// <summary>
    /// Helper class that extracts the command string corresponding to the different <see cref="Command"/>s
    /// </summary>
    internal static class EnumHelper
    {
        /// <summary>
        /// Retrieve the description on the enum
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetCommandString(this Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute) attrs[0]).Description;
                }
            }

            return en.ToString();
        }
    }
}