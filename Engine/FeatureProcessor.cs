﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using HandInput.Util;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

using Common.Logging;

namespace HandInput.Engine {
  public class FeatureProcessor : IFeatureProcessor {
    static readonly int FeatureImageWidth = 64;
    static readonly ILog Log = LogManager.GetCurrentClassLogger();

    public int MotionFeatureLength {
      get {
        return 3;
      }
    }

    public int DescriptorLength {
      get {
        return FeatureImageWidth * FeatureImageWidth * 2;
      }
    }

    Image<Gray, Single> floatImage = new Image<Gray, Single>(FeatureImageWidth, FeatureImageWidth);

    /// <summary>
    /// Creates a new feature array.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public Option<Array> Compute(TrackingResult result) {
      if (result.RelPos.IsSome) {
        var feature = new float[MotionFeatureLength + DescriptorLength];
        var relPos = result.RelPos.Value;
        feature[0] = (float)relPos.X;
        feature[1] = (float)relPos.Y;
        feature[2] = (float)relPos.Z;
        AddImageFeature(result.ColorImage, result.ColorBoundingBoxes.Last(), feature, 
                        MotionFeatureLength);
        AddImageFeature(result.DepthImage, result.DepthBoundingBoxes.Last(), feature,
                        MotionFeatureLength + FeatureImageWidth * FeatureImageWidth);
        return new Some<Array>(feature);
      }
      return new None<Array>();
    }

    void AddImageFeature(Image<Gray, Byte> image, Rectangle bb, float[] dst, int dstIndex) {
      image.ROI = bb;
      floatImage.ConvertFrom(image);
      image.ROI = Rectangle.Empty;
      System.Buffer.BlockCopy(floatImage.Bytes, 0, dst, dstIndex * 4, floatImage.Bytes.Length);
    }
  }
}
