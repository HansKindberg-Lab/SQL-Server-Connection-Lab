using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.Pages
{
	public class ConnectForm
	{
		#region Properties

		/// <summary>
		/// Connect timeout in seconds.
		/// </summary>
		public virtual int ConnectTimeout { get; set; }

		public virtual EncryptMode Encrypt { get; set; }
		public virtual IList<SelectListItem> Encrypts { get; } = new List<SelectListItem>();
		public virtual bool Pooling { get; set; }
		public virtual bool TrustServerCertificate { get; set; }

		#endregion
	}
}