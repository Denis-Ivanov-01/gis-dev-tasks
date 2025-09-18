import unittest
from unittest.mock import Mock, patch
from logic_checker import LogicChecker
from config import Config
from geo_access.geo_accessor import GeoDataAccessor
import logging

##################################
# GENERATED WITH THE HELP OF AI! #
##################################


class TestLogicChecker(unittest.TestCase):

    def setUp(self):
        # Set up basic configuration
        self.config = Mock(spec=Config)
        self.config.database_path = "test_database.gdb"
        self.config.room_layer_name = "Rooms"
        self.config.room_detail_layer_name = "RoomDetails"
        self.config.station_layer_name = "Stations"
        self.config.station_detail_layer_name = "StationDetails"

        # Set up mock geo accessor
        self.mock_accessor = Mock(spec=GeoDataAccessor)

        # Create LogicChecker instance
        self.logic_checker = LogicChecker(self.config, self.mock_accessor)

        # Capture log messages
        logging.basicConfig(level=logging.DEBUG)
        self.log_capture = []
        logging.debug = lambda msg: self.log_capture.append(msg)

    def tearDown(self):
        self.log_capture.clear()

    @patch('arcpy.management.GetCount')
    def test_get_layer_count(self, mock_get_count):
        # Mock the GetCount result
        mock_result = Mock()
        mock_result.__getitem__ = lambda self, index: 42 if index == 0 else None
        mock_get_count.return_value = mock_result

        count = self.logic_checker.get_layer_count("test_layer")

        self.assertEqual(count, 42)
        mock_get_count.assert_called_once_with("test_layer")
        self.assertIn("Layer 'test_layer' contains 42 features", self.log_capture)

    @patch('arcpy.management.SelectLayerByAttribute')
    @patch.object(LogicChecker, 'get_layer_count')
    def test_get_guid_success(self, mock_get_count, mock_select):
        # Mock layer count to return 1
        mock_get_count.return_value = 1

        # Mock search cursor to return a GUID
        self.mock_accessor.search_cursor.return_value = [("test-guid-123",)]

        guid = self.logic_checker.get_guid("test_layer", "OBJECTID = 1")

        self.assertEqual(guid, "test-guid-123")
        mock_select.assert_called_once_with("test_layer", where_clause="OBJECTID = 1")
        mock_get_count.assert_called_once()
        self.mock_accessor.search_cursor.assert_called_once_with(
            "test_layer", "GLOBALID", where="OBJECTID = 1"
        )

    @patch('arcpy.management.SelectLayerByAttribute')
    @patch.object(LogicChecker, 'get_layer_count')
    def test_get_guid_multiple_features_error(self, mock_get_count, mock_select):
        # Mock layer count to return more than 1 feature
        mock_get_count.return_value = 2

        with self.assertRaises(RuntimeError) as context:
            self.logic_checker.get_guid("test_layer", "OBJECTID = 1")

        self.assertIn("Expected exactly 1 feature", str(context.exception))

    def test_check_point_to_poly_relationship_valid(self):
        # Mock intersect to create temporary feature class
        self.mock_accessor.intersect.return_value = "in_memory\\intersected"

        # Mock search cursor to return valid data (GUIDs match)
        self.mock_accessor.search_cursor.return_value = [
            (1, 1, "point-guid-123")  # FID_Point, FID_Poly, logical_GUID
        ]

        # Mock get_guid to return matching GUIDs
        with patch.object(self.logic_checker, 'get_guid') as mock_get_guid:
            mock_get_guid.return_value = "point-guid-123"

            result = self.logic_checker._check_point_to_poly_relationship(
                "point_layer", "poly_layer", ["FID_Point", "FID_Poly", "POINT_GUID"]
            )

        # Should return empty list since GUIDs match
        self.assertEqual(result, [])
        self.mock_accessor.intersect.assert_called_once_with(
            ["point_layer", "poly_layer"], "in_memory\\intersected"
        )
        self.mock_accessor.search_cursor.assert_called_once_with(
            "in_memory\\intersected", ["FID_Point", "FID_Poly", "POINT_GUID"]
        )
        self.mock_accessor.delete.assert_called_once_with("in_memory\\intersected")

    def test_check_point_to_poly_relationship_invalid(self):
        # Mock intersect
        self.mock_accessor.intersect.return_value = "in_memory\\intersected"

        # Mock search cursor to return data with mismatched GUIDs
        self.mock_accessor.search_cursor.return_value = [
            (1, 1, "logical-guid-123")  # logical GUID doesn't match geometrical
        ]

        # Mock get_guid to return different GUIDs
        with patch.object(self.logic_checker, 'get_guid') as mock_get_guid:
            mock_get_guid.side_effect = [
                "geometrical-guid-456",  # For point layer
                "poly-guid-789"  # For poly layer
            ]

            result = self.logic_checker._check_point_to_poly_relationship(
                "point_layer", "poly_layer", ["FID_Point", "FID_Poly", "POINT_GUID"]
            )

        # Should return one invalid entry
        expected_entry = ("poly-guid-789", "logical-guid-123", "geometrical-guid-456")
        self.assertEqual(result, [expected_entry])

    def test_check_room_to_roomdetail_relationships(self):
        with patch.object(self.logic_checker, '_check_point_to_poly_relationship') as mock_check:
            mock_check.return_value = [("room-guid", "logical", "geometrical")]

            result = self.logic_checker.check_room_to_roomdetail_relationships()

            mock_check.assert_called_once_with(
                self.config.room_layer_name,
                self.config.room_detail_layer_name,
                ["FID_Room", "FID_RoomDetail", "ROOM_GUID"]
            )
            self.assertEqual(result, [("room-guid", "logical", "geometrical")])

    def test_check_station_to_stationdetail_relationships(self):
        with patch.object(self.logic_checker, '_check_point_to_poly_relationship') as mock_check:
            mock_check.return_value = [("station-guid", "logical", "geometrical")]

            result = self.logic_checker.check_station_to_stationdetail_relationships()

            mock_check.assert_called_once_with(
                self.config.station_layer_name,
                self.config.station_detail_layer_name,
                ["FID_Station", "FID_StationDetail", "STATION_GUID"]
            )
            self.assertEqual(result, [("station-guid", "logical", "geometrical")])

    def test_check_room_to_station_relationships_valid(self):
        # Mock intersect
        self.mock_accessor.intersect.return_value = "in_memory\\intersected"

        # Mock search cursor with matching GUIDs
        self.mock_accessor.search_cursor.return_value = [
            ("station-guid-123", "station-guid-123", 1)  # Matching GUIDs
        ]

        # Mock get_guid for room
        with patch.object(self.logic_checker, 'get_guid') as mock_get_guid:
            mock_get_guid.return_value = "room-guid-456"

            result = self.logic_checker.check_room_to_station_relationships()

        # Should return empty list since GUIDs match
        self.assertEqual(result, [])
        self.mock_accessor.intersect.assert_called_once_with(
            [self.config.room_layer_name, self.config.station_detail_layer_name],
            "in_memory\\intersected"
        )

    def test_check_room_to_station_relationships_invalid(self):
        # Mock intersect
        self.mock_accessor.intersect.return_value = "in_memory\\intersected"

        # Mock search cursor with mismatched GUIDs
        self.mock_accessor.search_cursor.return_value = [
            ("logical-station-guid", "geometrical-station-guid", 1)
        ]

        # Mock get_guid for room
        with patch.object(self.logic_checker, 'get_guid') as mock_get_guid:
            mock_get_guid.return_value = "room-guid-456"

            result = self.logic_checker.check_room_to_station_relationships()

        # Should return one invalid entry
        expected_entry = ("room-guid-456", "logical-station-guid", "geometrical-station-guid")
        self.assertEqual(result, [expected_entry])

    def test_check_room_to_station_relationships_multiple_rows(self):
        # Mock intersect
        self.mock_accessor.intersect.return_value = "in_memory\\intersected"

        # Mock search cursor with multiple rows, some valid, some invalid
        self.mock_accessor.search_cursor.return_value = [
            ("match-1", "match-1", 1),  # Valid
            ("logical-2", "geometrical-2", 2),  # Invalid
            ("match-3", "match-3", 3),  # Valid
            ("logical-4", "geometrical-4", 4),  # Invalid
        ]

        # Mock get_guid for rooms
        with patch.object(self.logic_checker, 'get_guid') as mock_get_guid:
            mock_get_guid.side_effect = ["room-2", "room-4"]

            result = self.logic_checker.check_room_to_station_relationships()

        # Should return two invalid entries
        expected_entries = [
            ("room-2", "logical-2", "geometrical-2"),
            ("room-4", "logical-4", "geometrical-4")
        ]
        self.assertEqual(result, expected_entries)
        self.assertEqual(len(result), 2)


if __name__ == '__main__':
    unittest.main()
