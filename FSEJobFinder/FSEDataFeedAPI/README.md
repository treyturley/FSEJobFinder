# FSEDataFeedAPI
This is an ASP.NET Web API that hooks into the FSEconomy Data Feeds to retrieve All-In assignments from the FSEconomy game world.

## Supported Operations

### Get MakeModels
Endpoint: GET /api/FSEJobFinder/v2/makemodels

Response: JSON body with available makemodels (airplane type) that job searches can be performed for


### Get Assignments for Aircraft MakeModel
Endpoint: GET /api/FSEJobFinder/v1/assignments/{aircraft}

Response: JSON body with a list of assignments for the give aircraft


### Get Best Assignment for Aircraft MakeModel
Endpoint: GET /api/FSEJobFinder/v1/bestAssignment/{aircraft}

Response: JSON body describing the best assignment for the give aircraft


### Get Assignments for Aircraft MakeModel that start or end in the US
Endpoint: GET /api/FSEJobFinder/v1/assignmentsFromOrToUS/{aircraft}

Response: JSON body with a list of assignments for the give aircraft that start or end in the US

## Swagger
![FSEDataFeedAPI_Swagger](https://user-images.githubusercontent.com/81778542/214963887-7e561be1-c55c-4bf3-b4bb-1556fc580108.PNG)
