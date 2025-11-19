using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject Camera;
    public GameObject SpawnSpot_P1;
	public GameObject SpawnSpot_P2;

    private PLAYER myRole;

	void Start()
    {
		//myRole = PhotonHelper.GetMyRole();
        //SpawnPlayerByRole();   
    }

    private void SpawnPlayerByRole()
    {
        switch(myRole)
        {
            case PLAYER.P1:
                Spawner_P1();
                break;
            case PLAYER.P2:
                Spawner_P2();
                break;
            default:
                Debug.LogError("NON ROLE ERROR!");
                break;
        }
    }

    private void Spawner_P1()
    {
        PhotonNetwork.Instantiate("Player/Player1", SpawnSpot_P1.transform.position, Quaternion.Euler(0f, 180f, 0f));
        Camera.SetActive(false);
    }

    private void Spawner_P2()
    {
		PhotonNetwork.Instantiate("Player/Player2", SpawnSpot_P2.transform.position, Quaternion.identity);
		Camera.SetActive(false);
	}
}
