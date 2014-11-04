# UDSH Survey Sync

A server object extension (SOE) for synchronizing user contributed buildings with vetted buildings.

## Usage

_Load the [postman collection](http://www.getpostman.com/) from `\SurveySync\SurveySync.json.postman_collection` for preconfigured requests._

#### Request
**POST** an integer value `surveyId` to `.../arcgis/rest/services/UDSH/SurveySync/MapServer/exts/SurveySyncSoe/Create` and `f=json` to recieve the response as json.

#### Response

Nothing to update or create.
```json
{
 "status": 200,
 "message": "No actions taken."
}
```

Survey id had no property records
```json
{
 "status": 400,
 "message": "No properties found for survey x"
}
```

Bad Input
```json
{
 "status": 400,
 "message": "Must contain 'surveyId' to find properties"
}
```

Success
```json
{
    "result": {
        "successful": true,
        "updated": 5,
        "deleted": 10,
        "created": 5
    },
    "status": 200
}
```

Error
```json
{
    "result": {
        "successful": false,
        "updated": 0,
        "deleted": 0,
        "created": 0
    },
    "status": 500,
    "message": "Error performing apply edits operation"
}
```



## Installation
**Requires**  
 
 - ArcGIS Server >= 10.1
 - Publish `SurveySync.Soe.soe` in `\SurveySync\src\SurveySync.Soe\bin\Release` to ArcGIS Site Extensions.
 - Publish the `.mxd` in `\SurveySync\maps`
    - **Development** `soe.localhost.mxd`
    - **Staging** `soe.itdb110.mxd`
    - **Production** `soe.itdb104.mxd`
        - Adjust `Capabilities`
            - Mapping
                - Operations allowed: Uncheck **all**
            - Feature Access
                - Operations allowed: `Create`, `Delete`, `Update`
                - Allow geometry updates
            - UDSH Survey Sync
                - `Buildings.LayerName` - the layer name of the **FeatureService** (_../arcgis/rest/services/UDSH/SurveySync/FeatureServer_)
                - `ContributionPropertyPoint.LayerName` - the layer name of the **FeatureService** (_.../arcgis/rest/services/UDSH/SurveySync/FeatureServer_)
                - `Buildings.PropertyId` - the property id primary key for the buildings table
                - `ContributionPropertyPoint.PropertyId` - the property id primary key for the contributions table
                - `Survey.PropertyId` - the property id foreign key value for the survey's table
                - `Survey.SurveyId` - the survey id primary key value for the survey's table
                - `Survey.ReturnFields` - a comma separated list of fields to return when quering the survey's table by survey id
                - `FeatureServiceUrl` - the url to the feature service where edits will be sent
                - `ConnectionString` - the database connection string to query the survey table. A few modifications need to happen to allow these properties to be set in the server object extension:
                    - `=` replaced with `--`
                    - `\` replaced with `\\`
                    - `;` replaced with `::`

## Tests
**Requires**  
 
 - ArcGIS Server >= 10.1
 - SQL Express
 - Create a database named **UDSHHistoricBuildings**
 	- Enable SDE via ArcCatalog
 	- Import all tables from `/data/UDSHHistoricBuildings.gdb` using `Feature Class To Feature Class (Conversion)`
 - Create a database named **UDSHSpatial_New**
	- Enable SDE via ArcCatalog
	- Import all tables from `/data/UdshSpatial_New.gdb` using `Table to Table (Conversion)`
 - Publish the map service and soe from the instructions above and configure for local development

## Reseting Data
- Update the `UDSHSpatial_New on localhost.sde` to match your settings
- Execute `\SurveySync\ResetData.py`