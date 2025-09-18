from geo_accessor import GeoDataAccessor


class FakeAccessor(GeoDataAccessor):
    def intersect(self, layers, out_fc):
        return "fake_intersect"

    def search_cursor(self, layer, fields, where=None):
        # Return predefined rows for testing
        if "Room" in layer:
            return [(1, 2, "guid1"), (3, 4, "guid2")]
        return []

    def delete(self, fc):
        pass
