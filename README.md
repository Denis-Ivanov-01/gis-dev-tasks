# How to start

1. Fork this repository into your own [GitHub account](https://github.com).
2. Implement your solution in your forked repo (**solution** dir).
3. Update the **README** in your fork with instructions on how to run your solution.
4. Share the link to your forked repository when submitting your task.

# Expectations

We will assess your submission based on:
- **Code quality** - is the code well-structured, readable and maintainable
- **Documentation** - quality and level of detail of the provided documentation
- **GIS knowledge** - how you approach the spatial operations and data handling
- **Correctness** - does the app work as specified

# Deliverables

- **Source code** (fork this repo and provide a link to your implementation)
- **solution/README.md** - should document your solution and provide info how to run it
> Please do not commit compiled binaries, executables, or large datasets. Only submit source code and supporting files.


# Tasks

## Task 1: Spatial Analysis Mini-Tool

You are asked to build a small .NET application (console, desktop, or service) targeting .NET Runtime 8.0 or higher (Visual Studio 2022 v17.8 or higher) that performs a simple spatial analysis. Any open-source external libraries can be used (no ESRI or other commercial libraries are allowed). 

### Input Data

1. Polygon dataset in GeoJSON format, where each polygon represents a rectangle.
   - Each polygon contains at least one attribute `PolygonId`
   - coordinates are 2D, units - meter
2. Point dataset in CSV format, containing X and Y coordinates.
   - coordinates are 2D, units - meter

### Requirements

Your application should:

1. **Load the input data** - polygon and point datasets.
2. **Determine which points fall inside which polygon** (point-in-polygon check).
3. **Count** the number of points inside each polygon.

#### Expected Output

Produce a CSV file in the format:
```
PolygonId,PointCount
Rect1,10
Rect2,0
Rect3,100
...
```

#### Bonus Points

You may implement one or more of the following:
- handle invalid geometries
- support different coordinate systems (polygons in projected CRS, points in `WGS84:4326`)
- include unit tests
- add support for reading input data in various formats (`GML`, `SHP`, `KML`, etc.)

## Task 2: 

You are asked to write a python script, SQL or build a small .NET application (console, desktop, or service) targeting .NET Runtime 8.0 or higher (Visual Studio 2022 v17.8 or higher), that processes data from multiple tables stored in an Esri Mobile Geodatabase.

The goal is to identify and report all features with invalid logical relationships.
You may use Esri APIs, SDKs, or tools (ArcGIS Pro SDK, ArcPy, etc.), as well as any external libraries.

### Input Data

The source data is provided as an Esri Mobile Geodatabase.

**Data definitions:**

- **Station** - point feature class representing stations
- **Room** - point feature class representing rooms inside stations
- **StationDetail** - polygon feature class representing station borders
- **RoomDetail** - polygon feature class representing room borders
- **Station__StationDetail** - relationship class between a station and its detail
- **Room__RoomDetail** - relationship class between a room and its detail
- **Station__Room** - relationship class between a station and the rooms it contains

### Requirements

Identify invalid logical relationships between stations and rooms.

- **Correct logic**: Every room that is spatially located inside a station must also be logically related to that same station through the **Station__Room** relationship.
- **Invalid case**: A room is inside the detail of **Station A**, but its logical relationship points to **Station B**. Or the room is not related to any station.

You need to produce a report listing all rooms that violate this rule.

> Solutions that only rely on manual geoprocessing in ArcGIS Pro will not be accepted. The task must be automated in code.

#### Expected Output

Produce a CSV file in the format:
```
RoomId,CurrentStationId
Room1,Station2
Room2,Station4
Room3,Station4
...
```

Where:
- `RoomId` = unique identifier of the room.
- `CurrentStationId` = the station currently linked through the logical relationship.

#### Bonus Points

You may implement one or more of the following:
- include unit tests
- Extend the report to also include the correct station based on spatial location, e.g.:
   ```
   RoomId,CurrentStationId,CorrectStationId
   Room1,Station2,Station1
   Room2,Station4,Station3
   ```