using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Turn.Net
{
    class MSDEGraph
    {
        Document document;
        PdfWriter writer;
        PdfContentByte cb;

        string Page = "A4";
        string Orient = "Land";
        bool   DrawRect = true;
        string FilePath = "";
        string FileName = "";
        string Title = "";
        float PageX = 21F;
        float PageY = 29.7F;
        float Minx = 9000F;
        float Maxx = -9000F;
        float Miny = 9000F;
        float Maxy = -9000F;
        float ut   = 72F / 2.54F;     // unit for cm
        float TextAngle = 0F;
        float AxisLenth = 14F;
        float scale = 100F;

        private Point op;

        public MSDEGraph(string orient, string filepath, string filename, string title)
        {
            document = new Document();

            op.X = 0F;
            op.Y = 0F;

            if (orient == "Land")
            {
                //PageX = 29.2F;
                //PageY = 20.5F;
                document.SetPageSize(PageSize.A4.Rotate());
                PageX = document.PageSize.Width / ut ;
                PageY = document.PageSize.Height / ut ;
            }
            else
            {
                Orient = "Port";
                //PageX = 20.5F;
                //PageY = 29.2F;
                document.SetPageSize(PageSize.A4);
                PageX = document.PageSize.Width / ut;
                PageY = document.PageSize.Height / ut;
            }

            //PageX = document.PageSize.Width  / ut - 0.5F;
            //PageY = document.PageSize.Height / ut - 0.5F;

            string pdfname = filepath + "\\" + filename + ".pdf";

            FileStream pdffs = new FileStream(pdfname, FileMode.Create, FileAccess.Write, FileShare.None);

            writer = PdfWriter.GetInstance(document, pdffs);

            document.Open();

            cb = writer.DirectContent;

            Title = title;

        }
        public Document DOCUMENT
        {
            get { return document; }
        }
        public string ORIENT
        {
            get { return Orient; }
            set
            {
                if (value == "Land")
                {
                    PageX = 29.7F;
                    PageY = 21F;
                    document.SetPageSize(PageSize.A4.Rotate());
                }
                else
                {
                    Orient = "Port";
                    PageX = 21F;
                    PageY = 29.7F;
                    document.SetPageSize(PageSize.A4);
                }
            }
        }
        public Point ORIGIN
        {
            get { return op; }
            set { op = value; }
        }
        public float SCALE {
            get { return AxisLenth / scale; }
            set { scale = AxisLenth / value; }
        }
        public float SC
        {
            get { return  scale; }
        }

        public float AXISLENTH
        {
            get { return AxisLenth; }
            set { AxisLenth = value; }
        }

        public bool drawRect(float width, float margin)
        {
            try
            {
                cb.SetLineWidth(width);  // Draw Rectanel Box
                cb.Rectangle(margin * ut, margin * ut, (PageX - margin*2) * ut, (PageY - margin*2) * ut);
                cb.Stroke();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool drawARect(float width, float x, float y, float dx, float dy)
        {
            try
            {
                cb.SetLineWidth(width);  // Draw Rectanel Box
                cb.Rectangle(x * ut, y * ut, dx * ut, dy * ut);
                cb.Stroke();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool drawAxis(string dir)
        {
            float intv = SCALE / 5, w = 0.1F;
            int min = 50;
            string lintype = "";
            try
            {
                float x = 10F * ut;
                float y = 10F * ut;
                if (SCALE > 250)
                    min = 100;
                if (SCALE > 500)
                    min = 200;
                int si = 1;
                if (dir == "PORT")
                    si = -1;

                for (int i = 0; i < 7; i++)
                {
                    int interv = Convert.ToInt32((intv * i)) - min;
                    if (i == 0 || i == 1 || i == 6)
                    {
                        w = 1.5F;
                        if (i == 1) w = 0.1F;
                        lintype = "Solid";
                    }
                    else
                    {
                        w = 0.1F;
                        lintype = "Dotted";
                    }
                    DrawLine(scale * i * intv * si, scale * 0, scale * i * intv * si, scale * 5 * intv, w, BaseColor.BLACK, lintype);                          // y axis  | | | |

                    TextWrite(interv.ToString(), scale * i * intv * si, -0.5F, 10, 0, BaseColor.BLUE);
                    if (i < 6)
                    {
                        if (i == 5)
                        {
                            w = 1.5F;
                            lintype = "Solid";
                        }
                        DrawLine((scale * 0 * intv), (scale * i * intv), (scale * 6 * intv * si), (scale * i * intv), w, BaseColor.BLACK, lintype);            // Hor - - - -
                        TextWrite(interv.ToString(), -0.5F*si, (scale * i * intv), 10, 0, BaseColor.BLUE);
                    }
                }
                ORIGIN = new Point(op.X + (scale * 1 * intv * ut * si), op.Y + (scale * 1 * intv * ut));
                DrawCircle(0, 0, 5);

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        public bool drawAxis2(string direction)
        {
            // direction 
            float intv = SCALE/5, w=0.1F;
            int   min  = 50;
            string lintype = "";
            try
            {
                float x = 10F * ut;
                float y = 10F * ut;
                if (SCALE > 250)
                    min = 100;
                if (SCALE > 500)
                    min = 200;
                if ( direction == "PORT")
                {

                    for ( int i = 0; i < 7; i ++)  
                    {
                        int interv = Convert.ToInt32((intv * i)) - min;
                        if (i == 0 || i == 1 || i == 6)
                        {
                            w = 1.5F;
                            if (i == 1) w = 0.1F;
                            lintype = "Solid";
                        }
                        else
                        {
                            w = 0.1F;
                            lintype = "Dotted";
                        }
                        DrawLine(scale * -i* intv , scale * 0, scale * -i * intv, scale * 5 * intv, w, BaseColor.BLACK, lintype);                          // y axis  | | | |

                        TextWrite(interv.ToString(), scale * -i * intv , -0.5F , 10, 0, BaseColor.BLUE);
                        if ( i < 6 )
                        {
                            if (i == 5)
                            {
                                w = 1.5F;
                                lintype = "Solid";
                            }
                            DrawLine((scale * 0 * intv), (scale * i * intv), (scale * 6 * -intv), (scale * i * intv), w, BaseColor.BLACK, lintype); // Hor - - - -
                            TextWrite(interv.ToString(), 0.5F,  (scale * i * intv),10, 0, BaseColor.BLUE);
                        }
                    }
                    ORIGIN = new Point(op.X + (-scale * 1 * intv*ut),  op.Y + (scale * 1 * intv * ut));
                    DrawCircle(0, 0, 5);
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {

                        int interv = Convert.ToInt32((intv * i)) - min;
                        if (i == 0 || i == 1 || i == 6)
                        {
                            w = 1.5F;
                            if (i == 1) w = 0.1F;
                            lintype = "Solid";
                        }
                        else
                        {
                            w = 0.1F;
                            lintype = "Dotted";
                        }
                        DrawLine(scale * i * intv, scale * 0, scale * i * intv, scale * 5 * intv, w, BaseColor.BLACK, lintype);        // y axis  | | | |                       
                        TextWrite(interv.ToString(), scale * i * intv , -0.5F, 10, 0, BaseColor.BLUE);
                        if (i < 6)
                        {
                            if (i == 5)
                            {
                                w = 1.5F;
                                lintype = "Solid";
                            }
                            DrawLine((scale * 0 * intv), (scale * i * intv), (scale * 6 * intv), (scale * i * intv), w, BaseColor.BLACK, lintype); // Hor - - - -
                            TextWrite(interv.ToString(), -0.8F, (scale * i * intv), 10, 0, BaseColor.BLUE);
                        }
                    }
                    DrawCircle((scale * 1 * intv), (scale * 1 * intv), 5);
                    ORIGIN = new Point(op.X + (scale * 1 * intv * ut), op.Y + (scale * 1 * intv * ut));
                }

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool CloseDocument()
        {
            document.Close();
            return true;
        }
        public void TextWrite(String text, float x, float y, float fs, int angle, BaseColor color)
        {
            BaseFont bf = BaseFont.CreateFont(@"C:\Windows\Fonts\malgun.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            cb.SaveState();
            cb.BeginText();
            cb.SetFontAndSize(bf, fs);
            cb.SetColorFill(color);
            cb.ShowTextAligned(Element.ALIGN_CENTER, text, ((float)op.X + x* ut), ((float)op.Y+y*ut), angle);
            cb.EndText();           
            cb.Stroke();
            cb.RestoreState();
        }
        public  void TextSymbol(String text, float x, float y, float fs, int angle, BaseColor color)
        {
            BaseFont bf = BaseFont.CreateFont(@"C:\Windows\Fonts\WINGDNG3.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);         
            cb.SaveState();
            cb.BeginText();
            cb.SetFontAndSize(bf, fs);
            cb.SetColorFill(color);
            cb.ShowTextAligned(Element.ALIGN_CENTER, text, ((float)op.X + x * ut) , ((float)op.Y + y * ut) , angle);
            cb.EndText();
            cb.Stroke();
            cb.RestoreState();
        }
        public void DrawArrow(float x, float y, float fs, int angle, BaseColor color)
        {
            //BaseFont bf = BaseFont.CreateFont(@"C:\Windows\Fonts\WINGDNG3.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            BaseFont bf = BaseFont.CreateFont(BaseFont.ZAPFDINGBATS, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);

            cb.SaveState();
            cb.BeginText();
            cb.SetFontAndSize(bf, fs);
            cb.SetColorFill(color);
            cb.ShowTextAligned(Element.ALIGN_CENTER|Element.ALIGN_MIDDLE, ((char)220).ToString(), ((float)op.X + x * ut), ((float)op.Y + y * ut), angle);
            cb.EndText();
            cb.Stroke();
            cb.RestoreState();
        }
        public void DrawLine(float x1, float y1, float x2, float y2, float w, BaseColor color, string type)
        {
            cb.SaveState();
            cb.SetLineWidth(w);
            if (type == "Dotted")
                cb.SetLineDash(3, 3);
            cb.SetColorStroke(color);
            cb.MoveTo( (float)op.X + x1 * ut , (float)op.Y + y1 * ut );       // Y Coordinate
            cb.LineTo( (float)op.X + x2 * ut , (float)op.Y + y2 * ut );
            cb.Stroke();
            cb.RestoreState();
        }

        public void DrawDashedLine(float x1, float y1, float x2, float y2, float w)
        {
            cb.SaveState();
            cb.SetLineWidth(w);
            //cb.SetLineDash(6, 0);
            cb.SetLineDash(6, 0);
            cb.MoveTo(((float)op.X + x1 * ut), ((float)op.Y + y1 * ut));       // Y Coordinate
            cb.LineTo(((float)op.X + x2 * ut), ((float)op.Y + y2 * ut));           
            cb.Stroke();
            cb.RestoreState();
        }
        private void DrawRLine(float x1, float y1, float x2, float y2, float w)
        {
            cb.SaveState();
            cb.SetLineWidth(w);
            cb.MoveTo(((float)op.X + x1 * ut) ,      ((float)op.Y + y1 * ut) );       // Y Coordinate
            cb.LineTo(((float)op.X + x1 + x2 * ut) , ((float)op.Y + y1 + y2 * ut) );
            cb.Stroke();
            cb.RestoreState();
        }
        public void DrawCircle(float x1, float y1,  float size)
        {
            cb.SaveState();
            cb.SetLineWidth(0.1F);
            cb.Circle(((float)op.X + x1 * ut), ((float)op.Y + y1 * ut), size);       // Y Coordinate
            cb.Stroke();
            cb.RestoreState();
        }

        public void FillStrokeTest()
        {
            //cb.SetColorStroke(new CMYKColor(1f, 0f, 0f, 0f));
            //cb.SetColorFill(new CMYKColor(0f, 0f, 1f, 0f));
            cb.SetColorStroke(BaseColor.CYAN);
            cb.SetColorFill(BaseColor.YELLOW);

            cb.MoveTo(70, 200);
            cb.LineTo(170, 200);
            cb.LineTo(170, 300);
            cb.LineTo(70, 300);
            cb.ClosePathStroke();
            //Path closed and stroked


            cb.MoveTo(190, 200);
            cb.LineTo(290, 200);
            cb.LineTo(290, 300);
            cb.LineTo(190, 300);
            cb.Fill();
            //Filled, but not stroked or closed


            cb.MoveTo(310, 200);
            cb.LineTo(410, 200);
            cb.LineTo(410, 300);
            cb.LineTo(310, 300);
            cb.FillStroke();
            //Filled, stroked, but path not closed


            cb.MoveTo(430, 200);
            cb.LineTo(530, 200);
            cb.LineTo(530, 300);
            cb.LineTo(430, 300);
            cb.ClosePathFillStroke();
            //Path closed, stroked and filled


        }
    }
}
