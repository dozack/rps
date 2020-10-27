using System;

namespace RPS_Modbus
{
    /// <summary>
    /// Class <c>RingBuffer</c> for buffering data streams.
    /// Source: https://github.com/lucasrabiec/RingBuffer
    /// </summary>
    /// <typeparam name="T">Type of stored data</typeparam>
    public class RingBuffer<T>
    {
        /// <summary>
        /// Buffer for data storage
        /// </summary>
        private readonly T[] Buffer;

        /// <summary>
        /// Tail index of buffer
        /// </summary>
        private int ReadPointer;

        /// <summary>
        /// Head index of buffer
        /// </summary>
        private int WritePointer;

        /// <summary>
        /// Number of elements stored in buffer
        /// </summary>
        private int Count;

        /// <summary>
        /// Last element stored
        /// </summary>
        public T LastElement
        {
            get
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Collection is empty.");
                }
                return Buffer[ReadPointer];
            }
        }

        /// <summary>
        /// True if buffer is empty
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity">Data buffer size</param>
        public RingBuffer(int capacity)
        {
            Buffer = new T[capacity];
            ReadPointer = 0;
            WritePointer = 0;
        }

        /// <summary>
        /// Push singe value to buffer
        /// </summary>
        /// <param name="value">Value to be pushed</param>
        public void Enqueue(T value)
        {
            Buffer[WritePointer] = value;
            WritePointer = (WritePointer + 1) % Buffer.Length;
            if (Count < Buffer.Length)
            {
                Count++;
            }
            else
            {
                ReadPointer = (ReadPointer + 1) % Buffer.Length;
            }
        }

        /// <summary>
        /// Pop single value from buffer
        /// </summary>
        /// <returns>Popped value</returns>
        public T Dequeue()
        {
            if (Count == 0)
                throw new InvalidOperationException("Collection is empty.");
            T value = Buffer[ReadPointer];
            Buffer[ReadPointer] = default(T);
            ReadPointer = (ReadPointer + 1) % Buffer.Length;
            Count--;
            return value;
        }

        /// <summary>
        /// Clear buffer
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Buffer.Length; i++)
            {
                Buffer[i] = default(T);
            }
            ReadPointer = 0;
            WritePointer = 0;
            Count = 0;
        }
    }
}
