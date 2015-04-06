namespace BaseTools.UI.Controls
{
    using System;

    public class BindableApplicationBarException : Exception
    {
        public BindableApplicationBarException()
        { }

        public BindableApplicationBarException(string message)
            : base(message)
        {

        }

        public BindableApplicationBarException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
