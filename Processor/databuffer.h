#pragma once
#include "pcheader.h"
#include "stbuffer.h"

namespace handinput {
  class PROCESSOR_API DataBuffer {
  public:
    DataBuffer(int size);
    ~DataBuffer() {};

    cv::Mat GetFrame(int istamp);
    cv::Mat GetSingleFrame(int i);
    int LastTimestamp() { return frame_indices_.Last(); }
    // Returns true is the buffer is fully occupied.
    bool IsFull() { return LastTimestamp() >= buffer_size_; }

    void Update(const cv::Mat& image);
    void TemporalConvolve(cv::Mat* dst, std::vector<float> mask);
  private:
    cv::Mat buffer_;
    int buffer_size_;
    int width_, height_;
    CircularIndex frame_indices_;

    DataBuffer(const DataBuffer&);
    DataBuffer& operator=(const DataBuffer&);
  };
}