#!/bin/bash
dotnet lambda package \
    --output packaged/cko-payment-gateway.zip \
    --project-location src/Cko.PaymentGateway \
    --msbuild-parameters "/p:PublishReadyToRun=true --self-contained false"

dotnet lambda deploy-serverless \
    --package packaged/cko-payment-gateway.zip \
    --region $AWS_DEPLOY_REGION \
    --aws-access-key-id $AWS_DEPLOY_ACCESS_KEY \
    --aws-secret-key $AWS_DEPLOY_ACCESS_SECRET \
    --template src/Cko.PaymentGateway/serverless.template.json \
    --stack-name cko-payment-gateway-stack \
    --s3-bucket cyberdisco-deploy-$AWS_DEPLOY_REGION