#!/bin/bash
dotnet lambda package \
    --output packaged/cko-bank-simulator.zip \
    --project-location src/Cko.BankSimulator \
    --msbuild-parameters "/p:PublishReadyToRun=true --self-contained false"

dotnet lambda deploy-serverless \
    --package packaged/cko-bank-simulator.zip \
    --region $AWS_DEPLOY_REGION \
    --aws-access-key-id $AWS_DEPLOY_ACCESS_KEY \
    --aws-secret-key $AWS_DEPLOY_ACCESS_SECRET \
    --template src/Cko.BankSimulator/serverless.template.json \
    --stack-name cko-bank-simulator-stack \
    --s3-bucket cyberdisco-deploy-$AWS_DEPLOY_REGION