namespace Application.Pages
{
	/// <summary>
	/// https://learn.microsoft.com/en-us/dotnet/api/microsoft.data.sqlclient.sqlconnectionencryptoption
	/// </summary>
	public enum EncryptMode
	{
		False,
		Mandatory,
		Optional,
		Strict,
		True
	}
}