using Unity.VisualScripting;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
	public Transform target;
	public float rotateSpeed = 30f;

	private void LateUpdate()
	{
		transform.RotateAround(target.position, Vector3.up, rotateSpeed *  Time.deltaTime);	
	}
}
