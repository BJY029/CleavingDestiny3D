using UnityEngine;
using System.Linq;

public static class ItemInfoSerializer
{
    //인벤토리 내 아이템 문자열로 직렬화
    //형식 예시) 12:potion|13:bomb|_|21:potion|_"
    //인자는 해당 아이템의 고유 id와 해당 아이템의 id의 튜플 형태
    public static string Encode((int uniqueId, string itemId)[] slots)
    {
        //직렬화 될 각 문자열 부분 구성
        string[] parts = new string[slots.Length];
        for(int i = 0; i < slots.Length; i++)
        {
            //고유 id가 유효하고, 아이템 정보가 존재하는 경우, 해당 아이템 고유 id와 아이템 id로 구성
            //그렇지 않으면 빈칸 (_)로 설정
            parts[i] = (slots[i].uniqueId > 0 && !string.IsNullOrEmpty(slots[i].itemId)) ? $"{slots[i].uniqueId}:{slots[i].itemId}" : "_";
        }
        //다음 문자열로 변환하여 반환
        return string.Join("|", parts);
    }

    //문자열로 구성된 인벤토리 데이터를 다시 역직렬화
    public static(int uniqueId, string itemID)[] Decode(string data, int capacity)
    {
        //튜플 객체 정의
        var res = new(int uniqueId, string itemId)[capacity];
        //빈 데이터인 경우 빈 튜플 객체 반환
        if (string.IsNullOrEmpty(data)) return res;

        //문자열로 구성된 데이터를 | 로 나눠서 배열로 저장
        var parts = data.Split('|');
        for(int i = 0; i < capacity; i++)
        {
            //만약 해당 데이터 부분이 빈 경우
            if (parts[i] == "_" || string.IsNullOrEmpty(parts[i])) continue;

            //해당 데이터를 또 다시 ":"로 나눠서 저장
            var p = parts[i].Split(":");
            //두 개의 데이터가 존재하고(uniqueId, itemid), uniqueid를 int로 변환이 가능하면
			if (p.Length == 2 && int.TryParse(p[0], out int uniqueId))
			{
                //해당 정보를 res 객체에 저장
                res[i] = (uniqueId, p[1]);
			}
		}
        //역직렬화 된 아이템 인벤토리 객체 반환
        return res;
    }

    //아이템 슬롯의 첫 번째 빈 구역에 아이템을 삽입하는 함수
    //인자로 아이템 슬롯 정보와, 삽입할 아이템 정보를 넘긴다.
    public static bool TryAddFirstEmpty((int uniqueId, string itemId)[] slots, (int uniqueId, string itemID) inst)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            //특정 슬롯의 uniqueId가 0보다 작으면, 즉 빈 구간인 경우
            if (slots[i].uniqueId <= 0)
            {
                //해당 슬롯에 아이템 정보 삽입
                slots[i] = inst;
                return true;
            }
        }
        //삽입 실패(꽉참)
        return false;
    }

    public static bool isFullInventory((int uniqueId, string itemId)[] slots)
    {
		for (int i = 0; i < slots.Length; i++)
		{
			//특정 슬롯의 uniqueId가 0보다 작으면, 즉 빈 구간인 경우
			if (slots[i].uniqueId <= 0)
			{
				return false;
			}
		}
		//삽입 실패(꽉참)
		return true;
	}

	//주어진 아이템 슬롯에서, uniqueId를 기반으로 아이템을 삭제하고 삭제된 아이템 이름을 반환(out)하는 함수
	public static int TryFindIndexByUniqueId((int uniqueId, string itemId)[] slots, int uniqueId)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].uniqueId == uniqueId)
            {
                //slots[i] = (0, null);
                return i;
            }
        }
        return -1;
    }

    public static string MakeEmptyInv(int cap)
    {
        return string.Join("|", Enumerable.Repeat("_", cap));
    }

    public static string AddInvSpace(string slotInfo, int oldCap, int newCap)
    {
        int cap = newCap -oldCap;
        string newInfo = string.Join("|", Enumerable.Repeat("_", cap));
        return slotInfo + "|" + newInfo;
    }
}
