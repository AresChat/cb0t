using System;
using System.Collections.Generic;
using System.Text;

namespace cb0t_chat_client_v2
{
    class ListEx<T> : List<T>
    {
        public int SequenceIndexOf(T[] sequence)
        {
            if (sequence.Length > this.Count)
                return -1;

            for (int i = 0; i < this.Count; i++)
            {
                if ((i + sequence.Length) > this.Count)
                    return -1;

                if (this.CompareArrays(this.GetRange(i, sequence.Length).ToArray(), sequence))
                    return i;
            }

            return -1;
        }

        public int SequenceLastIndexOf(T[] sequence)
        {
            if (sequence.Length > this.Count)
                return -1;

            int index = -1;

            for (int i = 0; i < this.Count; i++)
            {
                if ((i + sequence.Length) > this.Count)
                    return index;

                if (this.CompareArrays(this.GetRange(i, sequence.Length).ToArray(), sequence))
                    index = i;
            }

            return index;
        }

        private bool CompareArrays(T[] arr1, T[] arr2)
        {
            for (int i = 0; i < arr1.Length; i++)
                if (!arr1[i].Equals(arr2[i]))
                    return false;

            return true;
        }

    }
}
