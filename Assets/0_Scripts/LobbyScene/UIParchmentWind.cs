using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIParchmentWind : BaseMeshEffect
{
    [Header("분할 수")]
    [SerializeField, Range(2, 20)] private int columns = 6;
    [SerializeField, Range(2, 30)] private int rows = 12;

    [Header("바람 설정")]
    [SerializeField] private float swayAmount = 2f;
    [SerializeField] private float liftAmount = 3f;
    [SerializeField] private float speed = 1.8f;

    [Header("위쪽 고정 영역 비율")]
    [SerializeField, Range(0f, 0.9f)] private float pinnedTopRatio = 0.2f;

    private readonly UIVertex[] corners = new UIVertex[4];

    private void Update()
    {
        if (IsActive() && graphic != null)
            graphic.SetVerticesDirty();
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount != 4)
            return;

        // 기본 Simple Image의 정점 순서: 좌하, 좌상, 우상, 우하
        for (int i = 0; i < 4; i++)
            vh.PopulateUIVertex(ref corners[i], i);

        vh.Clear();

        float time = Application.isPlaying ? Time.unscaledTime * speed : 0f;

        for (int y = 0; y <= rows; y++)
        {
            float v = (float)y / rows;

            // 위쪽 고정 영역에서는 0, 아래쪽으로 갈수록 1
            float weight = 1f - Mathf.Clamp01(v / (1f - pinnedTopRatio));
            weight *= weight;

            for (int x = 0; x <= columns; x++)
            {
                float u = (float)x / columns;

                UIVertex vertex = corners[0];

                Vector3 bottom = Vector3.Lerp(corners[0].position, corners[3].position, u);
                Vector3 top = Vector3.Lerp(corners[1].position, corners[2].position, u);
                vertex.position = Vector3.Lerp(bottom, top, v);

                Vector4 bottomUV = Vector4.Lerp(corners[0].uv0, corners[3].uv0, u);
                Vector4 topUV = Vector4.Lerp(corners[1].uv0, corners[2].uv0, u);
                vertex.uv0 = Vector4.Lerp(bottomUV, topUV, v);

                Color bottomColor = Color.Lerp(corners[0].color, corners[3].color, u);
                Color topColor = Color.Lerp(corners[1].color, corners[2].color, u);
                vertex.color = Color.Lerp(bottomColor, topColor, v);

                float wave = time + u * 2.5f + v * 1.5f;

                vertex.position.x += Mathf.Sin(wave) * swayAmount * weight;
                vertex.position.y += (1f - Mathf.Cos(wave)) * 0.5f * liftAmount * weight;

                vh.AddVert(vertex);
            }
        }

        int stride = columns + 1;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                int index = y * stride + x;

                vh.AddTriangle(index, index + stride, index + 1);
                vh.AddTriangle(index + 1, index + stride, index + stride + 1);
            }
        }
    }
}
