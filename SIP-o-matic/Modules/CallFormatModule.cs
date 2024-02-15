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

		private string GetLegName(string CallID,string SourceDevice)
		{
			int index;
			string key;

			key = CallID+"/"+SourceDevice;

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


		private async Task FormatKeyFrameAsync(CancellationToken CancellationToken, ProjectViewModel Project,KeyFrameViewModel KeyFrame)
		{
			await foreach (CallViewModel call in KeyFrame.Calls.ToAsyncEnumerable())
			{
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					break;
				}
				call.LegName = GetLegName(call.CallID,call.SourceDevice);
				call.LegDescription = call.LegName;

				call.Color = GetColor(call.Caller, call.Callee);

				call.MessageIndicesDescription = string.Join(',', call.MessageIndices.Select(index=>$"[{index}]"));

				/*reader = new StringReader(message.Content, ' ');
				try
				{
					sipMessage = SIPGrammar.SIPMessage.Parse(reader);
				}
				catch (Exception ex)
				{
					string error = $"Failed to decode SIP message [{message.Index}]:\r\n" + ex.Message + "\r\n" + message.Content;
					Log(LogLevels.Error, error);
					throw new InvalidOperationException(error);
				}//*/

			}
		}
	


		public async Task FormatKeyFramesAsync(CancellationToken CancellationToken, ProjectViewModel Project, IDataSource DataSource, string Path)
		{

			LogEnter();

			await foreach (KeyFrameViewModel keyFrame in Project.KeyFrames.ToAsyncEnumerable())
			{
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					break;
				}
				await FormatKeyFrameAsync(CancellationToken, Project, keyFrame);
			}
		
		}


	}
}
