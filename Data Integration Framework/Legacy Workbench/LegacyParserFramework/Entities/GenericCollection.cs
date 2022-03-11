using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This class enables us to define Collections which are serializable.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyWorkbench.Entities
{
    public class GenericCollection<T> : ArrayList
    {
        /// <summary>
        /// Type specific implementation of ArrayList.
        /// </summary>
        /// <param name="index">The index in the array list which has to be returned.</param>
        /// <returns>Genric type which can be found at index position in the ArrayList.</returns>
        public new T this[int index]
        {
            get
            {
                return (T)base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Method to add a generic type T to the collection
        /// </summary>
        /// <param name="value">The object to be added</param>
        /// <returns>The index of the object where the item is added</returns>
        public int Add(T value)
        {
            return base.Add(value);
        }

        /// <summary>
        /// This method can be used to add an array of generic type objects
        /// </summary>
        /// <param name="index">The index where the array should be added to.</param>
        /// <param name="array">The gneric type T array which has to be added</param>
        /// <param name="arrayIndex">The zero based index where copying should begin</param>
        /// <param name="count">The number of items to be added.</param>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            base.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>
        /// Method to remove an object from the collection.
        /// </summary>
        /// <param name="obj">The object to be removed</param>
        public void Remove(T obj)
        {
            base.Remove(obj);
        }
        /// <summary>
        /// Method to add an array to the collection
        /// </summary>
        /// <param name="array">The items in the array which should be added</param>
        /// <param name="arrayIndex">The location where the array 
        /// objects should be added</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Method to insert objects to the collection
        /// </summary>
        /// <param name="index">Location where the object should be inserted</param>
        /// <param name="item">Object to be inserted</param>
        public void Insert(int index, T item)
        {
            base.Insert(index, item);
        }

        /// <summary>
        /// Used to find the index of an element in the collection
        /// </summary>
        /// <param name="item">The object whose index is required</param>
        /// <returns>index of the item in the collection</returns>
        public int IndexOf(T item)
        {
            return base.IndexOf(item);
        }

        /// <summary>
        /// Indicates whether the given item is present in the collection
        /// </summary>
        /// <param name="item">The item which should be chekced for in the collection</param>
        /// <returns>
        /// True, if the item exists in the collection. 
        /// False, otherwise
        /// </returns>
        public bool Contains(T item)
        {
            return base.Contains(item);
        }


        //#region ICollection<T> Members

        //void ICollection<T>.Add(T item)
        //{
        //    base.Add(item);
        //}

        //bool ICollection<T>.Remove(T item)
        //{
        //    base.Remove(item);
        //    return true;
        //}

        //#endregion

        //#region IEnumerable<T> Members

        //public new IEnumerator<T> GetEnumerator()
        //{
        //    return (IEnumerator<T>)GetEnumerator(0, base.Count);
        //}

        //#endregion
}
}
