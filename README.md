ProgReporter
===============

ProgReporter is a distribution and usage statistcis service for windows desktop applications.


Goals
-----

* Easy for integration
* Does not disturb the program's work in any way
* Transparrent for the programs users
* Support .NET 2.0 and .NET 4.0
* Count users, runs, geo distribution, features usage, license distribution, detect sales

Info
----

All resources about ProgReporter can be found at [http://ProgReporter.com/](http://ProgReporter.com/)

For additional information see [http://ProgReporter/docs/](http://ProgReporter/docs/)


Usage
-----

* Link ProgReporter.dll to your program and add a reference to it in your code: `using ProgReporter;`

* Set and run the service when your program starts:

```csharp
private ProgStats stats;

private void MainForm_Load(object sender, EventArgs e)
{
	stats = new ProgStats();

	// If your application uses licensing, you can set the license type of your app.
	// Available license types are: Free, Trial, Expired, Valid, NotValid, Unknown
	stats.AppLicenseType = LicenseType.Free;

	// Sets application version.
	stats.AppVersion = Application.ProductVersion;

	// Begin proceeding stats
	// Sets the particular application Id.
	stats.AppStart("cbc15a23946e34067c0085b2087ac33bf221a7d5");
}
```

* Stop the service when your program is going to stop

```csharp
private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
{
	// Tells ProgReporter that you are going to stop your application
	stats.AppStop();
}
```


----

Copyright (c) Miroslav Popov. All rights reserved.

See the License Agreement for license info.


