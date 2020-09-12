using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.Infra.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public ApiException()
        {

        }

        public ApiException(string message) : base(message)
        {

        }
    }
}
