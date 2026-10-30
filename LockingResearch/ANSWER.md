# Locking Research

## Q1 : What is System.Threading.Monitor used for?

System.Threading.Monitor is used to synchronize threads when they access shared data because It allows only one thread at a time to enter a critical section protected by the same lock object so This helps prevent race conditions when multiple threads try to modify shared data and It also provides methods such as Wait,Pulse and PulseAll to coordinate threads

## Q2 : What does the C# lock statement compile to?

For a normal object used as the lock target the C# lock statement uses Monitor.Enter and Monitor.Exit to acquire and release the lock and The compiler generates a try-finally structure to ensure that the lock is released even if an exception occurs inside the critical section
In modern C# locking an instance of System.Threading.Lock has different compiler behavior, using that type's locking mechanism instead

## Q3 : List at least two differences between using lock and calling Monitor APIs directly.

1. Simplicity and safety : The lock statement automatically handles releasing the lock through a finally block When using Monitor.Enter and Monitor.Exit directly I must ensure that Monitor.Exit is called in a finally block

2. More control : The Monitor APIs provide methods such as TryEnter which allows a thread to attempt to acquire a lock without waiting forever I can also specify a timeout

3. Thread coordination : Monitor.Wait,Monitor.Pulse,and Monitor.PulseAll provide additional ways for threads to wait for a condition and notify other waiting threads A plain lock statement does not provide these operations directly

## Q4 : When might a developer use Monitor.TryEnter,Monitor.Pulse,or Monitor.Wait instead of a plain lock?

Monitor.TryEnter : When a thread should attempt to acquire a lock but should not wait indefinitely For example it can try to acquire the lock for a limited time and perform another action if the attempt fails

Monitor.Wait : When a thread needs to wait until a shared object's state changes It releases the monitor lock while waiting and reacquires it before returning

Monitor.Pulse : When a thread changes a shared object's state and needs to notify one waiting thread that it may be able to continue

These methods are useful when threads need more control over waiting and notification than a normal lock statement provides

## Q5 : Write the same critical section twice: once with lock, once with Monitor.Enter / Monitor.Exit.

The following examples protect the same shared counter.

### Using lock

private readonly object \_sync = new();
private int \_counter;

public void Increment()
{
lock (\_sync)
{
\_counter++;
}
}

### Using Monitor.Enter / Monitor.Exit

using System.Threading;

private readonly object \_sync = new();
private int \_counter;

public void Increment()
{
Monitor.Enter(\_sync);
try
{
\_counter++;
}
finally
{
Monitor.Exit(\_sync);
}
}

Both examples protect the counter from simultaneous updates by threads using the same lock object and The Monitor version requires more code because I must explicitly release the lock in finally

## resources

1. Microsoft Learn — Monitor Class:
   https://learn.microsoft.com/en-us/dotnet/api/system.threading.monitor

2. Microsoft Learn — Monitor.Enter Method:
   https://learn.microsoft.com/en-us/dotnet/api/system.threading.monitor.enter

3. Microsoft Learn — Monitor.Wait Method :
   https://learn.microsoft.com/en-us/dotnet/api/system.threading.monitor.wait
