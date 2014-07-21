Photofy.Caching
===============

This is a simple caching library built for use with [AWS Elasticache](http://aws.amazon.com/elasticache/). Uses the [EnyimMemcached](https://github.com/enyim/EnyimMemcached) provider. Future addition for Redis.

I was tired of there being no good wrapper for C# .NET to use the Memcached service provided by Amazon Elasticache so I wrote this one.

If you have feedback please let me know or just fork it and have fun! I do ask if you use this library please checkout the Photofy app on [iOS](https://itunes.apple.com/us/app/photofy/id674208785?mt=8) and [Android](https://play.google.com/store/apps/details?id=com.photofy.android&hl=en) phones!

App Settings Required
---------------------
```xml
<configSections>
	<sectionGroup name="enyim.com">
		<section name="memcached" type="Enyim.Caching.Configuration.MemcachedClientSection, Enyim.Caching" />
	</sectionGroup>
</configSections>
<appSettings>
	<add key="CacheDisabled" value="false" /> <!-- This key is not required, however if not provided, the application assumes the cache is enabled and should be used -->    
</appSettings>
<enyim.com>
	<memcached protocol="Text">
		<servers>
			<add address="AWS Node Endpoint" port="11211" />
		</servers>
	</memcached>
</enyim.com>
```

Simple Use
---------------------
This is just a very simple way to test that you can add/remove an object to your node in a console app.

```
static void Main(string[] args)
{
	Console.WriteLine("== ElastiCache Test == ");

	Memached.MemcachedService.Initialize();

	var service = new Memached.MemcachedService();

	Console.WriteLine("Enter key: ");
	var key = Console.ReadLine();

	Console.WriteLine("Enter value: ");
	var value = Console.ReadLine();

	bool success = service.Add(key, value);

	Console.WriteLine(string.Format("Added key: {0} with value: {1} Response: {2}", key, value, success));

	Console.WriteLine("Hit enter to retrieve");
	Console.ReadLine();

	var val = service.Get(key);
	Console.WriteLine("Retrieve value: " + val);

	Console.ReadLine();
}
```

Ninject DI Use
---------------------
Inside your RegisterServices(IKernel kernel) method add the following:

```
kernel.Bind<Photofy.Caching.ICacheService>().To<Photofy.Caching.Memached.MemcachedService>().InSingletonScope();
            
//uses default configuration section in web.config/enyim.com/memcached
Photofy.Caching.Memached.MemcachedService.Initialize();
```

A test example for either an ApiContoller or web Controller you must pass in an instance of the service.

```
private readonly ICacheService _caching = null;
public CachingController(ICacheService caching)
{
	_caching = caching;
}

[HttpPost]
public ActionResult Add(string key, string value, int minutes)
{
	_caching.Add(key, value, minutes);
	return View("index");
}
```


Observations
---------------------
(07-2014) One of the most annoying things in regards to Amazon's Elasticache is that you will only be able to access your node(s) from within the amazon network. You cannot access from the outside even with security groups as this is just a limitation on their system.

You could create an SSH tunnel between your local network and the an EC2 instance you spin up and hit it through that however I found this to be cumbersome so I created the CacheDisabled app settings key to ignore caching locally and just managed my Memcached instance from a box behind my VPC.