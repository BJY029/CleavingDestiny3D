using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[AddComponentMenu("UI/Effects/Gradient")]
public class UIGradient : BaseMeshEffect
{
	[Header("Gradient Settings")]
	public Color color1 = Color.white;
	public Color color2 = Color.red;
	[Range(-180f, 180f)]
	public float angle = 0f;
	public bool ignoreRatio = true;

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive())
			return;

		List<UIVertex> vertexList = new List<UIVertex>();
		vh.GetUIVertexStream(vertexList);

		int count = vertexList.Count;
		if (count == 0) return;

		// 경계 계산
		float bottom = vertexList[0].position.y;
		float top = bottom;
		float left = vertexList[0].position.x;
		float right = left;

		for (int i = 1; i < count; i++)
		{
			float y = vertexList[i].position.y;
			if (y > top) top = y;
			else if (y < bottom) bottom = y;

			float x = vertexList[i].position.x;
			if (x > right) right = x;
			else if (x < left) left = x;
		}

		float uiWidth = right - left;
		float uiHeight = top - bottom;

		// 각도에 따른 방향 벡터 계산
		float rad = angle * Mathf.Deg2Rad;
		float cos = Mathf.Cos(rad);
		float sin = Mathf.Sin(rad);

		for (int i = 0; i < count; i++)
		{
			UIVertex uiVertex = vertexList[i];

			// 정규화된 위치 계산 (0 ~ 1)
			float x = (uiVertex.position.x - left) / uiWidth;
			float y = (uiVertex.position.y - bottom) / uiHeight;

			// 회전 적용하여 그라데이션 위치 결정
			float t = (x - 0.5f) * cos + (y - 0.5f) * sin + 0.5f;

			if (!ignoreRatio && uiWidth > 0 && uiHeight > 0)
			{
				// 비율 보정이 필요할 경우의 로직 (단순화함)
			}

			t = Mathf.Clamp01(t);

			// 색상 보간 (기존 색상 * 그라데이션 색상)
			uiVertex.color = Color.Lerp(color1, color2, t) * uiVertex.color;
			vertexList[i] = uiVertex;
		}

		vh.Clear();
		vh.AddUIVertexTriangleStream(vertexList);
	}
}