using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant
{
    public class FileHelper
    {
        static string remoteBase = ConfigurationHelper.GetSectionValue("remoteBase");
        public static bool connectState()
        {
            return connectState(ConfigurationHelper.GetSectionValue("remoteBase"), ConfigurationHelper.GetSectionValue("remoteUser"), ConfigurationHelper.GetSectionValue("remotePwd"));
        }

        public static bool connectState(string path)
        {
            return connectState(path, ConfigurationHelper.GetSectionValue("remoteUser"), ConfigurationHelper.GetSectionValue("remotePwd"));
        }

        /// <summary>
        /// 连接远程共享文件夹
        /// </summary>
        /// <param name="path">远程共享文件夹的路径</param>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public static bool connectState(string path, string userName, string passWord)
        {
            bool Flag = false;
            #region 先试一下网络连接建立了没有,避免频繁建立连接，导致1219错误
            try
            {
                if (Directory.Exists(path))
                    return true;
            }
            catch (Exception ex)
            {
                Logger.Error("网络链接未建立：" + ex.Message);
            }
            #endregion

            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                //string[] parts = path.Split("\\");
                //if (parts.Length > 4)
                //    path = $"\\\\{parts[2]}\\{parts[3]}";
                //string dosLine_pre = "net use * /del /y";
                //proc.StandardInput.WriteLine(dosLine_pre);
                //Logger.Debug(dosLine_pre);
                //这里user参数后面的“file\”是个任意内容，为了避免出现1312错误，完整例子为net use \\10.185.4.151\AI_Attach EvRlwo78 /user:file\IUSR_Castic /PERSISTENT:YES
                string dosLine = "net use " + path + " " + passWord + " /user:aifile\\" + userName + " /PERSISTENT:YES";
                proc.StandardInput.WriteLine(dosLine);
                Logger.Debug(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
                else
                {
                    Logger.Error("文件上传到远程服务器出错：" + errormsg);
                    throw new Exception(errormsg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }

        public static void Transport(byte[] buf, string dst, string fileName)
        {
            if (!Directory.Exists(dst))
            {
                Directory.CreateDirectory(dst);
            }
            dst = dst + fileName;
            if (!File.Exists(dst))
            {
                using (FileStream outFileStream = new FileStream(dst, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    int byteCount = buf.Length;
                    outFileStream.Write(buf, 0, byteCount);
                }
            }
        }

        /// <summary>
        /// 向远程文件夹保存本地内容，或者从远程文件夹下载文件到本地
        /// </summary>
        /// <param name="src">要保存的文件的路径，如果保存文件到共享文件夹，这个路径就是本地文件路径如：@"D:\1.avi"</param>
        /// <param name="dst">保存文件的路径，不含名称及扩展名</param>
        /// <param name="fileName">保存文件的名称以及扩展名</param>
        public static async Task Transport(string src, string dst, string fileName, bool isDel = false, bool isCover = true)
        {

            if (!Directory.Exists(dst))
            {
                Directory.CreateDirectory(dst);
            }
            dst = dst + fileName;
            if (File.Exists(dst) && isCover)
                File.Delete(dst);
            if (!File.Exists(dst))
            {
                FileStream inFileStream = new FileStream(src, FileMode.Open);
                FileStream outFileStream = new FileStream(dst, FileMode.Create, FileAccess.Write);
                byte[] buf = new byte[inFileStream.Length];
                int byteCount;
                while ((byteCount = await inFileStream.ReadAsync(buf, 0, buf.Length)) > 0)
                {
                    await outFileStream.WriteAsync(buf, 0, byteCount);
                }
                inFileStream.Flush();
                inFileStream.Close();
                outFileStream.Flush();
                outFileStream.Close();
                if (isDel)
                {
                    File.Delete(src);//移动到附件服务器后，就把视频源文件删掉，保留服务器空间

                }
            }
        }


        /// <summary>
        /// 从远程服务器下载文件到本地
        /// </summary>
        /// <param name="src">下载到本地后的文件路径，包含文件的扩展名</param>
        /// <param name="target_file">远程服务器路径（共享文件夹路径）</param>
        public static bool TransportRemoteToLocal(string src, string target_file)   //src：下载到本地后的文件路径  dst：远程服务器路径 fileName:远程服务器dst路径下的文件名
        {
            if (!File.Exists(target_file))
                return false;

            FileStream inFileStream = new FileStream(target_file, FileMode.Open);    //远程服务器文件  此处假定远程服务器共享文件夹下确实包含本文件，否则程序报错
            FileStream outFileStream = new FileStream(src, FileMode.OpenOrCreate);   //从远程服务器下载到本地的文件
            byte[] buf = new byte[inFileStream.Length];
            int byteCount;
            while ((byteCount = inFileStream.Read(buf, 0, buf.Length)) > 0)
            {
                outFileStream.Write(buf, 0, byteCount);
            }
            inFileStream.Flush();
            inFileStream.Close();
            outFileStream.Flush();
            outFileStream.Close();
            return true;
        }

        /// <summary>
        /// 从远程服务器下载文件到本地
        /// </summary>
        /// <param name="src">下载到本地后的文件路径，包含文件的扩展名</param>
        /// <param name="target_file">远程服务器路径（共享文件夹路径）</param>
        public static async Task<bool> TransportRemoteToLocalAsync(string src, string target_file)   //src：下载到本地后的文件路径  dst：远程服务器路径 fileName:远程服务器dst路径下的文件名
        {
            if (!File.Exists(target_file))
                return false;

            FileStream inFileStream = new FileStream(target_file, FileMode.Open);    //远程服务器文件  此处假定远程服务器共享文件夹下确实包含本文件，否则程序报错
            FileStream outFileStream = new FileStream(src, FileMode.OpenOrCreate);   //从远程服务器下载到本地的文件
            byte[] buf = new byte[inFileStream.Length];
            int byteCount;
            while ((byteCount = await inFileStream.ReadAsync(buf, 0, buf.Length)) > 0)
            {
                await outFileStream.WriteAsync(buf, 0, byteCount);
            }
            await inFileStream.FlushAsync();
            await outFileStream.FlushAsync();
            await inFileStream.DisposeAsync();
            await outFileStream.DisposeAsync();
            return true;
        }



        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="srcPath"></param>
        public static void DelectDir(string srcPath)
        {
            try
            {
                if (Directory.Exists(srcPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(srcPath);
                    FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                    foreach (FileSystemInfo i in fileinfo)
                    {
                        if (i is DirectoryInfo)            //判断是否文件夹
                        {
                            DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                            subdir.Delete(true);//删除子目录和文件
                        }
                        else
                        {  //如果 使用了 streamreader 在删除前 必须先关闭流 ，否则无法删除 sr.close();
                            File.Delete(i.FullName);      //删除指定文件
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                Logger.Error($"删除文件夹失败,error:{e.Message}");
                throw;
            }
        }


        public static bool FileExist(string path)
        {
            bool flag = connectState();
            if (flag)
            {
                DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                string target_path = $"{theFolder}{path}";
                if (File.Exists(target_path))
                    return true;
            }
            return false;
        }

        public static bool FileRemove(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }

        public static Stream getFileDataRemote(string path)
        {
            bool flag = connectState();
            if (flag)
            {
                DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                string target_path = $"{theFolder}{path}";
                if (File.Exists(target_path))
                {
                    FileInfo fi = new FileInfo(target_path);
                    return fi.OpenRead();
                }
            }
            return null;
        }
        public static Stream getFileData(string target_path)
        {
            if (File.Exists(target_path))
            {
                FileInfo fi = new FileInfo(target_path);
                return fi.OpenRead();
            }
            return null;
        }
        public static string getFilePath(string path)
        {
            bool flag = connectState();
            if (flag)
            {
                DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                return $"{theFolder}{path}";
            }
            return "";
        }
        public static byte[] getFileByte(string target_path)
        {
            if (File.Exists(target_path))
            {
                FileInfo fi = new FileInfo(target_path);
                Stream source = new FileStream(target_path, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[source.Length];
                source.Read(buffer, 0, (int)source.Length);
                source.Close();
                source.Dispose();
                return buffer;
            }
            return null;
        }
        private void CopyFile(string save_file, string path)
        {
            FileInfo fi = new FileInfo(save_file);
            string target = ConfigurationHelper.GetSectionValue("FileAttach") + $"\\{DateTime.Now.ToString("yyyyMM")}\\";
            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);
            string target_file = target + path;
            fi.CopyTo(target_file, true);
        }
        /// <summary>
        /// 本地拷贝
        /// </summary>
        /// <param name="save_file"></param>
        /// <param name="path"></param>
        public static void CopyFileLocal(string save_file, string file_name)
        {
            if (string.IsNullOrEmpty(Path.GetDirectoryName(file_name)))
            {
                string targetPath = ConfigurationHelper.GetSectionValue("virtualBase") + $"\\{DateTime.Now.ToString("yyyyMM")}\\";
                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);
                file_name = targetPath + file_name;
            }
            File.Copy(save_file, file_name);
        }

        public static async Task<string> CopyFileAsync(string save_file, string fileName, bool isDel = false)
        {
            bool flag = connectState();
            if (flag)
            {
                //共享文件夹的目录
                DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
                //获取保存文件的路径
                string PathName = "";
                string PathDate = $"{DateTime.Now.ToString("yyyyMM")}\\{Utils.GetCurrentWeekOfMonth(DateTime.Now)}";
                if (!string.IsNullOrEmpty(ConfigurationHelper.GetSectionValue("resourceDir")))
                {
                    PathName = $"{theFolder.ToString()}{ConfigurationHelper.GetSectionValue("resourceDir").TrimStart('/')}\\{PathDate}\\";
                }
                else
                    PathName = $"{theFolder.ToString()}{PathDate}\\";
                //string PathName =$"{theFolder.ToString()}{ConfigurationHelper.GetSectionValue("remoteFolder")}{DateTime.Now.ToString("yyyyMM")}\\";
                if (!string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
                {
                    PathName += $"{Path.GetDirectoryName(fileName)}\\";
                    //上传到附件服务器
                    await FileHelper.Transport(save_file, PathName, Path.GetFileName(fileName), isDel);
                    //return ConfigurationHelper.GetSectionValue("resourcehost") + "/" + DateTime.Now.ToString("yyyyMM") + "/" + Path.GetDirectoryName(fileName).Replace("\\", "/") + "/" + Path.GetFileName(fileName);
                    //return ConfigurationHelper.GetSectionValue("resourceDir") + "/" + PathDate.Replace("\\","/") + "/" + Path.GetDirectoryName(fileName).Replace("\\", "/") + "/" + Path.GetFileName(fileName);
                    return $"{ConfigurationHelper.GetSectionValue("resourceDir")}/{PathDate.Replace("\\", "/")}/{Path.GetDirectoryName(fileName).Replace("\\", "/")}/{Path.GetFileName(fileName)}";
                }
                else
                {
                    await FileHelper.Transport(save_file, PathName, fileName, isDel);
                    //return ConfigurationHelper.GetSectionValue("resourceDir") + "/" + DateTime.Now.ToString("yyyyMM") + "/" + fileName;
                    return $"{ConfigurationHelper.GetSectionValue("resourceDir")}/{PathDate.Replace("\\", "/")}/{fileName}";

                }
            }
            else
            {
                Logger.Error("远程连接建立失败");
                return "error";
            }
        }
        //public static string CopyFile(byte[] data, string fileName)
        //{

        //    bool flag = connectState();
        //    if (flag)
        //    {
        //        //共享文件夹的目录
        //        DirectoryInfo theFolder = new DirectoryInfo(remoteBase + "\\");
        //        //获取保存文件的路径
        //        //string PathName = $"{theFolder.ToString()}{DateTime.Now.ToString("yyyyMM")}\\";
        //        string PathName = "";
        //        string PathDate = $"{DateTime.Now.ToString("yyyyMM")}\\{Utils.GetCurrentWeekOfMonth(DateTime.Now)}";

        //        if (!string.IsNullOrEmpty(ConfigurationHelper.GetSectionValue("resourceDir")))
        //        {
        //            PathName = $"{theFolder.ToString()}{ConfigurationHelper.GetSectionValue("resourceDir").TrimStart('/')}\\{PathDate}\\";
        //        }
        //        else
        //            PathName = $"{theFolder.ToString()}{PathDate}\\";

        //        if (!string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
        //        {
        //            PathName = $"{theFolder.ToString()}{PathDate.Replace("\\", "/")}{Path.GetDirectoryName(fileName)}\\";
        //            fileName = Path.GetFileName(fileName);
        //        }
        //        //上传到附件服务器
        //        Transport(data, PathName, fileName);
        //        //return $"{ConfigurationHelper.GetSectionValue("resourcehost")}/{DateTime.Now.ToString("yyyyMM")}/{fileName}";
        //        return $"{ConfigurationHelper.GetSectionValue("resourceDir")}/{PathDate.Replace("\\","/")}/{fileName}";

        //    }
        //    else
        //    {
        //        Logger.Error("远程连接建立失败");
        //        return "远程连接建立失败";
        //    }
        //}

        /// <summary>
        /// 同步文件
        /// </summary>
        /// <param name="saveFile"></param>
        /// <param name="fileName"></param>
        /// <param name="isDel"></param>
        /// <returns></returns>
        public static async Task<string> SyncFile(string saveFile, string fileName = "", bool isDel = true, string storageType = "")
        {
            if (string.IsNullOrEmpty(storageType))
                storageType = ConfigurationHelper.GetSectionValue("storageType");
            if (storageType == "server")
            {
                return await CopyFileAsync(saveFile, fileName, isDel);
            }
            else
            {

                if (!saveFile.Contains("upfile"))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    File.Copy(saveFile, fileName, true);//注意，这个fileName需要是完整路径，且当storageType是local时，必须在upfile目录下
                    saveFile = fileName;
                }
                //这里有个问题，当存储类型为local时，要确保保存路径在wwwroot/upfile路径下！
                var parts = saveFile.Split("upfile");
                return $"/upfile{parts[1].Replace("\\\\", "/").Replace("\\", "/")}";
            }
        }

    }
}
