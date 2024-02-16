using LogLib;
using ModuleLib;
using SIP_o_matic.DataSources;
using SIP_o_matic.Models;
using SIP_o_matic.ViewModels;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SIP_o_matic.Modules
{
	public class CallFormatModule : Module
	{
		private List<string> legs;
		private List<string> colors;
		private ColorManager colorManager;

		public CallFormatModule(ILogger Logger) : base(Logger)
		{
			legs = new List<string>();
			colors = new List<string>();
			colorManager = new ColorManager(10);
		}

		private string GetLegName(string CallID,string SourceDevice,string DestinationDevice)
		{
			int index;
			string key;
			int hash; ;

			int hash1,hash2;
			hash1 = 23 * 31 + SourceDevice.GetHashCode();
			hash2 = 23 * 31 + DestinationDevice.GetHashCode();
			hash=hash1 ^ hash2;

			key = $"{CallID}/{hash}" ;

			index=legs.IndexOf(key);
			if (index==-1)
			{
				index = legs.Count;
				legs.Add(key); 
			}

			return $"L{index+1}";

		}

		private string GetColor(string Caller,string Callee)
		{
			int index;
			string key;

			key = Caller + "/" + Callee;

			index = colors.IndexOf(key);
			if (index == -1)
			{
				index = colors.Count;
				colors.Add(key);
			}

			return colorManager.GetColorString(index);


		}


		private async Task FormatKeyFrameAsync(CancellationToken CancellationToken, ProjectViewModel Project,KeyFrameViewModel KeyFrame,DateTime FirstEvent)
		{

			KeyFrame.TimeSpan = KeyFrame.Timestamp - FirstEvent;
			if (KeyFrame.TimeSpan.TotalSeconds < 1)
			{
				KeyFrame.TimeSpanDisplay = $"{KeyFrame.TimeSpan.Milliseconds}ms";
			}
			else if (KeyFrame.TimeSpan.TotalMinutes < 1)
			{
				KeyFrame.TimeSpanDisplay = $"{KeyFrame.TimeSpan.Seconds}.{KeyFrame.TimeSpan.Milliseconds}s";
			}
			else if (KeyFrame.TimeSpan.TotalHours < 1)
			{
				KeyFrame.TimeSpanDisplay = $"{KeyFrame.TimeSpan.Minutes}m{KeyFrame.TimeSpan.Seconds}.{KeyFrame.TimeSpan.Milliseconds}s";
			}
			else
			{
				KeyFrame.TimeSpanDisplay = $"{KeyFrame.TimeSpan.Hours}h{KeyFrame.TimeSpan.Minutes}m{KeyFrame.TimeSpan.Seconds}.{KeyFrame.TimeSpan.Milliseconds}s";
			}


			await foreach (CallViewModel call in KeyFrame.Calls.ToAsyncEnumerable())
			{
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					break;
				}
				call.LegName = GetLegName(call.CallID,call.SourceDevice,call.DestinationDevice);

				if (call.ReplacedCallID == null) call.LegDescription = call.LegName;
				else call.LegDescription = $"{call.LegName} (replaces {GetLegName(call.ReplacedCallID,call.SourceDevice, call.DestinationDevice)})";

				call.Color = GetColor(call.Caller, call.Callee);

				call.MessageIndicesDescription = string.Join(',', call.MessageIndices.Select(index=>$"[{index}]"));

				
			}
		}
	


		public async Task FormatKeyFramesAsync(CancellationToken CancellationToken, ProjectViewModel Project, IDataSource DataSource, string Path)
		{
			DateTime firstEvent;

			LogEnter();

			if (Project.KeyFrames.Count == 0) return;
			firstEvent = Project.KeyFrames[0].Timestamp;

			await foreach (KeyFrameViewModel keyFrame in Project.KeyFrames.ToAsyncEnumerable())
			{
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					break;
				}
				await FormatKeyFrameAsync(CancellationToken, Project, keyFrame,firstEvent);
			}
		
		}


	}
}
