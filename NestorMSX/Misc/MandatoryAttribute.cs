using System;

namespace Konamiman.NestorMSX.Misc
{
    /// <summary>
    /// Attribute used to signal a string parameter
    /// in the Configuration class as mandatory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MandatoryAttribute : Attribute
    {
    }
}