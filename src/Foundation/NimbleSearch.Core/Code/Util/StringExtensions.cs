namespace NimbleSearch.Foundation.Core.Util
{
    public static class StringExtensions
    {

        /// <summary>
        /// Ensures string ends with given suffix
        /// </summary>
        /// <param name="mainString"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string EnsureSuffix(this string mainString, string suffix)
        {
            if (string.IsNullOrEmpty(mainString))
                return suffix;

            if (mainString.EndsWith(suffix))
                return mainString;
            return mainString + suffix;
        }

        /// <summary>
        /// Ensures string begins with given prefix
        /// </summary>
        /// <param name="mainString"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string EnsurePrefix(this string mainString, string prefix)
        {
            if (string.IsNullOrEmpty(mainString))
                return prefix;

            if (mainString.StartsWith(prefix))
                return mainString;
            return prefix + mainString;
        }


    }
}