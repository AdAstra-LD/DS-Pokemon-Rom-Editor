using System;
using System.Collections.Generic;

namespace DSPRE.ROMFiles {
    public class UniqueList<T> {
        private readonly List<T> list = new List<T>();
        private readonly HashSet<T> set = new HashSet<T>();

        public UniqueList(int capacity) {
            list = new List<T>(capacity);
            set = new HashSet<T>(capacity);
        }

        public UniqueList() { }

        public void Add(T item) {
            if (!set.Contains(item)) {
                list.Add(item);
                set.Add(item);
            }
        }

        public bool Contains(T item) {
            return set.Contains(item);
        }

        public void Clear() {
            list.Clear();
            set.Clear();
        }

        public bool Remove(T item) {
            if (set.Contains(item)) {
                list.Remove(item);
                set.Remove(item);
                return true;
            }
            return false;
        }
        public bool RemoveAt(int index) {
            if (index >= 0 && index < list.Count) {
                T itemToRemove = list[index];
                list.RemoveAt(index);
                set.Remove(itemToRemove);
                return true;
            }
            return false;
        }

        public bool Insert(int index, T item) {
            if (set.Contains(item)) {
                //If the item is already in the set, we just move it to the new index
                int oldIndex = list.FindIndex(x => x.Equals(item));
                if (oldIndex != index) {
                    list.Move(oldIndex, index);
                }
                return false; //No insertion happened
            } else {
                //New item, insert it
                list.Insert(index, item);
                set.Add(item);
                return true; //Insertion happened
            }
        }

        // Expose some methods from the internal List
        public T Find(Predicate<T> match) {
            return list.Find(match);
        }
        public int FindIndex(Predicate<T> match) {
            return list.FindIndex(match);
        }
        public void Sort() {
            list.Sort();
        }
        public IEnumerator<T> GetEnumerator() {
            return list.GetEnumerator();
        }

        public T this[int index] {
            get { return list[index]; }

            set {
                if (set.Contains(value)) {
                    //Then the list also contains the value

                    int oldIndex = list.FindIndex(x => x.Equals(value)); //this is where it is
                    if (index == oldIndex) {
                        //No operation, same index and same existing value.
                        return;
                    }

                    //Otherwise, move the existing element in the list to the new index.
                    list.Move(oldIndex, index);
                } else {
                    //New element
                    list[index] = value;
                    set.Add(value);
                }
            }
        }

        public int Count => list.Count;
    }

}
