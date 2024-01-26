### Experience of parallel refactoring.

#### Introduction

We migrate code out of MF to Azure.
Tool we use produces plain good functionally equivalent C# code.
But it turns it's not enough!

So, what's the problem?

Converted code is very slow, especially for batch processing, 
where MF completes job, say in 30 minutes, while converted code
finishes in 8 hours.

At this point usually someone appears and whispers in the ear:

> Look, those old technologies are proven by the time. It worth to stick to old good Cobol, or better to Assembler if you want to do the real thing.

We're curious though: why is there are difference?

Turns out the issue lies in differences of network topology between MF and Azure solutions.
On MF all programs, database and file storage virtually sits in a single box, thus network latency is neglible. 

It's rather usual to see chatty SQL programs on MF that are doing a lot of small SQL queries.

In Azure - programs, database, file storage are different services most certainly sitting in different phisical boxes. 
You should be thankfull if they are co-located in a single datacenter. 
So, network latency immediately becomes a factor. 
Even if it just adds 1 millisecond per SQL roundtrip, it adds up in loops, and turns in the showstopper.

There is no easy workaround on the hardware level.

People advice to write programs differently: "[Tune applications and databases for performance in Azure SQL Database](https://learn.microsoft.com/en-us/azure/azure-sql/database/performance-guidance)".

That's a good advice for a new development but discouraging for migration done by a tool.

So, what is the way forward?

Well, there is one. While accepting weak sides of Azure we can exploit its strong sides.

#### Parallel refactoring

Before continuing let's consider a code demoing the problem:

(SerialProcessor.cs)[../Services/SerialProcessor.cs#L5-L29]

