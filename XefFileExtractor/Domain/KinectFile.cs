using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Kinect.Tools;

namespace XefFileExtractor.Domain {
    public struct Stat {
        public string Property { get; set; }
        public string Value { get; set; }

        public Stat(string p, string v) {
            Property = p;
            Value = v;
        }
    }

    public class FrameAnalysis {
        public int DroppedFrames { get; set; }
        public List<int> DroppedIndices { get; set; }
        public int FrameCount { get; set; }
        public List<int> FrameMap { get; set; }

        public FrameAnalysis() {
            DroppedFrames = 0;
            DroppedIndices = new List<int>();
            FrameCount = 0;
            FrameMap = new List<int>();
        }
    }

    public class KinectFile : System.Waf.Foundation.Model {
        private readonly KStudioEventFile _currentKinectFile;
        private readonly List<KinectStream> _streams;

        public KinectFile(string filePath) {
            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            var client = KStudio.CreateClient();
            _currentKinectFile = client.OpenEventFile(filePath);

            _streams = new List<KinectStream>();
            LoadStreams(_currentKinectFile.EventStreams);
        }

        public List<KinectStream> Streams => _streams;

        private void LoadStreams(IReadOnlyCollection<KStudioEventStream> streams) {
            foreach (var stream in streams) {
                switch (stream.DataTypeName) {
                    case "Nui Title Audio":
                        _streams.Add(new KinectAudio(stream));
                        break;
                    case "Nui Uncompressed Color":
                        _streams.Add(new KinectColor(stream));
                        break;
                    case "Nui Depth":
                        _streams.Add(new KinectDepth(stream));
                        break;
                    case "Nui IR":
                        _streams.Add(new KinectIr(stream));
                        break;
                    case "Nui Body Frame":
                        _streams.Add(new KinectBody(stream));
                        break;
                    default:
                        Debug.WriteLine("Stream type not supported.");
                        break;
                }
            }
        }

        /* Stream Statistics Functions */

        //public List<Stat> GetStatsForStream(int streamIndex) {
        //    if (streamIndex < 0) {
        //        return new List<Stat>();
        //    }

        //    var stream = (KStudioSeekableEventStream) _currentKinectFile.EventStreams[streamIndex];

        //    List<Stat> stats = new List<Stat>();
        //    FrameAnalysis fa;
        //    stats.Add(new Stat("Event Count", stream.EventCount.ToString()));
        //    stats.Add(new Stat("Start Time", stream.StartRelativeTime.ToString()));
        //    stats.Add(new Stat("End Time", stream.EndRelativeTime.ToString()));
        //    fa = 

        //    stats.Add(new Stat("Dropped Frames", fa.DroppedFrames.ToString()));
        //    stats.Add(new Stat("Dropped Indices",
        //        string.Join(", ", fa.DroppedIndices.Select(item => item.ToString()).ToArray())));

        //    return stats;
        //}
    }
}