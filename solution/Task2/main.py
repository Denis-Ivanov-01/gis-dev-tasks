from logic_checker import LogicChecker
from config import Config
from geo_access.arcpy_accessor import ArcpyAccessor
from utils.log_format import configure_logging
from utils.csv_generator import CsvGenerator
import logging
import os


def perform_logical_checks(config):
    checker = LogicChecker(config, ArcpyAccessor())
    csv = CsvGenerator(os.path.dirname(__file__))

    if config.check_rooms_relationships:
        logging.info("Checking room to room detail relations...")
        invalid_entries = checker.check_room_to_roomdetail_relationships()

        logging.info("Saving the result to a CSV file")
        csv_path = csv.generate_csv(
            "invalid_room_relations",
            ["PointDetail_GUID", "Point_GUID_logical", "Point_GUID_geometric"],
            invalid_entries
        )
        logging.info(f"The data with invalid room to room detail relations was saved to {csv_path}")

    if config.check_stations_relationships:
        logging.info("Checking station to station detail relations...")
        invalid_entries = checker.check_station_to_stationdetail_relationships()

        logging.info("Saving the result to a CSV file")
        csv_path = csv.generate_csv(
            "invalid_station_relations",
            ["StationDetail_GUID", "Point_GUID_logical", "Point_GUID_geometric"],
            invalid_entries
        )
        logging.info(f"The data with invalid station to station detail relations was saved to {csv_path}")

    logging.info("Checking the room to station geometric relations...")
    invalid_relations = checker.check_room_to_station_relationships()

    logging.info("Saving the result to a CSV file")
    csv_path = csv.generate_csv(
        "invalid_relations",
        ["RoomId", "CurrentStationId", "CorrectStationId"],
        invalid_relations
    )
    logging.info(f"The data with invalid room to station relations was saved to {csv_path}")


def main():
    config = Config.from_file()
    configure_logging(config.log_file, config.log_level)
    try:
        logging.info("Start processing...")
        perform_logical_checks(config)
        logging.info("Process finished successfully")
    except Exception as ex:
        logging.error(f"An error occurred: {ex}")


if __name__ == "__main__":
    main()
