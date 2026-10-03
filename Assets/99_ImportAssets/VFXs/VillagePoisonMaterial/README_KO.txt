Unity 6 / URP 독구름 파티클 머티리얼

설치
1. ZIP을 풀고 VillagePoisonMaterial 폴더 전체를 프로젝트 Assets 안에 복사합니다.
2. .meta 파일과 shader도 함께 복사합니다. 텍스처와 셰이더 참조 유지에 필요합니다.
3. Particle System > Renderer > Material에 아래 머티리얼 중 하나를 넣습니다.

권장: M_SmokeTintable.mat
머티리얼 색은 흰색입니다. 기존 설명대로 Start Color를 #A6BC45, Alpha 0.55로 설정합니다.
Color over Lifetime의 RGB는 흰색으로 유지하고 Alpha만 조절합니다.

간편: M_VillagePoison.mat
머티리얼에 황록색이 적용되어 있습니다.
Start Color는 흰색, Alpha 0.55로 설정합니다. 녹색을 중복 적용하지 마세요.

Renderer: Billboard / Render Alignment: View / GPU Instancing 끄기
Texture Sheet Animation: 끄기. 단일 연기 이미지이며 스프라이트 시트가 아닙니다.

동봉 셰이더
VillageVFX/Poison Smoke URP: 투명 Alpha 블렌딩, 조명 영향 없음, 입자 색/알파 반영.
Tint: 기본 색. Opacity: 추가 투명도 배율.
소프트 파티클(깊이 기반 접촉 페이드), 씬 Fog, 프레임 혼합, XR용 기능은 포함하지 않습니다.
표준 Billboard Particle System의 Position, Color, UV 스트림을 사용합니다.

확인
PNG: 실제 RGBA 투명도와 투명한 네 귀퉁이 확인.
머티리얼-셰이더-텍스처 GUID 참조와 ZIP 무결성 확인.
이 환경에 Unity Editor가 없어 실제 셰이더 컴파일 및 플레이 화면 검증은 하지 못했습니다.

텍스처 제작 사양
이미지 생성으로 제작한 단일 회백색 연기 덩어리. 부드러운 투명 외곽,
큰 불규칙한 덩어리와 내부 명암, 글자/배경/녹색 색조 없음.
