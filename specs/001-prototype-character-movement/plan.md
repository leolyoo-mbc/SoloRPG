# Implementation Plan: 임시 플레이어 캐릭터 이동

**브랜치** `codex/rpg` · **날짜** 2026-11-25 · [기능 명세](spec.md)

## Summary

기존 이동 씬을 등 뒤 가까이에서 보는 3인칭 시점으로 바꾼다. 마우스나 오른쪽 스틱으로 시선을 돌리고, 화면이 바라보는 수평 방향을 기준으로 이동한다.

## Technical Context

- C#과 Unity 6000.3.26f1을 사용한다. 기존 URP 17.3.0과 Input System 1.20.1의 `Player/Move`, `Player/Look`을 재사용한다.
- Windows Unity Editor의 전용 씬 하나에서 키보드·마우스와 게임패드를 Play Mode로 검증한다. 카메라 각도와 캐릭터 위치는 씬 실행 중에만 유지한다.

## Constitution Check

헌법 파일은 미작성 템플릿이다. 설계 전후 검토에서 적용할 프로젝트 고유 제약이나 위반을 확인하지 않았다.

## Project Structure

- `Assets/_SoloRPG/Scenes/MovementPrototype.unity`에서 기존 바닥과 경계를 사용한다. 캡슐 등 뒤에 방향을 알아볼 수 있는 작은 시각 표식을 자식으로 두고 콜라이더는 추가하지 않는다.
- `Assets/_SoloRPG/Scripts/Prototype/PrototypeFollowCamera.cs`가 시선 입력과 카메라 각도를 관리하며, 수평 회전을 이동 컴포넌트에 제공한다.
- `Assets/_SoloRPG/Scripts/Prototype/PrototypeCharacterMovement.cs`가 카메라의 수평 회전을 기준으로 이동 벡터와 캐릭터의 방향을 계산한다.

## Design

- 카메라는 캡슐 중심에서 0.65 Unity 단위 위를 기준점으로 잡는다. 현재 시선 회전의 로컬 좌표에서 기준점 오른쪽으로 0.55, 뒤쪽으로 3.0 떨어진 곳에 카메라를 두고 카메라에도 같은 회전을 적용한다. 뒤쪽 거리는 Inspector에서 조절한다.
- 시작할 때 캐릭터의 씬 방향을 수평 각도로 삼고 아래로 12° 기울인다. 위로 10°부터 아래로 25°까지 제한한다. 마우스는 0.12°/픽셀, 오른쪽 스틱은 최대 120°/초를 기본값으로 사용한다.
- 시선 입력 처리를 이동보다 먼저 실행하도록 실행 순서를 지정한다. 이동 컴포넌트는 제자리에서도 캐릭터가 시선의 수평 방향을 향하게 한다. 이동 입력은 길이를 최대 1로 제한한 뒤 해당 방향으로 회전시키고 초당 4 Unity 단위와 프레임 시간을 적용한다. 기존 CharacterController와 캐릭터 전체에 적용하는 바닥 경계를 재사용한다. 카메라 위치는 이동이 끝난 뒤 `LateUpdate`에서 갱신한다.
- 마우스 시선은 Game 뷰를 클릭해 커서를 잠근 동안 사용한다. Esc, 포커스 상실, 컴포넌트 비활성화 시 잠금을 해제한다. 게임패드 시선은 커서 잠금 여부와 독립적으로 동작한다.

[설계 결정](research.md) · [상태 모델](data-model.md) · [조작 계약](contracts/movement-controls.md) · [Play Mode 검증](quickstart.md)
