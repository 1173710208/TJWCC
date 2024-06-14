using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Code.MigraDoc
{
    public class Styles
    {
        /// <summary>
        /// 文档基础样式
        /// </summary>
        /// <param name="document"></param>
        public static void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            var style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "思源宋体";

            // Heading1 to Heading9 are predefined styles with an outline level. An outline level
            // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks) 
            // in PDF.

            style = document.Styles["Heading1"];
            style.Font.Name = "思源宋体";
            style.Font.Size = 16;
            //style.Font.Bold = true;
            style.Font.Color = Colors.DarkBlue;
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 6;
            // Set KeepWithNext for all headings to prevent headings from appearing all alone
            // at the bottom of a page. The other headings inherit this from Heading1.
            style.ParagraphFormat.KeepWithNext = true;

            style = document.Styles["Heading2"];
            style.Font.Name = "思源宋体";
            style.Font.Size = 16;
            //style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 3;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles["Heading3"];
            style.Font.Size = 12;
            //style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TextBox based on style Normal.
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            //TODO: Colors
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal.
            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;
        }

        /// <summary>
        /// 文档页眉页脚
        /// </summary>
        /// <param name="document"></param>
        public static void DefineContentSection(Document document)
        {
            var section = document.AddSection();
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            section.PageSetup.StartingNumber = 1;

            var header = section.Headers.Primary;
            header.AddParagraph("\tSAMBO");

            header = section.Headers.EvenPage;
            header.AddParagraph("\tSAMBO");

            // Create a paragraph with centered page number. See definition of style "Footer".
            var paragraph = new Paragraph();
            paragraph.AddTab();
            paragraph.AddPageField();

            // Add paragraph to footer for odd pages.
            section.Footers.Primary.Add(paragraph);
            // Add clone of paragraph to footer for odd pages. Cloning is necessary because an object must
            // not belong to more than one other object. If you forget cloning an exception is thrown.
            section.Footers.EvenPage.Add(paragraph.Clone());
        }
    }
}
