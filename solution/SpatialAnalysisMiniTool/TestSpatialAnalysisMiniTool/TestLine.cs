using GeoJSON.Net.Geometry;
using NUnit.Framework;
using SpatialAnalysisMiniTool;

namespace TestSpatialAnalysisMiniTool
{
    public class TestLine
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestIntersects_GeneralCase_IntersectingLines()
        {
            var line1 = new Line(new Position(0, 0), new Position(2, 2));
            var line2 = new Line(new Position(0, 2), new Position(2, 0));


            Assert.IsTrue(line1.Intersects(line2));
            Assert.IsTrue(line2.Intersects(line1)); // Should be symmetric
        }

        [Test]
        public void TestIntersects_GeneralCase_NonIntersectingLines()
        {
            var line1 = new Line(new Position(0, 0), new Position(1, 1));
            var line2 = new Line(new Position(2, 0), new Position(3, 1));


            Assert.IsFalse(line1.Intersects(line2));
            Assert.IsFalse(line2.Intersects(line1));
        }

        [Test]
        public void TestIntersects_ParallelLines()
        {
            var line1 = new Line(new Position(0, 0), new Position(2, 0));
            var line2 = new Line(new Position(0, 1), new Position(2, 1));


            Assert.IsFalse(line1.Intersects(line2));
        }

        [Test]
        public void TestIntersects_CollinearButNotOverlapping()
        {
            var line1 = new Line(new Position(0, 0), new Position(1, 0));
            var line2 = new Line(new Position(2, 0), new Position(3, 0));


            Assert.IsFalse(line1.Intersects(line2));
        }

        [Test]
        public void TestIntersects_CollinearAndOverlapping()
        {
            var line1 = new Line(new Position(0, 0), new Position(3, 0));
            var line2 = new Line(new Position(1, 0), new Position(2, 0));


            Assert.IsTrue(line1.Intersects(line2));
        }

        [Test]
        public void TestIntersects_EndpointTouching()
        {
            var line1 = new Line(new Position(0, 0), new Position(1, 1));
            var line2 = new Line(new Position(1, 1), new Position(2, 0));


            Assert.IsTrue(line1.Intersects(line2));
        }

        [Test]
        public void TestIntersects_TShapeIntersection()
        {
            var horizontal = new Line(new Position(0, 1), new Position(2, 1));
            var vertical = new Line(new Position(1, 0), new Position(1, 2));


            Assert.IsTrue(horizontal.Intersects(vertical));
        }

        [Test]
        public void TestIntersects_VerticalAndHorizontal()
        {
            var vertical = new Line(new Position(1, 0), new Position(1, 2));
            var horizontal = new Line(new Position(0, 1), new Position(2, 1));


            Assert.IsTrue(vertical.Intersects(horizontal));
        }

        [Test]
        public void TestIntersects_IdenticalLines()
        {
            var line1 = new Line(new Position(0, 0), new Position(1, 1));
            var line2 = new Line(new Position(0, 0), new Position(1, 1));


            Assert.IsTrue(line1.Intersects(line2));
        }

        [Test]
        public void TestIsPointOnLine_PointInMiddle()
        {
            var line = new Line(new Position(0, 0), new Position(2, 2));
            var point = new Point(new Position(1, 1));

            Assert.IsTrue(line.IsPointOnLine(point));
        }

        [Test]
        public void TestIsPointOnLine_PointAtEndpoint()
        {
            var line = new Line(new Position(0, 0), new Position(2, 2));
            var point = new Point(new Position(0, 0));

            Assert.IsTrue(line.IsPointOnLine(point));
        }

        [Test]
        public void TestIsPointOnLine_PointNotOnLine()
        {
            var line = new Line(new Position(0, 0), new Position(2, 2));
            var point = new Point(new Position(1, 2));

            Assert.IsFalse(line.IsPointOnLine(point));
        }

        [Test]
        public void TestIsPointOnLine_PointOutsideBoundingBox()
        {
            var line = new Line(new Position(0, 0), new Position(2, 2));
            var point = new Point(new Position(3, 3));

            Assert.IsFalse(line.IsPointOnLine(point));
        }

        [Test]
        public void TestIsPointOnLine_PointOnExtendedLineButNotSegment()
        {
            var line = new Line(new Position(0, 0), new Position(2, 2));
            var point = new Point(new Position(3, 3));

            Assert.IsFalse(line.IsPointOnLine(point));
        }

        [Test]
        public void TestIsPointOnLine_HorizontalLine()
        {
            var line = new Line(new Position(0, 5), new Position(10, 5));
            var point = new Point(new Position(5, 5));

            Assert.IsTrue(line.IsPointOnLine(point));
        }

        [Test]
        public void TestIsPointOnLine_VerticalLine()
        {
            var line = new Line(new Position(5, 0), new Position(5, 10));
            var point = new Point(new Position(5, 5));

            Assert.IsTrue(line.IsPointOnLine(point));
        }

        [Test]
        public void TestIntersects_EdgeCaseWithVeryCloseLines()
        {
            var line1 = new Line(new Position(0, 0), new Position(1, 0));
            var line2 = new Line(new Position(0, 0.0000001), new Position(1, 0.0000001));

            Assert.IsFalse(line1.Intersects(line2));
        }

        [Test]
        public void TestIntersects_SharedEndpointOnly()
        {
            var line1 = new Line(new Position(0, 0), new Position(1, 0));
            var line2 = new Line(new Position(1, 0), new Position(1, 1));

            Assert.IsTrue(line1.Intersects(line2));
        }
    }
}