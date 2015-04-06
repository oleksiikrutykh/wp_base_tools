namespace BaseTools.Core.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class StringParser : ResponseParser<string>
    {
        protected override DataResponse<string> Parse(System.IO.Stream content)
        {
            var reader = new StreamReader(content);
            var contentString = reader.ReadToEnd();
            return new DataResponse<string> { Result = contentString };
        }
    }
}
