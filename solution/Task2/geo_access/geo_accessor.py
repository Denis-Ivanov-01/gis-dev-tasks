from typing import List, Tuple, Union


class GeoDataAccessor:
    """
    Intended for abstracting away arcpy and enabling mocking for unit testing
    """

    def intersect(self, layers: List[str], out_fc: str) -> str:
        """Perform an intersect and return path to output feature class."""
        pass

    def search_cursor(self, layer: str, fields: Union[List[str], str], where: str = None) -> List[Tuple]:
        """Return rows from a layer matching optional where clause."""
        pass

    def delete(self, fc: str):
        """Delete a feature class or in-memory object."""
        pass
