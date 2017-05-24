using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using System.Waf.Foundation;
using XefFileExtractor.Domain;

namespace XefFileExtractor.Applications.Services {
    [Export, Export(typeof(ISelectionService))]
    internal class SelectionService : ISelectionService {
        private SynchronizingCollection<KinectStream, KinectStream> _streams;

        [ImportingConstructor]
        public SelectionService() { }

        public IReadOnlyObservableList<KinectStream> Streams => _streams;

        public IList<KinectStream> SelectedStreams {
            get { return Streams.Where(x => x.IsSelected).ToList(); }
        }

        public void Initialize(IEnumerable<KinectStream> streams) {
            _streams = new SynchronizingCollection<KinectStream, KinectStream>(streams, x => x);
        }
    }
}