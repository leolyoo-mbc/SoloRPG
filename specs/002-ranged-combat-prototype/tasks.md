# Tasks: 원거리 교전 프로토타입

**Input** — [명세](spec.md), [구현 계획](plan.md), [설계 근거](research.md), [데이터 모델](data-model.md), [조작 및 화면 계약](contracts/encounter-controls.md), [Play Mode 검증](quickstart.md)

## Format

각 작업은 체크박스와 순차 ID를 사용한다. `[P]`는 선행 작업이 끝난 뒤 다른 파일에서 독립적으로 진행할 수 있는 작업을 표시하고, `[USn]`은 해당 사용자 이야기를 표시한다. 코드 경로는 프로젝트 루트 기준이다.

## Phase 1: Setup

- [X] T001 `Assets/_SoloRPG/Scenes/MovementPrototype.unity`를 `Assets/_SoloRPG/Scenes/RangedCombatPrototype.unity`로 복제한다. 기존 이동과 바닥 참조를 재사용하고, 시작 카메라에서 보이는 고정 적 한 명과 플레이어를 계획의 시작 배치에 둔다. [데이터 모델](data-model.md)의 `교전.시작 위치와 시선`, `플레이어.캐릭터 콜라이더`, `상대.고정 위치`, `상대.콜라이더`를 적용한다. (FR-001)

## Phase 2: Foundational

- [X] T002 [P] `Assets/_SoloRPG/Scripts/Prototype/PrototypeFollowCamera.cs`에 마우스 조작 가능 여부와 해당 프레임에 새로 마우스를 확보했는지를 읽는 속성을 제공한다. 기존 카메라 입력과 추적 순서를 보존하고 기존 이동 씬에서도 클릭, Esc, 포커스 이탈 처리가 계속 작동하게 한다.
- [X] T003 [P] `Assets/_SoloRPG/Scripts/Prototype/PrototypeEncounterHud.cs`에 조준점, 양쪽 체력, 안내 및 결과를 그리는 uGUI 표시 컴포넌트를 만든다. 표시 값은 호출자가 전달하도록 구성한다. (FR-002, FR-006, FR-007)
- [X] T004 `Assets/_SoloRPG/Scripts/Prototype/PrototypeRangedEncounter.cs`에서 [데이터 모델](data-model.md)의 `교전.상태`, `플레이어.현재 체력`, `상대.현재 체력`을 초기화한다. 시작 대기에서 첫 클릭을 소비하며 진행 상태로 전환한다. Esc와 포커스 이탈 시 이동, 사격, 탄 및 시간 진행을 정지하고 재확보 클릭은 소비한다. `Assets/_SoloRPG/Scenes/RangedCombatPrototype.unity`에 컨트롤러와 HUD를 연결한다. (FR-002, FR-003, FR-004, FR-006)

## Phase 3: User Story 1 — 움직이며 조준하고 맞히기 (P1)

**독립 검증** — 적의 공격 기능이 없어도, 이동 중 적을 겨냥해 왼쪽 클릭 한 번으로 체력이 1 줄고 조준점이 벗어나면 줄지 않는지 확인할 수 있다.

- [X] T005 [US1] `Assets/_SoloRPG/Scripts/Prototype/PrototypeRangedEncounter.cs`와 `Assets/_SoloRPG/Scenes/RangedCombatPrototype.unity`에서 WASD 이동과 마우스 조준 중 왼쪽 클릭 한 번에 즉시 발사하도록 연결한다. 카메라 추적 뒤 중앙 조준선의 첫 유효 충돌을 확인하고 자기 콜라이더를 제외한다. [데이터 모델](data-model.md)의 `상대.현재 체력`, `상대.예고 및 명중 표시 상태`에 따라 명중 시 체력과 본체 색을 갱신한다. 시작 및 재개 클릭으로는 발사하지 않는다. 클릭을 유지해도 추가 발사가 없으며 빗나간 발은 피해를 주지 않는다. (FR-002, FR-003, SC-001)

## Phase 4: User Story 2 — 상대의 탄 피하기 (P1)

**독립 검증** — 플레이어가 쏘지 않아도 클릭 전 대기, 첫 발사 예고, 제자리에 서서 첫 발 맞기, 새 판에서 발사 후 옆으로 피하기를 확인할 수 있다.

- [X] T006 [P] [US2] `Assets/_SoloRPG/Scripts/Prototype/PrototypeEncounterProjectile.cs`에 [데이터 모델](data-model.md)의 `적 탄.발사 위치`, `적 탄.방향`, `적 탄.남은 수명`을 적용한다. 움직이는 탄을 표시하고 프레임 사이 이동 경로에서 플레이어와 바닥의 충돌을 검사해 탄마다 한 번만 충돌 결과를 반환하고 제거한다. (FR-004, FR-005)
- [X] T007 [P] [US2] `Assets/_SoloRPG/Scenes/RangedCombatPrototype.unity`에 적의 발사 위치와 본체 명중 색과 구분되는 발사 예고 표식을 배치한다.
- [X] T008 [US2] `Assets/_SoloRPG/Scripts/Prototype/PrototypeRangedEncounter.cs`와 `Assets/_SoloRPG/Scenes/RangedCombatPrototype.unity`에서 첫 클릭부터 적의 일정, 예고, 발사 및 피격 흐름을 연결한다. [데이터 모델](data-model.md)의 `교전.다음 적 발사 시각`, `교전.소유 탄 목록`, `상대.예고 및 명중 표시 상태`, `플레이어.현재 체력`을 적용한다. 발사 방향은 그 순간의 플레이어 콜라이더 중심을 기준으로 하고 피해는 한 번만 반영한다. (FR-004, FR-005, SC-002)

## Phase 5: User Story 3 — 한 판을 끝내고 다시 시작하기 (P2)

**독립 검증** — 양쪽의 체력이 각각 먼저 0이 되는 상황을 만들고 결과와 추가 피해 차단을 확인한다. 각 결과에서 R을 눌러 새 판의 초기 배치와 체력을 확인한다.

- [X] T009 [P] [US3] `Assets/_SoloRPG/Scripts/Prototype/PrototypeFollowCamera.cs`에 시작 yaw와 pitch 및 커서 해제 상태를 복원하는 메서드를 추가한다. [데이터 모델](data-model.md)의 `교전.시작 위치와 시선` 가운데 시선을 복원하며 기존 이동 씬에서는 별도 초기화 호출 없이 작동하게 한다.
- [X] T010 [P] [US3] `Assets/_SoloRPG/Scripts/Prototype/PrototypeRangedEncounter.cs`에서 [데이터 모델](data-model.md)의 `교전.상태`, `교전.소유 탄 목록`, `플레이어.현재 체력`, `상대.현재 체력`을 사용해 승패와 남은 체력, R 안내를 표시한다. 같은 프레임에 두 타격이 겹치면 플레이어 발사가 우선한다. 결과가 나면 즉시 남은 탄을 제거하고 공격과 피해를 차단한다. (FR-006, SC-003)
- [X] T011 [US3] `Assets/_SoloRPG/Scripts/Prototype/PrototypeRangedEncounter.cs`에서 결과 화면의 R 입력으로 다음 판을 시작하도록 연결한다. [데이터 모델](data-model.md)의 `교전.상태`, `교전.시작 위치와 시선`, `교전.다음 적 발사 시각`, `교전.소유 탄 목록`, `플레이어.현재 체력`, `상대.현재 체력`, `상대.예고 및 명중 표시 상태`를 초기화한 뒤 새 마우스 확보를 기다린다. (FR-007, SC-004)

## Final Phase: Polish & Cross-Cutting Concerns

- [X] T012 `specs/002-ranged-combat-prototype/quickstart.md`의 Play Mode 절차를 `Assets/_SoloRPG/Scenes/RangedCombatPrototype.unity`와 `Assets/_SoloRPG/Scenes/MovementPrototype.unity`에서 실행한다. 같은 프레임 이동·조준·사격, 예고와 명중 표시 중첩, Esc와 포커스 이탈 후 재개, 양쪽 승패와 재시작, 기존 이동 회귀를 확인하고 관찰 결과와 미검증 항목을 보고한다.

## Dependencies & Execution Order

- T001 → T002와 T003 → T004. 세 이야기의 구현은 공통 씬과 컨트롤러가 연결된 뒤 진행한다.
- T005 이후 T006과 T007을 병행할 수 있다. T008은 두 작업의 결과를 씬과 교전 컨트롤러에 연결한다.
- T009와 T010은 선행 교전 작업을 마친 뒤 동시에 진행할 수 있다. T011은 두 작업을 연결하고 T012는 통합 결과를 검증한다.

## Parallel Examples

- **US1** — HUD 작성(T003)과 카메라 조작 상태 제공(T002)을 병행한 뒤 중앙 조준과 발사(T005)를 연결한다.
- **US2** — 탄 이동과 충돌(T006), 씬의 예고 표식(T007)을 병행한다.
- **US3** — 카메라 복원(T009)과 교전 종료 처리(T010)를 병행한다.

## Implementation Strategy

1. **첫 플레이 확인** — US1까지 구현해 이동하면서 조준하고 명중하는지 확인한다.
2. **원거리 교전 확인** — US2에서 상대의 예고와 탄 회피를 연결한다. US3에서 한 판의 승패와 재시작을 묶는다.
3. **통합 확인** — 두 씬을 실행해 명세의 성공 기준과 기존 이동을 검증한다.
