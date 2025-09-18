import os
import csv


class CsvGenerator:

    def __init__(self, directory: str = None):

        if not directory:
            directory = os.path.dirname(__file__)

        self.directory = os.path.abspath(directory)
        os.makedirs(self.directory, exist_ok=True)

    def generate_csv(self, name_no_extension: str, header_row: list[str], rows: list[tuple]):
        csv_path = self.get_unique_name(name_no_extension)

        with open(csv_path, mode="w", newline="", encoding="utf-8") as file:
            writer = csv.writer(file)
            writer.writerow(header_row)
            writer.writerows(rows)

        return csv_path

    def get_unique_name(self, name_no_extension):
        csv_path = os.path.join(self.directory, f"{name_no_extension}.csv")
        counter = 1
        while os.path.exists(csv_path):
            csv_path = os.path.join(self.directory, f"{name_no_extension}_{counter}.csv")
            counter += 1
        return csv_path
