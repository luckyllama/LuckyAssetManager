﻿using System.Collections.Generic;
using System.Globalization;
using Lucky.AssetManager.Assets;

namespace Lucky.AssetManager.Processors {
    public interface IProcessor {
        IEnumerable<IAsset> Process(IEnumerable<IAsset> assets);
        CultureInfo CultureInfo { get; set; }
    }
}
