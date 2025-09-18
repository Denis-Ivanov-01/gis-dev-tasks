from dataclasses import dataclass
import json
import os


@dataclass
class Config:
    database_path: str
    check_rooms_relationships: bool
    check_stations_relationships: bool
    log_file: str
    log_level: str
    room_layer_name: str
    room_detail_layer_name: str
    station_layer_name: str
    station_detail_layer_name: str

    @classmethod
    def from_file(cls, file_path="config.json"):
        with open(file_path, "r") as f:
            content = json.load(f)
            return cls(**content)
