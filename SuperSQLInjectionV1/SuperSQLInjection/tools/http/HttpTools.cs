using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using model;
using System.Threading;

namespace tools
{
    
    class HttpTools
    {
         public static String getHTMLEncoding(String header){

             Match m=Regex.Match(header, "charset=\\S{0,8}\"");
             if (m.Success) {
                return m.Groups[0].Value.Replace("charset=","").Replace("\"","");
             }
             return "";
         }

    }
}
