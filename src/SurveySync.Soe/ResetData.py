import arcpy

arcpy.env.workspace = 'UDSHSpatial_New on localhost.sde'
arcpy.TruncateTable_management('UDSHSpatial_New.DBO.Buildings')
arcpy.TruncateTable_management('UDSHSpatial_New.DBO.CONTRIBUTION_PROPERTY_POINT')
arcpy.Append_management('UDSHSpatial_New.DBO.Template_CONTRIBUTION_PROPERTY_POINT', 'UDSHSpatial_New.DBO.CONTRIBUTION_PROPERTY_POINT')
arcpy.Append_management('UDSHSpatial_New.DBO.Template_Buildings','UDSHSpatial_New.DBO.Buildings')