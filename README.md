# Bank-Management
An ASP.NET Core application that uses .NET entitycoreframework to simulate bank accounts and transactions

## This was for an assessment for a software engineer role at BOC Bank in Amarillo, TX

## NuGet Packages Needed to Be Installed before running:
- EPPlus
- Microsoft.EntityFramework.Core
- Microsoft.EntityFramework.Core.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.InMemory
- Swashbuckle.AspNetCore
- Xunit

## Description on how to run:
- Clone the app using git clone https://github.com/srawlani22/Bank-Management.git
- Open the BankManagement.sln file in the cloned folder, make sure you have Visual Studio installed for this.
- Navigate to appsettings.json and find the connection string for the database. Change that to the local server and the name of your database, and server credentials if needed.
- Go to Package Manager Console and run the command add-migration "name" and then update-database. This should create the tables and the columns in the database.
- Now, run the solution. This will launch a swagger portal that will have all the API endpoints to run this.
- Start with running the POST endpoint for creating an Account. The endpoint name is /account/api/Account. Copy and Paste the given **"Data.json"** file here. This should create two accounts and transactions for those accounts.
  
![alt text](https://github.com/srawlani22/Bank-Management/blob/main/Images/create_Accounts_Transactions.jpg)

- Now, let's try to add more transactions to an account. Now use the endpoint POST /transaction.api/Transaction. Enter the account_id which will contain the transaction, the amount, the description, and the transaction type. If you're depositing the amount, choose 0, if withdrawing, choose 1. You can put as many transactions on an account.
  
![alt text](https://github.com/srawlani22/Bank-Management/blob/main/Images/post_transaction.jpg)

- To delete a transaction, use the Endpoint DELETE /transaction/api/Transaction. You just need the transaction id for this.
  
![alt text](https://github.com/srawlani22/Bank-Management/blob/main/Images/delete_transaction.jpg)

- To update a transaction, use Endpoint PUT /transaction/api/Transaction. You need the transaction_id, amount, description, and whether to debit or credit. Set the flag to true if its a credit, it is false.
  
![alt text](https://github.com/srawlani22/Bank-Management/blob/main/Images/put_transaction.jpg)

- To export the details to spreadsheets, use the endpoint GET /account/api/Account/export/{id}. This will give you an Excel file with two spreadsheets, one for the account itself and one for all the transactions on the account. You should be able to download the file from the API response.

![alt text](https://github.com/srawlani22/Bank-Management/blob/main/Images/export_account.jpg)

- To run the unit tests. In the top menu bar in Visual Studio, click on Test -> Run All Tests. In total, there are 11 unit tests to check various account conditions. More creation and testing can be done via the functions listed above.
