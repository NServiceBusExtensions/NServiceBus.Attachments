# <img src="/src/icon.png" height="25px"> NServiceBus.Attachments

[![Build status](https://ci.appveyor.com/api/projects/status/6483bemehfuowaa2/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/nservicebus-attachments)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.Attachments.FileShare.svg?label=Attachments.FileShare)](https://www.nuget.org/packages/NServiceBus.Attachments.FileShare/)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.Attachments.FileShare.Raw.svg?label=Attachments.FileShare.Raw)](https://www.nuget.org/packages/NServiceBus.Attachments.FileShare.Raw/)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.Attachments.Sql.svg?label=Attachments.Sql)](https://www.nuget.org/packages/NServiceBus.Attachments.Sql/)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.Attachments.Sql.Raw.svg?label=Attachments.Sql.Raw)](https://www.nuget.org/packages/NServiceBus.Attachments.Sql.Raw/)

Adds a streaming based attachment functionality to [NServiceBus](https://docs.particular.net/nservicebus/).

<!--- StartOpenCollectiveBackers -->

[Already a Patron? skip past this section](#endofbacking)


## Community backed

**It is expected that all developers either [become a Patron](https://opencollective.com/nservicebusextensions/contribute/patron-6976) or have a [Tidelift Subscription](#support-via-tidelift) to use NServiceBusExtensions. [Go to licensing FAQ](https://github.com/NServiceBusExtensions/Home/#licensingpatron-faq)**


### Sponsors

Support this project by [becoming a Sponsor](https://opencollective.com/nservicebusextensions/contribute/sponsor-6972). The company avatar will show up here with a website link. The avatar will also be added to all GitHub repositories under the [NServiceBusExtensions organization](https://github.com/NServiceBusExtensions).


### Patrons

Thanks to all the backing developers. Support this project by [becoming a patron](https://opencollective.com/nservicebusextensions/contribute/patron-6976).

<img src="https://opencollective.com/nservicebusextensions/tiers/patron.svg?width=890&avatarHeight=60&button=false">

<a href="#" id="endofbacking"></a>

<!--- EndOpenCollectiveBackers -->


## Support via TideLift

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-nservicebus.attachments.sql?utm_source=nuget-nservicebus.attachments.sql&utm_medium=referral&utm_campaign=enterprise).


toc


## NuGet packages

 * https://www.nuget.org/packages/NServiceBus.Attachments.FileShare
 * https://www.nuget.org/packages/NServiceBus.Attachments.FileShare.Raw
 * https://www.nuget.org/packages/NServiceBus.Attachments.Sql
 * https://www.nuget.org/packages/NServiceBus.Attachments.Sql.Raw


## Compared to the DataBus

This project delivers similar functionality to the [DataBus](https://docs.particular.net/nservicebus/messaging/databus/). However it does have some different behavior:


### Read on demand

With the DataBus all binary data is read every message received. This is irrespective of if the receiving endpoint requires that data. With NServiceBus.Attachments data is explicitly read on demand, so if data is not required there is no performance impact. NServiceBus.Attachments also supports processing all data items via an `IAsyncEnumerable`.


### Memory usage

With the DataBus all data items are place into byte arrays. This means that memory need to be allocated to store those arrays on either reading or writing. With NServiceBus.Attachments data can be streamed and processed in an async manner. This can significantly decrease the memory pressure on an endpoint.


### Variety of data APIs

With the DataBus the only interaction is via byte arrays. NServiceBus.Attachments supports reading and writing using streams, byte arrays, or string.


## SQL

[Full Docs](/docs/sql.md)


## FileShare

[Full Docs](/docs/fileshare.md)


## Icon

[Gecko](https://thenounproject.com/term/gecko/258949/) designed by [Alex Podolsky](https://thenounproject.com/alphatoster/) from [The Noun Project](https://thenounproject.com/).