using UnityEngine;

/// <summary>
/// 다중 씬이 로드되었을 때 이 컴포넌트가 붙어있는 게임 오브젝트를 비활성화 시키는 스크립트
/// 카메라, EventSystem 등이 중복으로 존재하는 것을 방지하기 위함
/// </summary>
public class SceneAutoCleaner : MonoBehaviour
{
    void Awake()
    {
        // 씬이 2개 이상 로드되어 있을 때 이 게임 오브젝트 비활성화
        if (UnityEngine.SceneManagement.SceneManager.sceneCount > 1)
        {
            gameObject.SetActive(false);
        }
    }
}