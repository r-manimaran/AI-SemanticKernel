using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp;

public class CloudService
{
    [VectorStoreKey]
    public int Key { get; set; }
    [VectorStoreData]
    public string Name { get; set; }
    [VectorStoreData]
    public string Description { get; set; }

    [VectorStoreVector(Dimensions:384, DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Vector { get; set; }
}
