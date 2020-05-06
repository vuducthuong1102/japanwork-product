﻿using System;

namespace MyCloud.ShareLibs.Exceptions
{
    public class CustomSystemException : Exception
    {
        public CustomSystemException()
            : base()
        {
        }

        public CustomSystemException(string key, string value)
            : base()
        {
            base.Data.Add(key, value);
        }

        public CustomSystemException(string message)
            : base(message)
        {
        }

        public CustomSystemException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
