using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.IService
{
    public interface INpoiExcelOperationService
    {

        /// <summary>
        /// 提交生成任务
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="whereJsonStr"></param>
        /// <returns></returns>
        Task<dynamic> SubmitExcelTask(string fileName, string whereJsonStr, string adminId);

        /// <summary>
        /// 通用导出模板
        /// </summary>
        /// <typeparam name="T">实体类型，类型中属性必须添加[Description("name")]特性，其中name需要标记导出excel二级标题栏名称</typeparam>
        /// <param name="filename">导出文件名称（方法内已集成时间后缀，无需重复加时间后缀）</param>
        /// <param name="sheetname">导出excel一级标题名称（方法内已集成时间后缀，无需重复加时间后缀）</param>
        /// <param name="list">要导出的数据集合</param>
        /// <param name="save_path">服务器保存路径（方法内自动在本路径下创建../upfile/当前时间/xxx.xls）</param>
        /// <returns></returns>
        Task<dynamic> ExcelDataExportTemplate<T>(string filename, string sheetname, List<T> list, string save_path) where T : new();

        Task<string> ExcelDataExportTemplateStr<T>(string filename, string sheetname, List<T> list, string save_path) where T : new();

        /// <summary>
        /// 通用导出模板2，可导出图片，需要单独调用SaveExcel方法
        /// </summary>
        /// <typeparam name="T">实体类型，类型中属性必须添加[Description("name")]特性，其中name需要标记导出excel二级标题栏名称</typeparam>
        /// <param name="filename">导出文件名称（方法内已集成时间后缀，无需重复加时间后缀）</param>
        /// <param name="sheetname">导出excel一级标题名称（方法内已集成时间后缀，无需重复加时间后缀）</param>
        /// <param name="list">要导出的数据集合</param>
        /// <param name="workbook">导出一个excel中包含多个sheet时，需要传此参数</param>
        /// <param name="img">导出图片时需要传此参数，base64格式</param>
        /// <returns></returns>
        HSSFWorkbook ExcelDataExportTemplate2<T>(string sheetname, List<T> list, string img = "", HSSFWorkbook workbook = null) where T : new();

        Task<dynamic> ExcelDataExportForYoungSV(string filename, string sheetname, List<Lib.ApplySummaryForExcelModel> list, string save_path, bool syncTocloud = false);
        Task<string> ExcelDataExportForYoungSVForPdf(string title, string subtitle, string programTypeCaption, string sheetname, List<Lib.ApplySummaryForPdfModel> list, string save_path);

        Task<string> DocDateExportForYoungSV(string savePath, string title, string eventName, string fieldName, List<Lib.ApplySummaryForPdfModel> list);
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<string> SaveFile(string filePath, string fileName, bool cloudSync = false);
        /// <summary>
        /// 保存excel
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="save_path">服务器保存路径（方法内自动在本路径下创建../upfile/当前时间/xxx.xls）</param>
        /// <returns></returns>
        Task<dynamic> SaveExcel(HSSFWorkbook workbook, string save_path);

        /// <summary>
        /// 往excel中插入图片，导出文本时从（横向 0,11）或（纵向17,0）开始，因为图片占用空间是10,17
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="img">img_base64</param>
        /// <param name="path">导出路径，如不需要单独导出图片不传此参数</param>
        /// <returns></returns>
        HSSFWorkbook CreatePircture(HSSFWorkbook workbook, ISheet sheet, string img, int column_index, string path = "");
    }
}
