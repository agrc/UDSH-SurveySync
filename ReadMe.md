# UDSH Survey Sync

A server object extension (SOE) for synchronizing user contributed buildings with vetted buildings.

## Installation
**Requires**  
 
 - ArcGIS Server >= 10.1
 - Publish `.mxd` in `/maps`
 - Publish `.soe` in `/TODO`

## Tests
**Requires**  
 
 - ArcGIS Server
 - SQL express
 - Create **UDSHHistoricBuildings** database
 	- Enable SDE
 	- Import all tables from `/data/UDSHHistoricBuildings.gdb`
 - Create **UDSHSpatial_New** database
	- Enable SDE 
	- Import all tables from `/data/UdshSpatial_New.gdb`

