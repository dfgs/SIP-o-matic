using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.DataSources
{
	public class DataSourceManager
	{
		private List<IDataSource> dataSources;

		public DataSourceManager()
		{
			this.dataSources = new List<IDataSource>();
		}

		public IEnumerable<IDataSource> GetDataSourceForFile(string FileName)
		{
			string ext;

			ext = System.IO.Path.GetExtension(FileName).TrimStart('.').ToLower();
			
			foreach(IDataSource source in this.dataSources)
			{
				if (source.GetSupportedFileExts().Contains(ext)) yield return source;	
			}
		}

		public void Register(IDataSource DataSource)
		{
			this.dataSources.Add(DataSource);
		}


	}
}
