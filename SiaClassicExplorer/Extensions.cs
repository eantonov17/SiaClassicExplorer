using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiaClassicExplorer
{
    public static class Extensions
    {
        public static bool TryGetParam(this HttpRequest request, string name, out string param)
        {
            param = request[name];
            if (string.IsNullOrWhiteSpace(param))
                return false;
            param = param.Trim();
            return true;
        }
    }
}