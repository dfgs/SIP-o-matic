﻿using SIP_o_matic.DataSources;
using SIP_o_matic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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



		public static readonly DependencyProperty TaskFactoryProperty = DependencyProperty.Register("TaskFactory", typeof(Func<CancellationToken, ProjectViewModel, IDataSource, string,  Task>), typeof(AnalysisStep), new PropertyMetadata(null));
		public Func<CancellationToken, ProjectViewModel,IDataSource, string, Task> TaskFactory
		{
			get { return (Func<CancellationToken, ProjectViewModel,IDataSource, string,  Task>)GetValue(TaskFactoryProperty); }
			set { SetValue(TaskFactoryProperty, value); }
		}




		public AnalysisStep()
		{

		}

		private void UpdateFullLabel()
		{
			this.FullLabel = $"{Label} ({Value + 1}/{Maximum})";
		}

		public void Init(int Maximum)
		{
			this.Value = 0;
			this.Maximum = Maximum;
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
			this.Maximum = Maximum;
			this.Value = Value;
			UpdateFullLabel();

		}
		public void End(string? ErrorMessage=null)
		{
			this.Value = Maximum-1;
			this.Maximum = Maximum;
			UpdateFullLabel();
			if (ErrorMessage == null) this.Status = StepStatuses.Terminated;
			else this.Status = StepStatuses.Error;
			this.ErrorMessage = ErrorMessage;
		}


	}
}
