using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.Models
{
	public class AnalysisStep:DependencyObject
	{


		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(AnalysisStep), new PropertyMetadata(10));
		public int Maximum
		{
			get { return (int)GetValue(MaximumProperty); }
			private set { SetValue(MaximumProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(AnalysisStep), new PropertyMetadata(1));
		public int Value
		{
			get { return (int)GetValue(ValueProperty); }
			private set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(AnalysisStep), new PropertyMetadata("Step"));
		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		public static readonly DependencyProperty FullLabelProperty = DependencyProperty.Register("FullLabel", typeof(string), typeof(AnalysisStep), new PropertyMetadata("Step"));
		public string FullLabel
		{
			get { return (string)GetValue(FullLabelProperty); }
			private set { SetValue(FullLabelProperty, value); }
		}



		public static readonly DependencyProperty IsTerminatedProperty = DependencyProperty.Register("IsTerminated", typeof(bool), typeof(AnalysisStep), new PropertyMetadata(false));
		public bool IsTerminated
		{
			get { return (bool)GetValue(IsTerminatedProperty); }
			set { SetValue(IsTerminatedProperty, value); }
		}




		public AnalysisStep()
		{

		}

		public void Begin(int Maximum)
		{
			this.Value = 0;
			this.Maximum= Maximum;
			this.FullLabel = $"{Label} ({Value + 1}/{Maximum})";
			this.IsTerminated = false;
		}
		public void Update(int Value)
		{
			this.Maximum = Maximum;
			this.Value = Value;
			this.FullLabel = $"{Label} ({Value + 1}/{Maximum})";
			
		}
		public void End()
		{
			this.Value = Maximum-1;
			this.Maximum = Maximum;
			this.FullLabel = $"{Label} ({Value + 1}/{Maximum})";
			this.IsTerminated = true;
		}


	}
}
