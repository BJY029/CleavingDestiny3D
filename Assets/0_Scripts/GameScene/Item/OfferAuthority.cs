using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using System;
using ExitGames.Client.Photon;
public class OfferAuthority : MonoBehaviourPunCallbacks
{
	public static OfferAuthority Instance;

	private void Awake()
	{
		if(Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}


	//각 턴에 호출될 아이템 3개 뽑기 함수
	public string MakeOfferForTurn(int turnActor, int turnIndex)
	{
		//마스터 클라이언트만 실행
		if (!PhotonNetwork.IsMasterClient) return "";

		Player player = PhotonNetwork.CurrentRoom.GetPlayer(turnActor);
		if (player == null) return "";

		//아이템 리스트 받아오기
		List<ItemSO> items = ItemDB.Instance.GetItemsList();
		Debug.Log("items length : " + items.Count);
		//ROOM_SEED를 가져오기
	    int roomSeed = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.RoomSeed);


		//아이템 3개 뽑아서 문자열 리스트 받아오기
		List<string> offer = Pick3(player, turnIndex, turnActor, roomSeed, items);

		//해당 문자열을 직렬화하기
		return string.Join("|", offer); // "potion|bomb|shield"
		//OFFER_{actor}의 이름으르 Room 프로퍼티에 해당 제안 아이템 저장하기
		//PhotonPropertyHelper.SetRoomProp(ItemPropKeys.OFFER(actor), offerStr);
		//Debug.Log($"Offer : {offerStr}, currentActorNumber : {actor}");
	}

	//MasterClient만 수행
	//Efraimidis-Spirakis 알고리즘
	public List<string> Pick3(Player player, int turnIndex, int actor, int roomSeed ,List<ItemSO> items)
    {
        //결정론적 난수 생성
        //turnIndex와 actor가 같으면 재현 가능한 난수 생성기
        var rng = new System.Random(roomSeed ^ (turnIndex * 73856093) ^ (actor * 19349663) ^ 12345);

        //각 플레이어의 아이템 등장 확률을 프로퍼티에서 불러와 딕셔너리에 저장
		Dictionary<ItemClass, float> RarityWeight = new()
		{
			{ItemClass.Common, PhotonPropertyHelper.GetPlayerProp<float>(player, PlayerPropKeys.Item_CommonWeight) },
			{ItemClass.Hero, PhotonPropertyHelper.GetPlayerProp<float>(player, PlayerPropKeys.Item_HeroWeight) },
			{ItemClass.Rare, PhotonPropertyHelper.GetPlayerProp<float>(player, PlayerPropKeys.Item_RareWeight) },
			{ItemClass.Legendary, PhotonPropertyHelper.GetPlayerProp<float>(player, PlayerPropKeys.Item_LegendaryWeight) },
		};

		//각 아이템에 점수(key) 부여 알고리즘
        //점수를 저장할 리스트 선언
		var scored = new List<(double key, string itemId)>(items.Count);
        //각 아이템을 돌면서
        foreach(var it in items)
        {
            //만약 아이템이 빈 경우 제외
            if(string.IsNullOrEmpty(it.itemId)) continue;
			// 등급 가중치 0이면 그 등급은 절대 안 나오게 제외
			if (!RarityWeight.TryGetValue(it.itemClass, out float rw) || rw <= 0f) continue;

			// 아이템 가중치도 0이면 제외
			if (it.itemWeight <= 0f) continue;

			double w = rw * it.itemWeight;

			//핵심!
			//(0,1] 범위로 난수 값 조정
			double u = 1.0 - rng.NextDouble();
            //각 아이템 키 값 계산
                //아이템의 가중치가 클 수록 결과값이 작아진다.
                //결과값이 작을수록 아이템이 상위권에 뽑히게 된다.
            double key = -Math.Log(u) / w;

            //계산한 점수를 저장한다.
            scored.Add((key, it.itemId));

            //알고리즘 설명
            // Key = (-ln(Random)) / (Weight)
            // 달리기 기록 = (트랙의 길이(랜덤) / (선수의 속도(가중치))
                //즉, 일반적으로는 Common item의 가중치가 크지만 운 나쁘게 큰 수의 랜덤 값이 나올 수 있고
                //반대로 Legendary item의 가중치가 작지만, 운 좋게 작은 수의 랜덤 값이 나올 수 있다.
        }

        //3개의 아이템을 고르는 LINQ문
        var picked = scored
            .GroupBy(s => s.itemId)                     //같은 아이템 ID끼리 묶기(중복 아이템 제거)
            .Select(g => g.OrderBy(x => x.key).First()) //같은 ID 아이템 중 가장 점수가 작은 아이템 하나만 남김
            .OrderBy(x => x.key)                        //전체 아이템을 점수 순으로 정렬
            .Take(CommonDefine.itemOfferCnt)                                    //그 중 상위 3등 까지 뽑기
            .Select(x => x.itemId)                      //해당 3 아이템의 id 가져오기
            .ToList();                                  //리스트로 만들기

        while (picked.Count < CommonDefine.itemOfferCnt) picked.Add("Error");   //만약 빈 요소가 있으면 에러

        return picked; //뽑힌 아이템 리스트 반환
    }
}
