using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.Infra.Exceptions
{
    public class ApiUnauthorizedException : Exception
    {
        public ApiUnauthorizedException()
        {

        }

        public ApiUnauthorizedException(string message) : base(message)
        {

        }
    }
}
