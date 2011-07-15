//-----------------------------------------------------------------------
// <copyright file="Month.cs" company="TWU">
// MIT Licenced
// </copyright>
//-----------------------------------------------------------------------

namespace Nvelope
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a month of the year
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
        Justification = "There's no such thing as a 'default' or 'none' month.")]
    public enum Month
    {
        /* These doc comments are stupid, but it keeps FxCop from getting made
         * and it by looking at Microsoft's docs it seems to be in line with
         * their practices */

        /// <summary>
        /// Indicates January
        /// </summary>
        January = 1,

        /// <summary>
        /// Indicates February
        /// </summary>
        February = 2,

        /// <summary>
        /// Indicates January
        /// </summary>
        March = 3,

        /// <summary>
        /// Indicates April
        /// </summary>
        April = 4,

        /// <summary>
        /// Indicates January
        /// </summary>
        May = 5,

        /// <summary>
        /// Indicates June
        /// </summary>
        June = 6,

        /// <summary>
        /// Indicates July
        /// </summary>
        July = 7,

        /// <summary>
        /// Indicates August
        /// </summary>
        August = 8,

        /// <summary>
        /// Indicates September
        /// </summary>
        September = 9,

        /// <summary>
        /// Indicates October
        /// </summary>
        October = 10,

        /// <summary>
        /// Indicates November
        /// </summary>
        November = 11,

        /// <summary>
        /// Indicates December
        /// </summary>
        December = 12
    }
}
