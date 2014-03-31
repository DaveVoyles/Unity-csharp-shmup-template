/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///	A list that works like a Dictionary
/// </summary>
public class KeyValueList<K, V> : IList 
{
    // Backing data - seperate key and value lists
    private List<K> keyList = new List<K>();
    private List<V> valList = new List<V>();


    #region Constructors
    // Default constructor
    public KeyValueList() { }

    // Use External Lists
    public KeyValueList(ref List<K> keyListRef, ref List<V> valListRef) 
    {
        this.keyList = keyListRef;
        this.valList = valListRef;
    }

    // Construtor which takes another KeyValueList and copies it in.
    public KeyValueList(KeyValueList<K, V> otherKeyValueList)
    {
        this.AddRange(otherKeyValueList);  // Custom implementation below
    }
    #endregion Constructors

    /// <summary>
    /// Tests for the existance of the key in the KeyValueList and populates an
    /// 'out' value at the same time.
    /// </summary>
    /// <param name="key">The key used to get the value</param>
    /// <param name="value">The value from the list or the Type default</param>
    /// <returns>True if the key exists in the KeyValueList, or false if not</returns>
    public bool TryGetValue(K key, out V value)
    {
        int index = keyList.IndexOf(key);
        if (index == -1)  // Failed to find key
        {
            value = default(V);
            return false;
        }

        value = this.valList[index];
        return true;
    }

    public int Add(object value)
    {
        throw new System.NotImplementedException("Use KeyValueList[key] = value or insert");
    }

    public void Clear()
    {
        this.keyList.Clear();
        this.valList.Clear();
    }

    public bool Contains(V value)
    {
        return this.valList.Contains(value);
    }

    public bool ContainsKey(K key)
    {
        return this.keyList.Contains(key);
    }

    public int IndexOf(K key)  // Needed for insert even if we don't use this[index] syntax
    {
        return keyList.IndexOf(key);
    }

    public void Insert(int index, K key, V value)
    {
        if (this.keyList.Contains(key))
            throw new System.Exception("Cannot insert duplicate key.");

        this.keyList.Insert(index, key);
        this.valList.Insert(index, value);
    }

    public void Insert(int index, KeyValuePair<K, V> kvp)
    {
        this.Insert(index, kvp.Key, kvp.Value);
    }

    public void Insert(int index, object value)
    {
        string msg = "Use Insert(K key, V value) or Insert(KeyValuePair<K, V>)";
        throw new System.NotImplementedException(msg);
    }

    public void Remove(K key)
    {
        int index = keyList.IndexOf(key);
        if (index == -1) throw new KeyNotFoundException();
        
        this.keyList.RemoveAt(index);
        this.valList.RemoveAt(index);
    }
    
    public void Remove(object value)
    {
        throw new System.NotImplementedException("Use Remove(K key)");
    }

    public void RemoveAt(int index)
    {
        this.keyList.RemoveAt(index);
        this.valList.RemoveAt(index);
    }
     
    /// <summary>
    /// Dictionary-like set/get. E.g. KeyValueList[key] = value
    /// </summary>
    /// <param name="key">The key used to get the value</param>
    /// <returns>he value found or error</returns>
    public V this[K key]
    {
        get 
        {
            V value;
            if (this.TryGetValue(key, out value))
                return value; 
            else
                throw new KeyNotFoundException();
        }

        set 
        {
            int index = this.keyList.IndexOf(key);
            if (index == -1)
            {
                this.keyList.Add(key);
                this.valList.Add(value);
            }
            else 
            {
                this.valList[index] = value;
            }

        }
    }

    /// <summary>
    /// Provides index-based access 
    /// </summary>
    /// <param name="index">The index of the value to get</param>
    /// <returns>The value found or error</returns>
    public V GetAt(int index)
    {
        if (index >= this.valList.Count)
            throw new System.IndexOutOfRangeException();
        else
            return this.valList[index];
    }

    /// <summary>
    /// Provides index-based setting 
    /// </summary>
    /// <param name="index">The index of the value to get</param>
    /// <returns>The value found or error</returns>
    public void SetAt(int index, V value)
    {
        if (index >= this.valList.Count)
            throw new System.IndexOutOfRangeException();
        else
            this.valList[index] = value;
    }

    /// <summary>
    /// Used by OTHERList.AddRange()
    /// This adds this list of values to the passed list. The keys are not copied over
    /// </summary>
    /// <param name="array">The list AddRange is being called on</param>
    /// <param name="arrayIndex">
    /// The starting index for the copy operation. AddRange seems to pass the last index.
    /// </param>
    public void CopyTo(V[] array, int index)
    {
        this.valList.CopyTo(array, index);
    }

    /// <summary>
    /// Used by OTHERKeyValueList.AddRange()
    /// This adds this list of values to the passed list. The keys are not copied over
    /// </summary>
    /// <param name="array">The list AddRange is being called on</param>
    /// <param name="arrayIndex">
    /// The starting index for the copy operation. AddRange seems to pass the last index.
    /// </param>
    public void CopyTo(KeyValueList<K, V> otherKeyValueList, int index)
    {
        foreach (KeyValuePair<K, V> kvp in this)
            otherKeyValueList[kvp.Key] = kvp.Value;
    }

    /// <summary>
    /// Appends (copies) another KeyValueList to the end of this KeyValueList
    /// </summary>
    /// <param name="otherKeyValueList">he list to copy from</param>
    public void AddRange(KeyValueList<K, V> otherKeyValueList)
    {
        otherKeyValueList.CopyTo(this, 0);
    }

    public int Count { get { return this.valList.Count; } }

    
    /// <summary>
    /// Impliments the ability to use this like a dictionary in a foreach loop
    /// </summary>
    /// <returns></returns>
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        foreach (K key in this.keyList)
            yield return new KeyValuePair<K, V>(key, this[key]);
    }

    // Non-generic version? Not sure why this is used by the interface
    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (K key in this.keyList)
            yield return new KeyValuePair<K, V>(key, this[key]);
    }

    /// <summary>
    /// Returns a formatted string showing all the prefab names
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        // Get a string[] array of the keys for formatting with join()
        var strArray = new string[this.keyList.Count];
        var formatStr = "{0}:{1}";
        int i = 0;
        foreach (KeyValuePair<K, V> kvp in this)
        {
            strArray[i] = string.Format(formatStr, kvp.Key, kvp.Value);
            i++;
        }
        
        // Return a comma-sperated list inside square brackets (Pythonesque)
        return string.Format("[{0}]", System.String.Join(", ", strArray));
    }




    public bool IsFixedSize { get { return false; } }
    public bool IsReadOnly { get { return false; } }


    // NOT IMPLIMENTED...
    public bool IsSynchronized { get { throw new System.NotImplementedException(); } }
    public object SyncRoot { get { throw new System.NotImplementedException(); } }

    /// <summary>
    /// Not Implimented. Use Overload
    /// </summary>
    public bool Contains(object value)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Not Implimented. Use Overload
    /// </summary>
    public int IndexOf(object value)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Not Implimented. Use Overload
    /// </summary>
    object IList.this[int index]
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Not Implimented. Use Overload
    /// </summary>
    public void CopyTo(System.Array array, int index)
    {
        throw new System.NotImplementedException();
    }
}

