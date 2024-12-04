using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant
{
    /// <summary>
    /// 依赖Magicodes的excel帮助类
    /// 该工具类为国人开发，比NPOI功能强大，尤其在导入方面，开源免费。
    /// https://github.com/dotnetcore/Magicodes.IE/blob/master/README.zh-CN.md
    /// </summary>
    public static class ExcelHelper<T> where T : class, new()
    {
        public static IImporter Importer = new ExcelImporter();

        /// <summary>
        /// 获取导入的数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<List<T>> GetImportData(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            if (filePath.StartsWith("http"))
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(filePath);
                //var inFileStream = await client.GetStreamAsync(filePath);
                //byte[] buf = new byte[inFileStream.Length];
                byte[] buf = await client.GetByteArrayAsync(filePath);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upfile", DateTime.Now.ToString("yyyyMM"),
            $"{fileName}.xlsx");
                if (File.Exists(savePath)) File.Delete(savePath);
                FileStream outFileStream = new FileStream(savePath, FileMode.OpenOrCreate);
                await outFileStream.WriteAsync(buf, 0, buf.Length);

                outFileStream.Flush();
                outFileStream.Close();
                var import = await Importer.Import<T>(savePath);
                if (import.HasError)
                {
                    throw import.Exception;
                }
                return import.Data.ToList();
            }
            else
            {
                string storageType = ConfigurationHelper.GetSectionValue("storageType");
                string savePath = "";
                if (storageType == "local")
                {
                    savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + filePath;
                    if (!File.Exists(savePath))
                        return null;
                }
                else if (storageType == "server")
                {
                    FileHelper.connectState();
                    savePath = ConfigurationHelper.GetSectionValue("remoteBase") + filePath;
                }
                var import = await Importer.Import<T>(savePath);
                if (import.HasError)
                {
                    throw import.Exception;
                }
                return import.Data.ToList();
            }
        }

        /// <summary>
        /// 生成导入模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public static async Task<string> GenerateTemplate(string templateName)
        {
            IExportFileByTemplate exporter = new ExcelExporter();
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upfile", DateTime.Now.ToString("yyyyMM"), Utils.GetCurrentWeekOfMonth(DateTime.Now).ToString());
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, $"{templateName}.xlsx");
            if (File.Exists(filePath)) File.Delete(filePath);
            await Importer.GenerateTemplate<T>(filePath);
            return await FileHelper.SyncFile(filePath, $"{templateName}.xlsx");
        }


        public static async Task<dynamic> ExpoertExcel(string filename, List<T> list)
        {
            IExporter exporter = new ExcelExporter();

            var result = await exporter.Export(filename + ".xlsx", list);
            return result;
        }
    }
}
