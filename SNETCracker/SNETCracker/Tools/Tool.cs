using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;
using System.Management;

namespace Tools
{
    class Tool
    {
        /**
         * 获取系统相关唯一ID,用于统计
         */
        public static String getSystemSid()
        {

            String sid = "";
            try
            {
                //获得系统名称
                RegistryKey rk = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion");
                sid = rk.GetValue("ProductName").ToString();
                rk.Close();
                //获得系统唯一号，系统安装id和mac组合
                sid += "_";

                var officeSoftware = new ManagementObjectSearcher("SELECT ID, ApplicationId, PartialProductKey, LicenseIsAddon, Description, Name, OfflineInstallationId FROM SoftwareLicensingProduct where PartialProductKey <> null");
                var result = officeSoftware.Get();
                foreach (var item in result)
                {
                    String c = item.GetPropertyValue("name").ToString();

                    if (item.GetPropertyValue("name").ToString().StartsWith("Windows"))
                    {

                        sid += item.GetPropertyValue("OfflineInstallationId").ToString() + "_";
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                sid += "ex_";
            }
            String mac = "";
            try
            {
                NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in fNetworkInterfaces)
                {
                    String fCardType = "o";
                    String fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + adapter.Id + "\\Connection";
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                    if (rk != null)
                    {
                        String fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                        int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                        if (!String.IsNullOrEmpty(fPnpInstanceID) && fPnpInstanceID.StartsWith("PCI"))
                        {
                            if (fMediaSubType == 2)
                            {
                                fCardType = "w";
                            }
                            else
                            {
                                fCardType = "n";
                            }
                            mac = fCardType + ":" + adapter.GetPhysicalAddress().ToString() + "--";
                        }
                    }
                }
                if (mac.EndsWith("--"))
                {
                    mac = mac.Substring(0, mac.Length - 2);
                }
            }
            catch
            {
            }
            return sid + mac;

        }
        public static void HttpDownloadFile(string url, string path)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();

            //创建本地文件写入流
            Stream stream = new FileStream(path, FileMode.Create);

            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
        }
    }
}
