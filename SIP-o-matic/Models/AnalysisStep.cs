using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.Models
{

	public enum StepStatuses { Undefined,Running,Terminated,Error};

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



		public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(StepStatuses), typeof(AnalysisStep), new PropertyMetadata(StepStatuses.Undefined));
		public StepStatuses Status
		{
			get { return (StepStatuses)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}

		public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register("ErrorMessage", typeof(string), typeof(AnalysisStep), new PropertyMetadata(null));
		public string? ErrorMessage
		{
			get { return (string?)GetValue(ErrorMessageProperty); }
			private set { SetValue(ErrorMessageProperty, value); }
		}



		public AnalysisStep()
		{

		}

		public void Begin(int Maximum)
		{
			this.Value = 0;
			this.Maximum= Maximum;
			this.FullLabel = $"{Label} ({Value + 1}/{Maximum})";
			this.Status = StepStatuses.Running;
		}
		public void Update(int Value)
		{
			this.Maximum = Maximum;
			this.Value = Value;
			this.FullLabel = $"{Label} ({Value + 1}/{Maximum})";
			
		}
		public void End(string? ErrorMessage=null)
		{
			this.Value = Maximum-1;
			this.Maximum = Maximum;
			this.FullLabel = $"{Label} ({Value + 1}/{Maximum})";
			if (ErrorMessage == null) this.Status = StepStatuses.Terminated;
			else this.Status = StepStatuses.Error;
			this.ErrorMessage = ErrorMessage;
		}


	}
}
