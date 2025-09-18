from typing import List, Tuple, Union, Optional


class GeoDataAccessor:
    """
    Complete abstraction for GIS operations. Allows mocking + separation of concerns
    """

    def set_workspace(self, workspace_path: str) -> None:
        """Set the current workspace"""
        pass

    def get_count(self, layer: str) -> int:
        """Get feature count for a layer"""
        pass

    def select_layer_by_attribute(self, layer: str, where_clause: str) -> str:
        """Select features by attribute and return selection layer"""
        pass

    def intersect(self, layers: List[str], out_fc: str) -> str:
        """Perform an intersect operation"""
        pass

    def search_cursor(self, layer: str, fields: Union[List[str], str],
                      where: Optional[str] = None) -> List[Tuple]:
        """Return rows from a layer"""
        pass

    def delete(self, fc: str) -> None:
        """Delete a feature class"""
        pass