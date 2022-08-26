using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic
{
	public class ColorManager
	{
		private Color[] colors;
		private int request;

		/// <summary>
		/// Convert HSV to RGB
		/// h is from 0-360
		/// s,v values are 0-1
		/// r,g,b values are 0-255
		/// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
		/// </summary>
		private void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
		{
			// ######################################################################
			// T. Nathan Mundhenk
			// mundhenk@usc.edu
			// C/C++ Macro HSV to RGB

			double H = h;
			while (H < 0) { H += 360; };
			while (H >= 360) { H -= 360; };
			double R, G, B;
			if (V <= 0)
			{ R = G = B = 0; }
			else if (S <= 0)
			{
				R = G = B = V;
			}
			else
			{
				double hf = H / 60.0;
				int i = (int)Math.Floor(hf);
				double f = hf - i;
				double pv = V * (1 - S);
				double qv = V * (1 - S * f);
				double tv = V * (1 - S * (1 - f));
				switch (i)
				{

					// Red is the dominant color

					case 0:
						R = V;
						G = tv;
						B = pv;
						break;

					// Green is the dominant color

					case 1:
						R = qv;
						G = V;
						B = pv;
						break;
					case 2:
						R = pv;
						G = V;
						B = tv;
						break;

					// Blue is the dominant color

					case 3:
						R = pv;
						G = qv;
						B = V;
						break;
					case 4:
						R = tv;
						G = pv;
						B = V;
						break;

					// Red is the dominant color

					case 5:
						R = V;
						G = pv;
						B = qv;
						break;

					// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

					case 6:
						R = V;
						G = tv;
						B = pv;
						break;
					case -1:
						R = V;
						G = pv;
						B = qv;
						break;

					// The color is not defined, we should throw an error.

					default:
						//LFATAL("i Value error in Pixel conversion, Value is %d", i);
						R = G = B = V; // Just pretend its black/white
						break;
				}
			}
			r = Clamp((int)(R * 255.0));
			g = Clamp((int)(G * 255.0));
			b = Clamp((int)(B * 255.0));
		}

		/// <summary>
		/// Clamp a value to 0-255
		/// </summary>
		private int Clamp(int i)
		{
			if (i < 0) return 0;
			if (i > 255) return 255;
			return i;
		}

		public ColorManager(int Count)
		{
			float delta;
			float h, s, v;
			int r, g, b;

			request = 0;

			delta = 360.0f / Count;

			colors = new Color[Count];
			for(int t=0;t<Count; t++)
			{
				h = t * delta;
				s = 1.0f;
				v = 1.0f;

				HsvToRgb(h, s, v, out r, out g, out b);
				colors[t] = Color.FromArgb(r, g, b);
			}
		}

		/*public Color GetColor(int Index)
		{
			return colors[Index%colors.Length];
		}*/
		public string GetColorString()
		{
			//int div;
			//int rest;
			int index;
			//int step;

			Color color;

			//div = request /2;
			//rest = request % colors.Length;
			//step = colors.Length / 2;

			index = request;// rest * step+div;
			color= colors[index % colors.Length];

			request++;
			return $"#{color.Name}"; ; 
		}
	}
}
