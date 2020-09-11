using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API
{
    public class ApiClient
    {
        private static ApiClient _instance;
        public static ApiClient Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ApiClient();
                return _instance;
            }
        }
    }
}
