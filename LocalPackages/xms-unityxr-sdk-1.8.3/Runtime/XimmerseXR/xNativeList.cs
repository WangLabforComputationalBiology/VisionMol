using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
namespace Ximmerse.XR.Collections
{
    struct InternalPointer
    {
        public IntPtr m_Pointer;
    }

    /// <summary>
    /// Native list which compares to Unity.Collections.NativeList, this type is able to be used as generic type to Unity's native array or native list collection types.
    /// 
    /// Make sure Dispose() is called properly.
    /// 
    /// Note: foreach() has GC problem due to type casting, using for() in most case.
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal unsafe struct xNativeList<T> : IDisposable, IEnumerable<T>, IList<T>, IReadOnlyList<T> where T : unmanaged
    {

        const int kDefaultIncrementElement = 8;

        private const string kErrorMsgFormat01 = "Invalid index: {0} of accessing array length = {1}";
        private const string kErrorMsg02 = "PENativeList is not created !";

        /// <summary>
        /// Length of int, should be 4 in x64 platform.
        /// </summary>
        static readonly int kLengthInt = Marshal.SizeOf<int>();

        /// <summary>
        /// Double kLengthInt.
        /// </summary>
        static readonly int kLengthTwoInt = 2 * Marshal.SizeOf<int>();

        /// <summary>
        /// Capacity of the list
        /// </summary>
        public int Capacity
        {
            get
            {
                CheckSafety();
                return Marshal.ReadInt32(m_InternalPointer->m_Pointer);
            }
            set
            {
                CheckSafety();
                Marshal.WriteInt32(m_InternalPointer->m_Pointer, value);
            }
        }

        /// <summary>
        /// Element length of the list
        /// </summary>
        public int Length
        {
            get
            {
                CheckSafety();
                return Marshal.ReadInt32(m_InternalPointer->m_Pointer, kLengthInt);
            }

            private set
            {
                CheckSafety();
                Marshal.WriteInt32(m_InternalPointer->m_Pointer, kLengthInt, value);
            }
        }

        int m_ElementSize;

        /// <summary>
        /// Element size in memory bytes.
        /// </summary>
        public int ElementSize
        {
            get
            {
                if (m_ElementSize == 0)
                {
                    m_ElementSize = Marshal.SizeOf<T>();
                }
                return m_ElementSize;
            }
        }

        InternalPointer* m_InternalPointer;

        /// <summary>
        /// Gets the pointer of the list memory.
        /// First int(32bits, or 4 bytes) = capacity.
        /// Second int(32bits, or 4 bytes) = length.
        /// </summary>
        public IntPtr Pointer
        {
            get
            {
                CheckSafety();
                return m_InternalPointer->m_Pointer;
            }
        }

        /// <summary>
        /// Gets the data pointer of the list. The capacity / length is skipped .
        /// The pointer should be used for reading or modifying data only.  
        /// DO NOT INSERT/DELETE DATA BY THIS POINTER !
        /// </summary>
        public IntPtr ReadOnlyDataPointer
        {
            get
            {
                CheckSafety();
                return IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt);
            }
        }

        bool m_IsCreated;

        /// <summary>
        /// If the native list has been allocated.
        /// </summary>
        public bool IsCreated
        {
            get => m_IsCreated;
        }

        /// <summary>
        /// Element count of the list
        /// </summary>
        public int Count => Length;

        /// <summary>
        /// Is the list readonly ?
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Create a new PENativeList with initial capacity.
        /// </summary>
        /// <param name="Capacity"></param>
        /// <returns></returns>
        public static xNativeList<T> Create(int Capacity)
        {
            xNativeList<T> list = default;
            list.m_ElementSize = Marshal.SizeOf<T>();
            int sizeOfInterPtr = Marshal.SizeOf<InternalPointer>();
            list.m_InternalPointer = (InternalPointer*)Marshal.AllocHGlobal(sizeOfInterPtr).ToPointer();
            list.m_InternalPointer->m_Pointer = Marshal.AllocHGlobal(list.m_ElementSize * Capacity + kLengthTwoInt); //[capacity] | [length] | [content] 
            Marshal.WriteInt32(list.m_InternalPointer->m_Pointer, Capacity);
            Marshal.WriteInt32(list.m_InternalPointer->m_Pointer, kLengthInt, 0);
            list.m_IsCreated = true;
            return list;
        }

        /// <summary>
        /// Create a new PENativeList by given array
        /// </summary>
        /// <param name="nativeArray"></param>
        /// <returns></returns>
        public static xNativeList<T> Create(NativeArray<T> nativeArray)
        {
            xNativeList<T> list = default;
            list.m_ElementSize = Marshal.SizeOf<T>();
            int lengthInByte = list.m_ElementSize * nativeArray.Length;
            list.m_InternalPointer = (InternalPointer*)Marshal.AllocHGlobal(Marshal.SizeOf<InternalPointer>()).ToPointer();
            list.m_InternalPointer->m_Pointer = Marshal.AllocHGlobal(lengthInByte + kLengthTwoInt); //[capacity] | [length] | [content] 
            Buffer.MemoryCopy(nativeArray.GetUnsafePtr(), IntPtr.Add(list.m_InternalPointer->m_Pointer, kLengthTwoInt).ToPointer(), lengthInByte, lengthInByte);
            Marshal.WriteInt32(list.m_InternalPointer->m_Pointer, nativeArray.Length);//capacity
            Marshal.WriteInt32(IntPtr.Add(list.m_InternalPointer->m_Pointer, kLengthInt), nativeArray.Length);//length
            list.m_IsCreated = true;
            return list;
        }

        /// <summary>
        /// Create a new PENativeList by given array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static xNativeList<T> Create(T[] array)
        {
            xNativeList<T> list = default;
            list.m_ElementSize = Marshal.SizeOf<T>();
            int lengthInByte = list.m_ElementSize * array.Length;
            list.m_InternalPointer = (InternalPointer*)Marshal.AllocHGlobal(Marshal.SizeOf<InternalPointer>()).ToPointer();
            list.m_InternalPointer->m_Pointer = Marshal.AllocHGlobal(lengthInByte + kLengthTwoInt); //[capacity] | [length] | [content] 
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement<T>(array, 0);
            Buffer.MemoryCopy(ptr.ToPointer(), IntPtr.Add(list.m_InternalPointer->m_Pointer, kLengthTwoInt).ToPointer(), lengthInByte, lengthInByte);
            Marshal.WriteInt32(list.m_InternalPointer->m_Pointer, array.Length);//capacity
            Marshal.WriteInt32(IntPtr.Add(list.m_InternalPointer->m_Pointer, kLengthInt), array.Length);//length
            list.m_IsCreated = true;
            return list;
        }


        /// <summary>
        /// Allocates by another native list.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static xNativeList<T> Create(xNativeList<T> source)
        {
            xNativeList<T> list = default;
            list.m_ElementSize = Marshal.SizeOf<T>();
            int lengthInByte = list.m_ElementSize * source.Length;
            list.m_InternalPointer = (InternalPointer*)Marshal.AllocHGlobal(Marshal.SizeOf<InternalPointer>()).ToPointer();
            list.m_InternalPointer->m_Pointer = Marshal.AllocHGlobal(lengthInByte + kLengthTwoInt); //[capacity] | [length] | [content] 
            //从source pointer执行 memory copy:
            IntPtr ptr_source = source.m_InternalPointer->m_Pointer;
            Buffer.MemoryCopy(ptr_source.ToPointer(), (list.m_InternalPointer->m_Pointer).ToPointer(), lengthInByte + kLengthTwoInt, lengthInByte + kLengthTwoInt);
            list.m_IsCreated = true;
            return list;
        }

        /// <summary>
        /// Gets/sets array element.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                CheckSafety();
                int length = this.Length;
                if (index < 0 || index >= length)
                {
                    throw new ArgumentOutOfRangeException(string.Format(kErrorMsgFormat01, index, length));
                }
                T val = Marshal.PtrToStructure<T>(IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + index * ElementSize));
                return val;
            }
            set
            {
                CheckSafety();
                int length = this.Length;
                if (index < 0 || index >= length)
                {
                    throw new ArgumentOutOfRangeException(string.Format(kErrorMsgFormat01, index, length));
                }
                T* p = &value;
                Buffer.MemoryCopy(p, IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + index * ElementSize).ToPointer(), ElementSize, ElementSize);
            }
        }

        /// <summary>
        /// Appends a value to the list.
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value)
        {
            CheckSafety();
            int length = this.Length;
            int capacity = this.Capacity;
            if (length >= capacity)
            {
                Reallocate(capacity + kDefaultIncrementElement);
            }
            T* p = &value;
            Buffer.MemoryCopy(p, IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + length * ElementSize).ToPointer(), ElementSize, ElementSize);
            Length = length + 1;
        }

        /// <summary>
        /// Add multiple elements.
        /// </summary>
        /// <param name="values"></param>
        public void AddRange(T[] values)
        {
            CheckSafety();
            int length = this.Length;
            int capacity = this.Capacity;
            if (length + values.Length >= capacity)
            {
                Reallocate(capacity + values.Length);
            }
            fixed (void* ptr = values)
            {
                Buffer.MemoryCopy(ptr, IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + length * ElementSize).ToPointer(), ElementSize * values.Length, ElementSize * values.Length);
                Length = length + values.Length;
            }
        }

        /// <summary>
        /// Add multiple elements.
        /// </summary>
        /// <param name="array"></param>
        public void AddRange(NativeArray<T> array)
        {
            void* src = array.GetUnsafePtr();
            if (this.Length + array.Length > this.Capacity)
            {
                Reallocate(this.Length + array.Length);
            }
            int currentLength = this.Length;
            void* dst = IntPtr.Add(this.m_InternalPointer->m_Pointer, kLengthTwoInt + currentLength * ElementSize).ToPointer();
            Buffer.MemoryCopy(src, dst, array.Length * ElementSize, array.Length * ElementSize);
            this.Length = array.Length + currentLength;
        }

        /// <summary>
        /// Remove element at index.
        /// Performance tips : remove from the last index has best performance than remove from the head.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            CheckSafety();
            int length = this.Length;
            if (index < 0 || index >= length)
            {
                throw new ArgumentOutOfRangeException(string.Format(kErrorMsgFormat01, index, length));
            }
            if (index == length - 1)
            {
                Length = length - 1;
            }
            else
            {
                //preservedLength 是要保留的内容长度
                int preservedLength = length - index;
                IntPtr slice = GetSlice(index + 1, preservedLength);
                Buffer.MemoryCopy(
    slice.ToPointer(),
    IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + index * ElementSize).ToPointer(),
    ElementSize * preservedLength,
    ElementSize * preservedLength);
                Marshal.FreeHGlobal(slice);
                Length = length - 1;
            }
        }

        /// <summary>
        /// Removes element 
        /// </summary>
        /// <param name="value"></param>
        public bool Remove(T value)
        {
            CheckSafety();
            int length = this.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                if (this[i].Equals(value))
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Convert the native list to native array.
        /// </summary>
        /// <returns></returns>
        public NativeArray<T> ToArray(Allocator allocator)
        {
            NativeArray<T> array = new NativeArray<T>(this.Length, allocator);
            xNativeList<T> list = this;
            for (int i = 0, iMax = list.Count; i < iMax; i++)
            {
                array[i] = list[i];
            }
            return array;
        }

        /// <summary>
        /// Clears all element
        /// </summary>
        public void Clear()
        {
            CheckSafety();
            Length = 0;
        }

        /// <summary>
        /// Reallocate a longer memory, the exists content is copied to new allocated memory area.List length remains unchanged.
        /// Content length remains unchanged , while the list's capacity is extended.
        /// </summary>
        /// <param name="length">The element length.</param>
        internal void Reallocate(int length)
        {
            IntPtr newBufferPtr = Marshal.AllocHGlobal(kLengthTwoInt + length * ElementSize);
            Buffer.MemoryCopy(m_InternalPointer->m_Pointer.ToPointer(), newBufferPtr.ToPointer(), kLengthTwoInt + ElementSize * Length, kLengthTwoInt + ElementSize * Length);
            Marshal.FreeHGlobal(m_InternalPointer->m_Pointer);
            m_InternalPointer->m_Pointer = newBufferPtr;
            Capacity = length;
        }

        /// <summary>
        /// Gets a slice inside the list.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        IntPtr GetSlice(int index, int length)
        {
            IntPtr slicePtr = Marshal.AllocHGlobal(length * ElementSize);
            IntPtr src = IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + index * ElementSize);
            Buffer.MemoryCopy(src.ToPointer(), slicePtr.ToPointer(), ElementSize * length, ElementSize * length);
            return slicePtr;
        }

        /// <summary>
        /// Gets the element's data pointer at the index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="elementPtr"></param>
        /// <returns></returns>
        public unsafe bool GetElementPtr(int index, out T* elementPtr)
        {
            elementPtr = default;
            if (this.Length <= index)
            {
                return false;
            }
            var ptr = IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + m_ElementSize * index);
            elementPtr = (T*)ptr.ToPointer();
            return true;
        }

        /// <summary>
        /// Dispose the native list and deallocate the memory.
        /// </summary>
        public void Dispose()
        {
            if (IsCreated)
            {
                Marshal.FreeHGlobal(this.m_InternalPointer->m_Pointer);
                Marshal.FreeHGlobal(new IntPtr(m_InternalPointer));
                m_IsCreated = false;
            }
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            CheckSafety();
            return new NativeListEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            CheckSafety();
            return new NativeListEnumerator<T>(this);
        }

        /// <summary>
        /// Find index of the element
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            CheckSafety();
            int length = this.Length;
            for (int i = 0, imax = length; i < imax; i++)
            {
                if (this[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Inserts item at the index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            CheckSafety();
            int length = this.Length;
            int capacity = this.Capacity;
            if (length >= capacity)
            {
                Reallocate(capacity + kDefaultIncrementElement);
            }

            //先将 index 以后的元素后移一位:
            int preservedLength = length - index;
            IntPtr slice = GetSlice(index, preservedLength);
            Buffer.MemoryCopy(
slice.ToPointer(),
IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + (index + 1) * ElementSize).ToPointer(),
ElementSize * preservedLength,
ElementSize * preservedLength);
            Marshal.FreeHGlobal(slice);

            //然后将新的内容copy到 index 对应的位置
            T* p = &item;
            Buffer.MemoryCopy(p, IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + index * ElementSize).ToPointer(), ElementSize, ElementSize);
            Length = length + 1;
        }

        /// <summary>
        /// Inserts items at the index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="items"></param>
        public void InsertValues(int index, T[] items)
        {
            CheckSafety();
            int length = this.Length;
            int capacity = this.Capacity;
            if (length + items.Length >= capacity)
            {
                Reallocate(capacity +
items.Length + kDefaultIncrementElement);
            }

            //先将 index 以后的元素后移N位:
            int preservedLength = length - index;
            IntPtr slice = GetSlice(index, preservedLength);
            Buffer.MemoryCopy(
slice.ToPointer(),
IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + (index + items.Length) * ElementSize).ToPointer(),
ElementSize * preservedLength,
ElementSize * preservedLength);
            Marshal.FreeHGlobal(slice);

            //然后将新的内容copy到 index 对应的位置
            IntPtr srcPtr = Marshal.UnsafeAddrOfPinnedArrayElement<T>(items, 0);
            Buffer.MemoryCopy(srcPtr.ToPointer(), IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + index * ElementSize).ToPointer(), ElementSize * items.Length, ElementSize * items.Length);
            Length = length + items.Length;
        }


        /// <summary>
        /// Inserts items at the index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void InsertValues(int index, NativeArray<T> items)
        {
            CheckSafety();
            int length = this.Length;
            int capacity = this.Capacity;
            if (length + items.Length >= capacity)
            {
                Reallocate(capacity +
items.Length + kDefaultIncrementElement);
            }

            //先将 index 以后的元素后移N位:
            int preservedLength = length - index;
            IntPtr slice = GetSlice(index, preservedLength);
            Buffer.MemoryCopy(
slice.ToPointer(),
IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + (index + items.Length) * ElementSize).ToPointer(),
ElementSize * preservedLength,
ElementSize * preservedLength);
            Marshal.FreeHGlobal(slice);

            //然后将新的内容copy到 index 对应的位置
            void* srcPtr = items.GetUnsafePtr();
            Buffer.MemoryCopy(srcPtr, IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt + index * ElementSize).ToPointer(), ElementSize * items.Length, ElementSize * items.Length);
            Length = length + items.Length;
        }

        /// <summary>
        /// If the item presents in the list 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            CheckSafety();
            int length = this.Length;
            for (int i = 0, imax = length; i < imax; i++)
            {
                if (this[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Copy content to array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            CheckSafety();
            int length = this.Length;
            if ((arrayIndex + length) > array.Length)
            {
                throw new ArgumentOutOfRangeException(string.Format("ArrayIndex + m_Length: {0} GE array length :{1}", arrayIndex + length, array.Length));
            }
            if (length == 0)
            {
                return;//nothing to copy ...
            }
            fixed (void* dst = array)
            {
                IntPtr dstPtr = arrayIndex > 0 ? IntPtr.Add(new IntPtr(dst), ElementSize * arrayIndex) : new IntPtr(dst);
                Buffer.MemoryCopy(IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt).ToPointer(), dstPtr.ToPointer(), ElementSize * length, ElementSize * length);
            }
        }

        /// <summary>
        /// Copy content to array
        /// </summary>
        /// <param name="nativeArray"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(NativeArray<T> nativeArray, int arrayIndex)
        {
            CheckSafety();
            int length = this.Length;
            if ((arrayIndex + length) > nativeArray.Length)
            {
                throw new ArgumentOutOfRangeException(string.Format("ArrayIndex + m_Length: {0} GE array length :{1}", arrayIndex + length, nativeArray.Length));
            }
            if (length == 0)
            {
                return;//nothing to copy ...
            }
            void* ptr = nativeArray.GetUnsafePtr();
            Buffer.MemoryCopy(IntPtr.Add(m_InternalPointer->m_Pointer, kLengthTwoInt).ToPointer(), ptr, ElementSize * length, ElementSize * length);
        }



        private void CheckSafety()
        {
            if (!m_IsCreated)
            {
                throw new InvalidOperationException(kErrorMsg02);
            }
        }

        /// <summary>
        /// Enumerator for PENativeList
        /// </summary>
        internal struct NativeListEnumerator<T> : IEnumerator<T> where T : unmanaged
        {
            xNativeList<T> m_list;

            int m_index;

            public NativeListEnumerator(xNativeList<T> lst)
            {
                m_list = lst;
                m_index = -1;
            }

            public T Current => m_list[m_index];

            object IEnumerator.Current => m_list[m_index];

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                m_index++;
                int max = m_list.Length;
                if (m_index < max)
                {
                    return true;
                }
                else return false;
            }

            public void Reset()
            {
                m_index = -1;
            }

        }
    }
}