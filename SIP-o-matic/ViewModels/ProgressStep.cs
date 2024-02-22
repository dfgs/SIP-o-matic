using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{

	public enum StepStatuses { Undefined,Running,Terminated,Error};

	public class ProgressStep:DependencyObject
	{


		

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(ProgressStep), new PropertyMetadata(1));
		public int Value
		{
			get { return (int)GetValue(ValueProperty); }
			private set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(ProgressStep), new PropertyMetadata("Step"));
		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		public static readonly DependencyProperty FullLabelProperty = DependencyProperty.Register("FullLabel", typeof(string), typeof(ProgressStep), new PropertyMetadata("Step"));
		public string FullLabel
		{
			get { return (string)GetValue(FullLabelProperty); }
			private set { SetValue(FullLabelProperty, value); }
		}



		public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(StepStatuses), typeof(ProgressStep), new PropertyMetadata(StepStatuses.Undefined));
		public StepStatuses Status
		{
			get { return (StepStatuses)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}

		public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register("ErrorMessage", typeof(string), typeof(ProgressStep), new PropertyMetadata(null));
		public string? ErrorMessage
		{
			get { return (string?)GetValue(ErrorMessageProperty); }
			private set { SetValue(ErrorMessageProperty, value); }
		}



		public static readonly DependencyProperty TaskFactoryProperty = DependencyProperty.Register("TaskFactory", typeof(Func<CancellationToken, int,  Task>), typeof(ProgressStep), new PropertyMetadata(null));
		public Func<CancellationToken, int, Task> TaskFactory
		{
			get { return (Func<CancellationToken, int,  Task>)GetValue(TaskFactoryProperty); }
			set { SetValue(TaskFactoryProperty, value); }
		}

		public static readonly DependencyProperty MaximumGetterProperty = DependencyProperty.Register("MaximumGetter", typeof(Func<int>), typeof(ProgressStep), new PropertyMetadata(null));
		public Func<int> MaximumGetter
		{
			get { return (Func<int>)GetValue(MaximumGetterProperty); }
			set { SetValue(MaximumGetterProperty, value); }
		}

		public int Maximum
		{
			get
			{
				if (MaximumGetter == null) return 1;
				else return MaximumGetter();
			}
		}


		public ProgressStep()
		{

		}

		private void UpdateFullLabel()
		{
			this.FullLabel = $"{Label} ({Value + 1}/{Maximum})";
		}

		public void Init()
		{
			this.Value = 0;
			UpdateFullLabel();
			this.Status = StepStatuses.Undefined;
		}
		public void Begin()
		{
			this.Value = 0;
			UpdateFullLabel();
			this.Status = StepStatuses.Running;
		}
		public void Update(int Value)
		{
			this.Value = Value;
			UpdateFullLabel();

		}
		public void End(string? ErrorMessage=null)
		{
			this.Value = Maximum-1;
			UpdateFullLabel();
			if (ErrorMessage == null) this.Status = StepStatuses.Terminated;
			else this.Status = StepStatuses.Error;
			this.ErrorMessage = ErrorMessage;
		}


	}
}
