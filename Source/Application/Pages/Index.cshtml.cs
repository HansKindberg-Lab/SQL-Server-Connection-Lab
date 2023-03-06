using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace Application.Pages
{
	public class IndexModel : PageModel
	{
		#region Fields

		private Lazy<string> _certificateSearch;
		private Lazy<string> _connectionString;
		private ConnectForm _form;
		private Lazy<string> _maskedConnectionString;
		private Lazy<string> _maskedConnectionStringBuilderString;
		private SqlConnectionStringBuilder _sqlConnectionStringBuilder;

		#endregion

		#region Constructors

		public IndexModel(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
		}

		#endregion

		#region Properties

		public virtual string CertificateSearch
		{
			get
			{
				this._certificateSearch ??= new Lazy<string>(() =>
				{
					var certificateSection = this.Configuration.GetSection("CertificateSearch");

					if(!certificateSection.GetChildren().Any())
						return "No certificate-search configured.";

					var options = new CertificateSearchOptions();
					certificateSection.Bind(options);

					try
					{
						using(var store = new X509Store(options.StoreName, options.StoreLocation, OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly))
						{
							var certificates = store.Certificates.Find(options.FindType, options.FindValue, options.ValidOnly);

							return $"Found {certificates.Count} certificates with options \"{options}\": {string.Join(", ", certificates.Select(certificate => certificate.Subject))}";
						}
					}
					catch(Exception exception)
					{
						var message = $"Error when searching certificate with the following options: {options}";
						this.Logger.LogError(exception, message);
						return message;
					}
				});

				return this._certificateSearch.Value;
			}
		}

		protected virtual IConfiguration Configuration { get; }

		protected virtual string ConnectionString
		{
			get
			{
				this._connectionString ??= new Lazy<string>(() => this.Configuration.GetConnectionString("Database"));

				return this._connectionString.Value;
			}
		}

		public virtual TimeSpan? Duration { get; set; }
		public virtual Exception Exception { get; set; }

		public virtual ConnectForm Form
		{
			get
			{
				// ReSharper disable InvertIf
				if(this._form == null)
				{
					var form = new ConnectForm
					{
						ConnectTimeout = this.SqlConnectionStringBuilder.ConnectTimeout,
						Encrypt = Enum.Parse<EncryptMode>(this.SqlConnectionStringBuilder.Encrypt.ToString()),
						Pooling = this.SqlConnectionStringBuilder.Pooling,
						TrustServerCertificate = this.SqlConnectionStringBuilder.TrustServerCertificate
					};

					this.PopulateEncrypts(form);

					this._form = form;
				}
				// ReSharper restore InvertIf

				return this._form;
			}
			set => this._form = value;
		}

		protected virtual ILogger Logger { get; }

		public virtual string MaskedConnectionString
		{
			get
			{
				this._maskedConnectionString ??= new Lazy<string>(() => this.GetMaskedConnectionString(this.ConnectionString));

				return this._maskedConnectionString.Value;
			}
		}

		public virtual string MaskedConnectionStringBuilderString
		{
			get
			{
				this._maskedConnectionStringBuilderString ??= new Lazy<string>(() => this.GetMaskedConnectionString(this.SqlConnectionStringBuilder.ConnectionString));

				return this._maskedConnectionStringBuilderString.Value;
			}
		}

		protected virtual SqlConnectionStringBuilder SqlConnectionStringBuilder => this._sqlConnectionStringBuilder ??= new SqlConnectionStringBuilder(this.ConnectionString);
		public virtual string Success { get; set; }

		#endregion

		#region Methods

		protected internal virtual string GetMaskedConnectionString(string connectionString)
		{
			if(string.IsNullOrWhiteSpace(connectionString))
				return connectionString;

			const char keyValueSeparator = '=';
			const char partSeparator = ';';

			var parts = new List<string>();

			foreach(var part in connectionString.Split(partSeparator))
			{
				var keyValuePair = part.Split(keyValueSeparator, 2);

				if(keyValuePair.Length > 1 && string.Equals(keyValuePair[0].Trim(), "password", StringComparison.OrdinalIgnoreCase))
					parts.Add($"{keyValuePair[0]}{keyValueSeparator}**********");
				else
					parts.Add(part);
			}

			return string.Join(partSeparator, parts);
		}

		public virtual void OnPost(ConnectForm form)
		{
			var begin = DateTime.UtcNow;

			this.PopulateEncrypts(form);
			this.Form = form;

			try
			{
				var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(this.ConnectionString)
				{
					ConnectTimeout = form.ConnectTimeout,
					Encrypt = SqlConnectionEncryptOption.Parse(form.Encrypt.ToString()),
					Pooling = form.Pooling,
					TrustServerCertificate = form.TrustServerCertificate,
				};

				using(var sqlConnection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
				{
					sqlConnection.Open();
				}

				this.Success = "Connection opened.";
			}
			catch(Exception exception)
			{
				this.Logger.LogError(exception, "Error");
				this.Exception = exception;
				this.Success = null;
			}

			var end = DateTime.UtcNow;

			this.Duration = end - begin;
		}

		protected virtual void PopulateEncrypts(ConnectForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			foreach(var encrypt in Enum.GetValues<EncryptMode>())
			{
				var text = encrypt.ToString();
				form.Encrypts.Add(new SelectListItem(text, text, form.Encrypt == encrypt));
			}
		}

		#endregion
	}
}