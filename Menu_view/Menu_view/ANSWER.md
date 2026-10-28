

###  Expected vs Actual

The program creates 8 threads and each thread registers 50,000 views

The expected number of views is => 8 × 50,000 = 400,000

### run results Before the Fix 

Before fixing the code I ran the program 5 times and compared the expected result with the actual result

first run => Expected = 400000 ..... Actual = 176732
second run => Expected = 400000 ..... Actual = 120927
third run => Expected = 400000 ..... Actual =  234169
fourth run => Expected = 400000 ..... Actual = 153456
fifth run => Expected = 400000 ..... Actual = 188458

The actual result could be lower than 400,000 because multiple threads were updating the same counter simultaneously The exact result could vary between runs

### After the Fix

After protecting the counter update with lock the expected and actual results matched

first run => Expected = 400000 ..... Actual = 400000
second run => Expected = 400000 ..... Actual = 400000
third run => Expected = 400000 ..... Actual = 400000 
...etc

###  Why Do the Numbers Disagree?

The original code used:


Views++;


This operation is not atomic It involves reading the current value incrementing it and writing the new value
When multiple threads execute this operation at the same time two threads can read the same value and overwrite
each other's updates This is called a race condition and it can cause lost updates
As a result the actual number of views can be less than the expected number

### Fix RegisterView

I fixed the problem by using a lock statement to protect the counter update.


private readonly object _lock = new();

public void RegisterView()
{

    lock (_lock)
    {
        Views++;
    }
}


The lock statement ensures that only one thread at a time can execute the code inside the protected section
This prevents simultaneous updates to Views ensuring that all 400000 increments are counted correctly

### Summary

The original code had a race condition because multiple threads updated the Views counter simultaneously using Views++
This caused lost updates making the actual result potentially lower than the expected 400000
I fixed the problem by using a lock statement inside RegisterView() to ensure that only one thread updates the counter at a time
After the fix the expected and actual results matched 400000 views
I would use Interlocked.Increment for a simple counter but lock is useful when multiple related operations must be protected together
