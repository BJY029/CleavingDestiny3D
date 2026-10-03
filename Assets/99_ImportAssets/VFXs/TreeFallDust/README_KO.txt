나무 착지 흙먼지 Material / Unity 6 URP

설치
1. ZIP을 압축 해제합니다.
2. TreeFallDust 폴더 전체를 Unity Project 창의 Assets 아래로 복사합니다.
   .meta 파일도 함께 복사해야 Material의 텍스처/셰이더 연결이 유지됩니다.
3. Particle System > Renderer > Material에 TreeDust.mat을 드래그합니다.
4. Renderer > Render Mode = Billboard.
5. Main > Start Color = #B6A184, Alpha = 약 0.5.
6. Color over Lifetime에서 Alpha를 0 → 1 → 0으로 설정합니다.

파일
DustCloud.png: 흰색 투명 먼지 텍스처. 파티클 Start Color로 흙색을 지정합니다.
TreeDust.mat: 텍스처가 연결된 투명 Material.
TreeDust.shader: URP용 Unlit 셰이더. 파티클 Vertex Color와 Alpha를 반영합니다.

참고
Material Tint는 기본 흰색입니다. 흙색은 Start Color에서 지정하면 중복 색상 곱셈을 피할 수 있습니다.
Custom Vertex Streams를 변경했다면 Position, Color, UV 채널을 포함하세요.
이 패키지는 Material과 텍스처이며, Particle System 프리팹은 포함하지 않습니다.
Soft Particles/깊이 페이드는 포함하지 않습니다.
Unity Editor가 없는 환경에서 구성한 파일로, Unity 프로젝트 내 컴파일/렌더 테스트는 수행하지 못했습니다.

기술 참고
https://docs.unity3d.com/kr/current/Manual/urp/use-built-in-shader-methods-transformations.html
