namespace BaseTools.Core.Security
{
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Calculate hash by MD5 algorytm.
    /// </summary>
    public static class MD5Calculator
    {

        /// <summary>
        /// Calculate hash by MD5 algorytm.
        /// </summary>
        /// <param name="input">A System.String which must be hashed by MD5 algorytm</param>
        /// <returns>A System.String contains representation of MD5 hash.</returns>
        public static string GetMD5Hash(string input)
        {
             // Create a new Stringbuilder to collect the bytes// and create a string.
            var sBuilder = new StringBuilder();

            using (var md5Hasher = new MD5Managed())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Loop through each byte of the hashed data // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2", CultureInfo.InvariantCulture));
                }
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
