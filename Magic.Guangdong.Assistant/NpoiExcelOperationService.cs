using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XWPF.UserModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Magic.Guangdong.Assistant.IService;

namespace Magic.Guangdong.Assistant
{
    public class NpoiExcelOperationService : INpoiExcelOperationService
    {
        private readonly IResponseHelper resp;
        public NpoiExcelOperationService(IResponseHelper resp)
        {
            this.resp = resp;
        }

        public async Task<dynamic> SubmitExcelTask(string fileName,string whereJsonStr,string adminId)
        {
            var tasks = await RedisHelper.HGetAllAsync("MakeGDComplexExcelTask");
            if (tasks.Values.Any(u => u.Equals(whereJsonStr)))
            {
                return resp.ret(-1, "提交的检索任务已在队列中存在，请稍后再试");
            }
            await RedisHelper.HSetAsync("MakeGDComplexExcelTask", $"{fileName}|{adminId}|{Utils.DateTimeToTimeStamp(DateTime.Now)}", whereJsonStr);
            return resp.success(1,"导出任务已提交到后台队列，请耐心等待邮件通知。");
        }

        /// <summary>
        /// 通用导出模板（返回dynamic）
        /// </summary>
        /// <typeparam name="T">实体类型，类型中属性必须添加[Description("name")]特性，其中name需要标记导出excel二级标题栏名称</typeparam>
        /// <param name="filename">导出文件名称（方法内已集成时间后缀，无需重复加时间后缀）</param>
        /// <param name="sheetname">导出excel一级标题名称（方法内已集成时间后缀，无需重复加时间后缀）</param>
        /// <param name="list">要导出的数据集合</param>
        /// <param name="save_path">服务器保存路径（方法内自动在本路径下创建../upfile/当前时间/xxx.xls）</param>
        /// <returns></returns>
        public async Task<dynamic> ExcelDataExportTemplate<T>(string filename, string sheetname, List<T> list, string save_path) where T : new()
        {
            
            //Response resp = new Response();
            try
            {
                Type t_type = new T().GetType();//反射获取实体类型
                
                var workbook = new HSSFWorkbook();
                string _sheetName = sheetname;
                if (sheetname.Length > 31)
                {
                    sheetname = sheetname.Substring(0, 31);
                }
                if (string.IsNullOrEmpty(_sheetName) || _sheetName.Length > 32)
                    _sheetName = "汇总表_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var sheet = workbook.CreateSheet(_sheetName);
                sheet.ForceFormulaRecalculation = true;//TODO:是否开始Excel导出后公式仍然有效（非必须）

                //创建一级标题行
                var row = NPOIUtil._.CreateRow(sheet, 0, 28);
                var cell = row.CreateCell(0);


                #region 二级标题行

                row = NPOIUtil._.CreateRow(sheet, 1, 24);//创建第二行
                //二级标题样式
                var secondTitleStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 15, true, 700, "楷体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Grey25Percent.Index, HSSFColor.Black.Index, FontUnderlineType.None, FontSuperScript.None, false);

                //标题从0开始添加单元格内容
                int column_index = 0;//列索引
                NPOIUtil._.CreateCells(row, secondTitleStyleFont, column_index, "序号");
                foreach (var propeties in t_type.GetProperties())
                {
                    //循环该属性所有特性  找出Description特性，加载到excel二级标题列
                    foreach (var attr in propeties.CustomAttributes)
                    {
                        if (attr.AttributeType.Name == "DescriptionAttribute")//判断某个特性时，需要在特性后面加上Attribute（即 想要获取‘Description’特性，判断时需要判断是否等于‘DescriptionAttribute’）
                        {
                            column_index++;
                            NPOIUtil._.CreateCells(row, secondTitleStyleFont, column_index, attr.ConstructorArguments[0].Value.ToString());
                            break;
                        }
                    }
                }
                #endregion

                #region 一级标题行，写在二级标题下面是因为合并单元格时需要用到包含Description特性的属性数量
                //合并单元格 例： 第1行到第2行 第3列到第4列围成的矩形区域
                //TODO:关于Excel行列单元格合并问题
                /**
                  第一个参数：从第几行开始合并
                  第二个参数：到第几行结束合并
                  第三个参数：从第几列开始合并
                  第四个参数：到第几列结束合并
                **/
                //合并单元格  column_index 单元格数量
                CellRangeAddress region = new CellRangeAddress(0, 0, 0, column_index);
                sheet.AddMergedRegion(region);//将一级标题行加入到当前sheet中
                cell.SetCellValue($"{filename}（{DateTime.Now.ToString("yyyy-MM-dd")}）");//设置一级标题内容
                var firstTitleStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 20, true, 700, "楷体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Coral.Index, HSSFColor.White.Index, FontUnderlineType.None, FontSuperScript.None, false);//标题样式
                cell.CellStyle = firstTitleStyleFont;//设置一级标题样式 

                #endregion

                #region 数据内容
                //单元格内容样式
                var dataStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 10, true,0);
                
                //获取数据内容--属性值
                int line_index = 0;
                foreach (var model in list)
                {
                    Type type = model.GetType();
                    row = NPOIUtil._.CreateRow(sheet, line_index + 2, 20);//创建行，+2是因为需要跳过一级标题行和二级标题行
                    line_index++;
                    NPOIUtil._.CreateCells(row, dataStyleFont, 0, line_index.ToString());//列元素-序号
                    int index = 1;//列元素在单元格的位置，应该从0开始，第0列是‘序号’所以从1开始
                    foreach (var property in type.GetProperties())
                    {
                        foreach (var attr in property.CustomAttributes)
                        {
                            if (attr.AttributeType.Name == "DescriptionAttribute")//只取出标记为Description特性的属性值
                            {
                                object obj_value = property.GetValue(model, null);
                                string value = "";
                                if (obj_value != null)
                                {
                                    if (obj_value.GetType() == typeof(DateTime))
                                    {
                                        value = Convert.ToDateTime(obj_value).ToString("yyyy-MM-dd HH:mm:ss");
                                    }
                                    else
                                    {
                                        value = obj_value.ToString();
                                    }
                                }
                                NPOIUtil._.CreateCells(row, dataStyleFont, index, value);//列元素-属性值
                                index++;
                                break;
                            }
                        }
                    }
                }
                #endregion

                #region 保存到本地服务器
                string folder = $"magicexam/export/{DateTime.Now.ToString("yyyyMM")}";
                //保存文件到静态资源文件夹中（wwwroot）,使用绝对路径
                //var uploadPath = save_path + "/upfile/" + folder + "/";
                string uploadPath = $"{save_path}/upfile/{folder}";
                //excel保存文件名
                string excelFileName = $"{filename}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls";
                //创建目录文件夹
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                //Excel的路径及名称
                string excelPath = Path.Combine(uploadPath, excelFileName);
                using (var fileStream = new FileStream(excelPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    //向Excel文件对象写入文件流，生成Excel文件
                    workbook.Write(fileStream);
                }
                #endregion

                #region 导出的文件就不放附件服务器了，放到本地，定时清除
                string url = await FileHelper.SyncFile(excelPath, excelFileName, true,"local");
                
                #endregion

                //excel文件保存的相对路径，提供前端下载
                return resp.success(url);
            }
            catch (Exception ex)
            {
                return resp.ret(-1, $"error:{ex.Message},{ex.StackTrace}");
            }
            //return resp;
        }


        /// <summary>
        /// 通用导出模板(返回字符串)
        /// </summary>
        /// <typeparam name="T">实体类型，类型中属性必须添加[Description("name")]特性，其中name需要标记导出excel二级标题栏名称</typeparam>
        /// <param name="filename">导出文件名称（方法内已集成时间后缀，无需重复加时间后缀）</param>
        /// <param name="sheetname">导出excel一级标题名称（方法内已集成时间后缀，无需重复加时间后缀）</param>
        /// <param name="list">要导出的数据集合</param>
        /// <param name="save_path">服务器保存路径（方法内自动在本路径下创建../upfile/当前时间/xxx.xls）</param>
        /// <returns></returns>
        public async Task<string> ExcelDataExportTemplateStr<T>(string filename, string sheetname, List<T> list, string save_path) where T : new()
        {

            //Response resp = new Response();
            try
            {
                Type t_type = new T().GetType();//反射获取实体类型

                var workbook = new HSSFWorkbook();
                //var sheet = workbook.CreateSheet(sheetname);
                string _sheetName = sheetname;
                if (string.IsNullOrEmpty(_sheetName) || _sheetName.Length > 31)
                    _sheetName = "汇总表_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var sheet = workbook.CreateSheet(_sheetName);
                sheet.ForceFormulaRecalculation = true;//TODO:是否开始Excel导出后公式仍然有效（非必须）

                //创建一级标题行
                var row = NPOIUtil._.CreateRow(sheet, 0, 28);
                var cell = row.CreateCell(0);


                #region 二级标题行

                row = NPOIUtil._.CreateRow(sheet, 1, 24);//创建第二行
                //二级标题样式
                var secondTitleStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 15, true, 700, "楷体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Grey25Percent.Index, HSSFColor.Black.Index, FontUnderlineType.None, FontSuperScript.None, false);

                //标题从0开始添加单元格内容
                int column_index = 0;//列索引
                NPOIUtil._.CreateCells(row, secondTitleStyleFont, column_index, "序号");
                foreach (var propeties in t_type.GetProperties())
                {
                    //循环该属性所有特性  找出Description特性，加载到excel二级标题列
                    foreach (var attr in propeties.CustomAttributes)
                    {
                        if (attr.AttributeType.Name == "DescriptionAttribute")//判断某个特性时，需要在特性后面加上Attribute（即 想要获取‘Description’特性，判断时需要判断是否等于‘DescriptionAttribute’）
                        {
                            column_index++;
                            NPOIUtil._.CreateCells(row, secondTitleStyleFont, column_index, attr.ConstructorArguments[0].Value.ToString());
                            break;
                        }
                    }
                }
                #endregion

                #region 一级标题行，写在二级标题下面是因为合并单元格时需要用到包含Description特性的属性数量
                //合并单元格 例： 第1行到第2行 第3列到第4列围成的矩形区域
                //TODO:关于Excel行列单元格合并问题
                /**
                  第一个参数：从第几行开始合并
                  第二个参数：到第几行结束合并
                  第三个参数：从第几列开始合并
                  第四个参数：到第几列结束合并
                **/
                //合并单元格  column_index 单元格数量
                CellRangeAddress region = new CellRangeAddress(0, 0, 0, column_index);
                sheet.AddMergedRegion(region);//将一级标题行加入到当前sheet中
                cell.SetCellValue($"{filename}（{DateTime.Now.ToString("yyyy-MM-dd")}）");//设置一级标题内容
                var firstTitleStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 20, true, 700, "楷体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Coral.Index, HSSFColor.White.Index, FontUnderlineType.None, FontSuperScript.None, false);//标题样式
                cell.CellStyle = firstTitleStyleFont;//设置一级标题样式 

                #endregion

                #region 数据内容
                //单元格内容样式
                var dataStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 10, true, 0);

                //获取数据内容--属性值
                int line_index = 0;
                foreach (var model in list)
                {
                    Type type = model.GetType();
                    row = NPOIUtil._.CreateRow(sheet, line_index + 2, 20);//创建行，+2是因为需要跳过一级标题行和二级标题行
                    line_index++;
                    NPOIUtil._.CreateCells(row, dataStyleFont, 0, line_index.ToString());//列元素-序号
                    int index = 1;//列元素在单元格的位置，应该从0开始，第0列是‘序号’所以从1开始
                    foreach (var property in type.GetProperties())
                    {
                        foreach (var attr in property.CustomAttributes)
                        {
                            if (attr.AttributeType.Name == "DescriptionAttribute")//只取出标记为Description特性的属性值
                            {
                                object obj_value = property.GetValue(model, null);
                                string value = "";
                                if (obj_value != null)
                                {
                                    if (obj_value.GetType() == typeof(DateTime))
                                    {
                                        value = Convert.ToDateTime(obj_value).ToString("yyyy-MM-dd HH:mm:ss");
                                    }
                                    else
                                    {
                                        value = obj_value.ToString();
                                    }
                                }
                                NPOIUtil._.CreateCells(row, dataStyleFont, index, value);//列元素-属性值
                                index++;
                                break;
                            }
                        }
                    }
                }
                #endregion

                #region 保存到本地服务器
                string folder = $"guangdong/export/{DateTime.Now.ToString("yyyyMM")}";
                //保存文件到静态资源文件夹中（wwwroot）,使用绝对路径
                //var uploadPath = save_path + "/upfile/" + folder + "/";
                string uploadPath = $"{save_path}/upfile/{folder}";
                //excel保存文件名
                string excelFileName = $"{filename}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls";
                //创建目录文件夹
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                //Excel的路径及名称
                string excelPath = uploadPath + excelFileName;
                using (var fileStream = new FileStream(excelPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    //向Excel文件对象写入文件流，生成Excel文件
                    workbook.Write(fileStream);
                }
                #endregion

                #region 同步至附件服务器                
                string url = await FileHelper.SyncFile(excelPath, excelFileName, true);

                #endregion

                //excel文件保存的相对路径，提供前端下载
                return url;
            }
            catch (Exception ex)
            {
                return resp.ret(-1, $"error:{ex.Message},{ex.StackTrace}");
            }
            //return resp;
        }

        /// <summary>
        /// 航天创新专用汇总报送导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        /// <param name="list"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public async Task<dynamic> ExcelDataExportForYoungSV(string filename, string sheetname, List<Lib.ApplySummaryForExcelModel> list, string save_path, bool syncTocloud = false)
        {
            try
            {
                Type t_type = new Lib.ApplySummaryForExcelModel().GetType();//反射获取实体类型

                var workbook = new HSSFWorkbook();
                string _sheetName = sheetname;
                if (string.IsNullOrEmpty(_sheetName) || _sheetName.Length > 31)
                    _sheetName = "汇总表_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var sheet = workbook.CreateSheet(_sheetName);
                sheet.ForceFormulaRecalculation = true;//TODO:是否开始Excel导出后公式仍然有效（非必须）

                //创建一级标题行
                var row = NPOIUtil._.CreateRow(sheet, 0, 28);
                var cell = row.CreateCell(0);


                #region 二级标题行

                row = NPOIUtil._.CreateRow(sheet, 1, 24);//创建第二行
                //二级标题样式
                var secondTitleStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 15, true, 700, "楷体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Grey25Percent.Index, HSSFColor.Black.Index, FontUnderlineType.None, FontSuperScript.None, false);

                //标题从0开始添加单元格内容
                int column_index = 0;//列索引
                NPOIUtil._.CreateCells(row, secondTitleStyleFont, column_index, "序号");
                foreach (var propeties in t_type.GetProperties())
                {
                    //循环该属性所有特性  找出Description特性，加载到excel二级标题列
                    foreach (var attr in propeties.CustomAttributes)
                    {
                        if (attr.AttributeType.Name == "DescriptionAttribute")//判断某个特性时，需要在特性后面加上Attribute（即 想要获取‘Description’特性，判断时需要判断是否等于‘DescriptionAttribute’）
                        {
                            column_index++;
                            NPOIUtil._.CreateCells(row, secondTitleStyleFont, column_index, attr.ConstructorArguments[0].Value.ToString());
                            break;
                        }
                    }

                }
                #endregion

                #region 一级标题行，写在二级标题下面是因为合并单元格时需要用到包含Description特性的属性数量
                //合并单元格 例： 第1行到第2行 第3列到第4列围成的矩形区域
                //TODO:关于Excel行列单元格合并问题
                /**
                  第一个参数：从第几行开始合并
                  第二个参数：到第几行结束合并
                  第三个参数：从第几列开始合并
                  第四个参数：到第几列结束合并
                **/
                //合并单元格  column_index 单元格数量
                CellRangeAddress region = new CellRangeAddress(0, 0, 0, column_index);
                sheet.AddMergedRegion(region);//将一级标题行加入到当前sheet中
                //cell.SetCellValue($"{list[0].eventName}{filename}（{DateTime.Now.ToString("yyyy-MM-dd")}）");//设置一级标题内容
                cell.SetCellValue($"{filename}（{DateTime.Now.ToString("yyyy-MM-dd")}）");//设置一级标题内容
                var firstTitleStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 20, true, 700, "楷体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Coral.Index, HSSFColor.White.Index, FontUnderlineType.None, FontSuperScript.None, false);//标题样式
                cell.CellStyle = firstTitleStyleFont;//设置一级标题样式 
                #endregion

                #region 数据内容
                //单元格内容样式
                var dataStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 10, true, 0);
                dataStyleFont.WrapText = true;
                //获取数据内容--属性值
                int line_index = 1;
                int mergeinRowIndex = 2;
                //string tmpTopicName = "";
                string tmpProjectNo = "";
                if (list.Count > 0)
                {
                    //tmpTopicName = list[0].topicName;
                    tmpProjectNo = list[0].projectNo;
                    //注意，最后加这一条很重要，且一定要加在判定合并列的字段上，也就是赛队编号上，否则最后合并的时候，最后几条是没办法合并的
                    list.Add(new Lib.ApplySummaryForExcelModel()
                    {
                        projectNo = "总计：" + list.Count + "条记录（报名人员）"
                    });
                }
                foreach (var model in list)
                {
                    Type type = model.GetType();
                    row = NPOIUtil._.CreateRow(sheet, line_index+1, 20);//创建行，+1是因为需要跳过一级标题行和二级标题行

                    NPOIUtil._.CreateCells(row, dataStyleFont, 0, line_index.ToString());//列元素-序号
                    int index = 1;//列元素在单元格的位置，应该从0开始，第0列是‘序号’所以从1开始
                    int mergeColumnIndex1_start = 1;//第一次合并列的标识(因为是从序列所在列后边的第一列就开始合并，所以这个start就设定成1即可)
                    int mergeColumnIndex1_end = 1;//第一次合并列的标识
                    int mergeColumnIndex2_start = 1;//第二次合并列的标识
                    int mergeColumnIndex2_end = 1;//第二次合并列的标识  这里设定两次合并是因为，整个表格的列顺序，按照需求，前后有两个部分是需要合并的，中间部分都是独立的
                    foreach (var property in type.GetProperties())
                    {
                        foreach (var attr in property.CustomAttributes)
                        {
                            if (attr.AttributeType.Name == "DescriptionAttribute")//只取出标记为Description特性的属性值
                            {
                                object obj_value = property.GetValue(model, null);
                                string value = "";
                                if (obj_value != null)
                                {
                                    if (obj_value.GetType() == typeof(DateTime))
                                    {
                                        value = Convert.ToDateTime(obj_value).ToString("yyyy-MM-dd HH:mm:ss");
                                    }
                                    else
                                    {
                                        value = obj_value.ToString();
                                    }
                                    value = string.IsNullOrEmpty(value) ? value : value.Replace("+86", "").Trim();
                                }
                                NPOIUtil._.CreateCells(row, dataStyleFont, index, value);//列元素-属性值
                                index++;
                                //指定合并的列标识，其实和结束，第一次组合并因为是从序号列后面开始，所以就设定成1即可
                                if (property.Name.ToLower() == "projectintro")
                                {
                                    mergeColumnIndex1_end = index;
                                }
                                if (property.Name.ToLower() == "teachernames")
                                    mergeColumnIndex2_start = index - 1;
                                else if (property.Name.ToLower() == "teacheremails")
                                    mergeColumnIndex2_end = index;
                                break;
                            }
                        }
                    }
                    //string currTopicName = model.topicName;
                    string currProjectNo = model.projectNo;
                    if (currProjectNo != tmpProjectNo)
                    {
                        for (int i = mergeColumnIndex1_start; i < mergeColumnIndex1_end; i++)
                        {
                            if (line_index > mergeinRowIndex)
                            {
                                sheet.AddMergedRegion(new CellRangeAddress(mergeinRowIndex, line_index, i, i));
                            }
                            else
                                break;
                        }
                        for (int j = mergeColumnIndex2_start; j < mergeColumnIndex2_end; j++)
                        {
                            if (line_index > mergeinRowIndex)
                            {
                                sheet.AddMergedRegion(new CellRangeAddress(mergeinRowIndex, line_index, j, j));
                            }
                            else
                                break;
                        }
                        tmpProjectNo = currProjectNo;
                        mergeinRowIndex = line_index + 1;                       
                    }
                    line_index++;
                    
                }
                #endregion

                #region 保存到本地服务器
                string folder = $"guangdong/export/{DateTime.Now.ToString("yyyyMM")}";
                //保存文件到静态资源文件夹中（wwwroot）,使用绝对路径
                //var uploadPath = save_path + "/upfile/" + folder + "/";
                string uploadPath = $"{save_path}/upfile/{folder}";
                //excel保存文件名
                //string excelFileName = $"{list[0].eventName}{filename}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls";
                string excelFileName = $"{filename}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls";
                //创建目录文件夹
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                //Excel的路径及名称
                string excelPath = uploadPath + excelFileName;
                using (var fileStream = new FileStream(excelPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    //向Excel文件对象写入文件流，生成Excel文件
                    workbook.Write(fileStream);
                }
                #endregion

                #region 同步至附件服务器
                //string remoteBase = ConfigurationHelper.GetSectionValue("remoteBase");
                //bool flag = FileHelper.connectState();
                //string finalUrl = "";
                //if (flag)
                //{
                //    //共享文件夹的目录
                //    DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                //    //获取保存文件的路径
                //    string PathName = theFolder.ToString() + $"{folder}\\";
                //    //上传文件到附件服务器，同时删掉本地文件节省服务器空间
                //    await FileHelper.Transport(excelPath, PathName, excelFileName, true);

                //    if (syncTocloud)
                //    {
                //        finalUrl = BceHelper.UploadFileToBosSingle(excelFileName, FileHelper.getFileData(PathName + excelFileName));
                //    }
                //    else
                //    {
                //        finalUrl = ConfigurationHelper.GetSectionValue("resourceHost") + "/" + folder + "/" + excelFileName;
                //    }
                //}
                string finalUrl = await FileHelper.SyncFile(excelPath, excelFileName, true);


                #endregion

                //excel文件保存的相对路径，提供前端下载
                return resp.success(finalUrl);
            }
            catch (Exception ex)
            {
                return resp.ret(-1, $"error:{ex.Message},{ex.StackTrace}");
            }
            //return resp;
        }
        
        /// <summary>
        /// 航天创新专用汇总报送导出（小表格，后续转成pdf）
        /// </summary>
        /// <param name="title"></param>
        /// <param name="subtitle"></param>
        /// <param name="programTypeCaption"></param>
        /// <param name="sheetname"></param>
        /// <param name="list"></param>
        /// <param name="save_path"></param>
        /// <returns></returns>
        public async Task<string> ExcelDataExportForYoungSVForPdf(string title, string subtitle, string programTypeCaption, string sheetname, List<Lib.ApplySummaryForPdfModel> list, string save_path)
        {
            try
            {
                Type t_type = new Lib.ApplySummaryForPdfModel().GetType();//反射获取实体类型

                var workbook = new HSSFWorkbook();
                string _sheetName = sheetname;
                if (string.IsNullOrEmpty(_sheetName) || _sheetName.Length > 31)
                    _sheetName = "汇总表_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var sheet = workbook.CreateSheet(_sheetName);
                sheet.ForceFormulaRecalculation = true;//TODO:是否开始Excel导出后公式仍然有效（非必须）

                //创建一级标题行
                var row = NPOIUtil._.CreateRow(sheet, 0, 80);
                var cell = row.CreateCell(0);
                var row2 = NPOIUtil._.CreateRow(sheet, 1, 30);
                var cell2 = row2.CreateCell(0);

                #region 二，三级标题行  
                //三级标题
                row = NPOIUtil._.CreateRow(sheet, 2, 24);//创建第三行（列标题行）
                //样式
                var thirdTitleStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 15, true, 700, "楷体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Grey25Percent.Index, HSSFColor.Black.Index, FontUnderlineType.None, FontSuperScript.None, false);
                //标题从0开始添加单元格内容
                int column_index = 0;//列索引
                NPOIUtil._.CreateCells(row, thirdTitleStyleFont, column_index, "序号");
                foreach (var propeties in t_type.GetProperties())
                {
                    //循环该属性所有特性  找出Description特性，加载到excel二级标题列
                    foreach (var attr in propeties.CustomAttributes)
                    {
                        if (attr.AttributeType.Name == "DescriptionAttribute")//判断某个特性时，需要在特性后面加上Attribute（即 想要获取‘Description’特性，判断时需要判断是否等于‘DescriptionAttribute’）
                        {
                            column_index++;

                            NPOIUtil._.CreateCells(row, thirdTitleStyleFont, column_index, attr.ConstructorArguments[0].Value.ToString());
                            break;
                        }
                    }

                }
                #endregion

                #region 一二级标题行，写在三级标题下面是因为合并单元格时需要用到包含Description特性的属性数量
                //合并单元格 例： 第1行到第2行 第3列到第4列围成的矩形区域
                //TODO:关于Excel行列单元格合并问题
                /**
                  第一个参数：从第几行开始合并
                  第二个参数：到第几行结束合并
                  第三个参数：从第几列开始合并
                  第四个参数：到第几列结束合并
                **/
                //合并单元格  column_index 单元格数量
                CellRangeAddress region = new CellRangeAddress(0, 0, 0, column_index);
                sheet.AddMergedRegion(region);//将一级标题行加入到当前sheet中
                cell.SetCellValue($"{title}\r\n{subtitle}");//设置一级标题内容
                var firstTitleStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 18, false, 700, "楷体", false, false, false, true, FillPattern.SolidForeground, HSSFColor.Aqua.Index, HSSFColor.White.Index, FontUnderlineType.None, FontSuperScript.None, false);//标题样式
                cell.CellStyle = firstTitleStyleFont;//设置一级标题样式 
                cell.CellStyle.WrapText = true;

                //二级标题
                CellRangeAddress regionsub = new CellRangeAddress(1, 1, 0, column_index);
                sheet.AddMergedRegion(regionsub);//将二级标题行加入到当前sheet中
                cell2.SetCellValue($"赛项(类别):{programTypeCaption}");//设置二级标题内容
                var secondTitleStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Left, VerticalAlignment.Center, 12, false, 450, "楷体", false, false, false, true, FillPattern.SolidForeground, HSSFColor.Grey25Percent.Index, HSSFColor.Black.Index, FontUnderlineType.None, FontSuperScript.None, false);
                cell2.CellStyle = secondTitleStyleFont;//设置二级标题样式 
                #endregion

                #region 数据内容
                //单元格内容样式
                var dataStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 12, true, 420);
                dataStyleFont.WrapText = true;
                //获取数据内容--属性值
                int line_index = 3;
                int mergeinRowIndex = 3;
                string tmpTopicName = "";
                if (list.Count > 0)
                    tmpTopicName = list[0].topicName;
                foreach (var model in list)
                {
                    Type type = model.GetType();
                    row = NPOIUtil._.CreateRow(sheet, line_index, 50);//创建行，+3是因为需要跳过标题行

                    NPOIUtil._.CreateCells(row, dataStyleFont, 0, (line_index - 2).ToString());//列元素-序号
                    int index = 1;//列元素在单元格的位置，应该从0开始，第0列是‘序号’所以从1开始
                    int mergeinColumnIndex = 1;
                    foreach (var property in type.GetProperties())
                    {
                        foreach (var attr in property.CustomAttributes)
                        {
                            if (attr.AttributeType.Name == "DescriptionAttribute")//只取出标记为Description特性的属性值
                            {
                                object obj_value = property.GetValue(model, null);
                                string value = "";
                                if (obj_value != null)
                                {
                                    if (obj_value.GetType() == typeof(DateTime))
                                    {
                                        value = Convert.ToDateTime(obj_value).ToString("yyyy-MM-dd HH:mm:ss");
                                    }
                                    else
                                    {
                                        value = obj_value.ToString();
                                    }
                                    value = string.IsNullOrEmpty(value) ? value : value.Replace("+86", "").Trim();

                                }
                                NPOIUtil._.CreateCells(row, dataStyleFont, index, value);//列元素-属性值
                                index++;
                                if (property.Name.ToLower() == "teachernames")
                                    mergeinColumnIndex = index;
                                break;
                            }
                        }
                    }
                    string currTopicName = model.topicName;
                    if (currTopicName != tmpTopicName)
                    {
                        for (int i = 1; i < mergeinColumnIndex; i++)
                        {
                            if (line_index > mergeinRowIndex)
                            {
                                sheet.AddMergedRegion(new CellRangeAddress(mergeinRowIndex, line_index, i, i));
                            }
                            else
                                break;
                        }
                        currTopicName = tmpTopicName;
                        mergeinRowIndex = line_index + 1;
                    }
                    line_index++;
                }

                sheet.SetColumnWidth(1, 15 * 256);
                sheet.SetColumnWidth(2, 12 * 256);
                sheet.SetColumnWidth(3, 25 * 256);
                sheet.SetColumnWidth(4, 12 * 256);
                sheet.SetColumnWidth(5, 20 * 256);
                sheet.SetColumnWidth(6, 12 * 256);
                sheet.SetColumnWidth(7, 15 * 256);
                sheet.SetColumnWidth(8, 30 * 256);

                #endregion

                #region 保存到本地服务器                
                string folder = $"magicexam/export/{DateTime.Now.ToString("yyyyMM")}/";
                //保存文件到静态资源文件夹中（wwwroot）,使用绝对路径
                //var uploadPath = save_path + "/upfile/" + folder + "/";
                string uploadPath = $"{save_path}/upfile/{folder}";
                //excel保存文件名
                string excelFileName = $"{title}-{programTypeCaption}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls";
                //创建目录文件夹
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                //Excel的路径及名称
                string excelPath = uploadPath + excelFileName;
                using (var fileStream = new FileStream(excelPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    //向Excel文件对象写入文件流，生成Excel文件
                    workbook.Write(fileStream);
                }
                #endregion

                #region 同步至附件服务器
                string remoteBase = ConfigurationHelper.GetSectionValue("remoteBase");
                string remoteUser = ConfigurationHelper.GetSectionValue("remoteUser");
                string remotePwd = ConfigurationHelper.GetSectionValue("remotePwd");
                bool flag = FileHelper.connectState(remoteBase, remoteUser, remotePwd);

                if (flag)
                {
                    //共享文件夹的目录
                    DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                    //获取保存文件的路径
                    string PathName = $"{theFolder.ToString()}{folder}\\";
                    if (!string.IsNullOrEmpty(ConfigurationHelper.GetSectionValue("resourceDir")))
                    {
                        PathName = $"{theFolder.ToString()}{ConfigurationHelper.GetSectionValue("resourceDir")}\\{folder}\\";
                    }
                    //上传文件到附件服务器(注意这个没有删除本地文件)
                    await FileHelper.Transport(excelPath, PathName, excelFileName, false);
                }
                string url = ConfigurationHelper.GetSectionValue("resourceHost")+ ConfigurationHelper.GetSectionValue("resourceDir") + "/" + folder + "/" + excelFileName;
                #endregion

                //excel文件保存的相对路径，提供前端下载
                return excelPath;
            }
            catch (Exception ex)
            {
                return $"error:{ex.Message},{ex.StackTrace}";
            }
            //return resp;
        }
        
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
        public HSSFWorkbook ExcelDataExportTemplate2<T>(string sheetname, List<T> list, string img = "", HSSFWorkbook workbook = null) where T : new()
        {
            try
            {
                Type t_type = new T().GetType();//反射获取实体类型
                if (workbook == null)
                {
                    workbook = new HSSFWorkbook();
                }
               
                string _sheetName = sheetname;
                if (string.IsNullOrEmpty(_sheetName) || _sheetName.Length > 31)
                    _sheetName = "汇总表_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var sheet = workbook.CreateSheet(_sheetName);
                sheet.ForceFormulaRecalculation = true;//TODO:是否开始Excel导出后公式仍然有效（非必须）

                //创建一级标题行
                var row = NPOIUtil._.CreateRow(sheet, 0, 28);
                var cell = row.CreateCell(0);


                #region 二级标题行

                row = NPOIUtil._.CreateRow(sheet, 1, 24);//创建第二行
                //二级标题样式
                var secondTitleStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 15, true, 700, "楷体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Grey25Percent.Index, HSSFColor.Black.Index, FontUnderlineType.None, FontSuperScript.None, false);

                //标题从0开始添加单元格内容
                int column_index = 0;//列索引
                NPOIUtil._.CreateCells(row, secondTitleStyleFont, column_index, "序号");
                foreach (var propeties in t_type.GetProperties())
                {
                    //循环该属性所有特性  找出Description特性，加载到excel二级标题列
                    foreach (var attr in propeties.CustomAttributes)
                    {
                        if (attr.AttributeType.Name == "DescriptionAttribute")//判断某个特性时，需要在特性后面加上Attribute（即 想要获取‘Description’特性，判断时需要判断是否等于‘DescriptionAttribute’）
                        {
                            column_index++;
                            NPOIUtil._.CreateCells(row, secondTitleStyleFont, column_index, attr.ConstructorArguments[0].Value.ToString());
                            break;
                        }
                    }
                }
                #endregion

                #region 一级标题行，写在二级标题下面是因为合并单元格时需要用到包含Description特性的属性数量
                //合并单元格 例： 第1行到第2行 第3列到第4列围成的矩形区域
                //TODO:关于Excel行列单元格合并问题
                /**
                  第一个参数：从第几行开始合并
                  第二个参数：到第几行结束合并
                  第三个参数：从第几列开始合并
                  第四个参数：到第几列结束合并
                **/
                //合并单元格  column_index 单元格数量
                CellRangeAddress region = new CellRangeAddress(0, 0, 0, column_index);
                sheet.AddMergedRegion(region);//将一级标题行加入到当前sheet中
                cell.SetCellValue($"{sheetname}（{DateTime.Now.ToString("yyyy-MM-dd")}）");//设置一级标题内容
                var firstTitleStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 20, true, 700, "楷体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Coral.Index, HSSFColor.White.Index, FontUnderlineType.None, FontSuperScript.None, false);//标题样式
                cell.CellStyle = firstTitleStyleFont;//设置一级标题样式 
                #endregion

                #region 数据内容
                //单元格内容样式
                var dataStyleFont = NPOIUtil._.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 10, true, 400);

                //获取数据内容--属性值
                int line_index = 0;
                foreach (var model in list)
                {
                    Type type = model.GetType();
                    row = NPOIUtil._.CreateRow(sheet, line_index + 2, 20);//创建行，+2是因为需要跳过一级标题行和二级标题行
                    line_index++;
                    NPOIUtil._.CreateCells(row, dataStyleFont, 0, line_index.ToString());//列元素-序号
                    int index = 1;//列元素在单元格的位置，应该从0开始，第0列是‘序号’所以从1开始
                    foreach (var property in type.GetProperties())
                    {
                        foreach (var attr in property.CustomAttributes)
                        {
                            if (attr.AttributeType.Name == "DescriptionAttribute")//只取出标记为Description特性的属性值
                            {
                                object obj_value = property.GetValue(model, null);
                                string value = "";
                                if (obj_value != null)
                                {
                                    if (obj_value.GetType() == typeof(DateTime))
                                    {
                                        value = Convert.ToDateTime(obj_value).ToString("yyyy-MM-dd HH:mm:ss");
                                    }
                                    else
                                    {
                                        value = obj_value.ToString();
                                    }
                                }
                                NPOIUtil._.CreateCells(row, dataStyleFont, index, value);//列元素-属性值
                                index++;
                                break;
                            }
                        }
                    }
                }
                #endregion

                if (!string.IsNullOrEmpty(img))
                {
                    CreatePircture(workbook, sheet, img, column_index);
                }
            }
            catch (Exception ex)
            {
            }
            return workbook;
        }


        public async Task<string> DocDateExportForYoungSV(string savePath, string title, string eventName, string fieldName, List<Lib.ApplySummaryForPdfModel> list)
        {
            try
            {
                string folder = $"magicexam/export/{DateTime.Now.ToString("yyyyMM")}/";
                //保存文件到静态资源文件夹中（wwwroot）,使用绝对路径
                //var uploadPath = save_path + "/upfile/" + folder + "/";
                string uploadPath = $"{savePath}/upfile/{folder}";
                //excel保存文件名
                string docFileName = $"{title}-{eventName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.doc";
                //创建目录文件夹
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                //word的路径及名称
                string docPath = uploadPath + docFileName;
                using (var fileStream = new FileStream(docPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    XWPFDocument myDoc = new XWPFDocument();

                    CT_SectPr pr = new CT_SectPr();
                    //上边的这一个横向的A4的页面大小，这里的单位比较特殊，用的是缇（Twip）这是一种和屏幕无关的长度单位，目的是为了让应用程序元素输出到不同设备时都能保持一致的计算方式。
                    //换算关系：1英寸 = 1440缇,1厘米 = 567缇,1磅 = 20缇,1像素 = 15缇
                    //常用页面尺寸：（单位Twip）
                    //A4（纵向）：W = 11906     H = 16838
                    //A4（纵向）：W = 16838     H = 11906
                    //A5 ： W = 8390    H = 11906
                    //A6 ： W = 5953    H = 8390
                    pr.pgSz.w = 16838;
                    pr.pgSz.h = 11906;
                    myDoc.Document.body.sectPr = pr;//设置页面尺寸
                    //标题
                    myDoc.SetParagraph(ParagraphInstanceSetting(myDoc, title, true, 20, "宋体", ParagraphAlignment.CENTER), 0);
                    //第一行内容
                    myDoc.SetParagraph(ParagraphInstanceSetting(myDoc, $"赛项（类别）：{fieldName}-{eventName}", true, 14, "宋体", ParagraphAlignment.LEFT), 1);

                    #region 插入表格
                    XWPFTable table = myDoc.CreateTable(list.Count+1, 9);                    
                    //table.Width = 5200;
                    //table.SetColumnWidth(0, 300);
                    //table.SetColumnWidth(1, 500);
                    //table.SetColumnWidth(2, 500);
                    //table.SetColumnWidth(3, 800);
                    //table.SetColumnWidth(4, 600);
                    //table.SetColumnWidth(5, 800);
                    //table.SetColumnWidth(6, 400);
                    //table.SetColumnWidth(7, 600);
                    //table.SetColumnWidth(8, 800);
                    
                    Type t_type = new Lib.ApplySummaryForPdfModel().GetType();//反射获取实体类型
                    int column_index = 0;
                    table.GetRow(0).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(myDoc, table, "序号", ParagraphAlignment.CENTER, 24, true,14));
                    foreach (var propeties in t_type.GetProperties())
                    {
                        //循环该属性所有特性  找出Description特性，加载到excel二级标题列
                        foreach (var attr in propeties.CustomAttributes)
                        {
                            if (attr.AttributeType.Name == "DescriptionAttribute")//判断某个特性时，需要在特性后面加上Attribute（即 想要获取‘Description’特性，判断时需要判断是否等于‘DescriptionAttribute’）
                            {
                                column_index++;
                                CT_TcPr mPr= table.GetRow(0).GetCell(column_index).GetCTTc().AddNewTcPr();
                                mPr.tcW = new CT_TblWidth();
                                mPr.tcW.w = "600";
                                mPr.tcW.type = ST_TblWidth.dxa;
                                table.GetRow(0).GetCell(column_index).SetParagraph(SetTableParagraphInstanceSetting(myDoc, table, attr.ConstructorArguments[0].Value.ToString(), ParagraphAlignment.CENTER, 24, true, 14));

                                break;
                            }
                        }
                    }
                    int rowIndex = 1;
                    foreach (var model in list)
                    {
                        Type type = model.GetType();                        
                        int columnIndex = 1;
                        table.GetRow(rowIndex).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(myDoc, table, rowIndex.ToString(), ParagraphAlignment.CENTER));

                        foreach (var property in type.GetProperties())
                        {
                            foreach (var attr in property.CustomAttributes)
                            {
                                object objValue = property.GetValue(model, null);
                                string value = "";
                                if (objValue != null)
                                {
                                    value = string.IsNullOrEmpty(objValue.ToString()) ? objValue.ToString() : objValue.ToString().Replace("+86", "").Trim();
                                }
                                table.GetRow(rowIndex).GetCell(columnIndex).SetParagraph(SetTableParagraphInstanceSetting(myDoc, table, value, ParagraphAlignment.CENTER, 24, false, 12));
                                columnIndex++;
                            }
                        }
                        rowIndex++;
                    }
                    #endregion
                    myDoc.Write(fileStream);
                    #region 同步至附件服务器
                    string remoteBase = ConfigurationHelper.GetSectionValue("remoteBase");
                    string remoteUser = ConfigurationHelper.GetSectionValue("remoteUser");
                    string remotePwd = ConfigurationHelper.GetSectionValue("remotePwd");
                    bool flag = FileHelper.connectState(remoteBase, remoteUser, remotePwd);

                    if (flag)
                    {
                        //共享文件夹的目录
                        DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                        //获取保存文件的路径
                        string PathName = theFolder.ToString() + $"{folder.Replace("/","\\")}";
                        if (!string.IsNullOrEmpty(ConfigurationHelper.GetSectionValue("resourceDir")))
                        {
                            PathName = $"{theFolder.ToString()}{ConfigurationHelper.GetSectionValue("resourceDir")}\\{folder}\\";
                        }
                        //上传文件到附件服务器(注意这个没有删除本地文件)
                        await FileHelper.Transport(docPath, PathName, docFileName, false);
                    }
                    string url = ConfigurationHelper.GetSectionValue("resourceHost")+ ConfigurationHelper.GetSectionValue("resourceDir") + "/" + folder + "/" + docFileName;
                    #endregion
                    return docPath;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"上传至附件服务器失败,{ex.Message}");
                return $"error:{ex.Message}";
            }
        }

        /// <summary>
        /// 创建word文档中的段落对象和设置段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary>
        /// <param name="document">document文档对象</param>
        /// <param name="fillContent">段落第一个文本对象填充的内容</param>
        /// <param name="isBold">是否加粗</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="paragraphAlign">段落排列（左对齐，居中，右对齐）</param>
        /// <param name="isStatement">是否在同一段落创建第二个文本对象（解决同一段落里面需要填充两个或者多个文本值的情况，多个文本需要自己拓展，现在最多支持两个）</param>
        /// <param name="secondFillContent">第二次声明的文本对象填充的内容，样式与第一次的一致</param>
        /// <param name="fontColor">字体颜色--十六进制</param>
        /// <param name="isItalic">是否设置斜体（字体倾斜）</param>
        /// <returns></returns>
        public XWPFParagraph ParagraphInstanceSetting(XWPFDocument document, string fillContent, bool isBold, int fontSize, string fontFamily, ParagraphAlignment paragraphAlign, bool isStatement = false, string secondFillContent = "", string fontColor = "000000", bool isItalic = false)
        {
            XWPFParagraph paragraph = document.CreateParagraph();//创建段落对象
            paragraph.Alignment = paragraphAlign;//文字显示位置,段落排列（左对齐，居中，右对齐）


            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
            xwpfRun.IsBold = isBold;//文字加粗
            xwpfRun.SetText(fillContent);//填充内容
            xwpfRun.FontSize = fontSize;//设置文字大小
            xwpfRun.IsItalic = isItalic;//是否设置斜体（字体倾斜）
            xwpfRun.SetColor(fontColor);//设置字体颜色--十六进制
            xwpfRun.SetFontFamily(fontFamily, FontCharRange.None); //设置标题样式如：（微软雅黑，隶书，楷体）根据自己的需求而定

            if (!isStatement) return paragraph;

            XWPFRun secondXwpfRun = paragraph.CreateRun();//创建段落文本对象
            secondXwpfRun.IsBold = isBold;//文字加粗
            secondXwpfRun.SetText(secondFillContent);//填充内容
            secondXwpfRun.FontSize = fontSize;//设置文字大小
            secondXwpfRun.IsItalic = isItalic;//是否设置斜体（字体倾斜）
            secondXwpfRun.SetColor(fontColor);//设置字体颜色--十六进制
            secondXwpfRun.SetFontFamily(fontFamily, FontCharRange.None); //设置标题样式如：（微软雅黑，隶书，楷体）根据自己的需求而定


            return paragraph;
        }

        /// <summary> 
        /// 创建Word文档中表格段落实例和设置表格段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary> 
        /// <param name="document">document文档对象</param> 
        /// <param name="table">表格对象</param> 
        /// <param name="fillContent">要填充的文字</param> 
        /// <param name="paragraphAlign">段落排列（左对齐，居中，右对齐）</param>
        /// <param name="textPosition">设置文本位置（设置两行之间的行间,从而实现表格文字垂直居中的效果），从而实现table的高度设置效果 </param>
        /// <param name="isBold">是否加粗（true加粗，false不加粗）</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontColor">字体颜色--十六进制</param>
        /// <param name="isItalic">是否设置斜体（字体倾斜）</param>
        /// <returns></returns> 
        public XWPFParagraph SetTableParagraphInstanceSetting(XWPFDocument document, XWPFTable table, string fillContent, ParagraphAlignment paragraphAlign, int textPosition = 24, bool isBold = false, int fontSize = 10, string fontColor = "000000", bool isItalic = false)
        {
            var para = new CT_P();
            //设置单元格文本对齐
            para.AddNewPPr().AddNewTextAlignment();

            XWPFParagraph paragraph = new XWPFParagraph(para, table.Body);//创建表格中的段落对象
            paragraph.Alignment = paragraphAlign;//文字显示位置,段落排列（左对齐，居中，右对齐）
                                                 //paragraph.FontAlignment =Convert.ToInt32(ParagraphAlignment.CENTER);//字体在单元格内显示位置与 paragraph.Alignment效果相似

            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
            xwpfRun.SetText(fillContent);
            xwpfRun.FontSize = fontSize;//字体大小
            xwpfRun.SetColor(fontColor);//设置字体颜色--十六进制
            xwpfRun.IsItalic = isItalic;//是否设置斜体（字体倾斜）
            xwpfRun.IsBold = isBold;//是否加粗
            xwpfRun.SetFontFamily("宋体", FontCharRange.None);//设置字体（如：微软雅黑,华文楷体,宋体）
            xwpfRun.TextPosition = textPosition;//设置文本位置（设置两行之间的行间），从而实现table的高度设置效果
            return paragraph;
        }


        /// <summary>
        /// 保存excel
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="save_path">服务器保存路径（方法内自动在本路径下创建../upfile/当前时间/xxx.xls）</param>
        /// <returns></returns>
        public async Task<dynamic> SaveExcel(HSSFWorkbook workbook, string save_path)
        {
            //Response resp = new Response();
            try
            {
                #region 保存到本地服务器
                //string folder = DateTime.Now.ToString("yyyyMM");
                string folder = $"/export/{DateTime.Now.ToString("yyyyMM")}";
                

                //保存文件到静态资源文件夹中（wwwroot）,使用绝对路径
                var uploadPath = save_path + "\\upfile\\" + folder + "\\";
                //excel保存文件名
                string excelFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                //创建目录文件夹
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                //Excel的路径及名称
                string excelPath = uploadPath + excelFileName;
                using (var fileStream = new FileStream(excelPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    //向Excel文件对象写入文件流，生成Excel文件
                    workbook.Write(fileStream);
                }
                #endregion

                #region 同步至附件服务器
                string remoteBase = ConfigurationHelper.GetSectionValue("remoteBase");
                string remoteUser = ConfigurationHelper.GetSectionValue("remoteUser");
                string remotePwd = ConfigurationHelper.GetSectionValue("remotePwd");
                bool flag = FileHelper.connectState(remoteBase, remoteUser, remotePwd);

                if (flag)
                {
                    //共享文件夹的目录
                    DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                    //获取保存文件的路径
                    string PathName = $"{theFolder.ToString()}{folder}\\";
                    if (!string.IsNullOrEmpty(ConfigurationHelper.GetSectionValue("resourceDir")))
                    {
                        PathName = $"{theFolder.ToString()}{ConfigurationHelper.GetSectionValue("resourceDir")}\\{folder}\\";
                    }
                        //上传文件到附件服务器，同时删掉本地文件节省服务器空间
                    await FileHelper.Transport(excelPath, PathName, excelFileName, true);
                }
                string url = ConfigurationHelper.GetSectionValue("resourceHost")+ ConfigurationHelper.GetSectionValue("resourceDir") + "/" + folder + "/" + excelFileName;
                #endregion
                //excel文件保存的相对路径，提供前端下载
                //resp.code = 1;
                //resp.data = url;
                return resp.success(url);
            }
            catch (Exception ex)
            {
                //resp.code = -1;
                //resp.message = $"error:{ex.Message},{ex.StackTrace}";
                return resp.ret(-1, $"error:{ex.Message},{ex.StackTrace}");
            }
            //return resp;
        }


        /// <summary>
        /// 往excel中插入图片，导出文本时从（横向 0,11）或（纵向17,0）开始，因为图片占用空间是10,17
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="img">img_base64</param>
        /// <param name="path">导出路径，如不需要单独导出图片不传此参数</param>
        /// <returns></returns>
        public HSSFWorkbook CreatePircture(HSSFWorkbook workbook, ISheet sheet, string img, int column_index, string path = "")
        {
            if (workbook == null)
            {
                workbook = new HSSFWorkbook();
            }
            if (img.Split(',').Length == 2)
            {
                byte[] bytes = Convert.FromBase64String(img.Split(',')[1]);
                int pictureIdx = workbook.AddPicture(bytes, NPOI.SS.UserModel.PictureType.JPEG);

                //create sheet
                if (sheet == null)
                {
                    sheet = workbook.CreateSheet("Sheet1");
                }

                // Create the drawing patriarch.  This is the top level container for all shapes. 
                var patriarch = sheet.CreateDrawingPatriarch();

                //CellRangeAddress region = new CellRangeAddress(0, 0, 0, column_index);
                //sheet.AddMergedRegion(region);//将一级标题行加入到当前sheet中

                //add a picture
                //0,0,0,0,x轴起始单元格，y轴起始单元格，x轴结束单元格，y轴结束单元格
                HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, column_index + 1, 0, 10 + column_index + 1, 17);
                //HSSFClientAnchor anchor = new HSSFClientAnchor();
                var pict = patriarch.CreatePicture(anchor, pictureIdx);
                //图片占用空间：x轴占用单元格数量，y轴占用单元格数量
                //pict.Resize(10, 17);

                if (!string.IsNullOrEmpty(path))
                {
                    using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        //向Excel文件对象写入文件流，生成Excel文件
                        workbook.Write(fileStream);
                    }
                }

            }
            return workbook;
        }

        public async Task<string> SaveFile(string filePath, string fileName, bool cloudSync = false)
        {
            string storageType = ConfigurationHelper.GetSectionValue("storageType");

            //string folder = DateTime.Now.ToString("yyyyMM");
            string folder = $"guangdong/export/{DateTime.Now.ToString("yyyyMM")}";

            string remoteBase = ConfigurationHelper.GetSectionValue("remoteBase");
            bool flag = FileHelper.connectState();
            string url = "";
            if (flag)
            {
                //共享文件夹的目录
                DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                //获取保存文件的路径
                string PathName = theFolder.ToString() + $"{folder}\\";
                //上传文件到附件服务器，同时删掉本地文件节省服务器空间
                await FileHelper.Transport(filePath, PathName, fileName, true);
                if (cloudSync)
                    url = BceHelper.UploadFileToBosSingle(fileName, FileHelper.getFileData(PathName + fileName));
                else
                    url = ConfigurationHelper.GetSectionValue("resourceHost") + ConfigurationHelper.GetSectionValue("resourceDir") + "/" + folder + "/" + fileName;
            }

            return url;
        }

    }

}
