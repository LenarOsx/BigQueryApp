using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Response : IResponse
    {
        public bool Success { get; }

        public string Message { get; } = string.Empty;

        public Response(bool success)
        {
            Success = success;
        }

        public Response(bool success, string message) : this(success)
        {
            Message = message;
        }
    }

    public class Response<T> : Response
    {
        public IEnumerable<T> Data { get; } = new List<T>();
        public long Rows { get; }

        public Response(bool success, IEnumerable<T> data, string message) : base(success, message) 
        {
            Data = data;
            Rows = data.Count();
        }

        public Response(bool success, string message) : this(success, new List<T>(), message) { }

        public Response(bool success, T data, string message) : this(success, new List<T>() { data }, message) { }
    }
}
