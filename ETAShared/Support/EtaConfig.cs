using System.Threading.Tasks;

namespace EtaShared
{
	/// <summary>
	/// Configuration information.
	/// </summary>
	public sealed class EtaConfig
	{
		public EtaConfig (string hostName)
		{
			this.Host = hostName;
		}

		/// <summary>
		/// The address of the host to connect to. In my network, this is the IP "192.168.178.35".
		/// Default communication of the EPA services is done on port 8080.
		/// </summary>
		public string Host
		{
			get
			{
				return this.host;
			}
			private set
			{
				this.host = value;
				if(this.host != null)
				{
					/*
					this.host.Trim();
					this.host = this.host.Replace("https:", "");
					this.host = this.host.Replace("http:", "");
					*/
					this.host = this.host.Trim().Trim('/');
				}
			}
		}
		string host;

		/// <summary>
		/// Gets the complete connetion address (e.g. "http://123.122.121:8080");
		/// </summary>
		public string ConnectionAddress
		{
			get
			{
				if(string.IsNullOrWhiteSpace(this.Host))
				{
					return null;
				}

				return this.Host;
			}
		}

		public override string ToString () => $"[EtaConfig: Host={this.Host}]";
	}
}

