# CSharp-API-Demo-Project
Sample implementation for consuming eworx Marketing Suite (eMS) API.

This demo project shows how to consume the eMS SOAP API, how to create and send campaigns, read statistic data and send a trigger mail.

There are several methods available and implemented for importing new subscribers, reading the available subscriber metadata, adding them to a certain subscriber group (profile), reading or updating campaigns etc.
Further descriptions of the eMS api can be found here: https://www.eworx.at/doku/api-schnittstellenbeschreibung/ (Documentation in German).

If you don't have access to the API, you can register your application here: https://www.eworx.at/doku/api-schnittstellenbeschreibung/#zugang-zur-api-anlegen

The name of the application registered is the one used for credentials of the service agent.

## Example

Here is a brief step by step example:

1. Preparation to use the API
2. Choose which example to execute

## 1. Preparation

First, create a ServiceAgent with the url to our webservice and your credentials.

| Login Data    | Instructions                                           |
|---------------|--------------------------------------------------------|
| Account       | Account name (Mandant) of the eMS to login             |
| Username      | User name to use to login                              |
| Password      | The user's password                                    |
| Application   | The name of the registered application                 |

## 2. Examples

Start the [Program.cs](Program.cs) file and choose which example to execute:

| Example                  | Instructions                                | Link                                                 |
|--------------------------|---------------------------------------------|------------------------------------------------------|
| Send a campaign          | Enter 'a'                                   | [README](./Examples/SendCampaign/)          |
| Read campaign statistics | Enter 'b'                                   | [README](./Examples/ReadCampaignStatistic/) |
| Send a trigger mail      | Enter 'c'                                   | [README](./Examples/SendTriggerMail/)       |
| End the program          | Press esc                                   |                                                      |
