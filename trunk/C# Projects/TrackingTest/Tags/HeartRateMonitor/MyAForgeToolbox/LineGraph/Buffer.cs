using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Graph
{
    class Buffer
    {

        #region Constructor

        public Buffer(int initialCount)
        {
            mMaxcount = initialCount;
            PointQ = new Queue(initialCount);
            Point2Q = new Queue(initialCount);
            timeValQ = new Queue(initialCount);
            timeQ = new Queue(initialCount);   
            pointBuf = new float[initialCount];
            point2Buf = new float[initialCount];
            timeValBuf = new string[initialCount];
            timeBuf = new string[initialCount];
            
        }

        #endregion

        #region Variables

        float[] pointBuf;
        float[] point2Buf;
        string[] timeValBuf;
        string[] timeBuf;
        Queue PointQ = null;
        Queue Point2Q = null;
        Queue timeValQ = null;
        Queue timeQ = null;
        int miTracker = 0;
        int mi2Tracker = 0;
        int mMaxcount = 0;

        #endregion

        #region Point

        public int PointCapacity
        {
            get { return this.pointBuf.Length; }
        }
        int pointHead = 0, pointTail = 0;
        bool pointEmpty = true;

        public int PointCount
        {
            get
            {
                if (pointEmpty) return 0;

                int count = pointHead - pointTail;
                if (count <= 0) count += pointBuf.Length;
                return count;
            }
        }

        public void PointAdd(float value)
        {
            if (!pointEmpty && pointHead == pointTail)
            {
                pointTail = (pointTail + 1) % pointBuf.Length;
            }

            miTracker = miTracker + 1;
            if (miTracker > mMaxcount)
                PointQ.Dequeue();
            PointQ.Enqueue(value);
            PointQ.CopyTo(pointBuf, 0);
            pointHead = (pointHead + 1) % pointBuf.Length;

            pointEmpty = false;
        }

        public float GetIndex(int i, string name)
        {
            if (name.Equals("Point"))
            {
                return pointBuf[PointShiftIndex(i)];
            }
            return 0;
        }

        public string GetTIMEIndex(int i,string type)
        {
            if (type.Equals("PointTime"))
                return timeValBuf[TimeValShiftIndex(i)];
            else if (type.Equals("Time"))
                return timeBuf[TimeShiftIndex(i)];
            else
                return timeBuf[TimeShiftIndex(i)];


        }

        int PointShiftIndex(int i)
        {
            if (i > PointCount) throw new ArgumentException("Index must be less than Count.");

            return (i) % pointBuf.Length;
        }

        int TIMEShiftIndex(int i)
        {
            if (i > timeValCount) throw new ArgumentException("Index must be less than Count.");

            return (i) % timeBuf.Length;
        }

        #endregion

        #region Point2

        public int Point2Capacity
        {
            get { return this.point2Buf.Length; }
        }
        int point2Head = 0, point2Tail = 0;
        bool point2Empty = true;

        public int Point2Count
        {
            get
            {
                if (point2Empty) return 0;

                int count = point2Head - point2Tail;
                if (count <= 0) count += point2Buf.Length;
                return count;
            }
        }

        public void Point2Add(float value)
        {
            if (!point2Empty && point2Head == point2Tail)
            {
                point2Tail = (point2Tail + 1) % point2Buf.Length;
            }

            mi2Tracker = mi2Tracker + 1;
            if (mi2Tracker > mMaxcount)
                Point2Q.Dequeue();
            Point2Q.Enqueue(value);
            Point2Q.CopyTo(point2Buf, 0);
            point2Head = (point2Head + 1) % point2Buf.Length;

            pointEmpty = false;
        }

        public float GetIndex2(int i, string name)
        {
            if (name.Equals("Point"))
            {
                return point2Buf[PointShiftIndex(i)];
            }
            return 0;
        }


        int Point2ShiftIndex(int i)
        {
            if (i > Point2Count) throw new ArgumentException("Index must be less than Count.");

            return (i) % point2Buf.Length;
        }

        #endregion

        #region time Value


        public int timeValCapacity
        {
            get { return this.timeValBuf.Length; }
        }

        int timeValHead = 0, timeValTail = 0;
        bool timeValEmpty = true;

        public int timeValCount
        {
            get
            {
                if (timeValEmpty) return 0;

                int count = timeValHead - timeValTail;
                if (count <= 0) count += timeValBuf.Length;
                return count;
            }
        }

        public void TimeValAdd(string value)
        {
            if (!timeValEmpty && timeValHead == timeValTail)
            {
                timeValTail = (timeValTail + 1) % timeValBuf.Length;
            }


            if (miTracker > mMaxcount)
                timeValQ.Dequeue();
            timeValQ.Enqueue(value);
            timeValQ.CopyTo(timeValBuf, 0);

            //mtimeValBuf[mitimeValHead] = value;

            timeValHead = (timeValHead + 1) % timeValBuf.Length;

            timeValEmpty = false;
        }

        int TimeValShiftIndex(int i)
        {
            if (i > timeValCount) throw new ArgumentException("Index must be less than Count.");

            return (timeValTail + i) % timeValBuf.Length;
        }


        #endregion

        #region time


        public int timeCapacity
        {
            get { return this.timeBuf.Length; }
        }

        int timeHead = 0, timeTail = 0;
        bool timeEmpty = true;

        public int timeCount
        {
            get
            {
                if (timeEmpty) return 0;

                int count = timeHead - timeTail;
                if (count <= 0) count += timeBuf.Length;
                return count;
            }
        }

        public void TimeAdd(string value)
        {
            if (!timeEmpty && timeHead == timeTail)
            {
                timeTail = (timeTail + 1) % timeBuf.Length;
            }


            if (miTracker > mMaxcount)
                timeQ.Dequeue();
            timeQ.Enqueue(value);
            timeQ.CopyTo(timeBuf, 0);

            //mtimeBuf[mitimeHead] = value;

            timeHead = (timeHead + 1) % timeBuf.Length;
            //timeBuf[timeHead] = value;

            timeEmpty = false;
        }

        int TimeShiftIndex(int i)
        {
            if (i > timeCount) throw new ArgumentException("Index must be less than Count.");

            return (timeTail + i) % timeBuf.Length;
        }


        #endregion

        #region Public methods

        public void ClearBuffer()
        {
            Array.Clear(pointBuf, 0, pointBuf.Length);
            Array.Clear(point2Buf, 0, point2Buf.Length);
            Array.Clear(timeValBuf, 0, timeValBuf.Length);
            Array.Clear(timeBuf, 0, timeBuf.Length);

            PointQ.Clear();
            Point2Q.Clear();
            timeValQ.Clear();
            timeQ.Clear();
            pointHead = 0; pointTail = 0;
            pointEmpty = true;
            point2Head = 0; point2Tail = 0;
            point2Empty = true;
            timeHead = 0; timeTail = 0;
            timeEmpty = true;
            timeValHead = 0; timeValTail = 0;
            timeValEmpty = true;
            miTracker = 0;
            mi2Tracker = 0;
           
        }

        #endregion

    }

}
