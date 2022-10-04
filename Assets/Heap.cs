using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T: IHeapItem<T>
{
    T[] Items;
    int CurrentItemCount;

    public Heap(int MaxHeapSize)
    {
        Items = new T[MaxHeapSize];
    }

    public void Add(T Item)
    {
        Item.HeapIndex = CurrentItemCount;
        Items[CurrentItemCount] = Item;
        SortUp(Item);
        CurrentItemCount++;
    }

    public T RemoveFirst()
    {
        T FirstItem = Items[0];
        CurrentItemCount--;

        Items[0] = Items[CurrentItemCount];
        Items[0].HeapIndex = 0;
        
        SortDown(Items[0]);
        return FirstItem;
    }

    public bool Contains(T Item)
    {
        return Equals(Items[Item.HeapIndex], Item);
    }

    public void UpdateItem(T Item)
    {
        SortUp(Item);
    }

    public int Count
    {
        get{
            return CurrentItemCount;
        }
    }


    void SortDown(T Item)
    {
        while(true)
        {
            int ChildIndexL = Item.HeapIndex*2 +1;
            int ChildIndexR = Item.HeapIndex*2 +2;
            int SwapIndex = 0;

            if(ChildIndexL < CurrentItemCount)
            {
                SwapIndex = ChildIndexL;

                if(ChildIndexR < CurrentItemCount)
                {
                    if(Items[ChildIndexL].CompareTo(Items[ChildIndexR])<0)
                    {
                        SwapIndex = ChildIndexR;
                    }
                }

                if(Item.CompareTo(Items[SwapIndex])<0)
                {
                    Swap(Item, Items[SwapIndex]);
                }
                else{
                    return;
                }
            }
            else{
                return;
            }

            

        }
    }

    void SortUp(T Item)
    {
        int ParentIndex = (Item.HeapIndex -1)/2;

        while(true)
        {
            T ParentItem = Items[ParentIndex];
            if(Item.CompareTo(ParentItem) > 0)
            {
                Swap(Item, ParentItem);
            }
            else
            {
                break;
            }
        }
    }

    void Swap(T ItemA, T ItemB)
    {
        Items[ItemA.HeapIndex] = ItemB;
        Items[ItemB.HeapIndex] = ItemA;

        int ItemAIndex = ItemA.HeapIndex;
        ItemA.HeapIndex = ItemB.HeapIndex;
        ItemB.HeapIndex = ItemAIndex;
    }
}


public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex{
        get; set;
    }
}
