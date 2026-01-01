using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class InventorySlot
{
    public ItemSO item;
    public bool IsEmpty => item == null;

    public void Clear()
    {
        item = null;
    }
}

public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity = 8;
    public List<InventorySlot> Slots { get; private set; }
     
    //UI 갱신용
    public event Action OnChanged;

	public void Awake()
	{
        //동적 할당 및 초기화
	    Slots = new List<InventorySlot>(capacity);
        for (int i = 0; i < capacity; i++) Slots.Add(new InventorySlot());
	}

    //인벤토리에 아이템 추가 하는 함수
    public bool Add(ItemSO item)
    {
        if (item == null) return false;

        //빈 자리 찾기
        for(int i = 0; i < Slots.Count;i++)
        {
            var s = Slots[i];
            if(s.IsEmpty)
            {
                //해당 슬롯에 아이템 삽입
                s.item = item;

                //관련 UI 처리 수행
                OnChanged?.Invoke();
                return true;
            }
        }
        //빈 자리가 없는 경우
        OnChanged?.Invoke();
        return false;
    }

    //인벤토리에 아이템 제거 하는 함수
    public bool Remove(ItemSO item)
    {
        if(item == null) return false;

        //실제 해당 아이템이 있는지 확인
        int totalAmount = 0;
        foreach(var s in Slots)
        {
            if (!s.IsEmpty && s.item == item) totalAmount += 1;
        }
        if (totalAmount < 0) return false;

        for(int i = 0; i < Slots.Count; i++)
        {
            var s = Slots[i];
            //빈 슬롯이거나 아이템이 일치하지 않는 경우
            if (s.IsEmpty || s.item != item) continue;

            //일치하는 아이템 찾은 경우 제거
            s.Clear();
            break;
        }

        OnChanged?.Invoke();
        return true;
    }
}
