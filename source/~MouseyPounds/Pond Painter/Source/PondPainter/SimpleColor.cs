using System;

namespace PondPainter
{
    // The MacOS & Linux versions of Stardew Valley and/or SMAPI refused to load the mod when we were using the Colormine and Colormineportable
    //  libraries to do our color conversions. Since we only need to go back and forth between RGB and HSV, we're now rolling our own. Joy.
    public class SimpleRGB
    {
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }

        public SimpleRGB()
        {
            this.R = 0.0;
            this.G = 0.0;
            this.B = 0.0;
        }
        public SimpleRGB(int r, int g, int b)
        {
            this.R = (double)r;
            this.G = (double)g;
            this.B = (double)b;
        }
        public SimpleRGB(double r, double g, double b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public SimpleHSV ToHSV()
        {
            // Algorithm from https://www.rapidtables.com/convert/color/rgb-to-hsv.html
            double rr = this.R / 255.0;
            double gg = this.G / 255.0;
            double bb = this.B / 255.0;

            double c_max = Math.Max(Math.Max(rr, gg), bb);
            double c_min = Math.Min(Math.Min(rr, gg), bb);

            double delta = c_max - c_min;

            SimpleHSV converted = new SimpleHSV();
            if (delta != 0.0) {
                if (c_max == rr)
                {
                    converted.H = 60.0 * ( ( (gg - bb) / delta ) % 6);
                }
                else if (c_max == gg)
                {
                    converted.H = 60.0 * ( (bb - rr) / delta + 2);
                }
                else if (c_max == bb)
                {
                    converted.H = 60.0 * ( (rr - gg) / delta + 4);
                }
            }
            if (c_max != 0.0)
            {
                converted.S = delta / c_max;
            }
            converted.V = c_max;

            // Just in case, we are clamping everything
            converted.H = Math.Min(360.0, Math.Max(0.0, converted.H));
            converted.S = Math.Min(1.0, Math.Max(0.0, converted.S));
            converted.V = Math.Min(1.0, Math.Max(0.0, converted.V));

            return converted;
        }
    }

    public class SimpleHSV
    {
        public double H { get; set; } = 0.0;
        public double S { get; set; } = 0.0;
        public double V { get; set; } = 0.0;

        public SimpleHSV()
        {
            this.H = 0.0;
            this.S = 0.0;
            this.V = 0.0;
        }
        public SimpleHSV(double h, double s, double v)
        {
            this.H = h;
            this.S = s;
            this.V = v;
        }

        public SimpleRGB ToRGB()
        {
            // Algorithm from https://www.rapidtables.com/convert/color/hsv-to-rgb.html
            double c = this.V * this.S;
            double x = c * (1 - Math.Abs((this.H / 60.0) % 2 - 1));
            double m = this.V - c;

            double rr = 0.0;
            double gg = 0.0;
            double bb = 0.0;
            if (this.H < 60.0)
            {
                rr = c;
                gg = x;
            }
            else if (this.H < 120.0)
            {
                rr = x;
                gg = c;
            }
            else if (this.H < 180.0)
            {
                gg = c;
                bb = x;
            }
            else if (this.H < 240.0)
            {
                gg = x;
                bb = c;
            }
            else if (this.H < 300.0)
            {
                rr = x;
                bb = c;
            }
            else
            {
                rr = c;
                bb = x;
            }

            SimpleRGB converted = new SimpleRGB();
            converted.R = Math.Min(255.0, Math.Max(0.0, 255.0 * (rr + m)));
            converted.G = Math.Min(255.0, Math.Max(0.0, 255.0 * (gg + m)));
            converted.B = Math.Min(255.0, Math.Max(0.0, 255.0 * (bb + m)));
            return converted;
        }

    }

}
