﻿using System;
using System.Text;
using System.Net.Sockets;
using System.IO.Compression;
using System.IO;
using System.Net.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using http.model;

namespace http
{
    public class HTTP
    {
        public const char T = '\n';
        public const String CT = "\r\n";
        public const String CTRL = "\r\n\r\n";
        public const String Content_Length_Str = "content-length: ";
        public const String Content_Length_Str_M = "Content-Length: ";
        public const String Content_Length = "content-length";
        public const String Content_Encoding = "content-encoding";
        public const String Transfer_Encoding = "transfer-encoding";
        public const String Connection = "connection";
        public const int WaitTime = 20;
        public static long index = 0;

        /**
         * 
         发生异常尝试重连  
         *
         */
        public static ServerInfo sendRequest(HttpRequest request)
        {
            int count = 0;

            ServerInfo server = new ServerInfo();
            while (count<= request.TryCount)
            {
                try
                {
                    if (!request.ISSSL)
                    {
                        server = sendHTTPRequest(request.Host, request.Port, request.Body, request.Timeout, request.Encode, request.Redirect,request.RedirectByGet);
                        return server;
                    }
                    else {
                        server = sendHTTPSRequest(request.Host, request.Port, request.Body, request.Timeout, request.Encode, request.Redirect, request.RedirectByGet);
                        return server;
                    }
                }
                catch (Exception e) {
                    Tools.SysLog("发包发生异常," + e.Message);
                    server.timeout = true;
                    continue;
                }
                finally {
                    count++;
                }

            }
            return server;

        }


        private static void checkContentLength(ref ServerInfo server, ref String request)
        {

            //重新计算并设置Content-length
            int sindex = request.IndexOf(CTRL);
            server.reuqestHeader = request;
            if (sindex != -1)
            {
                server.reuqestHeader = request.Substring(0, sindex);
                server.reuqestBody = request.Substring(sindex + 4, request.Length - sindex - 4);
                int contentLength = Encoding.UTF8.GetBytes(server.reuqestBody).Length;
                String newContentLength = Content_Length_Str_M + contentLength;
                
                if (request.IndexOf(Content_Length_Str_M) != -1)
                {
                    request = Regex.Replace(request, Content_Length_Str_M + "\\d+", newContentLength);
                }
                else
                {
                    request = request.Insert(sindex, "\r\n" + newContentLength);
                }
            }
            else
            {
                request = Regex.Replace(request, Content_Length_Str + "\\d+", Content_Length_Str_M + "0");
                request += CTRL;
            }


        }

        private static void doHeader(ref ServerInfo server, ref String[] headers, ref String encoding)
        {
            try
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    if (i == 0)
                    {
                        String[] codesplit = headers[i].Split(' ');
                        if (codesplit.Length > 0)
                        {
                            String[] sh = headers[i].Split(' ');
                            if (sh.Length > 1)
                            {
                                server.code = Tools.convertToInt(sh[1]);
                            }
                        }
                        else
                        {
                            server.code = 0;
                        }
                    }
                    else
                    {
                        String[] kv = Regex.Split(headers[i], ": ");
                        String key = kv[0].ToLower();
                        if (!server.headers.ContainsKey(key))
                        {
                            //自动识别编码
                            if ("".Equals(encoding))
                            {
                                if ("content-type".Equals(key))
                                {
                                    String hecnode = getHTMLEncoding(kv[1], "");
                                    if (!String.IsNullOrEmpty(hecnode))
                                    {
                                        server.encoding = hecnode;
                                    }
                                }
                            }

                            if (kv.Length > 1)
                            {
                                server.headers.Add(key, kv[1]);
                            }
                            else
                            {
                                server.headers.Add(key, "");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        private static ServerInfo sendHTTPRequest(String host, int port, String request, int timeout, String encoding, Boolean foward_302, Boolean redirectDoGet)
        {
            Interlocked.Increment(ref HTTP.index);
            String index = Thread.CurrentThread.Name + Interlocked.Read(ref HTTP.index);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ServerInfo server = new ServerInfo();
            TcpClient clientSocket = null;
            int sum = 0;
            Boolean isupdateEncoding = false;
            try
            {
                if (port > 0 && port <= 65556)
                {
                    server.request = request;
                    TimeOutSocket tos = new TimeOutSocket();
                    clientSocket = tos.Connect(host, port, timeout);
                    if (sw.ElapsedMilliseconds >= timeout)
                    {
                        return server;
                    }
                    clientSocket.SendTimeout = timeout - tos.useTime;
                    if (clientSocket.Connected)
                    {
                        checkContentLength(ref server, ref request);
                        server.request = request;

                        byte[] requestByte = Encoding.UTF8.GetBytes(request);
                        clientSocket.Client.Send(requestByte);
                        byte[] responseBody = new byte[1024 * 1000];
                        int len = 0;
                        //获取header头
                        String tmp = "";
                        StringBuilder sb = new StringBuilder();
                        clientSocket.ReceiveTimeout = timeout - (int)sw.ElapsedMilliseconds;
                        do
                        {
                            byte[] responseHeader = new byte[1];
                            len = clientSocket.Client.Receive(responseHeader, 1, SocketFlags.None);
                            if (len <= 0)
                            {
                                Thread.Sleep(WaitTime);
                            }
                            if (len == 1)
                            {

                                char c = (char)responseHeader[0];
                                sb.Append(c);
                                if (c.Equals(T))
                                {
                                    tmp = String.Concat(sb[sb.Length - 4], sb[sb.Length - 3], sb[sb.Length - 2], c);
                                }
                            }
                        } while (!tmp.Equals(CTRL) && sw.ElapsedMilliseconds < timeout);

                        server.header = sb.ToString().Replace(CTRL, "");
                        String[] headers = Regex.Split(server.header, CT);
                        if (headers != null && headers.Length > 0)
                        {
                            //处理header
                            doHeader(ref server, ref headers, ref encoding);
                            //302 301跳转
                            if ((server.code == 302 || server.code == 301) && foward_302)
                            {
                                StringBuilder rsb = new StringBuilder(server.request);
                                int urlStart = server.request.IndexOf(" ") + 1;
                                int urlEnd = server.request.IndexOf(" HTTP");
                                if (urlStart != -1 && urlEnd != -1)
                                {
                                    String url = server.request.Substring(urlStart, urlEnd - urlStart);
                                    rsb.Remove(urlStart, url.Length);
                                    String location = server.headers["location"];
                                    if (!server.headers["location"].StartsWith("/") && !server.headers["location"].StartsWith("http"))
                                    {
                                        location = Tools.getCurrentPath(url) + location;
                                    }
                                    location = location.Replace(" ", "%20");
                                    rsb.Insert(urlStart, location);
                                    String newReuqest = rsb.ToString();
                                    if (server.request.StartsWith("POST") && redirectDoGet)
                                    {
                                        rsb.Remove(0, 4);
                                        rsb.Insert(0, "GET");
                                    }
                                    return sendHTTPRequest(host, port, rsb.ToString(), timeout, encoding, false, redirectDoGet);
                                }

                            }


                            //根据请求头解析
                            if (server.headers.ContainsKey(Content_Length))
                            {
                                int length = int.Parse(server.headers[Content_Length]);

                                while (sum < length && sw.ElapsedMilliseconds < timeout)
                                {
                                    int readsize = length - sum;
                                    len = clientSocket.Client.Receive(responseBody, sum, readsize, SocketFlags.None);
                                    if (len > 0)
                                    {
                                        sum += len;
                                    }
                                    if (len <= 0 && sum < length)
                                    {
                                        Thread.Sleep(WaitTime);
                                    }
                                }
                            }
                            //解析chunked传输
                            else if (server.headers.ContainsKey(Transfer_Encoding))
                            {
                                //读取长度
                                int chunkedSize = 0;
                                byte[] chunkedByte = new byte[1];
                                //读取总长度
                                sum = 0;
                                do
                                {
                                    String ctmp = "";
                                    do
                                    {
                                        len = clientSocket.Client.Receive(chunkedByte, 1, SocketFlags.None);
                                        if (len <= 0)
                                        {
                                            Thread.Sleep(WaitTime);
                                        }
                                        ctmp += Encoding.UTF8.GetString(chunkedByte);

                                    } while ((ctmp.IndexOf(CT) == -1) && (sw.ElapsedMilliseconds < timeout));

                                    chunkedSize = Tools.convertToIntBy16(ctmp.Replace(CT, ""));

                                    //chunked的结束0\r\n\r\n是结束标志，单个chunked块\r\n结束
                                    if (ctmp.Equals(CT))
                                    {
                                        continue;
                                    }
                                    if (chunkedSize == 0)
                                    {
                                        //结束了
                                        break;
                                    }
                                    int onechunkLen = 0;
                                    while (onechunkLen < chunkedSize && sw.ElapsedMilliseconds < timeout)
                                    {
                                        len = clientSocket.Client.Receive(responseBody, sum, chunkedSize - onechunkLen, SocketFlags.None);
                                        if (len > 0)
                                        {
                                            onechunkLen += len;
                                            sum += len;
                                        }
                                        if (len <= 0 && onechunkLen < chunkedSize)
                                        {
                                            Thread.Sleep(WaitTime);
                                        }
                                    }

                                    //判断
                                } while (sw.ElapsedMilliseconds < timeout);
                            }
                            //connection close方式或未知body长度
                            else
                            {
                                while (sw.ElapsedMilliseconds < timeout)
                                {
                                    if (clientSocket.Client.Poll(timeout, SelectMode.SelectRead))
                                    {
                                        if (clientSocket.Available > 0)
                                        {
                                            len = clientSocket.Client.Receive(responseBody, sum, (1024 * 200) - sum, SocketFlags.None);
                                            if (len > 0)
                                            {
                                                sum += len;
                                            }
                                            else
                                            {
                                                Thread.Sleep(WaitTime);
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                            //自动识别编码
                            if ("".Equals(encoding))
                            {
                                if (!String.IsNullOrEmpty(server.encoding))
                                {
                                    encoding = server.encoding;//header找到编码
                                }
                                else
                                {
                                    encoding = "UTF-8";//默认一个编码
                                    isupdateEncoding = true;//body找编码
                                }
                                Encoding encod = Encoding.GetEncoding(encoding);
                                getBody(ref server, ref responseBody, ref sum, ref encod, ref index);
                                //修正编码
                                if (isupdateEncoding)
                                {
                                    String cEncoding = getHTMLEncoding("", server.body);
                                    if (!String.IsNullOrEmpty(cEncoding))
                                    {
                                        server.encoding = cEncoding;//body找到编码
                                        getBody(ref server, ref responseBody, ref sum, ref encod, ref index);
                                    }

                                }
                            }
                            else
                            {
                                //指定编码
                                Encoding encod = Encoding.GetEncoding(encoding);
                                getBody(ref server, ref responseBody, ref sum, ref encod, ref index);
                            }

                        }
                    }

                }
            }
            catch (Exception e)
            {
                Exception ee = new Exception("HTTP发包错误！错误消息：" + e.Message + e.TargetSite.Name + "----发包编号：" + index);
                throw ee;
            }
            finally
            {
                sw.Stop();
                server.length = sum;
                server.runTime = (int)sw.ElapsedMilliseconds;
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }
            }
            return server;

        }


        private static void getBody(ref ServerInfo server, ref byte[] responseBody, ref int sum, ref Encoding encod, ref String index)
        {
            if (server.headers.ContainsKey(Content_Encoding))
            {
                if (server.headers[Content_Encoding].IndexOf("gzip") != -1)
                {
                    server.body = unGzip(responseBody, sum, encod, index);
                }
                else if (server.headers[Content_Encoding].IndexOf("deflate") != -1)
                {
                    server.body = unDeflate(responseBody, sum, encod, index);
                }
                else
                {
                    server.body = encod.GetString(responseBody, 0, sum);
                }

            }
            else
            {
                server.body = encod.GetString(responseBody, 0, sum);
            }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private static ServerInfo sendHTTPSRequest(String host, int port, String request, int timeout, String encoding, Boolean foward_302, Boolean redirectDoGet)
        {
            Interlocked.Increment(ref HTTP.index);
            String index = Thread.CurrentThread.Name + Interlocked.Read(ref HTTP.index);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ServerInfo server = new ServerInfo();
            Boolean isupdateEncoding = false;
            int sum = 0;

            TcpClient clientSocket = null; ;

            try
            {

                if (port > 0 && port <= 65556)
                {

                    TimeOutSocket tos = new TimeOutSocket();
                    clientSocket = tos.Connect(host, port, timeout);
                    if (sw.ElapsedMilliseconds >= timeout)
                    {
                        return server;
                    }
                    clientSocket.SendTimeout = timeout - tos.useTime;

                    SslStream ssl = null;
                    if (clientSocket.Connected)
                    {
                        ssl = new SslStream(clientSocket.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate));
                        SslProtocols protocol = SslProtocols.Ssl3 | SslProtocols.Ssl2 | SslProtocols.Tls;
                        ssl.AuthenticateAsClient(host, null, protocol, false);
                        if (ssl.IsAuthenticated)
                        {
                            checkContentLength(ref server, ref request);
                            server.request = request;
                            byte[] requestByte = Encoding.UTF8.GetBytes(request);
                            ssl.Write(requestByte);
                            ssl.Flush();
                        }
                    }
                    server.request = request;
                    byte[] responseBody = new byte[1024 * 1000];
                    int len = 0;
                    //获取header头
                    String tmp = "";

                    StringBuilder sb = new StringBuilder();
                    StringBuilder bulider = new StringBuilder();
                    clientSocket.ReceiveTimeout = timeout - (int)sw.ElapsedMilliseconds;
                    do
                    {
                        byte[] responseHeader = new byte[1];
                        int read = ssl.ReadByte();
                        if (read <= 0)
                        {
                            Thread.Sleep(WaitTime);
                        }
                        char c = (char)read;
                        sb.Append(c);
                        if (c.Equals(T))
                        {
                            tmp = String.Concat(sb[sb.Length - 4], sb[sb.Length - 3], sb[sb.Length - 2], c);
                        }

                    } while (!tmp.Equals(CTRL) && sw.ElapsedMilliseconds < timeout);

                    server.header = sb.ToString().Replace(CTRL, "");
                    String[] headers = Regex.Split(server.header, CT);
                    //处理header
                    doHeader(ref server, ref headers, ref encoding);
                    //302 301跳转
                    if ((server.code == 302 || server.code == 301) && foward_302)
                    {

                        StringBuilder rsb = new StringBuilder(server.request);
                        int urlStart = server.request.IndexOf(" ") + 1;
                        int urlEnd = server.request.IndexOf(" HTTP");
                        if (urlStart != -1 && urlEnd != -1)
                        {
                            String url = server.request.Substring(urlStart, urlEnd - urlStart);
                            rsb.Remove(urlStart, url.Length);
                            String location = server.headers["location"];
                            if (!server.headers["location"].StartsWith("/") && !server.headers["location"].StartsWith("http"))
                            {
                                location = Tools.getCurrentPath(url) + location;
                            }
                            location = location.Replace(" ", "%20");
                            rsb.Insert(urlStart, location);
                            String newReuqest = rsb.ToString();
                            if (rsb.ToString().StartsWith("POST") && redirectDoGet)
                            {
                                rsb.Remove(0, 4);
                                rsb.Insert(0, "GET");
                            }

                            return sendHTTPSRequest(host, port, rsb.ToString(), timeout, encoding, false, redirectDoGet);
                        }




                    }


                    //根据请求头解析
                    if (server.headers.ContainsKey(Content_Length))
                    {
                        int length = int.Parse(server.headers[Content_Length]);
                        while (sum < length && sw.ElapsedMilliseconds < timeout)
                        {
                            len = ssl.Read(responseBody, sum, length - sum);
                            if (len > 0)
                            {
                                sum += len;
                            }
                            if (len <= 0 && sum < length)
                            {
                                Thread.Sleep(WaitTime);
                            }
                        }
                    }
                    //解析chunked传输
                    else if (server.headers.ContainsKey(Transfer_Encoding))
                    {
                        //读取长度
                        int chunkedSize = 0;
                        byte[] chunkedByte = new byte[1];
                        //读取总长度
                        sum = 0;
                        do
                        {
                            String ctmp = "";
                            do
                            {
                                len = ssl.Read(chunkedByte, 0, 1);
                                if (len <= 0)
                                {
                                    Thread.Sleep(WaitTime);
                                }
                                ctmp += Encoding.UTF8.GetString(chunkedByte);

                            } while (ctmp.IndexOf(CT) == -1 && sw.ElapsedMilliseconds < timeout);

                            chunkedSize = Tools.convertToIntBy16(ctmp.Replace(CT, ""));

                            //chunked的结束0\r\n\r\n是结束标志，单个chunked块\r\n结束
                            if (ctmp.Equals(CT))
                            {
                                continue;
                            }
                            if (chunkedSize == 0)
                            {
                                //结束了
                                break;
                            }
                            int onechunkLen = 0;

                            while (onechunkLen < chunkedSize && sw.ElapsedMilliseconds < timeout)
                            {
                                len = ssl.Read(responseBody, sum, chunkedSize - onechunkLen);
                                if (len > 0)
                                {
                                    onechunkLen += len;
                                    sum += len;
                                }
                                if (len <= 0 && onechunkLen < chunkedSize)
                                {
                                    Thread.Sleep(WaitTime);
                                }
                            }

                            //判断
                        } while (sw.ElapsedMilliseconds < timeout);
                    }
                    //connection close方式或未知body长度
                    else
                    {
                        while (sw.ElapsedMilliseconds < timeout)
                        {
                            if (clientSocket.Client.Poll(timeout, SelectMode.SelectRead))
                            {
                                if (clientSocket.Available > 0)
                                {
                                    len = ssl.Read(responseBody, sum, (1024 * 200) - sum);
                                    if (len > 0)
                                    {
                                        sum += len;
                                    }
                                    else
                                    {
                                        Thread.Sleep(WaitTime);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    //自动识别编码
                    if ("".Equals(encoding))
                    {
                        if (!String.IsNullOrEmpty(server.encoding))
                        {
                            encoding = server.encoding;//header找到编码
                        }
                        else
                        {
                            encoding = "UTF-8";//默认一个编码
                            isupdateEncoding = true;//body找编码
                        }
                        Encoding encod = Encoding.GetEncoding(encoding);
                        getBody(ref server, ref responseBody, ref sum, ref encod, ref index);
                        //修正编码
                        if (isupdateEncoding)
                        {
                            String cEncoding = getHTMLEncoding("", server.body);
                            if (!String.IsNullOrEmpty(cEncoding))
                            {
                                server.encoding = cEncoding;//body找到编码
                                getBody(ref server, ref responseBody, ref sum, ref encod, ref index);
                            }

                        }
                    }
                    else
                    {
                        //指定编码
                        Encoding encod = Encoding.GetEncoding(encoding);
                        getBody(ref server, ref responseBody, ref sum, ref encod, ref index);
                    }
                }

            }
            catch (Exception e)
            {
                Exception ee = new Exception("HTTPS发包错误！错误消息：" + e.Message + "----发包编号：" + index);
                if (ee.Message.IndexOf("doHeader") != -1)
                {
                    String a = e.Message;
                }
                throw ee;
            }
            finally
            {
                sw.Stop();
                server.length = sum;
                server.runTime = (int)sw.ElapsedMilliseconds;

                if (clientSocket != null)
                {
                    clientSocket.Close();
                }
            }
            return server;

        }

        public static String unGzip(byte[] data, int len, Encoding encoding, String index)
        {

            String str = "";
            MemoryStream ms = new MemoryStream(data, 0, len);
            GZipStream gs = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outbuf = new MemoryStream();
            byte[] block = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = gs.Read(block, 0, block.Length);
                    if (bytesRead <= 0)
                    {
                        break;
                    }
                    else
                    {
                        outbuf.Write(block, 0, bytesRead);
                    }
                }
                str = encoding.GetString(outbuf.ToArray());
            }
            catch (Exception e)
            {
                Tools.SysLog("解压Gzip发生异常----" + e.Message + "----" + index);

            }
            finally
            {
                outbuf.Close();
                gs.Close();
                ms.Close();

            }
            return str;

        }

        public static String unDeflate(byte[] data, int len, Encoding encoding, String index)
        {

            String str = "";
            MemoryStream ms = new MemoryStream(data, 0, len);
            DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress);
            MemoryStream outbuf = new MemoryStream();
            byte[] block = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = ds.Read(block, 0, block.Length);
                    if (bytesRead <= 0)
                    {
                        break;
                    }
                    else
                    {
                        outbuf.Write(block, 0, bytesRead);
                    }
                }
                str = encoding.GetString(outbuf.ToArray());
            }
            catch (Exception e)
            {
                Tools.SysLog("解压deflate发生异常----" + e.Message + "----" + index);

            }
            finally
            {
                outbuf.Close();
                ds.Close();
                ms.Close();

            }
            return str;

        }


        public String SetCookies(string sHtml, String sCookies)
        {

            //Set-Cookie: b_110128=0; domain=.qidian.com; expires=Fri, 15-Sep-2023 15:48:41 GMT; path=/

            string sName = "";

            string sValue = "";

            MatchCollection mc;

            Match m;

            Regex r;

            if (!sCookies.EndsWith(";") && sCookies != "")
            {

                sCookies += ";";

            }

            r = new Regex("Set-Cookie:\\s*(?<sName>.*?)=(?<sValue>.*?);", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

            mc = r.Matches(sHtml);

            for (int i = 0; i < mc.Count; i++)
            {

                sName = mc[i].Groups["sName"].Value.Trim();

                sValue = mc[i].Groups["sValue"].Value.Trim();

                r = new Regex(sName + "\\s*=\\s*.*?;", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                m = r.Match(sCookies);

                if (m.Success)
                {

                    sCookies = sCookies.Replace(m.Value, sName + "=" + sValue + ";");

                }

                else
                {

                    sCookies += sName + "=" + sValue + ";";

                }

            }

            try
            {

                if (sCookies.StartsWith(";"))
                {

                    sCookies = sCookies.Substring(1, sCookies.Length - 1);

                }

            }

            catch
            {

            }
            return sCookies;

        }

        public static String getHTMLEncoding(String header, String body)
        {
            if (String.IsNullOrEmpty(header) && String.IsNullOrEmpty(body))
            {
                return "";
            }
            body = body.ToUpper();

            String encode = "";
            Match m = Regex.Match(header, @"charset=(?<charset>[\w\-]+)", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                encode = m.Groups["charset"].Value.ToUpper();
            }
            else
            {
                if (String.IsNullOrEmpty(body))
                {
                    return "";
                }
                m = Regex.Match(body, @"charset=['""]{0,1}(?<charset>[\w\-]+)['""]{0,1}", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    encode = m.Groups["charset"].Value.ToUpper();
                }
            }
            if ("UTF8".Equals(encode))
            {
                encode = "UTF-8";
            }
            return encode;


        }

        


    }
}
