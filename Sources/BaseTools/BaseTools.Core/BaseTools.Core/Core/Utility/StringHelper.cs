namespace BaseTools.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class StringHelper
    {
        public static string ConvertFirstCharacterToLowerCase(string originValue)
        {
            string resultValue = null;
            if (originValue != null && originValue.Length > 0)
            {
                var firstCharacter = originValue[0].ToString().ToLower();
                resultValue = firstCharacter;
                if (originValue.Length > 1)
                {
                    resultValue = firstCharacter + originValue.Substring(1);
                }
            }

            return resultValue;
        }

        public static string ConvertFirstCharacterToUpperCase(string originValue)
        {
            string resultValue = null;
            if (originValue != null && originValue.Length > 0)
            {
                var firstCharacter = originValue[0].ToString().ToUpper();
                resultValue = firstCharacter;
                if (originValue.Length > 1)
                {
                    resultValue = firstCharacter + originValue.Substring(1);
                }
            }

            return resultValue;
        }
    }
}
