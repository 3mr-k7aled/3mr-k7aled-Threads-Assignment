
# Invoice Export Assignment

Student Name: Amr Khaled Mansour
Group : 3

### Hint 
i cann't write original output because it's had .net 9 and i can't  download or run it 
so i will write the output after changes only and i will explain the changes i made in the project

##  Design Problems

The original implementation has several problems when processing many orders:
Blocking operations => The code uses Thread.Sleep , File.ReadAllText and File.WriteAllText These operations block the current thread
instead of allowing asynchronous operations and The program processes each order completely before starting the next one
This increases the total execution time when there are many orders

Repeated database calls => The program first retrieves all pending orders then retrieves every order again using its ID
This adds unnecessary delay and repeated work

Scalability => As the number of pending orders increases the sequential processing approach becomes slower

##  Changes 

I updated the project to use asynchronous programming with async and await

### At OrderDatabase

Changed GetPendingOrders ==> to GetPendingOrdersAsync
Changed GetOrderById ==> to GetOrderByIdAsync
 Both methods now return Tasks and use await Task.Delay to simulate asynchronous database operations

### at CustomerFileStore

 Changed ReadProfile ==> to ReadProfileAsync
 Used File.ReadAllTextAsync to read customer files asynchronously

### at InvoiceFileWriter

 Changed WriteInvoice ==> to WriteInvoiceAsync
 Used File.WriteAllTextAsync to write invoice files asynchronously

### at InvoiceExportService

 Changed ExportAllPending ==> to ExportAllPendingAsync
Created a task for each order using ExportOrderAsync
Started processing the orders without waiting for each order to finish before starting the next one
Awaited the tasks one by one without using Task.WhenAll

### at Program.cs

Used await export.ExportAllPendingAsync() to run the asynchronous export process

### Why These Changes Help

Asynchronous operations allow the program to avoid blocking the current thread while waiting for operations to finish

Starting the order-processing tasks before awaiting them allows multiple orders to be processed concurrently during their asynchronous waits

The business flow and invoice content remain the same. The program should still generate five invoice files for orders 1001 through 1005

 Task.Delay simulates waiting time for this assignment It does not turn the in-memory database into a real asynchronous database

### After the Changes

Starting invoice export...
Customer files: C:\Users\de\Desktop\InvoiceExport\InvoiceExport\bin\Debug\net10.0\Data
Invoice output: C:\Users\de\Desktop\InvoiceExport\InvoiceExport\bin\Debug\net10.0\invoices

Wrote C:\Users\de\Desktop\InvoiceExport\InvoiceExport\bin\Debug\net10.0\invoices\invoice-1001.txt
Wrote C:\Users\de\Desktop\InvoiceExport\InvoiceExport\bin\Debug\net10.0\invoices\invoice-1002.txt
Wrote C:\Users\de\Desktop\InvoiceExport\InvoiceExport\bin\Debug\net10.0\invoices\invoice-1003.txt
Wrote C:\Users\de\Desktop\InvoiceExport\InvoiceExport\bin\Debug\net10.0\invoices\invoice-1004.txt
Wrote C:\Users\de\Desktop\InvoiceExport\InvoiceExport\bin\Debug\net10.0\invoices\invoice-1005.txt
Export finished in 1.8 s

C:\Users\de\Desktop\InvoiceExport\InvoiceExport\bin\Debug\net10.0\InvoiceExport.exe (process 86472) exited with code 0 (0x0).
Press any key to close this window . . .


### Summary

The original implementation processed orders sequentially using blocking operations The updated implementation uses asynchronous methods
and starts the processing tasks for multiple orders before awaiting their results
This improves how the program handles waiting operations while preserving the existing invoice format and business flow
