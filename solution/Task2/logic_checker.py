import logging
from config import Config
from geo_access.geo_accessor import GeoDataAccessor


class LogicChecker:

    def __init__(self, config: Config, geo_accessor: GeoDataAccessor):
        self.config = config
        self.accessor = geo_accessor

        self.accessor.set_workspace(config.database_path)
        logging.debug(f"LogicChecker initialized with workspace: {config.database_path}")

    def get_layer_count(self, layer: str) -> int:
        count = self.accessor.get_count(layer)
        logging.debug(f"Layer '{layer}' contains {count} features")
        return count

    def _check_point_to_poly_relationship(self, point_layer, poly_layer, intersect_target_fields):
        """
        Checks if the logical relationship between a point and polygon classes is correct
        (Room-RoomDetail) or (Station-StationDetail)
        Args:
            point_layer: the point layer (Room/Station)
            poly_layer: the polygon layer (RoomDetail/StationDetail)
            intersect_target_fields: The field names that will be returned by the intersect. They must be:
                [the FID of the point,
                the FID of the polygon,
                the GUID of the point inside the polygon class]

        Returns:
            A list of the incorrect relationships found in the following format:
            (
            the id of the polygon,
            the incorrect id of the point in the polygon class,
            the actual id of the point inside the polygon (geometrically)
            )
        """
        logging.debug(
            f"Checking point-to-polygon relationship: point={point_layer}, poly={poly_layer}"
        )
        invalid_entries = []

        intersected = "in_memory\\intersected"
        self.accessor.intersect([point_layer, poly_layer], intersected)
        logging.debug(f"Intersect created at {intersected}")

        for row in self.accessor.search_cursor(intersected, intersect_target_fields):
            fid_point, fid_poly, point_guid_logical = row
            point_guid_geometrical = self.get_guid(point_layer, f"OBJECTID = {fid_point}")
            if point_guid_logical != point_guid_geometrical:
                poly_guid = self.get_guid(poly_layer, f"OBJECTID = {fid_poly}")
                entry = (poly_guid, point_guid_logical, point_guid_geometrical)
                invalid_entries.append(entry)
                logging.debug(
                    f"Invalid relationship found: poly_guid={poly_guid}, "
                    f"logical_point_guid={point_guid_logical}, "
                    f"geometrical_point_guid={point_guid_geometrical}"
                )

        self.accessor.delete(intersected)
        logging.debug(f"Deleted temporary intersect {intersected}")

        return invalid_entries

    def get_guid(self, layer: str, where: str) -> str:
        """

        Args:
            layer: the path to a layer
            where: a where clause that must filter only 1 row as a result

        Returns: the GUID of the filtered row

        """
        logging.debug(f"Fetching GUID from layer '{layer}' with where-clause: {where}")

        filtered_layer = self.accessor.select_layer_by_attribute(layer, where)

        if self.get_layer_count(filtered_layer) != 1:
            raise RuntimeError(f"Expected exactly 1 feature for where={where} in {layer}")

        for row in self.accessor.search_cursor(layer, "GLOBALID", where=where):
            guid = row[0]
            logging.debug(f"Resolved GUID={guid} from layer={layer}")
            return guid

    def check_room_to_roomdetail_relationships(self):
        return self._check_point_to_poly_relationship(
            self.config.room_layer_name,
            self.config.room_detail_layer_name,
            ["FID_Room", "FID_RoomDetail", "ROOM_GUID"]
        )

    def check_station_to_stationdetail_relationships(self):
        return self._check_point_to_poly_relationship(
            self.config.station_layer_name,
            self.config.station_detail_layer_name,
            ["FID_Station", "FID_StationDetail", "STATION_GUID"]
        )

    def check_room_to_station_relationships(self):
        """
        The main logical check of the task. For every room that is contained inside a station polygon,
        it checks if the station id is geometrically correct.
        Returns: A list of the incorrect results in the format:
        (
        room guid,
        incorrect station guid,
        correct (geometrically) station guid
        )

        """
        logging.debug(
            f"Checking room-to-station relationships: room_layer={self.config.room_layer_name}, "
            f"station_detail_layer={self.config.station_detail_layer_name}"
        )
        invalid_entries = []

        intersected = "in_memory\\intersected"
        self.accessor.intersect([self.config.room_layer_name, self.config.station_detail_layer_name], intersected)
        logging.debug(f"Intersect created at {intersected}")

        for row in self.accessor.search_cursor(intersected, ["STATION_GUID", "STATION_GUID_1", "FID_Room"]):
            logical_station_id, geometrical_station_id, fid_room = row
            if logical_station_id != geometrical_station_id:
                room_guid = self.get_guid(self.config.room_layer_name, f"OBJECTID = {fid_room}")
                entry = (room_guid, logical_station_id, geometrical_station_id)
                invalid_entries.append(entry)
                logging.debug(
                    f"Invalid room-station relationship: room_guid={room_guid}, "
                    f"logical_station={logical_station_id}, "
                    f"geometrical_station={geometrical_station_id}"
                )

        self.accessor.delete(intersected)
        logging.debug(f"Deleted temporary intersect {intersected}")

        return invalid_entries
