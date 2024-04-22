# swapi

A simple solution showcasing proxying, caching and rate limiting.

Contents
==========

* [Problem](#problem)
* [Assumptions](#assumptions)
* [Structure](#structure)
* [General Approach](#general-approach)
* [Abstractions](#abstractions)
* [Next steps](#next-steps)

### Problem
---

The goal is to allow multiple users to query the swapi API, while aggregating results. An example would be to retrieve all the planets at once, as opposed to paginated.
Relevant aspects: 
- the backend plane of swapi.dev will rate limit us, both for a total amount of requests per day, as well as for smaller windows (15 mins from the looks of their source code).

### Assumptions
----

- The implementation balances execution speed, code flexibility and simplicity (due to its limited scope).
- The type of metadata we expose has a very slow change rate (e.g. a new movie gets added very infrequently).

- This would be an actual service used by multiple users. Thus, we should be minimizing downtime. For this, having no single point of failure is essential.
- With this in mind, multiple instances need to be deployed. 
- As we want to rate limit the access, we could either turn towards a "stateful" solution (e.g. sticky sessions, load balancing the users to the same instances all the time)
- Alternatively, we can use a distributed rate limiter approach. This is the chosen solution, as it enables our instances to be completely stateles. However, this adds a dependency to a redis deployment (which might, of course, fail). 
- Using a hosted redis (e.g. on Azure or AWS) should partially mitigate this due to their good uptimes. Nonetheless, when redis is down, so will our service. Alternatively, we could chose to allow all requests to go through if we cannot reach redis, but this might result in us exhausing our quota on the backend plane. The proper solution is to monitor how the system behaves and react accordingly. 

### Structure
---

The `Swapi` project contains all the logic, while `Swapi.Tests` contains a mix of Unit and Integration tests. The tests are just intended to hightligh the overall approach for testing the various components (i.e. by mocking their injected dependencies and testing in isolation), and are by no means exhaustive.

Use `dotnet run` to start the kestrel server. This assumes you have a Redis server working on the default 6379 port. 
User `dotnet test` to run the test suite. 

### General
---

Most of the time will be spent waiting for the http calls to return, so our application is I/O bound. 
For a single query, there are no options for parallelization. When getting an aggregate however, there are multiple options.


#### Approach 1: Sending the requests serially
---

**Pros**

- simpler to reason about: we pull the first page and continue as long as there's a nextPage
- no need for an intermediate layer to distribute the work, or any concurrent collections
- when used in combination with caching (with a properly configured TTL), the performance overhead gets amortized over the requests that will hit the cache. Since we don't have too many pages in our current example, it might be worth taking the performance hit once in a while, in order to keep things simpler.

**Cons**

- will take significantly longer when there's a large number of pages
- if we don't use caching, we'll continuously hit this inefficiency for every request

#### Approach 2: Parallelizing the pages

**Pros**

- faster in some scenarios (see above)

**Cons**

- might get throttled by the backplane if we send too many requests at once
- the implementation is more complex
- requires querying to determine the number of pages that have to be polled

#### Project choice - parallelized requests

- to offer the most performance, I chose to parallelize the requests when possible. 
- while this might not offer the most benefit today, it would in an actual project, where we might have significantly more pages and more frequently changing data.



TODO

- unit tests - DONE
- caching + decorator - DONE, with conditional registration
- multiple strategies for partitioning the requests - DONE
- docker bundling with redis - docker-compose done

- proper readme
- code cleanup
- send email with repo url - dimineata