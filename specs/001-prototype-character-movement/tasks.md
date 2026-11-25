---
description: "임시 플레이어 캐릭터 이동 구현 작업"
---

# Tasks: 임시 플레이어 캐릭터 이동

**설계 자료** [명세](spec.md) · [계획](plan.md) · [설계 결정](research.md) · [상태 모델](data-model.md) · [조작과 시야 계약](contracts/movement-controls.md) · [검증 절차](quickstart.md)

## Phase 1: Setup

- [X] T001 `ProjectSettings/ProjectVersion.txt`에 지정된 Unity 6000.3.26f1에서 기존 프로젝트를 열고 `Packages/manifest.json`의 Input System 및 URP가 로드되는지 확인한다.

## Phase 2: Foundational

- [X] T002 `Assets/InputSystem_Actions.inputactions`에서 `Player/Move`와 WASD·게임패드 왼쪽 스틱 바인딩을 확인하고, 기존 액션을 씬에서 참조할 수 있도록 준비한다.

## Phase 3: User Story 1 - 임시 캐릭터 움직이기 (P1, MVP)

**목표** 빈 공간에서 임시 캐릭터를 움직이고, 카메라가 그 캐릭터를 따라간다.

**독립 검증** 전용 씬을 실행해 시작 시 정지, 두 방향 이동, 입력 해제 후 정지, 멈춘 자리에서 재이동, 카메라 추적을 확인한다.

- [X] T003 [P] [US1] `Assets/_SoloRPG/Scenes/MovementPrototype.unity`에 20×20 바닥과 이동을 알아볼 표식, 중앙의 임시 캐릭터와 CharacterController, 조명, 카메라를 배치한다. 상태 모델의 [테스트 공간.이동 가능 영역](data-model.md)과 [임시 캐릭터.시작 위치](data-model.md)를 따른다.
- [X] T004 [P] [US1] `Assets/_SoloRPG/Scripts/Prototype/PrototypeCharacterMovement.cs`에서 기존 Move 입력을 읽어 XZ 평면의 이동·방향 변경·정지·재이동과 바닥 경계를 처리한다. 이동량은 프레임 시간과 초당 4 Unity 단위, 최대 크기 1로 제한한 입력 벡터로 계산한다. 상태 모델의 [임시 캐릭터.이동 입력](data-model.md), [임시 캐릭터.이동 속도](data-model.md), [임시 캐릭터.현재 위치](data-model.md), [테스트 공간.이동 가능 영역](data-model.md)을 따른다.
- [X] T005 [P] [US1] `Assets/_SoloRPG/Scripts/Prototype/PrototypeFollowCamera.cs`에서 시작 시 카메라를 대상 기준 (0, 5, -7)에 배치하고 대상을 향해 아래로 기울인다. 처음 정한 각도를 유지하고, 매 프레임 이동 처리 이후 같은 상대 위치로 카메라를 갱신한다. 상태 모델의 [추적 카메라.대상](data-model.md)과 [추적 카메라.상대 위치와 각도](data-model.md)를 따른다.
- [X] T006 [US1] `Assets/_SoloRPG/Scenes/MovementPrototype.unity`에 두 컴포넌트를 연결하고 Move 액션, CharacterController, 바닥 경계, 카메라 추적 대상을 지정한다. Play Mode에 진입하면 바닥과 캐릭터가 보이는지 확인한다.

## Phase 4: 최종 검증

- [X] T007 `specs/001-prototype-character-movement/quickstart.md`에 따라 Unity Play Mode에서 키보드·게임패드 이동, 방향 변경, 정지·재이동, 바닥 경계, 카메라 추적, 재실행 시 시작 위치 복원을 확인한다.

## Dependencies & Execution Order

`T001 → T002 → (T003, T004, T005) → T006 → T007`

US1은 선행 사용자 스토리 없이 구현할 수 있다. T003은 씬을 편집하고 T004와 T005는 서로 다른 스크립트를 편집하므로 세 작업을 병렬로 진행할 수 있다. 씬에 참조를 연결하는 T006은 세 작업이 모두 끝난 뒤 진행한다.

## Implementation Strategy

준비 작업을 마친 뒤 US1만 구현한다. 전용 씬에서 조작과 카메라 추적을 확인하면 이동 프로토타입의 MVP가 완성된다.
