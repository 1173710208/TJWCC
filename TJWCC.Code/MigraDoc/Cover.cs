using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Code.MigraDoc
{
    public class Cover
    {
        /// <summary>
        /// 文档封面
        /// </summary>
        /// <param name="document"></param>
        public static void DefineCover(Document document)
        {
            var section = document.AddSection();

            var paragraph = section.AddParagraph();
            paragraph.Format.SpaceAfter = "3cm";

            var image = section.AddImage(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"/Content/img/Logolandscape.png");
            image.Width = "10cm";

            paragraph = section.AddParagraph("由天津水务集团辅助调度管理平台生成");
            paragraph.Format.Font.Size = 16;
            paragraph.Format.Font.Color = Colors.DarkRed;
            paragraph.Format.SpaceBefore = "8cm";
            paragraph.Format.SpaceAfter = "3cm";

            paragraph = section.AddParagraph("生成时间: ");
            paragraph.AddDateField();
        }
    }
}
