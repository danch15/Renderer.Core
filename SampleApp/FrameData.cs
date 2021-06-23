using System;
using System.IO;

namespace SampleApp
{
    /// <summary>
    /// 帧类型定义
    /// </summary>
    public enum VideoType
    {
        Unknown = 0,
        AUDIO16 = 1,
        RGB32 = 2,
        YV12 = 3,
        UYVY = 4,
        YUV420 = 5,
        YUY2 = 6,
        AUDIO8 = 7,

        RGB24 = 9,
        GRAY = 10,

        RGB555 = 11,
        RGB565 = 12,
        YUV422 = 13,
        YUV444 = 14
    }

    /// <summary>
    /// 缓冲视频数据
    /// </summary>
    public class FrameData
    {
        /// <summary>
        /// 缓冲取到的视频帧
        /// </summary>
        public byte[] FrameBuffer
        {
            get;
            set;
        }

        /// <summary>
        /// 每帧大小
        /// </summary>
        public int FrameSize
        {
            get;
            set;
        }

        /// <summary>
        /// 总帧数
        /// </summary>
        public int Frames
        {
            get;
            set;
        }

        /// <summary>
        /// 每帧宽度
        /// </summary>
        public int FrameWidth
        {
            get;
            set;
        }

        /// <summary>
        /// 每帧高度
        /// </summary>
        public int FrameHeight
        {
            get;
            set;
        }

        /// <summary>
        /// 帧类型
        /// </summary>
        public VideoType Type
        {
            get;
            set;
        }


        #region Load Save保存接口

        /// <summary>
        /// 保存VideoData数据. 格式为Frame头+FrameData。
        /// 
        /// 其中Frame头固定为64个int。
        /// 格式为：[VideoType][Frames][FrameSize][FrameWidth][FrameHeight][VideoBufferSize][保留字节]
        /// </summary>
        /// <param name="path"></param>
        public static void SaveData(string filePath, FrameData videoData)
        {
            if (videoData != null)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(fs))
                    {
                        writer.Write(videoData.Frames);         // number of frames
                        writer.Write(videoData.FrameWidth);     // frame width
                        writer.Write(videoData.FrameHeight);    // frame height
                        writer.Write((int)videoData.Type);      // video type
                        writer.Write(videoData.FrameSize);      // frame size
                        for (int i = 0; i < 59; i++)
                        {
                            writer.Write((int)0);
                        }

                        if (videoData.FrameBuffer != null)
                        {
                            // 由于video buffer的size是重用的，因此buffer的size可能会大于实际数据的大小
                            // 这里使用实际的数据大小来进行保存
                            int dataSize = videoData.FrameSize * videoData.Frames;
                            writer.Write(videoData.FrameBuffer, 0, dataSize);
                        }
                    }
                }
            }
        }

        public static FrameData LoadData(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        FrameData videoData = new FrameData();

                        videoData.Frames = reader.ReadInt32();
                        videoData.FrameWidth = reader.ReadInt32();
                        videoData.FrameHeight = reader.ReadInt32();
                        videoData.Type = (VideoType)reader.ReadInt32();
                        videoData.FrameSize = reader.ReadInt32();
                        for (int i = 0; i < 59; i++)
                        {
                            reader.ReadInt32();
                        }

                        int bufferLength = videoData.Frames * videoData.FrameSize;
                        if (bufferLength > 0)
                        {
                            videoData.FrameBuffer = reader.ReadBytes(bufferLength);
                        }

                        return videoData;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static FrameData LoadData2(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        FrameData videoData = new FrameData();

                        videoData.FrameWidth = 1280;
                        videoData.FrameHeight = 720;
                        videoData.FrameSize = (int)(1280 * 720 * 1.5);
                        videoData.Frames = (int)(fs.Length / videoData.FrameSize);
                        videoData.Type = VideoType.YV12;
                        //for (int i = 0; i < 59; i++)
                        //{
                        //    reader.ReadInt32();
                        //}

                        int bufferLength = videoData.Frames * videoData.FrameSize;
                        if (bufferLength > 0)
                        {
                            videoData.FrameBuffer = reader.ReadBytes(bufferLength);

                            //byte[] allFrameBuffer = new byte[bufferLength];
                            ////数据组成为YYYYYYYY UU VV
                            //for (int i = 0; i < videoData.Frames; i++)
                            //{
                            //    byte[] frameByte = reader.ReadBytes(videoData.FrameSize);
                            //    //frameByte = decodeYUV420SP(frameByte, videoData.FrameWidth, videoData.FrameHeight);
                            //    //frameByte = YV12ToRGB(frameByte, videoData.FrameWidth, videoData.FrameHeight);

                            //    //yuv420
                            //    //yuv420p（yu12 / I420）
                            //    //[ y y y y ]
                            //    //[ y y y y ]
                            //    //[ y y y y ]
                            //    //[ y y y y ]
                            //    //[ u u ]
                            //    //[ u u ]
                            //    //[ v v ]
                            //    //[ v v ]

                            //    //或（yv12）
                            //    //[ y y y y ]
                            //    //[ y y y y ]
                            //    //[ y y y y ]
                            //    //[ y y y y ]
                            //    //[ v v ]
                            //    //[ v v ]
                            //    //[ u u ]
                            //    //[ u u ]

                            //    //反转Y
                            //    int yWidth = videoData.FrameWidth;
                            //    int yHeight = videoData.FrameHeight;
                            //    int yLength = yWidth * yHeight;
                            //    //for (int y = 0; y < yLength / 2; y++)
                            //    //{
                            //    //    byte tempByte = frameByte[y];
                            //    //    frameByte[y] = frameByte[y + (yLength / 2)];
                            //    //    frameByte[y + (yLength / 2)] = tempByte;
                            //    //}

                            //    //反转UV
                            //    int uvWidth = videoData.FrameWidth / 2;
                            //    int uvHeight = videoData.FrameHeight;
                            //    int uvLength = uvWidth * uvHeight;
                            //    for (int y = 0; y < uvLength / 2; y++)
                            //    {
                            //        byte tempByte = frameByte[yLength + y];
                            //        frameByte[yLength + y] = frameByte[yLength + y + (uvLength / 2)];
                            //        frameByte[yLength + y + (uvLength / 2)] = tempByte;
                            //    }
                            //    frameByte.CopyTo(allFrameBuffer, i * videoData.FrameSize);
                            //}
                            //videoData.FrameBuffer = allFrameBuffer;
                        }

                        return videoData;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        private class RGB
        {
            public int r, g, b;
        }


        //public static byte[] YV12ToRGB(byte[] src, int width, int height)
        //{
        //    int numOfPixel = width * height;
        //    int positionOfV = numOfPixel;
        //    int positionOfU = numOfPixel / 4 + numOfPixel;
        //    byte[] rgb = new byte[numOfPixel];

        //    for (int i = 0, yp = 0; i < height; i++)
        //    {
        //        int startY = i * width;
        //        int step = (i / 2) * (width / 2);
        //        int startV = positionOfV + step;
        //        int startU = positionOfU + step;
        //        for (int j = 0; j < width; j++, yp++)
        //        {
        //            int Y = startY + j;
        //            int V = startV + j / 2;
        //            int U = startU + j / 2;
        //            int index = Y * 3;

        //            //rgb[index+R] = (int)((src[Y]&0xff) + 1.4075 * ((src[V]&0xff)-128));
        //            //rgb[index+G] = (int)((src[Y]&0xff) - 0.3455 * ((src[U]&0xff)-128) - 0.7169*((src[V]&0xff)-128));
        //            //rgb[index+B] = (int)((src[Y]&0xff) + 1.779 * ((src[U]&0xff)-128));

        //            RGB tmp = yuvTorgb(src[Y], src[U], src[V]);
        //            //tmp = YUVtoRGB(src[Y], src[U], src[V]);

        //            //rgb[index + R] = tmp.r;
        //            //rgb[index + G] = tmp.g;
        //            //rgb[index + B] = tmp.b;
        //            var arg = 0xff000000 |
        //                ((tmp.r /*<< 6*/) & 0xff0000) |
        //                ((tmp.g /*>> 2*/) & 0xff00) |
        //                ((tmp.b /*>> 10*/) & 0xff);
        //            rgb[yp] = (byte)arg;
        //        }
        //    }
        //    return rgb;
        //}

        //private void Rgb2Yuv(byte[] img, double y, double u, double v, int height, int width)
        //{
        //    //  Y= 0.3*R + 0.59*G + 0.11*B
        //    //  U= (B-Y) * 0.493
        //    //  V= (R-Y) * 0.877
        //    byte[] rgb = new byte[height * width];
        //    double Y = 0.0, U = 0.0, V = 0.0;
        //    for (int h = 0; h < height; ++h)
        //    {
        //        for (int w = 0; w < width; ++w)
        //        {
        //            //Y = 0.11 * img.Data[h, w, 0] + 0.59 * img.Data[h, w, 1] + 0.3 * img.Data[h, w, 2];
        //            //U = (img.Data[h, w, 0] - Y) * 0.493;
        //            //V = (img.Data[h, w, 2] - Y) * 0.877;
        //            Y = 0.257 * y + 0.504 * u + 0.098 * v + 16;
        //            U = 0.148 * y - 0.291 * u + 0.439 * v + 128;
        //            V = 0.439 * y - 0.368 * u - 0.071 * v + 128;
        //            yImg.Data[h, w, 0] = Y;
        //            uImg.Data[h, w, 0] = U;
        //            vImg.Data[h, w, 0] = V;
        //        }
        //    }
        //}

        //private static RGB YUVtoRGB(double y, double u, double v)
        //{
        //    RGB rgb = new RGB();

        //    rgb.r = Convert.ToInt32((y + 1.139837398373983740 * v) * 255);
        //    rgb.g = Convert.ToInt32((
        //        y - 0.3946517043589703515 * u - 0.5805986066674976801 * v) * 255);
        //    rgb.b = Convert.ToInt32((y + 2.032110091743119266 * u) * 255);

        //    return rgb;
        //}

        //private static RGB yuvTorgb(byte Y, byte U, byte V)
        //{
        //    RGB rgb = new RGB();
        //    rgb.r = (int)((Y & 0xff) + 1.4075 * ((V & 0xff) - 128));
        //    rgb.g = (int)((Y & 0xff) - 0.3455 * ((U & 0xff) - 128) - 0.7169 * ((V & 0xff) - 128));
        //    rgb.b = (int)((Y & 0xff) + 1.779 * ((U & 0xff) - 128));
        //    rgb.r = (rgb.r < 0 ? 0 : rgb.r > 255 ? 255 : rgb.r);
        //    rgb.g = (rgb.g < 0 ? 0 : rgb.g > 255 ? 255 : rgb.g);
        //    rgb.b = (rgb.b < 0 ? 0 : rgb.b > 255 ? 255 : rgb.b);
        //    return rgb;
        //}

        //public static byte[] decodeYUV420SP(byte[] yuv420sp, int width, int height)
        //{
        //    int frameSize = width * height;
        //    byte[] rgb = new byte[frameSize];
        //    for (int j = 0, yp = 0; j < height; j++)
        //    {
        //        int uvp = frameSize + (j >> 1) * width, u = 0, v = 0;
        //        for (int i = 0; i < width; i++, yp++)
        //        {
        //            int y = (0xff & ((int)yuv420sp[yp])) - 16;
        //            if (y < 0)
        //                y = 0;
        //            if ((i & 1) == 0)
        //            {
        //                v = (0xff & yuv420sp[uvp++]) - 128;
        //                u = (0xff & yuv420sp[uvp++]) - 128;
        //            }
        //            int y1192 = 1192 * y;
        //            int r = (y1192 + 1634 * v);
        //            int g = (y1192 - 833 * v - 400 * u);
        //            int b = (y1192 + 2066 * u);
        //            if (r < 0)
        //                r = 0;
        //            else if (r > 262143)
        //                r = 262143;
        //            if (g < 0)
        //                g = 0;
        //            else if (g > 262143)
        //                g = 262143;
        //            if (b < 0)
        //                b = 0;
        //            else if (b > 262143)
        //                b = 262143;
        //            //var arg = 0xff000000 | ((r << 6) & 0xff0000) | ((g >> 2) & 0xff00) | ((b >> 10) & 0xff);
        //            var arg = 0xff000000 | ((r << 6) & 0xff0000) | ((g >> 2) & 0xff00) | ((b >> 10) & 0xff);
        //            rgb[yp] = (byte)arg;
        //        }
        //    }
        //    return rgb;
        //}
    }
}
