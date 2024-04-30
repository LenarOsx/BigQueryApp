using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class JsonResponse
    {
        public bool status;

        public string code;

        public string internalCode;

        public string message;

        public object? data;

        public JsonResponse()
        {
            status = false;
            internalCode = "";
            code = "";
            message = "";
        }
    }
}
