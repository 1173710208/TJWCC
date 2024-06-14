using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Code;

namespace TJWCC.Web.Areas.Elaborate.Controllers
{
    public class QualityPointController : ControllerBase
    {
        private BSM_Meter_InfoApp bmiApp = new BSM_Meter_InfoApp();
        private PIPEApp pipeApp = new PIPEApp();
        string qualityList = "";
        // 水质监测点优化布置: Elaborate/QualityPoint

        public ActionResult GetQualityPointList()
        {
            var result = bmiApp.GetPointList(3);
            return Content(result.ToJson());
        }
        public ActionResult CreateNewQualityPoint(int pCount)
        {
            var result = bmiApp.GetPointList(2);
            string sArguments = @"water_Quality.py";//这里是python的文件名字
            string strArr = pCount.ToString();
            RunPythonScript(sArguments, strArr, "-u");
            return Content(qualityList);
        }
        //调用python核心代码
        public void RunPythonScript(string sArgName, string teps, string args = "")
        {
            Process p = new Process();
            string path = @"" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + sArgName;// 获得python文件的绝对路径（将文件放在c#的debug文件夹中可以这样操作）
            path = @"D:\sambo\" + sArgName;//(因为我没放debug下，所以直接写的绝对路径,替换掉上面的路径了)
            p.StartInfo.FileName = @"C:\Users\Administrator\AppData\Local\Programs\Python\Python39\python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            string sArguments = path;
            sArguments += " " + teps;//传递参数
            sArguments += " " + args;
            p.StartInfo.Arguments = sArguments;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();
            p.BeginOutputReadLine();
            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            //Console.ReadLine();
            p.WaitForExit();
        }
        //输出打印的信息
        public void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                qualityList = e.Data.Substring(1, e.Data.Length - 2).Replace(" ","").Replace("'","");
            }
        }
        public ActionResult CreateQualityOrder(string ids)
        {
            TimeSpan time = new TimeSpan(DateTime.Now.Hour, 0, 0);
            return Success("下发成功！");
        }
        public ActionResult GetDownloadJson(string ids)
        {
            string path = pipeApp.GetDownload(ids);
            return Content(path);
        }
    }
}