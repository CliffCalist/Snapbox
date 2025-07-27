using System.Diagnostics;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK.Tests
{
    public class SnapboxTests
    {
        [Test, Performance]
        public async Task LoadNewSnapshotsAsync_WhenSnapshotsAreLoadedInParallel_ExecutesUnderExpectedTime()
        {
            // Arrange
            const int COUNT = 10;
            const int SIMULATED_DALEY_MS = 100;
            const int EXPECTED_MAX_DURATION = 1000;

            var mockLoader = new Mock<ISnapshotLoader>();
            var mockSaver = new Mock<ISnapshotSaver>();

            mockLoader
                .Setup(l => l.LoadAsync(It.IsAny<ISnapshotMetadata>()))
                .Returns(async () =>
                {
                    await Task.Delay(SIMULATED_DALEY_MS);
                    return new object();
                });

            var snapbox = new Snapbox(mockLoader.Object, mockSaver.Object);

            for (int i = 0; i < COUNT; i++)
            {
                var metadata = new TestSnapshotMetadata($"snapshot_{i}");
                snapbox.AddMetadata(metadata);
            }

            // Act
            var stopwatch = Stopwatch.StartNew();
            await snapbox.LoadNewSnapshotsAsync();
            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;
            Measure.Custom(new SampleGroup("Snapshot Loading Duration (ms)"), elapsedMs);

            // Assert 
            Assert.Less(elapsedMs, EXPECTED_MAX_DURATION,
                "Snapshots should load in parallel, significantly faster than sequential time.");
        }



        private class TestSnapshotMetadata : ISnapshotMetadata
        {
            public string SnapshotName { get; }
            public bool IsChanged { get; set; }
            public bool IsDeleted { get; set; }



            public TestSnapshotMetadata(string name)
            {
                SnapshotName = name;
            }
        }
    }
}
