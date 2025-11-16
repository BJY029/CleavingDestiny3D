using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BranchController : MonoBehaviourPunCallbacks
{
    private GameObject myBranch;
    private float myLength;
	private int myIndex = -1;

	private void Start()
	{
		myBranch = GetComponent<GameObject>();
	}
	
	public void setLength(float length)
	{
		myLength = length; 
		if(myBranch != null)
		{
			myBranch.transform.localScale = new Vector3(1, myLength, 1);
		}
	}

	public void setIndex(int index)
		{ myIndex = index; }


	public void OnClickMyStick()
	{
		if (myIndex == -1) return;
		if (!PhotonNetwork.InRoom) return;

		StickGameController.Instance.OnClickStick(myIndex);
	}

}
