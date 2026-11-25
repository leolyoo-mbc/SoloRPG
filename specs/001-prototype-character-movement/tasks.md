---
description: "임시 플레이어 캐릭터의 3인칭 시점 변경 작업"
---

# Tasks: 임시 플레이어 캐릭터 이동

**설계 자료** [명세](spec.md) · [계획](plan.md) · [설계 결정](research.md) · [상태 모델](data-model.md) · [조작 계약](contracts/movement-controls.md) · [검증 절차](quickstart.md)

## Phase 1: Setup

- [ ] T001 `ProjectSettings/ProjectVersion.txt`와 `Packages/manifest.json`의 Unity 및 패키지 버전으로 `Assets/_SoloRPG/Scenes/MovementPrototype.unity`를 열고 기존 캐릭터·바닥·카메라 및 컴포넌트 참조를 확인한다.

## Phase 2: Foundational

- [ ] T002 `Assets/InputSystem_Actions.inputactions`에서 `Player/Move`와 `Player/Look`의 바인딩을 확인한다. 마우스 델타와 오른쪽 스틱을 구분할 활성 컨트롤의 기기와 기존 Move 액션의 수명 주기를 확인한다. [시선 입력.마우스 델타](data-model.md), [시선 입력.오른쪽 스틱 값](data-model.md)

## Phase 3: User Story 1 - 임시 캐릭터 움직이기 (P1, MVP)

**목표** 캐릭터의 등 뒤 가까이에서 시선을 돌리고, 화면 기준으로 이동한다.

**독립 검증** 전용 씬에서 제자리로 멈춘 채 옆 표식을 바라본다. 앞으로 움직여 그 표식에 가까워지고, 화면 기준으로 좌우 이동한 뒤 멈췄다가 다시 움직인다. 시작과 바닥 안쪽에서 기본 상하 각도로 이동할 때 등과 앞쪽 바닥이 보이고, 가장자리와 상하 각도 한계에서도 캐릭터가 화면에 남는지 키보드·마우스와 게임패드로 각각 확인한다.

- [ ] T003 [P] [US1] `Assets/_SoloRPG/Scripts/Prototype/PrototypeFollowCamera.cs`에서 기존 Look 액션을 복제하고 수명 주기를 관리한다. 계획의 기기별 감도와 상하 각도 범위를 적용하고, Game 뷰 클릭으로 마우스를 잠그며 Esc·포커스 상실·비활성화 시 해제한다. 게임패드는 커서 잠금과 독립적으로 처리한다. 시선 갱신을 이동보다 먼저 실행하고, 이동 컴포넌트에 수평 회전을 제공한다. 시작 방향을 대상에서 읽고 계획의 어깨 오프셋으로 배치한 뒤 LateUpdate에서 이동한 대상을 따라간다. [시선 입력.마우스 델타](data-model.md), [시선 입력.오른쪽 스틱 값](data-model.md), [시선 입력.커서 잠금 상태](data-model.md), [3인칭 카메라.추적 대상](data-model.md), [3인칭 카메라.수평 각도](data-model.md), [3인칭 카메라.상하 각도](data-model.md), [3인칭 카메라.대상 기준 거리](data-model.md), [3인칭 카메라.오른쪽 오프셋](data-model.md) (FR-004, FR-005)
- [ ] T004 [P] [US1] `Assets/_SoloRPG/Scenes/MovementPrototype.unity`에서 기존 캡슐의 등 쪽에 앞뒤를 구별할 수 있는 작은 시각 표식을 자식으로 배치한다. 표식에는 콜라이더를 붙이지 않는다. [임시 캐릭터.시작 위치](data-model.md), [테스트 공간.20×20 바닥과 위치 확인용 표식](data-model.md) (FR-001, FR-004)
- [ ] T005 [US1] `Assets/_SoloRPG/Scripts/Prototype/PrototypeCharacterMovement.cs`에 카메라 참조를 추가한다. 이동 입력이 없을 때도 캐릭터가 카메라 수평 방향을 향하게 하고, 같은 프레임의 수평 회전으로 기존 Move 벡터를 변환한다. 기존 속도, 입력 크기 제한, 정지·재이동, CharacterController 및 바닥 경계 처리를 유지한다. [임시 캐릭터.현재 위치](data-model.md), [임시 캐릭터.수평 방향](data-model.md), [임시 캐릭터.이동 입력](data-model.md), [임시 캐릭터.이동 속도](data-model.md), [테스트 공간.바닥 경계](data-model.md) (FR-002, FR-003, FR-005)
- [ ] T006 [US1] `Assets/_SoloRPG/Scenes/MovementPrototype.unity`에서 카메라에 기존 입력 에셋과 추적 대상을, 이동 컴포넌트에 카메라를 연결한다. 편집 모드의 시작 구도를 계획에 맞추고 Play Mode에서 누락된 참조 없이 등 표식과 앞쪽 공간이 함께 보이는지 확인한다. [3인칭 카메라.추적 대상](data-model.md) (FR-001, FR-004)

## Phase 4: 최종 검증

- [ ] T007 `specs/001-prototype-character-movement/quickstart.md`에 따라 Unity Play Mode에서 두 조작 기기의 시선·이동, 같은 프레임의 방향 갱신, 정지·재이동, 기본 구도와 가장자리에서 상하 각도 한계의 캐릭터 가시성, 바닥 경계, 커서 해제·재잠금과 재실행 초기화를 확인한다. (SC-001~SC-004)

## Dependencies & Execution Order

`T001 → T002 → (T003 ∥ T004), T003 → T005, (T004 + T005) → T006 → T007`

US1은 독립적으로 검증할 수 있다. T003은 카메라 스크립트에서 시선과 추적을 구현하고 T004는 씬에 등 표식을 배치하므로 함께 진행할 수 있다. T005는 T003의 수평 회전이 필요하고, 씬 연결 T006은 T004와 T005가 끝난 뒤 진행한다.

## Implementation Strategy

기존 이동 씬과 바닥 경계를 활용해 US1을 완성한다. 카메라 구도, 시선 조작과 화면 기준 이동을 같은 씬에서 확인하면 이번 MVP가 완성된다.
