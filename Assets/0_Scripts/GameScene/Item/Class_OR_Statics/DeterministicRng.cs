using System;

//랜덤 데미지를 결정해야 할 때, 이를 Master가 대표로 랜덤 함수를 통해 랜덤 함수를 얻는다.
//Room Seed 값을 활용
public class DeterministicRng
{
    private readonly Random _random;

    //랜덤 값 발생기
    public DeterministicRng(int seed)
    {
        _random = new Random(seed);
    }

    /// <summary>
    /// Unity의 Random.Range(min, max)와 동일한 규칙:
    /// [min, max)
    /// </summary>
    public int Range(int minInclusive, int maxExclusive)
    {
        return _random.Next(minInclusive, maxExclusive);
    }
}
