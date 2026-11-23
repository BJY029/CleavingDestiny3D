# 마을 종료 후 기력 회복 이펙트 확인

## 원인과 수정

`PlayerStatus.SetPlayerStatusUIVFX()`가 기력 증가량을 계산할 때, 계산용 캐시 `currentEnergy`를 이전 기력으로 사용하고 있었다. 그러나 하루 초기화의 `ApplyInitStatus()`는 속성을 전송하기 전에 이 캐시를 회복 후 값으로 바꾸며, AI 상태 조회도 같은 캐시를 덮어쓴다. 그 결과 회복량을 0으로 판단해 효과를 놓치거나 후속 UI 갱신에서 잘못 판단할 수 있었다.

이전 표시 기력(`lastPresentedEnergy`)을 계산용 캐시와 분리했다. UI를 갱신할 때 실제 이전 표시와 비교하고, 양의 증가량에만 기존 EnergyUp RPC를 보낸다. 첫 상태 표시와 같은 값의 반복 갱신은 회복 이펙트를 만들지 않는다. 회복량, 마을 복귀 페이드와 턴 시작 순서, 이펙트 외형은 변경하지 않았다.

EnergyUp 프리팹의 ParticleSystem 4개 모두 시작 지연은 0초였다.

## Editor 재현 및 결과

오프라인 AI 경기에서 기존 `FinishDayForVillageTest()`로 마을 진입을 단축하고, 해당 경기의 플레이어 기력을 0으로 설정한 뒤 정상 준비 완료 처리로 마을을 종료했다. 저장 재화·보상은 조작하지 않았다. 타이밍 관찰은 Editor update에서 실제 기력 속성, UI 표시, 로컬 EnergyUp 인스턴스의 재생 상태를 기록했다.

- 수정 전: 66.067초, frame 6210에서 기력 0→5 및 UI 갱신. 이후 관찰한 2초 동안 로컬 EnergyUp 재생 없음.
- 수정 후: 57.502초, **frame 4676에서 기력 0→6, UI 갱신, EnergyUp 재생 시작이 함께 관찰됨**. 경기별 회복량은 각 경기의 실제 설정을 사용했다.
- 수정 후 AI 상태를 읽고 UI를 두 번 추가 갱신해도 EnergyUp 재생은 1회였다.
- 기력을 0으로 낮춘 상태 변경에서는 회복 효과가 재생되지 않았다.
- Unity 컴파일 오류 없음. Windows 실행 파일 재빌드는 수행하지 않았다. 실제 온라인 두 클라이언트의 통신 지연이나 물리 키보드 입력을 검증한 결과는 아니다.

[수정 전 기록](QA_EVIDENCE/energy-vfx-before-2026-11-11.txt) · [수정 후 기록](QA_EVIDENCE/energy-vfx-after-2026-11-11.txt)

## 별도 관찰

기력 이펙트 수정 전·후 모두 AI 마을 강화에서 `AIBrain.OnRoomPropertiesUpdate():164`의 `InvalidCastException`을 관찰했다. 보호막 수치를 직접 float로 캐스팅하는 경로로, 기력 이펙트 변경과는 별개였다. 이후 아래 후속 작업에서 수정했다.

## 후속: AI 마을 강화 자료형 오류 수정

- `AIBrain`의 기력·최대 기력·보호막·방어력 변경 알림과 AI 상태 조회를 기존 `PhotonPropertyHelper.GetHashtableValue<T>()`로 통일했다. 정수로 저장된 방어력도 float로 안전하게 읽는다. 강화 가격과 적용 수치는 변경하지 않았다.
- Unity 컴파일 오류 없음. 실제 공통 변환 함수에서 int/float/double/short → float 및 누락 값의 기본값 처리 5개 검증을 통과했다.
- 오프라인 AI 경기에서 기존 마을 전환 테스트 도구를 사용했다. 재화 주입 없이 AI가 Mine → Mine → Barrier → Forge를 강화했다. 골드 500→400→200→100→0, 방어력 0→100, 준비 상태 false→true를 확인했다.
- 방어력 알림에 int/float/double 값을 각각 전달한 실제 `AIBrain.OnRoomPropertiesUpdate()` 호출도 예외 없이 완료했다. 정상 마을 종료 후 다음 날(day 2) 진입을 확인했다. `InvalidCastException` 재발 없음.
- 로비 오브젝트 파괴 시 `ShopUIManager.OnDestroy()`의 별도 KeyInteractManager 부재 로그는 관찰됐으며, 이번 AI 수치 변환 수정에 포함하지 않았다.
- [AI 강화 재검증 기록](QA_EVIDENCE/ai-upgrade-fixed-2026-11-11.txt). 이번 수정은 Editor에서 검증했으며 실행 파일 재빌드는 수행하지 않았다.

테스트를 종료하고 Editor를 GameScene으로 복귀했다. 기존 사용자 변경은 보존했다.
