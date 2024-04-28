using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.Assistant.IService;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant
{
    internal class SixLaborHelper: ISixLaborHelper
    {

        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="templateImagePath"></param>
        /// <param name="mergeImagePath">合并图片</param>
        /// <param name="x">X坐标(不是像素单位px，是相对位置单位pt)</param>
        /// <param name="y">y坐标(不是像素单位px，是相对位置单位pt)</param>
        /// <returns></returns>
        public Image MergeImage(string templateImagePath, string mergeImagePath, int x, int y)
        {
            var templateImage = Image.Load(templateImagePath);
            // 加载需要合并的图片
            var mergeImage = Image.Load(mergeImagePath);

            templateImage.Mutate(o =>
            {
                o.DrawImage(mergeImage, new Point(x, y), 1);
            });

            return templateImage;
        }
        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="templateImageData"></param>
        /// <param name="mergeImageData">合并图片字节流</param>
        /// <param name="x">X坐标(不是像素单位px，是相对位置单位pt)</param>
        /// <param name="y">y坐标(不是像素单位px，是相对位置单位pt)</param>
        /// <returns></returns>
        public Image MergeImage(byte[] templateImageData, byte[] mergeImageData, int x, int y)
        {
            var templateImage = Image.Load(templateImageData);
            // 加载需要合并的图片
            var mergeImage = Image.Load(mergeImageData);

            templateImage.Mutate(o =>
            {
                o.DrawImage(mergeImage, new Point(x, y), 1);
            });

            return templateImage;
        }
        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="templateImageData"></param>
        /// <param name="mergeImageData">合并图片文件流</param>
        /// <param name="x">X坐标(不是像素单位px，是相对位置单位pt)</param>
        /// <param name="y">y坐标(不是像素单位px，是相对位置单位pt)</param>
        /// <returns></returns>
        public Image MergeImage(Stream templateImageData, Stream mergeImageData, int x, int y)
        {
            var templateImage = Image.Load(templateImageData);
            // 加载需要合并的图片
            var mergeImage = Image.Load(mergeImageData);

            templateImage.Mutate(o =>
            {
                o.DrawImage(mergeImage, new Point(x, y), 1);
            });

            return templateImage;
        }


        /// <summary>
        /// 图片上写字
        /// https://docs.sixlabors.com/articles/imagesharp.drawing/gettingstarted.html#expanded-example
        /// </summary>
        /// <param name="ImageData"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="txt"></param>
        /// <param name="fontPath"></param>
        /// <returns></returns>
        public Image DrawingTextOnImage(byte[] ImageData, int x, int y, string txt, string fontPath)
        {
            var image = Image.Load(ImageData);
            FontCollection collection = new FontCollection();
            string fontFile = @$"{fontPath}\fonts\siyuansongti.ttf";
            FontFamily family = collection.Add(fontFile);
            Font font = family.CreateFont(12, FontStyle.Regular);
            PointF point = new PointF(x, y);
            image.Mutate(x => x.DrawText(txt, font, Color.Black, point));
            return image;
        }

        /// <summary>
        /// 生成图片证书
        /// </summary>
        /// <param name="model"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public async Task<string> MakeCertPic(string savePath, CertTemplateDto model, string filename)
        {
            string storageType = ConfigurationHelper.GetSectionValue("storageType");
            if (storageType == "local")
            {
                string localPath = savePath + model.certTempUrl.Replace("/", "\\");
                model.certTempData = FileHelper.getFileByte(localPath);
            }
            else if (model.certTempData == null && !string.IsNullOrEmpty(model.certTempUrl))
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                model.certTempData = wc.DownloadData(model.certTempUrl);
                wc.Dispose();
            }
            FontCollection collection = new FontCollection();
            string fontFile = @$"{savePath}\fonts\siyuansongti.ttf";
            FontFamily family = collection.Add(fontFile);
            Image image = Image.Load(model.certTempData);
            var contentList = model.contentList.OrderBy(u => u.orderIndex);
            Regex regex = new Regex("，+|、+|,+");
            foreach (var item in contentList)
            {
                if (string.IsNullOrEmpty(item.content))
                    continue;
                if (regex.IsMatch(item.content))
                {
                    var parts = regex.Split(item.content);
                    Hashtable ht = new Hashtable();
                    foreach (var part in parts)
                    {
                        if (!string.IsNullOrEmpty(part) && !ht.ContainsKey(part.Trim()))
                            ht.Add(part.Trim(), part.Trim());
                    }
                    if (ht.Count == 1)//只有多个学校都一致的时候，才只输出一个名字，别的时候都是全部输出
                    {
                        item.content = parts[0];
                    }
                }
                Font font = family.CreateFont(item.fontSize, FontStyle.Regular);
                //是否设置了折行显示
                if (item.row_maxwords > 0)
                {
                    double rowCnt = Math.Round((double)item.content.Length / item.row_maxwords, 1);
                    rowCnt = Math.Ceiling(rowCnt);//进一

                    for (int i = 0; i < rowCnt; i++)
                    {
                        float newLocationY = item.location_y + item.row_spacing * i;
                        PointF point = new PointF(item.location_x, newLocationY);
                        Point p = Point.Round(point);
                        p = Point.Round(point);
                        string newContent = item.content;
                        if (item.content.Length > item.row_maxwords && item.content.Length - ((i + 1) * item.row_maxwords) >= 0)
                            newContent = item.content.Substring(i * item.row_maxwords, item.row_maxwords);
                        else if (item.content.Length > item.row_maxwords && item.content.Length - ((i + 1) * item.row_maxwords) < 0)
                            newContent = item.content.Substring(i * item.row_maxwords, item.content.Length - (i * item.row_maxwords));
                        image.Mutate(x => x.DrawText(newContent, font, Color.ParseHex(item.fontColor), p));
                    }
                }
                else if (item.specialHandle == SpecialHandle.DoubleKeyBreak && regex.IsMatch(item.content) && regex.Split(item.content).Length == 2)
                {

                    var parts = regex.Split(item.content);
                    for (int i = 0; i < parts.Length; i++)
                    {
                        float newLocationY = item.location_y + item.row_spacing * i;
                        PointF point = new PointF(item.location_x, newLocationY);
                        Point p = Point.Round(point);
                        p = Point.Round(point);
                        image.Mutate(x => x.DrawText(parts[i], font, Color.ParseHex(item.fontColor), p));
                    }

                }
                else
                {
                    PointF point = new PointF(item.location_x, item.location_y);
                    Point p = Point.Round(point);
                    //image.Mutate(x => x.DrawText(item.content, font, Color.Black, p));
                    image.Mutate(x => x.DrawText(item.content, font, Color.ParseHex(item.fontColor), p));
                }
            }
            var imgList = model.imgList.OrderBy(u => u.orderIndex);
            foreach (var item in imgList)
            {
                string tmpQrcodeUrl = "";
                //二维码都放在本地
                if (item.imgType == 0)
                {
                    //filename = $"test_{DateTime.Now.ToString("yyyyMMdd")}";
                    string resourceHost = ConfigurationHelper.GetSectionValue("baseHost");
                    if (!resourceHost.Contains("upfile"))
                        resourceHost = resourceHost + "upfile";
                    string imgUrl = $"{resourceHost}/{DateTime.Now.ToString("yyyyMM")}/{Utils.GetCurrentWeekOfMonth(DateTime.Now)}/{filename}.jpg";
                    // item.imgUrl = QrcodeHelper.MakeQrcode(imgUrl, 0, filename, 10, savePath);
                    // tmpQrcodeUrl = savePath + item.imgUrl;//二维码赋给临时值
                    // Logger.Debug($"生成二维码：{tmpQrcodeUrl}");
                    // ResizeImg(tmpQrcodeUrl, (int)item.width, (int)item.height);

                    tmpQrcodeUrl = savePath + $"\\upfile\\{DateTime.Now.ToString("yyyyMM")}\\{Utils.GetCurrentWeekOfMonth(DateTime.Now)}\\qrcode_{filename}.png";
                    item.imgData = QrcodeHelper.MakeQrcodeByte(imgUrl, 10);
                    ResizeImg(item.imgData, tmpQrcodeUrl, (int)item.width, (int)item.height);
                    // Logger.Debug($"重置二维码尺寸：ok");
                    item.imgData = FileHelper.getFileByte(tmpQrcodeUrl);
                    // Logger.Debug($"获取二维码字节流：ok");
                    // Logger.Debug($"{filename} end!-----------------");
                }
                // 加载需要合并的图片（注意要转换单位像素单位pt转成印刷单位pt）
                var mergeImage = Image.Load(item.imgData);
                PointF pointf = new PointF(item.location_x, item.location_y);
                image.Mutate(o =>
                {
                    o.DrawImage(mergeImage, Point.Round(pointf), 1);
                });
                mergeImage.Dispose();
                item.imgData = null;
                File.Delete(tmpQrcodeUrl);
            }
            string uploadPath = @$"{savePath}\upfile\{DateTime.Now.ToString("yyyyMM")}\{Utils.GetCurrentWeekOfMonth(DateTime.Now)}\{filename}.jpg";
            if (File.Exists(uploadPath))
                File.Delete(uploadPath);
            await image.SaveAsJpegAsync(uploadPath);
            string ret = await FileHelper.SyncFile(uploadPath, filename);
            return ret;
        }

        

        /// <summary>
        /// 放大缩小图片
        /// </summary>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ResizeImg(string path, int width, int height)
        {
            using (Image image = Image.Load(path))
            {
                image.Mutate(x => x
                     .Resize(width, height)
                     .Grayscale());

                image.Save(path); // Automatic encoder selected based on extension.
                image.Dispose();
            }
        }

        public void ResizeImg(byte[] bytes, string output, int width, int height)
        {
            using (Image image = Image.Load(bytes))
            {
                if (File.Exists(output))
                    File.Delete(output);
                image.Mutate(x => x
                     .Resize(width, height)
                     .Grayscale());

                image.Save(output); // Automatic encoder selected based on extension.
                image.Dispose();
            }
        }


    }
}
