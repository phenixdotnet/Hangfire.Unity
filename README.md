Hangfire.Unity
=========

This project is based on the [Hangfire.Ninject](https://github.com/HangfireIO/Hangfire.Ninject) project.

[Hangfire](http://hangfire.io) background job activator based on 
[Unity](https://unity.codeplex.com/) IoC Container. It allows you to use instance
methods of classes that define parametrized constructors:

Thanks to mwillebrands for update and unit tests

```csharp
public class EmailService
{
	private DbContext _context;
    private IEmailSender _sender;
	
	public EmailService(DbContext context, IEmailSender sender)
	{
		_context = context;
		_sender = sender;
	}
	
	public void Send(int userId, string message)
	{
		var user = _context.Users.Get(userId);
		_sender.Send(user.Email, message);
	}
}	

// Somewhere in the code
BackgroundJob.Enqueue<EmailService>(x => x.Send(1, "Hello, world!"));
```

Improve the testability of your jobs without static factories!

Installation
--------------

Hangfire.Unity is available as a NuGet Package. Type the following
command into NuGet Package Manager Console window to install it:

```
Install-Package Hangfire.Unity
```

Usage
------

The package provides an extension method for [OWIN bootstrapper](http://docs.hangfire.io/en/latest/users-guide/getting-started/owin-bootstrapper.html):

```csharp
app.UseHangfire(config =>
{
    var container = new UnityContainer();
	// Setup your unity container
	
	// Use it with Hangfire
    config.UseUnityActivator(container);
});
```

In order to use the library outside of web application, set the static `JobActivator.Current` property:

```csharp
var container = new UnityContainer();
JobActivator.Current = new UnityJobActivator(container);
```


Thanks to all contributors
------
(in no special order):
- indiaweblab
- NiSHoW 
- mwillebrands
- victor-ponce 
- xavero
- Kesmy