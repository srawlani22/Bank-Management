# Bank-Management
An ASP.NET Core application that uses .NET entitycoreframework to simulate bank accounts and transactions

## This was for an assessment for a software engineer role at BOC Bank in Amarillo, TX

## Description on how to run:
- Clone the app using git clone https://github.com/srawlani22/Bank-Management.git
- Open the BankManagement.sln file in the cloned folder, make sure you have visual studio installed for this.
- Navigate to appsettings.json and find the connection string for the database. Change that to local server and the name of your database, and server credentials if needed.
- Go to Package Manager Console and run the command add-migration "name" and then update-database. This should create the tables and the columns in the database.
- Now, run the solution. This will launch a swagger portal that will have all the API endpoints to run this.
- Start with running the POST endpoint for creatign an Account. The endpoint name is /account/api/Account. Enter the values, the datatypes are shown below. This will also create the initial transaction for that account. If trying to get an account info, use endpoint -GET /account/api/Account.
![alt text](post_account.jpg)

- Now, lets try to add more transactions to an account. Now use the endpoint POST /transaction.api/Transaction. Enter the account_id which will contain the trasaction, the amount, the description and the transcation type. If you're depositing the amount, choose 0, if withdrawing, choose 1. You can put as many transactions on an account.
![alt text](post_transaction.jpg)

- To delete a transaction, use the Endpoint DELETE /transaction/api/Transaction. You just need the transaction id for this.
![alt text](delete_transaction.jpg)

- To update a transaction, use Endpoint PUT /transaction/api/Transaction. You need the transaction_id, amount, description, and weather to debit or credit. Set the flag to true if its a credit, otherwise false.
![alt text](update_transaction.jpg)

- To export the details to spreadsheets, use the endpoint GET /account/api/Account/export/{id}. This will give you an excel file with two spreahsheets, one for the account itself and one for all the transactions on the account. You should be able to download the file from the api response. 
![alt text](export_account_details.jpg)

