﻿using System;
using System.Collections.Generic;
using System.Text;
using SuperSQLInjection.model;
using System.Text.RegularExpressions;
using SuperSQLInjection.tools;
using System.Collections;
using tools;

namespace SuperSQLInjection.bypass
{
    class StringReplace
    {
        public static String lastRand = "";
        public static String lastpayload = "";
        public static String strReplaceCenter(Config config, String request, Hashtable replaceList,String payload)
        {
            //修改随机值
            String rand = "";
            if (payload.Equals(lastpayload)) {
                rand = lastRand;
            }
            else {
               rand = System.Guid.NewGuid().ToString("N");
            }
           
            if (payload.Equals("请求二次注入页面")) {
                rand = lastRand;
            }
            request = Regex.Replace(request, "(\\<Rand\\>[.\\s\\S]*?\\<\\/Rand\\>)", rand);
            lastRand = rand;
            lastpayload = payload;
            //找到需要处理的字符
            MatchCollection mc = Regex.Matches(request, "(?<=(\\<Encode\\>))[.\\s\\S]*?(?=(\\<\\/Encode\\>))");
            String str="";
            foreach (Match m in mc)
            {
                str = m.Value;
                str = bypassUseBetweentAnd(config, str);
                if (config.reaplaceBeforURLEncode || config.isOpenURLEncoding==false)
                {
                    //替换字符
                    str = ReplaceString(replaceList, str);
                    if (config.inculdeStr)
                    {
                        String split = " ";
                        ///*!包含分隔符*/
                        String val=getValue(replaceList," ");
                        if (!"".Equals(val)) {
                            split = val;
                        }
                        str = IncludeString(str);
                    }
                    if (config.useUnicode)
                    {
                        //unicode
                        str = Tools.String2Unicode(str);
                    }
                    else
                    {
                        if (config.isOpenURLEncoding)
                        {
                            //URL编码
                            str = urlEncoding(str, config.urlencodeCount);
                        }
                    }
                    
                }
                else {

                   if (config.inculdeStr)
                    {
                        ///*!包含*/
                        str = IncludeString(str);
                    }

                    if (config.useUnicode)
                    {
                        str = Tools.String2Unicode(str);
                    }
                    else
                    {
                        //unicode
                        if (config.isOpenURLEncoding)
                        {
                            //URL编码
                            str = urlEncoding(str, config.urlencodeCount);
                        }
                    }
                   
                    //替换字符
                    str = ReplaceString(replaceList, str);
                }
                //随机大小写
                if (config.keyReplace>0)
                {
                    String splitstr = " ";
                    if (config.isOpenURLEncoding) {
                        splitstr = "%20";
                    }
                    str = toLowerOrUpperCase(str, splitstr, config.keyReplace);
                }
                //base64处理
                if (config.base64Count>0) {
                    str = base64Encoding(str,config.base64Count);
                }

                //hex处理
                if (config.usehex)
                {
                    str = Tools.strToHex(str,"UTF-8");
                }

                //替换request
                request = request.Replace("<Encode>" + m.Value + "</Encode>", str);
            }
            return request;
        }

        public static String urlEncoding(String str,int index)
        {

            for (int i = 1; i <= index; i++)
            {
                str=URLEncode.UrlEncode(str);
            }
            return str;

        }


        public static String base64Encoding(String str,int index) {

            for (int i = 1; i <= index; i++) {
                str = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
            }
            return str;
            
        }
        public static String ReplaceString(Hashtable repalceList,String str) {
            if (repalceList != null && repalceList.Count > 0) {
                try
                {
                    IDictionaryEnumerator ite = repalceList.GetEnumerator();
                    while (ite.MoveNext())
                    {
                        String key = ite.Key.ToString();
                        if (!String.IsNullOrEmpty(key)) {
                            str = str.Replace(key, ite.Value + "");
                        }
                        
                    }
                }
                catch (Exception e)
                {
                    Tools.SysLog("替换字符发生错误！----" + e.Message);
                }
               
            }
            return str;

        }

        public static String IncludeString(String str)
        {
            try
            {
                MatchCollection mc = Regex.Matches(str, "[a-zA-Z_\\.\\(\\@]{2,50}");
                int sum = 0;
                StringBuilder sb = new StringBuilder();
                foreach (Match m in mc)
                {
                    if (m.Value.IndexOf("@") != -1)
                    {
                        continue;
                    }
                    str =str.Insert(m.Index+(sum), "/*!");
                    sum += 3;
                    str=str.Insert(m.Index+(sum) +m.Length, "*/");
                    sum += 2;
                }
            }
            catch (Exception e)
            {
                Tools.SysLog("使用/*!*/包含关键字发生错误！----" + e.Message);
            }
            return str;

        }

        public static String getValue(Hashtable table, String key)
        {
            try
            {
                IDictionaryEnumerator ite = table.GetEnumerator();
                while (ite.MoveNext())
                {
                    if (key.Equals(ite.Key)) {
                        return ite.Value+"";
                    }
                }
            }
            catch (Exception e)
            {
                Tools.SysLog("获取对应键值对发生错误！----" + e.Message);
            }
            return "";
        }


        public static String randStr(String key) {
            StringBuilder sb = new StringBuilder();
            Char[] cs = new Char[key.Length];
            cs = key.ToCharArray(0, key.Length);
            for (int j = 0; j < cs.Length; j++)
            {
                string c = cs[j] + "";
                if (j % 2 == 0)
                {
                    c = c.ToUpper();
                }
                else
                {
                    c = c.ToLower();
                }
                sb.Append(c);
            }
            return sb.ToString();
        }


        public static String bypassUseBetweentAnd(Config config,String paylaod) {

            if (config.useBetweenByPass)
            {
                

                MatchCollection mc=Regex.Matches(paylaod, @"(?<str>[\>\<\=]+)(?<len>\d+)"); ;
                if (mc.Count <= 0) {
                    return paylaod;
                }
                int offset = 0;
                foreach (Match mt in mc)
                {
                    String mstr = mt.Groups["str"].Value;
                    int findex = mt.Index;
                    String is16 = "";
                    if (findex!=0&&findex < paylaod.Length- mt.Length- offset) {
                        is16 = paylaod.Substring(findex + offset, mt.Length + 1);
                    }
                    if (is16.Contains("0x"))
                    {
                        //判断是否存在16进制情况，有则跳出
                        continue;
                    }
                    else
                    {
                        int len = Tools.convertToInt(mt.Groups["len"].Value);
                      
                        if (mstr.Contains(">="))
                        {
                            String rp = " not between 0 and " + (len - 1);
                            paylaod =paylaod.Remove(findex+offset, mt.Length).Insert(findex+offset, rp);
                            offset += rp.Length- mt.Length;
                        }
                        else if (mstr.Equals("<="))
                        {
                            String rp = " between 0 and " + len;
                            paylaod =paylaod.Remove(findex, mt.Length).Insert(findex, rp);
                           
                        }
                        else if (mstr.Equals(">"))
                        {
                            String rp = " not between 0 and " + len;
                            paylaod =paylaod.Remove(findex+ offset, mt.Length).Insert(findex+ offset, rp);
                            offset += rp.Length - mt.Length;
                        }
                        else if (mstr.Equals("="))
                        {
                            String rp = " between " + len + " and " + len;
                            paylaod =paylaod.Remove(findex + offset, mt.Length).Insert(findex + offset, rp);
                            offset += rp.Length - mt.Length;
                        }

                        else if (mstr.Equals("<"))
                        {
                            String rp = " between 0 and " + (len - 1);
                            paylaod =paylaod.Remove(findex, mt.Length).Insert(findex, rp);
                            offset += rp.Length - mt.Length;
                        }
                    }

                }
            }
            return paylaod;
        }
   
        public static String toLowerOrUpperCase(String oldStr, String split,int changeType)
        {

            StringBuilder sb = new StringBuilder();
            try
            {
                MatchCollection mc = Regex.Matches(oldStr, "([a-zA-Z_\\.]+|\\@+[a-zA-Z_]+|[a-zA-Z_]\\(\\)+|\\'[a-zA-Z_\\-]+\\')");
                foreach (Match m in mc) {

                    String keyStr =m.Groups[0].Value;
                    //库名.表,全局变量,环境变量不处理防止部分情况出现错误
                    if (keyStr.IndexOf("'") != -1||keyStr.IndexOf(".") != -1||keyStr.IndexOf("@") != -1 || keyStr.IndexOf("()") != -1) {
                        continue;
                    }
                    if (changeType == 1) {
                        oldStr = oldStr.Replace(keyStr, randStr(keyStr));
                    }
                    if (changeType == 2)
                    {
                        oldStr = oldStr.Replace(keyStr, keyStr.ToUpper());
                    }
                    if (changeType == 3)
                    {
                        oldStr = oldStr.Replace(keyStr, keyStr.ToLower());
                    } 
                    m.NextMatch();

                }
                /*
                String[] strs = Regex.Split(oldStr, split);
                for (int i = 0; i < strs.Length; i++)
                {
                    String s = strs[i];
                    if (s.IndexOf(".") != -1||s.IndexOf("@") != -1||s.IndexOf("(") != -1)
                    {
                        sb.Append(s);
                    }
                    else {
                    Char[] cs=new Char[s.Length];
                    cs = s.ToCharArray(0, s.Length);
                    for (int j = 0; j < cs.Length; j++)
                    {
                        string c = cs[j]+"";
                        if (j % 2 == 0)
                        {
                            c = c.ToUpper();
                        }
                        else {
                            c = c.ToLower();
                        }
                        sb.Append(c);
                    }
                    }
                    if (i+1 != strs.Length) { 
                        sb.Append(split);
                    }
                }*/
            }
            catch (Exception e)
            {
                Tools.SysLog("生成随机大小写字母发生错误！----" + e.Message);
            }
            return oldStr.Replace("0X","0x");
        }
    }
}
