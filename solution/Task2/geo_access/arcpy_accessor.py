import arcpy
from geo_access.geo_accessor import GeoDataAccessor


class ArcpyAccessor(GeoDataAccessor):
    def intersect(self, layers, out_fc):
        arcpy.analysis.Intersect(
            in_features=layers,
            out_feature_class=out_fc,
            join_attributes="ALL",
            cluster_tolerance=None,
            output_type="INPUT"
        )
        return out_fc

    def search_cursor(self, layer, fields, where=None):
        with arcpy.da.SearchCursor(layer, fields, where_clause=where) as sc:
            for row in sc:
                yield row

    def delete(self, fc):
        arcpy.management.Delete(fc)

    def set_workspace(self, workspace_path: str) -> None:
        arcpy.env.workspace = workspace_path

    def get_count(self, layer: str) -> int:
        result = arcpy.management.GetCount(layer)
        return int(result[0])

    def select_layer_by_attribute(self, layer: str, where_clause: str) -> str:
        return arcpy.management.SelectLayerByAttribute(layer, where_clause=where_clause)
