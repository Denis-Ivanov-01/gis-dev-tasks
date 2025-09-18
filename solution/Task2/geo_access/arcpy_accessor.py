import arcpy
from geo_accessor import GeoDataAccessor


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