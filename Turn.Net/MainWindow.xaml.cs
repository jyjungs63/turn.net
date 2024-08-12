using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.awt.geom;
using iTextSharp.text.pdf.parser;

namespace Turn.Net
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        static string filename = "";
        float[] rpm     = new float[200];
        float[] v       = new float[200];
        float[] ha      = new float[200];
        float[] x       = new float[200];
        float[] he      = new float[200];
        float[] xx      = new float[1000];
        float[] vv      = new float[1000];
        float[] hh      = new float[1000];
        float[] deriv   = new float[1000];
        float[] sumaarr = new float[1000];
        float[] sumtarr = new float[1000];

        float[] xvp = new float[1000];
        float[] yvp = new float[1000];
        float[] xvs = new float[1000];
        float[] yvs = new float[1000];
        float[] hhp = new float[1000];
        float[] hhs = new float[1000];
        int mmm = 0;


        int ijk=0;
        string ship, cond;
        float lbp = 999F, b = 999F, draft = 999F, cb = 999F;
        string line;
        ushort index = 0, ii = 0, ical = 0;
        int m,  ist = 0,  isg = 1, inarg , iis = 0, iip = 0;
        public MainWindow()
        {
            if ( DateTime.Compare(DateTime.Now, new DateTime(2023,12,31) ) < 0)
            {
                InitializeComponent();
            }       
            else
                Environment.Exit(0);
        }

        private void bntOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                richTextBox.AppendText(File.ReadAllText(openFileDialog.FileName));
                index = 0; ii = 0;
                ist = 0; isg = 1; ical = 0; inarg = 0;
            }

            filename = openFileDialog.FileName;

            StreamReader file = new StreamReader(openFileDialog.FileName);


            while ( (line = file.ReadLine()) != null )
            {
                var words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                switch (index)
                {
                    case 0:
                        ship = words.ToString();
                        break;
                    case 1:       
                        lbp   = float.Parse(words[0]);
                        b     = float.Parse(words[1]);
                        draft = float.Parse(words[2]);
                        cb    = float.Parse(words[3]);
                        break;
                    case 2:
                        mmm = (int)float.Parse(words[0]);
                        break;
                    case 3:
                        cond = line.ToString();
                        break;
                    case 4:
                        rpm[ii] = float.Parse(words[0]);
                        v[ii]   = float.Parse(words[1]);
                        ha[ii]  = float.Parse(words[2]);
                        ii++;
                        while ((line = file.ReadLine()) != null)
                        {
                            //string t = line.Replace('\t', ' ');
                            words = line.Replace('\t', ' ').Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (!line.Contains("9999."))
                            {
                                rpm[ii] = float.Parse(words[0]);
                                v[ii]   = float.Parse(words[1]);
                                ha[ii]  = float.Parse(words[2]);
                                ii++;
                            }
                            else
                            {
                                CalculateRunning();
                                ii = 0;
                                break;
                            }
                        }
                        break;
                    case 5:
                        cond = line.ToString();
                        break;
                    case 6:
                        if ( words.Length == 3)
                        {
                            rpm[ii] = float.Parse(words[0]);
                            v[ii] = float.Parse(words[1]);
                            ha[ii] = float.Parse(words[2]);
                            ii++;

                            while ((line = file.ReadLine()) != null)
                            {
                                words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (!line.Contains("9999."))
                                {
                                    rpm[ii] = float.Parse(words[0]);
                                    v[ii] = float.Parse(words[1]);
                                    ha[ii] = float.Parse(words[2]);
                                    ii++;
                                }
                                else
                                {
                                    CalculateRunning();
                                    ii = 0;
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
                index++;
            }

        }

        private void CalculateRunning ()
        {

            m = ii;
            Array.Clear(sumaarr, 0, sumaarr.Length);
            Array.Clear(sumtarr,0, sumtarr.Length);
            float dir = ha[m - 1] - ha[0];

            if (dir > 0.0)
            {
                ist = 1;
                isg = -1;
            }
            else
            {
                ist = 0;
                isg = 1;
            }

            for (int j = 0; j < m; j++)
            {
                x[j] = mmm * (j / 60F);
            }
            float TSP = 4F / 60F;
            int narg = (int)((x[m - 1] / TSP) + 1F);

            if (ist == 1)
            {
                ijk = 1;
                //MessageBox.Show("TURNING CIRCLE TO STARBOARD");
            }
            else
            {
                ijk = -1;
                //MessageBox.Show("TURNING CIRCLE TO PORT");
            }
            int ichk = 0, iq = 0;
            float vlogc = 0F, t360 = 0F;
            for (int i = 1; i < m; i++)
            {

                he[i] = ha[i] - ha[0];

                if (Math.Abs(he[i]) >= 360F || ichk == 1)
                {
                    continue;
                }
                else
                {
                    ichk = 1;
                    iq = i;
                    t360 = (x[i] - x[i - 1]) / (he[i] - he[i - 1]) * (-isg * 360F - he[i - 1]) + x[i - 1];
                    t360 = t360 * 60F;
                    vlogc = (v[i] - v[i - 1]) / (he[i] - he[i - 1]) * (-isg * 360F - he[i - 1]) + x[i - 1];
                }
            }

            float psidc = (he[Math.Min(m - 1, iq + 2)] - he[Math.Min(m - 3, iq - 1)]) / mmm / 4F;

            SPLINE(m, narg, TSP, x, v, ref xx, ref vv, ref deriv);
            SPLINE(m, narg, TSP, x, he, ref xx, ref hh, ref deriv);

            float XADV = 0F;
            float XTT = 0F;
            float SUMT = 0F;
            float SUMA = 0F;
            float SPF = 1852F / 3600F;
            float BETAC1 = 30F;
            float GGG = 0F;
            float OPCL;

            vlogc = vlogc * SPF;

            OPCL = 0.45F + (cb * b / draft - 2.0F) / 3F;
            if (Math.Abs(OPCL - 0.5) > 0.2) OPCL = 0.5F;

            while (true)
            {
                GGG = (OPCL * lbp * psidc * (float)Math.Cos(BETAC1 * 3.141592 / 180F) / vlogc / 57.3F);
                if (GGG > 1)
                {
                    BETAC1 = BETAC1 + 10;
                }
                else
                    break;
            }
            float BETAC2 = 0F, BETAC = 0F;

            while (true)
            {
                BETAC2 = (float)Math.Asin(GGG) * 180F / 3.141592F;
                if (Math.Abs(BETAC1 - BETAC2) < 0.1F)
                {
                    BETAC = BETAC1;
                    break;
                }
                else
                    BETAC1 = BETAC2;
            }

            int L11 = 0;
            int KKK = 0, MIN_A = 7419868, ISG = 0;
            float PREG = 0F, XTAA = 0F, XTA = 0F, FART, BETA, VM, G1, G2, PREA, PRET;

            for (int I = 1; I < narg; I++)
            {

                FART = xx[I] / t360 * 60F;
                if (FART >= 1F) FART = 1F;
                BETA = (float)BETAC * (float)Math.Sin(3.141592 / 2F * FART);

                VM = vv[I] * SPF;
                VM = VM / (float)Math.Cos(BETA * 3.141592 / 180F);
                G1 = hh[I];
                G2 = G1 - BETA;
                PREA = SUMA;
                PRET = SUMT;

                SUMA = VM * (float)Math.Cos(G2 * 3.141592F / 180F) * TSP * 60F + SUMA;
                SUMT = VM * (float)Math.Sin(G2 * 3.141592F / 180F) * TSP * 60F + SUMT;

                sumaarr[I] = SUMA;
                sumtarr[I] = SUMT;

                if (KKK == 1 || Math.Abs(G1) < 90F)
                {
                    if (L11 == 1 || Math.Abs(G1) < 180F)
                    {
                        if (MIN_A > SUMA) MIN_A = (int)SUMA;
                        PREG = G1;
                    }
                    else
                    {
                        L11 = 1;
                        XTAA = (SUMA - PREA) / (G1 - PREG) * (-180F * isg - PREG) + PREA;
                        XTT = (SUMT - PRET) / (G1 - PREG) * (-180F * isg - PREG) + PRET;
                    }
                }
                else
                {
                    KKK = 1;
                    XADV = (SUMA - PREA) / (G1 - PREG) * (-90F * isg - PREG) + PREA;
                    XTA = (SUMT - PRET) / (G1 - PREG) * (-90F * isg - PREG) + PRET;
                    MIN_A = (int)XADV;
                }
            }
            PrintResult(ist, narg, xx, hh, sumaarr, sumtarr, XADV, XTA, XTT, XTAA, ijk);
        }

        private bool PrintResult(int ist, int narg, float [] xx, float [] hh, float[] sumaarr, float[] sumtarr, float XADV, float XTA, float XTT, float XTAA, int IJK)
        {
            bool rt = true;
            string path = System.IO.Path.GetDirectoryName(filename);
            string fname = System.IO.Path.GetFileNameWithoutExtension(filename);
            string file = "STARBOARD";
    
            if (ist == 0)
                file = "PORT";
            string resultfile = path + "\\" + fname + "-" + file + ".TXT";

            try  {
                using (StreamWriter wt = new StreamWriter(resultfile))
                //using (StreamWriter wt = new StreamWriter("TUNNING-" + file+ ".TXT"))
                {
                    wt.WriteLine(string.Format("     1. TURNING CIRCLE TO {0}", file));
                    for ( int i = 0; i < narg; i++ )
                    {
                        wt.WriteLine(string.Format("{0,10} {1,10:N3} {2,10:N3} {3,10:N3} {4,10:N3}", i+1, xx[i], hh[i],  sumtarr[i], sumaarr[i]));
                    }

                    wt.WriteLine(string.Format("***"));
                    wt.WriteLine(string.Format("{0,10:N3}", XADV));
                    wt.WriteLine(string.Format("{0,10:N3}", (float)Math.Abs(XTA*IJK) ));
                    wt.WriteLine(string.Format("{0,10:N3}", (float)Math.Abs(XTT*IJK) ));
                    wt.WriteLine(string.Format("          (Y-COORDINATE)  = {0,10:N3} M", Math.Abs(XTAA)));
                }

                if (radioButton.IsChecked == true)
                    DrawTunningCircle(file, narg, hh, sumtarr, sumaarr);
                else
                    DrawLifeSaveCircle(file, narg, hh, sumtarr, sumaarr);
            }
            catch ( Exception ex)
            {
                rt = false;
            }

            return rt;
        }
        private bool DrawLifeSaveCircle(string file, int narg, float[] hh, float[] sumtarr, float[] sumaarr)
        {
            Dictionary<int, Point2D> shipangle = new Dictionary<int, Point2D>();

            int[] ship = new int[] {0, 9999, 90, -9999 };

            float cm = 72F / 2.54F;

            List<float> ax = sumtarr.ToList();
            List<float> ay = sumaarr.ToList();

            float minx, miny, maxx, maxy;

            minx = ax.Min();
            miny = ay.Min();
            maxx = ax.Max();
            maxy = ay.Max();

            float max = (Math.Abs(maxx) - Math.Abs(minx) > Math.Abs(maxy) - Math.Abs(miny) ? Math.Abs(maxx) - Math.Abs(minx) : Math.Abs(maxy) - Math.Abs(miny));

            string path  = System.IO.Path.GetDirectoryName(filename);
            string fname = System.IO.Path.GetFileNameWithoutExtension(filename);

            string pdfname = path + "\\" + fname + "-" + file + ".pdf";

            MSDEGraph g = new MSDEGraph("Port", path, fname + "-" + file, textBox.Text);

            g.AXISLENTH = 15;
            if ( max < 1000 )
                g.SCALE = 1000;
            else  if ( max < 1300)
                g.SCALE = 1250;

            g.drawRect(2F, 0.5F);

            int min = 200;
            if (g.SCALE > 250)
                min = 400;
            if (g.SCALE > 500)
                min = 600;

            float sc   = g.SC;
            float intv = g.SCALE / 5;

            for (int i = 1; i <= 8; i++)              
            {
                string lintype = "Dotted";
                int interv = Convert.ToInt32((intv * i)) - min;
                var color = BaseColor.BLUE;
                if (i == 3)
                {
                    color = BaseColor.BLACK;
                    lintype = "Solid";
                }
                if ( i <= 8 )
                {
                    g.DrawLine(3F, 3F * i, g.DOCUMENT.PageSize.Width / cm - (3F), 3F * i, 0.1F, color, lintype);
                    if ( i < 7)
                        g.TextWrite(interv.ToString(), sc * i * intv, 2.5F, 10, 0, BaseColor.BLUE);
                }

                g.DrawLine(3F*i, 3F ,  3F * i, 3F * 8, 0.1F, color, lintype);
                g.TextWrite(interv.ToString(), 2.5F , (sc * i * intv), 10, 0, BaseColor.BLUE);
            }

            g.drawARect(1F, 3F, 3F, 3F * 5, 3F * 7);

            float headx = 22F * cm;

            if ("PORT" != file)
                headx = 7F * cm;
            g.TextWrite(textBox.Text + " MAN OVERBOARD  " + file, g.DOCUMENT.PageSize.Width / 2F / 28.34F, g.DOCUMENT.PageSize.Height / 28.34F - 2, 20, 0, BaseColor.BLACK);

            g.ORIGIN = new System.Windows.Point( 9F*cm, 9F*cm );   // move to graphics orgin (0,0) 

            for (int i = 1; i < narg; i++)
            {
                float x1, x2, y1, y2;
                x1 = sumtarr[i - 1] * sc;
                x2 = sumtarr[i] * sc;
                y1 = sumaarr[i - 1] * sc;
                y2 = sumaarr[i] * sc;
                g.DrawLine(x1, y1, x2, y2, 0.5F, BaseColor.BLACK, "Solid");
            }


            float dif = 14;
            if (g.SCALE < 499)
            {
                dif = 7;
            }
            else if (g.SCALE > 500)
                dif = 28;

            foreach (var a in ship)
            {
                float x = 0F, y = 0F, xx = 0F, yy = 0F, angle;

                CalValGivenAngle(narg, a, hh, sumtarr, sumaarr, ref x, ref y);

                angle = 90 + a;
                xx = x;
                yy = y;

                switch (a)
                {
                    case 0:
                    case 9999:
                        x += dif;
                        break;
                    case 90:
                        if (file == "PORT")
                            y += dif;
                        else
                            y -= dif;
                        break;
                }
                int ang = 90;

                if (a == 90)
                {
                    ang = 180;
                }
                if ( a != -9999 )
                    g.TextSymbol("\u00B4", x * sc, y * sc, 30, ang, BaseColor.BLUE);

                if (a != 360)
                {
                    float minmax = minx;
                    if (a == 90)
                    {
                        if (file != "PORT")                   // Draw Guide Line 
                            minmax = maxx;

                        g.DrawLine(x * sc, yy * sc, (minmax + (minmax * 0.2F)) * sc, yy * sc, 0.1F, BaseColor.BLACK, "Solid");                              // horizontal line
                        g.DrawLine((minmax + (minmax * 0.15F)) * sc, 0 * sc, (minmax + (minmax * 0.15F)) * sc, yy * sc, 0.1F, BaseColor.BLACK, "Solid");    // horizontal line

                        g.DrawCircle((minmax + (minmax * 0.15F)) * sc, (yy) * sc, 1);
                        g.DrawCircle((minmax + (minmax * 0.15F)) * sc, 0, 1);

                        g.TextWrite("WAY AHEAD INCL, STERN TRACK = " + yy.ToString("F0") + " M", (minmax + (minmax * 0.15F)) * sc, yy * sc / 2F, 8, 90, BaseColor.BLUE);

                    }
                    else if ( a == 9999)
                    {
                        g.DrawLine(xx * sc, yy * sc, xx * sc, (yy + (yy * 0.2F)) * sc, 0.1F, BaseColor.BLACK, "Solid");                  // arrow head to y
                        g.DrawLine(0, (yy + (yy * 0.15F)) * sc, xx * sc, (yy + (yy * 0.15F)) * sc, 0.1F, BaseColor.BLACK, "Solid");    // horizontal line
                        g.DrawCircle(xx * sc, (yy + (yy * 0.15F)) * sc, 1);
                        g.DrawCircle(0, (yy + (yy * 0.15F)) * sc, 1);
                        g.TextWrite("TRANSVERSE DRIFT = " + Math.Abs(xx).ToString("F0") + " M", xx * sc / 2F, (yy + (yy * 0.15F)) * sc, 8, 0, BaseColor.BLUE);
                    }
                    else if(a == -9999)
                    {
                        g.DrawCircle(x * sc, 0 * sc, 1);
                        g.DrawCircle(0,  0 * sc , 1);

                        if (g.SCALE < 500)
                            g.TextWrite("PASS BY DISTANCE = ", 0 * sc, (-10F) * sc, 8, 0, BaseColor.BLUE);
                        else
                            g.TextWrite("PASS BY DISTANCE = " + Math.Abs(x).ToString(), 20F * sc, (y) * sc , 8, 0, BaseColor.BLUE);
                    }
                    else if (a == 0)
                    {
                        if (g.SCALE < 500)
                            g.TextWrite("BASE COURSE ", 0 * sc, (-10F) * sc, 8, 0, BaseColor.BLUE);
                        else
                            g.TextWrite("BASE COURSE ", 0 * sc, (-20F) * sc - 0.5F, 8, 0, BaseColor.BLUE);
                    }
                }
            }

            g.CloseDocument();

            Browser1.Navigate(pdfname);

            this.Title = pdfname;

            return true;
        }
        private bool DrawTunningCircle(string file, int narg, float [] hh,  float[] sumtar,  float[] sumaarr)
        {
            comboBox.IsEnabled = true;

            Dictionary<int, Point2D> shipangle = new Dictionary<int, Point2D>();


            int[] ship  = new int[] { 0, 90, 180, 270, 360 };

            float cm = 72F / 2.54F;

            string path  = System.IO.Path.GetDirectoryName(filename);
            string fname = System.IO.Path.GetFileNameWithoutExtension(filename);

            MSDEGraph graph = new MSDEGraph("Land", path, fname + "-" + file, textBox.Text);

            graph.AXISLENTH = 14;

            graph.drawRect(2F, 0.5F);

            string pdfname = path + "\\" + fname + "-" + file + ".pdf";

            List<float> ax = sumtar.ToList();
            List<float> ay = sumaarr.ToList();

            float minx, miny, maxx, maxy;

            minx = ax.Min();
            miny = ay.Min();
            maxx = ax.Max();
            maxy = ay.Max();

            float max = (Math.Abs(maxx) - Math.Abs(minx) > Math.Abs(maxy) - Math.Abs(miny) ? Math.Abs(maxx) - Math.Abs(minx) : Math.Abs(maxy) - Math.Abs(miny));

            if (max < 101F)
                graph.SCALE = 250;
            else if( max < 201F )
                graph.SCALE = 250;
            else if (max < 301F)
                graph.SCALE = 500;
            else if (max < 401F)
                graph.SCALE = 500;
            else if (max < 501F)
                graph.SCALE = 1000;
            else if (max < 601F)
                graph.SCALE = 1000;
            else
                graph.SCALE = 1000;

            float dif = 14;
            if (graph.SCALE < 499)
            {
                dif = 7;
            }
            else if (graph.SCALE > 500)
                dif = 28;

            float sc = graph.SC;

            float headx = 22F * cm;

            if ("PORT" != file)
                headx = 7F * cm;
            graph.TextWrite(textBox.Text + " TUNNING CIRCLE TO " + file, graph.DOCUMENT.PageSize.Width / 2F / 28.34F, graph.DOCUMENT.PageSize.Height / 28.34F - 2, 20, 0, BaseColor.BLACK);

            graph.ORIGIN = new System.Windows.Point(headx , 2F * 72F / 2.54F);

            graph.drawAxis(file);

            //graph.drawData(narg, sumtar, sumaarr);

            for (int i = 1; i < narg; i++)
            {
                float x1, x2, y1, y2;
                x1 = sumtar[i - 1]  * sc;
                x2 = sumtar[i]      * sc;
                y1 = sumaarr[i - 1] * sc;
                y2 = sumaarr[i]     * sc;
                graph.DrawLine(x1, y1, x2, y2, 0.5F, BaseColor.BLACK,"Solid");
            }
            
            foreach (var a in ship)
            {
                float x = 0F, y = 0F, xx = 0F, yy = 0F, angle ;

                CalValGivenAngle(narg, a, hh, sumtar, sumaarr, ref x, ref y);

                angle = 90 + a;
                xx = x;
                yy = y;
                if (file != "PORT")
                {
                    if (angle == 180)
                        angle = 360;
                    else if (angle == 360)
                        angle = 180;
                }

                switch (a)
                {
                    case 0:
                    case 360:
                        x += dif;
                        break;
                    case 180:
                        x -= dif;
                        break;
                    case 90:
                        if (file == "PORT")
                            y += dif;
                        else
                            y -= dif;
                        break;
                    case 270:
                        if (file == "PORT")
                            y -= dif;
                        else
                            y += dif;
                        break;
                }
                int   ang = 0;

                if (a == 180)
                    ang = 90;

                graph.TextSymbol("\u00B4", x * sc, y * sc, 30, (int)angle, BaseColor.BLUE);
                
                if ( a != 360)
                {
                    float minmax = minx;
                    if ( a == 90)
                    {
                        graph.DrawLine(xx * sc, yy * sc, xx * sc, (yy + (yy*0.2F)) * sc, 0.1F, BaseColor.BLACK, "Solid");                  // arrow head to y
                        graph.DrawLine(0, (yy + (yy * 0.15F)) * sc, xx * sc, (yy + (yy * 0.15F)) * sc, 0.1F, BaseColor.BLACK, "Solid");    // horizontal line
                        graph.DrawCircle(xx * sc, (yy + (yy * 0.15F)) * sc, 1);
                        graph.DrawCircle(0, (yy + (yy * 0.15F)) * sc, 1);
                        graph.TextWrite("TRANSFER = " + Math.Abs(xx).ToString("F0") + " M", xx * sc / 2F, (yy + (yy * 0.15F)) * sc, 8, ang, BaseColor.BLUE);

                        if (file != "PORT")                   // Draw Guide Line 
                            minmax = maxx;

                        graph.DrawLine(x * sc, yy * sc, (minmax + (minmax * 0.2F)) * sc, yy * sc, 0.1F, BaseColor.BLACK, "Solid");                              // horizontal line
                        graph.DrawLine((minmax + (minmax * 0.15F)) * sc, 0 * sc, (minmax + (minmax * 0.15F)) * sc, yy * sc, 0.1F, BaseColor.BLACK, "Solid");    // horizontal line

                        graph.DrawCircle((minmax + (minmax * 0.15F)) * sc, (yy) * sc, 1);
                        graph.DrawCircle((minmax + (minmax * 0.15F)) * sc, 0, 1);

                        graph.TextWrite("ADVANSED = " + yy.ToString("F0") + " M", (minmax + (minmax * 0.15F)) * sc, yy * sc / 2F, 8, 90, BaseColor.BLUE);

                    }
                    else if ( a == 180)
                    {
                        float ys = 0.6F;
                        if (graph.SCALE < 500)
                            ys = 0.2F;
                        graph.DrawLine(xx * sc, y * sc, xx * sc, (0 - (y * ys)) * sc, 0.1F, BaseColor.BLACK, "Solid");                                  // guide line to x
                        graph.DrawLine(0, (0 - (y * (ys-0.05F))) * sc, xx * sc, (0 - (y * (ys - 0.05F))) * sc, 0.1F, BaseColor.BLACK, "Solid");         // horizontal line

                        graph.DrawCircle(xx * sc, (y) * sc, 1);
                        graph.DrawCircle(xx * sc, (0 - (y * (ys - 0.05F))) * sc, 1);

                        graph.TextWrite("TACTICAL DIAMETER = " + Math.Abs(x).ToString("F0") + " M", xx * sc / 2F, (0 - (y * (ys - 0.05F))) * sc, 8, 0, BaseColor.BLUE);

                    }
                    else if ( a ==270)
                    {
                        if (graph.SCALE < 500)
                            graph.TextWrite("BASE COURSE = 270 DEG", 0 * sc, (-10F)  * sc, 8, 0, BaseColor.BLUE);
                        else
                            graph.TextWrite("BASE COURSE = 270 DEG", 0 * sc , (y) * sc - 0.5F, 8, 0, BaseColor.BLUE);
                    }
                }
            }

            //graph.FillStrokeTest();

            graph.CloseDocument();

            Browser1.Navigate(pdfname);

            this.Title = pdfname;


            if ( file == "PORT" )
            {
                for (int k = 0; k < narg; k++)
                {
                    xvp[ k] = sumtar[k];
                    yvp[ k] = sumaarr[k];
                    hhp[k] = hh[k];
                }
                iip = narg;
            }
            else
            {
                for (int k = 0; k < narg; k++)
                {
                    xvs[k] = sumtar[k];
                    yvs[k] = sumaarr[k];
                    hhs[k] = hh[k];
                }
                iis = narg;
            }

            inarg +=  narg;

            if ( ical == 1)
            {               
                drawPS(filename, iip, iis, xvp, yvp, xvs, yvs, max, hhp, hhs);
            }
                
            ical++;
            return true;
        }

        private void drawPS(string filename,int iip, int iis, float[] xvp, float[] yvp ,float[] xvs, float[] yvs, float max, float[] hhp, float[] hhs)
        {
            float cm = 72F / 2.54F;
            string lintype = "Dotted";
            int[] ship = new int[] { 0, 90, 180, 270, 360 };

            string path = System.IO.Path.GetDirectoryName(filename);
            string fname = System.IO.Path.GetFileNameWithoutExtension(filename);

            MSDEGraph g = new MSDEGraph("Land", path, fname + "-PS" , textBox.Text);

            g.TextWrite(textBox.Text + " TUNNING CIRCLE TO PORT & STARBOARD" , g.DOCUMENT.PageSize.Width / 2F / cm, g.DOCUMENT.PageSize.Height / cm - 2, 20, 0, BaseColor.BLACK);

            int min  = 400;
            int minh = 100;

            if (max > 800)
                g.AXISLENTH = 12;
            else 
                g.AXISLENTH = 7.5F;

            if (max < 201F)
            {
                min = 200;
                minh = 50;
                g.SCALE = 125;
            }
            else if  (max < 401F)
            {
                g.SCALE = 250;
            }
            else if (max < 801F)
            {
                min = 800;
                minh = 200;
                g.SCALE = 500;
            }
            else
            {
                min = 1600;
                minh = 400;
                g.SCALE = 1000;
            }

            g.drawRect(2F, 0.5F);

            float sc   = g.SC;
            float intv = g.SCALE / 5;

            for (int i = 0; i < 17; i++)
            {

                int interv = Convert.ToInt32((intv * i)) - min;
                int interh = Convert.ToInt32((intv * i)) - minh;
                var color  = BaseColor.BLUE;

                lintype = "Dotted";
                if (i == 8)
                {
                    color = BaseColor.BLACK;
                    lintype = "Solid";
                }
                if (i < 11)
                {
                    if (i == 2)
                        g.DrawLine(3F, (2F + 1.5F * i), 1.5F * 18F, (2F + 1.5F * i), 0.1F, BaseColor.BLACK, "Solid");   // Hori
                    else
                        g.DrawLine(3F, (2F+ 1.5F * i),  1.5F * 18F, (2F + 1.5F * i), 0.1F, BaseColor.BLUE, "Dotted");   // Hori

                    g.TextWrite(interh.ToString(), 2F, (2F + 1.5F * i), 10, 0, BaseColor.BLUE);
                }

                g.DrawLine((3 + 1.5F * i), 2F, (3 + 1.5F * i), (2+1.5F * 10F), 0.1F, color, lintype);      // Vert
                
                g.TextWrite(interv.ToString(), (3F + 1.5F * i), 1.5F, 10, 0, BaseColor.BLUE);
            }

            g.drawARect(1F, 3F, 2F, 24, 1.5F * 10);

            g.ORIGIN = new System.Windows.Point(15*cm,  5F*cm);

            float dif = 28;
            if (g.SCALE < 499)
            {
                //dif = 14;
                dif = 7;
            }
            else if (g.SCALE > 500)
                dif = 28;

            for (int i = 1; i < iis; i++)   // Starboard
            {
                float x1, x2, y1, y2;
                x1 = xvs[i - 1] * sc;
                x2 = xvs[i] * sc;
                y1 = yvs[i - 1] * sc;
                y2 = yvs[i] * sc;
                g.DrawLine(x1, y1, x2, y2, 0.5F, BaseColor.BLACK, "Solid");
            }


            List<float> ax = xvs.ToList();
            List<float> ay = yvs.ToList();

            float minx, miny, maxx, maxy;

            minx = ax.Min();
            miny = ay.Min();
            maxx = ax.Max();
            maxy = ay.Max();

            string file = "STARBOARD";

            foreach (var a in ship)
            {
                float x = 0F, y = 0F, xx = 0F, yy = 0F, angle;

                CalValGivenAngle(iis, a, hhs, xvs, yvs, ref x, ref y);

                angle = 90 + a;
                xx = x;
                yy = y;
                if (file != "PORT")
                {
                    if (angle == 180)
                        angle = 360;
                    else if (angle == 360)
                        angle = 180;
                }

                switch (a)
                {
                    case 0:
                    case 360:
                        x += dif;
                        break;
                    case 180:
                        x -= dif;
                        break;
                    case 90:
                        if (file == "PORT")
                            y += dif;
                        else
                            y -= dif;
                        break;
                    case 270:
                        if (file == "PORT")
                            y -= dif;
                        else
                            y += dif;
                        break;
                }
                int ang = 0;

                if (a == 180)
                    ang = 90;

                if (g.SCALE < 499)
                    g.TextSymbol("\u00B4", x * sc, y * sc, 20, (int)angle, BaseColor.BLUE);
                else
                    g.TextSymbol("\u00B4", x * sc, y * sc, 30, (int)angle, BaseColor.BLUE);

                if (a != 360)
                {
                    float minmax = minx;
                    if (a == 90)
                    {
                        g.DrawLine(xx * sc, yy * sc, xx * sc, (yy + (yy * 0.2F)) * sc, 0.1F, BaseColor.BLACK, "Solid");                  // arrow head to y
                        g.DrawLine(0, (yy + (yy * 0.15F)) * sc, xx * sc, (yy + (yy * 0.15F)) * sc, 0.1F, BaseColor.BLACK, "Solid");      // horizontal line
                        g.DrawCircle(xx * sc, (yy + (yy * 0.15F)) * sc, 1);
                        g.DrawCircle(0, (yy + (yy * 0.15F)) * sc, 1);
                        g.TextWrite("TRANSFER = " + Math.Abs(xx).ToString("F0") + " M", xx * sc / 2F, (yy + (yy * 0.15F)) * sc, 8, ang, BaseColor.BLUE);

                        if (file != "PORT")                   // Draw Guide Line 
                            minmax = maxx;

                        g.DrawLine(x * sc, yy * sc, (minmax + (minmax * 0.2F)) * sc, yy * sc, 0.1F, BaseColor.BLACK, "Solid");                              // horizontal line
                        g.DrawLine((minmax + (minmax * 0.15F)) * sc, 0 * sc, (minmax + (minmax * 0.15F)) * sc, yy * sc, 0.1F, BaseColor.BLACK, "Solid");    // horizontal line
                        g.DrawCircle((minmax + (minmax * 0.15F)) * sc, (yy) * sc, 1);
                        g.DrawCircle((minmax + (minmax * 0.15F)) * sc, 0, 1);
                        g.TextWrite("ADVANSED = " + yy.ToString("F0") + " M", (minmax + (minmax * 0.15F)) * sc, yy * sc / 2F, 8, 90, BaseColor.BLUE);

                    }
                    else if (a == 180)
                    {
                        float ys = 0.6F;
                        if (g.SCALE < 500)
                            ys = 0.2F;
                        g.DrawLine(xx * sc, y * sc, xx * sc, (0 - (y * ys)) * sc, 0.1F, BaseColor.BLACK, "Solid");                                  // guide line to x
                        g.DrawLine(0, (0 - (y * (ys - 0.05F))) * sc, xx * sc, (0 - (y * (ys - 0.05F))) * sc, 0.1F, BaseColor.BLACK, "Solid");         // horizontal line

                        g.DrawCircle(xx * sc, (y) * sc, 1);
                        g.DrawCircle(xx * sc, (0 - (y * (ys - 0.05F))) * sc, 1);

                        g.TextWrite("TACTICAL DIAMETER = " + Math.Abs(x).ToString("F0") + " M", xx * sc / 2F, (0 - (y * (ys - 0.05F))) * sc, 8, 0, BaseColor.BLUE);

                    }
                    else if (a == 270)
                    {
                        if (g.SCALE < 500)
                            g.TextWrite("BASE COURSE = 270 DEG", 0 * sc, (-10F) * sc, 8, 0, BaseColor.BLUE);
                        else
                            g.TextWrite("BASE COURSE = 270 DEG", 0 * sc, (y) * sc - 0.5F, 8, 0, BaseColor.BLUE);
                    }
                }
            }
            file = "PORT";
            for (int i =  1 ; i < iip; i++) // Port
            {
                float x1, x2, y1, y2;
                x1 = xvp[i - 1] * sc;
                x2 = xvp[i] * sc;
                y1 = yvp[i - 1] * sc;
                y2 = yvp[i] * sc;
                g.DrawLine(x1, y1, x2, y2, 0.5F, BaseColor.BLACK, "Solid");
            }


            List<float> axx = xvp.ToList();
            List<float> ayy = yvp.ToList();

            minx = axx.Min();
            miny = ayy.Min();
            maxx = axx.Max();
            maxy = ayy.Max();

            //if (maxx > 10F)
            //    file = "STARBOARD";
            //else
            //    file = "PORT";

            foreach (var a in ship)
            {
                float x = 0F, y = 0F, xx = 0F, yy = 0F, angle;

                CalValGivenAngle(iip, a, hhp, xvp, yvp, ref x, ref y);

                angle = 90 + a;
                xx = x;
                yy = y;
                if (file != "PORT")
                {
                    if (angle == 180)
                        angle = 360;
                    else if (angle == 360)
                        angle = 180;
                }

                switch (a)
                {
                    case 0:
                    case 360:
                        x += dif;
                        break;
                    case 180:
                        x -= dif;
                        break;
                    case 90:
                        if (file == "PORT")
                            y += dif;
                        else
                            y -= dif;
                        break;
                    case 270:
                        if (file == "PORT")
                            y -= dif;
                        else
                            y += dif;
                        break;
                }
                int ang = 0;

                if (a == 180)
                    ang = 90;

                if (g.SCALE < 499)
                    g.TextSymbol("\u00B4", x * sc, y * sc, 20, (int)angle, BaseColor.BLUE);
                else
                    g.TextSymbol("\u00B4", x * sc, y * sc, 30, (int)angle, BaseColor.BLUE);

                if (a != 360)
                {
                    float minmax = minx;
                    if (a == 90)
                    {
                        g.DrawLine(xx * sc, yy * sc, xx * sc, (yy + (yy * 0.2F)) * sc, 0.1F, BaseColor.BLACK, "Solid");                  // arrow head to y
                        g.DrawLine(0, (yy + (yy * 0.15F)) * sc, xx * sc, (yy + (yy * 0.15F)) * sc, 0.1F, BaseColor.BLACK, "Solid");    // horizontal line
                        g.DrawCircle(xx * sc, (yy + (yy * 0.15F)) * sc, 1);
                        g.DrawCircle(0, (yy + (yy * 0.15F)) * sc, 1);
                        g.TextWrite("TRANSFER = " + Math.Abs(xx).ToString("F0") + " M", xx * sc / 2F, (yy + (yy * 0.15F)) * sc, 8, ang, BaseColor.BLUE);

                        if (file != "PORT")                   // Draw Guide Line 
                            minmax = maxx;

                        g.DrawLine(x * sc, yy * sc, (minmax + (minmax * 0.2F)) * sc, yy * sc, 0.1F, BaseColor.BLACK, "Solid");                              // horizontal line
                        g.DrawLine((minmax + (minmax * 0.15F)) * sc, 0 * sc, (minmax + (minmax * 0.15F)) * sc, yy * sc, 0.1F, BaseColor.BLACK, "Solid");    // horizontal line
                        g.DrawCircle((minmax + (minmax * 0.15F)) * sc, (yy) * sc, 1);
                        g.DrawCircle((minmax + (minmax * 0.15F)) * sc, 0, 1);
                        g.TextWrite("ADVANSED = " + yy.ToString("F0") + " M", (minmax + (minmax * 0.15F)) * sc, yy * sc / 2F, 8, 90, BaseColor.BLUE);

                    }
                    else if (a == 180)
                    {
                        float ys = 0.6F;
                        if (g.SCALE < 500)
                            ys = 0.2F;
                        g.DrawLine(xx * sc, y * sc, xx * sc, (0 - (y * ys)) * sc, 0.1F, BaseColor.BLACK, "Solid");                                  // guide line to x
                        g.DrawLine(0, (0 - (y * (ys - 0.05F))) * sc, xx * sc, (0 - (y * (ys - 0.05F))) * sc, 0.1F, BaseColor.BLACK, "Solid");         // horizontal line

                        g.DrawCircle(xx * sc, (y) * sc, 1);
                        g.DrawCircle(xx * sc, (0 - (y * (ys - 0.05F))) * sc, 1);

                        g.TextWrite("TACTICAL DIAMETER = " + Math.Abs(x).ToString("F0") + " M", xx * sc / 2F, (0 - (y * (ys - 0.05F))) * sc, 8, 0, BaseColor.BLUE);

                    }
                    else if (a == 270)
                    {

                    }
                }
            }
            g.CloseDocument();

            Browser1.Navigate(path + "\\" + fname + "-PS" + ".pdf");

        }
        private void SPLINE(int N, int NARG, float XSP, float[] X, float[] Y, ref float[] DOMAIN, ref float[] FUNC, ref float[] DERIV)
        {

            float[] S = new float[200];
            float[] G = new float[200];
            float[] WORK = new float[200];
            float XI, XIM1, XIP1, YI, YIM1, YIP1, X1, H, T;
            float EPS = 0.05F;

            if (NARG != 1) {
                for (int i = 1; i <= NARG; i++)
                {
                    DOMAIN[i-1] = XSP * (i - 1);
                }
            }
            
            for (int i = 1; i < N-1; i++)
            {

                XI      = X[i];
                XIM1    = X[i - 1];
                XIP1    = X[i + 1];
                YI      = Y[i];
                YIM1    = Y[i - 1];
                YIP1    = Y[i + 1];
                X1      = XI - XIM1;
                H       = XIP1 - XIM1;
                WORK[i] = 0.5F * X1 / H;
                T = ((YIP1 - YI) / (XIP1 - XI) - (YI - YIM1) / X1) / H;
                S[i] = 2 * T;
                G[i] = 3 * T;
            }

            S[1] = 0F;
            S[N] = 0F;
            float W = 8F - 4F *  (float) Math.Sqrt(3F);
    ReCal:
            float U = 0F;
            //while ( U >= EPS )
            //{
                U = 0F;
                for (int ik = 1 ; ik <= N-2; ik++ )
                {
                  T = W * (float) (-S[ik] - WORK[ik] * S[ik - 1] - (0.5 - WORK[ik]) * S[ik + 1] + G[ik]);
                  H = Math.Abs(T);

                  if (H > U)
                     U = H;
                  S[ik] = S[ik] + T;
                }
            //}
            if (U >= EPS) goto ReCal;

            for ( int II = 0; II <= N-2; II++)
            {
                G[II] = (S[II + 1] - S[II]) / (X[II + 1] - X[II]);
            }

            int I = 0;
            float X2, S1, Z;

            if (NARG != 0)
            {      
                for (int J = 0; J < NARG; J++)
                {
                    I = 1;
                    T = DOMAIN[J];

                    if (T >= X[0])
                    {
                        while(true)
                        {
                            if (I > N) {
                                break;
                            }

                            if (T < X[I])
                            {
                                I = I - 1;
                                H = DOMAIN[J] - X[I];
                                T = DOMAIN[J] - X[I + 1];
                                X2 = H * T;
                                S1 = S[I] + H * G[I];
                                Z = 1F / 6F;
                                U = Z * (S[I] + S[I + 1] + S1);
                                W = (Y[I + 1] - Y[I]) / (X[I + 1] - X[I]);
                                FUNC[J] = W * H + Y[I] + X2 * U;
                                DERIV[J] = W + (H + T) * U + Z * X2 * G[I];
                                break;
                            }
                            else
                                I = I + 1;
                        }
                    }
                    else
                        break;
                }
            } 
        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object paper = ((System.Windows.Controls.ContentControl)comboBox.SelectedItem).Content;

            string file = "";
            switch (paper.ToString())
            {
                case "Port Drawing":
                    file = "PORT";
                    break;
                case "Starboard Drawing":
                    file = "STARBOARD";
                    break;
                default:
                    file = "PS";
                    break;
            }
            if (filename != "")
            {
                string path  = System.IO.Path.GetDirectoryName(filename);
                string fname = System.IO.Path.GetFileNameWithoutExtension(filename);

                string pdfname = path +"\\"+ fname + "-" + file + ".pdf";

                if (File.Exists(pdfname))
                    Browser1.Navigate(pdfname);
                else
                    MessageBox.Show("Drawing File " + pdfname + " Not exist!!!");

                string resultfile = path + "\\"+ fname + "-" + file + ".TXT";

                if (File.Exists(resultfile))
                {
                    richTextBox.Document.Blocks.Clear();
                    richTextBox.AppendText(File.ReadAllText(resultfile));
                }
                else
                    MessageBox.Show("Result File " + resultfile + " Not exist!!!");
            }
        }
        private void btnView_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
            //Document document = new Document();
            //PdfWriter.GetInstance(document, new FileStream( @"d:\\test.pdf", FileMode.Create, FileAccess.Write, FileShare.None));
            //document.Open();
            //Font f1 = FontFactory.GetFont(FONT1, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12);
            //Font f2 = FontFactory.GetFont(FONT2, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12);
            //Font f3 = FontFactory.GetFont(FONT3, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12);
            //Font f4 = FontFactory.GetFont(FONT3, BaseFont.WINANSI,    BaseFont.EMBEDDED, 12);
            //document.Add(new iTextSharp.text.Paragraph(RUPEE, f1));
            //document.Add(new iTextSharp.text.Paragraph(RUPEE, f2));
            //document.Add(new iTextSharp.text.Paragraph(RUPEE, f3));
            //document.Add(new iTextSharp.text.Paragraph(RUPEE, f4));

            //document.Add(new iTextSharp.text.Paragraph(SHIP,  f4));
            //document.Close();
        }
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {

        }
        private void TextSymbol(PdfWriter wr, String text, float x, float y, float fs, int angle, BaseColor color)
        {
            BaseFont bf = BaseFont.CreateFont(@"C:\Windows\Fonts\WINGDNG3.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            PdfContentByte cb = wr.DirectContent;

            cb.SaveState();
            cb.BeginText();
            cb.SetFontAndSize(bf, fs);
            cb.SetColorFill(color);
            cb.ShowTextAligned(Element.ALIGN_LEFT, text, x, y, angle);

            cb.EndText();
            cb.RestoreState();

            cb.Stroke();
        }
        private void TextWrite(PdfWriter wr, String text, float x, float y, float fs, int angle)
        {
            //BaseFont zapfdingbats;

            //zapfdingbats = BaseFont.CreateFont(BaseFont.ZAPFDINGBATS, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);

            BaseFont bf = BaseFont.CreateFont(@"C:\Windows\Fonts\malgun.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            
            PdfContentByte cb = wr.DirectContent;
            //BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            //BaseFont bf = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb.SaveState();

            cb.BeginText();
            cb.SetFontAndSize(bf, fs);
            cb.SetColorFill(BaseColor.BLACK);
            cb.ShowTextAligned(Element.ALIGN_LEFT, text, x, y, angle);

            cb.EndText();
            cb.RestoreState();

            cb.Stroke();
        }
        private bool DrawTunningCircle()
        {
            System.Windows.Point p1 = new System.Windows.Point(20, 20);
            System.Windows.Point p2 = new System.Windows.Point(40, 40);


            // angle in radians
            var angleRadians = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);

            // angle in degrees
            var angleDeg     = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) * 180 / Math.PI;

            return true;

        }
        private bool CalValGivenAngle(int narg, int angle, float[] hh, float[] sumtar, float[] sumaarr, ref float  x, ref float y)
        {
            float maxx = -99999F;
            int maxi = 999;
            if ( angle == 9999)
            {
                for (int ii = 0; ii < narg; ii++)
                {
                        if (sumtar[ii] > maxx)
                        {
                            maxi = ii;
                            maxx = sumtar[ii];
                        }
                }

                x = sumtar[maxi] ;
                y = sumaarr[maxi] ;
            }
            else if ( angle == -9999)     
            {
                for (int ii = 1; ii < narg; ii++)
                {
                    if (sumaarr[ii] < 0)
                    {
                            maxi = ii;
                            maxx = x;
                            break;
                    }
                }
                int i = maxi;
                x = sumtar[i - 1] + (sumtar[i] - sumtar[i - 1])  *   ((0 - Math.Abs(sumaarr[i - 1])) / (Math.Abs(sumaarr[i]) - Math.Abs(sumaarr[i - 1])));
                y = sumaarr[i - 1] + (sumaarr[i] - sumaarr[i - 1]) * ((0 - Math.Abs(sumaarr[i - 1])) / (Math.Abs(sumaarr[i]) - Math.Abs(sumaarr[i - 1])));
            }
            else
            {
                for ( int i = 0; i < narg; i++)
                {
                    if ( Math.Abs(hh[i]) > angle)
                    {
                        x = sumtar[i - 1] +   (sumtar[i]  - sumtar[i - 1]) *   ( (angle - Math.Abs(hh[i - 1])) / (Math.Abs(hh[i]) - Math.Abs(hh[i - 1])));
                        y = sumaarr[i - 1] +  (sumaarr[i] - sumaarr[i - 1]) *  ( (angle - Math.Abs(hh[i - 1])) / (Math.Abs(hh[i]) - Math.Abs(hh[i - 1])));
                        break;
                    }
                }
            }

            return true;
        }
    }
}
