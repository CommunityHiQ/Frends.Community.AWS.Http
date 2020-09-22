
# Frends.Community.AWS.Http

This repository is for frends Community HTTP tasks that have AWS functionality.

[![Actions Status](https://github.com/CommunityHiQ/Frends.Community.AWS.Http/workflows/PackAndPushAfterMerge/badge.svg)](https://github.com/CommunityHiQ/Frends.Community.AWS.Http/actions) ![MyGet](https://img.shields.io/myget/frends-community/v/Frends.Community.AWS.Http) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) 

- [Installing](#installing)
- [Tasks](#tasks)
     - [HttpRequestWithAWSSigV4](#HttpRequestWithAWSSigV4)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing

You can install the task via frends UI Task View or you can find the NuGet package from the following NuGet feed
https://www.myget.org/F/frends-community/api/v3/index.json and in Gallery view in MyGet https://www.myget.org/feed/frends-community/package/nuget/Frends.Community.AWS.Http

# Tasks

## HttpRequestWithAWSSigV4

Generic http request task that signs the request using AWS Signature Version 4. Uses 3rd party library [AwsSignatureVersion4](https://github.com/FantasticFiasco/aws-signature-version-4) to perform the signing. 

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Message | string | The message to be sent with the request. Not used for Get requests | `{"Name" : "Adam", "Age":42}` |
| Method| Enum(GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS, CONNECT) | Http method of request. | `POST` |
| Url| string | The URL with protocol and path to call | `https://abcd1234.execute-api.us-east-1.amazonaws.com/stagename/` |
| Headers| Array{Name: string, Value: string} | List of HTTP headers to be added to the request. | `Name = Content-Type, Value = application/json` |

### Options

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| AccessKey | string | AWS_IAM access key |
| SecretKey | string | AWS_IAM secret key |
| Region | Enum| AWS region |
| ServiceName | string | Name of the AWS service. Default value is 'execute-api' | `execute-api` |
| Connection Timeout Seconds | int | Timeout in seconds to be used for the connection and operation. Default is 30 seconds. |
| Follow Redirects | bool | If FollowRedirects is set to false, all responses with an HTTP status code from 300 to 399 is returned to the application. Default is true.|
| Allow Invalid Certificate | bool | Do not throw an exception on certificate error. Setting this to true is discouraged in production. |
| Throw Exception On ErrorResponse | bool | Throw a WebException if return code of request is not successful. |
| Allow Invalid Response Content Type Char Set | bool | Some Api's return faulty content-type charset header. If set to true this overrides the returned charset. |

### Returns

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Body | string | Response body as string |
| Headers | Dictionary<string,string> | Response headers |
| StatusCode | int | Response status code |


# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.AWS.Http.git`

Rebuild the project

`dotnet build`

Run Tests

`dotnet test`

Create a NuGet package

`dotnet pack --configuration Release`

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version | Changes |
| ------- | ------- |
| 0.0.1   | Development still going on. |
