# Cko Test Solution

## What is this?
This is a monorepo, containing the implementation of two different .NET core APIs:
1. Cko.BankSimulator: *A bank system simulator*
2. Cko.PaymentGateway: *A payment gateway simulator*

The two projects above, are `.NET core 3.1` projects, that contain the root DI setup logic in their `Startup.cs` files, as well as two different entry points: One for *local execution* (``LocalEntryPoint.cs``) and one for *AWS Lambda execution* (``LambdaEntryPoint.cs``). As you may have well guessed, they were created using the Lambda templates for .NET. 

These projects include the *Controllers* for the APIs, each in the `Controllers/TransactionController.cs` files, as well as the `serverless.template.json` files, which contain AWS Cloudformation descriptions of how AWS should build their infrastructure.

At the root of the repo, under the `.github` folder, one can find the github workflows that will test, build and deploy the two APIs to AWS Lambda.

## Invoking the APIs from AWS Lambda
As per the deliverables definition, one can perform a new transaction as well as get the results of a previously executed transaction.

### To add a new transaction:

Make a POST request to `https://d39bxfiiqg.execute-api.eu-west-2.amazonaws.com/Prod/transaction`
``` 
curl --request POST \
  --url https://d39bxfiiqg.execute-api.eu-west-2.amazonaws.com/Prod/transaction \
  --header 'Content-Type: application/json' \
  --data '{
	"from":{
		"cardNumber": "5200828282828210",
		"cvv": "123",
		"cardHoldersName": "Some dude",
		"expiryDate": "12/12"
	},
	"to":{
		"cardNumber": "5200828282828210",
		"cvv": "123",
		"cardHoldersName": "Some dude",
		"expiryDate": "12/12"
	}
}'
```

A successful response to that will have a `202 Accepted` status code, the `transactionStatusText` field will read `Success`, and the response will look like following:
```
{
  "transactionId": "2caf07ec-9502-485d-b507-0a061a7dd192",
  "bankTransactionResult": {
    "bankTransactionResult": {
      "transactionId": "c57452bd-df5f-4c7a-b418-451ef758c0bb",
      "fromErrorReasons": [],
      "toErrorReasons": []
    },
    "connectionError": null
  },
  "transactionStatus": 1,
  "transactionStatusText": "Success",
  "transactionDateUtc": "2021-10-31T12:04:03.6168535Z",
  "originalTransaction": {
    "from": {
      "cardNumber": "***************9367",
      "cvv": "***",
      "cardHoldersName": "Some Dude",
      "expiryDate": "12/12"
    },
    "to": {
      "cardNumber": "************8210",
      "cvv": "***",
      "cardHoldersName": "Some Other Dude",
      "expiryDate": "12/12"
    },
    "amount": 1000.01
  }
}
```

A transaction can also possibly *fail* for the following reasons:

* The connection to the Bank Simulator may fail. (error code: )
* The Bank Simulator may flag a card as a fraudulent one (you can simulate that with card number 0000-0000-0000-0000) (error code: FRAUD).
* The card number may be invalid (error code: CARD_DETAILS).
* The CVV may be invalid (error code: CVV)
* The expiry date may be in the past (error code: EXPIRY)
* The card holder's name may be less than four characters long (error code: NAME)

In case of any of the above the response to the POST request will be a `412 Precondition Failed` and the corresponding error fields will be filled in with the appropriate error text (`fromErrorReasons`, `toErrorReasons`, `connectionError`). The `transactionStatusText` will also be filled with an error code (`BankDenied` or `Failed`).

### To get an existing transaction
Make a GET request to `https://d39bxfiiqg.execute-api.eu-west-2.amazonaws.com/Prod/transaction/{transactionId}`

```
curl --request GET \
  --url https://d39bxfiiqg.execute-api.eu-west-2.amazonaws.com/Prod/transaction/2caf07ec-9502-485d-b507-0a061a7dd192
```


That should return a `200 OK` response and the exact same payload as the original POST request did:
```
{
  "transactionId": "2caf07ec-9502-485d-b507-0a061a7dd192",
  "bankTransactionResult": {
    "bankTransactionResult": {
      "transactionId": "c57452bd-df5f-4c7a-b418-451ef758c0bb",
      "fromErrorReasons": [],
      "toErrorReasons": []
    },
    "connectionError": null
  },
  "transactionStatus": 1,
  "transactionStatusText": "Success",
  "transactionDateUtc": "2021-10-31T12:04:03.6168535Z",
  "originalTransaction": {
    "from": {
      "cardNumber": "***************9367",
      "cvv": "***",
      "cardHoldersName": "Some Dude",
      "expiryDate": "12/12"
    },
    "to": {
      "cardNumber": "************8210",
      "cvv": "***",
      "cardHoldersName": "Some Other Dude",
      "expiryDate": "12/12"
    },
    "amount": 1000.01
  }
}
```

Note that even if the original transaction failed, a record of with including the error codes should still be returned.

If the transactionId provided does not exist, you will get a `404 Not Found` response.

## Running the project locally
### Environment Variables
In order to run the project locally we need to set a couple of environment variables in `Cko.PaymentGateway/Properties/launchSettings.json`

```
"BANK_API_URL": "http://localhost:5001",
"USE_LOCAL_STORAGE": "true",
```

`BANK_API_URL` tells the PaymentGateway where the BankSimulator API lives.

`USE_LOCAL_STORAGE` indicates that we want to use the very simplistic local implementation of `IDocumentPersistance`. If that environment variable is not set to true, the API will try to connect to a `DynamoDB` table called `CkoTransactions` using the standard AWS environment variables as below:
```
"AWS_ACCESS_KEY_ID": "xxx",
"AWS_SECRET_ACCESS_KEY": "xxx",
"AWS_REGION": "xxx"
``` 



## Project Structure ##
Each main project (`Cko.BankSimulator` and `Cko.PaymentGateway`) contain references a `.Core` and a `.Infrastructure` projects.

As I tend to prefer to add as little logic as possible to the controllers themselves, most of the logic lives in the `.Core` projects, usually in the `Service` layers. Also any additional functionality such as Providers and Repositories are at the Core projects for this example (with the exception of the DynamoDb persistance layer which is in its own project).

The `.Infrastructure` projects have no dependencies and are used to define `Models` & `Interfaces` as well as the occasional static helper or extension.

There is also a `Common.Core` project and a `Common.Infrastructure`, which I would use only in a monorepo, for the other projects to share functionality and definitions.

The `.Test` suffix is to indicate `xUnit` test projects for each of the parent projects. 








