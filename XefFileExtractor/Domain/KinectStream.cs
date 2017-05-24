using System;
using Microsoft.Kinect.Tools;

namespace XefFileExtractor.Domain {
    public class KinectFileProgressChangedEventArgs : EventArgs {
        public int Progress { get; set; }
        public string StatusName { get; set; }
    }

    public abstract class KinectStream : System.Waf.Foundation.Model {
        protected readonly KStudioEventStream _stream;
        public event EventHandler ProgressUpdated;

        protected KinectStream(KStudioEventStream stream) {
            _stream = stream;
        }

        protected virtual void OnProgressUpdated(KinectFileProgressChangedEventArgs e) {
            ProgressUpdated?.Invoke(this, e);
        }

        public string Name => _stream.DataTypeName;

        public bool IsSelected { get; set; }

        public abstract void Extract(string outputPath);

        public abstract FrameAnalysis CountDroppedFrames();
    }
}