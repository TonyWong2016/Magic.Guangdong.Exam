using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Color = System.Drawing.Color;


namespace Magic.Guangdong.Assistant
{
    public class QrcodeHelper
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="strCode">二维码内容</param>
        /// <param name="retType">返回类型(当存储路径为local时，要确保retType传入0)</param>
        /// <param name="name">文件名</param>
        /// <param name="pixelsPerModule">码眼大小，直接返回二维码的话，建议设定成5-8</param>
        /// <param name="localPath">本地存储路径</param>
        /// <returns></returns>
        public static string MakeQrcode(string strCode, int retType = 0, string name = "", int pixelsPerModule = 8, string localPath = "")
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                try
                {
                    //QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrcode = new QRCode(qrCodeData);
                    // qrcode.GetGraphic 方法可参考最下发“补充说明”
                    if (strCode.Length > 100)
                        pixelsPerModule = 10;
                    //Bitmap qrCodeImage = qrcode.GetGraphic(pixelsPerModule, Color.Black, Color.FromArgb(249, 248, 244), null, 15, 6, false);
                    Bitmap qrCodeImage = qrcode.GetGraphic(pixelsPerModule, Color.Black, Color.Transparent, null, 15, 6, false);
                    string filename = "";
                    if (string.IsNullOrEmpty(name))
                        filename = $"qrcode_{Security.GenerateMD5Hash(strCode)}.png";
                    else
                        filename = $"qrcode_{name}.png";
                    string storageType = ConfigurationHelper.GetSectionValue("storageType");
                    string remoteBase = ConfigurationHelper.GetSectionValue("remoteBase");
                    string date = DateTime.Now.ToString("yyyyMM");

                    if (storageType == "server" && FileHelper.connectState())
                    {
                        MemoryStream ms = new MemoryStream();
                        qrCodeImage.Save(ms, ImageFormat.Jpeg);
                        byte[] data = new byte[ms.Length];
                        ms.Seek(0, System.IO.SeekOrigin.Begin);
                        ms.Read(data, 0, Convert.ToInt32(ms.Length));
                        DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                        string PathName = theFolder.ToString() + $"{date}\\";
                        FileHelper.Transport(data, PathName, filename);
                    }
                    //else if (storageType == "local")
                    else //其余情况都保存在本地
                    {
                        string savePath = $"{localPath}\\upfile\\{date}";
                        if (!Directory.Exists(savePath))
                            Directory.CreateDirectory(savePath);
                        string saveFile = $"{savePath}\\{filename}";
                        if (File.Exists(saveFile))
                            File.Delete(saveFile);
                        qrCodeImage.Save(saveFile);
                        qrCodeImage.Dispose();
                    }
                    string url = "";
                    if (retType == 1)//当storageType为local时，type为1时会出错
                        url = $"{remoteBase}\\{date}\\{filename}";//返回服务器相对路径
                    else
                        url = $"\\upfile\\{date}\\{filename}";//返回本地路径                
                    return url;
                }
                catch (Exception ex)
                {
                    Logger.Error($"error:生成失败{ex.Message},{ex.StackTrace}");
                }
                return "error";
            }

        }
        public static byte[] MakeQrcodeByte(string strCode, int width)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
                QRCode qrcode = new QRCode(qrCodeData);
                // qrcode.GetGraphic 方法可参考最下发“补充说明”                

                Bitmap qrCodeImage = qrcode.GetGraphic(width, Color.Black, Color.White, null, 15, 6, false);

                MemoryStream ms = new MemoryStream();
                qrCodeImage.Save(ms, ImageFormat.Jpeg);
                byte[] data = new byte[ms.Length];
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                ms.Read(data, 0, Convert.ToInt32(ms.Length));
                ms.Flush();
                ms.Close();
                ms.Dispose();
                return data;
            }
            catch (Exception ex)
            {
                Logger.Error($"error:生成失败{ex.Message},{ex.StackTrace}");
            }
            return null;
        }


    }
}
