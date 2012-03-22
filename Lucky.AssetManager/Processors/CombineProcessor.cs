using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.AssetReaders;

namespace Lucky.AssetManager.Processors {
    public class CombineProcessor : IProcessor {
        public IEnumerable<IAsset> Process(IEnumerable<IAsset> assets) {
            var results = assets.Where(a => !a.IsProcessable).ToList();

            IEnumerable<IGrouping<IAssetKey, IAsset>> assetGroups = assets.Where(a => a.IsProcessable).GroupBy(asset => asset.Key);

            foreach (IGrouping<IAssetKey, IAsset> assetGroup in assetGroups) {
                if (assetGroup.Count() == 1) {
                    results.Add(assetGroup.Single());
                } else {

                    var combinedTextBuilder = new StringBuilder();
                    var associatedFilePaths = new List<string>();
                    foreach (IAsset asset in assetGroup.OrderByDescending(a => a.OnLayoutPage)) {
                        associatedFilePaths.AddRange(asset.Reader.AssociatedFilePaths);
                        combinedTextBuilder.AppendLine(asset.Reader.Content);
                    }

                    var newContent = combinedTextBuilder.ToString();

                    var newAsset = assetGroup.First();
                    newAsset.Reader = new MemoryAssetReader(associatedFilePaths, newContent);
                    results.Add(newAsset);
                }
            }

            return results;
        }

    }
}
