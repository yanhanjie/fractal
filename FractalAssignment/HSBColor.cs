using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

/// <summary>
/// djm added, Java can work with the HSB/V colour system
/// c# can only do RGB
/// so I searched the internet for some code to convert
/// see http://www.codeproject.com/dotnet/HSBColorClass.asp
/// note I have removed some code from the downloaded class that isn't needed, just to make it clearer
/// </summary>
namespace FractalAssignment
{
    public struct HSBColor
    {
        float h;
        float s;
        float b;
        int a;

        public HSBColor(float h, float s, float b)
        {
            this.a = 0xff;
            this.h = Math.Min(Math.Max(h, 0), 255) * 255; // JC added * 255 to correct colour input for rgb range
            this.s = Math.Min(Math.Max(s, 0), 255) * 255; // JC added * 255 to correct colour input for rgb range
            this.b = Math.Min(Math.Max(b, 0), 255) * 255; // JC added * 255 to correct colour input for rgb range
        }

        public HSBColor(int a, float h, float s, float b)
        {
            this.a = a;
            this.h = Math.Min(Math.Max(h, 0), 255) * 255; // JC added * 255 to correct colour input for rgb range
            this.s = Math.Min(Math.Max(s, 0), 255) * 255; // JC added * 255 to correct colour input for rgb range
            this.b = Math.Min(Math.Max(b, 0), 255) * 255; // JC added * 255 to correct colour input for rgb range
        }

        public float H
        {
            get { return h; }
        }

        public float S
        {
            get { return s; }
        }

        public float B
        {
            get { return b; }
        }

        public int A
        {
            get { return a; }
        }

        public Color Color
        {
            get
            {
                return FromHSB(this);
            }
        }

        public Color FromHSB(HSBColor hsbColor) // static removed to fix errors
        {
            float r = hsbColor.b;
            float g = hsbColor.b;
            float b = hsbColor.b;
            if (hsbColor.s != 0)
            {
                float max = hsbColor.b;
                float dif = hsbColor.b * hsbColor.s / 255f;
                float min = hsbColor.b - dif;

                float h = hsbColor.h * 360f / 255f;

                if (h < 60f)
                {
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                }
                else if (h < 120f)
                {
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                }
                else if (h < 180f)
                {
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                }
                else if (h < 240f)
                {
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                }
                else if (h < 300f)
                {
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                }
                else if (h <= 360f)
                {
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
                }
                else
                {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }

            return Color.FromArgb
                (
                    hsbColor.a,
                    (int)Math.Round(Math.Min(Math.Max(r, 0), 255)),
                    (int)Math.Round(Math.Min(Math.Max(g, 0), 255)),
                    (int)Math.Round(Math.Min(Math.Max(b, 0), 255))
                    );
        }

    }

}
