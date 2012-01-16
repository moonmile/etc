using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace TwiLib
{
    public interface IHttpConnection
    {
        
        HttpStatusCode GetContent(
            string method,
            Uri requestUri,
            Dictionary<string,string> param,
            ref string content,
            Dictionary<string,string> headerInfo);
        bool Authenticate( 
            Uri url,
            string username, 
            string password );
       
        string AuthUsername { get; }
    }
}
