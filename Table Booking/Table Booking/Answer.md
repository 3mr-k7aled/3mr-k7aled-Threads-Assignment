
Student Name: Amr Khaled Mansour
Group / ID: .net 3

### Run the Project

I ran the original project 5 times to observe the final booking totals The application creates 40 customer thread 
but the restaurant has only 5 tables available

### Results Before the Fix

first run => Booked=18, TablesLeft=-13
second run => Booked=22, TablesLeft=4
third run => Booked=36, TablesLeft=-31
fourth run => Booked=28, TablesLeft=4
fifth run => Booked=29, TablesLeft=-17

### What Is Wrong and Why?

The problem is a race condition in the Book method Multiple threads can check the value of TablesLeft at the same time If a table is available
more than one thread may pass the check before any of them updates the counters This happens because checking the available tables and updating the 
booking counters are not protected as one operation...
As a result the final booking totals may become incorrect when many customers try to book simultaneously
The Thread.Sleep(20) call does not prevent this problem It only pauses the current thread and does not protect the shared data

### solution

I fixed the problem by using a lock statement in the Book method I added a private lock object and placed the 
availability check, delay, counter updates, and booking message inside the lock
This ensures that only one thread at a time can execute the protected booking code for the same booking object
The availability check and counter updates are now performed safely, preventing multiple customers from booking the same remaining table
I did not need to change Program.cs because it already creates the customer threads and waits for all of them to finish

### Results After the solution

first run => Booked=5, TablesLeft=0
second run => Booked=5, TablesLeft=0	
third run => Booked=5, TablesLeft=0
fourth run => Booked=5, TablesLeft=0
fifth run => Booked=5, TablesLeft=0

### summary

Using lock protects the shared booking data from concurrent access This ensures that the number of successful bookings never 
exceeds the number of available tables...The solution preserves the existing program structure while making the booking operation thread-safe
