

## Failures Before the Fix

Before fixing the code I ran the project and encountered concurrency exceptions while multiple threads tried to update the same Dictionary

The error is =>

InvalidOperationException => The Dictionary was corrupted because multiple threads modified it concurrently
ArgumentException => Multiple threads tried to add the same product key koshary-bowl at the same time
The program failed before it could complete normally

## solution

I replaced Dictionary<string, int> with ConcurrentDictionary<string, int>
I also replaced the original ContainsKey, Add, and increment operations with => Views.AddOrUpdate(product, 1, (key, oldValue) => oldValue + 1);
This safely adds a new product or increments its view count when multiple threads access the dictionary concurrently

## Email Jobs

I replaced Queue<string> with ConcurrentQueue<string> and I used TryDequeue() instead of checking Count before calling Dequeue()
this safely retrieves jobs without throwing an exception when the queue is empty
I kept the lock around SentEmails.Add(job) because sentEmails is still a regular List<string> that multiple workers can modify
The workers stop when the producers have finished and the queue is empty

##  Why Is a Thread-Safe Collection Not Enough ?

A thread-safe collection protects its own operations but it does not automatically make a sequence of business operations atomic

For example => checking whether a key exists and then adding or updating it involves multiple steps With a regular Dictionary, another thread 
could change the collection between those steps
Similarly checking whether a Queue contains items and then removing an item is unsafe when performed as separate operations on a regular Queue
Using ConcurrentDictionary and ConcurrentQueue provides safer individual collection operations However the application must still coordinate related 
operations and handle its business rules correctly, including when workers should stop and how processed emails are recorded

## Final Test Results

After the fix, the project ran successfully with the following output =>

Views expected=50000, actual=50000
Email jobs placed=20, unique sent=20, total sends=20

The view count matched the expected value of 50,000 All 20 email jobs were sent exactly once with no duplicate sends or concurrency exceptions in this run
