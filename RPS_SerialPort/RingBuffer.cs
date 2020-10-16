using System;

namespace rps_serialport_potentiometer
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
        private T[] _buffer;

        /// <summary>
        /// Tail index of buffer
        /// </summary>
        private int _readPointer;

        /// <summary>
        /// Head index of buffer
        /// </summary>
        private int _writePointer;

        /// <summary>
        /// Number of elements stored in buffer
        /// </summary>
        private int _count;

        /// <summary>
        /// Last element stored
        /// </summary>
        public T LastElement
        {
            get
            {
                if (_count == 0)
                {
                    throw new InvalidOperationException("Collection is empty.");
                }
                return _buffer[_readPointer];
            }
        }

        /// <summary>
        /// True if buffer is empty
        /// </summary>
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity">Data buffer size</param>
        public RingBuffer(int capacity)
        {
            _buffer = new T[capacity];
            _readPointer = 0;
            _writePointer = 0;
        }

        /// <summary>
        /// Push singe value to buffer
        /// </summary>
        /// <param name="value">Value to be pushed</param>
        public void Enqueue(T value)
        {
            _buffer[_writePointer] = value;
            _writePointer = (_writePointer + 1) % _buffer.Length;
            if (_count < _buffer.Length)
            {
                _count++;
            }
            else
            {
                _readPointer = (_readPointer + 1) % _buffer.Length;
            }
        }

        /// <summary>
        /// Pop single value from buffer
        /// </summary>
        /// <returns>Popped value</returns>
        public T Dequeue()
        {
            if (_count == 0)
                throw new InvalidOperationException("Collection is empty.");
            T value = _buffer[_readPointer];
            _buffer[_readPointer] = default(T);
            _readPointer = (_readPointer + 1) % _buffer.Length;
            _count--;
            return value;
        }

        /// <summary>
        /// Clear buffer
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _buffer.Length; i++)
            {
                _buffer[i] = default(T);
            }
            _readPointer = 0;
            _writePointer = 0;
            _count = 0;
        }
    }
}
