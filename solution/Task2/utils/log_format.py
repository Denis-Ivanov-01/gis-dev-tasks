import logging
import sys


class CustomFormatter(logging.Formatter):
    """Custom logging formatter with ANSI colors and UTF-8 support."""

    # ANSI color codes
    grey = "\x1b[38;20m"
    yellow = "\x1b[33;20m"
    red = "\x1b[31;20m"
    bold_red = "\x1b[31;1m"
    reset = "\x1b[0m"

    format_str = "%(asctime)s - %(name)s - %(levelname)s - %(message)s (%(filename)s:%(lineno)d)"

    FORMATS = {
        logging.DEBUG: grey + format_str + reset,
        logging.INFO: grey + format_str + reset,  # Default color
        logging.WARNING: yellow + format_str + reset,  # Yellow warnings
        logging.ERROR: red + format_str + reset,  # Red errors
        logging.CRITICAL: bold_red + format_str + reset,  # Bold red critical errors
    }

    def format(self, record):
        log_fmt = self.FORMATS.get(record.levelno, self.grey + self.format_str + self.reset)
        formatter = logging.Formatter(log_fmt)
        return formatter.format(record)


def configure_logging(log_file, log_level):
    # Create root logger
    logger = logging.getLogger()
    logger.setLevel(log_level)

    # Console handler (UTF-8 encoding)
    console_handler = logging.StreamHandler(sys.stdout)
    console_handler.setLevel(log_level)
    console_handler.setFormatter(CustomFormatter())

    # File handler (UTF-8 encoding)
    file_handler = logging.FileHandler(log_file, encoding="utf-8")
    file_handler.setLevel(log_level)
    file_handler.setFormatter(logging.Formatter(CustomFormatter.format_str))  # No color in file logs

    # Add handlers to logger
    logger.addHandler(console_handler)
    logger.addHandler(file_handler)
