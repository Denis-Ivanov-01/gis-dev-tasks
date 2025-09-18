using NUnit.Framework;
using GeoJSON.Net.Geometry;
using SpatialAnalysisMiniTool;

namespace TestSpatialAnalysisMiniTool
{
    public class TestPolygonExtensions
    {
        private Polygon CreateSquarePolygon(double minX, double minY, double maxX, double maxY)
        {
            var coordinates = new List<IPosition>
            {
                new Position(minX, minY), // bottom-left
                new Position(maxX, minY), // bottom-right
                new Position(maxX, maxY), // top-right
                new Position(minX, maxY), // top-left
                new Position(minX, minY)  // close polygon
            };

            var lineString = new LineString(coordinates);
            return new Polygon(new List<LineString> { lineString });
        }

        private Polygon CreateTrianglePolygon()
        {
            var coordinates = new List<IPosition>
            {
                new Position(0, 0),
                new Position(2, 0),
                new Position(1, 2),
                new Position(0, 0)  // close polygon
            };

            var lineString = new LineString(coordinates);
            return new Polygon(new List<LineString> { lineString });
        }

        private Polygon CreatePolygonWithHole()
        {
            // Outer ring (square)
            var outerRing = new List<IPosition>
            {
                new Position(0, 0),
                new Position(4, 0),
                new Position(4, 4),
                new Position(0, 4),
                new Position(0, 0)
            };

            // Inner ring (hole)
            var innerRing = new List<IPosition>
            {
                new Position(1, 1),
                new Position(3, 1),
                new Position(3, 3),
                new Position(1, 3),
                new Position(1, 1)
            };

            return new Polygon(new List<LineString>
            {
                new LineString(outerRing),
                new LineString(innerRing)
            });
        }

        [Test]
        public void TestContains_PointInsideSquare()
        {
            // Arrange
            var polygon = CreateSquarePolygon(0, 0, 4, 4);
            var point = new Point(new Position(2, 2));

            // Act & Assert
            Assert.IsTrue(polygon.Contains(point));
        }

        [Test]
        public void TestContains_PointOutsideSquare()
        {
            // Arrange
            var polygon = CreateSquarePolygon(0, 0, 4, 4);
            var point = new Point(new Position(5, 5));

            // Act & Assert
            Assert.IsFalse(polygon.Contains(point));
        }

        [Test]
        public void TestContains_PointOnBoundary()
        {
            // Arrange
            var polygon = CreateSquarePolygon(0, 0, 4, 4);
            var point = new Point(new Position(2, 0)); // On bottom edge

            // Act & Assert
            Assert.IsTrue(polygon.Contains(point));
        }

        [Test]
        public void TestContains_PointOnVertex()
        {
            // Arrange
            var polygon = CreateSquarePolygon(0, 0, 4, 4);
            var point = new Point(new Position(0, 0)); // Bottom-left vertex

            // Act & Assert
            Assert.IsTrue(polygon.Contains(point));
        }

        [Test]
        public void TestContains_PointOutsideButAligned()
        {
            // Arrange
            var polygon = CreateSquarePolygon(0, 0, 4, 4);
            var point = new Point(new Position(2, -1)); // Below bottom edge

            // Act & Assert
            Assert.IsFalse(polygon.Contains(point));
        }

        [Test]
        public void TestContains_PointInTriangle()
        {
            // Arrange
            var polygon = CreateTrianglePolygon();
            var point = new Point(new Position(1.5, 0.1)); // Inside triangle

            // Act & Assert
            Assert.IsTrue(polygon.Contains(point));
        }

        [Test]
        public void TestContains_PointOutsideTriangle()
        {
            // Arrange
            var polygon = CreateTrianglePolygon();
            var point = new Point(new Position(1, 3)); // Outside triangle

            // Act & Assert
            Assert.IsFalse(polygon.Contains(point));
        }

        [Test]
        public void TestContains_PointOnEdgeOfTriangle()
        {
            // Arrange
            var polygon = CreateTrianglePolygon();
            var point = new Point(new Position(1, 0)); // On base edge

            // Act & Assert
            Assert.IsTrue(polygon.Contains(point));
        }

        [Test]
        public void TestContains_PointInPolygonWithHole_OutsideHole()
        {
            // Arrange - Point is in outer ring but outside inner ring (hole)
            var polygon = CreatePolygonWithHole();
            var point = new Point(new Position(0.5, 0.5));

            // Act & Assert
            Assert.IsTrue(polygon.Contains(point));
        }

        [Test]
        public void TestContains_PointInPolygonWithHole_InsideHole()
        {
            // Arrange - Point is inside the hole (should be considered outside)
            var polygon = CreatePolygonWithHole();
            var point = new Point(new Position(2, 2));

            // Act & Assert
            Assert.IsFalse(polygon.Contains(point));
        }

        [Test]
        public void TestContains_PointOnHoleBoundary()
        {
            // Arrange - Point is on the boundary of the hole
            var polygon = CreatePolygonWithHole();
            var point = new Point(new Position(1, 1)); // Hole vertex

            // Act & Assert
            Assert.IsTrue(polygon.Contains(point)); // Boundary points are inside
        }

        [Test]
        public void TestContains_PointExactlyOnHorizontalEdge()
        {
            // Arrange
            var polygon = CreateSquarePolygon(0, 0, 4, 4);
            var point = new Point(new Position(2, 2)); // Not on edge, but let's test horizontal edge
            var edgePoint = new Point(new Position(2, 0)); // On horizontal edge

            // Act & Assert
            Assert.IsTrue(polygon.Contains(edgePoint));
        }

        [Test]
        public void TestContains_PointExactlyOnVerticalEdge()
        {
            // Arrange
            var polygon = CreateSquarePolygon(0, 0, 4, 4);
            var point = new Point(new Position(0, 2)); // On vertical edge

            // Act & Assert
            Assert.IsTrue(polygon.Contains(point));
        }

        [Test]
        public void TestContains_PointVeryCloseToBoundary()
        {
            // Arrange
            var polygon = CreateSquarePolygon(0, 0, 4, 4);
            var pointInside = new Point(new Position(0.000001, 0.000001));
            var pointOutside = new Point(new Position(-0.000001, -0.000001));

            // Act & Assert
            Assert.IsTrue(polygon.Contains(pointInside));
            Assert.IsFalse(polygon.Contains(pointOutside));
        }

        [Test]
        public void TestContains_PointAtInfinity()
        {
            // Arrange
            var polygon = CreateSquarePolygon(0, 0, 4, 4);
            var point = new Point(new Position(double.MaxValue, double.MaxValue));

            // Act & Assert
            Assert.IsFalse(polygon.Contains(point));
        }

        [Test]
        public void TestContains_ComplexPolygon_StarShape()
        {
            // Arrange - Create a star-shaped polygon
            var coordinates = new List<IPosition>
            {
                new Position(2, 0),
                new Position(2.5, 1.5),
                new Position(4, 2),
                new Position(2.5, 2.5),
                new Position(2, 4),
                new Position(1.5, 2.5),
                new Position(0, 2),
                new Position(1.5, 1.5),
                new Position(2, 0)
            };

            var polygon = new Polygon(new List<LineString> { new LineString(coordinates) });
            //todo: test why this fails
            // Points inside the star
            var pointInside1 = new Point(new Position(2.2, 1));
            var pointInside2 = new Point(new Position(3, 2));

            // Points outside the star (in the gaps)
            var pointOutside1 = new Point(new Position(1, 1));
            var pointOutside2 = new Point(new Position(3, 3));

            // Act & Assert
            Assert.IsTrue(polygon.Contains(pointInside1));
            Assert.IsTrue(polygon.Contains(pointInside2));
            Assert.IsFalse(polygon.Contains(pointOutside1));
            Assert.IsFalse(polygon.Contains(pointOutside2));
        }

        [Test]
        public void TestContains_PointOnConcavePart()
        {
            // Arrange - Create a concave polygon (C-shaped)
            var coordinates = new List<IPosition>
            {
                new Position(0, 0),
                new Position(4, 0),
                new Position(4, 4),
                new Position(3, 4),
                new Position(3, 1),
                new Position(1, 1),
                new Position(1, 4),
                new Position(0, 4),
                new Position(0, 0)
            };

            var polygon = new Polygon(new List<LineString> { new LineString(coordinates) });

            // Point in the concave part (should be outside)
            var pointInConcavity = new Point(new Position(2, 2));

            // Point in the solid part (should be inside)
            var pointInSolid = new Point(new Position(0.5, 0.5));

            // Act & Assert
            Assert.IsFalse(polygon.Contains(pointInConcavity));
            Assert.IsTrue(polygon.Contains(pointInSolid));
        }

        [Test]
        public void TestContains_MultipleRings()
        {
            // Arrange - Polygon with multiple separate rings (like islands)
            var ring1 = new LineString(new List<IPosition>
            {
                new Position(0, 0), new Position(2, 0), new Position(2, 2), new Position(0, 2), new Position(0, 0)
            });

            var ring2 = new LineString(new List<IPosition>
            {
                new Position(4, 4), new Position(6, 4), new Position(6, 6), new Position(4, 6), new Position(4, 4)
            });

            var polygon = new Polygon(new List<LineString> { ring1, ring2 });

            // Points in each ring
            var pointInRing1 = new Point(new Position(1, 1));
            var pointInRing2 = new Point(new Position(5, 5));
            var pointOutsideBoth = new Point(new Position(3, 3));

            // Act & Assert
            Assert.IsTrue(polygon.Contains(pointInRing1));
            Assert.IsTrue(polygon.Contains(pointInRing2));
            Assert.IsFalse(polygon.Contains(pointOutsideBoth));
        }

        [Test]
        public void TestContains_PerformanceTest()
        {
            // Arrange - Large polygon and multiple points
            var polygon = CreateSquarePolygon(0, 0, 1000, 1000);
            var points = new List<Point>
            {
                new Point(new Position(500, 500)),
                new Point(new Position(1500, 1500)),
                new Point(new Position(250, 250))
            };

            // Act & Assert - Test performance with multiple calls
            foreach (var point in points)
            {
                var result = polygon.Contains(point);
                // Just verify it doesn't throw and returns consistent results
                Assert.IsTrue(result == (point.Coordinates.Latitude <= 1000 && point.Coordinates.Longitude <= 1000));
            }
        }
    }
}