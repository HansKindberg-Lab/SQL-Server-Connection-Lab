# SQL-Server-Connection-Lab

Application for laborating with SQL Server connections in Linux containers.

## 1 Development

The connection string in [appsettings.Development.json](/Source/Application/appsettings.Development.json#L4) is just an example. If you want to run during development you can add another connection-string in your **secrets.json**. Right-click on the Application-project and click **Manage User Secrets**.

- C:\Users\{USER_NAME}\AppData\Roaming\Microsoft\UserSecrets\0ba4f750-0032-4e68-82f0-0a57c3036ed5\secrets.json

There you can add for example:

	{
		"ConnectionStrings": {
			"Database": "Server=your-db.example.com;Database=SQL-Server-Connection-Lab;User Id=SQL-Server-Connection-Lab;Password=P@ssword12;Encrypt=Mandatory;Pooling=False;Connect Timeout=4"
		}
	}

## 2 [Certificates](/Certificates)

Just an example.

- **root-ca.crt:** *CN=Root CA, DC=example, DC=org* to get encrypted connection to database-server db01.example.org to work.