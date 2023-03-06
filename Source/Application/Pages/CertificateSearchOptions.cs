using System.Security.Cryptography.X509Certificates;

namespace Application.Pages
{
	public class CertificateSearchOptions
	{
		#region Properties

		public virtual X509FindType FindType { get; set; } = X509FindType.FindBySubjectName;
		public virtual object FindValue { get; set; }
		public virtual StoreLocation StoreLocation { get; set; } = StoreLocation.LocalMachine;
		public virtual StoreName StoreName { get; set; } = StoreName.Root;
		public virtual bool ValidOnly { get; set; }

		#endregion

		#region Methods

		public override string ToString()
		{
			return $"FindType={this.FindType}, FindValue={this.FindValue}, StoreLocation={this.StoreLocation}, StoreName={this.StoreName}, ValidOnly={this.ValidOnly}";
		}

		#endregion
	}
}