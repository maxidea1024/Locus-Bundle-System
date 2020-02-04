using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BundleSystem
{
    public static partial class BundleManager
    {
        struct TupleObjectKey : IEqualityComparer<TupleObjectKey>
        {
            public TupleObjectKey(Object a, Object b)
            {
                id1_ = a.GetInstanceID();
                id2_ = b.GetInstanceID();
            }

            public int id1_;
            public int id2_;

            public bool Equals(TupleObjectKey x, TupleObjectKey y)
            {
                return x.id1_ == y.id1_ && x.id2_ == y.id2_;
            }

            public int GetHashCode(TupleObjectKey obj)
            {
                unchecked
                {
                    return ((id1_ << 5) + id1_) ^ id2_;
                }
            }
        }

        class IndexedList<T>
        {
            public int CurrentIndex { get; private set; } = -1;
            List<T> innerList_;

            public int Count => innerList_.Count;

            public IndexedList(int capacity)
            {
                innerList_ = new List<T>(capacity);
            }

            public void Add(T item)
            {
                innerList_.Add(item);
            }

            public void RemoveAt(int index)
            {
                //swap remove
                if (Count <= 1) 
                {
                    innerList_.RemoveAt(index);
                }
                else if (index == Count - 1) //last index
                {
                    innerList_.RemoveAt(index);
                }
                else
                {
                    innerList_[index] = innerList_[Count - 1]; //set last index
                    innerList_.RemoveAt(Count - 1); //remove
                }
            }

            public void ResetCurrentIndex()
            {
                CurrentIndex = -1;
            }

            public bool TryGetNext(out T value)
            {
                if (Count == 0)
                {
                    value = default;
                    return false;
                }

                CurrentIndex++;
                if (CurrentIndex >= Count) 
                {
                    CurrentIndex = 0;
                }
                value = innerList_[CurrentIndex];
                return true;
            }
        }

        class IndexedDictionary<TKey, TValue>
        {
            public int CurrentIndex { get; private set; } = -1;


            List<KeyValuePair<TKey, TValue>> innerList_;
            Dictionary<TKey, int> keyDictionary_;

            public int Count => innerList_.Count;

            public TValue this[TKey key]
            {
                get => innerList_[keyDictionary_[key]].Value;
                set
                {
                    if (keyDictionary_.TryGetValue(key, out var index)) 
                    {
                        innerList_[index] = new KeyValuePair<TKey, TValue>(key, value);
                    }
                    else 
                    {
                        Add(key, value);
                    }
                }
            }

            public KeyValuePair<TKey, TValue> GetAtIndex(int index) => innerList_[index];

            public IndexedDictionary(int capacity)
            {
                innerList_ = new List<KeyValuePair<TKey, TValue>>(capacity);
                keyDictionary_ = new Dictionary<TKey, int>(capacity);
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                if (!keyDictionary_.TryGetValue(key, out var index))
                {
                    value = default;
                    return false;
                }

                value = innerList_[index].Value;
                return true;
            }

            public bool ContainsKey(TKey key) => keyDictionary_.ContainsKey(key);

            public void Add(TKey key, TValue value)
            {
                keyDictionary_.Add(key, innerList_.Count); //add key with list count
                innerList_.Add(new KeyValuePair<TKey, TValue>(key, value)); //index will be list count
            }

            public bool Remove(TKey key)
            {
                if (!keyDictionary_.TryGetValue(key, out var index)) 
                {
                    return false;
                }

                if (Count == 1) //only one
                {
                    innerList_.Clear();
                    keyDictionary_.Clear();
                }
                else if (index == Count - 1) //last index
                {
                    innerList_.RemoveAt(innerList_.Count - 1); //remote last
                    keyDictionary_.Remove(key); //remove key
                }
                else
                {
                    innerList_[index] = innerList_[innerList_.Count - 1]; //last object to remove index
                    innerList_.RemoveAt(innerList_.Count - 1); //remove last as it's duplicated
                    keyDictionary_[innerList_[index].Key] = index; //update last index's key
                    keyDictionary_.Remove(key); //remove key
                }

                return true;
            }

            public void ResetCurrentIndex()
            {
                CurrentIndex = -1;
            }

            public bool TryGetNext(out KeyValuePair<TKey, TValue> value)
            {
                if (Count == 0)
                {
                    value = default;
                    return false;
                }

                CurrentIndex++;
                if (CurrentIndex >= Count) 
                {
                    CurrentIndex = 0;
                }
                value = innerList_[CurrentIndex];
                return true;
            }
        }
    }
}
