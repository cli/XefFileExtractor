using System.Collections.Generic;
using System.Waf.Foundation;
using XefFileExtractor.Domain;

namespace XefFileExtractor.Applications.Services {
    public interface ISelectionService {
        IReadOnlyObservableList<KinectStream> Streams { get; }

        IList<KinectStream> SelectedStreams { get; }
    }
}